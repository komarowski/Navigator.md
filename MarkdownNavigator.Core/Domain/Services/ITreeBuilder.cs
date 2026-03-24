namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// Builds a tree structure from markdown files.
/// Recursively walks directories, applies ignore rules, and collects markdown files for conversion.
/// Folder structure is represented by index.md files; content files (.md) are organized beneath them.
/// </summary>
public interface ITreeBuilder
{
    /// <summary>
    /// Build a tree structure from the given root folder.
    /// </summary>
    /// <param name="rootFolder">Wiki folder path</param>
    /// <returns>Complete tree structure</returns>
    TreeStructure GetTreeStructure(string rootFolder);
}
