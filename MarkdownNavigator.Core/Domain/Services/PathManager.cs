namespace MarkdownNavigator.Core.Domain;

/// <inheritdoc />
public class PathManager(AppSettings settings) : IPathManager
{
    public static readonly string AssetsFolderName = "assets";
    public static readonly string WikiFolderName = "_wiki";
    public static readonly string TasksFolderName = "_tasks";
    public static readonly string QaFolderName = "_qa";

    public string SourceFolder => settings.SourceFolder;

    public string IndexPath => Path.Combine(SourceFolder, "index.html");

    public string WikiFolder => Path.Combine(SourceFolder, WikiFolderName);

    public string TasksFolder => Path.Combine(SourceFolder, TasksFolderName);

    public string QaFolder => Path.Combine(SourceFolder, QaFolderName);

    public string AssetsFolder => Path.Combine(SourceFolder, AssetsFolderName);

    public string ChangeExtensionToHtml(string markdownPath)
    {
        return Path.ChangeExtension(markdownPath, ".html");
    }

    public string GetRelativeHtmlPath(string filePath)
    {
        return GetRelativeHtmlPath(filePath, SourceFolder);
    }

    public string GetRelativeHtmlPath(string filePath, string basePath)
    {
        var relativePath = Path.GetRelativePath(basePath, filePath).Replace("\\", "/");
        return ChangeExtensionToHtml(relativePath);
    }
}
