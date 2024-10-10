using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Models;
using NowAround.Api.Interfaces;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Authentication.Services;

public class EstablishmentService : IEstablishmentService
{
    
    private readonly IAccountManagementService _accountManagementService;
    private readonly IMapboxService _mapboxService;
    private readonly IEstablishmentRepository _establishmentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;

    public EstablishmentService(
        IAccountManagementService accountManagementService, 
        IMapboxService mapboxService,
        IEstablishmentRepository establishmentRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository)
    {
        _accountManagementService = accountManagementService;
        _mapboxService = mapboxService;
        _establishmentRepository = establishmentRepository;
        _categoryRepository = categoryRepository;
        _tagRepository = tagRepository;
    }
    
    /// <summary>
    /// Registers a new establishment asynchronously.
    /// Registration process includes:
    /// Name validation, address validation category and tag validation.
    /// Establishment is registered on Auth0 and saved to the database on success.
    /// If database operation fails, the establishment account is deleted from Auth0.
    /// </summary>
    
    public async Task<int> RegisterEstablishmentAsync(EstablishmentRegisterRequest establishmentRequest)
    {
        establishmentRequest.ValidateProperties();
        
        var establishmentInfo = establishmentRequest.EstablishmentInfo;
        var personalInfo = establishmentRequest.PersonalInfo;
        
        // Check if establishment with this name already exists
        if (await _establishmentRepository.CheckIfEstablishmentExistsByNameAsync(establishmentInfo.Name))
        {
            throw new InvalidOperationException("An establishment with this name already exists.");
        }
        
        // Get coordinates from address using Mapbox API and set them to variable
        var fullAddress = $"{establishmentInfo.Adress}, {establishmentInfo.City}";
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(fullAddress);
        
        // Check if categories and tags exist and set them to variable
        var catsAndTags = await SetCategoriesAndTagsAsync(establishmentInfo);
        
        // Register establishment on Auth0
        var auth0Id = await _accountManagementService.RegisterEstablishmentAccountAsync(establishmentInfo.Name, personalInfo);
        
        try 
        {
            var establishmentEntity = new Establishment()
            {
                Auth0Id = auth0Id,
                Name = establishmentInfo.Name,
                Latitude = coordinates.lat,
                Longitude = coordinates.lng,
                Address = establishmentInfo.Adress,
                City = establishmentInfo.City,
                PriceCategory = establishmentInfo.PriceCategory,
                EstablishmentCategories = catsAndTags.categories.Select(c => new EstablishmentCategory() { Category = c }).ToList(),
                EstablishmentTags = catsAndTags.tags.Select(t => new EstablishmentTag() { Tag = t }).ToList()
            };
            
            // Save establishment to database    
            return await _establishmentRepository.CreateEstablishmentAsync(establishmentEntity);
        }
        catch (Exception e)
        {
            await _accountManagementService.DeleteAccountAsync(auth0Id);
            throw new Exception($"Failed to create establishment: {e.Message}", e);
        }
    }
    
    public async Task<EstablishmentDto> GetEstablishmentAsync(string auth0Id)
    {
        var establishment = await _establishmentRepository.GetEstablishmentByAuth0IdAsync(auth0Id);
        if (establishment == null)
        {
            throw new Exception("Establishment not found");
        }

        return establishment.ToDto();
    }
    
    public async Task<bool> DeleteEstablishmentAsync(string auth0Id)
    {
        ArgumentNullException.ThrowIfNull(auth0Id);
        
        await _accountManagementService.DeleteAccountAsync(auth0Id);
        
        try
        {
            return await _establishmentRepository.DeleteEstablishmentByAuth0IdAsync(auth0Id);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to delete establishment", e);
        }
    }
    
    private async Task<(Category[] categories, Tag[] tags)> SetCategoriesAndTagsAsync(EstablishmentInfo establishmentInfo)
    {
        List<Category> categories = [];
        List<Tag> tags = [];
        
        foreach (var categoryName in establishmentInfo.CategoryNames)
        {
            var categoryEntity = await _categoryRepository.GetCategoryByNameWithTagsAsync(categoryName);
                
            if (categoryEntity == null)
            {
                throw new Exception("Category not found");
            }
            
            categories.Add(categoryEntity);
        }

        if (establishmentInfo.TagNames != null)
        {
            foreach (var tag in establishmentInfo.TagNames)
            {
                var tagEntity = categories
                                    .SelectMany(c => c.Tags)
                                    .FirstOrDefault(t => t.Name == tag) 
                                ?? await _tagRepository.GetTagByNameAsync(tag);

                if (tagEntity == null)
                {
                    throw new Exception("Tag not found");
                }
                
                tags.Add(tagEntity);
            }
        }
        
        return (categories.ToArray(), tags.ToArray());
    }
}