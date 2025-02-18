using System.Net.Http.Headers;
using System.Net.Http.Json;
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

    public static async Task AuthenticateUserAsync(this BaseIntegrationTest baseIntegrationTest, string username, string password)
    {
        var authResponse = await baseIntegrationTest.SignupUserAsync(username, password);
        baseIntegrationTest.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);
    }

    public static void LogoutUser(this BaseIntegrationTest baseIntegrationTest)
    {
        baseIntegrationTest.HttpClient.DefaultRequestHeaders.Authorization = null;
    }
}