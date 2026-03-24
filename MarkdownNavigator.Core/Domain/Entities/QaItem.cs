namespace MarkdownNavigator.Core.Domain;

public class QaItem
{
    public required string Question { get; set; }

    public required FileInfo File { get; set; }

    public string? Context { get; set; }

    public List<string> SearchTags { get; set; } = [];

    public int Popularity { get; set; } = 0;    

    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}