namespace MarkdownNavigator.Core.Domain;

public class TaskItem
{
    public required string Name { get; set; }

    public required FileInfo File { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? ExternalLink { get; set; }
}