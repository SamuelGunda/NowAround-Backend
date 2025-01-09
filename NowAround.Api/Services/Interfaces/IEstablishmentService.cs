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
    Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id, bool tracked = false);
    Task<EstablishmentProfileResponse> GetEstablishmentProfileByAuth0IdAsync(string auth0Id);
    Task<List<PendingEstablishmentResponse>> GetPendingEstablishmentsAsync();
    Task<List<EstablishmentDto>> GetEstablishmentsWithFilterAsync(SearchValues searchValues, int page);
    Task<int> GetEstablishmentsCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd);
    Task UpdateEstablishmentAsync(string auth0Id, EstablishmentUpdateRequest request);
    Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus);
    Task UpdateEstablishmentPictureAsync(string auth0Id, string pictureContext, IFormFile image);
    Task DeleteEstablishmentAsync(string auth0Id);
    
    // Menu Methods
    
    Task AddMenuAsync(string auth0Id, MenuCreateRequest menu);
    Task UpdateMenuAsync(string auth0Id, int menuId, MenuCreateRequest updatedMenu);
    Task DeleteMenuAsync(string auth0Id, int menuId);
    Task DeleteMenuItemAsync(string auth0Id, int menuItemId);
}