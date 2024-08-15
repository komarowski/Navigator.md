using Markdig;
using Markdig.Renderers;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Extension for tabs.
  /// </summary>
  public class TabsExtension : IMarkdownExtension
  {
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
      if (!pipeline.BlockParsers.Contains<TabsBlockParser>())
      {
        pipeline.BlockParsers.Add(new TabsBlockParser());
      }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
      if (!renderer.ObjectRenderers.Contains<TabsBlockRenderer>())
      {
        renderer.ObjectRenderers.Add(new TabsBlockRenderer(pipeline));
      }
    }
  }
}
