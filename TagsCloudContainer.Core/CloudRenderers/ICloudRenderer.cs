using System.Drawing;
using TagsCloudContainer.Core.DTOs;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Core.CloudRenderers;

public interface ICloudRenderer
{
    Result<Bitmap> RenderCloud(List<WordLayout> wordLayouts, VisualizationOptions options);
}