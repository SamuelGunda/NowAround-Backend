using NowAround.Domain.Interfaces.Base;
using NowAround.Domain.Models;

namespace NowAround.Domain.Interfaces.Specific;

public interface IEstablishmentRepository : IBaseAccountRepository<Establishment>
{
    Task<Establishment> GetProfileByAuth0IdAsync(string auth0Id);
    Task<List<Establishment>> GetAllWhereRegisterStatusPendingAsync();

    Task<List<Establishment>> GetRangeWithFilterAsync(
        Func<IQueryable<Establishment>, IQueryable<Establishment>> queryBuilder,
        int page);
}