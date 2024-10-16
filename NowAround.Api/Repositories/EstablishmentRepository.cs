using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public class EstablishmentRepository : IEstablishmentRepository
{
    
    private readonly AppDbContext _context;
    private readonly ILogger<EstablishmentRepository> _logger;
    
    public EstablishmentRepository(AppDbContext context, ILogger<EstablishmentRepository> logger)
    {
        _context = context;
        _logger = logger;
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
            _logger.LogError("Failed to create establishment: {Message}", e.Message);
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
            _logger.LogError("Failed to get establishment by Auth0 ID: {Message}", e.Message);
            throw new Exception("Failed to get establishment by Auth0 ID", e);
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
            _logger.LogError("Failed to delete establishment: {Message}", e.Message);
            throw new InvalidOperationException("Failed to delete establishment", e);
        }
    }
}