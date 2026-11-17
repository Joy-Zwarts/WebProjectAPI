using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApplication.UnitTests;

public class AuthHttpTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public AuthHttpTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterNieuweGebruiker_GeeftOkEnLoginWerkt()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var email = $"test-{Guid.NewGuid():N}@example.com";
        const string password = "Test123!";

        using var registerContent = new StringContent(
            JsonSerializer.Serialize(new
            {
                email,
                password
            }),
            Encoding.UTF8,
            "application/json");

        // Act
        var registerResponse = await client.PostAsync(
            "/auth/register",
            registerContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        using var loginContent = new StringContent(
            JsonSerializer.Serialize(new
            {
                email,
                password
            }),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await client.PostAsync(
            "/auth/login?useCookies=false&useSessionCookies=false",
            loginContent);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadAsStringAsync();
        using var loginDocument = JsonDocument.Parse(loginJson);

        Assert.Equal(
            "Bearer",
            loginDocument.RootElement.GetProperty("tokenType").GetString());

        Assert.False(
            string.IsNullOrWhiteSpace(
                loginDocument.RootElement.GetProperty("accessToken").GetString()));
    }
}
