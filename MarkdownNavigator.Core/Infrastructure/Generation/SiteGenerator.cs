using MarkdownNavigator.Core.Domain;
using System.Text;
using System.Text.Json;

namespace MarkdownNavigator.Core.Infrastructure;

public class SiteGenerator(
    IPathManager pathManager, 
    IEmbeddedResourceProvider embeddedResourceProvider) : ISiteGenerator
{
    private static readonly JsonSerializerOptions CachedJsonOptions = new()
    {
        WriteIndented = true
    };

    public void GenerateWeb(SiteContent content, bool forceRebuild = false)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content), "Content cannot be null");
        }

        if (content.WikiTree == null || content.Tasks == null || content.QaItems == null)
        {
            throw new ArgumentException("Content properties cannot be null", nameof(content));
        }

        // Step 1: Process wiki pages
        CreateMissingIndexFiles(content.WikiTree.IndexFilesToGenerate);
        ConvertMarkdownToHtml(content.WikiTree.MarkdownFilesToConvert, forceRebuild);

        // Step 2: Process tasks pages
        ProcessTasks(content.Tasks, forceRebuild);

        // Step 3: Process Q&A pages
        ProcessQaItems(content.QaItems, forceRebuild);

        // Step 4: Generate data.js with tree structure, tasks, and Q&A metadata
        GenerateDataJs(content.WikiTree, content.Tasks, content.QaItems);

        // Step 5: Copy assets if needed
        CopyAssets();

        // Step 6: Generate root index.html (home page)
        GenerateHomePage();
    }

    /// <summary>
    /// Creates index.md files for folders that don't have one.
    /// </summary>
    private void CreateMissingIndexFiles(List<FileInfo> files)
    {
        foreach (var file in files)
        {
            if (!file.Exists)
            {
                var content = MarkdownTemplateGenerator.GetWikiFolderTemplate(file.Directory?.Name ?? "Folder");
                File.WriteAllText(file.FullName, content);
                file.Refresh();
            }
        }

        ConvertMarkdownToHtml(files, forceRebuild: true);
    }

    /// <summary>
    /// Converts markdown files to HTML using the markdown manager and wraps with site template.
    /// Only converts files that have been modified since the last conversion, unless forceRebuild is true.
    /// </summary>
    private void ConvertMarkdownToHtml(List<FileInfo> files, bool forceRebuild = false)
    {
        foreach (var file in files)
        {
            if (!file.Exists)
            {
                continue;
            }

            var htmlPath = pathManager.ChangeExtensionToHtml(file.FullName);
            
            // Skip conversion unless forced or HTML doesn't exist or markdown is newer
            if (!forceRebuild && File.Exists(htmlPath))
            {
                var htmlFileInfo = new FileInfo(htmlPath);
                if (htmlFileInfo.LastWriteTimeUtc >= file.LastWriteTimeUtc)
                {
                    continue;
                }
            }

            var mdContent = File.ReadAllText(file.FullName, Encoding.UTF8);
            var htmlContent = MarkdownManager.ConvertToHtml(mdContent);

            var relativeHtmlPath = pathManager.GetRelativeHtmlPath(file.FullName);
            var wrappedHtml = HtmlTemplateGenerator.WrapContent(htmlContent, relativeHtmlPath);

            File.WriteAllText(htmlPath, wrappedHtml, Encoding.UTF8);
        }
    }

    private void ProcessTasks(List<TaskItem> tasks, bool forceRebuild = false)
    {
        if (tasks.Count == 0)
        {
            return;
        }

        if (!Directory.Exists(pathManager.TasksFolder))
        {
            Directory.CreateDirectory(pathManager.TasksFolder);
        }

        var files = tasks
            .Select(task => task.File)
            .ToList();

        ConvertMarkdownToHtml(files, forceRebuild);
    }

    private void ProcessQaItems(List<QaItem> qaItems, bool forceRebuild = false)
    {
        if (qaItems.Count == 0)
        {
            return;
        }

        if (!Directory.Exists(pathManager.QaFolder))
        {
            Directory.CreateDirectory(pathManager.QaFolder);
        }

        var files = qaItems
            .Select(qa => qa.File)
            .ToList();

        ConvertMarkdownToHtml(files, forceRebuild);
    }

    /// <summary>
    /// Generates data.js file with tree structure, tasks, and Q&A metadata for client-side rendering.
    /// </summary>
    private void GenerateDataJs(TreeStructure tree, IEnumerable<TaskItem> tasks, IEnumerable<QaItem> qaItems)
    {
        // TODO: minimize json, js 
        var sourceFolder = pathManager.SourceFolder;
        Directory.CreateDirectory(sourceFolder);

        TransformNodePaths(tree.RootNode, pathManager.SourceFolder);
        var rootNodeJson = JsonSerializer.Serialize(tree.RootNode, CachedJsonOptions);

        var taskList = tasks.Select(t => new
        {
            name = t.Name,
            path = pathManager.GetRelativeHtmlPath(t.File.FullName, pathManager.TasksFolder),
            status = t.Status.ToString(),
            lastUpdate = t.File.LastWriteTimeUtc
        }).ToList();
        var tasksJson = JsonSerializer.Serialize(taskList, CachedJsonOptions);

        var qaList = qaItems.Select(q => new
        {
            question = q.Question,
            path = pathManager.GetRelativeHtmlPath(q.File.FullName, pathManager.QaFolder),
            popularity = q.Popularity,
            lastUpdate = q.File.LastWriteTimeUtc
        }).OrderByDescending(q => q.popularity).ToList();
        var qaJson = JsonSerializer.Serialize(qaList, CachedJsonOptions);

        var dataJsPath = Path.Combine(sourceFolder, "data.js");
        var content = $"const rootNode = {rootNodeJson};\nconst tasks = {tasksJson};\nconst qa = {qaJson};";

        File.WriteAllText(dataJsPath, content);
    }

    private void CopyAssets()
    {
        if (!Directory.Exists(pathManager.AssetsFolder))
        {
            embeddedResourceProvider.CopyPredefinedResources(pathManager.SourceFolder);
        }
    }

    /// <summary>
    /// Generates the root index.html (home/help page).
    /// </summary>
    private void GenerateHomePage()
    {
        var htmlContent = MarkdownManager.ConvertToHtml(MarkdownTemplateGenerator.GetIndexTemplate());
        var htmlPage = HtmlTemplateGenerator.WrapContent(htmlContent, "index.html");
        File.WriteAllText(pathManager.IndexPath, htmlPage);
    }

    /// <summary>
    /// Recursively transforms all node paths from full file paths to relative HTML paths.
    /// </summary>
    /// <param name="node">Node to transform (and its children).</param>
    /// <param name="sourceFolder">Source folder for relative path calculation.</param>
    private void TransformNodePaths(Node node, string sourceFolder)
    {
        if (node == null)
            return;

        if (!string.IsNullOrEmpty(node.Path))
        {
            node.Path = pathManager.GetRelativeHtmlPath(node.Path, sourceFolder);
        }

        // Recursively transform all children
        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                TransformNodePaths(child, sourceFolder);
            }
        }
    }
}
