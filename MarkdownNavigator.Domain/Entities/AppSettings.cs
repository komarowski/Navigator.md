namespace MarkdownNavigator.Domain.Entities
{
  public class AppSettings : IAppSettings
  {
    public string Name { get; set; } = string.Empty;

    public string SourceFolder { get; set; } = string.Empty;

    public bool IsExport { get; set; }

    public bool IsStandalone { get; set; }

    public bool IsWebServer { get; set; }
  }
}
