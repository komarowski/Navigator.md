using MarkdownNavigator.Core.Domain;
using NUnit.Framework;
using System.Text;

namespace MarkdownNavigator.Tests.IntegrationTests;

public class QaScannerTests
{
    private string _testQaFolder;
    private QaScanner _qaScanner;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _testQaFolder = Path.Combine(Path.GetTempPath(), $"QaScannerTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testQaFolder);

        _qaScanner = new QaScanner();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (Directory.Exists(_testQaFolder))
        {
            Directory.Delete(_testQaFolder, true);
        }
    }

    [Test]
    public void ScanForQa_ReturnsQaItemsFromMarkdownFiles()
    {
        // Arrange
        CreateQaMarkdownFile(
            "faq1.md",
            "What is .NET?",
            "A comprehensive framework for building applications",
            ["dotnet", "framework"],
            5);

        CreateQaMarkdownFile(
            "faq2.md",
            "How do I install Visual Studio?",
            context: "Installation guide for developers",
            searchTags: ["visual-studio", "installation"]);

        CreateQaMarkdownFile(
            "faq3.md",
            "What is ASP.NET Core?",
            popularity: 10);

        CreateQaMarkdownFile(Path.Combine("subdir", "nested_qa.md"), "Question in subdirectory?", searchTags: ["nested"]);

        File.WriteAllText(Path.Combine(_testQaFolder, "no_frontmatter.md"), "# No front matter here");
        File.WriteAllText(Path.Combine(_testQaFolder, "invalid_frontmatter.md"), "---\nWrong: Format\n--\n\n# Content");

        // Act
        var qaItems = _qaScanner.ScanForQa(_testQaFolder).ToList();

        // Assert
        Assert.That(qaItems, Has.Count.EqualTo(4));
        Assert.That(qaItems, Has.Some.Matches<QaItem>(q => q.Question == "What is .NET?" && q.Popularity == 5 && q.SearchTags.Contains("framework")));
        Assert.That(qaItems, Has.Some.Matches<QaItem>(q => q.Question == "How do I install Visual Studio?" && q.Context == "Installation guide for developers" && q.SearchTags.Contains("installation")));
        Assert.That(qaItems, Has.Some.Matches<QaItem>(q => q.Question == "What is ASP.NET Core?" && q.Popularity == 10));
        Assert.That(qaItems, Has.Some.Matches<QaItem>(q => q.Question == "Question in subdirectory?" && q.SearchTags.Contains("nested")));
    }

    [Test]
    public void ScanForQa_ReturnsEmptyListForNonExistentFolder()
    {
        // Arrange
        var nonExistentFolder = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid()}");

        // Act
        var qaItems = _qaScanner.ScanForQa(nonExistentFolder).ToList();

        // Assert
        Assert.That(qaItems, Is.Empty);
    }

    /// <summary>
    /// Creates a markdown file with Q&A front matter for testing.
    /// </summary>
    private void CreateQaMarkdownFile(
        string fileName,
        string? question = null,
        string? context = null,
        List<string>? searchTags = null,
        int popularity = 0)
    {
        var fullPath = Path.Combine(_testQaFolder, fileName);
        
        // Create subdirectory if needed
        var directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        var sb = new StringBuilder();
        sb.AppendLine("---");

        if (question != null)
        {
            sb.AppendLine($"question: {question}");
        }

        if (context != null)
        {
            sb.AppendLine($"context: {context}");
        }

        if (searchTags != null && searchTags.Count > 0)
        {
            sb.AppendLine($"searchTags: [{string.Join(", ", searchTags)}]");
        }

        if (popularity > 0)
        {
            sb.AppendLine($"popularity: {popularity}");
        }

        sb.AppendLine("---");
        sb.AppendLine("\n\n# Answer\n\nThis is the Q&A content.");

        File.WriteAllText(fullPath, sb.ToString());
    }
}
