using MarkdownNavigator.Core.Domain;
using MarkdownNavigator.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace MarkdownNavigator.Core.Application;

/// <summary>
/// Orchestrates the site building process.
/// </summary>
public class SiteBuilder(
    ITreeBuilder treeBuilder,
    ITaskScanner taskScanner,
    IQaScanner qaScanner,
    IPathManager pathManager,
    ISiteGenerator siteGenerator,
    ILogger<SiteBuilder> logger) : ISiteBuilder
{
    private readonly ITreeBuilder treeBuilder = treeBuilder;
    private readonly ITaskScanner taskScanner = taskScanner;
    private readonly IQaScanner qaScanner = qaScanner;
    private readonly IPathManager pathManager = pathManager;
    private readonly ISiteGenerator siteGenerator = siteGenerator;
    private readonly ILogger<SiteBuilder> logger = logger;

    /// <summary>
    /// Builds the HTML site from the source markdown files.
    /// </summary>
    /// <param name="forceRebuild">If true, forces a rebuild of all content regardless of changes.</param>
    public void Build(bool forceRebuild = false)
    {
        if (!SourceFolderExists())
        {
            return;
        }

        var content = new SiteContent
        {
            WikiTree = BuildTree(),
            Tasks = ScanTasks(),
            QaItems = ScanQa(),
        };

        siteGenerator.GenerateWeb(content);
    }

    private bool SourceFolderExists()
    {
        var sourceFolder = pathManager.SourceFolder;
        if (!Directory.Exists(sourceFolder))
        {
            logger.LogInformation($"Source folder not found: {sourceFolder}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Builds the tree structure from the _wiki/ folder.
    /// </summary>
    private TreeStructure BuildTree()
    {
        var wikiFolder = pathManager.WikiFolder;
        if (!Directory.Exists(wikiFolder))
        {
            logger.LogInformation("No _wiki folder found");
            return new TreeStructure();
        }

        var tree = treeBuilder.GetTreeStructure(wikiFolder);
        logger.LogInformation("Found {MarkdownCount} markdown files in _wiki folder", tree.MarkdownFilesToConvert.Count);
        return tree;
    }

    /// <summary>
    /// Scans _tasks/ folder for task items.
    /// </summary>
    private List<TaskItem> ScanTasks()
    {
        var tasksFolder = pathManager.TasksFolder;
        if (!Directory.Exists(tasksFolder))
        {
            logger.LogInformation("No _tasks folder found");
            return [];
        }

        var tasks = taskScanner.ScanForTasks(tasksFolder).ToList();
        logger.LogInformation("Found {TaskCount} tasks", tasks.Count);

        return tasks;
    }

    /// <summary>
    /// Scans _qa/ folder for Q&amp;A items.
    /// </summary>
    private List<QaItem> ScanQa()
    {
        var qaFolder = pathManager.QaFolder;
        if (!Directory.Exists(qaFolder))
        {
            logger.LogInformation("No _qa folder found");
            return [];
        }

        var qaItems = qaScanner.ScanForQa(qaFolder).ToList();
        logger.LogInformation("Found {QaCount} Q&A items", qaItems.Count);

        return qaItems;
    }
}