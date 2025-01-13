﻿using Microsoft.EntityFrameworkCore;
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
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IBaseRepository<Tag> _tagRepository;
    private readonly IStorageService _storageService;
    private readonly ILogger<EstablishmentService> _logger;

    public EstablishmentService(
        IAuth0Service auth0Service, 
        IMapboxService mapboxService,
        IEstablishmentRepository establishmentRepository,
        IBaseRepository<Category> categoryRepository,
        IBaseRepository<Tag> tagRepository,
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
        
        if (await _establishmentRepository.CheckIfExistsAsync("Name", establishmentInfo.Name))
        {
            _logger.LogWarning("An establishment with the name {Name} already exists.", establishmentInfo.Name);
            throw new EntityAlreadyExistsException("Establishment", "Name", establishmentInfo.Name);
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
    
    public async Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id, bool tracked = false, params Func<IQueryable<Establishment>, IQueryable<Establishment>>[] includeProperties)
    {
        return await _establishmentRepository.GetAsync(e => e.Auth0Id == auth0Id, tracked, includeProperties);
    }
    
    public async Task<EstablishmentProfileResponse> GetEstablishmentProfileByAuth0IdAsync(string auth0Id)
    {
        var establishment = await _establishmentRepository.GetProfileByAuth0IdAsync(auth0Id);

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

    public async Task<List<EstablishmentMarkerResponse>> GetEstablishmentsWithFilterAsync(SearchValues searchValues, int page)
    {
        searchValues.ValidateProperties();
        page = page >= 0 ? page : throw new InvalidSearchActionException("Page must be greater than 0");
        
        var establishmentMarkers = await _establishmentRepository.GetRangeWithFilterAsync(searchValues, page);
        
        return establishmentMarkers;
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
        
        establishment.Name = request.Name ?? establishment.Name;
        establishment.Description = request.Description ?? establishment.Description;
        establishment.PriceCategory = request.PriceCategory.HasValue ? (PriceCategory) request.PriceCategory.Value : establishment.PriceCategory;
        establishment.Categories = catsAndTags.categories.Length > 0 ? catsAndTags.categories.ToList() : establishment.Categories.ToList();
        establishment.Tags = catsAndTags.tags.Length > 0 ? catsAndTags.tags.ToList() : establishment.Tags.ToList();
        
        await _establishmentRepository.UpdateAsync(establishment);
        
        var genericInfo = new GenericInfo(
            establishment.Name,
            establishment.ProfilePictureUrl,
            establishment.BackgroundPictureUrl,
            establishment.Description,
            establishment.PriceCategory.ToString(),
            establishment.Categories.Select(c => c.Name).ToList(),
            establishment.Tags.Select(t => t.Name).ToList(),
            establishment.SocialLinks.Select(sl => new SocialLinkDto(sl.Name, sl.Url)).ToList()
        );
        
        return genericInfo;
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
        
        await _establishmentRepository.UpdateAsync(establishment);
    }
    
    public async Task UpdateRatingStatisticsAsync(string auth0Id, int rating)
    {
        var establishment = await _establishmentRepository.GetAsync(
            e => e.Auth0Id == auth0Id, 
            true, 
            query => query.Include(e => e.RatingStatistic));

        if (establishment.RatingStatistic == null)
        {
            _logger.LogWarning("RatingStatistic not found for the establishment with Auth0 ID: {Auth0Id}", auth0Id);
            throw new Exception("RatingStatistic not found for the establishment.");
        }

        var ratingMap = new Dictionary<int, Action>
        {
            { 1, () => establishment.RatingStatistic.OneStar++ },
            { 2, () => establishment.RatingStatistic.TwoStars++ },
            { 3, () => establishment.RatingStatistic.ThreeStars++ },
            { 4, () => establishment.RatingStatistic.FourStars++ },
            { 5, () => establishment.RatingStatistic.FiveStars++ }
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
                var categoryEntity = await _categoryRepository.GetAsync(c => c.Name == categoryName);

                categories.Add(categoryEntity);
            }
        }

        if (tagNames != null)
        {
            foreach (var tag in tagNames)
            {
                var tagEntity = await _tagRepository.GetAsync(t => t.Name == tag);
                
                tags.Add(tagEntity);
            }
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