namespace MarkdownNavigator.Domain.Services
{
  /// <summary>
  /// Service for converting markdown files to html.
  /// </summary>
  public interface IConvertService
  {
    /// <summary>
    ///  Converting all markdown files to html.
    /// </summary>
    /// <param name="forceRefreshAll">Update all files anyway.</param>
    /// <returns>Number of updated or added files.</returns>
    public int ConvertAllHtml(bool forceRefreshAll = false);

    /// <summary>
    /// Converting markdown file to html and save.
    /// </summary>
    /// <param name="markdownPath">Markdown file path.</param>
    /// <param name="code">Path code.</param>
    public void ConvertHtml(string markdownPath, string? code = null);
  }
}
