using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Entities;
using MarkdownNavigator.Infrastructure.Services;
using MarkdownNavigator.Services;
using Spectre.Console;

namespace MarkdownNavigator.Domain.Services
{
  public class AppSettingsService(IConfiguration configuration, IAppSettings appSettings) : IAppSettingsService
  {
    private readonly IConfiguration configuration = configuration;
    private readonly IAppSettings appSettings = appSettings;

    public bool TryReadAppSettings()
    {
      var customSettings = new CustomSettings();
      configuration.GetSection(nameof(CustomSettings)).Bind(customSettings);

      if (customSettings is null || customSettings.SettingsList.Count == 0)
      {
        return false;
      }

      var settingsList = customSettings.SettingsList;
      string selectedName = settingsList.Count > 1
        ? AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select application settings.")
            .PageSize(settingsList.Count + 1)
            .AddChoices(settingsList.Select(x => x.Name)))
        : settingsList[0].Name;

      var selectedAppSettings = settingsList.FirstOrDefault(x => x.Name == selectedName);
      var isValidAppSettings = selectedAppSettings is not null
        && IsDirectoryExists(selectedAppSettings.SourceFolder);

      if (isValidAppSettings)
      {
        appSettings.Name = selectedAppSettings.Name;
        appSettings.SourceFolder = selectedAppSettings.SourceFolder;
        appSettings.IsExport = selectedAppSettings.IsExport;
        appSettings.IsStandalone = selectedAppSettings.IsStandalone;
        appSettings.IsWebServer = selectedAppSettings.IsWebServer;
      }
      
      return isValidAppSettings;
    }

    /// <summary>
    /// Checks if the directory exists.
    /// </summary>
    /// <param name="directory">Directory.</param>
    /// <returns>true if the directory exists.</returns>
    private static bool IsDirectoryExists(string directory)
    {
      var result = Directory.Exists(directory);
      if (!result)
      {
        ConsoleService.WriteLog($"\"{directory}\" directory doesn't exist.", LogType.Error);
      }
      return result;
    }
  }
}
