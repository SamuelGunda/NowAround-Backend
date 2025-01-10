using System.Linq.Expressions;
using NowAround.Api.Models.Entities;

namespace NowAround.Api.Repositories.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T> CreateAsync(T entity);
    Task<bool> CheckIfExistsByPropertyAsync(string propertyName, object propertyValue);
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByPropertyAsync(string propertyName, object propertyValue);
    Task<T> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        bool tracked = true,
        params Func<IQueryable<T>, IQueryable<T>>[] includeProperties);
    Task<IEnumerable<T>> GetAllAsync();
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task DeleteRangeAsync<TEntity>(IQueryable<TEntity> entities) where TEntity : class;
}