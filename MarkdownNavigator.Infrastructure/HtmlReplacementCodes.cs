namespace MarkdownNavigator.Infrastructure
{
  /// <summary>
  /// HTML template codes for replacement.
  /// </summary>
  public class HtmlReplacementCodes
  {
    /// <summary>
    /// Adds base folder for all relative URLs in a template. 
    /// </summary>
    public const string RelativePath = "{@RelativePath}";

    /// <summary>
    /// &lt;div id="tree-view" data-file="{@CurrentNodeId}" class="tree-view"&gt;
    /// </summary>
    public const string CurrentNodeId = "{@CurrentNodeId}";

    /// <summary>
    /// &lt;div id="blog" class="blog"&gt;{@MainBody}&lt;/div&gt;
    /// </summary>
    public const string MainBody = "{@MainBody}";

    /// <summary>
    /// 
    /// </summary>
    public const string EditLink = "{@EditLink}";
  }
}
