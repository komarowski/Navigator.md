using MarkdownNavigator.Domain.Services;
using MarkdownNavigator.DTO;
using System.Text;

namespace MarkdownNavigator.Services
{
  /// <summary>
  /// Service for API enpoints.
  /// </summary>
  public class ApiService
  {
    /// <summary>
    /// Registers API endpoints.
    /// </summary>
    /// <param name="app">WebApplication</param>
    public void RegisterEndpoints(WebApplication app)
    {
      app.MapGet($"/markdown", async (string? path) => {
        var markdown = string.Empty;
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
          markdown = await File.ReadAllTextAsync(path);
        }

        return Results.Text(markdown, "text/html");
      });

      app.MapPost($"/markdown", async (MarkdownDTO markdown, IConvertService convertService) => {
        if (markdown is not null && File.Exists(markdown.Path))
        {
          await File.WriteAllTextAsync(markdown.Path, markdown.Text, Encoding.UTF8);
          convertService.ConvertHtml(markdown.Path);
        }

        return Results.Ok();
      });
    }
  }
}
