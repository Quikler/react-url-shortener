using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.Identity;
using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.IntegrationTests.Abstractions;
using WebApi.IntegrationTests.Extensions;

namespace WebApi.IntegrationTests.Identity;

public class SignupIdentityTests(IntegrationTestWebApplicationFactory factory) : BaseIdentityTests(factory)
{
    [Fact]
    public async Task Signup_WhenDataIsValid_ShouldSucceedAndAddUserToDatabase()
    {
        // Arrange
        var signupRequest = CreateSignupRequest(TestUsername, TestPassword, TestPassword);

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders);
        setCookieHeaders.ShouldNotBeNull();
        setCookieHeaders.Any(header => header.StartsWith("refreshToken=")).ShouldBeTrue();

        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();

        content.ShouldNotBeNull();
        content.User.Username.ShouldBe(TestUsername);

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == signupRequest.Username);
        user.ShouldNotBeNull();
    }

    [Fact]
    public async Task Signup_WhenPasswordsDoNotMatch_ShouldFailAndNotAddUserToDatabase()
    {
        // Arrange
        var signupRequest = CreateSignupRequest(TestUsername, TestPassword, "different_password");

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == signupRequest.Username);
        user.ShouldBeNull();
    }

    [Fact]
    public async Task Signup_WhenUsernameExist_ShouldFailAndNotAddUserToDatabase()
    {
        // Arrange
        await this.SignupUserAsync(TestUsername, TestPassword);
        var secondSignupRequest = CreateSignupRequest(TestUsername, TestPassword, TestPassword);

        // Act
        var secondSignupResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, secondSignupRequest);

        // Assert
        secondSignupResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);

        var user = await DbContext.Users.SingleOrDefaultAsync(u => u.UserName == secondSignupRequest.Username);
        user.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Signup_WhenUsernameIsInvalid_ShouldFailAndNotAddUserToDatabase(string username)
    {
        // Arrange
        var signupRequest = CreateSignupRequest(username, TestPassword, TestPassword);

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == signupRequest.Username);
        user.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("short")]
    [InlineData("1234567")]
    public async Task Signup_WhenPasswordIsInvalid_ShouldFailAndNotAddUserToDatabase(string password)
    {
        // Arrange
        var signupRequest = CreateSignupRequest(TestUsername, password, password);

        // Act
        var signupResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);

        // Assert
        signupResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == signupRequest.Username);
        user.ShouldBeNull();
    }
}