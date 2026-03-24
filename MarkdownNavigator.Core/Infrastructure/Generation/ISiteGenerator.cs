using MarkdownNavigator.Core.Domain;

namespace MarkdownNavigator.Core.Infrastructure;

public interface ISiteGenerator
{
    /// <summary>
    /// Generates the complete website from parsed content.
    /// </summary>
    /// <param name="content">The parsed site content (wiki, tasks, Q&A).</param>
    /// <param name="forceRebuild">If true, regenerates all HTML files. If false (default), only regenerates files newer than their corresponding HTML.</param>
    void GenerateWeb(SiteContent content, bool forceRebuild = false);
}
