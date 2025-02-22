﻿using System.ComponentModel.DataAnnotations;
using NowAround.Domain.Common;
using NowAround.Domain.Enum;

namespace NowAround.Domain.Models;

public class Establishment : BaseAccountEntity
{
    [MaxLength(32)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public string? Description { get; set; }
    [MaxLength(32)]
    public required string City { get; set; }
    [MaxLength(64)]
    public required string Address { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required PriceCategory PriceCategory { get; set; }
    public RequestStatus RequestStatus { get; set; }
    
    public BusinessHours BusinessHours { get; set; } = new();
    public RatingStatistic RatingStatistic { get; set; } = new();
    
    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>(); 
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
}