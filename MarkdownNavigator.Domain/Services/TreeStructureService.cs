using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Infrastructure;
using MarkdownNavigator.Infrastructure.Services;

namespace MarkdownNavigator.Domain.Services
{
  public class TreeStructureService(IAppSettings settings) : ITreeStructureService
  {
    private readonly IAppSettings settings = settings;

    private readonly string[] excludeFolders = [FolderReservedNames.AssetsFolder, FolderReservedNames.ExportFolder];

    private readonly string[] excludeFileExtensions = [".js", ".html", ".css"];

    private readonly string[] excludeMarkdownFiles = ["index.md", "help.md"];

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
          var folderCode = GetPathCode(subDir.FullName);
          var parentNode = tree.CurrentNode;
          tree.CurrentNode = tree.AddFolderNode(folderCode, subDir.Name);
          tree = WalkDirectoryTree(subDir, tree, refreshAll);
          tree.CurrentNode = parentNode;
        }
      }

      return tree;
    }

    public string GetPathCode(string path)
    {
      path = path.Replace(ExtensionService.ExtensionMarkdown, string.Empty);
      var relativePath = GetRelativePath(path);
      return relativePath
        .ToLower()
        .Replace(' ', '-')
        .Replace("\\", "__");
    }

    /// <summary>
    /// Processes the file.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="tree">Folder structure.</param>
    /// <param name="forceRefreshAll">Update all files anyway.</param>
    private void ProcessFile(FileInfo file, TreeStructure tree, bool forceRefreshAll)
    {
      if (file.Extension == ExtensionService.ExtensionMarkdown)
      {
        var htmlCode = GetPathCode(file.FullName);
        var htmlFile = settings.EnableExport
          ? new FileInfo(Path.Combine(settings.SourceFolder, FolderReservedNames.ExportFolder, ExtensionService.GetHtml(htmlCode)))
          : new FileInfo(ExtensionService.MarkdownToHtml(file.FullName));

        if (!htmlFile.Exists
          || file.LastWriteTimeUtc > htmlFile.LastWriteTimeUtc
          || forceRefreshAll)
        {
          tree.AddMarkdownToUpdate(file.FullName, htmlCode);
        }

        if (excludeMarkdownFiles.Contains(file.Name))
        {
          return;
        }

        var title = GetFileTitle(file);
        var href = settings.EnableExport
          ? ExtensionService.GetHtml(htmlCode)
          : "file:///" + htmlFile.FullName.Replace('\\', '/');
        tree.AddFileNode(htmlCode, title, href);

        return;
      }

      if (settings.EnableExport && !excludeFileExtensions.Contains(file.Extension))
      {
        var targetPath = Path.Combine(settings.SourceFolder, FolderReservedNames.ExportFolder, file.Name);
        var targetFile = new FileInfo(targetPath);
        if (!targetFile.Exists
          || file.LastWriteTimeUtc > targetFile.LastWriteTimeUtc
          || forceRefreshAll)
        {
          tree.FilesToCopy.Add(new ImageFile(targetPath, file.FullName));
        }
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
