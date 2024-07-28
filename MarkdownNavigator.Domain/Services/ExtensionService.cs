namespace MarkdownNavigator.Domain.Services
{
  /// <summary>
  /// Extension management service.
  /// </summary>
  public static class ExtensionService
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
    /// Gets html file name with extension from name without extension. 
    /// </summary>
    /// <param name="name">Name without extension.</param>
    /// <returns>Name with html extension.</returns>
    public static string GetHtml(string name)
    {
      return $"{name}{ExtensionHtml}";
    }

    /// <summary>
    /// Replaces the markdown extension with html.
    /// </summary>
    /// <param name="name">Name with markdown extension.</param>
    /// <returns>Name with html extension.</returns>
    public static string MarkdownToHtml(string name)
    {
      return name.Replace(ExtensionMarkdown, ExtensionHtml);
    }
  }
}
