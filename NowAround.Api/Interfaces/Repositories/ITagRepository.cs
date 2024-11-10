using NowAround.Api.Models.Domain;

namespace NowAround.Api.Interfaces.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetTagByNameAsync(string name);
}