using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Renderer for slider block. 
  /// </summary>
  public class SliderBlockRenderer : HtmlObjectRenderer<SliderBlock>
  {
    protected override void Write(HtmlRenderer renderer, SliderBlock obj)
    {
      var heightStyle = string.IsNullOrEmpty(obj.Height) 
        ? string.Empty 
        : $" style=\"height: {obj.Height};\"";

      renderer.Write($"<div class=\"slider\"{heightStyle}>").WriteLine();

      var imageNumber = obj.ImageList.Count;
      foreach (var image in obj.ImageList)
      {
        renderer.Write("<div class=\"slide\">").WriteLine();
        renderer.Write($"<img src=\"{image.Url}\">").WriteLine();
        renderer.Write($"<span>{image.Caption}</span>").WriteLine();
        renderer.Write("</div>").WriteLine();
      }

      if (imageNumber > 1)
      {
        renderer.Write("<button class=\"button-slider button-slider--prev\"> &lt; </button>").WriteLine();
        renderer.Write("<button class=\"button-slider button-slider--next\"> &gt; </button>").WriteLine();
      }

      renderer.Write("</div>").WriteLine();
    }
  }
}
