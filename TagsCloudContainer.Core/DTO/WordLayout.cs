using System.Drawing;

namespace TagsCloudContainer.Core.DTO;

public readonly record struct WordLayout(string Word, Rectangle Bounds, float FontSize);