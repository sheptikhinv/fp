using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using TagsCloudContainer.Core.Utils;

namespace TagsCloudContainer.Core.FileReaders;

public class DocxFileReader : IFileReader
{
    private static readonly string[] SupportedExtensions = [".docx"];

    public Result<bool> CanReadFile(string extension)
    {
        return !extension.StartsWith('.')
            ? Result.Fail<bool>($"Invalid file extension: {extension}")
            : Result.Ok(SupportedExtensions.Contains(extension));
    }

    public Result<List<string>> ReadWords(string filePath)
    {
        try
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart?.Document?.Body;
            if (body == null)
                return Result.Ok(new List<string>());

            var result = body.Descendants<Paragraph>()
                .Select(p => p.InnerText.Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
            return Result.Ok(result);
        }
        catch (IOException e)
        {
            return Result.Fail<List<string>>($"File at {filePath} could not be read: {e.Message}");
        }
    }
}