﻿using System.ComponentModel.DataAnnotations;

namespace NowAround.Domain.Common;

public abstract class BaseAccountEntity : BaseEntity
{
    [MaxLength(48)]
    public required string Auth0Id { get; set; }

    public string ProfilePictureUrl { get; set; } = "default";
    public string BackgroundPictureUrl { get; set; } = "default";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    
}