using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.Identity;
using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.IntegrationTests.Abstractions;

namespace WebApi.IntegrationTests.Identity;

public class SignupIdentityTests(IntegrationTestWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Signup_WithValidData_SucceedsAndAddsUserToDatabase()
    {
        // Arrange
        var request = new SignupRequest
        {
            Username = "test",
            Password = "testtest",
            ConfirmPassword = "testtest",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();

        content.ShouldNotBeNull();
        content.Token.ShouldNotBeNull();
        content.User.Username.ShouldBe("test");

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
        user.ShouldNotBeNull();
    }

    [Fact]
    public async Task Signup_WithNotMatchingPasswords_FailsAndNotAddsUserToDatabase()
    {
        // Arrange
        var request = new SignupRequest
        {
            Username = "test",
            Password = "different_password",
            ConfirmPassword = "testtest",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<FailureResponse>();

        content.ShouldNotBeNull();
        content.Errors.ShouldContain("Passwords do not match.");

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
        user.ShouldBeNull();
    }

    [Fact]
    public async Task Signup_WithEmptyUsername_FailsAndNotAddsUserToDatabase()
    {
        // Arrange
        var request = new SignupRequest
        {
            Username = "",
            Password = "testtest",
            ConfirmPassword = "testtest",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<FailureResponse>();

        content.ShouldNotBeNull();
        content.Errors.ShouldContain("Username '' is invalid, can only contain letters or digits.");

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
        user.ShouldBeNull();
    }

    [Fact]
    public async Task Signup_WithExistingUsername_FailsAndNotAddsUserToDatabase()
    {
        // Arrange
        var firstSignupRequest = new SignupRequest
        {
            Username = "test",
            Password = "testtest",
            ConfirmPassword = "testtest",
        };

        var firstSignupResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, firstSignupRequest);
        firstSignupResponse.EnsureSuccessStatusCode();

        var secondSignupRequest = new SignupRequest
        {
            Username = "test",
            Password = "testtest",
            ConfirmPassword = "testtest",
        };

        // Act
        var secondSignupResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, secondSignupRequest);

        // Assert
        secondSignupResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);
        var content = await secondSignupResponse.Content.ReadFromJsonAsync<FailureResponse>();

        content.ShouldNotBeNull();
        content.Errors.ShouldContain("Username already exist.");

        var user = await DbContext.Users.SingleOrDefaultAsync(u => u.UserName == secondSignupRequest.Username);
        user.ShouldNotBeNull();
    }

    [Fact]
    public async Task Signup_WithShortPassword_FailsAndNotAddsUserToDatabase()
    {
        // Arrange
        var signupRequest = new SignupRequest
        {
            Username = "test",
            Password = "test",
            ConfirmPassword = "test",
        };

        // Act
        var secondSignupResponse = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);

        // Assert
        secondSignupResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        var content = await secondSignupResponse.Content.ReadFromJsonAsync<FailureResponse>();

        content.ShouldNotBeNull();
        content.Errors.ShouldContain("Passwords must be at least 8 characters.");

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == signupRequest.Username);
        user.ShouldBeNull();
    }
}