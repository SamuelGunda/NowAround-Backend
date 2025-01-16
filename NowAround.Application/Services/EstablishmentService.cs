using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NowAround.Application.Common.Exceptions;
using NowAround.Application.Dtos;
using NowAround.Application.Interfaces;
using NowAround.Application.Mapping;
using NowAround.Application.Requests;
using NowAround.Application.Responses;
using NowAround.Domain.Enum;
using NowAround.Domain.Interfaces.Base;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;
using NowAround.Application.Common.Helpers;

namespace NowAround.Application.Services;

public class EstablishmentService : IEstablishmentService
{
    private readonly IAuth0Service _auth0Service;
    private readonly IMapboxService _mapboxService;
    private readonly IEstablishmentRepository _establishmentRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IBaseRepository<Tag> _tagRepository;
    private readonly IStorageService _storageService;
    private readonly IMailService _mailService;
    private readonly ILogger<EstablishmentService> _logger;

    public EstablishmentService(
        IAuth0Service auth0Service, 
        IMapboxService mapboxService,
        IEstablishmentRepository establishmentRepository,
        IBaseRepository<Category> categoryRepository,
        IBaseRepository<Tag> tagRepository,
        IStorageService storageService,
        IMailService mailService,
        ILogger<EstablishmentService> logger)
    {
        _auth0Service = auth0Service;
        _mapboxService = mapboxService;
        _establishmentRepository = establishmentRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _storageService = storageService;
        _mailService = mailService;
        _logger = logger;
    }
    
    public async Task RegisterEstablishmentAsync(EstablishmentRegisterRequest request)
    {
        var establishmentInfo = request.EstablishmentInfo;
        var personalInfo = request.EstablishmentOwnerInfo;
        
        if (await _establishmentRepository.CheckIfExistsAsync("Name", establishmentInfo.Name))
        {
            _logger.LogWarning("An establishment with the name {Name} already exists.", establishmentInfo.Name);
            throw new EntityAlreadyExistsException("Establishment", "Name", establishmentInfo.Name);
        }
        
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(establishmentInfo.Address, establishmentInfo.PostalCode, establishmentInfo.City);
        
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
            /*
            await _mailService.SendWelcomeEmailAsync($"{personalInfo.FirstName} {personalInfo.LastName}", establishmentInfo.Name ,personalInfo.Email);
        */
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to create establishment in the database");
            
            await _auth0Service.DeleteAccountAsync(auth0Id);
            
            throw new Exception("Failed to create establishment in the database");
        }
    }
    
    public async Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id, bool tracked = false, params Func<IQueryable<Establishment>, IQueryable<Establishment>>[] includeProperties)
    {
        return await _establishmentRepository.GetAsync(e => e.Auth0Id == auth0Id, tracked, includeProperties);
    }
    
    public async Task<EstablishmentProfileResponse> GetEstablishmentProfileByAuth0IdAsync(string auth0Id)
    {
        var establishment = await _establishmentRepository.GetProfileByAuth0IdAsync(auth0Id);
        if (establishment == null)
        {
            throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
        }

        return establishment.ToProfileResponse();
    }

    public async Task<List<PendingEstablishmentResponse>> GetPendingEstablishmentsAsync()
    {
        var establishments = await _establishmentRepository.GetAllWhereRegisterStatusPendingAsync();
        
        var pendingEstablishments = 
            establishments.Select(e => new PendingEstablishmentResponse
                (e.Auth0Id, e.Name, _auth0Service.GetEstablishmentOwnerFullNameAndEmailAsync(e.Auth0Id).Result.fullName)).ToList();
        
        return pendingEstablishments;
    }

    public async Task<List<EstablishmentMarkerResponse>> GetEstablishmentsWithFilterAsync(SearchValues searchValues, int page)
    {
        if (!SearchValueValidator.Validate(searchValues))
        {
            _logger.LogWarning("Invalid search values");
            throw new InvalidSearchActionException("Invalid search values");
        }
        
        page = page >= 0 ? page : throw new InvalidSearchActionException("Page must be greater than 0");

        var queryBuilder = EstablishmentSearchQueryBuilder.BuildSearchQuery(searchValues);

        var establishments = await _establishmentRepository.GetRangeWithFilterAsync(queryBuilder, page);

        return establishments.Select(e => e.ToMarkerResponse()).ToList();
    }

    public async Task<int> GetEstablishmentsCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd)
    {
        return await _establishmentRepository.GetCountByCreatedAtBetweenDatesAsync(monthStart, monthEnd);
    }

    public async Task<GenericInfo> UpdateEstablishmentGenericInfoAsync(string auth0Id, EstablishmentGenericInfoUpdateRequest request)
    {
        var establishment = await _establishmentRepository.GetAsync
        (
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query
                .Include(e => e.Tags)
                .Include(e => e.Categories)
                .Include(e => e.SocialLinks)
        );
        
        var catsAndTags = await GetCategoriesAndTagsAsync(request.Categories, request.Tags);
        
        establishment.Name = request.Name;
        establishment.Description = request.Description;
        establishment.PriceCategory = (PriceCategory)request.PriceCategory;
        establishment.Categories = catsAndTags.categories.ToList();
        establishment.Tags = catsAndTags.tags.ToList();
        
        await _establishmentRepository.UpdateAsync(establishment);
        
        var genericInfo = new GenericInfo(
            establishment.Name,
            establishment.ProfilePictureUrl,
            establishment.BackgroundPictureUrl,
            establishment.Description,
            establishment.PriceCategory.ToString(),
            establishment.Tags.Select(t => t.Name).ToList(),
            establishment.Categories.Select(c => c.Name).ToList(),
            establishment.SocialLinks.Select(sl => new SocialLinkDto(sl.Name, sl.Url)).ToList()
        );
        
        return genericInfo;
    }

    public async Task<LocationInfo> UpdateEstablishmentLocationInfoAsync(string auth0Id, EstablishmentLocationInfoUpdateRequest request)
    {
        var establishment = await _establishmentRepository.GetAsync(
            e => e.Auth0Id == auth0Id,
            true,
            query => query.Include(e => e.BusinessHours));

        var addressAndCity = await _mapboxService.GetAddressFromCoordinatesAsync(request.Lat, request.Long);
        establishment.Latitude = request.Lat;
        establishment.Longitude = request.Long;
        establishment.Address = addressAndCity.address;
        establishment.City = addressAndCity.city;

        var businessHours = request.BusinessHours;
        establishment.BusinessHours.Monday = businessHours.Monday;
        establishment.BusinessHours.Tuesday = businessHours.Tuesday;
        establishment.BusinessHours.Wednesday = businessHours.Wednesday;
        establishment.BusinessHours.Thursday = businessHours.Thursday;
        establishment.BusinessHours.Friday = businessHours.Friday;
        establishment.BusinessHours.Saturday = businessHours.Saturday;
        establishment.BusinessHours.Sunday = businessHours.Sunday;
        
        foreach (var exception in request.BusinessHours.BusinessHoursExceptions)
        {
            establishment.BusinessHours.BusinessHoursExceptions.Add(new BusinessHoursException
            {
                Date = exception.Date,
                Status = exception.Status
            });
        }
        
        await _establishmentRepository.UpdateAsync(establishment);
        
        return new LocationInfo(
            establishment.Address,
            establishment.City,
            establishment.Latitude,
            establishment.Longitude,
            new BusinessHoursDto(
                establishment.BusinessHours.Monday,
                establishment.BusinessHours.Tuesday,
                establishment.BusinessHours.Wednesday,
                establishment.BusinessHours.Thursday,
                establishment.BusinessHours.Friday,
                establishment.BusinessHours.Saturday,
                establishment.BusinessHours.Sunday,
                establishment.BusinessHours.BusinessHoursExceptions
                    .Select(e => new BusinessHoursExceptionsDto(e.Date, e.Status))
                    .ToList()
            )
        );
    }

    public async Task<string> UpdateEstablishmentPictureAsync(string auth0Id, string pictureContext, IFormFile picture)
    {
        if (pictureContext != "profile-picture" && pictureContext != "background-picture")
        {
            _logger.LogWarning("Invalid image context: {ImageContext}", pictureContext);
            throw new ArgumentException("Invalid image context", nameof(pictureContext));
        }
        
        var establishment = await _establishmentRepository.GetAsync(e => e.Auth0Id == auth0Id);
        var pictureUrl = await _storageService.UploadPictureAsync(picture, "Establishment", auth0Id, pictureContext);
        
        establishment.ProfilePictureUrl = pictureUrl.Contains("profile-picture") ? pictureUrl : establishment.ProfilePictureUrl;
        establishment.BackgroundPictureUrl = pictureUrl.Contains("background-picture") ? pictureUrl : establishment.BackgroundPictureUrl;
        
        await _establishmentRepository.UpdateAsync(establishment);
        
        return pictureUrl;
    }

    public async Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus)
    {
        var establishment = await _establishmentRepository.GetAsync(
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query.IgnoreQueryFilters());
        
        establishment.RequestStatus = requestStatus;
        
        if (requestStatus == RequestStatus.Accepted)
        {
            var password = PasswordUtils.Generate();
            var nameAndEmail = await _auth0Service.GetEstablishmentOwnerFullNameAndEmailAsync(auth0Id);
            
            await _auth0Service.ChangeAccountPasswordAsync(auth0Id, password);
            
            await _mailService.SendAccountAcceptedEmailAsync(nameAndEmail.fullName, establishment.Name, nameAndEmail.email, password);
        }
        
        await _establishmentRepository.UpdateAsync(establishment);
    }
    
    public async Task UpdateRatingStatisticsAsync(int id, int rating, bool increment = true)
    {
        var establishment = await _establishmentRepository.GetAsync(
            e => e.RatingStatistic.Id == id, 
            true, 
            query => query.Include(e => e.RatingStatistic));

        if (establishment.RatingStatistic == null)
        {
            _logger.LogWarning("RatingStatistic not found for the establishment with Auth0 ID: {establishment.Auth0Id}", establishment.Auth0Id);
            throw new Exception("RatingStatistic not found for the establishment.");
        }

        var ratingMap = new Dictionary<int, Action>
        {
            { 1, () => { if (increment) establishment.RatingStatistic.OneStar++; else establishment.RatingStatistic.OneStar--; } },
            { 2, () => { if (increment) establishment.RatingStatistic.TwoStars++; else establishment.RatingStatistic.TwoStars--; } },
            { 3, () => { if (increment) establishment.RatingStatistic.ThreeStars++; else establishment.RatingStatistic.ThreeStars--; } },
            { 4, () => { if (increment) establishment.RatingStatistic.FourStars++; else establishment.RatingStatistic.FourStars--; } },
            { 5, () => { if (increment) establishment.RatingStatistic.FiveStars++; else establishment.RatingStatistic.FiveStars--; } }
        };

        if (ratingMap.TryGetValue(rating, out var value))
        {
            value();
        }
        else
        {
            _logger.LogWarning("Invalid rating value: {Rating}", rating);
            throw new ArgumentException("Invalid rating value", nameof(rating));
        }

        await _establishmentRepository.UpdateAsync(establishment);
    }

    public async Task DeleteEstablishmentAsync(string auth0Id)
    {
        await _auth0Service.DeleteAccountAsync(auth0Id);
        
        await _storageService.DeleteAsync("Establishment", auth0Id);
        
        var result = await _establishmentRepository.DeleteByAuth0IdAsync(auth0Id);
        if (!result)
        {
            _logger.LogError("Failed to delete establishment with Auth0 ID: {Auth0Id}", auth0Id);
            throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
        }
    }

    private async Task<(Category[] categories, Tag[] tags)> GetCategoriesAndTagsAsync(ICollection<string> categoryNames, ICollection<string> tagNames)
    {
        List<Category> categories = [];
        List<Tag> tags = [];
        
        foreach (var categoryName in categoryNames)
        {
            var categoryEntity = await _categoryRepository.GetAsync(c => c.Name == categoryName);

            categories.Add(categoryEntity);
        }
        
        foreach (var tag in tagNames)
        {
            var tagEntity = await _tagRepository.GetAsync(t => t.Name == tag);
            
            tags.Add(tagEntity);
        }
        
        return (categories.ToArray(), tags.ToArray());
    }
    
    // Menu methods
    
    public async Task<MenuDto> AddMenuAsync(string auth0Id, MenuCreateRequest menu)
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
        
        return menuEntity.ToDto();
    }

    public async Task<MenuDto> UpdateMenuAsync(string auth0Id, MenuUpdateRequest updatedMenu)
    {
        var establishment = await _establishmentRepository.GetAsync
        (
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query
                .Include(e => e.Menus)
                .ThenInclude(m => m.MenuItems)
        );
        
        var menuId = updatedMenu.Id;
        
        var menu = establishment.Menus.FirstOrDefault(m => m.Id == menuId);
        
        if (menu == null)
        {
            _logger.LogWarning("Menu with ID {MenuId} not found", menuId);
            throw new EntityNotFoundException("Menu", "ID", menuId.ToString());
        }
        
        var updatedMenuItems = updatedMenu.MenuItems.ToList();
        var existingMenuItems = menu.MenuItems.ToList();
        
        foreach (var updatedMenuItem in updatedMenuItems)
        {
            if (updatedMenuItem.Id == null)
            {
                menu.MenuItems.Add(new MenuItem
                {
                    Name = updatedMenuItem.Name,
                    Description = updatedMenuItem.Description,
                    Price = updatedMenuItem.Price
                });
                continue;
            }
            
            var existingMenuItem = existingMenuItems.FirstOrDefault(mi => mi.Id == updatedMenuItem.Id);
            if (existingMenuItem == null)
            {
                _logger.LogWarning("Menu item with ID {MenuItemId} not found", updatedMenuItem.Id);
                throw new EntityNotFoundException("Menu item", "ID", updatedMenuItem.Id.ToString());
            }
            
            existingMenuItem.Name = updatedMenuItem.Name;
            existingMenuItem.Description = updatedMenuItem.Description;
            existingMenuItem.Price = updatedMenuItem.Price;
        }
        
        menu.UpdatedAt = DateTime.Now;
        
        await _establishmentRepository.UpdateAsync(establishment);
        
        return menu.ToDto();
    }

    public async Task<string> UpdateMenuItemPictureAsync(string auth0Id, int menuItemId, IFormFile picture)
    {
        var pictureType = picture.ContentType;
        _storageService.CheckPictureType(pictureType);
        
        var establishment = await _establishmentRepository.GetAsync
        ( 
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query
            .Include(e => e.Menus)
            .ThenInclude(m => m.MenuItems)
        );
        
        var menuItem = establishment.Menus.SelectMany(m => m.MenuItems).FirstOrDefault(mi => mi.Id == menuItemId);
        
        if (menuItem == null)
        {
            _logger.LogWarning("Menu item with ID {MenuItemId} not found", menuItemId);
            throw new EntityNotFoundException("Menu item", "ID", menuItemId.ToString());
        }
        
        var pictureUrl = await _storageService.UploadPictureAsync(picture, "Establishment", auth0Id, $"menu/{menuItem.Menu.Id}/{menuItemId}");
        
        menuItem.PictureUrl = pictureUrl;
        menuItem.Menu.UpdatedAt = DateTime.Now;
        
        await _establishmentRepository.UpdateAsync(establishment);
        
        return pictureUrl;
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
        
        await _storageService.DeleteAsync("Establishment", auth0Id, $"menu/{menuId}");
        
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
        
        await  _storageService.DeleteAsync("Establishment", auth0Id, $"menu/{menu.Id}/{menuItemId}");
        
        menu.MenuItems = menu.MenuItems.Where(mi => mi.Id != menuItemId).ToList();
        
        menu.UpdatedAt = DateTime.Now;
        
        await _establishmentRepository.UpdateAsync(establishment);
    }
}