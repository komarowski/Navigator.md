using Markdig;
using Markdig.Renderers;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Converts slider block to custom html image slider.
  /// </summary>
  public class SliderExtension : IMarkdownExtension
  {
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
      if (!pipeline.BlockParsers.Contains<SliderBlockParser>())
      {
        pipeline.BlockParsers.Add(new SliderBlockParser());
      }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
      if (!renderer.ObjectRenderers.Contains<SliderBlockRenderer>())
      {
        renderer.ObjectRenderers.Add(new SliderBlockRenderer());
      }
    }
  }
}
