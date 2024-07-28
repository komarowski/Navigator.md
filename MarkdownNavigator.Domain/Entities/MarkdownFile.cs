namespace MarkdownNavigator.Domain.Entities
{
  /// <summary>
  /// Markdown file information.
  /// </summary>
  public class MarkdownFile
  {
    /// <summary>
    /// HTML file code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Relative markdown file path.
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;
  }
}