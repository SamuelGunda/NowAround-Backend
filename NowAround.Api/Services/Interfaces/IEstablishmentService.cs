using NowAround.Api.Apis.Auth0.Models.Requests;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Enum;
using NowAround.Api.Models.Requests;
using NowAround.Api.Models.Responses;

namespace NowAround.Api.Services.Interfaces;

public interface IEstablishmentService
{
    Task RegisterEstablishmentAsync(EstablishmentRegisterRequest request);
    /*Task<bool> CheckIfEstablishmentExistsAsync(string auth0Id);*/
    /*Task<EstablishmentResponse> GetEstablishmentByIdAsync(int id);*/
    Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id, bool tracked = false, params Func<IQueryable<Establishment>, IQueryable<Establishment>>[] includeProperties);
    Task<EstablishmentProfileResponse> GetEstablishmentProfileByAuth0IdAsync(string auth0Id);
    Task<List<PendingEstablishmentResponse>> GetPendingEstablishmentsAsync();
    Task<List<EstablishmentMarkerResponse>> GetEstablishmentsWithFilterAsync(SearchValues searchValues, int page);
    Task<int> GetEstablishmentsCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd);
    Task<GenericInfo> UpdateEstablishmentGenericInfoAsync(string auth0Id, EstablishmentGenericInfoUpdateRequest request);
    Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus);
    Task<string> UpdateEstablishmentPictureAsync(string auth0Id, string pictureContext, IFormFile picture);
    Task UpdateRatingStatisticsAsync(string auth0Id, int rating);
    Task DeleteEstablishmentAsync(string auth0Id);
    
    // Menu Methods
    
    Task<MenuDto> AddMenuAsync(string auth0Id, MenuCreateRequest menu);
    Task<MenuDto> UpdateMenuAsync(string auth0Id, MenuUpdateRequest updatedMenu);
    Task<string> UpdateMenuItemPictureAsync(string auth0Id, int menuItemId, IFormFile picture);
    Task DeleteMenuAsync(string auth0Id, int menuId);
    Task DeleteMenuItemAsync(string auth0Id, int menuItemId);
}