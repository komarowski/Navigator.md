namespace MarkdownNavigator.Core.Infrastructure;

/// <summary>
/// Generates HTML pages with standard templates for content, tasks, and Q&A.
/// </summary>
public static class HtmlTemplateGenerator
{
    public static string WrapContent(string htmlContent, string nodePath)
    {
        var root = GetRelativeRoot(nodePath);

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <title>Navigator.md</title>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <link rel=""icon"" type=""image/x-icon"" href=""{root}assets/logo.ico"">
  <link rel=""stylesheet"" href=""{root}assets/core.css"" />
  <link rel=""stylesheet"" href=""{root}assets/plugins/code/plugin.css"" />
  <link rel=""stylesheet"" href=""{root}assets/plugins/slider/plugin.css"" />
  <link rel=""stylesheet"" href=""{root}assets/plugins/prism/plugin.css"" />
</head>

<body>
  <header class=""site-header"">
    <nav class=""header-tabs"" role=""tablist"">
      <button class=""header-tab"" data-tab=""wiki"" role=""tab"">Wiki</button>
      <button class=""header-tab"" data-tab=""tasks"" role=""tab"">Tasks</button>
      <button class=""header-tab"" data-tab=""qa"" role=""tab"">Q&amp;A</button>
    </nav>
    <a class=""header-home"" href=""{root}index.html"">Navigator.md</a>
  </header>

  <main class=""main-content"">
    <aside id=""sidebar"" class=""sidebar"" role=""tabpanel"">
      <div id=""nav-tree"" data-node=""{nodePath}"" data-root=""{root}"" class=""navigation-tree""></div>
    </aside>

    <div class=""page-content"">
      <div class=""text-container"">
        <div class=""content-flex"">
          <article class=""content-block"">
            <div id=""markdown"" class=""markdown"">
              {htmlContent}
            </div>
          </article>

          <nav class=""content-table-block"">
            <div class=""content-table-container"">
              <ul id=""content-table"" class=""content-table""></ul>
            </div>
          </nav>
        </div>
      </div>
    </div>
  </main>

  <script src=""{root}data.js""></script>
  <script src=""{root}assets/core.js""></script>
  <script src=""{root}assets/plugins/prism/plugin.js""></script>
  <script src=""{root}assets/plugins/slider/plugin.js""></script>
  <script src=""{root}assets/plugins/code/plugin.js""></script>
</body>

</html>";
    }

    /// <summary>
    /// Computes the relative path to root from a node path by counting directory depth.
    /// </summary>
    /// <example>
    /// "index.html" → "./"
    /// "_tasks/index.html" → "../"
    /// "_wiki/folder/file.html" → "../../"
    /// </example>
    private static string GetRelativeRoot(string nodePath)
    {
        var depth = nodePath.Count(c => c == '/');
        return depth == 0 ? "./" : string.Concat(Enumerable.Repeat("../", depth));
    }
}
