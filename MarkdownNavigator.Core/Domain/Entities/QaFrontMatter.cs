namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// Metadata extracted from the front matter of a Q&amp;A markdown file.
/// </summary>
public class QaFrontMatter
{
    /// <summary>
    /// The main question shown for the Q&amp;A item.
    /// </summary>
    public required string Question { get; set; }

    /// <summary>
    /// Optional additional context that helps clarify the question.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// Search keywords used to improve lookup and filtering.
    /// </summary>
    public List<string> SearchTags { get; set; } = [];

    /// <summary>
    /// Optional popularity score used for sorting or ranking.
    /// </summary>
    public int Popularity { get; set; } = 0;
}