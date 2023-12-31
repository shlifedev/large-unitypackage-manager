using System.Text.RegularExpressions;

 
 

public class FileSystemNode 
{
    
    private Regex VersionRegex = new Regex(@"\bv?(\d+(\.\d+)+)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public string Name { get; set; }
    private string _CachedVersion = null;
    public NodeType NodeType { get; set; }

    public string FullPath { get; set; }
    public List<string> Tags { get; set; }

    
    /// <summary>
    /// 정확하지 않을 수 있습니다.
    /// </summary>
    public string Version
    {
        get
        {
            if (!string.IsNullOrEmpty(_CachedVersion))
                return _CachedVersion;
            try
            {
                var match = VersionRegex.Matches(Name)[0].Value;
                if (string.IsNullOrEmpty(_CachedVersion))
                    _CachedVersion = match;

                return _CachedVersion;
            }
            catch(Exception e)
            { 
                return null;
            }

            return null;
        }
        
    }
    public List<FileSystemNode> Children { get; set; } = new List<FileSystemNode>(); 
}