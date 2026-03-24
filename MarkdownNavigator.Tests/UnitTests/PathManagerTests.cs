using MarkdownNavigator.Core.Domain;
using NUnit.Framework;

namespace MarkdownNavigator.Tests.UnitTests;

public class PathManagerTests
{
    private PathManager pathManager;
    private const string TestSourceFolder = "/test/source";

    [SetUp]
    public void Setup()
    {
        var settings = new AppSettings 
        { 
            SourceFolder = TestSourceFolder
        };
        pathManager = new PathManager(settings);
    }

    [Test]
    public void ChangeExtensionToHtml_WithMultipleDots_OnlyChangesLastExtension()
    {
        // Arrange
        var markdownPath = "/path/to/file.backup.md";

        // Act
        var result = pathManager.ChangeExtensionToHtml(markdownPath);

        // Assert
        Assert.That(result, Is.EqualTo("/path/to/file.backup.html"));
    }

    [Test]
    public void GetRelativeHtmlPath_SingleParam_WithFileInSourceRoot_ReturnsSimpleFilename()
    {
        // Arrange
        var filePath = Path.Combine(TestSourceFolder, "readme.md");

        // Act
        var result = pathManager.GetRelativeHtmlPath(filePath);

        // Assert
        Assert.That(result, Is.EqualTo("readme.html"));
    }

    [Test]
    public void GetRelativeHtmlPath_TwoParams_WithCustomBasePath_CalculatesRelativePath()
    {
        // Arrange
        var basePath = Path.Combine(TestSourceFolder, "docs");
        var filePath = Path.Combine(TestSourceFolder, "docs", "guides", "tutorial.md");

        // Act
        var result = pathManager.GetRelativeHtmlPath(filePath, basePath);

        // Assert
        Assert.That(result, Is.EqualTo("guides/tutorial.html"));
    }
}