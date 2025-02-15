using DAL.Entities;
using WebApi.DTOs.Identity;
using WebApi.DTOs.Url;

namespace WebApi.Mapping;

public static class DomainToDto
{
    public static UserDto ToUserDto(this UserEntity userEntity)
    {
        return new UserDto
        {
            Id = userEntity.Id,
            Username = userEntity.UserName,
        };
    }

    public static UrlDto ToUrlDto(this UrlEntity urlEntity)
    {
        return new UrlDto
        {
            Id = urlEntity.Id,
            UserId = urlEntity.UserId,
            CreatedAt = urlEntity.CreatedAt,
            ShortCode = urlEntity.ShortCode,
            UrlOriginal = urlEntity.UrlOriginal,
        };
    }

    public static UrlInfoDto ToUrlInfoDto(this UrlEntity urlEntity)
    {
        return new UrlInfoDto
        {
            Id = urlEntity.Id,
            UserId = urlEntity.UserId,
            CreatedAt = urlEntity.CreatedAt,
            ShortCode = urlEntity.ShortCode,
            UrlOriginal = urlEntity.UrlOriginal,
        };
    }
}