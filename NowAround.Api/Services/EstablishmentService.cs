using System.Runtime.InteropServices.JavaScript;
using Microsoft.IdentityModel.Tokens;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Mapbox.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Interfaces;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;

// ReSharper disable InvertIf

namespace NowAround.Api.Services;

public class EstablishmentService : IEstablishmentService
{
    
    private readonly IAuth0Service _auth0Service;
    private readonly IMapboxService _mapboxService;
    private readonly IEstablishmentRepository _establishmentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<EstablishmentService> _logger;

    public EstablishmentService(
        IAuth0Service auth0Service, 
        IMapboxService mapboxService,
        IEstablishmentRepository establishmentRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        ILogger<EstablishmentService> logger)
    {
        _auth0Service = auth0Service;
        _mapboxService = mapboxService;
        _establishmentRepository = establishmentRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
        _logger = logger;
    }
    
    /// <summary>
    /// Registers a new establishment.
    /// Establishment is registered on Auth0 and saved to the database on success.
    /// If database operation fails, the establishment account gets deleted from Auth0.
    /// </summary>
    /// <param name="request"> The establishment register request </param>
    /// <returns> A task that represents the asynchronous operation </returns>
    /// <exception cref="EstablishmentAlreadyExistsException"> If establishment with the same name already exists </exception>
    /// <exception cref="InvalidCategoryException"> If no categories are found </exception>
    /// <exception cref="Exception"> If establishment creation in the database fails </exception>
    public async Task RegisterEstablishmentAsync(EstablishmentRegisterRequest request)
    {
        request.ValidateProperties();
        
        var establishmentInfo = request.EstablishmentInfo;
        var personalInfo = request.PersonalInfo;
        
        // Check if establishment with this name already exists
        if (await _establishmentRepository.CheckIfEstablishmentExistsByNameAsync(establishmentInfo.Name))
        {
            _logger.LogWarning("An establishment with the name {Name} already exists.", establishmentInfo.Name);
            throw new EstablishmentAlreadyExistsException(establishmentInfo.Name);
        }
        
        // Get coordinates from address using Mapbox API and set them to a variable
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(establishmentInfo.Address, establishmentInfo.PostalCode, establishmentInfo.City);
        
        // Check if categories and tags exist by their name and set them to a variable
        var catsAndTags = await SetCategoriesAndTagsAsync(establishmentInfo.Category, establishmentInfo.Tags);
        if (catsAndTags.categories.Length == 0)
        {
            _logger.LogWarning("No categories found");
            throw new InvalidCategoryException("At least one category is required");
        }
        
        // Register establishment on Auth0
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
            EstablishmentCategories = catsAndTags.categories.Select(c => new EstablishmentCategory { Category = c }).ToList(),
            EstablishmentTags = catsAndTags.tags.Select(t => new EstablishmentTag { Tag = t }).ToList()
        };
            
        try
        {
            // Save establishment to the database
            await _establishmentRepository.CreateEstablishmentAsync(establishmentEntity);
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to create establishment in the database");
            await _auth0Service.DeleteAccountAsync(auth0Id);
            throw new Exception("Failed to create establishment in the database");
        }
    }
    
    /// <summary>
    /// Gets an establishment by its ID.
    /// </summary>
    /// <param name="id"> The ID of the establishment </param>
    /// <returns> Establishment detailed response </returns>
    /// <exception cref="EstablishmentNotFoundException"> If establishment with the specified ID is not found </exception>
    public async Task<EstablishmentResponse> GetEstablishmentByIdAsync(int id)
    {
        var establishment = await _establishmentRepository.GetEstablishmentByIdAsync(id);
        if (establishment == null)
        {
            _logger.LogWarning("Establishment with ID {id} not found", id);
            throw new EstablishmentNotFoundException($"ID: {id}");
        }

        return establishment.ToDetailedResponse();
    }

    /// <summary>
    /// Gets an establishment by its Auth0 ID.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment </param>
    /// <returns> Establishment detailed response </returns>
    /// <exception cref="EstablishmentNotFoundException"> If establishment with the specified Auth0 ID is not found </exception>
    public async Task<EstablishmentResponse> GetEstablishmentByAuth0IdAsync(string auth0Id)
    {
        var establishment = await _establishmentRepository.GetEstablishmentByAuth0IdAsync(auth0Id);
        if (establishment == null)
        {
            _logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
            throw new EstablishmentNotFoundException($"Auth0ID: {auth0Id}");
        }

        return establishment.ToDetailedResponse();
    }
    
    /// <summary>
    /// Gets a list of establishments with pending register status.
    /// </summary>
    /// <returns> List of pending establishments </returns>
    public async Task<List<PendingEstablishmentResponse>?> GetPendingEstablishmentsAsync()
    {
        var establishments = await _establishmentRepository.GetEstablishmentsWithPendingRegisterStatusAsync();
        
        var pendingEstablishments = establishments?.Select(e => new PendingEstablishmentResponse
        {
            Auth0Id = e.Auth0Id,
            Name = e.Name,
            OwnerName = _auth0Service.GetEstablishmentOwnerFullNameAsync(e.Auth0Id).Result
        }).ToList();
        
        return pendingEstablishments;
    }

    /// <summary>
    /// Gets a list of establishment markers with applied filter.
    /// </summary>
    /// <param name="name"> The name to be set </param>
    /// <param name="priceCategory"> The price category to be set </param>
    /// <param name="categoryName"> The category name to be set </param>
    /// <param name="tagNames"> The list of tag names to be set </param>
    /// <returns> List of establishment markers </returns>
    public async Task<List<EstablishmentResponse>?> GetEstablishmentMarkersWithFilterAsync(string? name, int? priceCategory, string? categoryName, List<string>? tagNames)
    {
        var establishments = await _establishmentRepository.GetEstablishmentsWithFilterAsync(name, priceCategory, categoryName, tagNames);
        
        return establishments?.Select(e => e.ToMarker()).ToList();
    }
    
    /// <summary>
    /// Gets a list of establishment markers with applied filter in the specified area.
    /// </summary>
    /// <param name="mapBounds"> The map bounds to be set </param>
    /// <param name="name"> The name to be set </param>
    /// <param name="priceCategory"> The price category to be set </param>
    /// <param name="categoryName"> The category name to be set </param>
    /// <param name="tagNames"> The list of tag names to be set </param>
    /// <returns> List of establishment markers </returns>
    public async Task<List<EstablishmentResponse>?> GetEstablishmentMarkersWithFilterInAreaAsync(
        MapBounds mapBounds, string? name, int? priceCategory, string? categoryName, List<string>? tagNames)
    {
        
        var establishments = await _establishmentRepository.GetEstablishmentsWithFilterInAreaAsync(
            mapBounds.NwLat, mapBounds.NwLong,
            mapBounds.SeLat, mapBounds.SeLong,
            name, priceCategory, categoryName, tagNames);
        
        return establishments?.Select(e => e.ToMarker()).ToList();
    }

    /// <summary>
    /// Gets the count of establishments created within a specified date range.
    /// </summary>
    /// <param name="monthStart"> The start date of the month </param>
    /// <param name="monthEnd"> The end date of the month </param>
    /// <returns> The count of establishments created within the specified date range </returns>
    public async Task<int> GetEstablishmentsCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd)
    {
        return await _establishmentRepository.GetEstablishmentsCountByCreatedAtBetweenDatesAsync(monthStart, monthEnd);
    }
    
    /// <summary>
    /// Updates an establishment.
    /// </summary>
    /// <param name="request"> The establishment update request </param>
    /// <exception cref="ArgumentNullException"> If Auth0 ID is null </exception>
    /// <exception cref="EstablishmentNotFoundException"> If establishment with the specified Auth0 ID is not found </exception>
    public async Task UpdateEstablishmentAsync(EstablishmentUpdateRequest request)
    {
        var auth0Id = request.Auth0Id;
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        var catsAndTags = await SetCategoriesAndTagsAsync(request.Category, request.Tags);
        
        var establishmentDto = new EstablishmentDto
        {
            Name = request.Name,
            Description = request.Description,
            PriceCategory = request.PriceCategory.HasValue ? (PriceCategory)request.PriceCategory.Value : null,
            EstablishmentCategories = catsAndTags.categories.Select(c => new EstablishmentCategory { Category = c }).ToList(),
            EstablishmentTags = catsAndTags.tags.Select(t => new EstablishmentTag { Tag = t }).ToList()
        };
        
        var result = await _establishmentRepository.UpdateEstablishmentByAuth0IdAsync(auth0Id, establishmentDto);
        if (!result)
        {
            _logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
            throw new EstablishmentNotFoundException($"Auth0 ID: {auth0Id}");
        }
    }
    
    /// <summary>
    /// Updates the register request status of an establishment.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment </param>
    /// <param name="requestStatus"> The status to be set </param>
    /// <exception cref="ArgumentNullException"> If Auth0 ID is null </exception>
    /// <exception cref="EstablishmentNotFoundException"> If establishment with the specified Auth0 ID is not found </exception>
    public async Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus)
    {
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        var establishmentDto = new EstablishmentDto
        {
            RequestStatus = requestStatus
        };
        
        var result = await _establishmentRepository.UpdateEstablishmentByAuth0IdAsync(auth0Id, establishmentDto);
        if (!result)
        {
            _logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
            throw new EstablishmentNotFoundException($"Auth0 ID: {auth0Id}");
        }
    }

    /// <summary>
    /// Deletes an establishment.
    /// </summary>
    /// <param name="auth0Id"> The Auth0 ID of the establishment to delete </param>
    /// <exception cref="ArgumentNullException"> If Auth0 ID is null </exception>
    /// <exception cref="EstablishmentNotFoundException"> If establishment with the specified Auth0 ID is not found </exception>
    public async Task DeleteEstablishmentAsync(string auth0Id)
    {
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        // Delete establishment account from Auth0
        await _auth0Service.DeleteAccountAsync(auth0Id);
        
        // Delete establishment from the database
        var result = await _establishmentRepository.DeleteEstablishmentByAuth0IdAsync(auth0Id);
        if (!result)
        {
            throw new EstablishmentNotFoundException($"Auth0 ID: {auth0Id}");
        }
    }
    
    /// <summary>
    /// Sets categories and tags for the establishment.
    /// If category name or tag name is not found, an exception is thrown.
    /// Categories are fetched from the database.
    /// Tags are fetched from the categories list,
    /// if they don't belong to any category (they have null Category in DB),
    /// they are fetched from the database.
    /// </summary>
    /// <param name="categoryNames"> The list of category names to be set </param>
    /// <param name="tagNames"> The list of tag names to be set </param>
    /// <returns> Tuple of categories and tags </returns>
    /// <exception cref="InvalidCategoryException"> If category name is not found </exception>
    /// <exception cref="InvalidTagException"> If tag name is not found </exception>
    private async Task<(Category[] categories, Tag[] tags)> SetCategoriesAndTagsAsync(ICollection<string>? categoryNames, ICollection<string>? tagNames)
    {
        List<Category> categories = [];
        List<Tag> tags = [];

        if (categoryNames != null)
        {
            foreach (var categoryName in categoryNames)
            {
                // Get category from database by name, including tags
                var categoryEntity = await _categoryRepository.GetCategoryByNameWithTagsAsync(categoryName);

                if (categoryEntity == null)
                {
                    _logger.LogWarning("Category {CategoryName} not found", categoryName);
                    throw new InvalidCategoryException();
                }

                categories.Add(categoryEntity);
            }
        }

        if (tagNames != null)
        {
            foreach (var tag in tagNames)
            {
                // Check if tag belongs to any of the categories, if not, get it from the database
                var tagEntity = categories
                                    .SelectMany(c => c.Tags)
                                    .FirstOrDefault(t => t.Name == tag) 
                                ?? await _tagRepository.GetTagByNameAsync(tag);

                if (tagEntity == null)
                {
                    _logger.LogWarning("Tag {TagName} not found", tag);
                    throw new InvalidTagException();
                }
                
                tags.Add(tagEntity);
            }
        }
        
        return (categories.ToArray(), tags.ToArray());
    }
}