using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Contains information about details block.
  /// </summary>
  /// <param name="parser"></param>
  public class DetailsBlock(BlockParser parser) : ContainerBlock(parser)
  {
    /// <summary>
    /// Summary of details block.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Lines of text in details block.
    /// </summary>
    public List<string> ContentLines { get; set; } = [];
  }
}
