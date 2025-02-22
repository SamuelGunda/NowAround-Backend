﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NowAround.WebApi.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }
    
    [Authorize]
    [HttpGet("secure")]
    public IActionResult GetSecure()
    {
        return Ok("Secure");
    }
}