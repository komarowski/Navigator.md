using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
            var markdownContent = File.ReadAllText(file.FullName);
            if (TryGetQaFrontMatter(markdownContent, out var qaFrontMatter))
            {
                if (qaFrontMatter == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(qaFrontMatter.Question))
                {
                    continue;
                }

                var qaItem = new QaItem() 
                { 
                    Question = qaFrontMatter.Question,
                    File = file,
                    Context = qaFrontMatter.Context,
                    SearchTags = qaFrontMatter.SearchTags ?? [],
                    Popularity = qaFrontMatter.Popularity,
                    UpdatedUtc = file.LastWriteTimeUtc
                };

                qaItems.Add(qaItem);
            }
        }

        return qaItems;
    }

    private static bool TryGetQaFrontMatter(string markdownContent, out QaFrontMatter? qaFrontMatter)
    {
        qaFrontMatter = null;

        try
        {
            var document = MarkdownManager.Parse(markdownContent);
            var yamlBlock = document
                .Descendants<YamlFrontMatterBlock>()
                .FirstOrDefault();


            if (yamlBlock != null)
            {
                string yaml = yamlBlock.Lines.ToString();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var metadata = deserializer.Deserialize<QaFrontMatter>(yaml);
                qaFrontMatter = metadata;

                return true;
            }
        }
        catch (YamlException)
        {
            return false;
        }

        return false;
    }
}
