using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Domain.Services;
using MarkdownNavigator.Infrastructure.Services;
using MarkdownNavigator.Web.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
builder.Services.AddSingleton<ITreeStructureService, TreeStructureService>();
builder.Services.AddSingleton<IConvertService, ConvertService>();
builder.Services.AddScoped<ApiService>();

var app = builder.Build();
builder.WebHost.UseUrls("https://localhost:7024");

using (var scope = app.Services.CreateScope())
{
  var appSettingsService = scope.ServiceProvider.GetService<IAppSettingsService>();
  var configuration = app.Services.GetRequiredService<IConfiguration>();
  var appSettings = new AppSettings();
  configuration.GetSection(nameof(AppSettings)).Bind(appSettings);
  if (!appSettingsService.TryReadAppSettings(appSettings))
  {
    ConsoleService.WriteLog("Failed to read application settings. Try to change the settings.", LogType.Error);
    ConsoleService.Prompt("Press ENTER to continue...");
    Environment.Exit(1);
  }

  var apiService = scope.ServiceProvider.GetService<ApiService>();
  apiService.RegisterEndpoints(app);
}

app.Use(async (context, next) =>
{
  await next();
  if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
  {
    context.Request.Path = "/index.html";
    await next();
  }
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
