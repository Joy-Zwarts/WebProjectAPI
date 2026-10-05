var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string helloworld() => "Hello, World!";

app.MapGet("/hello", helloworld);


app.Run();