using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.DTOs;
using WebApi.DTOs.Identity;

namespace WebApi.Mapping;

public static class DtoToApiContract
{
    public static FailureResponse ToResponse(this FailureDto failureDto) => new FailureResponse(failureDto.Errors);

    public static AuthResponse ToResponse(this AuthDto authDto)
    {
        return new AuthResponse
        {
            Token = authDto.Token,
            User = authDto.User.ToResponse(),
        };
    }

    public static UserResponse ToResponse(this UserDto userDto)
    {
        return new UserResponse
        {
            Id = userDto.Id,
            UserName = userDto.UserName,
        };
    }
}