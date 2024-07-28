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
    public const string BaseFolder = "{@BaseFolder}";

    /// <summary>
    /// &lt;div id="tree-view" data-file="{@FileCode}" data-path="{@MarkdownPath}" class="tree-view"&gt;
    /// </summary>
    public const string HtmlCode = "{@FileCode}";

    /// <summary>
    /// &lt;div id="tree-view" data-file="{@FileCode}" data-path="{@MarkdownPath}" class="tree-view"&gt;
    /// </summary>
    public const string MarkdownPath = "{@MarkdownPath}";

    /// <summary>
    /// &lt;div id="blog" class="blog"&gt;{@MainBody}&lt;/div&gt;
    /// </summary>
    public const string MainBody = "{@MainBody}";
  }
}
