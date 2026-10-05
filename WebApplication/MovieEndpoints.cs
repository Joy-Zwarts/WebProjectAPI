namespace MovieCatalog;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this global::Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.MapGet("/movies", async (MovieDb db) =>
            await db.Movies
                .Select(movie => new MovieResponse
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Year = movie.Year
                })
                .ToListAsync());

        app.MapGet("/movies/{id:int}", async (int id, MovieDb db) =>
        {
            var movie = await db.Movies.FindAsync(id);

            if (movie is null)
                return Results.NotFound();

            return Results.Ok(new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year
            });
        });

        app.MapPost("/movies", async (CreateMovieRequest request, MovieDb db) =>
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);

            bool isValid = Validator.TryValidateObject(
                request,
                validationContext,
                validationResults,
                true);

            if (!isValid)
            {
                return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
            }

            var movie = new Movie
            {
                Title = request.Title,
                Year = request.Year
            };

            db.Movies.Add(movie);
            await db.SaveChangesAsync();

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