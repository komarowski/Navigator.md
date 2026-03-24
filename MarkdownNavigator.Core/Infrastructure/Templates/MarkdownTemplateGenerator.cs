namespace MarkdownNavigator.Core.Infrastructure;

public static class MarkdownTemplateGenerator
{
    public static string GetWikiFolderTemplate(string folderName)
    {
        return $"# {folderName}\n\nThis file was auto-generated.";
    }

    public static string GetIndexTemplate()
    {
        return @"# Navigator.md

Welcome to your local documentation wiki.";
    }
}
