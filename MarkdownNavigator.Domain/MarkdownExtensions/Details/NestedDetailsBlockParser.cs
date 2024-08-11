namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Parser for nested details block.
  /// </summary>
  public class NestedDetailsBlockParser : DetailsBlockParser
  {
    protected override string DetailsBlockStart => "@@@details";
    protected override string DetailsBlockEnd => "@@@";
  }
}
