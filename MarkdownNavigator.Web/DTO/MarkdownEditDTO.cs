namespace MarkdownNavigator.Web.DTO
{
  /// <summary>
  /// DTO for edit markdown file.
  /// </summary>
  public class MarkdownEditDTO
  {
    /// <summary>
    /// Markdown file full path.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    ///  Markdown text.
    /// </summary>
    public string Text { get; set; } = string.Empty;
  }
}
