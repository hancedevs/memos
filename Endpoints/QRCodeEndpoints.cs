using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SkiaSharp;

namespace backend.Endpoints
{
    public static class QRCodeEndpoints
    {
        public static void MapQRCodeEndpoints(this WebApplication app)
        {
            app.MapGet("/api/qrcodes/generate", async (Guid weddingId, MemoDbContext db, IWebHostEnvironment env) =>
            {
                var story = await db.Weddings.Where(w => w.Id == weddingId).FirstOrDefaultAsync();
                if (story == null) return Results.NotFound();

                var domain = app.Configuration["Domain"];
                var url = $"{domain}/story/{weddingId}";
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCodeService(qrCodeData);
                // Generate QR code using SkiaSharp
                using var bitmap = qrCode.GetGraphic(20); // Default: black on white, 20 pixels per module

                // Ensure the media directory exists
                var mediaPath = Path.Combine(env.WebRootPath, "qrcodes");
                Directory.CreateDirectory(mediaPath); // Create if it doesn’t exist

                // Save the QR code as PNG
                var qrCodePath = Path.Combine(mediaPath, $"qrcode-{weddingId}.png");
                var fileUrl = $"/qrcodes/qrcode-{weddingId}.png";
                using (var stream = new FileStream(qrCodePath, FileMode.Create, FileAccess.Write))
                {
                    bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
                }
                var qrScan = new WQRCode { WeddingId = weddingId, Url = url, Scans = 0, AssetUrl = fileUrl};
                await db.QRCodes.AddAsync(qrScan);
                await db.SaveChangesAsync();

                return Results.Ok(new { url = qrScan.Url, assetUrl = fileUrl });
            });
            app.MapGet("/api/qrcodes/{id}", async (Guid id, MemoDbContext db) =>
            {
                var qrCode = await db.QRCodes.FirstOrDefaultAsync(q => q.Id == id);
                return qrCode != null ? Results.Ok(qrCode) : Results.NotFound();
            });
        }
        
    }
}