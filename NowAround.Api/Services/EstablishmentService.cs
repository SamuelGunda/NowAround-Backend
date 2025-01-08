using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Mapbox.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Services;

public class EstablishmentService : IEstablishmentService
{
    private readonly IAuth0Service _auth0Service;
    private readonly IMapboxService _mapboxService;
    private readonly IEstablishmentRepository _establishmentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IStorageService _storageService;
    private readonly ILogger<EstablishmentService> _logger;

    public EstablishmentService(
        IAuth0Service auth0Service, 
        IMapboxService mapboxService,
        IEstablishmentRepository establishmentRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IStorageService storageService,
        ILogger<EstablishmentService> logger)
    {
        _auth0Service = auth0Service;
        _mapboxService = mapboxService;
        _establishmentRepository = establishmentRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _storageService = storageService;
        _logger = logger;
    }
    
    public async Task RegisterEstablishmentAsync(EstablishmentRegisterRequest request)
    {
        request.ValidateProperties();
        
        var establishmentInfo = request.EstablishmentInfo;
        var personalInfo = request.OwnerInfo;
        
        if (await _establishmentRepository.CheckIfExistsByPropertyAsync("Name", establishmentInfo.Name))
        {
            _logger.LogWarning("An establishment with the name {Name} already exists.", establishmentInfo.Name);
            throw new EstablishmentAlreadyExistsException(establishmentInfo.Name);
        }
        
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(establishmentInfo.Address, establishmentInfo.PostalCode, establishmentInfo.City);
        
        // Check if categories and tags exist by their name and get them from the database
        var catsAndTags = await GetCategoriesAndTagsAsync(establishmentInfo.Category, establishmentInfo.Tags);
        
        var auth0Id = await _auth0Service.RegisterEstablishmentAccountAsync(personalInfo);

        var establishmentEntity = new Establishment
        {
            Auth0Id = auth0Id,
            Name = establishmentInfo.Name,
            Latitude = coordinates.lat,
            Longitude = coordinates.lng,
            Address = $"{establishmentInfo.Address}, {establishmentInfo.PostalCode}",
            City = establishmentInfo.City,
            PriceCategory = (PriceCategory) establishmentInfo.PriceCategory,
            Categories = catsAndTags.categories,
            Tags = catsAndTags.tags
        };
            
        try
        {
            await _establishmentRepository.CreateAsync(establishmentEntity);
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to create establishment in the database");
            
            await _auth0Service.DeleteAccountAsync(auth0Id);
            
            throw new Exception("Failed to create establishment in the database");
        }
    }

    /*
    public Task<bool> CheckIfEstablishmentExistsAsync(string auth0Id)
    {
        return _establishmentRepository.CheckIfExistsByPropertyAsync("Auth0Id", auth0Id);
    }
    */

    /*
    public async Task<EstablishmentResponse> GetEstablishmentByIdAsync(int id)
    {
        var establishment = await _establishmentRepository.GetByIdAsync(id);
        if (establishment == null)
        {
            throw new EntityNotFoundException("Establishment","ID", id.ToString());
        }

        return establishment.ToDetailedResponse();
    }
    */
    
    public async Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id, bool tracked = false)
    {
        return await _establishmentRepository.GetAsync(e => e.Auth0Id == auth0Id, tracked);
    }
    
    public async Task<EstablishmentProfileResponse> GetEstablishmentProfileByAuth0IdAsync(string auth0Id)
    {
        var establishment = await _establishmentRepository.GetProfileByAuth0IdAsync(auth0Id);
        if (establishment == null)
        {
            throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
        }

        return establishment;
    }

    public async Task<List<PendingEstablishmentResponse>> GetPendingEstablishmentsAsync()
    {
        var establishments = await _establishmentRepository.GetAllWhereRegisterStatusPendingAsync();
        
        var pendingEstablishments = 
            establishments.Select(e => new PendingEstablishmentResponse
                (e.Auth0Id, e.Name, _auth0Service.GetEstablishmentOwnerFullNameAsync(e.Auth0Id).Result)).ToList();
        
        return pendingEstablishments;
    }

    public async Task<List<EstablishmentDto>> GetEstablishmentsWithFilterAsync(SearchValues searchValues, int page)
    {
        searchValues.ValidateProperties();
        page = page >= 0 ? page : throw new InvalidSearchActionException("Page must be greater than 0");
        
        var establishmentDtos = await _establishmentRepository.GetRangeWithFilterAsync(searchValues, page);
        
        return establishmentDtos;
    }

    public async Task<int> GetEstablishmentsCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd)
    {
        return await _establishmentRepository.GetCountByCreatedAtBetweenDatesAsync(monthStart, monthEnd);
    }

    public async Task UpdateEstablishmentAsync(string auth0Id, EstablishmentUpdateRequest request)
    {
        var catsAndTags = await GetCategoriesAndTagsAsync(request.Categories, request.Tags);
        
        var establishmentDto = new EstablishmentDto
        {
            Auth0Id = auth0Id,
            Name = request.Name,
            Description = request.Description,
            PriceCategory = request.PriceCategory.HasValue ? (PriceCategory)request.PriceCategory.Value : null,
            Categories = catsAndTags.categories,
            Tags = catsAndTags.tags
        };
        
        await _establishmentRepository.UpdateAsync(establishmentDto);
    }

    public async Task UpdateEstablishmentPictureAsync(string auth0Id, string pictureContext, IFormFile picture)
    {
        if (pictureContext != "profile-picture" && pictureContext != "background-picture")
        {
            _logger.LogWarning("Invalid image context: {ImageContext}", pictureContext);
            throw new ArgumentException("Invalid image context", nameof(pictureContext));
        }
        
        var pictureType = picture.ContentType;
        _storageService.CheckPictureType(pictureType);
        
        var establishment = await GetEstablishmentByAuth0IdAsync(auth0Id, true);
        var pictureUrl = await _storageService.UploadPictureAsync(picture, "Establishment", auth0Id, pictureContext, null);
        
        establishment.ProfilePictureUrl = pictureUrl.Contains("profile-picture") ? pictureUrl : establishment.ProfilePictureUrl;
        establishment.BackgroundPictureUrl = pictureUrl.Contains("background-picture") ? pictureUrl : establishment.BackgroundPictureUrl;
        
        await _establishmentRepository.UpdateAsync(establishment);
    }

    public async Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus)
    {
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        var establishmentDto = new EstablishmentDto
        {
            Auth0Id = auth0Id,
            RequestStatus = requestStatus
        };
        
        await _establishmentRepository.UpdateAsync(establishmentDto);
    }

    public async Task DeleteEstablishmentAsync(string auth0Id)
    {
        await _auth0Service.DeleteAccountAsync(auth0Id);
        
        await _storageService.DeleteAccountFolderAsync("Establishment", auth0Id);
        
        //TODO: Get whole establishment object and delete it
        var result = await _establishmentRepository.DeleteByAuth0IdAsync(auth0Id);
        if (!result)
        {
            _logger.LogError("Failed to delete establishment with Auth0 ID: {Auth0Id}", auth0Id);
            throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
        }
    }

    private async Task<(Category[] categories, Tag[] tags)> GetCategoriesAndTagsAsync(ICollection<string>? categoryNames, ICollection<string>? tagNames)
    {
        List<Category> categories = [];
        List<Tag> tags = [];

        if (categoryNames != null)
        {
            foreach (var categoryName in categoryNames)
            {
                var categoryEntity = await _categoryRepository.GetByPropertyAsync("Name", categoryName);

                if (categoryEntity == null)
                {
                    _logger.LogWarning("Category {CategoryName} not found", categoryName);
                    throw new ArgumentException($"Category {categoryName} not found", nameof(Category));
                }

                categories.Add(categoryEntity);
            }
        }

        if (tagNames != null)
        {
            foreach (var tag in tagNames)
            {
                var tagEntity = await _tagRepository.GetByPropertyAsync("Name", tag);

                if (tagEntity == null)
                {
                    _logger.LogWarning("Tag {TagName} not found", tag);
                    throw new ArgumentException($"Tag {tag} not found", nameof(Tag));
                }
                
                tags.Add(tagEntity);
            }
        }
        return (categories.ToArray(), tags.ToArray());
    }
    
    // Menu methods
    
    public async Task AddMenuAsync(string auth0Id, MenuCreateRequest menu)
    {
        var establishment = await _establishmentRepository.GetAsync
        (
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query
                .Include(e => e.Menus)
        );
        
        var menuEntity = new Menu
        {
            Name = menu.Name,
            EstablishmentId = establishment.Id,
            MenuItems = menu.MenuItems.Select(mi => new MenuItem
            {
                Name = mi.Name,
                Description = mi.Description,
                Price = mi.Price
            }).ToList()
        };
        
        establishment.Menus.Add(menuEntity);
        
        await _establishmentRepository.UpdateAsync(establishment);
    }

    public async Task AddMenuItemAsync(string auth0Id, int menuId, ICollection<MenuItemCreateRequest> menuItem)
    {
        var establishment = await _establishmentRepository.GetAsync
        (
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query
                .Include(e => e.Menus)
                .ThenInclude(m => m.MenuItems)
        );
        
        var menu = establishment.Menus.FirstOrDefault(m => m.Id == menuId);
        
        if (menu == null)
        {
            _logger.LogWarning("Menu with ID {MenuId} not found", menuId);
            throw new EntityNotFoundException("Menu", "ID", menuId.ToString());
        }
        
        var menuItemEntities = menuItem.Select(mi => new MenuItem
        {
            Name = mi.Name,
            Description = mi.Description,
            Price = mi.Price
        }).ToList();

        foreach (var menuItemEntity in menuItemEntities)
        {
            menu.MenuItems.Add(menuItemEntity);
        }
        
        menu.UpdatedAt = DateTime.Now;
        
        await _establishmentRepository.UpdateAsync(establishment);
    }

    public async Task DeleteMenuAsync(string auth0Id, int menuId)
    {
        var establishment = await _establishmentRepository.GetAsync
        (
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query
                .Include(e => e.Menus)
        );

        if (establishment.Menus.All(m => m.Id != menuId))
        {
            _logger.LogWarning("Menu with ID {MenuId} not found", menuId);
            throw new EntityNotFoundException("Menu", "ID", menuId.ToString());
        }
        
        //TODO: Add picture deletion
        
        establishment.Menus = establishment.Menus.Where(m => m.Id != menuId).ToList();
        
        await _establishmentRepository.UpdateAsync(establishment);
    }

    public async Task DeleteMenuItemAsync(string auth0Id, int menuItemId)
    {
        var establishment = await _establishmentRepository.GetAsync
        (
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query
                .Include(e => e.Menus)
                .ThenInclude(m => m.MenuItems)
        );
        
        var menu = establishment.Menus.FirstOrDefault(m => m.MenuItems.Any(mi => mi.Id == menuItemId));
        
        if (menu == null)
        {
            _logger.LogWarning("Menu item with ID {MenuItemId} not found", menuItemId);
            throw new EntityNotFoundException("Menu item", "ID", menuItemId.ToString());
        }
        
        //TODO: Add picture deletion
        
        menu.MenuItems = menu.MenuItems.Where(mi => mi.Id != menuItemId).ToList();
        
        menu.UpdatedAt = DateTime.Now;
        
        await _establishmentRepository.UpdateAsync(establishment);
    }
}