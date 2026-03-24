namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// Scans markdown files in the _qa/ feature area for Q&A documents.
/// Extracts question, context, and tags from YAML front matter.
/// </summary>
public interface IQaScanner
{
    /// <summary>
    /// Scan _qa/ folder specifically for Q&A documents.
    /// </summary>
    /// <param name="rootFolder">Q&A folder path</param>
    /// <returns>Enumerable of Q&A items</returns>
    IEnumerable<QaItem> ScanForQa(string rootFolder);
}
