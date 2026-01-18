using TagsCloudContainer.Core.Utils;

namespace TagsCloudContainer.Core.FileReaders;

public class TxtFileReader : IFileReader
{
    private static readonly string[] SupportedExtensions = [".txt"];

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
            var lines = File.ReadAllLines(filePath);
            return Result.Ok(lines.ToList());
        }
        catch (IOException e)
        {
            return Result.Fail<List<string>>($"File at {filePath} could not be read: {e.Message}");
        }
    }
}