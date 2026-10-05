public static class MovieEndpoints
{
    private static readonly List<Movie> Movies =
    [
        new Movie { Id = 1, Title = "Inception", Genre = "Sci-Fi", Year = 2010 },
        new Movie { Id = 2, Title = "The Dark Knight", Genre = "Action", Year = 2008 },
        new Movie { Id = 3, Title = "Interstellar", Genre = "Sci-Fi", Year = 2014 }
    ];

    public static void MapMovieEndpoints(this WebApplication app)
    {
        app.MapGet("/movies", () => Movies);

        app.MapGet("/movies/{id:int}", (int id) =>
        {
            var movie = Movies.FirstOrDefault(movie => movie.Id == id);
            if (movie is not null)
            {
                return Results.Ok(movie);
            }
            else
            {
                return Results.NotFound();
            }
        });

        app.MapPost("/movies", (Movie movie) =>
        {
            movie.Id = Movies.Count + 1;
            Movies.Add(movie);
            return Results.Created($"/movies/{movie.Id}", movie);
        });
    }
}