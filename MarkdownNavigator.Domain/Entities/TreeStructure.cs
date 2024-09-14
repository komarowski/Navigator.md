namespace MarkdownNavigator.Domain.Entities
{
  /// <summary>
  /// Storing information about the source folder structure.
  /// </summary>
  public class TreeStructure
  {
    /// <summary>
    /// Stores information about the current node only.
    /// </summary>
    public Node CurrentNode { get; set; }

    /// <summary>
    /// Stores information about all nodes.
    /// </summary>
    public Node RootNode { get; }

    /// <summary>
    /// List of markdown files to convert to html.
    /// </summary>
    public List<string> MdFilesToConvert { get; }

    /// <summary>
    /// Storing information about the folder structure.
    /// </summary>
    public TreeStructure()
    {
      this.RootNode = new Node("", "root", NodeType.Folder);
      this.CurrentNode = RootNode;
      this.MdFilesToConvert = [];
    }

    /// <summary>
    /// Add file node to tree view.
    /// </summary>
    /// <param name="nodeId">Node id.</param>
    /// <param name="name">Node name.</param>
    public void AddFileNode(string nodeId, string name)
    {
      var node = new Node(nodeId, name, NodeType.File);
      this.CurrentNode.Children!.Add(node);
    }

    /// <summary>
    /// Add start of folder block.
    /// </summary>
    /// <param name="nodeId">Folder code.</param>
    /// <param name="folderName">Folder name.</param>
    public Node AddFolderNode(string nodeId, string folderName)
    {
      var node = new Node(nodeId, folderName, NodeType.Folder);
      this.CurrentNode.Children!.Add(node);
      return node;
    }

    /// <summary>
    /// Add a markdown to files to generate list.
    /// </summary>
    /// <param name="markdownPath">Markdown path.</param>
    public void AddMarkdownToUpdate(string markdownPath)
    {
      this.MdFilesToConvert.Add(markdownPath);
    }
  }
}