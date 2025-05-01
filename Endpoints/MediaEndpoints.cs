using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dto;
using Microsoft.AspNetCore.Mvc;

namespace backend.Endpoints
{
    public  static class MediaEndpoints
{
    public static void MapMediaEndpoints(this WebApplication app)
    {
        app.MapPost("/api/media/upload",async ([FromForm] MediaFileDto file, IWebHostEnvironment env) =>
        {
            if (file.File.Length > 50 * 1024 * 1024) // 50MB limit
                return Results.BadRequest("File too large.");

            var uploadsDir = Path.Combine(env.WebRootPath, "media");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.File.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.File.CopyToAsync(stream);
            }
            var fileUrl = $"/media/{fileName}";
            return Results.Ok(new { Url = fileUrl });
        }).DisableAntiforgery();
    }
}
public class IgnoreAntiforgeryTokenFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return await next(context);
    }
}
}