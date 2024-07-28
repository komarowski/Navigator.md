using MarkdownNavigator.Domain.Entities;

namespace MarkdownNavigator.Domain.Services
{
  /// <summary>
  /// Service for obtaining the tree structure of the source folder with markdown files.
  /// </summary>
  public interface ITreeStructureService
  {
    /// <summary>
    /// Walk a directory tree by using recursion.
    /// </summary>
    /// <param name="root">Root directory.</param>
    /// <param name="tree">Storing information.</param>
    /// <param name="forceRefreshAll">Update all files anyway.</param>
    /// <returns>Information about the folder structure.</returns>
    public TreeStructure WalkDirectoryTree(DirectoryInfo root, TreeStructure tree, bool forceRefreshAll);

    /// <summary>
    /// Get code from markdown or folder full path.
    /// </summary>
    /// <param name="path">Markdown or folder full path.</param>
    /// <returns>Code.</returns>
    public string GetPathCode(string path);
  }
}
