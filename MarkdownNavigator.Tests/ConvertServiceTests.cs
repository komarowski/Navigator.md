using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Domain.Services;
using System.Text;

namespace MarkdownNavigator.Tests
{
  public class ConvertServiceTests(TestFixture fixture) : IClassFixture<TestFixture>
  {
    private readonly TestFixture fixture = fixture;

    [Theory]
    [InlineData("notes/node.md", "../")]
    [InlineData("programming/javascript/theory.md", "../../")]
    [InlineData("index.md", "")]
    [InlineData("", "")]
    public void GetRelativePathForNode(string markdownPath, string expected)
    {
      // Arrange
      var treeService = new TreeStructureService(new AppSettings() { SourceFolder = fixture.TestDirectory });

      // Act
      var fullMarkdownPath = Path.Combine(fixture.TestDirectory, markdownPath);
      var actual = treeService.GetRelativePathForNode(fullMarkdownPath);

      // Assert
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void WalkDirectoryTree()
    {
      // Arrange
      var treeService = new TreeStructureService(new AppSettings() { SourceFolder = fixture.TestDirectory });
      var tree = new TreeStructure();

      // Act
      tree = treeService.WalkDirectoryTree(new DirectoryInfo(fixture.TestDirectory), tree, true);

      // Assert
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

    [Fact]
    public void ConvertAllHtml()
    {
      // Arrange
      var appSettings = new AppSettings() { SourceFolder = fixture.TestDirectory, DisableCopyAssets = true };
      var treeService = new TreeStructureService(appSettings);
      var convertService = new ConvertService(appSettings, treeService);
      var testDirectory = new DirectoryInfo(fixture.TestDirectory);

      // Case 1 - convert all files
      var actualCount = convertService.ConvertAllHtml();
      VerifyConvertAllHtmlResults(testDirectory, 6, actualCount);

      // Case 2 - no changes -> no files to convert 
      actualCount = convertService.ConvertAllHtml();
      VerifyConvertAllHtmlResults(testDirectory, 0, actualCount);

      // Case 3 - force convert all files
      actualCount = convertService.ConvertAllHtml(true);
      VerifyConvertAllHtmlResults(testDirectory, 6, actualCount);

      // Case 4 - change one file
      var file = Path.Combine(fixture.TestDirectory, "notes/note.md");
      var content = File.ReadAllText(file);
      File.WriteAllText(file, content, Encoding.UTF8);
      actualCount = convertService.ConvertAllHtml();
      VerifyConvertAllHtmlResults(testDirectory, 1, actualCount);
    }

    private static void VerifyConvertAllHtmlResults(DirectoryInfo testDirectory, int expectedCount, int actualCount)
    {
      var markdownFileNames = testDirectory.GetFiles("*.md", SearchOption.AllDirectories)
        .Select(x => x.Name.Split('.').First());
      var htmlFileNames = testDirectory.GetFiles("*.html", SearchOption.AllDirectories)
        .Select(x => x.Name.Split('.').First());

      Assert.Equal(expectedCount, actualCount);
      foreach (var markdownFileName in markdownFileNames)
      {
        Assert.Contains(markdownFileName, htmlFileNames);
      }
      Assert.Contains("index", htmlFileNames);
      Assert.Contains("help", htmlFileNames);
    }
  }
}
