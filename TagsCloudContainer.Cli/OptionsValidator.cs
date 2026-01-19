using System.Drawing;
using TagsCloudContainer.Core.Utils;
using Color = System.Drawing.Color;

namespace TagsCloudContainer.Cli;

public static class OptionsValidator
{
    public static Result<None> Validate(this Options options)
    {
        var validations = new[]
        {
            ValidatePath(options.FilePath, isRequired: true),
            ValidatePath(options.FilterFilePath, isRequired: false),
            ValidateOutputPath(options.OutputFilePath),
            ValidateOnlyPositiveValues(options.OutputWidthPx, nameof(options.OutputWidthPx), isRequired: false),
            ValidateOnlyPositiveValues(options.OutputHeightPx, nameof(options.OutputHeightPx), isRequired: false),
            ValidateFont(options.FontFamily),
            ValidateOnlyPositiveValues<float>(options.FontSize, nameof(options.FontSize), isRequired: false),
            ValidateColor(options.BackgroundColor),
            ValidateColor(options.TextColor),
            ValidateOnlyPositiveValues<double>(options.AngleStepRadians, nameof(options.AngleStepRadians),
                isRequired: false)
        };

        var errors = validations
            .Where(r => !r.IsSuccess)
            .Select(r => r.Error)
            .ToList();

        return errors.Count == 0
            ? Result.Ok()
            : Result.Fail<None>("Options validation failed with following errors:"
                                + Environment.NewLine
                                + string.Join(Environment.NewLine, errors));
    }

    private static Result<None> ValidatePath(string? filePath, bool isRequired = true)
    {
        if (filePath == null)
        {
            return isRequired
                ? Result.Fail<None>("File path is required, but was null")
                : Result.Ok();
        }

        return File.Exists(filePath)
            ? Result.Ok()
            : Result.Fail<None>($"File {filePath} not found");
    }

    private static Result<None> ValidateOutputPath(string? outputPath)
    {
        if (outputPath == null) return Result.Ok();
        var imageFormat = Path.GetExtension(outputPath).ToLower().GetImageFormat();
        return imageFormat.IsSuccess
            ? Result.Ok()
            : Result.Fail<None>(imageFormat.Error);
    }

    private static Result<None> ValidateOnlyPositiveValues<T>(T? value, string argName, bool isRequired = true)
        where T : struct, IComparable<T>
    {
        if (value == null)
        {
            return isRequired
                ? Result.Fail<None>($"Value {argName} is required, but was null")
                : Result.Ok();
        }

        var zero = default(T);
        return value.Value.CompareTo(zero) <= 0
            ? Result.Fail<None>($"Value {argName} must be positive")
            : Result.Ok();
    }

    private static Result<None> ValidateFont(string? fontName)
    {
        if (fontName == null) return Result.Ok();
        var tempFont = new Font(fontName, 12);
        var result = tempFont.Name == fontName
            ? Result.Ok()
            : Result.Fail<None>($"Font {fontName} not found");
        tempFont.Dispose();
        return result;
    }

    private static Result<None> ValidateColor(string color)
    {
        return Color.FromName(color).IsKnownColor
            ? Result.Ok()
            : Result.Fail<None>($"Color {color} is not known");
    }
}