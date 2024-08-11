using System.Reflection;

namespace MarkdownNavigator.Infrastructure.Services
{
  /// <summary>
  /// Resource file management service.
  /// </summary>
  public class ResourceService
  {
    private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

    private const char Dot = '.';
    private const string StartPath = "MarkdownNavigator.Infrastructure.Resources";

    /// <summary>
    /// Gets html tempalte.
    /// </summary>
    /// <param name="isExport">Export mode.</param>
    /// <returns>Html tempalte.</returns>
    public static string GetTemplate(bool isExport)
    {
      var templateName = isExport ? "export" : "default";
      var resourceName = GetFullResourceName($"templates.{templateName}.html");
      return GetEmbeddedResource(resourceName);
    }

    /// <summary>
    /// Gets index.md content text.
    /// </summary>
    /// <returns>index.md content text.</returns>
    public static string GetIndexMdContent()
    {
      var resourceName = GetFullResourceName("content.index.md");
      return GetEmbeddedResource(resourceName);
    }

    /// <summary>
    /// Gets help.md content text.
    /// </summary>
    /// <returns>help.md content text.</returns>
    public static string GetHelpMdContent()
    {
      var resourceName = GetFullResourceName("content.help.md");
      return GetEmbeddedResource(resourceName);
    }

    /// <summary>
    /// Copies all asset files to the target folder.
    /// </summary>
    /// <param name="targetFolder">Target folder.</param>
    public static void CopyAllAssets(string targetFolder)
    {
      if (Directory.Exists(Path.Combine(targetFolder, FolderReservedNames.AssetsFolder)))
      {
        return;
      }

      CopyResourceFiles(targetFolder, FolderReservedNames.AssetsFolder);
    }

    /// <summary>
    /// Copies resource files to the target folder.
    /// </summary>
    /// <param name="targetFolder">Target folder.</param>
    /// <param name="resourceFolder">Resource folder for copying.</param>
    public static void CopyResourceFiles(string targetFolder, string resourceFolder)
    {
      var resourceFolderName = GetFullResourceName(resourceFolder);
      var resourceNames = ExecutingAssembly.GetManifestResourceNames()
        .Where(name => name.StartsWith(resourceFolderName));

      if (resourceNames is null)
      {
        ConsoleService.WriteLog($"Resources not found", LogType.Warning);
        return;
      };

      foreach (var resourceName in resourceNames)
      {
        CopyEmbeddedResourceToFile(resourceName, targetFolder);
      }
    }

    /// <summary>
    /// Creates a directory if it does not exist.
    /// </summary>
    /// <param name="directory">Вirectory path.</param>
    public static void EnsureDirectoryExists(string directory)
    {
      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }
    }

    /// <summary>
    /// Copies resource file to the target folder.
    /// </summary>
    /// <param name="resourceName">Full resource file name.</param>
    /// <param name="targetFolder">Target folder.</param>
    private static void CopyEmbeddedResourceToFile(string resourceName, string targetFolder)
    {
      if (ExecutingAssembly is null)
      {
        ConsoleService.WriteLog($"ExecutingAssembly is null", LogType.Warning);
        return;
      }

      using Stream? resourceStream = ExecutingAssembly.GetManifestResourceStream(resourceName);

      if (resourceStream is null)
      {
        ConsoleService.WriteLog($"Resource '{resourceName}' not found", LogType.Warning);
        return;
      };

      var resourcePath = GetPathFromResourceName(resourceName);
      var targetFilePath = Path.Combine(targetFolder, resourcePath);
      EnsureFileDirectoryExists(targetFilePath);

      using FileStream fileStream = new(targetFilePath, FileMode.Create, FileAccess.Write);
      resourceStream.CopyTo(fileStream);
    }

    /// <summary>
    /// Gets full resource file name.
    /// </summary>
    /// <param name="resourceName">Resource file name.</param>
    /// <returns>Full resource file name.</returns>
    private static string GetFullResourceName(string resourceName)
    {
      return string.Join(Dot, StartPath, resourceName);
    }

    /// <summary>
    /// Gets file system path from resource file path.
    /// </summary>
    /// <param name="resourceName">Resource file path.</param>
    /// <returns>File system path.</returns>
    private static string GetPathFromResourceName(string resourceName)
    {
      var resourcesStartPath = StartPath + Dot;
      var relativeResourceName = resourceName.StartsWith(resourcesStartPath) 
        ? resourceName[resourcesStartPath.Length..] 
        : resourceName;

      int lastDotIndex = relativeResourceName.LastIndexOf(Dot);
      if (lastDotIndex == -1)
      {
        return resourceName;
      }

      var beforeLastDot = relativeResourceName[..lastDotIndex].Replace(Dot, '\\');
      var afterLastDot = relativeResourceName[lastDotIndex..];

      return beforeLastDot + afterLastDot;
    }

    /// <summary>
    /// Gets resource file content.
    /// </summary>
    /// <param name="resourceName">Full resource file name.</param>
    /// <returns>Resource file content.</returns>
    private static string GetEmbeddedResource(string resourceName)
    {
      if (ExecutingAssembly is null)
      {
        ConsoleService.WriteLog($"ExecutingAssembly is null", LogType.Warning);
        return string.Empty;
      }

      using Stream? resourceStream = ExecutingAssembly.GetManifestResourceStream(resourceName);

      if (resourceStream is null)
      {
        ConsoleService.WriteLog($"Resource '{resourceName}' not found", LogType.Warning);
        return string.Empty;
      };

      using StreamReader reader = new(resourceStream);
      return reader.ReadToEnd();
    }

    /// <summary>
    /// Creates a file directory if it does not exist.
    /// </summary>
    /// <param name="filePath">File path.</param>
    private static void EnsureFileDirectoryExists(string filePath)
    {
      var directory = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directory))
      {
        EnsureDirectoryExists(directory);
      }
    }
  }
}
