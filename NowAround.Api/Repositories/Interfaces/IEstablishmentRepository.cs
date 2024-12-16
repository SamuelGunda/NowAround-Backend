using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Dtos;
using NowAround.Api.Models.Entities;
using NowAround.Api.Models.Responses;

namespace NowAround.Api.Repositories.Interfaces;

public interface IEstablishmentRepository : IBaseAccountRepository<Establishment>
{
    new Task<EstablishmentProfileResponse> GetByAuth0IdAsync(string auth0Id);
    Task<List<Establishment>> GetAllWhereRegisterStatusPendingAsync();
    Task<List<EstablishmentDto>> GetRangeWithFilterAsync(SearchValues searchValues, int page);
    Task UpdateAsync(EstablishmentDto establishmentDto);
}