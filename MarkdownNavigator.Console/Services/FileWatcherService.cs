using MarkdownNavigator.Core.Application;
using MarkdownNavigator.Core.Domain;

namespace MarkdownNavigator.Console.Services;

/// <summary>
/// Watches the source folder and triggers refresh or rebuild actions.
/// </summary>
public sealed class FileWatcherService : IDisposable
{
    private readonly string sourceFolderPath;
    private readonly ISiteBuilder siteBuilder;
    private readonly object syncRoot = new();

    private FileSystemWatcher? watcher;
    private Timer? debounceTimer;
    private PendingAction pendingAction = PendingAction.None;
    private bool isStarted;

    public FileWatcherService(IPathManager pathManager, ISiteBuilder siteBuilder)
    {
        ArgumentNullException.ThrowIfNull(pathManager);
        ArgumentNullException.ThrowIfNull(siteBuilder);

        sourceFolderPath = pathManager.SourceFolder;
        this.siteBuilder = siteBuilder;
    }

    /// <summary>
    /// Starts file watching.
    /// </summary>
    public void Start()
    {
        if (isStarted)
            return;

        if (string.IsNullOrWhiteSpace(sourceFolderPath))
            throw new InvalidOperationException("Source folder is not configured.");

        if (!Directory.Exists(sourceFolderPath))
            throw new DirectoryNotFoundException($"Source folder not found: {sourceFolderPath}");

        watcher = new FileSystemWatcher(sourceFolderPath)
        {
            Filter = "*",
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName
                         | NotifyFilters.DirectoryName
                         | NotifyFilters.LastWrite
        };

        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;
        watcher.EnableRaisingEvents = true;

        isStarted = true;
    }

    private void OnChanged(object? sender, FileSystemEventArgs e)
    {
        if (!IsMarkdownFile(e.FullPath))
            return;

        QueueAction(PendingAction.Refresh);
    }

    private void OnCreated(object? sender, FileSystemEventArgs e)
    {
        QueueAction(PendingAction.Rebuild);
    }

    private void OnDeleted(object? sender, FileSystemEventArgs e)
    {
        QueueAction(PendingAction.Rebuild);
    }

    private void OnRenamed(object? sender, RenamedEventArgs e)
    {
        QueueAction(PendingAction.Rebuild);
    }

    private void OnError(object? sender, ErrorEventArgs e)
    {
        QueueAction(PendingAction.Rebuild);
    }

    private void QueueAction(PendingAction action)
    {
        lock (syncRoot)
        {
            if (action > pendingAction)
                pendingAction = action;

            debounceTimer?.Dispose();
            debounceTimer = new Timer(
                _ => RunPendingAction(),
                null,
                500,
                Timeout.Infinite);
        }
    }

    private void RunPendingAction()
    {
        PendingAction actionToRun;

        lock (syncRoot)
        {
            actionToRun = pendingAction;
            pendingAction = PendingAction.None;

            debounceTimer?.Dispose();
            debounceTimer = null;
        }

        if (actionToRun == PendingAction.Refresh)
            siteBuilder.Build();
        else if (actionToRun == PendingAction.Rebuild)
            siteBuilder.Build(forceRebuild: true);
    }

    private static bool IsMarkdownFile(string path)
    {
        var extension = Path.GetExtension(path);

        return extension.Equals(".md", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".markdown", StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        debounceTimer?.Dispose();

        if (watcher is null)
            return;

        watcher.EnableRaisingEvents = false;
        watcher.Changed -= OnChanged;
        watcher.Created -= OnCreated;
        watcher.Deleted -= OnDeleted;
        watcher.Renamed -= OnRenamed;
        watcher.Error -= OnError;
        watcher.Dispose();
    }

    private enum PendingAction
    {
        None = 0,
        Refresh = 1,
        Rebuild = 2
    }
}