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
}