namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// Scans markdown files in the _tasks/ feature area for task documents.
/// Extracts task metadata from YAML front matter.
/// </summary>
public interface ITaskScanner
{
    /// <summary>
    /// Scan _tasks/ folder specifically for task documents.
    /// Much more efficient than full tree traversal.
    /// </summary>
    /// <param name="rootFolder">Tasks folder path</param>
    /// <returns>Enumerable of task items, ordered by status, due date, priority</returns>
    IEnumerable<TaskItem> ScanForTasks(string rootFolder);
}
