namespace MarkdownNavigator.Domain.Entities
{
  public class AppSettings : IAppSettings
  {
    public string SourceFolder { get; set; } = string.Empty;

    public string Server { get; set; } = string.Empty;

    public bool DisableCopyAssets { get; set; }
  }
}
