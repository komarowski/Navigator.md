namespace MarkdownNavigator.Console.Services
{
  /// <summary>
  /// Monitors changes to markdown files in source folder.
  /// </summary>
  public interface IFileWatcherService
  {
    /// <summary>
    /// Launches FileSystemWatcher.
    /// </summary>
    public void StartFileWatcher();

    /// <summary>
    /// Stops FileSystemWatcher.
    /// </summary>
    public void StopFileWatcher();
  }
}
