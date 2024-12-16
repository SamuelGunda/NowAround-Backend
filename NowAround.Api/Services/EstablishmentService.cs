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
        
        // Get coordinates from address using Mapbox API 
        var coordinates = await _mapboxService.GetCoordinatesFromAddressAsync(establishmentInfo.Address, establishmentInfo.PostalCode, establishmentInfo.City);
        
        // Check if categories and tags exist by their name and get them from the database
        var catsAndTags = await SetCategoriesAndTagsAsync(establishmentInfo.Category, establishmentInfo.Tags);
        
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
    
    public async Task<EstablishmentResponse> GetEstablishmentByIdAsync(int id)
    {
        var establishment = await _establishmentRepository.GetByIdAsync(id);
        if (establishment == null)
        {
            throw new EntityNotFoundException("Establishment","ID", id.ToString());
        }

        return establishment.ToDetailedResponse();
    }
    
    public async Task<EstablishmentProfileResponse> GetEstablishmentByAuth0IdAsync(string auth0Id)
    {
        var establishment = await _establishmentRepository.GetByAuth0IdAsync(auth0Id);
        if (establishment == null)
        {
            throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
        }

        return establishment;
    }

    public async Task<List<PendingEstablishmentResponse>> GetPendingEstablishmentsAsync()
    {
        var establishments = await _establishmentRepository.GetAllWhereRegisterStatusPendingAsync();
        
        var pendingEstablishments = establishments.Select(e => new PendingEstablishmentResponse
        {
            Auth0Id = e.Auth0Id,
            Name = e.Name,
            OwnerName = _auth0Service.GetEstablishmentOwnerFullNameAsync(e.Auth0Id).Result
        }).ToList();
        
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

    public async Task UpdateEstablishmentAsync(EstablishmentUpdateRequest request)
    {
        var auth0Id = request.Auth0Id;
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(request.Auth0Id));
        }
        
        var catsAndTags = await SetCategoriesAndTagsAsync(request.Categories, request.Tags);
        
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
        if (auth0Id.IsNullOrEmpty())
        {
            _logger.LogWarning("auth0Id is null");
            throw new ArgumentNullException(nameof(auth0Id));
        }
        
        await _auth0Service.DeleteAccountAsync(auth0Id);
        
        var result = await _establishmentRepository.DeleteByAuth0IdAsync(auth0Id);
        if (!result)
        {
            throw new EntityNotFoundException("Establishment", "Auth0 ID", auth0Id);
        }
    }

    private async Task<(Category[] categories, Tag[] tags)> SetCategoriesAndTagsAsync(ICollection<string>? categoryNames, ICollection<string>? tagNames)
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
}