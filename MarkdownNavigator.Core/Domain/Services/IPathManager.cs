namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// Defines file path management operations for output generation.
/// Handles path construction, relative URL calculations, and folder filtering based on exclusion rules.
/// </summary>
public interface IPathManager
{
    /// <summary>
    /// Gets the source folder where markdown files are located.
    /// </summary>
    public string SourceFolder { get; }

    /// <summary>
    /// Gets the full path to the root index.html (help page).
    /// </summary>
    public string IndexPath { get; }

    /// <summary>
    /// Gets the full path to the wiki folder (_wiki).
    /// </summary>
    public string WikiFolder { get; }

    /// <summary>
    /// Gets the full path to the tasks output folder.
    /// </summary>
    public string TasksFolder { get; }

    /// <summary>
    /// Gets the full path to the Q&A output folder.
    /// </summary>
    public string QaFolder { get; }

    /// <summary>
    /// Gets the full path to the assets folder.
    /// </summary>
    public string AssetsFolder { get; }

    /// <summary>
    /// Converts a file path's extension to HTML (.html).
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <returns>Same path with .html extension</returns>
    public string ChangeExtensionToHtml(string path);

    /// <summary>
    /// Gets the relative HTML file path from source folder root.
    /// Converts the file path to relative format and changes extension to .html.
    /// </summary>
    /// <param name="filePath">Full path to the file</param>
    /// <returns>Relative HTML path (e.g., "docs/guide.html")</returns>
    public string GetRelativeHtmlPath(string filePath);

    /// <summary>
    /// Gets the relative HTML file path from a custom root folder.
    /// </summary>
    /// <param name="filePath">Full path to the file</param>
    /// <param name="basePath">Custom base path to calculate relative path from</param>
    /// <returns>Relative HTML path from the specified base folder</returns>
    public string GetRelativeHtmlPath(string filePath, string basePath);
}
