// Sample - Web API
// Run: dotnet run (from this src/ folder), then call GET /api/hello

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/hello", () => Results.Ok(new { message = "Hello from the sample Web API!" }));

app.Run();
