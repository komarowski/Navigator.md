using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Domain.Services;
using MarkdownNavigator.Infrastructure;

namespace MarkdownNavigator.Tests
{
  public class ConvertServiceTests(TestFixture fixture) : IClassFixture<TestFixture>
  {
    private readonly TestFixture fixture = fixture;

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void WalkDirectoryTree(bool isExport, int filesToCopyCount)
    {
      // Arrange
      var treeService = new TreeStructureService(new AppSettings() { SourceFolder = fixture.TestDirectory, EnableExport = isExport });
      var tree = new TreeStructure();

      // Act
      tree = treeService.WalkDirectoryTree(new DirectoryInfo(fixture.TestDirectory), tree, true);

      // Assert
      Assert.Equal(filesToCopyCount, tree.FilesToCopy.Count);
      Assert.Equal(6, tree.MdFilesToConvert.Count);
      Assert.NotNull(tree.RootNode.Children);
      Assert.Equal(2, tree.RootNode.Children.Count);

      var notesNode = tree.RootNode.Children.FirstOrDefault(x => x.Id == "notes");
      Assert.NotNull(notesNode);
      Assert.Equal(NodeType.Folder.ToString(), notesNode.Type);
      Assert.NotNull(notesNode.Children);
      Assert.Single(notesNode.Children);

      var programmingNode = tree.RootNode.Children.FirstOrDefault(x => x.Id == "programming");
      Assert.NotNull(programmingNode);
      Assert.Equal(NodeType.Folder.ToString(), programmingNode.Type);
      Assert.NotNull(programmingNode.Children);
      Assert.Equal(3, programmingNode.Children.Count);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConvertAllHtml(bool isExport)
    {
      // Arrange
      var appSettings = new AppSettings() { SourceFolder = fixture.TestDirectory, EnableExport = isExport, DisableCopyAssets = true };
      var treeService = new TreeStructureService(appSettings);
      var convertService = new ConvertService(appSettings, treeService);
      var testDirectory = new DirectoryInfo(fixture.TestDirectory);
      var exportDirectory = new DirectoryInfo(Path.Combine(fixture.TestDirectory, FolderReservedNames.ExportFolder));

      // Act
      var count = convertService.ConvertAllHtml();
      var markdownFileNames = testDirectory.GetFiles("*.md", SearchOption.AllDirectories)
        .Select(x => x.Name.Split('.').First());
      var htmlFileNames = isExport 
        ? GetHtmlFileNames(exportDirectory, isExport) 
        : GetHtmlFileNames(testDirectory, isExport);

      // Assert
      Assert.Equal(6, count);
      foreach (var markdownFileName in markdownFileNames)
      {
        Assert.Contains(markdownFileName, htmlFileNames);
      }
      Assert.Contains("index", htmlFileNames);
    }

    private static IEnumerable<string> GetHtmlFileNames(DirectoryInfo directory, bool isExport)
    {
      if (isExport)
      {
        return directory.GetFiles("*.html", SearchOption.TopDirectoryOnly)
          .Select(x => x.Name.Split('.').First().Split("__").Last());
      }

      return directory.GetFiles("*.html", SearchOption.AllDirectories)
        .Select(x => x.Name.Split('.').First());
    }
  }
}
