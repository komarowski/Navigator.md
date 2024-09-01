using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Domain.Services;
using MarkdownNavigator.Web.DTO;
using System.Text;

namespace MarkdownNavigator.Web.Services
{
  /// <summary>
  /// Service for API endpoints.
  /// </summary>
  public class ApiService
  {
    /// <summary>
    /// Registers API endpoints.
    /// </summary>
    /// <param name="app">WebApplication</param>
    public void RegisterEndpoints(WebApplication app)
    {
      app.MapPost($"api/html", (MarkdownToHtmlDTO markdown) => {
        if (markdown is null || string.IsNullOrEmpty(markdown.Text))
        {
          return new ResultTextDTO("Text is null ot empty", true);
        }

        var html = MarkdownService.ConvertToHtml(markdown.Text);

        return new ResultTextDTO() { Text = html };
      });

      app.MapGet($"api/markdown", async (string? path, IAppSettings settings) => {
        if (string.IsNullOrEmpty(path))
        {
          return new ResultTextDTO("Parameter 'path' is null or empty.", true);
        }

        var fullPath = Path.Combine(settings.SourceFolder, path);
        if (!File.Exists(fullPath))
        {
          return new ResultTextDTO($"File '{fullPath}' not found.", true);
        }

        var markdown = await File.ReadAllTextAsync(fullPath);

        return new ResultTextDTO(markdown);
      });

      app.MapPost($"api/markdown", async (MarkdownEditDTO markdown, IConvertService convertService, IAppSettings settings) =>
      {
        if (markdown is null || string.IsNullOrEmpty(markdown.Path))
        {
          return new ResultTextDTO("Parameter 'path' is null or empty.", true);
        }

        var fullPath = Path.Combine(settings.SourceFolder, markdown.Path);
        if (!File.Exists(fullPath))
        {
          return new ResultTextDTO($"File '{markdown.Path}' not found.", true);
        }

        await File.WriteAllTextAsync(fullPath, markdown.Text, Encoding.UTF8);
        convertService.ConvertHtml(fullPath);

        return new ResultTextDTO();
      });
    }
  }
}
