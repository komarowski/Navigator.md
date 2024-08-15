using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Extension for opening links in a new tab.
  /// </summary>
  public class CustomLinkExtension : IMarkdownExtension
  {
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
      if (renderer is HtmlRenderer htmlRenderer)
      {
        htmlRenderer.ObjectRenderers.RemoveAll(x => x is LinkInlineRenderer);
        htmlRenderer.ObjectRenderers.Add(new CustomLinkRenderer());
      }
    }
  }
}
