using Microsoft.IdentityModel.Tokens;
using NowAround.Api.Apis.Auth0.Exceptions;
using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Models;
using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Apis.Mapbox.Interfaces;
using NowAround.Api.Exceptions;
using NowAround.Api.Interfaces;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Requests;

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
    /// Registers a new establishment asynchronously.
    /// Establishment Request is validated.
    /// Categories and tags are validated and fetched from the database.
    /// Coordinates are fetched from the address using Mapbox API.
    /// Establishment is registered on Auth0 and saved to the database on success.
    /// If database operation fails, the establishment account gets deleted from Auth0.
    /// </summary>
    
    public async Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest)
    {
        establishmentRequest.ValidateProperties();
        
        var establishmentInfo = establishmentRequest.EstablishmentInfo;
        var personalInfo = establishmentRequest.PersonalInfo;
        
        // Check if establishment with this name already exists
        if (await _establishmentRepository.CheckIfEstablishmentExistsByNameAsync(establishmentInfo.Name))
        {
            _logger.LogWarning("An establishment with the name {Name} already exists.", establishmentInfo.Name);
            throw new EstablishmentAlreadyExistsException(establishmentInfo.Name);
        }
        
        // Get coordinates from address using Mapbox API and set them to variable
        var fullAddress = $"{establishmentInfo.Address}, {establishmentInfo.City}";
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(fullAddress);
        
        // Check if categories and tags exist and set them to variable
        var catsAndTags = await SetCategoriesAndTagsAsync(establishmentInfo);
        
        // Register establishment on Auth0
        var auth0Id = await _auth0Service.RegisterEstablishmentAccountAsync(establishmentInfo.Name, personalInfo);

        var establishmentEntity = new Establishment()
        {
            Auth0Id = auth0Id,
            Name = establishmentInfo.Name,
            Latitude = coordinates.lat,
            Longitude = coordinates.lng,
            Address = establishmentInfo.Address,
            City = establishmentInfo.City,
            PriceCategory = establishmentInfo.PriceCategory,
            EstablishmentCategories = catsAndTags.categories.Select(c => new EstablishmentCategory() { Category = c }).ToList(),
            EstablishmentTags = catsAndTags.tags.Select(t => new EstablishmentTag() { Tag = t }).ToList()
        };
            
        try
        {
            // Save establishment to the database
            var result =  await _establishmentRepository.CreateEstablishmentAsync(establishmentEntity);
            return result;
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to create establishment in the database");
            await _auth0Service.DeleteAccountAsync(auth0Id);
            throw new Exception("Failed to create establishment in the database");
        }
    }
    
    public async Task<EstablishmentDto> GetEstablishmentByIdAsync(int id)
    {
        var establishment = await _establishmentRepository.GetEstablishmentByIdAsync(id);
        if (establishment == null)
        {
            throw new EstablishmentNotFoundException($"ID: {id}");
        }

        return establishment.ToDto();
    }
    
    public async Task<EstablishmentDto> GetEstablishmentByAuth0IdAsync(string auth0Id)
    {
        var establishment = await _establishmentRepository.GetEstablishmentByAuth0IdAsync(auth0Id);
        if (establishment == null)
        {
            throw new EstablishmentNotFoundException($"Auth0ID: {auth0Id}");
        }

        return establishment.ToDto();
    }

    public async Task<List<EstablishmentPin>?> GetEstablishmentPinsByAreaAsync(EstablishmentsInAreaRequest establishmentsInAreaRequest)
    {
        establishmentsInAreaRequest.ValidateProperties();
        
        var nwLatitude = establishmentsInAreaRequest.NWCorner[0];
        var nwLongitude = establishmentsInAreaRequest.NWCorner[1];
        var seLatitude = establishmentsInAreaRequest.SECorner[0];
        var seLongitude = establishmentsInAreaRequest.SECorner[1];
        
        var establishments = await _establishmentRepository.GetEstablishmentsByAreaAsync(nwLatitude, nwLongitude, seLatitude, seLongitude);

        return establishments?.Select(e => e.ToPin()).ToList();
    }

    public async Task DeleteEstablishmentAsync(string auth0Id)
    {
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        // Delete establishment account from Auth0
        await _auth0Service.DeleteAccountAsync(auth0Id);
        
        var result = await _establishmentRepository.DeleteEstablishmentByAuth0IdAsync(auth0Id);
        if (!result)
        {
            throw new EstablishmentNotFoundException($"Auth0 ID: {auth0Id}");
        }
    }
    
    /// <summary>
    /// Sets categories and tags for the establishment.
    /// Categories are validated and fetched from the database.
    /// Tags are fetched from the categories list,
    /// if they don't belong to any category (they have null Category in DB),
    /// they are fetched from the database.
    /// </summary>
    
    private async Task<(Category[] categories, Tag[] tags)> SetCategoriesAndTagsAsync(EstablishmentInfo establishmentInfo)
    {
        List<Category> categories = [];
        List<Tag> tags = [];
        
        foreach (var categoryName in establishmentInfo.CategoryNames)
        {
            // Get category from database by name, including tags
            var categoryEntity = await _categoryRepository.GetCategoryByNameWithTagsAsync(categoryName);
                
            if (categoryEntity == null)
            {
                _logger.LogWarning("Category {CategoryName} not found", categoryName);
                throw new Exception("Category not found");
            }
            
            categories.Add(categoryEntity);
        }

        if (establishmentInfo.TagNames != null)
        {
            foreach (var tag in establishmentInfo.TagNames)
            {
                // Check if tag belongs to any of the categories, if not, get it from the database
                var tagEntity = categories
                                    .SelectMany(c => c.Tags)
                                    .FirstOrDefault(t => t.Name == tag) 
                                ?? await _tagRepository.GetTagByNameAsync(tag);

                if (tagEntity == null)
                {
                    _logger.LogWarning("Tag {TagName} not found", tag);
                    throw new Exception("Tag not found");
                }
                
                tags.Add(tagEntity);
            }
        }
        
        return (categories.ToArray(), tags.ToArray());
    }
}