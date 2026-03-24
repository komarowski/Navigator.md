namespace MarkdownNavigator.Core.Domain;

public class TaskFrontMatter
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? Link { get; set; }
}