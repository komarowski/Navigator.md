using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Infrastructure.Services;
using System.Text.Json;

namespace MarkdownNavigator.Domain.Services
{
  public class TreeStructureService(IAppSettings settings) : ITreeStructureService
  {
    private readonly IAppSettings settings = settings;

    private readonly string[] excludeFromTreeFiles = ["index.md", "help.md"];

    public TreeStructure WalkDirectoryTree(
      DirectoryInfo root, 
      TreeStructure tree,
      IgnoreRules? ignoreRules = null,
      bool forceRefresh = false, 
      bool excludedFromTree = false, 
      bool isRoot = false)
    {
      ignoreRules ??= GetIgnoreRules();
      FileInfo[]? files = null;
      try
      {
        files = GetMarkdownFiles(root, ignoreRules, isRoot);
      }
      catch (Exception ex) when (ex is UnauthorizedAccessException or DirectoryNotFoundException)
      {
        ConsoleService.WriteLog(ex.Message, LogType.Error);
      }

      if (files is not null)
      {
        foreach (FileInfo file in files)
        {
          var excludedFromTreeFile = isRoot && excludeFromTreeFiles.Contains(file.Name) 
            || excludedFromTree;

          ProcessFile(file, tree, forceRefresh, excludedFromTreeFile);
        }

        DirectoryInfo[] subDirs = GetMarkdownDirectories(root, ignoreRules, isRoot);
        foreach (DirectoryInfo subDir in subDirs)
        {
          if (!excludedFromTree)
          {
            excludedFromTree = IsExcludedFromTreeFolder(subDir.Name);
          }

          var parentNode = tree.CurrentNode;
          if (!excludedFromTree)
          {
            var nodeId = GetNodeId(subDir.FullName);
            tree.CurrentNode = tree.AddFolderNode(nodeId, subDir.Name);
          }

          tree = WalkDirectoryTree(subDir, tree, ignoreRules, forceRefresh, excludedFromTree);
          tree.CurrentNode = parentNode;
        }
      }

      return tree;
    }

    public string GetNodeId(string path)
    {
      var htmlPath = FileExtensionService.MarkdownToHtml(path);
      var relativePath = GetRelativePath(htmlPath);
      return relativePath.Replace("\\", "/");
    }

    public string GetRelativePathForNode(string markdownPath)
    {
      var result = Path.GetRelativePath(markdownPath, settings.SourceFolder).Replace("\\", "/");
      return result.Length <= 2
        ? string.Empty
        : result[..^2];
    }

    /// <summary>
    /// Processes the file.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="tree">Folder structure.</param>
    /// <param name="forceRefresh">Update all files anyway.</param>
    /// <param name="excludedFromTree">Exclude file from the tree structure.</param>
    private void ProcessFile(FileInfo file, TreeStructure tree, bool forceRefresh, bool excludedFromTree)
    {
      var htmlFile = new FileInfo(FileExtensionService.MarkdownToHtml(file.FullName));
      if (!htmlFile.Exists
        || file.LastWriteTimeUtc > htmlFile.LastWriteTimeUtc
        || forceRefresh)
      {
        tree.AddMarkdownToUpdate(file.FullName);
      }

      if (excludedFromTree)
      {
        return;
      }

      var nodeId = GetNodeId(file.FullName);
      var title = GetFileTitle(file);
      tree.AddFileNode(nodeId, title);
    }

    /// <summary>
    /// Gets the path to the file relative to the source folder.
    /// </summary>
    /// <param name="path">Full path.</param>
    /// <returns>Relative path.</returns>
    private string GetRelativePath(string path)
    {
      return Path.GetRelativePath(settings.SourceFolder, path);
    }

    /// <summary>
    /// Loads ignore rules from the <c>ignore.json</c> file in the source folder.
    /// If the file does not exist or contains invalid JSON, default ignore rules are returned.
    /// </summary>
    /// <returns>An <see cref="IgnoreRules"/> instance with configured or default values.</returns>
    private IgnoreRules GetIgnoreRules()
    {
      var filePath = Path.Combine(settings.SourceFolder, "ignore.json");
      if (!File.Exists(filePath))
      {
        return new IgnoreRules();
      }

      try
      {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<IgnoreRules>(json) ?? new IgnoreRules();
      }
      catch (JsonException)
      {
        return new IgnoreRules();
      }
    }

    /// <summary>
    /// Checks that the node is not displayed in the tree view.
    /// </summary>
    /// <param name="folderName">Node folder name.</param>
    /// <returns>True if the node should be ignored.</returns>
    private static bool IsExcludedFromTreeFolder(string folderName)
    {
      return folderName.StartsWith('_');
    }

    /// <summary>
    /// Retrieves all Markdown files (<c>*.md</c>) from the specified directory,
    /// excluding any files based on the provided <see cref="IgnoreRules"/>.
    /// </summary>
    /// <param name="directory">The directory to search for Markdown files.</param>
    /// <param name="ignoreRules">The ignore rules to apply when filtering files.</param>
    /// <param name="isRoot">First call in the recursive tree walk.</param>
    /// <returns>An array of <see cref="FileInfo"/>.</returns>
    private static FileInfo[] GetMarkdownFiles(DirectoryInfo directory, IgnoreRules ignoreRules, bool isRoot)
    {
      if (isRoot)
      {
        return directory
          .GetFiles("*.md")
          .Where(file => !ignoreRules.ExcludeFiles.Contains(file.Name, StringComparer.OrdinalIgnoreCase))
          .ToArray();
      }

      return directory.GetFiles("*.md");
    }

    /// <summary>
    /// Retrieves all subdirectories from the specified directory,
    /// excluding any folders based on the provided <see cref="IgnoreRules"/>.
    /// </summary>
    /// <param name="directory">The parent directory to search for subdirectories.</param>
    /// <param name="ignoreRules">The ignore rules to apply when filtering files.</param>
    /// <param name="isRoot">First call in the recursive tree walk.</param>
    /// <returns>An array of <see cref="DirectoryInfo"/>.</returns>
    private static DirectoryInfo[] GetMarkdownDirectories(DirectoryInfo directory, IgnoreRules ignoreRules, bool isRoot)
    {
      if (isRoot)
      {
        return directory
          .GetDirectories()
          .Where(dir => !ignoreRules.ExcludeFolders.Contains(dir.Name, StringComparer.OrdinalIgnoreCase))
          .ToArray();
      }

      return directory.GetDirectories();
    }

    /// <summary>
    /// Gets file title from # heading.
    /// </summary>
    /// <param name="file">File information.</param>
    /// <returns>Title.</returns>
    private static string GetFileTitle(FileInfo file)
    {
      using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      using (var streamReader = new StreamReader(fileStream))
      {
        for (int i = 0; i < 3; i++)
        {
          var line = streamReader.ReadLine();
          if (line is not null && line.TrimStart().StartsWith("# "))
          {
            return line.Trim()[2..];
          }
        }
      }
      return file.Name;
    }
  }
}
