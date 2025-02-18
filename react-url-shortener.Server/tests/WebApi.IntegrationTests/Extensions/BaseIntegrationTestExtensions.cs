using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Shouldly;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.Identity;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.IntegrationTests.Abstractions;

namespace WebApi.IntegrationTests.Extensions;

public static class BaseIntegrationTestExtensions
{
    public static async Task<AuthResponse> SignupUserAsync(this BaseIntegrationTest baseIntegrationTest, string username, string password)
    {
        var signupRequest = new SignupRequest
        {
            Username = username,
            Password = password,
            ConfirmPassword = password,
        };

        var response = await baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, signupRequest);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var data = await response.Content.ReadFromJsonAsync<AuthResponse>();

        data.ShouldNotBeNull();
        return data;
    }

    public static async Task<AuthResponse> LoginUserAsync(this BaseIntegrationTest baseIntegrationTest, string username, string password)
    {
        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = password,
        };

        var response = await baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Login, loginRequest);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var data = await response.Content.ReadFromJsonAsync<AuthResponse>();

        data.ShouldNotBeNull();
        return data;
    }

    public static async Task SignupAndAuthenticateUserAsync(this BaseIntegrationTest baseIntegrationTest, string username, string password)
    {
        var authResponse = await baseIntegrationTest.SignupUserAsync(username, password);
        baseIntegrationTest.SetBearerToken(authResponse.Token);
    }

    public static async Task LoginAndAuthenticateUserAsync(this BaseIntegrationTest baseIntegrationTest, string username, string password)
    {
        var authResponse = await baseIntegrationTest.LoginUserAsync(username, password);
        baseIntegrationTest.SetBearerToken(authResponse.Token);
    }

    public static void SetBearerToken(this BaseIntegrationTest baseIntegrationTest, string token)
    {
        baseIntegrationTest.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void LogoutUser(this BaseIntegrationTest baseIntegrationTest)
    {
        baseIntegrationTest.HttpClient.DefaultRequestHeaders.Authorization = null;
    }
}