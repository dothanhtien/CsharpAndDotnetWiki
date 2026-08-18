# Template Lecture - Web API

> This is a **template** lecture illustrating a lecture folder paired with an ASP.NET Core Web API project. Copy this folder to start a new lecture.

## Goals

- Understand the structure of a minimal API project.
- Know how to run and test the API with `curl` or an `.http` file.

## Folder structure

```
00-web-api-template/
├── README.md                   # Lecture content (English) - required, this is the default
├── README.vi.md                # Vietnamese translation (optional)
└── src/
    ├── WebApiTemplate.csproj
    ├── Program.cs
    └── appsettings.json
```

## Running the project

```bash
cd lectures/00-web-api-template/src
dotnet run
```

Then try:

```bash
curl http://localhost:5000/api/hello
```

## Lecture content

The minimal API in [src/Program.cs](./src/Program.cs) defines a single simple endpoint:

```csharp
app.MapGet("/api/hello", () => Results.Ok(new { message = "Hello from the sample Web API!" }));
```

## Notes

- Add a matching entry to [lectures.json](../../lectures.json) at the repo root so the lecture shows up in the sidebar.
- Feel free to rename `id`/`dir` when copying the folder, as long as they match the actual folder name.
