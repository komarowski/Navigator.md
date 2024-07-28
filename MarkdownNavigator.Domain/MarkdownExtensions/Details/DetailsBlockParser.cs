using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Parser for details block.
  /// </summary>
  public class DetailsBlockParser : BlockParser
  {
    private const string DetailsBlockStart = "@@details";
    private const string DetailsBlockEnd = "@@";

    public DetailsBlockParser()
    {
      OpeningCharacters = ['@'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
      var slice = processor.Line;
      var content = slice.ToString();

      if (content.StartsWith(DetailsBlockStart))
      {
        var detailsBlock = new DetailsBlock(this)
        {
          Summary = content[DetailsBlockStart.Length..].Trim()
        };

        processor.NewBlocks.Push(detailsBlock);
        processor.GoToColumn(slice.Start + content.Length);
        return BlockState.Continue;
      }

      return BlockState.None;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
      var slice = processor.Line;
      var content = slice.ToString();

      if (content == DetailsBlockEnd)
      {
        processor.Close(block);
        return BlockState.BreakDiscard;
      }

      var contentWithIndent = new string(' ', processor.Indent) + content;
      var currentDetailsBlock = (DetailsBlock)block;
      currentDetailsBlock.ContentLines.Add(contentWithIndent);
      processor.GoToColumn(slice.Start + content.Length);
      return BlockState.Continue;
    }
  }
}
