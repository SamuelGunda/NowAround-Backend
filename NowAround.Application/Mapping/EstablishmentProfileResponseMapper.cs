using NowAround.Application.Dtos;
using NowAround.Application.Responses;
using NowAround.Domain.Models;

namespace NowAround.Application.Mapping;

public static class EstablishmentProfileResponseMapper
{
    public static EstablishmentProfileResponse ToProfileResponse(this Establishment establishment)
    {
        ArgumentNullException.ThrowIfNull(establishment, nameof(establishment));

        return new EstablishmentProfileResponse(
            establishment.Auth0Id,
            new GenericInfo(
                establishment.Name,
                establishment.ProfilePictureUrl,
                establishment.BackgroundPictureUrl,
                establishment.Description,
                establishment.PriceCategory.ToString(),
                establishment.Tags.Select(et => et.Name).ToList(),
                establishment.Categories.Select(ec => ec.Name).ToList(),
                establishment.SocialLinks.Select(sl => new SocialLinkDto(sl.Name, sl.Url)).ToList()
            ),
            new LocationInfo(
                establishment.Address,
                establishment.City,
                establishment.Longitude,
                establishment.Latitude,
                new BusinessHoursDto(
                    establishment.BusinessHours.Monday,
                    establishment.BusinessHours.Tuesday,
                    establishment.BusinessHours.Wednesday,
                    establishment.BusinessHours.Thursday,
                    establishment.BusinessHours.Friday,
                    establishment.BusinessHours.Saturday,
                    establishment.BusinessHours.Sunday,
                    establishment.BusinessHours.BusinessHoursExceptions
                        .Select(bhe => new BusinessHoursExceptionsDto(bhe.Date, bhe.Status))
                        .ToList()
                )
            ),
            establishment.Posts.Select(p => p.ToDto()).ToList(),
            establishment.Menus.Select(m => new MenuDto(
                m.Id,
                m.Name,
                m.MenuItems.Select(mi => new MenuItemDto(
                    mi.Id,
                    mi.Name,
                    mi.PictureUrl,
                    mi.Description,
                    mi.Price
                )).ToList()
            )).ToList(),
            establishment.Events.Select(ev => new EventDto(
                ev.Id,
                null,
                ev.Title,
                ev.Body,
                ev.Price,
                ev.EventPriceCategory,
                ev.City,
                ev.Address,
                ev.Latitude,
                ev.Longitude,
                ev.MaxParticipants,
                ev.PictureUrl,
                ev.Start,
                ev.End,
                ev.EventCategory.ToString(),
                ev.CreatedAt,
                ev.InterestedUsers.Select(iu => iu.Auth0Id).ToList()
            )).ToList(),
            new RatingStatisticResponse(
                establishment.RatingStatistic.OneStar,
                establishment.RatingStatistic.TwoStars,
                establishment.RatingStatistic.ThreeStars,
                establishment.RatingStatistic.FourStars,
                establishment.RatingStatistic.FiveStars,
                establishment.RatingStatistic.Reviews.Select(r => new ReviewWithAuthIdsResponse(
                    r.User.Auth0Id,
                    null,
                    r.User.FullName,
                    r.Body,
                    r.Rating,
                    r.CreatedAt
                )).ToList()
            )
        );
    }
}