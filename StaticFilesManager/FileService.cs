namespace StaticFilesManager
{
  public class FileService
  {
    private const string ExcludeFolder = "assets";
    private static readonly string[] ExcludeFiles = ["main.js"];

    private string? assetsDirectoryPath;
    private string? reactBuildDirectoryPath;
    private string? reactPublicDirectoryPath;
    private string? webDirectoryPath;
    private string? webAssetsDirectoryPath;

    private string AssetsDirectoryPath
    {
      get
      { 
        if (string.IsNullOrEmpty(assetsDirectoryPath))
        {
          assetsDirectoryPath = GetRelativeDirectoryPath("MarkdownNavigator.Infrastructure\\Resources\\assets");
        }

        return assetsDirectoryPath; 
      }
    }

    private string ReactBuildDirectoryPath
    {
      get
      {
        if (string.IsNullOrEmpty(reactBuildDirectoryPath))
        {
          reactBuildDirectoryPath = GetRelativeDirectoryPath("MarkdownNavigator.React\\build");
        }

        return reactBuildDirectoryPath;
      }
    }

    private string ReactPublicDirectoryPath
    {
      get
      {
        if (string.IsNullOrEmpty(reactPublicDirectoryPath))
        {
          reactPublicDirectoryPath = GetRelativeDirectoryPath("MarkdownNavigator.React\\public\\assets");
        }

        return reactPublicDirectoryPath;
      }
    }

    private string WebDirectoryPath
    {
      get
      {
        if (string.IsNullOrEmpty(webDirectoryPath))
        {
          webDirectoryPath = GetRelativeDirectoryPath("MarkdownNavigator.Web\\wwwroot");
        }

        return webDirectoryPath;
      }
    }

    private string WebAssetsDirectoryPath
    {
      get
      {
        if (string.IsNullOrEmpty(webAssetsDirectoryPath))
        {
          webAssetsDirectoryPath = GetRelativeDirectoryPath("MarkdownNavigator.Web\\wwwroot\\assets");
        }

        return webAssetsDirectoryPath;
      }
    }

    public void UpdateStaticFiles()
    {
      var assetsFiles = GetSharedAssetsFiles();
      var reactBuildFiles = GetReactBuildFiles();

      CopyFiles(assetsFiles, AssetsDirectoryPath, ReactPublicDirectoryPath);
      CopyFiles(assetsFiles, AssetsDirectoryPath, WebAssetsDirectoryPath);
      CopyFiles(reactBuildFiles, ReactBuildDirectoryPath, WebDirectoryPath);
    }

    private IEnumerable<FileInfo> GetSharedAssetsFiles()
    {
      if (!TryGetDirectoryInfo(AssetsDirectoryPath, out var directory))
      {
        return Enumerable.Empty<FileInfo>();
      }

      return directory
        .GetFiles("*", SearchOption.AllDirectories)
        .Where(file => !ExcludeFiles.Contains(file.Name));
    }

    private IEnumerable<FileInfo> GetReactBuildFiles()
    {
      if (!TryGetDirectoryInfo(ReactBuildDirectoryPath, out var directory))
      {
        return Enumerable.Empty<FileInfo>();
      }

      return directory
        .GetFiles("*", SearchOption.AllDirectories)
        .Where(file => !file.FullName.Contains($@"\{ExcludeFolder}\"));
    }

    private static void CopyFiles(IEnumerable<FileInfo> sourceFiles, string sourceDir, string destinationDir)
    {
      foreach (var sourceFile in sourceFiles)
      {
        var destinationSubDirPath = sourceFile.DirectoryName.Replace(sourceDir, destinationDir);
        if (!Directory.Exists(destinationSubDirPath))
        {
          Directory.CreateDirectory(destinationSubDirPath);
        }

        var destinationFilePath = sourceFile.FullName.Replace(sourceDir, destinationDir);
        var destinationFile = new FileInfo(destinationFilePath);

        if (!destinationFile.Exists || sourceFile.LastWriteTime != destinationFile.LastWriteTime)
        {
          sourceFile.CopyTo(destinationFilePath, true);
        }
      }
    }

    private static bool TryGetDirectoryInfo(string path, out DirectoryInfo? directoryInfo)
    {
      directoryInfo = new DirectoryInfo(path);
      if (directoryInfo.Exists)
      {
        return true;
      }

      Console.WriteLine($"Directory '{path}' not found!");

      return false;
    }

    private static string GetRelativeDirectoryPath(string path)
    {
      var rootDirectory = GetParentDirectory(Directory.GetCurrentDirectory(), 4);
      return Path.Combine(rootDirectory, path);
    }

    private static string GetParentDirectory(string folderPath, int levelsUp)
    {
      var directoryInfo = new DirectoryInfo(folderPath);
      for (int i = 0; i < levelsUp; i++)
      {
        directoryInfo = directoryInfo.Parent;
      }

      return directoryInfo.FullName;
    }
  }
}
