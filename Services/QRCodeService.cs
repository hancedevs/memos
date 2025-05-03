using Google.Protobuf.WellKnownTypes;
using QRCoder;
using SkiaSharp;
using System;
using System.Drawing;

namespace backend.Services
{
    public class QRCodeService : AbstractQRCode, IDisposable
    {
        public QRCodeService()
        {
        }

        public QRCodeService(QRCodeData data)
            : base(data)
        {
        }

        public SKBitmap GetGraphic(int pixelsPerModule)
        {
            return GetGraphic(pixelsPerModule, SKColors.Black, SKColors.White, drawQuietZones: true);
        }

        public SKBitmap GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true)
        {
            // Parse HTML hex colors to SkiaSharp SKColor
            SKColor darkColor = SKColor.Parse(darkColorHtmlHex);
            SKColor lightColor = SKColor.Parse(lightColorHtmlHex);
            return GetGraphic(pixelsPerModule, darkColor, lightColor, drawQuietZones);
        }

        public SKBitmap GetGraphic(int pixelsPerModule, SKColor darkColor, SKColor lightColor, bool drawQuietZones = true)
        {
            int matrixSize = base.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8);
            int size = matrixSize * pixelsPerModule;
            int offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            // Create a new SkiaSharp bitmap
            var bitmap = new SKBitmap(size, size);
            using var canvas = new SKCanvas(bitmap);

            // Fill the background with light color
            canvas.Clear(lightColor);

            // Create paints for dark and light colors
            using var darkPaint = new SKPaint { Color = darkColor, Style = SKPaintStyle.Fill };
            using var lightPaint = new SKPaint { Color = lightColor, Style = SKPaintStyle.Fill };

            // Draw QR code modules
            for (int i = 0; i < size + offset; i += pixelsPerModule)
            {
                for (int j = 0; j < size + offset; j += pixelsPerModule)
                {
                    bool isDark = base.QrCodeData.ModuleMatrix[(j + pixelsPerModule) / pixelsPerModule - 1][(i + pixelsPerModule) / pixelsPerModule - 1];
                    var paint = isDark ? darkPaint : lightPaint;
                    canvas.DrawRect(i - offset, j - offset, pixelsPerModule, pixelsPerModule, paint);
                }
            }
            return bitmap;
        }

        public SKBitmap GetGraphic(int pixelsPerModule, SKColor darkColor, SKColor lightColor, SKBitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 0, bool drawQuietZones = true, SKColor? iconBackgroundColor = null)
        {
            int matrixSize = base.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8);
            int size = matrixSize * pixelsPerModule;
            int offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            // Create a new SkiaSharp bitmap
            var bitmap = new SKBitmap(size, size);
            using var canvas = new SKCanvas(bitmap);

            // Fill the background with light color
            canvas.Clear(lightColor);

            // Create paints for dark and light colors
            using var darkPaint = new SKPaint { Color = darkColor, Style = SKPaintStyle.Fill };
            using var lightPaint = new SKPaint { Color = lightColor, Style = SKPaintStyle.Fill };

            // Draw QR code modules
            for (int i = 0; i < size + offset; i += pixelsPerModule)
            {
                for (int j = 0; j < size + offset; j += pixelsPerModule)
                {
                    bool isDark = base.QrCodeData.ModuleMatrix[(j + pixelsPerModule) / pixelsPerModule - 1][(i + pixelsPerModule) / pixelsPerModule - 1];
                    var paint = isDark ? darkPaint : lightPaint;
                    canvas.DrawRect(i - offset, j - offset, pixelsPerModule, pixelsPerModule, paint);
                }
            }

            // Draw icon if provided
            bool hasIcon = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;
            if (hasIcon)
            {
                float iconWidth = (iconSizePercent * size) / 100f;
                float iconHeight = iconWidth * icon.Height / icon.Width;
                float x = (size - iconWidth) / 2f;
                float y = (size - iconHeight) / 2f;

                // Draw icon background with border (if specified)
                if (iconBorderWidth > 0)
                {
                    var backgroundColor = iconBackgroundColor ?? lightColor;
                    using var borderPaint = new SKPaint { Color = backgroundColor, Style = SKPaintStyle.Fill };
                    var borderRect = new SKRect(x - iconBorderWidth, y - iconBorderWidth, x + iconWidth + iconBorderWidth, y + iconHeight + iconBorderWidth);
                    using var path = CreateRoundedRectanglePath(borderRect, iconBorderWidth * 2);
                    canvas.DrawPath(path, borderPaint);
                }

                // Resize and draw the icon
                using var resizedIcon = ResizeBitmap(icon, (int)iconWidth, (int)iconHeight);
                canvas.DrawBitmap(resizedIcon, x, y);
            }
            return bitmap;
        }

        internal SKPath CreateRoundedRectanglePath(SKRect rect, int cornerRadius)
        {
            var path = new SKPath();
            float x = rect.Left;
            float y = rect.Top;
            float width = rect.Width;
            float height = rect.Height;
            float diameter = cornerRadius;
            float radius = cornerRadius / 2f;

            // Top-left arc
            path.AddArc(new SKRect(x, y, x + diameter, y + diameter), 180, 90);

            // Top line
            path.LineTo(x + width - radius, y);

            // Top-right arc
            path.AddArc(new SKRect(x + width - diameter, y, x + width, y + diameter), 270, 90);

            // Right line
            path.LineTo(x + width, y + height - radius);

            // Bottom-right arc
            path.AddArc(new SKRect(x + width - diameter, y + height - diameter, x + width, y + height), 0, 90);

            // Bottom line
            path.LineTo(x + radius, y + height);

            // Bottom-left arc
            path.AddArc(new SKRect(x, y + height - diameter, x + diameter, y + height), 90, 90);

            // Left line
            path.LineTo(x, y + radius);
            path.Close();
            return path;
        }

        private SKBitmap ResizeBitmap(SKBitmap source, int width, int height)
        {
            var resizedBitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(resizedBitmap);
            canvas.Clear(SKColors.Transparent);

            float scaleX = (float)width / source.Width;
            float scaleY = (float)height / source.Height;
            canvas.Scale(scaleX, scaleY);
            canvas.DrawBitmap(source, 0, 0);

            return resizedBitmap;
        }

        public void Dispose()
        {
            base.QrCodeData?.Dispose();
        }
    }
}