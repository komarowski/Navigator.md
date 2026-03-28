using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MarkdownNavigator.Core.Infrastructure;

/// <inheritdoc />
public class EmbeddedResourceProvider(ILogger<EmbeddedResourceProvider> logger) : IEmbeddedResourceProvider
{
    private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

    private const string ResourcePrefix = "MarkdownNavigator.Core.Resources.";

    private static readonly string[] Resources =
    [
        "MarkdownNavigator.Core.Resources.assets.core.css",
        "MarkdownNavigator.Core.Resources.assets.core.js",
        "MarkdownNavigator.Core.Resources.assets.logo.ico",
        "MarkdownNavigator.Core.Resources.assets.fonts.Roboto-Regular.woff2",
        "MarkdownNavigator.Core.Resources.assets.fonts.Roboto-Medium.woff2",
        "MarkdownNavigator.Core.Resources.assets.fonts.Roboto-Bold.woff2",
        "MarkdownNavigator.Core.Resources.assets.plugins.code.plugin.css",
        "MarkdownNavigator.Core.Resources.assets.plugins.code.plugin.js",
        "MarkdownNavigator.Core.Resources.assets.plugins.prism.plugin.css",
        "MarkdownNavigator.Core.Resources.assets.plugins.prism.plugin.js",
        "MarkdownNavigator.Core.Resources.assets.plugins.slider.plugin.css",
        "MarkdownNavigator.Core.Resources.assets.plugins.slider.plugin.js"
    ];

    public void CopyPredefinedResources(string sourceFolder)
    {
        if (ExecutingAssembly is null)
        {
            logger.LogError("ExecutingAssembly is null");
            return;
        }

        if (!Directory.Exists(sourceFolder))
        {
            Directory.CreateDirectory(sourceFolder);
        }

        foreach (var resourceName in Resources)
        {
            ExtractResourceToFile(resourceName, sourceFolder);
        }
    }

    /// <summary>
    /// Converts a fully qualified resource name to a relative file path.
    /// Extracts the folder structure after "Resources." prefix.
    /// Example: "MarkdownNavigator.Core.Resources.assets.core.css" -> "assets/core.css"
    /// </summary>
    private static string ConvertResourceNameToPath(string resourceName)
    {
        var pathToProcess = resourceName.StartsWith(ResourcePrefix)
            ? resourceName[ResourcePrefix.Length..]
            : resourceName;

        // Find the last dot (file extension separator)
        var lastDotIndex = pathToProcess.LastIndexOf('.');
        if (lastDotIndex > 0)
        {
            // Replace dots before the extension with path separators
            var withoutExtension = pathToProcess[..lastDotIndex].Replace(".", Path.DirectorySeparatorChar.ToString());
            var extension = pathToProcess[lastDotIndex..];
            return withoutExtension + extension;
        }

        // No extension found, replace all dots
        return pathToProcess.Replace(".", Path.DirectorySeparatorChar.ToString());
    }

    /// <summary>
    /// Extracts a single resource from assembly to file system.
    /// Maintains the resource's namespace structure as folder hierarchy.
    /// </summary>
    /// <param name="resourceName">Fully qualified resource name from assembly</param>
    /// <param name="targetFolder">Base folder where resource should be extracted</param>
    private void ExtractResourceToFile(string resourceName, string targetFolder)
    {
        try
        {
            using Stream? resourceStream = ExecutingAssembly.GetManifestResourceStream(resourceName);
            if (resourceStream is null)
            {
                logger.LogWarning($"Resource '{resourceName}' not found in assembly");
                return;
            }

            // Convert resource name to file path (e.g., "Namespace.assets.core.css" -> "assets/core.css")
            var relativePath = ConvertResourceNameToPath(resourceName);
            var targetFilePath = Path.Combine(targetFolder, relativePath);

            var targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            using FileStream fileStream = new(targetFilePath, FileMode.Create, FileAccess.Write);
            resourceStream.CopyTo(fileStream);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to extract resource '{resourceName}': {ex.Message}");
        }
    }
}
