using TagsCloudContainer.Core.Utils;

namespace TagsCloudContainer.Core.FileReaders;

public class FileReaderFactory
{
    private readonly IEnumerable<IFileReader> _readers;

    public FileReaderFactory(IEnumerable<IFileReader> readers)
    {
        _readers = readers;
    }

    public Result<IFileReader> GetReader(string filePath)
    {
        var reader = _readers.FirstOrDefault(r =>
        {
            var canReadResult = r.CanReadFile(Path.GetExtension(filePath));
            return canReadResult is { IsSuccess: true, Value: true };
        });

        if (reader == null)
        {
            return Result.Fail<IFileReader>($"No reader found for file: {filePath}");
        }

        return reader.AsResult();
    }
}