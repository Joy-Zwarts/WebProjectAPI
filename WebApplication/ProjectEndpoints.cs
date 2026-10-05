using Microsoft.EntityFrameworkCore;

namespace WebsiteAPI;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this global::Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.MapGet("/projects", async (AppDbContext db) =>
            await db.Projects
                .Select(project => new ProjectResponse
                {
                    Id = project.Id,
                    Titel = project.Titel,
                    MediaUrl = project.MediaUrl,
                    MediaType = project.MediaType,
                    ThumbnailUrl = project.ThumbnailUrl,
                    CssClass = project.CssClass,
                    AltText = project.AltText,
                    Beschrijving = project.Beschrijving,
                    Tags = project.Tags,
                    Datum = project.Datum
                })
                .ToListAsync());

        app.MapGet("/projects/{id:int}", async (int id, AppDbContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
                return Results.NotFound();

            return Results.Ok(new ProjectResponse
            {
                Id = project.Id,
                Titel = project.Titel,
                MediaUrl = project.MediaUrl,
                MediaType = project.MediaType,
                ThumbnailUrl = project.ThumbnailUrl,
                CssClass = project.CssClass,
                AltText = project.AltText,
                Beschrijving = project.Beschrijving,
                Tags = project.Tags,
                Datum = project.Datum
            });
        });

        app.MapPost("/projects", async (CreateProjectRequest request, AppDbContext db) =>
        {
            var project = new Project
            {
                Titel = request.Titel,
                MediaUrl = request.MediaUrl,
                MediaType = request.MediaType,
                ThumbnailUrl = request.ThumbnailUrl,
                CssClass = request.CssClass,
                AltText = request.AltText,
                Beschrijving = request.Beschrijving,
                Tags = request.Tags,
                Datum = request.Datum
            };

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            var response = new ProjectResponse
            {
                Id = project.Id,
                Titel = project.Titel,
                MediaUrl = project.MediaUrl,
                MediaType = project.MediaType,
                ThumbnailUrl = project.ThumbnailUrl,
                CssClass = project.CssClass,
                AltText = project.AltText,
                Beschrijving = project.Beschrijving,
                Tags = project.Tags,
                Datum = project.Datum
            };

            return Results.Created($"/projects/{project.Id}", response);
        });

        app.MapPut("/projects/{id:int}", async (int id, UpdateProjectRequest request, AppDbContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
                return Results.NotFound();

            project.Titel = request.Titel;
            project.MediaUrl = request.MediaUrl;
            project.MediaType = request.MediaType;
            project.ThumbnailUrl = request.ThumbnailUrl;
            project.CssClass = request.CssClass;
            project.AltText = request.AltText;
            project.Beschrijving = request.Beschrijving;
            project.Tags = request.Tags;
            project.Datum = request.Datum;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        app.MapDelete("/projects/{id:int}", async (int id, AppDbContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
                return Results.NotFound();

            db.Projects.Remove(project);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}