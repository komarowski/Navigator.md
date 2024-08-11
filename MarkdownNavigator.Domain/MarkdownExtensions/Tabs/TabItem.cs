namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Contains information about tab item.
  /// </summary>
  public class TabItem
  {
    /// <summary>
    /// Tab title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Lines of text in tab item.
    /// </summary>
    public List<string> ContentLines { get; set; } = [];
  }
}
