namespace MarkdownNavigator.Domain.Entities
{
  public enum NodeType
  {
    Folder,
    File
  }

  /// <summary>
  /// A node object representing a folder structure.
  /// </summary>
  public class Node
  {
    /// <summary>
    /// Node Id (html relative path).
    /// </summary>
    public string Id { get; set; }

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

    public Node(string id, string name, NodeType type)
    {
      Id = id;
      Name = name;
      Type = type.ToString();
      if (type == NodeType.Folder)
      {
        Children = [];
      }
    }
  }
}
