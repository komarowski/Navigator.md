using MarkdownNavigator.Console.Logger;
using MarkdownNavigator.Console.Services;
using MarkdownNavigator.Core.Application;
using MarkdownNavigator.Core.Domain;
using MarkdownNavigator.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Create host with DI configuration
var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
});

hostBuilder.ConfigureServices((context, services) =>
{
    services.AddSingleton(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        return new AppSettings
        {
            SourceFolder = config.GetValue<string>("SourceFolder")
                ?? throw new InvalidOperationException("SourceFolder is not configured.")
        };
    });

    // Core Domain Services
    services.AddSingleton<ITreeBuilder, TreeBuilder>();
    services.AddSingleton<IPathManager, PathManager>();
    services.AddSingleton<ITaskScanner, TaskScanner>();
    services.AddSingleton<IQaScanner, QaScanner>();

    // Core Infrastructure Services
    services.AddSingleton<IEmbeddedResourceProvider, EmbeddedResourceProvider>();
    services.AddSingleton<ISiteGenerator, SiteGenerator>();

    // Core Application Services
    services.AddSingleton<ISiteBuilder, SiteBuilder>();

    // Console-Specific Services
    services.AddSingleton<ILoggerProvider, SpectreConsoleLoggerProvider>();
    services.AddSingleton<FileWatcherService>();
    services.AddSingleton<ConsoleCommandService>();
});

var host = hostBuilder.Build();

try
{
    var consoleService = host.Services.GetRequiredService<ConsoleCommandService>();
    await consoleService.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fatal error: {ex.Message}");
    Console.Error.WriteLine(ex.StackTrace);
    Environment.Exit(1);
}
