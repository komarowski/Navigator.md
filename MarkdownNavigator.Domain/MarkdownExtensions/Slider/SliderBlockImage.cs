namespace MarkdownNavigator.Domain.MarkdownExtensions
{
  /// <summary>
  /// Contains information about slider image.
  /// </summary>
  public class SliderBlockImage
  {
    /// <summary>
    /// Image source url.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Image caption text.
    /// </summary>
    public string Caption { get; set; } = string.Empty;
  }
}
