using WebsiteAPI;
using Microsoft.EntityFrameworkCore;

var builder = global::Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebsiteDb")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapProjectEndpoints();

app.Run();