using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;

namespace NowAround.Api.Repositories.Interfaces;

public interface IEstablishmentRepository : IBaseAccountRepository<Establishment>
{
    Task<List<Establishment>> GetAllWhereRegisterStatusPendingAsync();
    Task<List<Establishment>> GetRangeWithFilterAsync(string? name, int? priceCategory, string? categoryName, List<string>? tagNames);
    Task<List<Establishment>> GetRangeWithFilterInAreaAsync(
        double nwLat, double nwLong, double seLat, double seLong,
        string? name, int? priceCategory, string? categoryName, List<string>? tagNames
    );
    Task UpdateAsync(EstablishmentDto establishmentDto);
}