namespace MarkdownNavigator.Services
{
  /// <summary>
  /// Service for reading app settings.
  /// </summary>
  public interface IAppSettingsService
  {
    /// <summary>
    /// Tries to read app settings.
    /// </summary>
    /// <returns>True if the settings were read successfully.</returns>
    public bool TryReadAppSettings();
  }
}
