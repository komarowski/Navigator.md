using Markdig;
using Markdig.Renderers;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Converts nested details block to html details tag.
  /// </summary>
  public class NestedDetailsExtension : IMarkdownExtension
  {
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
      if (!pipeline.BlockParsers.Contains<NestedDetailsBlockParser>())
      {
        pipeline.BlockParsers.Add(new NestedDetailsBlockParser());
      }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
      if (!renderer.ObjectRenderers.Contains<DetailsBlockRenderer>())
      {
        renderer.ObjectRenderers.Add(new DetailsBlockRenderer(pipeline));
      }
    }
  }
}
