// Sample - Web API to containerize with Docker
// Run locally:  dotnet run (from this src/ folder), then call GET /api/hello
// Run in Docker: see ../README.md section 5-6

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Reads an environment variable so we can show how `docker run -e` / compose
// `environment:` reach the app - defaults to "local" when not set.
var greetingSource = Environment.GetEnvironmentVariable("GREETING_SOURCE") ?? "local (no GREETING_SOURCE set)";

app.MapGet("/api/hello", () => Results.Ok(new
{
    message = "Hello from the Dockerized Web API!",
    source = greetingSource,
    machineName = Environment.MachineName, // inside a container, this is the container ID
}));

// Health check endpoint - used by the HEALTHCHECK instruction in the Dockerfile
// and by docker-compose's `healthcheck:`.
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
