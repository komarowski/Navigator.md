namespace MarkdownNavigator.Core.Domain;

/// <summary>
/// A node object representing a folder structure.
/// </summary>
public class Node
{
    /// <summary>
    /// Node path - full file path during tree construction, relative HTML path in final output.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Folder name or markdown header. 
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Node type <see cref="NodeType"/>.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Nodes in a folder for <see cref="NodeType.Folder"/> or null for <see cref="NodeType.File"/>.
    /// </summary>
    public List<Node>? Children { get; set; }

    public Node(string path, string name, NodeType type)
    {
        Path = path;
        Name = name;
        Type = type.ToString();

        if (type == NodeType.Folder)
        {
            Children = [];
        }
    }
}