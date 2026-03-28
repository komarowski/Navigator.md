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
    public void Build_SourceFolderNotExist_ReturnsWithoutError()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid()}");
        var settings = new AppSettings { SourceFolder = nonExistentPath };
        var pathManager = new PathManager(settings);

        var siteBuilder = CreateSiteBuilder();

        // Act & Assert - should not throw
        Assert.DoesNotThrow(() => siteBuilder.Build());

        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Source folder not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public void Build_GeneratesExpectedSiteFiles()
    {
        // Arrange
        Directory.CreateDirectory(_testRootPath);
        var paths = CreateRealisticSiteStructure();
        var siteBuilder = CreateSiteBuilder();

        // Act
        siteBuilder.Build();

        // Assert
        Assert.That(Directory.Exists(Path.Combine(_testRootPath, "assets")), Is.True, "Assets folder should be generated");
        Assert.That(File.Exists(paths.RootIndexHtmlPath), Is.True, "Root help page should be generated");
        Assert.That(File.Exists(paths.DataJsPath), Is.True, "data.js should be generated");
        Assert.That(File.Exists(paths.IntroHtmlPath), Is.True, "_wiki/intro.html should be generated");
        Assert.That(File.Exists(paths.GettingStartedHtmlPath), Is.True, "_wiki/docs/getting-started.html should be generated");
        Assert.That(File.Exists(paths.SiteBuilderHtmlPath), Is.True, "_wiki/docs/architecture/site-builder.html should be generated");
        Assert.That(File.Exists(paths.PathResolutionHtmlPath), Is.True, "_wiki/docs/architecture/path-resolution.html should be generated");
        Assert.That(File.Exists(paths.IntegrationTestsHtmlPath), Is.True, "_wiki/docs/testing/integration-tests.html should be generated");
        Assert.That(File.Exists(paths.MarkdownExamplesHtmlPath), Is.True, "_wiki/snippets/markdown-examples.html should be generated");
        Assert.That(File.Exists(paths.DocsIndexMdPath), Is.True, "_wiki/docs/index.md should be generated");
        Assert.That(File.Exists(paths.ArchitectureIndexMdPath), Is.True, "_wiki/docs/architecture/index.md should be generated");
        Assert.That(File.Exists(paths.TestingIndexMdPath), Is.True, "_wiki/docs/testing/index.md should be generated");
        Assert.That(File.Exists(paths.DocsIndexHtmlPath), Is.True, "_wiki/docs/index.html should be generated");
        Assert.That(File.Exists(paths.ArchitectureIndexHtmlPath), Is.True, "_wiki/docs/architecture/index.html should be generated");
        Assert.That(File.Exists(paths.TestingIndexHtmlPath), Is.True, "_wiki/docs/testing/index.html should be generated");
        Assert.That(File.Exists(paths.TaskSiteBuilderTestsHtmlPath), Is.True, "_tasks/add-site-builder-tests.html should be generated");
        Assert.That(File.Exists(paths.TaskHelpPageHtmlPath), Is.True, "_tasks/improve-help-page.html should be generated");
        Assert.That(File.Exists(paths.TaskRelativePathsHtmlPath), Is.True, "_tasks/fix-relative-paths.html should be generated");
        var rootIndexContent = File.ReadAllText(paths.RootIndexHtmlPath);
        Assert.That(rootIndexContent, Does.Contain("Navigator.md"), "Root help page should contain Navigator.md title");
    }

    [Test]
    public void Build_GeneratesNavigationData_ForWikiTasksAndQa()
    {
        // Arrange
        Directory.CreateDirectory(_testRootPath);
        var paths = CreateRealisticSiteStructure();
        var siteBuilder = CreateSiteBuilder();

        // Act
        siteBuilder.Build();

        // Assert
        var dataJsContent = File.ReadAllText(paths.DataJsPath);

        Assert.That(dataJsContent, Does.Contain("const rootNode = {"), "data.js should contain rootNode");
        Assert.That(dataJsContent, Does.Contain("const tasks = ["), "data.js should contain tasks array");
        Assert.That(dataJsContent, Does.Contain("const qa = ["), "data.js should contain qa array");

        Assert.That(dataJsContent, Does.Contain("\"Name\": \"root\""), "data.js should contain root node");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"Introduction\""), "data.js should contain Introduction page");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"Getting Started\""), "data.js should contain Getting Started page");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"Site Builder\""), "data.js should contain Site Builder page");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"Path Resolution\""), "data.js should contain Path Resolution page");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"Integration Tests\""), "data.js should contain Integration Tests page");
        Assert.That(dataJsContent, Does.Contain("\"Name\": \"Markdown Examples\""), "data.js should contain Markdown Examples page");

        Assert.That(dataJsContent, Does.Contain("Improve Site Builder Tests"), "data.js should contain task metadata");
        Assert.That(dataJsContent, Does.Contain("Improve Help Page"), "data.js should contain second task metadata");
        Assert.That(dataJsContent, Does.Contain("Fix Relative Paths"), "data.js should contain third task metadata");

        Assert.That(dataJsContent, Does.Contain("Why is index.md generated for folders?"), "data.js should contain Q&A question");
        Assert.That(dataJsContent, Does.Contain("How do relative asset paths work in generated pages?"), "data.js should contain Q&A question");
    }

    [Test]
    public void Build_RendersMarkdownAndRelativePaths_CorrectlyAcrossDepths()
    {
        // Arrange
        Directory.CreateDirectory(_testRootPath);
        var paths = CreateRealisticSiteStructure();
        var siteBuilder = CreateSiteBuilder();

        // Act
        siteBuilder.Build();

        // Assert
        var markdownExamplesHtmlContent = File.ReadAllText(paths.MarkdownExamplesHtmlPath);
        Assert.That(markdownExamplesHtmlContent, Does.Contain("<strong>Bold</strong>"), "Markdown examples should render bold text");
        Assert.That(markdownExamplesHtmlContent, Does.Contain("<em>Italic</em>"), "Markdown examples should render italic text");
        Assert.That(markdownExamplesHtmlContent, Does.Contain("<del>old</del>"), "Markdown examples should render strikethrough");
        Assert.That(markdownExamplesHtmlContent, Does.Contain("<code>code</code>"), "Markdown examples should render inline code");
        Assert.That(markdownExamplesHtmlContent, Does.Contain("<table>"), "Markdown examples should render table");
        Assert.That(markdownExamplesHtmlContent, Does.Contain("<details>"), "Markdown examples should render details");
        Assert.That(markdownExamplesHtmlContent, Does.Contain("<code class=\"language-sql\">"), "Markdown examples should render fenced SQL code");

        var taskHtmlContent = File.ReadAllText(paths.TaskSiteBuilderTestsHtmlPath);
        Assert.That(taskHtmlContent, Does.Contain("Improve Site Builder Tests"), "Task page should contain task title");
        Assert.That(taskHtmlContent, Does.Contain("Split one large integration test"), "Task page should contain task description");

        var nestedHtmlContent = File.ReadAllText(paths.SiteBuilderHtmlPath);
        Assert.That(nestedHtmlContent, Does.Contain("<link rel=\"stylesheet\" href=\"../../../assets/core.css\" />"), "Nested wiki page should contain correct core.css link");
        Assert.That(nestedHtmlContent, Does.Contain("<script src=\"../../../assets/core.js\"></script>"), "Nested wiki page should contain correct core.js link");
        Assert.That(nestedHtmlContent, Does.Contain("data-node=\"_wiki/docs/architecture/site-builder.html\""), "Nested wiki page should contain correct node path");

        var taskPageHtmlContent = File.ReadAllText(paths.TaskRelativePathsHtmlPath);
        Assert.That(taskPageHtmlContent, Does.Contain("<link rel=\"stylesheet\" href=\"../assets/core.css\" />"), "Task page should contain correct core.css link");
        Assert.That(taskPageHtmlContent, Does.Contain("<script src=\"../assets/core.js\"></script>"), "Task page should contain correct core.js link");
        Assert.That(taskPageHtmlContent, Does.Contain("data-node=\"_tasks/fix-relative-paths.html\""), "Task page should contain correct node path");
    }

    private SiteBuilder CreateSiteBuilder()
    {
        return new SiteBuilder(
            _treeBuilder,
            _taskScanner,
            _qaScanner,
            _pathManager,
            _fileWriter,
            _mockLogger.Object);
    }

    private TestSitePaths CreateRealisticSiteStructure()
    {
        var wikiFolder = Path.Combine(_testRootPath, "_wiki");
        var tasksFolder = Path.Combine(_testRootPath, "_tasks");
        var qaFolder = Path.Combine(_testRootPath, "_qa");

        Directory.CreateDirectory(wikiFolder);
        Directory.CreateDirectory(tasksFolder);
        Directory.CreateDirectory(qaFolder);

        WriteWikiFiles(wikiFolder);
        WriteTaskFiles(tasksFolder);
        WriteQaFiles(qaFolder);

        return new TestSitePaths
        {
            RootIndexHtmlPath = Path.Combine(_testRootPath, "index.html"),
            DataJsPath = Path.Combine(_testRootPath, "data.js"),
            IntroHtmlPath = Path.Combine(wikiFolder, "intro.html"),
            GettingStartedHtmlPath = Path.Combine(wikiFolder, "docs", "getting-started.html"),
            SiteBuilderHtmlPath = Path.Combine(wikiFolder, "docs", "architecture", "site-builder.html"),
            PathResolutionHtmlPath = Path.Combine(wikiFolder, "docs", "architecture", "path-resolution.html"),
            IntegrationTestsHtmlPath = Path.Combine(wikiFolder, "docs", "testing", "integration-tests.html"),
            MarkdownExamplesHtmlPath = Path.Combine(wikiFolder, "snippets", "markdown-examples.html"),
            DocsIndexMdPath = Path.Combine(wikiFolder, "docs", "index.md"),
            ArchitectureIndexMdPath = Path.Combine(wikiFolder, "docs", "architecture", "index.md"),
            TestingIndexMdPath = Path.Combine(wikiFolder, "docs", "testing", "index.md"),
            DocsIndexHtmlPath = Path.Combine(wikiFolder, "docs", "index.html"),
            ArchitectureIndexHtmlPath = Path.Combine(wikiFolder, "docs", "architecture", "index.html"),
            TestingIndexHtmlPath = Path.Combine(wikiFolder, "docs", "testing", "index.html"),
            TaskSiteBuilderTestsHtmlPath = Path.Combine(tasksFolder, "add-site-builder-tests.html"),
            TaskHelpPageHtmlPath = Path.Combine(tasksFolder, "improve-help-page.html"),
            TaskRelativePathsHtmlPath = Path.Combine(tasksFolder, "fix-relative-paths.html")
        };
    }

    private static void WriteWikiFiles(string wikiFolder)
    {
        var docsFolder = Path.Combine(wikiFolder, "docs");
        var architectureFolder = Path.Combine(docsFolder, "architecture");
        var testingFolder = Path.Combine(docsFolder, "testing");
        var snippetsFolder = Path.Combine(wikiFolder, "snippets");

        Directory.CreateDirectory(docsFolder);
        Directory.CreateDirectory(architectureFolder);
        Directory.CreateDirectory(testingFolder);
        Directory.CreateDirectory(snippetsFolder);

        File.WriteAllText(Path.Combine(wikiFolder, "index.md"), """
# Navigator.md

Welcome to the local knowledge base.

## Sections

- [Introduction](intro.html)
- [Getting Started](docs/getting-started.html)
- [Site Builder](docs/architecture/site-builder.html)
- [Integration Tests](docs/testing/integration-tests.html)
- [Markdown Examples](snippets/markdown-examples.html)

## Notes

This wiki stores:

- **docs**
- tasks
- Q&A
- code snippets

> Use the sidebar to navigate.
""");

        File.WriteAllText(Path.Combine(wikiFolder, "intro.md"), """
# Introduction

Navigator.md is a local static documentation generator.

## Main idea

- Store notes as markdown
- Build html locally
- Keep everything simple

## Related

- [Getting Started](docs/getting-started.html)
- [Markdown Examples](snippets/markdown-examples.html)
""");

        File.WriteAllText(Path.Combine(docsFolder, "getting-started.md"), """
# Getting Started

## Steps

1. Create `_wiki`
2. Add markdown files
3. Run the builder
4. Open `index.html`

## Example

```bash
dotnet test
dotnet run
```

## See also

- [Site Builder](architecture/site-builder.html)
""");

        File.WriteAllText(Path.Combine(architectureFolder, "site-builder.md"), """
# Site Builder

The site builder scans source folders and generates html pages.

## Responsibilities

- build wiki tree
- scan tasks
- scan Q&A
- write shared assets
- generate folder indexes

## Example Code

```csharp
var builder = new SiteBuilder(
    treeBuilder,
    taskScanner,
    qaScanner,
    pathManager,
    siteGenerator,
    logger);

builder.Build();
```

## Related

- [Path Resolution](path-resolution.html)
- [Integration Tests](../testing/integration-tests.html)
""");

        File.WriteAllText(Path.Combine(architectureFolder, "path-resolution.md"), """
# Path Resolution

Nested pages should use correct relative links to shared assets.

## Examples

- root wiki page -> `../assets/core.css`
- nested page -> `../../../../assets/core.css`

## Why it matters

Broken relative paths make generated pages unusable.
""");

        File.WriteAllText(Path.Combine(testingFolder, "integration-tests.md"), """
# Integration Tests

Integration tests should verify the full generated site.

## Good assertions

- expected html files exist
- generated data.js contains navigation data
- headings render correctly
- code blocks render correctly
- relative assets work at different nesting levels
""");

        File.WriteAllText(Path.Combine(snippetsFolder, "markdown-examples.md"), """
# Markdown Examples

## Emphasis

**Bold**, *Italic*, ***Both***, `code`, ~~old~~

## Task List

- [x] Add docs
- [ ] Add more tests

## Details

<details>
<summary>Show SQL</summary>

```sql
select *
from tasks
where status = 'Open';
```

</details>

## Table

| Name | Purpose |
|---|---|
| `_wiki` | Documentation |
| `_tasks` | Task pages |
| `data.js` | Navigation data |
""");
    }

    private static void WriteTaskFiles(string tasksFolder)
    {
        File.WriteAllText(Path.Combine(tasksFolder, "add-site-builder-tests.md"), """
---
name: Improve Site Builder Tests
status: 0
---

# Improve Site Builder Tests

## Goal

Split one large integration test into smaller tests.

## Checklist

- [x] Review current integration test
- [ ] Add realistic site structure
- [ ] Separate rendering assertions
- [ ] Separate data.js assertions
""");

        File.WriteAllText(Path.Combine(tasksFolder, "improve-help-page.md"), """
---
name: Improve Help Page
status: 1
---

# Improve Help Page

Use a compact table-based cheat sheet and clearer examples.
""");

        File.WriteAllText(Path.Combine(tasksFolder, "fix-relative-paths.md"), """
---
name: Fix Relative Paths
status: 2
---

# Fix Relative Paths

Resolved incorrect asset paths for deeply nested wiki pages.
""");
    }

    private static void WriteQaFiles(string qaFolder)
    {
        File.WriteAllText(Path.Combine(qaFolder, "why-is-index-generated.md"), """
---
question: Why is index.md generated for folders?
popularity: 3
---

# Why is index.md generated for folders?

Folders need their own landing pages so users can navigate nested sections more easily.
""");

        File.WriteAllText(Path.Combine(qaFolder, "how-relative-paths-work.md"), """
---
question: How do relative asset paths work in generated pages?
---

# How do relative asset paths work?

The deeper the generated page is, the more ../ segments it needs to reach /assets.
""");
    }

    private sealed class TestSitePaths
    {
        public required string RootIndexHtmlPath { get; init; }
        public required string DataJsPath { get; init; }
        public required string IntroHtmlPath { get; init; }
        public required string GettingStartedHtmlPath { get; init; }
        public required string SiteBuilderHtmlPath { get; init; }
        public required string PathResolutionHtmlPath { get; init; }
        public required string IntegrationTestsHtmlPath { get; init; }
        public required string MarkdownExamplesHtmlPath { get; init; }
        public required string DocsIndexMdPath { get; init; }
        public required string ArchitectureIndexMdPath { get; init; }
        public required string TestingIndexMdPath { get; init; }
        public required string DocsIndexHtmlPath { get; init; }
        public required string ArchitectureIndexHtmlPath { get; init; }
        public required string TestingIndexHtmlPath { get; init; }
        public required string TaskSiteBuilderTestsHtmlPath { get; init; }
        public required string TaskHelpPageHtmlPath { get; init; }
        public required string TaskRelativePathsHtmlPath { get; init; }
    }
}
