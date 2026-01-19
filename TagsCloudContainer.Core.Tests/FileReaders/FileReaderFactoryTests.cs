using TagsCloudContainer.Core.FileReaders;

namespace TagsCloudContainer.Core.Tests.FileReaders;

[TestFixture]
public class FileReaderFactoryTests
{
    [Test]
    public void GetReader_ForTxt_ReturnsBasicFileReader_Test()
    {
        var factory = new FileReaderFactory(new IFileReader[] { new TxtFileReader() });

        var reader = factory.GetReader("sample.txt");

        Assert.That(reader.GetValueOrThrow(), Is.TypeOf<TxtFileReader>());
    }

    [Test]
    public void GetReader_UnsupportedExtension_ReturnsError_Test()
    {
        var factory = new FileReaderFactory(new IFileReader[] { new TxtFileReader() });
        var reader = factory.GetReader("sample.ihopetheresnofileextensionslikethis");
        Assert.That(reader is { IsSuccess: false }, Is.True);
    }

    [Test]
    public void ReadFile_ReadsAsWritten_FromTxt_Test()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"readfile_{Guid.NewGuid()}.txt");
        try
        {
            File.WriteAllLines(tempFile, new[] { "Hello", "WORLD" });
            var factory = new FileReaderFactory(new IFileReader[] { new TxtFileReader() });

            var reader = factory.GetReader(tempFile).GetValueOrThrow();
            var lines = reader.ReadWords(tempFile);

            CollectionAssert.AreEqual(new[] { "Hello", "WORLD" }, lines.GetValueOrThrow());
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}