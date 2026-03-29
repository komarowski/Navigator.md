using NUnit.Framework;
using MarkdownNavigator.Core.Domain;

namespace MarkdownNavigator.Tests.IntegrationTests;

public class TreeBuilderTests
{
    private string _testRootPath;

    [SetUp]
    public void Setup()
    {
        _testRootPath = Path.Combine(Path.GetTempPath(), $"TreeBuilderTest_{Guid.NewGuid()}");
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

    /// <summary>
    /// Creates a test folder for testing.
    /// </summary>
    private void CreateTestFolder()
    {
        if (!Directory.Exists(_testRootPath))
        {
            Directory.CreateDirectory(_testRootPath);
        }
    }

    /// <summary>
    /// Creates a sample folder structure for testing.
    /// </summary>
    private void CreateTestStructure()
    {
        CreateTestFolder();

        // Root level
        File.WriteAllText(Path.Combine(_testRootPath, "intro.md"), "# Introduction\n\nIntro content");

        // docs folder
        var docsPath = Path.Combine(_testRootPath, "docs");
        Directory.CreateDirectory(docsPath);
        File.WriteAllText(Path.Combine(docsPath, "index.md"), "# Documentation\n\nDocs content");
        File.WriteAllText(Path.Combine(docsPath, "guide.md"), "# Guide\n\nGuide content");

        // docs/api folder
        var apiPath = Path.Combine(docsPath, "api");
        Directory.CreateDirectory(apiPath);
        File.WriteAllText(Path.Combine(apiPath, "index.md"), "# API Reference\n\nAPI docs");
        File.WriteAllText(Path.Combine(apiPath, "endpoints.md"), "# Endpoints\n\nEndpoint docs");

        // tutorials folder
        var tutorialsPath = Path.Combine(_testRootPath, "tutorials");
        Directory.CreateDirectory(tutorialsPath);
        File.WriteAllText(Path.Combine(tutorialsPath, "index.md"), "# Tutorials\n\nTutorial content");
    }

    [Test]
    public void GetTreeStructure_PopulatesChildNodesCorrectly()
    {
        // Arrange
        CreateTestStructure();
        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert
        Assert.That(tree.RootNode?.Children, Is.Not.Null);
        Assert.That(tree.RootNode.Children, Has.Count.EqualTo(3)); // intro.md + docs + tutorials
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "Introduction"), Is.True);
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "Documentation"), Is.True);
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "Tutorials"), Is.True);
    }

    [Test]
    public void GetTreeStructure_CollectsAllMarkdownFilesForConversion()
    {
        // Arrange
        CreateTestStructure();
        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert
        Assert.That(tree.MarkdownFilesToConvert, Has.Count.EqualTo(6));
    }

    [Test]
    public void GetTreeStructure_HandlesDeepNesting()
    {
        // Arrange - Create 5 levels deep
        CreateTestStructure();
        var deepPath = Path.Combine(_testRootPath, "docs", "api", "v1", "endpoints", "users");
        Directory.CreateDirectory(deepPath);
        File.WriteAllText(Path.Combine(deepPath, "index.md"), "# Users API\n\nUsers endpoints");
        File.WriteAllText(Path.Combine(deepPath, "list.md"), "# List Users\n\nList all users");
        
        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert - Navigate through deep structure
        var docsNode = tree.RootNode?.Children?.FirstOrDefault(n => n.Name == "Documentation");
        var apiNode = docsNode?.Children?.FirstOrDefault(n => n.Name == "API Reference");
        var v1Node = apiNode?.Children?.FirstOrDefault(n => n.Name == "v1");
        var endpointsNode = v1Node?.Children?.FirstOrDefault(n => n.Name == "endpoints");
        var usersNode = endpointsNode?.Children?.FirstOrDefault(n => n.Name == "Users API");
        
        Assert.That(usersNode, Is.Not.Null);
        Assert.That(usersNode.Children, Is.Not.Null);
        Assert.That(usersNode.Children, Has.Count.EqualTo(1));
        Assert.That(usersNode.Children.First().Name, Is.EqualTo("List Users"));
    }

    [Test]
    public void GetTreeStructure_GeneratesIndexFilesForMissingFolders()
    {
        // Arrange - Create folders without index.md at multiple levels
        CreateTestFolder();
        var emptyFolders = Path.Combine(_testRootPath, "guides", "quick-start", "setup");
        Directory.CreateDirectory(emptyFolders);
        File.WriteAllText(Path.Combine(_testRootPath, "index.md"), "# Root");
        File.WriteAllText(Path.Combine(emptyFolders, "install.md"), "# Install");

        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert - Should generate index files for guides, quick-start, and setup
        Assert.That(tree.IndexFilesToGenerate, Is.Not.Empty);
        Assert.That(tree.IndexFilesToGenerate.Any(f => f.FullName.EndsWith("guides\\index.md")), Is.True);
        Assert.That(tree.IndexFilesToGenerate.Any(f => f.FullName.EndsWith("quick-start\\index.md")), Is.True);
        Assert.That(tree.IndexFilesToGenerate.Any(f => f.FullName.EndsWith("setup\\index.md")), Is.True);
    }

    [Test]
    public void GetTreeStructure_ExcludesSystemAndConfigFolders()
    {
        // Arrange
        CreateTestStructure();
        Directory.CreateDirectory(Path.Combine(_testRootPath, ".git"));
        Directory.CreateDirectory(Path.Combine(_testRootPath, ".vs"));
        
        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert - Excluded folders should not appear
        var children = tree.RootNode?.Children ?? [];
        Assert.That(children.Any(n => n.Name == ".git"), Is.False);
        Assert.That(children.Any(n => n.Name == ".vs"), Is.False);
    }

    [Test]
    public void GetTreeStructure_HandlesMultipleMixedContentAtSameLevel()
    {
        // Arrange - Same level has both folders and markdown files
        CreateTestFolder();
        File.WriteAllText(Path.Combine(_testRootPath, "index.md"), "# Root");
        File.WriteAllText(Path.Combine(_testRootPath, "overview.md"), "# Overview");
        File.WriteAllText(Path.Combine(_testRootPath, "quickstart.md"), "# Quick Start");
        File.WriteAllText(Path.Combine(_testRootPath, "faq.md"), "# FAQ");
        
        var docsPath = Path.Combine(_testRootPath, "docs");
        Directory.CreateDirectory(docsPath);
        File.WriteAllText(Path.Combine(docsPath, "index.md"), "# Docs");
        
        var examplesPath = Path.Combine(_testRootPath, "examples");
        Directory.CreateDirectory(examplesPath);
        File.WriteAllText(Path.Combine(examplesPath, "index.md"), "# Examples");
        File.WriteAllText(Path.Combine(examplesPath, "basic.md"), "# Basic Example");
        
        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert - All content should be present
        Assert.That(tree?.RootNode?.Children, Is.Not.Null);
        Assert.That(tree.RootNode.Children, Has.Count.EqualTo(5)); // 3 md files + 2 folders
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "Overview"), Is.True);
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "Quick Start"), Is.True);
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "FAQ"), Is.True);
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "Docs"), Is.True);
        Assert.That(tree.RootNode.Children.Any(n => n.Name == "Examples"), Is.True);
    }

    [Test]
    public void GetTreeStructure_HandlesIndexFilesWithoutHeadings()
    {
        // Arrange - index.md files without # heading should use folder name
        CreateTestFolder();
        File.WriteAllText(Path.Combine(_testRootPath, "index.md"), "Just text, no heading");
        
        var referenceFolder = Path.Combine(_testRootPath, "reference");
        Directory.CreateDirectory(referenceFolder);
        File.WriteAllText(Path.Combine(referenceFolder, "index.md"), "No h1 heading here\n\nJust content");
        File.WriteAllText(Path.Combine(referenceFolder, "api.md"), "# API");

        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert - Should fallback to folder/file names
        var refNode = tree.RootNode?.Children?.FirstOrDefault(n => n.Name == "reference");
        Assert.That(refNode, Is.Not.Null); // Uses folder name as fallback
        Assert.That(refNode.Children, Is.Not.Null);
        Assert.That(refNode.Children, Has.Count.EqualTo(1));
        Assert.That(refNode.Children.First().Name, Is.EqualTo("API"));
    }

    [Test]
    public void GetTreeStructure_TracksBothIndexAndContentFilesForConversion()
    {
        // Arrange - Mix of folders with/without index.md
        CreateTestFolder();
        File.WriteAllText(Path.Combine(_testRootPath, "root.md"), "# Root");
        File.WriteAllText(Path.Combine(_testRootPath, "readme.md"), "# Readme");
        
        var tutoPath = Path.Combine(_testRootPath, "tutorials");
        Directory.CreateDirectory(tutoPath);
        File.WriteAllText(Path.Combine(tutoPath, "index.md"), "# Tutorials");
        File.WriteAllText(Path.Combine(tutoPath, "intro.md"), "# Intro");
        File.WriteAllText(Path.Combine(tutoPath, "advanced.md"), "# Advanced");
        
        var buildPath = Path.Combine(_testRootPath, "build");
        Directory.CreateDirectory(buildPath);
        // No index.md here
        File.WriteAllText(Path.Combine(buildPath, "compile.md"), "# Compile");

        var treeBuilder = new TreeBuilder();

        // Act
        var tree = treeBuilder.GetTreeStructure(_testRootPath);

        // Assert
        // Should track: root.md, readme.md, tutorials/index.md, tutorials/intro.md, 
        //              tutorials/advanced.md, build/compile.md (6 files)
        Assert.That(tree.MarkdownFilesToConvert, Has.Count.EqualTo(6));
        // Should generate: build/index.md (1 file)
        Assert.That(tree.IndexFilesToGenerate, Has.Count.EqualTo(1));
    }
}