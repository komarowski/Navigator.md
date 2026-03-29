namespace MarkdownNavigator.Core.Domain;

public class QaScanner : IQaScanner
{
    public IEnumerable<QaItem> ScanForQa(string qaFolder)
    {
        var qaItems = new List<QaItem>();

        var qaFolderInfo = new DirectoryInfo(qaFolder);
        if (!qaFolderInfo.Exists)
        {
            return qaItems;
        }

        var files = qaFolderInfo.GetFiles("*.md", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            string? question = null;
            int popularity = 0;

            if (MarkdownManager.TryReadFrontMatter<QaFrontMatter>(file, out var qaFrontMatter)
                && qaFrontMatter != null)
            {
                question = qaFrontMatter.Question;
                popularity = qaFrontMatter.Popularity ?? 0;
            }

            if (string.IsNullOrEmpty(question))
            {
                question = MarkdownManager.ReadTitleOrDefault(
                    file,
                    Path.GetFileNameWithoutExtension(file.Name),
                    8);
            }

            var qaItem = new QaItem()
            {
                Question = question,
                File = file,
                Popularity = popularity
            };

            qaItems.Add(qaItem);
        }

        return qaItems
            .OrderByDescending(qa => qa.Popularity)
            .ThenByDescending(qa => qa.File.CreationTimeUtc);
    }
}
