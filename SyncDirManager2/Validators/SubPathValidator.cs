 
public class SubPathValidator : IValidator
{
    private readonly string _pathA;
    private readonly string _pathB;

    public SubPathValidator(string pathA, string pathB)
    {
        _pathA = pathA;
        _pathB = pathB;
    }
    public bool Validate()
    {
        var dirInfoA = new System.IO.DirectoryInfo(_pathA);
        var dirInfoB = new System.IO.DirectoryInfo(_pathB);

        var subDirs = dirInfoA.GetDirectories();
        var subDirs2 = dirInfoB.GetDirectories();
        var subDirsA = dirInfoA.GetDirectories().Select(d => d.Name).OrderBy(n => n).ToList();
        var subDirsB = dirInfoB.GetDirectories().Select(d => d.Name).OrderBy(n => n).ToList();

        bool areEqual = subDirsA.SequenceEqual(subDirsB);

        if (dirInfoA.GetDirectories().Length == 0) return false;
        return areEqual;
    }
}