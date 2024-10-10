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
    
    public async Task<Establishment> GetEstablishmentByAuth0IdAsync(string auth0Id)
    {
        try
        {
            return await _context.Establishments.FirstOrDefaultAsync(e => e.Auth0Id == auth0Id) ?? throw new Exception("Establishment not found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<bool> DeleteEstablishmentByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var establishment = await _context.Establishments.FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (establishment == null)
            {
                throw new KeyNotFoundException("Establishment not found");
            }
            
            _context.Establishments.Remove(establishment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to delete establishment", e);
        }
    }
}