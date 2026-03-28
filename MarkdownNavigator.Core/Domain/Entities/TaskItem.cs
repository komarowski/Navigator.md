namespace MarkdownNavigator.Core.Domain;

public class TaskItem
{
    public required string Name { get; set; }

    public required FileInfo File { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Open;
}