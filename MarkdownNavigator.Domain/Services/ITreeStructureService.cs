using MarkdownNavigator.Domain.Entities;

namespace MarkdownNavigator.Domain.Services
{
  /// <summary>
  /// Service for obtaining the tree structure of the source folder with markdown files.
  /// Excludes any folder and its contents if the name starts with '_'.
  /// </summary>
  public interface ITreeStructureService
  {
    /// <summary>
    /// Walk a directory tree by using recursion.
    /// </summary>
    /// <param name="root">Root directory.</param>
    /// <param name="tree">Storing information.</param>
    /// <param name="ignoreRules">The ignore rules to apply when filtering files.</param>
    /// <param name="forceRefresh">Update all files anyway.</param>
    /// <param name="excludeFromTree">Exclude folders and files from the tree structure.</param>
    /// <param name="isRoot">First call in the recursive tree walk.</param>
    /// <returns>Information about the folder structure.</returns>
    public TreeStructure WalkDirectoryTree(
      DirectoryInfo root, 
      TreeStructure tree,
      IgnoreRules? ignoreRules = null,
      bool forceRefresh = false, 
      bool excludeFromTree = false, 
      bool isRoot = false);

    /// <summary>
    /// Get node id from markdown or folder full path.
    /// </summary>
    /// <param name="path">Markdown or folder full path.</param>
    /// <returns>Node id.</returns>
    public string GetNodeId(string path);

    /// <summary>
    /// Get relative path to root folder.
    /// </summary>
    /// <param name="markdownPath">Node full markdown path.</param>
    /// <returns>Relative path to root folder.</returns>
    public string GetRelativePathForNode(string markdownPath);
  }
}
