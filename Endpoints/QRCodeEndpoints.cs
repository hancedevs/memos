using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace backend.Endpoints
{
    public static class QRCodeEndpoints
    {
        public static void MapQRCodeEndpoints(this WebApplication app)
        {
            app.MapPost("/api/qrcodes/generate", async (Guid weddingId, MemoDbContext db, IWebHostEnvironment env) =>
            {

                var story = await db.Weddings.Where(w => w.Id == weddingId).FirstOrDefaultAsync();
                if (story == null) return Results.NotFound();

                var domain = app.Configuration["AppSettings:Domain"];
                var url = $"{domain}/wedding/{weddingId}";
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCodeService(qrCodeData);
                var bitmap = qrCode.GetGraphic(20);
                var qrCodePath = Path.Combine(env.WebRootPath, "media", $"qrcode-{weddingId}.png");
                bitmap.Save(qrCodePath, ImageFormat.Png);

                var qrScan = new WQRCode { WeddingId = weddingId, Url = $"/media/qrcode-{weddingId}.png", Scans = 0 };
                await db.QRCodes.AddAsync(qrScan);
                await db.SaveChangesAsync();

                return Results.Ok(new { QRCodeUrl = qrScan.Url });
            }).RequireAuthorization();


        }
    }
}