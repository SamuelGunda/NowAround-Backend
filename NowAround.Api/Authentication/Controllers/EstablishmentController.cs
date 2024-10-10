using Microsoft.AspNetCore.Mvc;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Models;

namespace NowAround.Api.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstablishmentController(IEstablishmentService establishmentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateEstablishmentAsync(EstablishmentRegisterRequest establishment)
    {
        try
        {
            var establishmentId = await establishmentService.RegisterEstablishmentAsync(establishment);
            return Ok(establishmentId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetEstablishmentAsync(string auth0Id)
    {
        try
        {
            var establishment = await establishmentService.GetEstablishmentAsync(auth0Id);
            return Ok(establishment);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteEstablishmentAsync(string auth0Id)
    {
        try
        {
            await establishmentService.DeleteEstablishmentAsync(auth0Id);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}