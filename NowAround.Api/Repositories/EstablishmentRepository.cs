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
        try
        {
            return await _context.Establishments.AnyAsync(e => e.Name == name);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to check if establishment exists by name: {Message}", e.Message);
            throw new Exception("Failed to check if establishment exists by name", e);
        }
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
    
    public async Task<Establishment?> GetEstablishmentByIdAsync(int id)
    {
        try
        {
            var establishment = await _context.Establishments.FirstOrDefaultAsync(e => e.Id == id);
            if (establishment == null)
            {
                _logger.LogWarning("Establishment with ID {id} not found", id);
                return null;
            }
            
            return establishment;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get establishment by ID: {Message}", e.Message);
            throw new Exception("Failed to get establishment by ID", e);
        }
    }
    
    public async Task<bool> DeleteEstablishmentByAuth0IdAsync(string auth0Id)
    {
        try
        {
            var establishment = await _context.Establishments.FirstOrDefaultAsync(e => e.Auth0Id == auth0Id);
            if (establishment == null)
            {
                _logger.LogWarning("Establishment with Auth0 ID {Auth0Id} not found", auth0Id);
                return false;
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