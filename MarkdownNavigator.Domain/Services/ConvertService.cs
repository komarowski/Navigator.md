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
    /// Template for generating html.
    /// </summary>
    private string Template
    {
      get
      {
        template ??= ResourceService.GetTemplate(settings.IsExport, settings.IsStandalone);
        return template;
      }
    }

    /// <summary>
    /// The path where the html files will be generated.
    /// </summary>
    private string TargetFolder => settings.IsExport
      ? Path.Combine(settings.SourceFolder, FolderReservedNames.ExportFolder)
      : settings.SourceFolder;

    private string TreeDataPath => Path.Combine(TargetFolder, "tree.js");
    private string IndexHtmlPath => Path.Combine(TargetFolder, "index.html");

    public int ConvertAllHtml(bool forceRefreshAll = false)
    {
      if (settings.IsExport)
      {
        ResourceService.EnsureDirectoryExists(TargetFolder);
      }

      var tree = new TreeStructure();
      tree = treeService.WalkDirectoryTree(new DirectoryInfo(settings.SourceFolder), tree, forceRefreshAll);
      foreach (var markdownFile in tree.MdFilesToConvert)
      {
        ConvertHtml(markdownFile.SourcePath, markdownFile.Code);
      }

      if (settings.IsExport)
      {
        foreach (var imageFile in tree.FilesToCopy)
        {
          File.Copy(imageFile.SourcePath, imageFile.TargetPath, true);
        }
      }

      if (!File.Exists(IndexHtmlPath) || forceRefreshAll)
      {
        var html = InsertIntoTemplate(IndexHtmlContent, string.Empty, string.Empty);
        File.WriteAllText(IndexHtmlPath, html);
      }

      GenerateJS(tree);

      if (!settings.IsStandalone) 
      {
        ResourceService.CopyAllAssets(TargetFolder);
      }
      
      return tree.MdFilesToConvert.Count;
    }

    public void ConvertHtml(string markdownPath, string? code = null)
    {
      if (File.Exists(markdownPath))
      {
        using var fileStream = new FileStream(markdownPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
        var markdown = streamReader.ReadToEnd();       
        var htmlText = MarkdownService.ConvertToHtml(markdown);
        code ??= treeService.GetPathCode(markdownPath);
        var html = InsertIntoTemplate(htmlText, code, markdownPath);
        var targetPath = settings.IsExport
          ? Path.Combine(TargetFolder, ExtensionService.GetHtml(code))
          : ExtensionService.MarkdownToHtml(markdownPath);
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
    /// <param name="htmlText">Main html text.</param>
    /// <param name="htmlCode">HTML file code.</param>
    /// <returns>Final html page.</returns>
    private string InsertIntoTemplate(string htmlText, string htmlCode, string markdownPath)
    {
      var template = new string(Template);
      template = template.Replace(HtmlReplacementCodes.HtmlCode, htmlCode);
      template = template.Replace(HtmlReplacementCodes.MarkdownPath, markdownPath);
      template = template.Replace(HtmlReplacementCodes.MainBody, htmlText);
      template = template.Replace(HtmlReplacementCodes.BaseFolder, settings.SourceFolder.Replace('\\', '/'));

      return template;
    }
  }
}
