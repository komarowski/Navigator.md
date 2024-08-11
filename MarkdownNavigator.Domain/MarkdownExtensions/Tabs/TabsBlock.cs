using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Contains information about tabs block.
  /// </summary>
  /// <param name="parser"></param>
  public class TabsBlock(BlockParser parser) : ContainerBlock(parser)
  {
    /// <summary>
    /// Common name for related tabs.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of tabs in the tabs block.
    /// </summary>
    public List<TabItem> Tabs { get; set; } = [];
  }
}
