namespace MarkdownNavigator.Domain.Services
{
  /// <summary>
  /// File extension management service.
  /// </summary>
  public static class FileExtensionService
  {
    /// <summary>
    /// Markdown extension.
    /// </summary>
    public const string ExtensionMarkdown = ".md";

    /// <summary>
    /// Html extension.
    /// </summary>
    public const string ExtensionHtml = ".html";

    /// <summary>
    /// Replaces the markdown extension with html.
    /// </summary>
    /// <param name="name">Name with markdown extension.</param>
    /// <returns>Name with html extension.</returns>
    public static string MarkdownToHtml(string name)
    {
      return name.Replace(ExtensionMarkdown, ExtensionHtml);
    }

    /// <summary>
    /// Replaces the html extension with markdown.
    /// </summary>
    /// <param name="name">Name with html extension.</param>
    /// <returns>Name with markdown extension.</returns>
    public static string HtmlToMarkdown(string name)
    {
      return name.Replace(ExtensionHtml, ExtensionMarkdown);
    }
  }
}
