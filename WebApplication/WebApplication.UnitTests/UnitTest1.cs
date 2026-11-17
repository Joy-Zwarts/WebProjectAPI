using Xunit;
using WebsiteAPI;

namespace WebApplication.UnitTests;

public class UnitTest1
{
    [Fact]
    public void Project_TitelWordtCorrectOpgeslagen()
    {
        // Arrange
        var project = new Project
        {
            Titel = "Testproject",
            Beschrijving = "Testbeschrijving",
            Datum = 20261116
        };

        // Act
        var titel = project.Titel;

        // Assert
        Assert.Equal("Testproject", titel);
    }

    [Fact]
    public void Blogpost_InhoudWordtCorrectOpgeslagen()
    {
        // Arrange
        var verwachteInhoud = "Testinhoud";
        var blogpost = new Blogpost
        {
            Publicatiedatum = new DateTime(2026, 11, 16),
            Inhoud = verwachteInhoud
        };

        // Act
        var inhoud = blogpost.Inhoud;

        // Assert
        Assert.Equal(verwachteInhoud, inhoud);
    }

    [Theory]
    [InlineData("Eerste project")]
    [InlineData("Tweede project")]
    public void Project_VerschillendeTitels_WordtJuisteTitel(string verwachteTitel)
    {
        // Arrange
        var project = new Project
        {
            Titel = verwachteTitel
        };

        // Act
        var werkelijkeTitel = project.Titel;

        // Assert
        Assert.Equal(verwachteTitel, werkelijkeTitel);
    }
}