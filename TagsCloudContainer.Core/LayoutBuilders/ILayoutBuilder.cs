using TagsCloudContainer.Core.DTOs;
using TagsCloudContainer.Core.Utils;

namespace TagsCloudContainer.Core.LayoutBuilders;

public interface ILayoutBuilder
{
    Result<List<WordLayout>> BuildLayout(Dictionary<string, int> wordFrequencies);
}