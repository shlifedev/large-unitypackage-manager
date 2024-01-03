namespace SyncDir;

public static class Utils
{
    public static bool IsNetworkDrive(string path)
    {
        if (string.IsNullOrEmpty(path) || path.Length < 2 || path[1] != ':')
            throw new ArgumentException("The path is not valid.");

        char driveLetter = path[0];
        DriveInfo driveInfo = new DriveInfo(driveLetter.ToString());

        return driveInfo.DriveType == DriveType.Network;
    }
    
    public static bool IsValidDirectoryPath(string path)
    {
        return !string.IsNullOrEmpty(path) && Directory.Exists(path);
    } 
}