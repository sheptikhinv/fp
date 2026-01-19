using System.Drawing;
using DocumentFormat.OpenXml.Office2013.Word;
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
            ValidateOnlyPositiveValues(options.OutputWidthPx, "OutputWidthPx", isRequired: false),
            ValidateOnlyPositiveValues(options.OutputHeightPx, "OutputHeightPx", isRequired: false),
            string.IsNullOrEmpty(options.FontFamily) ? Result.Ok() : ValidateFont(options.FontFamily),
            options.FontSize <= 0 ? Result.Fail<None>("Value FontSize must be positive") : Result.Ok(),
            ValidateColor(options.BackgroundColor),
            ValidateColor(options.TextColor),
            options.AngleStepRadians <= 0 ? Result.Fail<None>("Value AngleStepRadians must be positive") : Result.Ok()
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

        return !File.Exists(filePath)
            ? Result.Fail<None>($"File {filePath} not found")
            : Result.Ok();
    }

    private static Result<None> ValidateOutputPath(string? outputPath)
    {
        if (outputPath == null) return Result.Ok();
        var imageFormat = Path.GetExtension(outputPath).ToLower().GetImageFormat();
        return !imageFormat.IsSuccess
            ? Result.Fail<None>(imageFormat.Error)
            : Result.Ok();
    }

    private static Result<None> ValidateOnlyPositiveValues(int? value, string argName, bool isRequired = true)
    {
        if (value == null)
        {
            return isRequired
                ? Result.Fail<None>($"Value {argName} is required, but was null")
                : Result.Ok();
        }

        return value <= 0
            ? Result.Fail<None>($"Value {argName} must be positive")
            : Result.Ok();
    }

    private static Result<None> ValidateFont(string fontName)
    {
        var tempFont = new Font(fontName, 12);
        return tempFont.Name == fontName
            ? Result.Ok()
            : Result.Fail<None>($"Font {fontName} not found");
    }

    private static Result<None> ValidateColor(string color)
    {
        return Color.FromName(color).IsKnownColor
            ? Result.Ok()
            : Result.Fail<None>($"Color {color} is not known");
    }
}