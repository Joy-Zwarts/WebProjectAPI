using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApplication.UnitTests;

public class BlogpostsHttpTests : IClassFixture<TestWebApplicationFactory>
{
   private readonly TestWebApplicationFactory _factory;

    public BlogpostsHttpTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetBlogposts_Publiek_GeeftSuccesvolleResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBlogpost_OnbekendId_GeeftNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        const int onbekendBlogpostId = 999999;

        // Act
        var response = await client.GetAsync($"/blogposts/{onbekendBlogpostId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBlogpost_BestaandId_GeeftOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var lijstResponse = await client.GetAsync("/blogposts");
        Assert.Equal(HttpStatusCode.OK, lijstResponse.StatusCode);

        var lijstJson = await lijstResponse.Content.ReadAsStringAsync();
        using var lijstDocument = JsonDocument.Parse(lijstJson);
        var bestaandBlogpostId = lijstDocument.RootElement
            .EnumerateArray()
            .First()
            .GetProperty("id")
            .GetInt32();

        // Act
        var response = await client.GetAsync($"/blogposts/{bestaandBlogpostId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostBlogpost_ZonderAanmelding_GeeftUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        using var content = new StringContent(
            "{\"publicatiedatum\":\"2026-11-17T00:00:00Z\",\"inhoud\":\"Testblogpost\"}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/blogposts", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogpost_ZonderAanmelding_GeeftUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        using var content = new StringContent(
            "{\"publicatiedatum\":\"2026-11-17T00:00:00Z\",\"inhoud\":\"Aangepaste testblogpost\"}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PutAsync("/blogposts/999999", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogpost_ZonderAanmelding_GeeftUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostBlogpost_Editor_GeeftCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");

        using var content = new StringContent(
            "{\"publicatiedatum\":\"2026-11-17T00:00:00Z\",\"inhoud\":\"Blogpost aangemaakt door Editor\"}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/blogposts", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseJson = await response.Content.ReadAsStringAsync();
        using var responseDocument = JsonDocument.Parse(responseJson);

        Assert.Equal(
            "Blogpost aangemaakt door Editor",
            responseDocument.RootElement.GetProperty("inhoud").GetString());
    }

    [Fact]
    public async Task PutBlogpost_Editor_GeeftNoContent()
    {
        // Arrange
        var testData = await MaakBlogpostAlsEditorAsync();
        using var client = testData.Client;
        var blogpostId = testData.BlogpostId;

        using var content = new StringContent(
            "{\"publicatiedatum\":\"2026-11-17T00:00:00Z\",\"inhoud\":\"Blogpost bijgewerkt door Editor\"}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PutAsync($"/blogposts/{blogpostId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogpost_Editor_GeeftNoContent()
    {
        // Arrange
        var testData = await MaakBlogpostAlsEditorAsync();
        using var client = testData.Client;
        var blogpostId = testData.BlogpostId;

        // Act
        var response = await client.DeleteAsync($"/blogposts/{blogpostId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PostBlogpost_Editor_OngeldigeInhoud_GeeftBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");
        using var content = new StringContent(
            "{\"publicatiedatum\":\"2026-11-17T00:00:00Z\",\"inhoud\":\"\"}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/blogposts", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutBlogpost_Editor_WijzigingIsDaarnaZichtbaar()
    {
        // Arrange
        var testData = await MaakBlogpostAlsEditorAsync();
        using var client = testData.Client;
        var blogpostId = testData.BlogpostId;
        using var content = new StringContent(
            "{\"publicatiedatum\":\"2026-11-17T00:00:00Z\",\"inhoud\":\"Blogpost na update\"}",
            Encoding.UTF8,
            "application/json");

        // Act
        var updateResponse = await client.PutAsync($"/blogposts/{blogpostId}", content);
        var getResponse = await client.GetAsync($"/blogposts/{blogpostId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        using var updatedDocument = JsonDocument.Parse(await getResponse.Content.ReadAsStringAsync());
        Assert.Equal("Blogpost na update", updatedDocument.RootElement.GetProperty("inhoud").GetString());
    }

    [Fact]
    public async Task DeleteBlogpost_Editor_VerwijderdBlogpostGeeftNotFound()
    {
        // Arrange
        var testData = await MaakBlogpostAlsEditorAsync();
        using var client = testData.Client;
        var blogpostId = testData.BlogpostId;

        // Act
        var deleteResponse = await client.DeleteAsync($"/blogposts/{blogpostId}");
        var getResponse = await client.GetAsync($"/blogposts/{blogpostId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<(HttpClient Client, int BlogpostId)> MaakBlogpostAlsEditorAsync()
    {
        var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");

        using var content = new StringContent(
            "{\"publicatiedatum\":\"2026-11-17T00:00:00Z\",\"inhoud\":\"Blogpost voor PUT of DELETE\"}",
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/blogposts", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseJson = await response.Content.ReadAsStringAsync();
        using var responseDocument = JsonDocument.Parse(responseJson);
        var blogpostId = responseDocument.RootElement.GetProperty("id").GetInt32();

        return (client, blogpostId);
    }

    private static async Task LoginAsync(HttpClient client, string email, string password)
    {
        using var loginContent = new StringContent(
            JsonSerializer.Serialize(new { email, password }),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await client.PostAsync(
            "/auth/login?useCookies=false&useSessionCookies=false",
            loginContent);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadAsStringAsync();
        using var loginDocument = JsonDocument.Parse(loginJson);
        var accessToken = loginDocument.RootElement
            .GetProperty("accessToken")
            .GetString();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
