using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories.Interfaces;

public interface IEstablishmentRepository : IBaseAccountRepository<Establishment>
{
    Task<List<Establishment>> GetAllWhereRegisterStatusPendingAsync();
    /*Task<List<Establishment>> GetRangeWithFilterDeprecatedAsync(string? name, int? priceCategory, string? categoryName, List<string>? tagNames);*/
    Task<List<Establishment>> GetRangeWithFilterAsync(FilterValues filterValues);
    Task UpdateAsync(EstablishmentDto establishmentDto);
}