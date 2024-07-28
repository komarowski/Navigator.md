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
    public List<MarkdownFile> MdFilesToConvert { get; }

    /// <summary>
    /// List of markdown files to convert to html.
    /// </summary>
    public List<ImageFile> FilesToCopy { get; set; }

    /// <summary>
    /// Storing information about the folder structure.
    /// </summary>
    public TreeStructure()
    {
      this.RootNode = new Node("", "root", "");
      this.CurrentNode = RootNode;
      this.MdFilesToConvert = [];
      this.FilesToCopy = [];
    }

    /// <summary>
    /// Add file node to tree view.
    /// </summary>
    /// <param name="filePath">Absolute file path.</param>
    /// <param name="fileCode">File code.</param>
    /// <param name="title">File title.</param>
    public void AddFileNode(string fileCode, string title, string href)
    {
      var node = new Node(fileCode, title, NodeType.File.ToString(), href);
      this.CurrentNode.Children!.Add(node);
    }

    /// <summary>
    /// Add start of folder block.
    /// </summary>
    /// <param name="folderCode">Folder code.</param>
    /// <param name="folderName">Folder name.</param>
    public Node AddFolderNode(string folderCode, string folderName)
    {
      var node = new Node(folderCode, folderName, NodeType.Folder.ToString());
      this.CurrentNode.Children!.Add(node);
      return node;
    }

    /// <summary>
    /// Add a markdown to files to generate list.
    /// </summary>
    /// <param name="markdownPath">Markdown path.</param>
    /// <param name="code">File code.</param>
    public void AddMarkdownToUpdate(string markdownPath, string code)
    {
      this.MdFilesToConvert.Add(new MarkdownFile { Code = code, SourcePath = markdownPath });
    }
  }
}