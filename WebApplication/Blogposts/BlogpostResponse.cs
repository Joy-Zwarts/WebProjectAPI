namespace WebsiteAPI;

public class BlogpostResponse
{
    public int Id { get; set; }
    public DateTime Publicatiedatum { get; set; }
    public string Inhoud { get; set; } = string.Empty;
}