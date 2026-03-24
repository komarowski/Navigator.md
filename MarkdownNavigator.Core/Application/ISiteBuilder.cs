namespace MarkdownNavigator.Core.Application;

/// <summary>
/// Orchestrates the site building process.
/// </summary>
public interface ISiteBuilder
{
    /// <summary>
    /// Builds the HTML site from the source markdown files.
    /// </summary>
    /// <param name="forceRebuild">If true, forces a rebuild of all content regardless of changes.</param>
    void Build(bool forceRebuild = false);
}
