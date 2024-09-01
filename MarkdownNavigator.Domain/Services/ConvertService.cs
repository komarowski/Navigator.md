using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Infrastructure;
using MarkdownNavigator.Infrastructure.Services;
using System.Text;
using System.Text.Json;

namespace MarkdownNavigator.Domain.Services
{
  public class ConvertService(IAppSettings settings, ITreeStructureService treeStructureService) : IConvertService
  {
    private readonly IAppSettings settings = settings;
    private readonly ITreeStructureService treeService = treeStructureService;

    private string? indexHtmlContent;
    private string? helpHtmlContent;
    private string? template;

    /// <summary>
    /// Content for index.html file.
    /// </summary>
    private string IndexHtmlContent
    {
      get
      {
        indexHtmlContent ??= MarkdownService.ConvertToHtml(ResourceService.GetIndexMdContent());
        return indexHtmlContent;
      }
    }

    /// <summary>
    /// Content for index.html file.
    /// </summary>
    private string HelpHtmlContent
    {
      get
      {
        helpHtmlContent ??= MarkdownService.ConvertToHtml(ResourceService.GetHelpMdContent());
        return helpHtmlContent;
      }
    }

    /// <summary>
    /// Template for generating html.
    /// </summary>
    private string Template
    {
      get
      {
        template ??= ResourceService.GetTemplate();
        return template;
      }
    }

    private string TreeDataPath => Path.Combine(settings.SourceFolder, "tree.js");
    private string IndexMdPath => Path.Combine(settings.SourceFolder, "index.md");
    private string IndexHtmlPath => Path.Combine(settings.SourceFolder, "index.html");
    private string HelpHtmlPath => Path.Combine(settings.SourceFolder, "help.html");

    public int ConvertAllHtml(bool forceRefreshAll = false)
    {
      var tree = new TreeStructure();
      tree = treeService.WalkDirectoryTree(new DirectoryInfo(settings.SourceFolder), tree, forceRefreshAll);
      foreach (var markdownFile in tree.MdFilesToConvert)
      {
        ConvertHtml(markdownFile);
      }

      if (!File.Exists(IndexMdPath) && (!File.Exists(IndexHtmlPath) || forceRefreshAll))
      {
        var html = GetHtmlPageFromTemplate(IndexHtmlContent, string.Empty, string.Empty);
        File.WriteAllText(IndexHtmlPath, html);
      }

      if (!File.Exists(HelpHtmlPath) || forceRefreshAll)
      {
        var html = GetHtmlPageFromTemplate(HelpHtmlContent, string.Empty, string.Empty);
        File.WriteAllText(HelpHtmlPath, html);
      }

      GenerateJS(tree);

      if (!settings.DisableCopyAssets) 
      {
        ResourceService.CopyAllAssets(settings.SourceFolder);
      }
      
      return tree.MdFilesToConvert.Count;
    }

    public void ConvertHtml(string markdownPath)
    {
      if (File.Exists(markdownPath))
      {
        using var fileStream = new FileStream(markdownPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
        var markdown = streamReader.ReadToEnd();       
        var htmlContent = MarkdownService.ConvertToHtml(markdown);
        var nodeId = treeService.GetNodeId(markdownPath);
        var nodeRelativePath = treeService.GetRelativePathForNode(markdownPath);
        var html = GetHtmlPageFromTemplate(htmlContent, nodeId, nodeRelativePath);
        var targetPath = FileExtensionService.MarkdownToHtml(markdownPath);
        using var streamWriter = new StreamWriter(targetPath);
        streamWriter.WriteLine(html);
      }
    }

    /// <summary>
    /// Generate javascript file (tree.js) with tree view. 
    /// </summary>
    /// <param name="tree">Tree view.</param>
    public void GenerateJS(TreeStructure tree)
    {
      var jsonString = JsonSerializer.Serialize(tree.RootNode.Children);
      var jsText = $"const nodeList = {jsonString};";
      File.WriteAllText(TreeDataPath, jsText, Encoding.UTF8);
    }

    /// <summary>
    /// Insert html text into a template.
    /// </summary>
    /// <param name="htmlContent">Main html content.</param>
    /// <param name="nodeId">Node id.</param>
    /// <param name="nodeRelativePath"></param>
    /// <returns>Final html page.</returns>
    private string GetHtmlPageFromTemplate(string htmlContent, string nodeId, string nodeRelativePath)
    {
      var htmlPage = new string(Template);
      htmlPage = htmlPage.Replace(HtmlReplacementCodes.CurrentNodeId, nodeId);
      htmlPage = htmlPage.Replace(HtmlReplacementCodes.MainBody, htmlContent);
      htmlPage = htmlPage.Replace(HtmlReplacementCodes.RelativePath, nodeRelativePath);

      var editLink = string.IsNullOrEmpty(settings.Server)
        ? "/"
        : $"{settings.Server}/{FileExtensionService.HtmlToMarkdown(nodeId)}";
      htmlPage = htmlPage.Replace(HtmlReplacementCodes.EditLink, editLink);

      return htmlPage;
    }
  }
}
