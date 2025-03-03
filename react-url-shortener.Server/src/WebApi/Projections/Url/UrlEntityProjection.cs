using System.Linq.Expressions;
using DAL.Entities;
using WebApi.DTOs.Identity;
using WebApi.DTOs.Url;

namespace WebApi.Projections.Url;

public static class UrlEntityProjection
{
    public static Expression<Func<UrlEntity, UrlDto>> UrlDto => u => new UrlDto
    {
        Id = u.Id,
        CreatedAt = u.CreatedAt,
        ShortCode = u.ShortCode,
        UrlOriginal = u.UrlOriginal,
        UserId = u.UserId,
    };

    public static Expression<Func<UrlEntity, UrlInfoDto>> UrlInfoDto => u => new UrlInfoDto
    {
        Id = u.Id,
        CreatedAt = u.CreatedAt,
        ShortCode = u.ShortCode,
        UrlOriginal = u.UrlOriginal,
        UserId = u.UserId,
        User = new UserDto
        {
            Id = u.UserId,
            Username = u.User.UserName,
        }
    };
}