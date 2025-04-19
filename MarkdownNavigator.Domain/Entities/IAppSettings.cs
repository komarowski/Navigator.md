namespace MarkdownNavigator.Domain.Entities
{
  /// <summary>
  /// Application settings.
  /// </summary>
  public interface IAppSettings
  {
    /// <summary>
    /// Folder with markdwon files.
    /// </summary>
    public string SourceFolder { get; set; }

    /// <summary>
    /// List of plugins to enable.
    /// </summary>
    public List<string> PluginList { get; set; }

    /// <summary>
    /// If true, do not copy asset files.
    /// </summary>
    public bool DisableCopyAssets { get; set; }
  }
}
