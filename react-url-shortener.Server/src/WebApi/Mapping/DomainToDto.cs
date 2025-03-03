using DAL.Entities;
using WebApi.DTOs;
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
            User = urlEntity.User?.ToUserDto() ?? throw new Exception(),
        };
    }

    public static PaginationDto<TResult> ToPagination<T, TResult>(this IEnumerable<T> collection, Func<T, TResult> itemsSelect, int totalRecords, int pageNumber, int pageSize)
    {
        return new PaginationDto<TResult>
        {
            TotalCount = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = collection.Select(itemsSelect)
        };
    }
}