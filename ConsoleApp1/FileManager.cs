using System.Collections;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1;

public class FileManager : IEnumerable<FileSystemNode>
{
    private readonly string _path;
    private readonly string _filePattern;
    public FileSystemNode Root;
    private Regex KeywordsRegex { get; set; }
    private Regex FileRegex { get; set; }

    int totalFileCount = 0;
    int progressedFileCount = 0;

    public FileManager(string path, string filePattern)
    {
        this._path = path;
        this._filePattern = filePattern;


        Console.WriteLine("Regex Compiling.. Please Wait.");
        KeywordsRegex = new Regex(
            @"(shader[s]|3d|2d|stylized|animation[s]|rpg|medieval|lowpoly|poly|paint|terrain|texture|material|pixel|vfx|sfx|sound[s]|gui|ui|sword|bow|building|house|town|addon|prop|plant|animal|tool|sound|particle)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        FileRegex = new Regex(filePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public List<string> GetAllTags(FileSystemNode node)
    {
        if (node.Tags == null)
            node.Tags = new List<string>();

        var tags = new List<string>(node.Tags);
        foreach (var child in node.Children)
        {
            tags.AddRange(GetAllTags(child));
        }

        return tags.Distinct().ToList();
    }

    public async Task<FileSystemNode> Initialize()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Analyzing File...");
        Console.ResetColor();

        Func<FileSystemNode, Task> modifyFile = node =>
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(node.Name);

            MatchCollection matches = KeywordsRegex.Matches(fileNameWithoutExtension); // 1. 최적화된 Regex 사용
            if (matches.Count > 0)
            {
                var matchedKeywords = matches.Cast<Match>().Select(match => match.Value).ToList();
                node.Tags = matchedKeywords;
            }

            progressedFileCount++;

            Console.WriteLine($"{progressedFileCount}");
            return Task.CompletedTask;
        };
        Root = await TraverseTreeAsync(_path, _filePattern, modifyFile);

        return Root;
    }

    public void ExtreactFileFromZipWhenPatternMatched(FileSystemNode node,
        string searchPattern = ".*\\.unitypackage|.zip|.7z", bool shouldDeleteZip = false)
    {
        ExtreactFileFromZipWhenPatternMatched(node.FullPath, searchPattern, shouldDeleteZip);
    }

    private bool IsNumberingFile(string filename)
    {
        return Regex.IsMatch(filename, @"^\d+\s");
    }

    private void RenameFile(string currentFilePath, string newFileName)
    {
        string directory = Path.GetDirectoryName(currentFilePath);
        string newFilePath = Path.Combine(directory, newFileName);

        if (File.Exists(currentFilePath))
        {
            File.Move(currentFilePath, newFilePath);
        }
        else
        {
            Console.WriteLine($"The file {currentFilePath} does not exist.");
        }
    }

    private string RemovePrefixNumbersFromPath(string filePath)
    {
        string filename = Path.GetFileName(filePath);
        string directory = Path.GetDirectoryName(filePath);

        string modifiedFilename = Regex.Replace(filename, @"^\d+\s", "");

        return Path.Combine(directory, modifiedFilename);
    }

    /// <summary>
    /// 0820 Snaps Prototype  School 1.1 같은 파일 Snaps Prototype  School 1.1 으로 변환됩니다.
    /// </summary>
    /// <param name="node"></param>
    public void FixFileName(FileSystemNode node)
    {
        if (IsNumberingFile(node.Name))
        {
            RenameFile(node.FullPath, RemovePrefixNumbersFromPath(node.FullPath));
        }
    }

    /// <summary>
    /// 압축 파일에 특정 확장자가 존재하는 경우에만 그 확장자에 대해서 압축을 해제합니다.
    /// </summary> 
    public void ExtreactFileFromZipWhenPatternMatched(string filePath,
        string searchPattern = ".*\\.unitypackage|.zip|.7z", bool shouldDeleteZip = false)
    {
 
        Regex regex = new Regex(@"\.(zip|7z|tar|rar)$");
        if (!regex.IsMatch(filePath))
        {
            return;
        }

        var fileRegex = new Regex(searchPattern);

        using (var zip = ZipFile.OpenRead(filePath))
        {
            string extractPath = Path.GetDirectoryName(filePath);
            string zipFileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            int fileCounter = 1; // Initialize counter

            foreach (var entry in zip.Entries)
            {
                if (fileRegex.Match(entry.Name).Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{filePath}");
                    Console.ResetColor();
                    Console.WriteLine(
                        $"Uncompressing..  {searchPattern} Pattern Applied and Original .zip File Will {(shouldDeleteZip ? "Delete" : "Not Delete.")}");
                    Console.ResetColor();
                    
                    string newFileName =
                        $"{zipFileNameWithoutExt}--({Path.GetFileNameWithoutExtension(entry.Name)}){Path.GetExtension(entry.Name)}";
                    entry.ExtractToFile(Path.Combine(extractPath, newFileName), overwrite: true);
                }
            }
        }

        if (shouldDeleteZip)
        {
            File.Delete(filePath);
        }
    }

    public void PrintFileSystemTree(FileSystemNode node, string indent)
    {
        var nodeType = node.NodeType == NodeType.Directory ? "<D>" : "<F>";
        Console.WriteLine($"{indent}{node.Name} {nodeType}");
        foreach (var child in node.Children)
        {
            PrintFileSystemTree(child, indent + "\t");
        }
    }

    public void FindFilesByName(FileSystemNode node, string targetFileName, List<string> matchingFiles)
    {
        if (node.Name.ToLowerInvariant().Contains(targetFileName.ToLowerInvariant()) &&
            FilePatternMatch(node.FullPath))
        {
            matchingFiles.Add(node.FullPath);
        }

        foreach (var childNode in node.Children)
        {
            FindFilesByName(childNode, targetFileName, matchingFiles);
        }
    }

    bool FilePatternMatch(string filePath)
    {
        var fileExt = Path.GetExtension(filePath);
        return FileRegex.IsMatch(fileExt);
    }

    public void FindFilesByTag(FileSystemNode node, string targetTags, List<string> resultFiles)
    {
        if (node == null)
            return;

        var targetTagArray = targetTags.Split(',').Select(tag => tag.Trim().ToLowerInvariant()).ToList();

        if (node.Tags != null &&
            targetTagArray.All(targetTag => node.Tags.Contains(targetTag, StringComparer.OrdinalIgnoreCase)) &&
            FilePatternMatch(node.FullPath))
        {
            resultFiles.Add(node.Name);
        }

        foreach (var childNode in node.Children)
        {
            FindFilesByTag(childNode, targetTags, resultFiles);
        }
    }


    protected async Task<FileSystemNode> TraverseTreeAsync(string root, string regexPattern,
        Func<FileSystemNode, Task> fileModifier = null)
    {
        var node = new FileSystemNode { Name = Path.GetFileName(root), FullPath = root };
        var attr = File.GetAttributes(root);
        var regex = new Regex(regexPattern);

        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        {
            node.NodeType = NodeType.Directory;

            var dirTasks = Directory.GetDirectories(root)
                .Select(dir => TraverseTreeAsync(dir, regexPattern, fileModifier));

            var fileTasks = Directory.GetFiles(root)
                .Where(file => regex.IsMatch(file))
                .Select(async file =>
                {
                    var fileNode = new FileSystemNode
                        { Name = Path.GetFileName(file), NodeType = NodeType.File, FullPath = file };
                    if (fileModifier != null)
                    {
                        await fileModifier(fileNode);
                    }

                    return fileNode;
                });

            var children = await Task.WhenAll(dirTasks.Concat(fileTasks));
            node.Children.AddRange(children);
        }
        else
        {
            node.NodeType = NodeType.File;
        }

        if (fileModifier != null)
        {
            await fileModifier(node);
        }

        return node;
    }


    public IEnumerator<FileSystemNode> GetEnumerator()
    {
        yield return Root;

        // Then return all children
        foreach (var child in Root.Children)
        {
            // Use recursion to get all descendants of each child
            foreach (var descendant in child.Children)
            {
                yield return descendant;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}