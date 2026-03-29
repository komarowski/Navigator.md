namespace MarkdownNavigator.Core.Domain;

public class TaskScanner : ITaskScanner
{
    public IEnumerable<TaskItem> ScanForTasks(string tasksFolder)
    {
        var tasks = new List<TaskItem>();

        var tasksFolderInfo = new DirectoryInfo(tasksFolder);
        if (!tasksFolderInfo.Exists)
        {
            return tasks;
        }

        var files = tasksFolderInfo.GetFiles("*.md", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            string? taskName = null;
            var taskStatus = TaskItemStatus.Open;

            if (MarkdownManager.TryReadFrontMatter<TaskFrontMatter>(file, out var taskFrontMatter)
                && taskFrontMatter != null)
            {
                taskName = taskFrontMatter.Name;
                taskStatus = taskFrontMatter.Status ?? TaskItemStatus.Open;
            }

            if (string.IsNullOrEmpty(taskName))
            {
                taskName = MarkdownManager.ReadTitleOrDefault(
                    file,
                    Path.GetFileNameWithoutExtension(file.Name),
                    8);
            }

            var task = new TaskItem()
            {
                Name = taskName,
                File = file,
                Status = taskStatus
            };

            tasks.Add(task);
        }

        return tasks
            .OrderBy(task => task.Status)
            .ThenByDescending(task => task.File.CreationTimeUtc);
    }
}
