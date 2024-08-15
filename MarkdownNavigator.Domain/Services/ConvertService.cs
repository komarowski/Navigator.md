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
        template ??= GetTempalte();
        return template;
      }
    }

    /// <summary>
    /// The path where the html files will be generated.
    /// </summary>
    private string TargetFolder => settings.EnableExport
      ? Path.Combine(settings.SourceFolder, FolderReservedNames.ExportFolder)
      : settings.SourceFolder;

    private string TreeDataPath => Path.Combine(TargetFolder, "tree.js");
    private string IndexMdPath => Path.Combine(TargetFolder, "index.md");
    private string IndexHtmlPath => Path.Combine(TargetFolder, "index.html");
    private string HelpHtmlPath => Path.Combine(TargetFolder, "help.html");
    private string CustomTemplatePath => Path.Combine(TargetFolder, "assets\\template.html");

    public int ConvertAllHtml(bool forceRefreshAll = false)
    {
      if (settings.EnableExport)
      {
        ResourceService.EnsureDirectoryExists(TargetFolder);
      }

      var tree = new TreeStructure();
      tree = treeService.WalkDirectoryTree(new DirectoryInfo(settings.SourceFolder), tree, forceRefreshAll);
      foreach (var markdownFile in tree.MdFilesToConvert)
      {
        ConvertHtml(markdownFile.SourcePath, markdownFile.Code);
      }

      if (settings.EnableExport)
      {
        foreach (var imageFile in tree.FilesToCopy)
        {
          File.Copy(imageFile.SourcePath, imageFile.TargetPath, true);
        }
      }

      if (!File.Exists(IndexMdPath) && (!File.Exists(IndexHtmlPath) || forceRefreshAll))
      {
        var html = InsertIntoTemplate(IndexHtmlContent, string.Empty, string.Empty);
        File.WriteAllText(IndexHtmlPath, html);
      }

      if (!File.Exists(HelpHtmlPath) || forceRefreshAll)
      {
        var html = InsertIntoTemplate(HelpHtmlContent, string.Empty, string.Empty);
        File.WriteAllText(HelpHtmlPath, html);
      }

      GenerateJS(tree);

      if (!settings.DisableCopyAssets) 
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
        var targetPath = settings.EnableExport
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

    /// <summary>
    /// Gets the HTML template.
    /// </summary>
    /// <returns></returns>
    private string GetTempalte()
    {
      if (settings.EnableCustomTemplate)
      {
        if (File.Exists(CustomTemplatePath))
        {
          return File.ReadAllText(CustomTemplatePath);
        }

        ConsoleService.WriteLog($"Custom template not found \"{CustomTemplatePath}\"", LogType.Warning);
       }

      return ResourceService.GetTemplate(settings.EnableExport);
    }
  }
}
