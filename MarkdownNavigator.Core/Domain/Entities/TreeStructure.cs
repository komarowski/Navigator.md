namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// Storing information about the source folder structure.
/// </summary>
public class TreeStructure
{
    /// <summary>
    /// Root node of the folder tree structure.
    /// </summary>
    public Node RootNode { get; set; }

    /// <summary>
    /// List of markdown files to convert to html.
    /// </summary>
    public List<FileInfo> MarkdownFilesToConvert { get; }

    /// <summary>
    /// List of index markdown files to generate for folders without index.md.
    /// </summary>
    public List<FileInfo> IndexFilesToGenerate { get; }

    /// <summary>
    /// Storing information about the folder structure.
    /// </summary>
    public TreeStructure()
    {
        RootNode = new Node("", "root", NodeType.Folder);
        MarkdownFilesToConvert = [];
        IndexFilesToGenerate = [];
    }

    /// <summary>
    /// Add a child node to the given parent node.
    /// </summary>
    /// <param name="parentNode">Parent node to add child to.</param>
    /// <param name="path">Node path.</param>
    /// <param name="name">Node name.</param>
    /// <param name="type">Node type.</param>
    /// <returns>The newly created node.</returns>
    public static Node AddNode(Node parentNode, string path, string name, NodeType type)
    {
        var node = new Node(path, name, type);
        parentNode.Children?.Add(node);
        return node;
    }
}