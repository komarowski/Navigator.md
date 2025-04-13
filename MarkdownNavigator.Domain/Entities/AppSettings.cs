namespace MarkdownNavigator.Domain.Entities
{
  public class AppSettings : IAppSettings
  {
    public string SourceFolder { get; set; } = string.Empty;

    public List<string> PluginList { get; set; } = [];

    public bool DisableCopyAssets { get; set; }
  }
}
