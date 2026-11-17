using WebsiteAPI;
using Microsoft.EntityFrameworkCore;

var builder = global::Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Bearer",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Vul in: Bearer gevolgd door je access token."
    });

    options.AddSecurityRequirement(document => new()
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebsiteDb")));

builder.Services.AddIdentityApiEndpoints<User>()
    .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanEditProjects", policy =>
        policy.RequireRole("Editor", "Admin"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/auth").MapIdentityApi<User>();

app.MapProjectEndpoints();
app.MapBlogpostEndpoints();

await DbSeeder.SeedAsync(app.Services);

app.Run();

public partial class Program
{
}