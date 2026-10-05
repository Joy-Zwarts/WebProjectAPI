using MovieCatalog;
using Microsoft.EntityFrameworkCore;

var builder = global::Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MovieDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MovieLab")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapMovieEndpoints();

app.Run();