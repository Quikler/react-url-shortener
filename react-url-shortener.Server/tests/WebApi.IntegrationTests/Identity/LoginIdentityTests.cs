using System.Net.Http.Json;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.Identity;
using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.IntegrationTests.Abstractions;

namespace WebApi.IntegrationTests.Identity;

public class LoginIdentityTests(IntegrationTestWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_WithCorrectPassword_Succeeds()
    {
        // Arrange
        var signupRequest = new SignupRequest
        {
            Username = "test",
            Password = "testtest",
            ConfirmPassword = "testtest",
        };
        
        var signupResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);
        signupResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var loginRequest = new LoginRequest
        {
            Username = "test",
            Password = "testtest"
        };

        // Act
        var loginReponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Login, loginRequest);

        // Assert
        loginReponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var content = await loginReponse.Content.ReadFromJsonAsync<AuthResponse>();
        content.ShouldNotBeNull();
        content.Token.ShouldNotBeNull();
        content.User.Username.ShouldBe("test");
    }

    [Fact]
    public async Task Login_WithIncorrectPassword_Fails()
    {
        // Arrange
        var signupRequest = new SignupRequest
        {
            Username = "test",
            Password = "testtest",
            ConfirmPassword = "testtest",
        };
        
        var signupResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);
        signupResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var loginRequest = new LoginRequest
        {
            Username = "test",
            Password = "incorrect_password"
        };

        // Act
        var loginReponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Login, loginRequest);

        // Assert
        loginReponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);

        var content = await loginReponse.Content.ReadFromJsonAsync<FailureResponse>();
        content.ShouldNotBeNull();
        content.Errors.ShouldContain("Invalid username or password.");
    }

    [Fact]
    public async Task Login_WithIncorrectUsername_Fails()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "test",
            Password = "random_password"
        };

        // Act
        var loginReponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Login, loginRequest);

        // Assert
        loginReponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);

        var content = await loginReponse.Content.ReadFromJsonAsync<FailureResponse>();
        content.ShouldNotBeNull();
        content.Errors.ShouldContain("Invalid username or password.");
    }
}