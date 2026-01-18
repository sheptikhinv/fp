using System.Drawing;
using TagsCloudContainer.Core.CoordinateGenerators;
using TagsCloudContainer.Core.DTOs;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Core.LayoutBuilders;

public class BasicLayoutBuilder : ILayoutBuilder
{
    private readonly ICoordinateGenerator _coordinateGenerator;
    private readonly VisualizationOptions _visualizationOptions;

    public BasicLayoutBuilder(ICoordinateGenerator coordinateGenerator, VisualizationOptions visualizationOptions)
    {
        _coordinateGenerator = coordinateGenerator;
        _visualizationOptions = visualizationOptions;
    }

    public Result<List<WordLayout>> BuildLayout(Dictionary<string, int> wordFrequencies)
    {
        if (wordFrequencies.Count == 0)
            return new List<WordLayout>().AsResult();

        var layouts = new List<WordLayout>();
        var maxFrequency = wordFrequencies.Values.Max();
        var sortedWords = wordFrequencies.OrderByDescending(w => w.Value).ToList();

        foreach (var (word, frequency) in sortedWords)
        {
            var fontSize = CalculateFontSize(frequency, maxFrequency);
            var wordSize = GetWordSize(word, fontSize, _visualizationOptions.FontFamily);
            var bounds = FindPositionForWord(wordSize, layouts);

            layouts.Add(new WordLayout
            {
                Word = word,
                Bounds = bounds,
                FontSize = fontSize
            });
        }

        return layouts.AsResult();
    }

    private float CalculateFontSize(int frequency, int maxFrequency)
    {
        var ratio = (float)frequency / maxFrequency;
        return _visualizationOptions.FontSize + ratio * _visualizationOptions.FontSize * 5;
    }

    private Rectangle FindPositionForWord(Size wordSize, List<WordLayout> existingLayouts)
    {
        Rectangle bounds;

        do
        {
            var position = _coordinateGenerator.GetNextPosition();
            bounds = CreateRectangleAtPosition(position, wordSize);
        } while (IntersectsWithExistingWords(bounds, existingLayouts));

        return bounds;
    }

    private Rectangle CreateRectangleAtPosition(Point position, Size wordSize)
    {
        return new Rectangle(
            position.X - wordSize.Width / 2,
            position.Y - wordSize.Height / 2,
            wordSize.Width,
            wordSize.Height);
    }

    private bool IntersectsWithExistingWords(Rectangle rect, List<WordLayout> layouts)
    {
        foreach (var layout in layouts)
        {
            if (layout.Bounds.IntersectsWith(rect))
                return true;
        }

        return false;
    }

    private Size GetWordSize(string word, float fontSize, string fontFamily)
    {
        using var tempBitmap = new Bitmap(1, 1);
        using var graphics = Graphics.FromImage(tempBitmap);
        using var font = new Font(fontFamily, fontSize, FontStyle.Regular);

        var size = graphics.MeasureString(word, font);

        return new Size(
            (int)Math.Ceiling(size.Width),
            (int)Math.Ceiling(size.Height));
    }
}