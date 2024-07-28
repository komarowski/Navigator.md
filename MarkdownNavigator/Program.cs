using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Domain.Services;
using MarkdownNavigator.Infrastructure.Services;
using MarkdownNavigator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddSingleton<ConsoleCommandService>();
builder.Services.AddSingleton<IFileWatcherService, FileWatcherService>();
builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<ITreeStructureService, TreeStructureService>();
builder.Services.AddSingleton<IConvertService, ConvertService>();

builder.Services.AddCors();

var app = builder.Build();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

using (var scope = app.Services.CreateScope())
{
  var appSettingsService = scope.ServiceProvider.GetService<IAppSettingsService>();
  while (!appSettingsService.TryReadAppSettings())
  {
    ConsoleService.WriteLog("Failed to read application settings. Try to change the settings.", LogType.Warning);
    ConsoleService.Prompt("Press ENTER to continue...");
  }

  var consoleService = scope.ServiceProvider.GetService<ConsoleCommandService>();
  var settings = scope.ServiceProvider.GetService<IAppSettings>();
  if (settings.IsWebServer)
  {
    var apiService = scope.ServiceProvider.GetService<ApiService>();
    apiService.RegisterEndpoints(app);
    var cancelTokenSource = new CancellationTokenSource();
    var webServerTask = app.RunAsync(cancelTokenSource.Token);
    var consoleTask = Task.Run(() => consoleService.HandleConsoleCommands(cancelTokenSource));
    await Task.WhenAny(webServerTask, consoleTask);
  }
  else
  {
    await Task.Run(() => consoleService.HandleConsoleCommands());
  }
}
