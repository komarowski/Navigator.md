using MarkdownNavigator.Domain.Entities;
using MarkdownNavigator.Domain.Services;
using System.Text;

namespace MarkdownNavigator.Tests.Tests
{
  public class ConvertServiceTests(TestFixture fixture) : IClassFixture<TestFixture>
  {
    private readonly TestFixture fixture = fixture;

    [Theory]
    [InlineData("notes/todo.md", "../")]
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
      const int rootSubfolderCount = 2;
      const int rootFilesCount = 8;
      const int programmingSubfolderCount = 3;
      const int programmingFilesCount = 5;

      // Act
      tree = treeService.WalkDirectoryTree(
        new DirectoryInfo(fixture.TestDirectory), 
        tree,
        forceRefresh: true,
        isRoot: true);

      // Assert
      Assert.Equal(rootFilesCount, tree.MdFilesToConvert.Count);
      Assert.NotNull(tree.RootNode.Children);
      Assert.Equal(rootSubfolderCount, tree.RootNode.Children.Count);

      var tasksNode = tree.RootNode.Children.FirstOrDefault(x => x.Id == "_tasks");
      Assert.Null(tasksNode);

      var notesNode = tree.RootNode.Children.FirstOrDefault(x => x.Id == "notes");
      Assert.NotNull(notesNode);
      Assert.Equal(NodeType.Folder.ToString(), notesNode.Type);
      Assert.NotNull(notesNode.Children);
      Assert.Single(notesNode.Children);

      var programmingNode = tree.RootNode.Children.FirstOrDefault(x => x.Id == "programming");
      Assert.NotNull(programmingNode);
      Assert.Equal(NodeType.Folder.ToString(), programmingNode.Type);
      Assert.NotNull(programmingNode.Children);
      Assert.Equal(programmingSubfolderCount, programmingNode.Children.Count);

      var programmingChildNodes = programmingNode.Children
        .SelectMany(x => x.Children.Select(y => y))
        .ToList();
      Assert.Equal(programmingFilesCount, programmingChildNodes.Count);
    }

    [Fact]
    public void ConvertAllHtml()
    {
      // Arrange
      var appSettings = new AppSettings() { 
        SourceFolder = fixture.TestDirectory, 
        PluginList = [ "code", "prism", "slider" ], 
        DisableCopyAssets = false
      };
      var treeService = new TreeStructureService(appSettings);
      var convertService = new ConvertService(appSettings, treeService);
      var testDirectory = new DirectoryInfo(fixture.TestDirectory);

      // Case 1 - convert all files
      var actualCount = convertService.ConvertAllHtml();
      VerifyConvertAllHtmlResults(testDirectory, 8, actualCount);
      VerifyAssetsFiles(testDirectory);

      // Case 2 - no changes -> no files to convert 
      actualCount = convertService.ConvertAllHtml();
      VerifyConvertAllHtmlResults(testDirectory, 0, actualCount);

      // Case 3 - force convert all filesS
      actualCount = convertService.ConvertAllHtml(true);
      VerifyConvertAllHtmlResults(testDirectory, 8, actualCount);

      // Case 4 - change one file
      var file = Path.Combine(fixture.TestDirectory, "notes/todo.md");
      var content = File.ReadAllText(file);
      File.WriteAllText(file, content, Encoding.UTF8);
      actualCount = convertService.ConvertAllHtml();
      VerifyConvertAllHtmlResults(testDirectory, 1, actualCount);
    }

    private static void VerifyConvertAllHtmlResults(DirectoryInfo testDirectory, int expectedCount, int actualCount)
    {
      var markdownFileNames = testDirectory.GetFiles("*.md", SearchOption.AllDirectories)
        .Where(x => x.Name != "README.md")
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
      Assert.DoesNotContain("README", htmlFileNames);
    }

    private static void VerifyAssetsFiles(DirectoryInfo testDirectory)
    {
      var expectedAssetsFiles = new[]
      {
        "assets/logo.ico",
        "assets/core/core.css",
        "assets/core/core.js",
        "assets/plugins/code/plugin.css",
        "assets/plugins/code/plugin.js",
        "assets/plugins/prism/plugin.css",
        "assets/plugins/prism/plugin.js",
        "assets/plugins/slider/plugin.css",
        "assets/plugins/slider/plugin.js",
      };

      var assetsDirectory = new DirectoryInfo(Path.Combine(testDirectory.FullName, "assets"));

      Assert.True(assetsDirectory.Exists);

      var actualFiles = assetsDirectory.GetFiles("*.*", SearchOption.AllDirectories)
        .Select(x => Path.GetRelativePath(testDirectory.FullName, x.FullName).Replace("\\", "/"));

      foreach (var expectedAssetsFile in expectedAssetsFiles)
      {
        Assert.Contains(expectedAssetsFile, actualFiles);
      }
    }
  }
}
