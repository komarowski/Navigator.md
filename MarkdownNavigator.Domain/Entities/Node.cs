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
    /// Node Id.
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
    /// Local file location for <see cref="NodeType.File"/> or null for <see cref="NodeType.Folder"/>.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Nodes in a folder for <see cref="NodeType.Folder"/> or null for <see cref="NodeType.File"/>.
    /// </summary>
    public List<Node>? Children { get; set; }

    public Node(string id, string name, string type)
    {
      Id = id;
      Name = name;
      Type = type;
      Children = [];
    }

    public Node(string id, string name, string type, string link)
    {
      Id = id;
      Name = name;
      Type = type;
      Link = link;
    }
  }
}
