using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Contains information about slider block.
  /// </summary>
  /// <param name="parser"></param>
  public class SliderBlock(BlockParser parser) : ContainerBlock(parser)
  {
    /// <summary>
    /// Height of slider block.
    /// </summary>
    public string Height { get; set; } = string.Empty;

    /// <summary>
    /// List of sldier images.
    /// </summary>
    public List<SliderBlockImage> ImageList { get; set; } = [];
  }
}
