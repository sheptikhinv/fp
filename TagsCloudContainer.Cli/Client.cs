using System.Diagnostics.CodeAnalysis;
using Autofac;
using TagsCloudContainer.Core.CloudRenderers;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.LayoutBuilders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;
using TagsCloudContainer.Core.WordProcessing;

namespace TagsCloudContainer.Cli;

public class Client
{
    [SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
    public void Run(Options options)
    {
        var container = Startup.ConfigureServices(options);

        using var scope = container.BeginLifetimeScope();
        var wordProcessor = scope.Resolve<WordsProcessor>();
        var fileReaderFactory = scope.Resolve<FileReaderFactory>();
        var layoutBuilder = scope.Resolve<ILayoutBuilder>();
        var cloudRenderer = scope.Resolve<ICloudRenderer>();
        var visualizationOptions = scope.Resolve<VisualizationOptions>();

        var result = fileReaderFactory.GetReader(options.FilePath)
            .Then(reader => reader.ReadWords(options.FilePath))
            .Then(rawWords => wordProcessor.ProcessAndCountWords(rawWords))
            .Then(count => layoutBuilder.BuildLayout(count))
            .Then(layout => cloudRenderer.RenderCloud(layout, visualizationOptions))
            .Then(bitmap =>
            {
                var output = options.OutputFilePath ??
                             Path.Combine(Environment.CurrentDirectory,
                                 $"TagsCloud_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.png");
                return (bitmap, output);
            })
            .Then(data => FileSaver.SaveFile(data.bitmap, data.output))
            .OnFail(Console.WriteLine);

        if (result.IsSuccess)
        {
            var output = options.OutputFilePath ?? Path.Combine(Environment.CurrentDirectory,
                $"TagsCloud_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.png");
            Console.WriteLine($"Visualization saved to file {output}");
        }

        ;

        // var rawWords = fileReaderFactory.GetReader(options.FilePath).ReadWords(options.FilePath);
        // var count = wordProcessor.ProcessAndCountWords(rawWords);
        // var layout = layoutBuilder.BuildLayout(count);
        // var bitmap = cloudRenderer.RenderCloud(layout, visualizationOptions);
        // var output = options.OutputFilePath ??
        //              Path.Combine(Environment.CurrentDirectory, $"TagsCloud_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.png");
        //
        // FileSaver.SaveFile(bitmap, output);
        //
        // Console.WriteLine($"Visualization saved to file {options.OutputFilePath}");
    }
}