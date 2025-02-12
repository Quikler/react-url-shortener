using DAL.Entities;
using WebApi.DTOs.Identity;

namespace WebApi.Mapping;

public static class DomainToDto
{
    public static UserDto ToUserDto(this UserEntity userEntity)
    {
        return new UserDto
        {
            Id = userEntity.Id,
            UserName = userEntity.UserName,
        };
    }
}