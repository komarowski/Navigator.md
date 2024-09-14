using MarkdownNavigator.Domain.Entities;

namespace MarkdownNavigator.Domain.Services
{
  /// <summary>
  /// Service for reading app settings.
  /// </summary>
  public interface IAppSettingsService
  {
    /// <summary>
    /// Tries to read app settings.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns>True if the settings were read successfully.</returns>
    public bool TryReadAppSettings(AppSettings settings);
  }
}
