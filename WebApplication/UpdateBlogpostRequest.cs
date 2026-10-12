namespace WebsiteAPI;

public class UpdateBlogpostRequest
{
    public DateTime Publicatiedatum { get; set; }
    public string Inhoud { get; set; } = string.Empty;
}