using WebApi.Contracts.V1.Requests.Identity;
using WebApi.DTOs.Identity;

namespace WebApi.Mapping;

public static class ApiContractToDto
{
    public static LoginDto ToDto(this LoginRequest request)
    {
        return new LoginDto
        {
            Username = request.Username,
            Password = request.Password,
        };
    }

    public static SignupDto ToDto(this SignupRequest request)
    {
        return new SignupDto
        {
            Username = request.Username,
            Password = request.Password,
        };
    }
}