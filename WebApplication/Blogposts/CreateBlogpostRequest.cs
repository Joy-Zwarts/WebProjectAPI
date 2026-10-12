namespace WebsiteAPI;

public class CreateBlogpostRequest
{
    public DateTime Publicatiedatum { get; set; }
    public string Inhoud { get; set; } = string.Empty;
}