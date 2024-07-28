namespace MarkdownNavigator.Domain.Entities
{
  /// <summary>
  /// Application settings.
  /// </summary>
  public interface IAppSettings
  {
    /// <summary>
    /// App settings name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Folder with markdwon files.
    /// </summary>
    public string SourceFolder { get; set; }

    /// <summary>
    /// If true, all HTML files are generated in a separate export folder, convenient for transferring to other devices.
    /// </summary>
    public bool IsExport { get; set; }

    /// <summary>
    /// If true, standalone templates are used.
    /// </summary>
    public bool IsStandalone { get; set; }

    /// <summary>
    /// If true, the web server is launched to edit markdown files.
    /// </summary>
    public bool IsWebServer { get; set; }
  }
}
