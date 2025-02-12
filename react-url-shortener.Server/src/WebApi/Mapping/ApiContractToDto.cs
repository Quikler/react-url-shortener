using WebApi.Contracts.V1.Requests.Identity;
using WebApi.DTOs.Identity;

namespace WebApi.Mapping;

public static class ApiContractToDto
{
    public static LoginDto ToDto(this LoginRequest request)
    {
        return new LoginDto
        {
            UserName = request.UserName,
            Password = request.Password,
        };
    }

    public static SignupDto ToDto(this SignupRequest request)
    {
        return new SignupDto
        {
            UserName = request.UserName,
            Password = request.Password,
        };
    }
}