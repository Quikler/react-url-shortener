using WebApi.Contracts.V1.Requests.Identity;
using WebApi.IntegrationTests.Abstractions;

namespace WebApi.IntegrationTests.Identity;

public class BaseIdentityTests(IntegrationTestWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    protected const string TestUsername = "test";
    protected const string TestPassword = "testtest";
    protected const string InvalidUsernameOrPasswordMessage = "Invalid username or password.";

    protected static LoginRequest CreateLoginRequest(string username, string password) => new()
    {
        Username = username,
        Password = password,
    };

    protected static SignupRequest CreateSignupRequest(string username, string password, string confirmPassword) => new()
    {
        Username = username,
        Password = password,
        ConfirmPassword = confirmPassword,
    };
}