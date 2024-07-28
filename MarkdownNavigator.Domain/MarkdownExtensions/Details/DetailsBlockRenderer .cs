using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Renderer for details block.
  /// </summary>
  public class DetailsBlockRenderer(MarkdownPipeline pipeline) : HtmlObjectRenderer<DetailsBlock>
  {
    private readonly MarkdownPipeline _pipeline = pipeline;

    protected override void Write(HtmlRenderer renderer, DetailsBlock obj)
    {
      renderer.Write("<details>").WriteLine();
      renderer.Write("<summary>").WriteEscape(obj.Summary).Write("</summary>").WriteLine();
      renderer.Write("<div>");

      var nestedMarkdown = string.Join("\n", obj.ContentLines);
      var nestedHtml = Markdown.ToHtml(nestedMarkdown, _pipeline);
      renderer.Write(nestedHtml);

      renderer.Write("</div>").WriteLine();
      renderer.Write("</details>").WriteLine();
    }
  }
}
