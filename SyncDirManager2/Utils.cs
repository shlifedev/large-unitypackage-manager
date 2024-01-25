using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SyncDir;

public static class Utils
{
    /// <summary>
    /// 네트워크 드라이브 유무 체크
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool IsNetworkDrive(string path)
    {
        if (string.IsNullOrEmpty(path) || path.Length < 2 || path[1] != ':')
            throw new ArgumentException("The path is not valid.");

        char driveLetter = path[0];
        DriveInfo driveInfo = new DriveInfo(driveLetter.ToString());

        return driveInfo.DriveType == DriveType.Network;
    }

    public static List<string> FindEmptyMp4AndSrtFiles(string directoryPath)
    {
        List<string> emptyFiles = new List<string>();

        string[] mp4Files = Directory.GetFiles(directoryPath, "*.mp4", SearchOption.AllDirectories);
        string[] srtFiles = Directory.GetFiles(directoryPath, "*.srt", SearchOption.AllDirectories);

        foreach (string mp4File in mp4Files)
        {
            if (new FileInfo(mp4File).Length == 0)
            {
                emptyFiles.Add(mp4File);
            }
        }

        foreach (string srtFile in srtFiles)
        {
            if (new FileInfo(srtFile).Length == 0)
            {
                emptyFiles.Add(srtFile);
            }
        }

        return emptyFiles;
    }
    /// <summary>
    /// 경로가 존재하는지 확인
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsValidDirectoryPath(string path)
    {
        return !string.IsNullOrEmpty(path) && Directory.Exists(path);
    }

 
    
    
    public static void CheckFilesInDirectory(string dirPath, string ext1, string ext2)
    {
        var filesExt1 = Directory.GetFiles(dirPath, $"*.{ext1}", SearchOption.AllDirectories);
        var filesExt2 = Directory.GetFiles(dirPath, $"*.{ext2}", SearchOption.AllDirectories);

        foreach (var fileExt1 in filesExt1)
        {
            if (fileExt1.EndsWith("_ko.srt") || fileExt1.EndsWith("_vi.srt")) continue;
            
            var matchingFileExt2 = Path.ChangeExtension(fileExt1, ext2);

            if (!File.Exists(matchingFileExt2))
            {
                Console.WriteLine($"'{Path.GetFileName(fileExt1)}' has no matching .{ext2} file in '{fileExt1}'");
            }
        }

        foreach (var fileExt2 in filesExt2)
        {
            if (fileExt2.EndsWith("_ko.srt") || fileExt2.EndsWith("_vi.srt")) continue;
            var matchingFileExt1 = Path.ChangeExtension(fileExt2, ext1);

            if (!File.Exists(matchingFileExt1))
            {
                Console.WriteLine($"'{Path.GetFileName(fileExt2)}' has no matching .{ext1} file in '{fileExt2}'");
            }
        }
    }
    
    
 
    /// <summary>
    /// 중복되는 파일을 삭제, (수정시간, 파일 사이즈로 유추함.)
    /// </summary>
    /// <param name="directoryPath"></param>
    public static void RemoveDuplicateFiles(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            var fileInfoDictionary = new Dictionary<string, FileInfo>(); 
            var allFilesInDirectory = Directory.GetFiles(directoryPath); 

            foreach (var file in allFilesInDirectory)
            {
                FileInfo fileInfo = new FileInfo(file);
                string fileKey = fileInfo.Length + "_" + fileInfo.LastWriteTime;

                if (fileInfoDictionary.ContainsKey(fileKey)) 
                {
                    try
                    {
                        File.Delete(file);
                        Console.WriteLine($"Duplicate file {file} has been deleted.");
                    }
                    catch (System.IO.IOException)
                    {
                        Console.WriteLine($"Cannot delete file {file} as it is being used by another process.");
                    }
                }
                else
                {
                    fileInfoDictionary[fileKey] = fileInfo;
                }
            }
        }
        else
        {
            Console.WriteLine("The provided directory path does not exist.");
        }
    }
}