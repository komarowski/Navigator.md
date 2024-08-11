using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Renderer for tabs block.
  /// </summary>
  public class TabsBlockRenderer(MarkdownPipeline pipeline) : HtmlObjectRenderer<TabsBlock>
  {
    private readonly MarkdownPipeline _pipeline = pipeline;

    protected override void Write(HtmlRenderer renderer, TabsBlock obj)
    {
      renderer.Write("<div class=\"tabs\">").WriteLine();

      for (int i = 0; i < obj.Tabs.Count; i++)
      {
        var tab = obj.Tabs[i];
        var checkedAttribute = i == 0 ? " checked=\"checked\"" : string.Empty;
        var tabId = $"{obj.Name}-{i + 1}";

        renderer.Write($"<input class=\"input\" name=\"{obj.Name}\" type=\"radio\" id=\"{tabId}\"{checkedAttribute}/>").WriteLine();
        renderer.Write($"<label class=\"label\" for=\"{tabId}\">{tab.Title}</label>").WriteLine();
        renderer.Write("<div class=\"panel\">").WriteLine();

        var nestedMarkdown = string.Join("\n", tab.ContentLines);
        var nestedHtml = Markdown.ToHtml(nestedMarkdown, _pipeline);
        renderer.Write(nestedHtml);

        renderer.Write("</div>").WriteLine();
      }

      renderer.Write("</div>").WriteLine();
    }
  }
}
