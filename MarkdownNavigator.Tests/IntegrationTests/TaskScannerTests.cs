using MarkdownNavigator.Core.Domain;
using NUnit.Framework;
using System.Text;

namespace MarkdownNavigator.Tests.IntegrationTests;

public class TaskScannerTests
{
    private string _testTasksFolder;
    private TaskScanner _taskScanner;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _testTasksFolder = Path.Combine(Path.GetTempPath(), $"TaskScannerTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testTasksFolder);

        _taskScanner = new TaskScanner();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (Directory.Exists(_testTasksFolder))
        {
            Directory.Delete(_testTasksFolder, true);
        }
    }

    [Test]
    public void ScanForTasks_ReturnsTasksFromMarkdownFiles()
    {
        // Arrange
        CreateTaskMarkdownFile(
            "task1.md",
            "Task 1",
            0);

        CreateTaskMarkdownFile(
            "task2.md",
            "Task 2",
            1);

        CreateTaskMarkdownFile(
            "task3.md",
            "Task 3",
            2);

        File.WriteAllText(Path.Combine(_testTasksFolder, "task4.md"), "Empty task");

        File.WriteAllText(Path.Combine(_testTasksFolder, "task5.md"), "---\nname: Task 6\n--\n\n# Incorrect fontmatter");

        File.WriteAllText(Path.Combine(_testTasksFolder, "task6.md"), "---\nNaMe: Task 7\n---\n\n# Incorrect fontmatter field");

        // Act
        var tasks = _taskScanner.ScanForTasks(_testTasksFolder).ToList();

        // Assert
        Assert.That(tasks, Has.Count.EqualTo(6));
        Assert.That(tasks, Has.Some.Matches<TaskItem>(t => t.Name == "Task 1" && t.Status == TaskItemStatus.Open));
        Assert.That(tasks, Has.Some.Matches<TaskItem>(t => t.Name == "Task 2" && t.Status == TaskItemStatus.Frozen));
        Assert.That(tasks, Has.Some.Matches<TaskItem>(t => t.Name == "Task 3" && t.Status == TaskItemStatus.Closed && t.File.FullName == Path.Combine(_testTasksFolder, "task3.md")));
        Assert.That(tasks, Has.Some.Matches<TaskItem>(t => t.Name == "task4" && t.Status == TaskItemStatus.Open));
        Assert.That(tasks, Has.Some.Matches<TaskItem>(t => t.Name == "Incorrect fontmatter" && t.Status == TaskItemStatus.Open));
        Assert.That(tasks, Has.Some.Matches<TaskItem>(t => t.Name == "Incorrect fontmatter field" && t.Status == TaskItemStatus.Open));
    }

    [Test]
    public void ScanForTasks_ReturnsEmptyListForNonExistentFolder()
    {
        // Arrange
        var nonExistentFolder = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid()}");

        // Act
        var tasks = _taskScanner.ScanForTasks(nonExistentFolder).ToList();

        // Assert
        Assert.That(tasks, Is.Empty);
    }

    /// <summary>
    /// Creates a markdown file with task front matter for testing.
    /// </summary>
    private void CreateTaskMarkdownFile(
        string fileName,
        string? taskName = null,
        int? status = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("---");

        if (taskName != null)
        {
            sb.AppendLine($"name: {taskName}");
        }

        if (status != null)
        {
            sb.AppendLine($"status: {status}");
        }

        sb.AppendLine("---");
        sb.AppendLine("\n\n# Task Content\n\nThis is the task content.");

        File.WriteAllText(Path.Combine(_testTasksFolder, fileName), sb.ToString());
    }
}
