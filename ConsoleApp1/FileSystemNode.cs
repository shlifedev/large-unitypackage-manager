public class FileSystemNode
{
    public string Name { get; set; }

    public NodeType NodeType { get; set; }

    public string FullPath { get; set; }
    public List<string> Tags { get; set; }

    public List<FileSystemNode> Children { get; set; } = new List<FileSystemNode>();
}