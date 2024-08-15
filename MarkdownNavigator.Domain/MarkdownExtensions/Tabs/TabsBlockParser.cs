using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Parser for tabs block.
  /// </summary>
  public class TabsBlockParser : BlockParser
  {
    private const string TabsBlockStart = "@@tabs";
    private const string TabBlockStart = "@@@tab";
    private const string TabsBlockEnd = "@@";

    public TabsBlockParser()
    {
      OpeningCharacters = ['@'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
      var slice = processor.Line;
      var content = slice.ToString();

      if (content.StartsWith(TabsBlockStart))
      {
        var tabsBlock = new TabsBlock(this)
        {
          Name = content[TabsBlockStart.Length..].Trim()
        };

        processor.NewBlocks.Push(tabsBlock);
        processor.GoToColumn(slice.Start + content.Length);
        return BlockState.Continue;
      }

      return BlockState.None;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
      var slice = processor.Line;
      var content = slice.ToString();
      var currentTabsBlock = (TabsBlock)block;

      if (content.StartsWith(TabBlockStart))
      {
        currentTabsBlock.Tabs.Add(new TabItem
        {
          Title = content[TabBlockStart.Length..].Trim()
        });
        processor.GoToColumn(slice.Start + content.Length);
        return BlockState.Continue;
      }

      if (content == TabsBlockEnd)
      {
        processor.Close(block);
        return BlockState.BreakDiscard;
      }

      var contentWithIndent = new string(' ', processor.Indent) + content;
      var lastTab = currentTabsBlock.Tabs.Last();
      if (lastTab is not null)
      {
        lastTab.ContentLines.Add(contentWithIndent);
      }
      
      return BlockState.Continue;
    }
  }
}
