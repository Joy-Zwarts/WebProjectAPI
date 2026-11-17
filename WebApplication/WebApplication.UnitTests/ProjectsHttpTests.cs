using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WebApplication.UnitTests;

public class ProjectsHttpTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public ProjectsHttpTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProjects_Publiek_GeeftSuccesvolleResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProject_OnbekendId_GeeftNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        const int onbekendProjectId = 999999;

        // Act
        var response = await client.GetAsync($"/projects/{onbekendProjectId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProject_BestaandId_GeeftOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");
        var bestaandProjectId = await MaakProjectAsync(client);

        // Act
        var response = await client.GetAsync($"/projects/{bestaandProjectId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_ZonderAanmelding_GeeftUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        using var content = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/projects", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_ZonderAanmelding_GeeftUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        using var content = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/projects/999999", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_ZonderAanmelding_GeeftUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_AangemeldZonderRol_GeeftForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var ordinaryUser = await RegisterOrdinaryUserAsync(client);
        await LoginAsync(client, ordinaryUser.Email, ordinaryUser.Password);

        using var content = new StringContent(
            "{\"titel\":\"Gewone gebruiker test\",\"beschrijving\":\"Test\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/projects", content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_Editor_GeeftCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");

        using var content = new StringContent(
            "{\"titel\":\"Editor integratietest\",\"beschrijving\":\"Project aangemaakt door Editor\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/projects", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Editor integratietest", document.RootElement.GetProperty("titel").GetString());
    }

    [Fact]
    public async Task PostProject_Admin_GeeftCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "admin@websiteapi.com", "Admin123!");

        using var content = new StringContent(
            "{\"titel\":\"Admin integratietest\",\"beschrijving\":\"Project aangemaakt door Admin\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/projects", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Admin integratietest", document.RootElement.GetProperty("titel").GetString());
    }

    [Fact]
    public async Task PutProject_Editor_GeeftNoContent()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");
        var projectId = await MaakProjectAsync(client);

        using var content = new StringContent(
            "{\"titel\":\"Editor update test\",\"beschrijving\":\"Project bijgewerkt door Editor\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PutAsync($"/projects/{projectId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_Admin_GeeftNoContent()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "admin@websiteapi.com", "Admin123!");
        var projectId = await MaakProjectAsync(client);

        using var content = new StringContent(
            "{\"titel\":\"Admin update test\",\"beschrijving\":\"Project bijgewerkt door Admin\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PutAsync($"/projects/{projectId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_Editor_GeeftNoContent()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");
        var projectId = await MaakProjectAsync(client);

        // Act
        var response = await client.DeleteAsync($"/projects/{projectId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_Editor_OngeldigeTitel_GeeftBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");
        using var content = new StringContent(
            "{\"titel\":\"\",\"beschrijving\":\"Ongeldige test\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/projects", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutProject_Editor_WijzigingIsDaarnaZichtbaar()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");
        var projectId = await MaakProjectAsync(client);
        using var content = new StringContent(
            "{\"titel\":\"Project na update\",\"beschrijving\":\"Nieuw\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        // Act
        var updateResponse = await client.PutAsync($"/projects/{projectId}", content);
        var getResponse = await client.GetAsync($"/projects/{projectId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        using var document = JsonDocument.Parse(await getResponse.Content.ReadAsStringAsync());
        Assert.Equal("Project na update", document.RootElement.GetProperty("titel").GetString());
    }

    [Fact]
    public async Task DeleteProject_Editor_VerwijderdProjectGeeftNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await LoginAsync(client, "editor@websiteapi.com", "Editor123!");
        var projectId = await MaakProjectAsync(client);

        // Act
        var deleteResponse = await client.DeleteAsync($"/projects/{projectId}");
        var getResponse = await client.GetAsync($"/projects/{projectId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<int> MaakProjectAsync(HttpClient client)
    {
        using var content = new StringContent(
            "{\"titel\":\"Project voor test\",\"beschrijving\":\"Testproject\",\"datum\":20261117}",
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/projects", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("id").GetInt32();
    }

    private static async Task<(string Email, string Password)> RegisterOrdinaryUserAsync(HttpClient client)
    {
        var email = $"ordinary-{Guid.NewGuid():N}@example.com";
        const string password = "Ordinary123!";

        using var content = new StringContent(
            JsonSerializer.Serialize(new { email, password }),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/auth/register", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return (email, password);
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

        using var document = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        var accessToken = document.RootElement.GetProperty("accessToken").GetString();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
