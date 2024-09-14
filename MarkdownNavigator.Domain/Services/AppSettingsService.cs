using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Infrastructure.Services;

namespace MarkdownNavigator.Domain.Services
{
  public class AppSettingsService(IAppSettings appSettings): IAppSettingsService
  {
    private readonly IAppSettings appSettings = appSettings;

    public bool TryReadAppSettings(AppSettings? settings)
    {
      if (settings is null || !IsDirectoryExists(settings.SourceFolder))
      {
        return false;
      }

      appSettings.SourceFolder = settings.SourceFolder;
      appSettings.DisableCopyAssets = settings.DisableCopyAssets;
      appSettings.Server = settings.Server;

      return true;
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
