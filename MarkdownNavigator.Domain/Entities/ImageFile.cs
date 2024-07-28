namespace MarkdownNavigator.Domain.Entities
{
  /// <summary>
  /// Markdown image file info.
  /// </summary>
  /// <param name="targetPath">The target path to copy.</param>
  /// <param name="sourcePath">Path to copy from.</param>
  public class ImageFile(string targetPath, string sourcePath)
  {
    /// <summary>
    /// The target path to copy.
    /// </summary>
    public string TargetPath { get; set; } = targetPath;

    /// <summary>
    /// Path to copy from.
    /// </summary>
    public string SourcePath { get; set; } = sourcePath;
  }
}
