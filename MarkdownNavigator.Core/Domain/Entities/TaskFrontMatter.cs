namespace MarkdownNavigator.Core.Domain;

public class TaskFrontMatter
{
    public required string Name { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Open;
}