using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories.Interfaces;

public interface IEstablishmentRepository : IBaseAccountRepository<Establishment>
{
    Task<List<Establishment>> GetAllWhereRegisterStatusPendingAsync();
    Task<List<EstablishmentDto>> GetRangeWithFilterAsync(SearchValues searchValues, int page);
    Task UpdateAsync(EstablishmentDto establishmentDto);
}