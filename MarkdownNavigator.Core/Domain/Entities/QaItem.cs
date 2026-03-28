namespace MarkdownNavigator.Core.Domain;

public class QaItem
{
    public required string Question { get; set; }

    public required FileInfo File { get; set; }

    public int Popularity { get; set; } = 0;    
}