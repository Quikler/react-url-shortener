using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.Contracts.V1.Responses.Url;
using WebApi.DTOs;
using WebApi.DTOs.Identity;
using WebApi.DTOs.Url;

namespace WebApi.Mapping;

public static class DtoToApiContract
{
    public static FailureResponse ToResponse(this FailureDto failureDto) => new(failureDto.Errors);

    public static AuthResponse ToResponse(this AuthDto authDto)
    {
        return new AuthResponse
        {
            Token = authDto.Token,
            User = authDto.User.ToResponse(),
            Roles = authDto.Roles,
        };
    }

    public static UserResponse ToResponse(this UserDto userDto)
    {
        return new UserResponse
        {
            Id = userDto.Id,
            Username = userDto.Username,
        };
    }

    public static UrlResponse ToResponse(this UrlDto urlDto, string rootApiUrl)
    {
        return new UrlResponse
        {
            Id = urlDto.Id,
            UserId = urlDto.UserId,
            UrlShortened = $"{rootApiUrl}/{urlDto.ShortCode}",
            UrlOriginal = urlDto.UrlOriginal,
        };
    }

    public static UrlInfoResponse ToResponse(this UrlInfoDto urlInfoDto, string rootApiUrl)
    {
        return new UrlInfoResponse
        {
            Id = urlInfoDto.Id,
            UserId = urlInfoDto.UserId,
            CreatedAt = urlInfoDto.CreatedAt,
            UrlShortened = $"{rootApiUrl}/{urlInfoDto.ShortCode}",
            UrlOriginal = urlInfoDto.UrlOriginal,
            User = urlInfoDto.User.ToResponse(),
        };
    }
}