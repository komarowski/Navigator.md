namespace MarkdownNavigator.Core.Domain;

public class SiteContent
{
    public required TreeStructure WikiTree { get; set; }

    public required List<TaskItem> Tasks { get; set; }

    public required List<QaItem> QaItems { get; set; }
}
