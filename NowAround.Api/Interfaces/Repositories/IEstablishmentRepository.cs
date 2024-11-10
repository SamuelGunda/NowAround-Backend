using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Interfaces.Repositories;

public interface IEstablishmentRepository
{
    Task<bool> CheckIfEstablishmentExistsByNameAsync(string name);
    Task<int> CreateEstablishmentAsync(Establishment establishment);
    Task<Establishment?> GetEstablishmentByIdAsync(int id);
    Task<Establishment?> GetEstablishmentByAuth0IdAsync(string auth0Id);
    Task<List<Establishment>?> GetEstablishmentsWithPendingRegisterStatusAsync();
    Task<List<Establishment>?> GetEstablishmentsWithFilterAsync(string? name, int? priceCategory, string? categoryName, List<string>? tagNames);
    Task<List<Establishment>?> GetEstablishmentsWithFilterInAreaAsync(
        double nwLat, double nwLong, double seLat, double seLong,
        string? name, int? priceCategory, string? categoryName, List<string>? tagNames
    );
    Task<int> GetEstablishmentsCountByCreatedAtBetweenDatesAsync(DateTime startDate, DateTime endDate);
    Task<bool> UpdateEstablishmentByAuth0IdAsync(string auth0Id, EstablishmentDto establishmentDto);
    Task<bool> DeleteEstablishmentByAuth0IdAsync(string auth0Id);
}