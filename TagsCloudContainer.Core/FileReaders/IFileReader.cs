using TagsCloudContainer.Core.Utils;

namespace TagsCloudContainer.Core.FileReaders;

public interface IFileReader
{
    Result<bool> CanReadFile(string extension);
    Result<List<string>> ReadWords(string filePath);
}