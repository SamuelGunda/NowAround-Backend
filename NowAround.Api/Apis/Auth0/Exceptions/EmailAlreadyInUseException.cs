﻿namespace NowAround.Api.Apis.Auth0.Exceptions;

public sealed class EmailAlreadyInUseException : Exception
{
    public EmailAlreadyInUseException(string email) 
        : base($"The email address: {email} is already in use by another existing account")
    {
    }
    
}