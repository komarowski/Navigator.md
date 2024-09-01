using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Infrastructure;
using MarkdownNavigator.Infrastructure.Services;

namespace MarkdownNavigator.Domain.Services
{
  public class TreeStructureService(IAppSettings settings) : ITreeStructureService
  {
    private readonly IAppSettings settings = settings;

    private readonly string[] excludeFolders = [FolderReservedNames.AssetsFolder];

    private readonly string[] excludeFromTreeViewFiles = ["index.md", "help.md"];

    public TreeStructure WalkDirectoryTree(DirectoryInfo root, TreeStructure tree, bool refreshAll)
    {
      FileInfo[]? files = null;
      try
      {
        files = root.GetFiles("*.*");
      }
      catch (UnauthorizedAccessException ex)
      {
        ConsoleService.WriteLog(ex.Message, LogType.Error);
      }
      catch (DirectoryNotFoundException ex)
      {
        ConsoleService.WriteLog(ex.Message, LogType.Error);
      }

      if (files is not null)
      {
        foreach (FileInfo file in files)
        {
          ProcessFile(file, tree, refreshAll);
        }

        DirectoryInfo[] subDirs = root.GetDirectories();
        foreach (DirectoryInfo subDir in subDirs)
        {
          if (excludeFolders.Contains(subDir.Name))
          {
            continue;
          }
          var nodeId = GetNodeId(subDir.FullName);
          var parentNode = tree.CurrentNode;
          tree.CurrentNode = tree.AddFolderNode(nodeId, subDir.Name);
          tree = WalkDirectoryTree(subDir, tree, refreshAll);
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
    /// <param name="forceRefreshAll">Update all files anyway.</param>
    private void ProcessFile(FileInfo file, TreeStructure tree, bool forceRefreshAll)
    {
      if (file.Extension == FileExtensionService.ExtensionMarkdown)
      {
        var htmlFile = new FileInfo(FileExtensionService.MarkdownToHtml(file.FullName));
        if (!htmlFile.Exists
          || file.LastWriteTimeUtc > htmlFile.LastWriteTimeUtc
          || forceRefreshAll)
        {
          tree.AddMarkdownToUpdate(file.FullName);
        }

        if (excludeFromTreeViewFiles.Contains(file.Name) 
          || file.Directory!.Name.StartsWith('_'))
        {
          return;
        }

        var nodeId = GetNodeId(file.FullName);
        var title = GetFileTitle(file);
        tree.AddFileNode(nodeId, title);
      }
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
