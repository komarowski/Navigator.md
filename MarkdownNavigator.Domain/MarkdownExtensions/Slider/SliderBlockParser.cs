using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Parser for slider block.
  /// </summary>
  public class SliderBlockParser : BlockParser
  {
    private const string SliderBlockStart = "@@slider";
    private const string SliderBlockEnd = "@@";
    private const string SliderBlockDefaultHeight = "350px";
    private const string ImageUrlStart = "![";
    private const char ImageUrlEnd = ']';
    private const char ImageCaptionStart = '(';
    private const char ImageCaptionend = ')';

    public SliderBlockParser()
    {
      OpeningCharacters = ['@'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
      var slice = processor.Line;
      var content = slice.ToString();

      if (content.StartsWith(SliderBlockStart))
      {
        var height = content[SliderBlockStart.Length..].Trim();

        var sliderBlock = new SliderBlock(this)
        {
          Height = string.IsNullOrEmpty(height) ? SliderBlockDefaultHeight : height
        };

        processor.NewBlocks.Push(sliderBlock);
        processor.GoToColumn(slice.Start + slice.Length);
        return BlockState.Continue;
      }

      return BlockState.None;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
      var slice = processor.Line;
      if (slice.Match(SliderBlockEnd))
      {
        processor.Close(block);
        return BlockState.BreakDiscard;
      }

      var content = slice.ToString();
      if (block is SliderBlock sliderBlock 
        && slice.Match(ImageUrlStart) 
        && TryGetSubstringBetween(content, ImageUrlStart, ImageUrlEnd.ToString(), out var imageUrl))
      {
        TryGetSubstringBetween(content, ImageCaptionStart.ToString(), ImageCaptionend.ToString(), out var imageCaption);

        sliderBlock.ImageList.Add(
          new SliderBlockImage
          {
            Url = imageUrl,
            Caption = imageCaption
          });
      }

      return BlockState.Continue;
    }

    /// <summary>
    /// Returns a substring between two specified strings.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <param name="start">Start string.</param>
    /// <param name="end">End string.</param>
    /// <param name="result">Result substring.</param>
    /// <returns>True if substring exist.</returns>
    private static bool TryGetSubstringBetween(string text, string start, string end, out string result)
    {
      result = string.Empty;
      if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
      {
        return false;
      }

      int startIndex = text.IndexOf(start);
      if (startIndex == -1)
      {
        return false;
      }

      startIndex += start.Length;
      int endIndex = text.IndexOf(end, startIndex);
      if (endIndex == -1)
      {
        return false;
      }

      result = text[startIndex..endIndex];
      return true;
    }
  }
}
