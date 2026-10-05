public static class MovieEndpoints
{
    private static readonly List<Movie> Movies =
    [
        new Movie { Id = 1, Title = "Inception", Year = 2010 },
        new Movie { Id = 2, Title = "Interstellar", Year = 2014 },
        new Movie { Id = 3, Title = "The Backrooms", Year = 2026},
        new Movie { Id = 4, Title = "Resident Evil", Year = 2026}
    ];

    public static void MapMovieEndpoints(this WebApplication app)
    {
        app.MapGet("/movies", () =>
            Movies.Select(movie => new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year
            }));

        app.MapPost("/movies", (CreateMovieRequest request) =>
        {
            var movie = new Movie
            {
                Id = Movies.Count + 1,
                Title = request.Title,
                Year = request.Year
            };

            Movies.Add(movie);

            var response = new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year
            };

            return Results.Created($"/movies/{movie.Id}", response);
        });
    }
}