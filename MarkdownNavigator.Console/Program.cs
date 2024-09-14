using MarkdownNavigator.Console.Services;
using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Domain.Services;
using MarkdownNavigator.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.None);

builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
builder.Services.AddSingleton<IFileWatcherService, FileWatcherService>();
builder.Services.AddSingleton<ITreeStructureService, TreeStructureService>();
builder.Services.AddSingleton<IConvertService, ConvertService>();
builder.Services.AddSingleton<ConsoleCommandService>();

using IHost host = builder.Build();

var appSettingsService = host.Services.GetRequiredService<IAppSettingsService>();
var appSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("appsettings.json"));
while (!appSettingsService.TryReadAppSettings(appSettings))
{
  ConsoleService.WriteLog("Failed to read application settings. Try to change the settings.", LogType.Warning);
  ConsoleService.Prompt("Press ENTER to continue...");
  appSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("appsettings.json"));
}

var consoleCommandService = host.Services.GetRequiredService<ConsoleCommandService>();
var cancelTokenSource = new CancellationTokenSource();
var hostTask = host.RunAsync(cancelTokenSource.Token);
var consoleTask = Task.Run(() => consoleCommandService.HandleConsoleCommands(cancelTokenSource));
await Task.WhenAny(hostTask, consoleTask);