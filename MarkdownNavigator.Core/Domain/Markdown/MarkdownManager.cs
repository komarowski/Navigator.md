using Markdig;
using Markdig.Syntax;

namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// Provides shared Markdown parsing and HTML conversion using the application's configured pipeline.
/// </summary>
public static class MarkdownManager
{
    /// <summary>
    /// Shared Markdown pipeline with front matter support and custom extensions.
    /// </summary>
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseYamlFrontMatter()
        .UseAdvancedExtensions()
        .Use<CustomLinkExtension>()
        .Use<TabsExtension>()
        .Build();

    /// <summary>
    /// Converts markdown text to HTML using the shared pipeline.
    /// </summary>
    /// <param name="markdownText">The markdown text to convert.</param>
    /// <returns>The generated HTML.</returns>
    public static string ConvertToHtml(string markdownText)
    {
        return Markdown.ToHtml(markdownText, Pipeline);
    }

    /// <summary>
    /// Parses markdown text into a Markdown document using the shared pipeline.
    /// </summary>
    /// <param name="markdownText">The markdown text to parse.</param>
    /// <returns>The parsed Markdown document.</returns>
    public static MarkdownDocument Parse(string markdownText)
    {
        return Markdown.Parse(markdownText, Pipeline);
    }
}
