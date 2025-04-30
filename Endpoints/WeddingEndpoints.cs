// Endpoints/WeddingEndpoints.cs
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using backend.Dto;
using backend;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

public static class WeddingEndpoints
{
    public static void MapWeddingEndpoints(this WebApplication app)
    {
        app.MapGet("/api/weddings", async (MemoDbContext db, HttpContext context) =>
        {
            var plannerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var weddings = await db.Weddings
                .Where(w => w.PlannerId == plannerId)
                .Include(w => w.Gallery)
                .Include(w => w.QRCode)
                .ToListAsync();
            return Results.Ok(weddings);
        }).RequireAuthorization();

        app.MapPost("/api/weddings", async ([FromBody] WeddingCreateDto story, MemoDbContext db, HttpContext context, FileStorageService fileStorageService) =>
        {
            foreach (var file in story.MediaFiles)
            {

                var fileType = file.ContentType.StartsWith("image/") ? "image" :
                               file.ContentType.StartsWith("video/") ? "video" : "unknown";
                if (fileType == "unknown")
                {
                    return Results.BadRequest("Unsupported file type.");
                }

            }
            story.PlannerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(story.CoupleName) || string.IsNullOrWhiteSpace(story.PlannerId))
                return Results.BadRequest("Couple name and planner ID are required.");
            var weddingStory = new WeddingStory
            {
                CoupleName = story.CoupleName,
                PlannerId = story.PlannerId,
                ThankYouMessage = story.ThankYouMessage,
                Proposal = story.Proposal,
                HowWeMet = story.HowWeMet,
            };
            await db.Weddings.AddAsync(weddingStory);
            await db.SaveChangesAsync();
            var medias = new List<Media>();
            foreach (var file in story.MediaFiles)
            {
                var result = await fileStorageService.SaveFileAsync(file, weddingStory.Id.ToString());
                if (result != null)
                {
                    var fileType = file.ContentType.StartsWith("image/") ? "image" :
                                   file.ContentType.StartsWith("video/") ? "video" : "unknown";
                    if (fileType == "unknown")
                    {
                        return Results.BadRequest("Unsupported file type.");
                    }
                    medias.Add(new Media { Url = result, Type = fileType, WeddingId = weddingStory.Id });
                }
            }
            if (medias != null && medias.Count > 0)
            {
                foreach (var media in medias)
                {

                    await db.Media.AddAsync(media);
                }
                await db.SaveChangesAsync();
            }
            return Results.Created($"/api/weddings/{weddingStory.Id}", story);
        }).RequireAuthorization();

        app.MapGet("/api/weddings/{id}", async (Guid id, MemoDbContext db) =>
        {
            var story = await db.Weddings
                .Include(w => w.Gallery)
                .Include(w => w.QRCode)
                .FirstOrDefaultAsync(w => w.Id == id);
            return story != null ? Results.Ok(story) : Results.NotFound();
        }).WithResponseCache(3600);

        app.MapPut("/api/weddings/{id}", async (Guid id, [FromBody] WeddingStory updatedStory, MemoDbContext db, HttpContext context) =>
        {
            updatedStory.Id = id;
            updatedStory.PlannerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingStory = await db.Weddings.FirstOrDefaultAsync(w => w.Id == id && w.PlannerId == updatedStory.PlannerId);
            if (existingStory == null) return Results.NotFound();

            existingStory.CoupleName = updatedStory.CoupleName;
            existingStory.PlannerId = updatedStory.PlannerId;
            existingStory.ThankYouMessage = updatedStory.ThankYouMessage;
            existingStory.Proposal = updatedStory.Proposal;
            existingStory.HowWeMet = updatedStory.HowWeMet;

            db.Weddings.Update(existingStory);
            await db.SaveChangesAsync();
            return Results.Ok(existingStory);
        }).RequireAuthorization();
    }

    private static RouteHandlerBuilder WithResponseCache(this RouteHandlerBuilder builder, int duration)
    {
        return duration > 0
            ? builder.AddEndpointFilter(async (context, next) =>
            {
                context.HttpContext.Response.GetTypedHeaders().CacheControl = new()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(duration)
                };
                return await next(context);
            })
            : builder;
    }
}