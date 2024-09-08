using Markdig;
using MarkdownNavigator.Domain.MarkdownExtensions;

namespace MarkdownNavigator.Domain.Services
{
  /// <summary>
  /// Markdown to HTML text conversion service.
  /// </summary>
  public static class MarkdownService
  {
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
      .UseAdvancedExtensions()
      .Use<CustomLinkExtension>()
      .Use<TabsExtension>()
      .Build();

    /// <summary>
    /// Convert markdown text to html.
    /// </summary>
    /// <param name="markdownText">Markdown text.</param>
    /// <returns>Html text.</returns>
    public static string ConvertToHtml(string markdownText)
    {
      return Markdown.ToHtml(markdownText, Pipeline);
    }
  }
}
