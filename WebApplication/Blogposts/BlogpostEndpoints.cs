using Microsoft.EntityFrameworkCore;

namespace WebsiteAPI;

public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this global::Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.MapGet("/blogposts", async (AppDbContext db) =>
            await db.Blogposts
                .Select(blogpost => new BlogpostResponse
                {
                    Id = blogpost.Id,
                    Publicatiedatum = blogpost.Publicatiedatum,
                    Inhoud = blogpost.Inhoud
                })
                .ToListAsync());

        app.MapGet("/blogposts/{id:int}", async (int id, AppDbContext db) =>
        {
            var blogpost = await db.Blogposts.FindAsync(id);

            if (blogpost is null)
                return Results.NotFound();

            return Results.Ok(new BlogpostResponse
            {
                Id = blogpost.Id,
                Publicatiedatum = blogpost.Publicatiedatum,
                Inhoud = blogpost.Inhoud
            });
        });

        app.MapPost("/blogposts", async (CreateBlogpostRequest request, AppDbContext db) =>
        {
            var blogpost = new Blogpost
            {
                Publicatiedatum = request.Publicatiedatum,
                Inhoud = request.Inhoud
            };

            db.Blogposts.Add(blogpost);
            await db.SaveChangesAsync();

            var response = new BlogpostResponse
            {
                Id = blogpost.Id,
                Publicatiedatum = blogpost.Publicatiedatum,
                Inhoud = blogpost.Inhoud
            };

            return Results.Created($"/blogposts/{blogpost.Id}", response);
        });

        app.MapPut("/blogposts/{id:int}", async (int id, UpdateBlogpostRequest request, AppDbContext db) =>
        {
            var blogpost = await db.Blogposts.FindAsync(id);

            if (blogpost is null)
                return Results.NotFound();

            blogpost.Publicatiedatum = request.Publicatiedatum;
            blogpost.Inhoud = request.Inhoud;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        app.MapDelete("/blogposts/{id:int}", async (int id, AppDbContext db) =>
        {
            var blogpost = await db.Blogposts.FindAsync(id);

            if (blogpost is null)
                return Results.NotFound();

            db.Blogposts.Remove(blogpost);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}