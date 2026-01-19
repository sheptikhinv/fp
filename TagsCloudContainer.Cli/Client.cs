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
    public void Run(Options options)
    {
        var container = Startup.ConfigureServices(options);

        using var scope = container.BeginLifetimeScope();
        var wordProcessor = scope.Resolve<WordsProcessor>();
        var fileReaderFactory = scope.Resolve<FileReaderFactory>();
        var layoutBuilder = scope.Resolve<ILayoutBuilder>();
        var cloudRenderer = scope.Resolve<ICloudRenderer>();
        var visualizationOptions = scope.Resolve<VisualizationOptions>();

        var result = options.Validate()
            .Then(_ => fileReaderFactory.GetReader(options.FilePath))
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
            .Then(output => Console.WriteLine($"Visualization saved to file {output}"))
            .OnFail(error => Console.WriteLine(error));
    }
}