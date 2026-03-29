using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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

    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
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
    /// Extracts the file title from the first h1 heading (# ...).
    /// Falls back to defaultTitle if no heading found in first 3 lines.
    /// </summary>
    public static string ReadTitleOrDefault(FileInfo file, string defaultTitle, int maxLinesToCheck = 3)
    {
        if (!file.Exists)
        {
            return defaultTitle;
        }

        try
        {
            using var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(fileStream);

            for (var i = 0; i < maxLinesToCheck; i++)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (!line.TrimStart().StartsWith("# "))
                {
                    continue;
                }

                var title = line.Trim()[2..].Trim();
                return string.IsNullOrWhiteSpace(title) ? defaultTitle : title;
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }

        return defaultTitle;
    }

    public static bool TryReadFrontMatter<T>(FileInfo file, out T? metadata) where T : class
    {
        metadata = null;

        if (!file.Exists)
        {
            return false;
        }

        try
        {
            var markdownContent = File.ReadAllText(file.FullName);
            var document = Parse(markdownContent);
            var yamlBlock = document
                .Descendants<YamlFrontMatterBlock>()
                .FirstOrDefault();

            if (yamlBlock == null)
            {
                return false;
            }

            var yaml = yamlBlock.Lines.ToString();
            metadata = Deserializer.Deserialize<T>(yaml);

            return metadata != null;
        }
        catch (YamlException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>
    /// Parses markdown text into a Markdown document using the shared pipeline.
    /// </summary>
    /// <param name="markdownText">The markdown text to parse.</param>
    /// <returns>The parsed Markdown document.</returns>
    private static MarkdownDocument Parse(string markdownText)
    {
        return Markdown.Parse(markdownText, Pipeline);
    }
}
