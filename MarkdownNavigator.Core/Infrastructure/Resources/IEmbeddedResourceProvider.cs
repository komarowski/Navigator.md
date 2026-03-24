namespace MarkdownNavigator.Core.Infrastructure;

/// <summary>
/// Handles extraction of resources from assembly to file system while preserving folder structure.
/// </summary>
public interface IEmbeddedResourceProvider
{
    /// <summary>
    /// Copies all predefined resources to target folder, preserving original folder structure.
    /// Creates target directory if it doesn't exist. Logs warnings for missing resources.
    /// </summary>
    /// <param name="targetFolder">Base folder where resources will be extracted</param>
    void CopyPredefinedResources(string targetFolder);
}
