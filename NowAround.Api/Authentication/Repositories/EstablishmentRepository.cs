using Microsoft.EntityFrameworkCore;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Authentication.Repositories;

public class EstablishmentRepository : IEstablishmentRepository
{
    
    private readonly AppDbContext _context;
    
    public EstablishmentRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> CheckIfEstablishmentExistsByNameAsync(string name)
    {
        return await _context.Establishments.AnyAsync(e => e.Name == name);
    }
    
    public async Task<int> CreateEstablishmentAsync(Establishment establishment)
    {
        try
        {
            await _context.Establishments.AddAsync(establishment);
            await _context.SaveChangesAsync();
            return establishment.Id;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create establishment", e);
        }
    }
}