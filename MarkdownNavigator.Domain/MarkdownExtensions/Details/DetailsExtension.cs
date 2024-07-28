using Markdig;
using Markdig.Renderers;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Converts details block to html details tag.
  /// </summary>
  public class DetailsExtension : IMarkdownExtension
  {
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
      if (!pipeline.BlockParsers.Contains<DetailsBlockParser>())
      {
        pipeline.BlockParsers.Add(new DetailsBlockParser());
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
