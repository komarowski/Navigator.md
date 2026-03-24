namespace MarkdownNavigator.Core.Domain;

public class TreeBuilder : ITreeBuilder
{
    private const string IndexFileName = "index.md";

    public TreeStructure GetTreeStructure(string rootFolder)
    {;
        var tree = new TreeStructure();
        var wikiDir = new DirectoryInfo(rootFolder);
        if (!wikiDir.Exists)
        {
            return tree;
        }

        ProcessMarkdownFiles(wikiDir, rootFolder, tree, tree.RootNode);
        
        // Walk subdirectories
        var directories = GetSubdirectories(wikiDir);
        foreach (var subDir in directories)
        {
            WalkDirectory(subDir, rootFolder, tree, tree.RootNode);
        }
        
        return tree;
    }

    /// <summary>
    /// Recursively walks a directory: creates folder node, processes its contents and subdirectories.
    /// </summary>
    private void WalkDirectory(DirectoryInfo dirInfo, string rootFolder, TreeStructure tree, Node parentNode)
    {
        var indexFile = new FileInfo(Path.Combine(dirInfo.FullName, IndexFileName));

        var title = indexFile.Exists 
            ? ExtractFileTitle(indexFile, dirInfo.Name) 
            : dirInfo.Name;

        var folderNode = TreeStructure.AddNode(parentNode, indexFile.FullName, title, NodeType.Folder);

        ProcessMarkdownFiles(dirInfo, rootFolder, tree, folderNode);
        
        // Recursively walk subdirectories
        var subdirectories = GetSubdirectories(dirInfo);
        foreach (var subDir in subdirectories)
        {
            WalkDirectory(subDir, rootFolder, tree, folderNode);
        }
    }

    /// <summary>
    /// Processes folder contents: tracks index.md and adds markdown files as nodes.
    /// </summary>
    private static void ProcessMarkdownFiles(DirectoryInfo dirInfo, string rootFolder, TreeStructure tree, Node folderNode)
    {
        // Handle index.md
        var indexFile = new FileInfo(Path.Combine(dirInfo.FullName, IndexFileName));
        
        if (!indexFile.Exists)
        {
            tree.IndexFilesToGenerate.Add(indexFile);
        }
        else
        {
            tree.MarkdownFilesToConvert.Add(indexFile);
        }
        
        // Process markdown files
        var markdownFiles = dirInfo
            .GetFiles("*.md")
            .Where(file => file.Name != IndexFileName);
        
        foreach (var file in markdownFiles)
        {
            var title = ExtractFileTitle(file, Path.GetFileNameWithoutExtension(file.Name));
            TreeStructure.AddNode(folderNode, file.FullName, title, NodeType.File);
            tree.MarkdownFilesToConvert.Add(file);
        }
    }

    /// <summary>
    /// Extracts the file title from the first h1 heading (# ...).
    /// Falls back to defaultTitle if no heading found in first 3 lines.
    /// </summary>
    private static string ExtractFileTitle(FileInfo file, string defaultTitle)
    {
        if (!file.Exists)
        {
            return defaultTitle;
        }

        try
        {
            using var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(fileStream);
            
            for (int i = 0; i < 3; i++)
            {
                var line = reader.ReadLine();
                if (line != null && line.TrimStart().StartsWith("# "))
                {
                    return line.Trim()[2..].Trim();
                }
            }
        }
        catch
        {
            // Fall through to filename
        }

        return defaultTitle;
    }

    private DirectoryInfo[] GetSubdirectories(DirectoryInfo dirInfo)
    {
        return dirInfo
            .GetDirectories()
            .Where(d => !d.Name.StartsWith('.')) // Exclude hidden folders
            .ToArray();
    }
}
