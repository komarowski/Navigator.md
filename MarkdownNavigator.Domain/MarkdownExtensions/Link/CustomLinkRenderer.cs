using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Renderer for custom links. 
  /// </summary>
  public class CustomLinkRenderer : LinkInlineRenderer
  {
    protected override void Write(HtmlRenderer renderer, LinkInline link)
    {
      if (!link.IsImage && !link.IsShortcut)
      {
        renderer.Write("<a href=\"").WriteEscapeUrl(link.Url).Write("\"");

        if (!string.IsNullOrEmpty(link.Title))
        {
          renderer.Write(" title=\"").WriteEscape(link.Title).Write("\"");
        }

        renderer.Write(" target=\"_blank\" rel=\"noopener noreferrer\">");
        renderer.WriteChildren(link);
        renderer.Write("</a>");
      }
      else
      {
        base.Write(renderer, link);
      }
    }
  }
}
