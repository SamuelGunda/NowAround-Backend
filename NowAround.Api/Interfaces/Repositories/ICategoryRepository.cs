using NowAround.Api.Models.Domain;

namespace NowAround.Api.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetCategoryByNameAsync(string name);
    Task<Category?> GetCategoryByNameWithTagsAsync(string name);
}