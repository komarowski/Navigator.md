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
    public bool EnableExport { get; set; }

    /// <summary>
    /// If true, do not copy asset files.
    /// </summary>
    public bool DisableCopyAssets { get; set; }

    /// <summary>
    /// If true, a custom template is used for the path "assets/template.html".
    /// </summary>
    public bool EnableCustomTemplate { get; set; }

    /// <summary>
    /// If true, the web server is not launched to edit markdown files.
    /// </summary>
    public bool DisableWebServer { get; set; }
  }
}
