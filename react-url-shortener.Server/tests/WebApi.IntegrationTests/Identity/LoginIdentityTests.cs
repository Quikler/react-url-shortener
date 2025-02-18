using System.Net.Http.Json;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.Identity;
using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Extensions;

namespace WebApi.IntegrationTests.Identity;

public class LoginIdentityTests(IntegrationTestWebApplicationFactory factory) : BaseIdentityTests(factory)
{
    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnAuthResponse()
    {
        // Arrange
        await this.SignupUserAsync(TestUsername, TestPassword);
        var loginRequest = CreateLoginRequest(TestUsername, TestPassword);

        // Act
        var loginResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Login, loginRequest);

        // Assert
        loginResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var content = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        content.ShouldNotBeNull();
        content.User.Username.ShouldBe(TestUsername);
    }

    [Fact]
    public async Task Login_WhenPasswordIsIncorrect_ShouldReturnUnauthorized()
    {
        // Arrange
        await this.SignupUserAsync(TestUsername, TestPassword);
        var loginRequest = CreateLoginRequest(TestUsername, TestPassword);
        loginRequest.Password = "incorrect_password";

        // Act
        var loginReponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Login, loginRequest);

        // Assert
        loginReponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);

        var content = await loginReponse.Content.ReadFromJsonAsync<FailureResponse>();
        content.ShouldNotBeNull();
        content.Errors.ShouldContain(InvalidUsernameOrPasswordMessage);
    }

    [Fact]
    public async Task Login_WhenUsernameIsNotFound_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = CreateLoginRequest(TestUsername, TestPassword);

        // Act
        var loginReponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Login, loginRequest);

        // Assert
        loginReponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);

        var content = await loginReponse.Content.ReadFromJsonAsync<FailureResponse>();
        content.ShouldNotBeNull();
        content.Errors.ShouldContain(InvalidUsernameOrPasswordMessage);
    }
}