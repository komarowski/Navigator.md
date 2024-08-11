namespace MarkdownNavigator.Domain.Entities
{
  public class AppSettings : IAppSettings
  {
    public string Name { get; set; } = string.Empty;

    public string SourceFolder { get; set; } = string.Empty;

    public bool EnableExport { get; set; }

    public bool DisableCopyAssets { get; set; }

    public bool EnableCustomTemplate { get; set; }

    public bool DisableWebServer { get; set; }
  }
}
