using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MarkdownNavigator.Core.Domain;

public class TaskScanner : ITaskScanner
{
    public IEnumerable<TaskItem> ScanForTasks(string tasksFolder)
    {
        var tasks = new List<TaskItem>();

        if (!Directory.Exists(tasksFolder))
        {
            return tasks;
        }

        var files = Directory.GetFiles(tasksFolder, "*.md", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var markdownContent = File.ReadAllText(file);
            if (TryGetTaskFrontMatter(markdownContent, out var taskFrontMatter))
            {
                if (taskFrontMatter == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(taskFrontMatter.Name))
                {
                    continue;
                }

                var task = new TaskItem() 
                { 
                    Name = taskFrontMatter.Name,
                    File = new FileInfo(file),
                    Description = taskFrontMatter.Description,
                    Status = taskFrontMatter.Status,
                    ExternalLink = taskFrontMatter.Link,
                };

                tasks.Add(task);
            }
        }

        return tasks;
    }

    private static bool TryGetTaskFrontMatter(string markdownContent, out TaskFrontMatter? taskFrontMatter)
    {
        taskFrontMatter = null;

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

                var metadata = deserializer.Deserialize<TaskFrontMatter>(yaml);
                taskFrontMatter = metadata;

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
