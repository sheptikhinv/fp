using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using TagsCloudContainer.Core.DTO;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Core.CloudRenderers;

public class BasicCloudRenderer : ICloudRenderer
{
    private readonly Random _random;

    public BasicCloudRenderer()
    {
        _random = new Random();
    }
    
    public Result<Bitmap> RenderCloud(List<WordLayout> wordLayouts, VisualizationOptions visualizationOptions)
    {
        var bitmap = CalculateCloudBounds(wordLayouts).AsResult()
            .Then(bounds => CreateBitmapForCloud(wordLayouts, visualizationOptions.ImageWidthPx,
                visualizationOptions.ImageHeightPx, visualizationOptions.Padding, bounds))
            .Then(bitmap => DrawWordsOnBitmap(bitmap, wordLayouts, visualizationOptions));
        return bitmap;
    }

    private Result<Bitmap> CreateBitmapForCloud(List<WordLayout> wordLayouts, int? imageWidthPx, int? imageHeightPx,
        int padding, CloudBounds bounds)
    {
        if (wordLayouts.Count == 0)
            return new Bitmap(VisualizationDefaults.OutputWidthPx, VisualizationDefaults.OutputHeightPx).AsResult();

        if (imageWidthPx.HasValue && imageHeightPx.HasValue)
        {
            if (imageWidthPx.Value <= 0 || imageHeightPx.Value <= 0)
                return Result.Fail<Bitmap>("Image size must be positive");

            if (bounds.Width > imageWidthPx.Value || bounds.Height > imageHeightPx.Value)
                return Result.Fail<Bitmap>(
                    "Cloud bounds are larger than image size. Try to increase imageSize, or leave them empty for auto calculation");

            return new Bitmap(
                imageWidthPx.Value, imageHeightPx.Value);
        }

        var width = imageWidthPx ?? bounds.Width + padding * 2;
        var height = imageHeightPx ?? bounds.Height + padding * 2;

        return new Bitmap(Math.Max(width, VisualizationDefaults.OutputWidthPx),
            Math.Max(height, VisualizationDefaults.OutputHeightPx));
    }

    private Result<Bitmap> DrawWordsOnBitmap(Bitmap bitmap, List<WordLayout> wordLayouts,
        VisualizationOptions visualizationOptions)
    {
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(visualizationOptions.BackgroundColor);
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        if (wordLayouts.Count == 0)
            return bitmap.AsResult();

        if (visualizationOptions.ImageWidthPx > 0)
        {
            DrawWordsWithFixedSize(graphics, wordLayouts, visualizationOptions);
        }
        else
        {
            var cloudBounds = CalculateCloudBounds(wordLayouts);
            foreach (var layout in wordLayouts)
            {
                DrawSingleWord(graphics, layout, cloudBounds, visualizationOptions);
            }
        }

        return bitmap.AsResult();
    }

    private void DrawWordsWithFixedSize(Graphics graphics, List<WordLayout> wordLayouts,
        VisualizationOptions visualizationOptions)
    {
        var cloudBounds = CalculateCloudBounds(wordLayouts);
        var centerX = visualizationOptions.ImageWidthPx.Value / 2;
        var centerY = visualizationOptions.ImageHeightPx.Value / 2;

        var cloudCenterX = (cloudBounds.MinX + cloudBounds.MaxX) / 2;
        var cloudCenterY = (cloudBounds.MinY + cloudBounds.MaxY) / 2;

        foreach (var layout in wordLayouts)
        {
            using var font = new Font(visualizationOptions.FontFamily, layout.FontSize, FontStyle.Regular);
            using var brush = new SolidBrush(visualizationOptions.FontColor ?? GetRandomColor());

            var adjustedX = layout.Bounds.Left - cloudCenterX + centerX;
            var adjustedY = layout.Bounds.Top - cloudCenterY + centerY;

            graphics.DrawString(layout.Word, font, brush, new Point(adjustedX, adjustedY));
        }
    }

    private void DrawSingleWord(Graphics graphics, WordLayout layout, CloudBounds cloudBounds,
        VisualizationOptions visualizationOptions)
    {
        using var font = new Font(visualizationOptions.FontFamily, layout.FontSize, FontStyle.Regular);
        using var brush = new SolidBrush(visualizationOptions.FontColor ?? GetRandomColor());

        var adjustedPosition = CalculateDrawPosition(layout.Bounds, cloudBounds, visualizationOptions.Padding);
        graphics.DrawString(layout.Word, font, brush, adjustedPosition);
    }

    private Point CalculateDrawPosition(Rectangle bounds, CloudBounds cloudBounds, int padding)
    {
        return new Point(
            bounds.Left - cloudBounds.MinX + padding,
            bounds.Top - cloudBounds.MinY + padding);
    }

    private CloudBounds CalculateCloudBounds(List<WordLayout> wordLayouts)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var layout in wordLayouts)
        {
            var rect = layout.Bounds;
            minX = Math.Min(minX, rect.Left);
            minY = Math.Min(minY, rect.Top);
            maxX = Math.Max(maxX, rect.Right);
            maxY = Math.Max(maxY, rect.Bottom);
        }

        return new CloudBounds(minX, minY, maxX, maxY);
    }

    private Color GetRandomColor()
    {
        return Color.FromArgb(
            _random.Next(256),
            _random.Next(256),
            _random.Next(256));
    }
}