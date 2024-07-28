using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Infrastructure.Services;

namespace MarkdownNavigator.Domain.Services
{
  public class FileWatcherService(IConvertService renderService, IAppSettings appSettings) : IFileWatcherService
  {
    private readonly IAppSettings settings = appSettings;
    private FileSystemWatcher? fileSystemWatcher;
    private readonly IConvertService convertService = renderService;
    private DateTime lastRead = DateTime.MinValue;

    public void StartFileWatcher()
    {
      this.fileSystemWatcher = new FileSystemWatcher(settings.SourceFolder);
      this.fileSystemWatcher.Changed += OnChanged;
      this.fileSystemWatcher.Created += OnCreated;
      this.fileSystemWatcher.Deleted += OnDeleted;
      this.fileSystemWatcher.Error += OnError;
      this.fileSystemWatcher.Filter = "*.md";
      this.fileSystemWatcher.IncludeSubdirectories = true;
      this.fileSystemWatcher.EnableRaisingEvents = true;
      ConsoleService.WriteLogBeforeReadLine($"FileWatcher started.", LogType.Info, string.Empty);
    }

    public void StopFileWatcher()
    {
      if (this.fileSystemWatcher is not null)
      {
        this.fileSystemWatcher.Dispose();
        this.fileSystemWatcher = null;
      }
      ConsoleService.WriteLogBeforeReadLine($"FileWatcher stopped.", LogType.Info, string.Empty);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
      var exception = e.GetException();
      ConsoleService.WriteLogBeforeReadLine($"An error occurred: {exception.Message}", LogType.Error, "> ");
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
      if (CheckLastReadTime(WatcherChangeTypes.Changed, e))
      {
        convertService.ConvertHtml(e.FullPath);
        ConsoleService.WriteLogBeforeReadLine($"changed \"{e.Name}\"", LogType.Info, "> ");
      }
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
      if (CheckLastReadTime(WatcherChangeTypes.Created, e))
      {
        ConsoleService.WriteLogBeforeReadLine($"added \"{e.Name}\"", LogType.Info, "> ");
        convertService.ConvertAllHtml();
      }
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
      if (CheckLastReadTime(WatcherChangeTypes.Deleted, e))
      {
        ConsoleService.WriteLogBeforeReadLine($"deleted \"{e.Name}\"", LogType.Info, "> ");
        convertService.ConvertAllHtml();
      }
    }

    /// <summary>
    /// Duplicate Event Check.
    /// </summary>
    private bool CheckLastReadTime(WatcherChangeTypes watcherChangeType, FileSystemEventArgs e)
    {
      DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
      var isValid = e.ChangeType == watcherChangeType && lastWriteTime != this.lastRead;
      if (isValid)
      {
        this.lastRead = lastWriteTime;
      }
      return isValid;
    }
  }
}
