using MarkdownNavigator.Infrastructure;

namespace MarkdownNavigator.Domain.Entities
{
  /// <summary>
  /// Defines ignore rules for skipping files when generating HTML from Markdown.
  /// </summary>
  public class IgnoreRules
  {
    /// <summary>
    /// The list of file names to exclude from processing.
    /// </summary>
    public string[] ExcludeFiles { get; set; } = [];

    /// <summary>
    /// The list of folder names to exclude entirely from traversal.
    /// </summary>
    public string[] ExcludeFolders { get; set; } = [FolderReservedNames.AssetsFolder];
  }
}
