﻿using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NowAround.Api.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestAuthScheme = "TestAuthScheme";
    

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Context.Request.Headers.Authorization;
        if (authHeader.Count == 0)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        
        var authHeaderParts = authHeader[0].Split(' ');
        var role = authHeaderParts[1];

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, role),
            new Claim("roles", role)
        };

        if (authHeaderParts.Length >= 3)
        {
            var sub = authHeaderParts[2];
            claims.Add(new Claim("sub", sub));
        }
        
        var identity = new ClaimsIdentity(claims, TestAuthScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestAuthScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}