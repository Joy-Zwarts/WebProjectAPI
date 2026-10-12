namespace WebsiteAPI;

public class Blogpost
{
    public int Id { get; set; }
    public DateTime Publicatiedatum { get; set; }
    public string Inhoud { get; set; } = string.Empty;
}