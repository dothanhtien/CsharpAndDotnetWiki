# Bài giảng mẫu - Web API

> Đây là bài giảng **mẫu** minh hoạ cấu trúc bài giảng đi kèm project ASP.NET Core Web API. Sao chép folder này để tạo bài giảng mới.

## Mục tiêu

- Hiểu cấu trúc một project minimal API.
- Biết cách chạy và test API bằng `curl` hoặc file `.http`.

## Cấu trúc folder

```
00-web-api-template/
├── README.md
└── src/
    ├── WebApiTemplate.csproj
    ├── Program.cs
    └── appsettings.json
```

## Chạy thử project

```bash
cd lectures/00-web-api-template/src
dotnet run
```

Sau đó gọi thử:

```bash
curl http://localhost:5000/api/hello
```

## Nội dung bài giảng

Minimal API trong [src/Program.cs](./src/Program.cs) định nghĩa một endpoint đơn giản:

```csharp
app.MapGet("/api/hello", () => Results.Ok(new { message = "Xin chào từ Web API mẫu!" }));
```

## Ghi chú

- Thêm entry tương ứng vào [lectures.json](../../lectures.json) ở thư mục gốc để bài giảng xuất hiện trên sidebar.
- Có thể đổi `id`/`dir` khi sao chép folder, miễn khớp với tên folder thực tế.
