using System.Drawing;
using TagsCloudContainer.Core.Utils;

namespace TagsCloudContainer.Core.Visualizators;

public record VisualizationOptions
{
    public int Padding { get; init; } = VisualizationDefaults.Padding;
    public float FontSize { get; init; } = VisualizationDefaults.FontSize;
    public Color BackgroundColor { get; init; } = VisualizationDefaults.BackgroundColor;
    public Color? FontColor { get; init; }
    public string FontFamily { get; init; } = VisualizationDefaults.FontFamily;
    public int? ImageWidthPx { get; init; }
    public int? ImageHeightPx { get; init; }
}