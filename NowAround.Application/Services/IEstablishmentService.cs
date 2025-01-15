using Microsoft.AspNetCore.Http;
using NowAround.Application.Dtos;
using NowAround.Application.Requests;
using NowAround.Application.Responses;
using NowAround.Domain.Enum;
using NowAround.Domain.Models;

namespace NowAround.Application.Services;

public interface IEstablishmentService
{
    Task RegisterEstablishmentAsync(EstablishmentRegisterRequest request);
    /*Task<bool> CheckIfEstablishmentExistsAsync(string auth0Id);*/
    /*Task<EstablishmentResponse> GetEstablishmentByIdAsync(int id);*/
    Task<EstablishmentProfileResponse> GetEstablishmentProfileByAuth0IdAsync(string auth0Id);
    Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id, bool tracked = false, params Func<IQueryable<Establishment>, IQueryable<Establishment>>[] includeProperties);
    Task<List<PendingEstablishmentResponse>> GetPendingEstablishmentsAsync();
    Task<List<EstablishmentMarkerResponse>> GetEstablishmentsWithFilterAsync(SearchValues searchValues, int page);
    Task<int> GetEstablishmentsCountCreatedInMonthAsync(DateTime monthStart, DateTime monthEnd);
    Task<GenericInfo> UpdateEstablishmentGenericInfoAsync(string auth0Id, EstablishmentGenericInfoUpdateRequest request);
    Task<LocationInfo> UpdateEstablishmentLocationInfoAsync(string auth0Id, EstablishmentLocationInfoUpdateRequest request);
    Task UpdateEstablishmentRegisterRequestAsync(string auth0Id, RequestStatus requestStatus);
    Task<string> UpdateEstablishmentPictureAsync(string auth0Id, string pictureContext, IFormFile picture);
    Task UpdateRatingStatisticsAsync(int id, int rating, bool increment = true);
    Task DeleteEstablishmentAsync(string auth0Id);
    
    // Menu Methods
    
    Task<MenuDto> AddMenuAsync(string auth0Id, MenuCreateRequest menu);
    Task<MenuDto> UpdateMenuAsync(string auth0Id, MenuUpdateRequest updatedMenu);
    Task<string> UpdateMenuItemPictureAsync(string auth0Id, int menuItemId, IFormFile picture);
    Task DeleteMenuAsync(string auth0Id, int menuId);
    Task DeleteMenuItemAsync(string auth0Id, int menuItemId);
}