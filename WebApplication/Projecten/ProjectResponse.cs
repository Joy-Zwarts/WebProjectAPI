namespace WebsiteAPI;

public class ProjectResponse
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? CssClass { get; set; }
    public string? AltText { get; set; }
    public string Beschrijving { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public int Datum { get; set; }
}