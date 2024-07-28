using MarkdownNavigator.Domain.Entities;

namespace MarkdownNavigator.Entities
{
  /// <summary>
  /// Custom settings section in appsettings.json.
  /// </summary>
  public class CustomSettings
  {
    /// <summary>
    /// List of <see cref="AppSettings"/>.
    /// </summary>
    public IList<AppSettings> SettingsList { get; set; } = [];
  }
}
