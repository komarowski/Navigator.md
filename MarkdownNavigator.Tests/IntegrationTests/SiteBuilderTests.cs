using NUnit.Framework;
using MarkdownNavigator.Core.Domain;
using MarkdownNavigator.Core.Application;
using MarkdownNavigator.Core.Infrastructure;
using Moq;
using Microsoft.Extensions.Logging;

namespace MarkdownNavigator.Tests.IntegrationTests;

public class SiteBuilderTests
{
    private string _testRootPath;
    private IPathManager _pathManager;
    private ITreeBuilder _treeBuilder;
    private ITaskScanner _taskScanner;
    private IQaScanner _qaScanner;
    private ISiteGenerator _fileWriter;
    private Mock<ILogger<SiteBuilder>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _testRootPath = Path.Combine(Path.GetTempPath(), $"SiteBuilderTest_{Guid.NewGuid()}");

        var settings = new AppSettings
        {
            SourceFolder = _testRootPath
        };

        _pathManager = new PathManager(settings);
        _treeBuilder = new TreeBuilder();
        _taskScanner = new TaskScanner();
        _qaScanner = new QaScanner();
        
        var embeddedResourceProviderLogger = new Mock<ILogger<EmbeddedResourceProvider>>().Object;
        var embeddedResourceProvider = new EmbeddedResourceProvider(embeddedResourceProviderLogger);
        _fileWriter = new SiteGenerator(_pathManager, embeddedResourceProvider);
        
        _mockLogger = new Mock<ILogger<SiteBuilder>>();
    }

    [TearDown]
    public void TearDown()
    {
        // Cleanup temporary directory
        if (Directory.Exists(_testRootPath))
        {
            Directory.Delete(_testRootPath, true);
        }
    }

    [Test]
    public void BuildAll_SourceFolderNotExist_ReturnsWithoutError()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid()}");
        var settings = new AppSettings { SourceFolder = nonExistentPath };
        var pathManager = new PathManager(settings);
        
        var siteBuilder = new SiteBuilder(
            _treeBuilder,
            _taskScanner,
            _qaScanner,
            pathManager,
            _fileWriter,
            _mockLogger.Object);

        // Act & Assert - should not throw
        Assert.DoesNotThrow(() => siteBuilder.Build());

        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Source folder not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public void BuildAll_WithCompleteStructure_ProcessesAllComponents()
    {
        // Arrange
        Directory.CreateDirectory(_testRootPath);
        var wikiFolder = Path.Combine(_testRootPath, "_wiki");
        Directory.CreateDirectory(wikiFolder);

        var complexIndexMarkdown = GetComplexMarkdownContent();
        File.WriteAllText(Path.Combine(wikiFolder, "index.md"), complexIndexMarkdown);
        File.WriteAllText(Path.Combine(wikiFolder, "intro.md"), "# intro\n\nThis is the introduction.");
        
        var docsFolder = Path.Combine(wikiFolder, "docs");
        Directory.CreateDirectory(docsFolder);
        File.WriteAllText(Path.Combine(docsFolder, "guide.md"), "# Developer Guide\n\nGuide content here.");
        
        var apiFolder = Path.Combine(docsFolder, "api");
        Directory.CreateDirectory(apiFolder);
        File.WriteAllText(Path.Combine(apiFolder, "overview.md"), "# API Overview\n\nAPI documentation.");

        var tasksFolder = Path.Combine(_testRootPath, "_tasks");
        Directory.CreateDirectory(tasksFolder);
        File.WriteAllText(Path.Combine(tasksFolder, "task1.md"), "---\nname: Task 1\n---\n\n# Add unit tests");
        File.WriteAllText(Path.Combine(tasksFolder, "task2.md"), "---\nname: Task 2\n---\n\n# Add integraion tests");

        var siteBuilder = new SiteBuilder(
            _treeBuilder,
            _taskScanner,
            _qaScanner,
            _pathManager,
            _fileWriter,
            _mockLogger.Object);

        // Act
        siteBuilder.Build();

        // Assert
        // assets folder should be generated
        var assetsFolderPath = Path.Combine(_testRootPath, "assets");
        Assert.That(Directory.Exists(assetsFolderPath), Is.True, "Assets folder should be generated");

        // Root index.html (help page) should be generated
        var rootIndexHtmlPath = Path.Combine(_testRootPath, "index.html");
        Assert.That(File.Exists(rootIndexHtmlPath), Is.True, "Root index.html (help page) should be generated");
        var rootIndexContent = File.ReadAllText(rootIndexHtmlPath);
        Assert.That(rootIndexContent, Does.Contain("Navigator.md"), "Help page should contain Navigator.md title");

        // data.js should be generated
        var dataJsPath = Path.Combine(_testRootPath, "data.js");
        Assert.That(File.Exists(dataJsPath), Is.True, "data.js should be generated");

        // HTML files should be created for markdown files in _wiki
        var wikiIndexHtmlPath = Path.Combine(wikiFolder, "index.html");
        var introHtmlPath = Path.Combine(wikiFolder, "intro.html");
        var overviewHtmlPath = Path.Combine(wikiFolder, "docs", "api", "overview.html");
        Assert.That(File.Exists(wikiIndexHtmlPath), Is.True, "_wiki/index.html should be generated");
        Assert.That(File.Exists(introHtmlPath), Is.True, "_wiki/intro.html should be generated");
        Assert.That(File.Exists(overviewHtmlPath), Is.True, "_wiki/docs/api/overview.html should be generated");

        // HTML files should be created for folders with index.md
        var docsIndexMdPath = Path.Combine(wikiFolder, "docs", "index.md");
        var docsIndexHtmlPath = Path.Combine(wikiFolder, "docs", "index.html");
        var apiIndexMdPath = Path.Combine(wikiFolder, "docs", "api", "index.md");
        var apiIndexHtmlPath = Path.Combine(wikiFolder, "docs", "api", "index.html");
        Assert.That(File.Exists(docsIndexMdPath), Is.True, "_wiki/docs/index.md should be generated");
        Assert.That(File.Exists(apiIndexMdPath), Is.True, "_wiki/docs/api/index.md should be generated");
        Assert.That(File.Exists(docsIndexHtmlPath), Is.True, "_wiki/docs/index.html should be generated");
        Assert.That(File.Exists(apiIndexHtmlPath), Is.True, "_wiki/docs/api/index.html should be generated");

        // HTML files should be created for markdown files in _tasks
        var task1HtmlPath = Path.Combine(tasksFolder, "task1.html");
        var task2HtmlPath = Path.Combine(tasksFolder, "task2.html");
        Assert.That(File.Exists(task1HtmlPath), Is.True, "_tasks/task1.html should be generated");
        Assert.That(File.Exists(task2HtmlPath), Is.True, "_tasks/task2.html should be generated");

        // Verify data.js contains valid JSON structure with rootNode, tasks, qa, and qaTags
        var dataJsContent = File.ReadAllText(dataJsPath);
        Assert.That(dataJsContent, Does.Contain("const rootNode = {"), "data.js should contain rootNode");
        Assert.That(dataJsContent, Does.Contain("const tasks = ["), "data.js should contain tasks array");
        Assert.That(dataJsContent, Does.Contain("const qa = ["), "data.js should contain qa array");
        Assert.That(dataJsContent, Does.Contain("const qaTags = ["), "data.js should contain qaTags array");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"root\""), "data.js should contain root node");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"intro\""), "data.js should contain intro.md node");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"Developer Guide\""), "data.js should contain guide.md node");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"API Overview\""), "data.js should contain overview.md node");

        // Verify complex HTML content is properly converted from _wiki/index.md
        var wikiIndexHtmlContent = File.ReadAllText(wikiIndexHtmlPath);
        Assert.That(wikiIndexHtmlContent, Does.Contain("Main Documentation"), "HTML should contain h1 heading");
        Assert.That(wikiIndexHtmlContent, Does.Contain("<strong>main page</strong>"), "HTML should contain bold text");
        Assert.That(wikiIndexHtmlContent, Does.Contain("<em>complex</em>"), "HTML should contain italic text");
        Assert.That(wikiIndexHtmlContent, Does.Contain("Features"), "HTML should contain Features section");
        Assert.That(wikiIndexHtmlContent, Does.Contain("<li>"), "HTML should contain list items");
        Assert.That(wikiIndexHtmlContent, Does.Contain("Getting Started"), "HTML should contain Getting Started section");
        Assert.That(wikiIndexHtmlContent, Does.Contain("<ol>"), "HTML should contain ordered list");
        Assert.That(wikiIndexHtmlContent, Does.Contain("<code class=\"language-csharp\">"), "HTML should contain code elements");
        Assert.That(wikiIndexHtmlContent, Does.Contain("HelloWorld"), "HTML should contain code example content");

        // Verify relative links in the overview.html
        var overviewHtmlContent = File.ReadAllText(overviewHtmlPath);
        Assert.That(overviewHtmlContent, Does.Contain("<link rel=\"stylesheet\" href=\"../../../assets/core.css\" />"), "HTML should contain core.css link");
        Assert.That(overviewHtmlContent, Does.Contain("data-node=\"_wiki/docs/api/overview.html\""), "HTML should contain correct node path");
        Assert.That(overviewHtmlContent, Does.Contain("<script src=\"../../../assets/core.js\"></script>"), "HTML should contain core.js link");
    }

    private static string GetComplexMarkdownContent()
    {
        return @"# Main Documentation

Welcome to the **main page** with _complex_ markdown.

## Features

- First feature item
- Second feature item
  - Nested item
- Third feature item

## Getting Started

1. Install the package
2. Configure settings
3. Start building

### Code Example

```csharp
public class Example
{
    public void HelloWorld()
    {
        Console.WriteLine(""Hello!"");
    }
}
```";
    }
}
