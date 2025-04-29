// backend/Program.cs
using backend;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using QRCoder;
using System.Drawing.Imaging;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseStaticFiles(); // Serve media files
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// POST /api/auth/login
app.MapPost("/api/auth/login", async (LoginRequest request, AuthService authService) =>
{
    var token = await authService.Login(request.Email, request.Password);
    return token != null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
});

// GET /api/weddings
app.MapGet("/api/weddings", async (MongoDbContext db, HttpContext context) =>
{
    var plannerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var weddings = await db.WeddingStories.Find(w => w.PlannerId == plannerId).ToListAsync();
    return Results.Ok(weddings);
}).RequireAuthorization();

// POST /api/weddings
app.MapPost("/api/weddings", async (WeddingStory story, MongoDbContext db, HttpContext context) =>
{
    story.PlannerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    await db.WeddingStories.InsertOneAsync(story);
    return Results.Created($"/api/weddings/{story.Id}", story);
}).RequireAuthorization();

// GET /api/weddings/{id}
app.MapGet("/api/weddings/{id}", async (string id, MongoDbContext db) =>
{
    var story = await db.WeddingStories.Find(w => w.Id == id).FirstOrDefaultAsync();
    return story != null ? Results.Ok(story) : Results.NotFound();
});

// PUT /api/weddings/{id}
app.MapPut("/api/weddings/{id}", async (string id, WeddingStory updatedStory, MongoDbContext db, HttpContext context) =>
{
    updatedStory.Id = id;
    updatedStory.PlannerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var result = await db.WeddingStories.ReplaceOneAsync(w => w.Id == id && w.PlannerId == updatedStory.PlannerId, updatedStory);
    return result.ModifiedCount > 0 ? Results.Ok(updatedStory) : Results.NotFound();
}).RequireAuthorization();

// POST /api/media/upload
app.MapPost("/api/media/upload", async (IFormFile file, IWebHostEnvironment env) =>
{
    var uploadsDir = Path.Combine(env.WebRootPath, "media");
    Directory.CreateDirectory(uploadsDir);
    var filePath = Path.Combine(uploadsDir, $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    var fileUrl = $"/media/{Path.GetFileName(filePath)}";
    return Results.Ok(new { Url = fileUrl });
}).RequireAuthorization();

// POST /api/qrcodes/generate
app.MapPost("/api/qrcodes/generate", async (string storyId, MongoDbContext db, IWebHostEnvironment env) =>
{
    var story = await db.WeddingStories.Find(w => w.Id == storyId).FirstOrDefaultAsync();
    if (story == null) return Results.NotFound();

    var url = $"https://yourdomain.com/wedding/{storyId}";
     var qrGenerator = new QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
     var qrCode = new QRCodeService(qrCodeData);
     var bitmap = qrCode.GetGraphic(20);
    var qrCodePath = Path.Combine(env.WebRootPath, "media", $"qrcode-{storyId}.png");
    bitmap.Save(qrCodePath, ImageFormat.Png);

    var qrScan = new QRScan { StoryId = storyId, Url = $"/media/qrcode-{storyId}.png", Scans = 0 };
    await db.QRScans.InsertOneAsync(qrScan);

    story.QRCodeId = qrScan.Id;
    await db.WeddingStories.ReplaceOneAsync(w => w.Id == storyId, story);

    return Results.Ok(new { QRCodeUrl = qrScan.Url });
}).RequireAuthorization();

// POST /api/qrcodes/{id}/scans
app.MapPost("/api/qrcodes/{id}/scans", async (string id, MongoDbContext db) =>
{
    var qrCode = await db.QRScans.Find(q => q.Id == id).FirstOrDefaultAsync();
    if (qrCode == null) return Results.NotFound();

    qrCode.Scans++;
    await db.QRScans.ReplaceOneAsync(q => q.Id == id, qrCode);
    return Results.Ok(new { Scans = qrCode.Scans });
});

app.Run();