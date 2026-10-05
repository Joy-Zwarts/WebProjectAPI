using System.ComponentModel.DataAnnotations;

namespace MovieCatalog;

public class CreateMovieRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Range(1888, 2100)]
    public int Year { get; set; }
}