using System.Security.Cryptography;
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

    /// <summary>
    /// 경로가 존재하는지 확인
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsValidDirectoryPath(string path)
    {
        return !string.IsNullOrEmpty(path) && Directory.Exists(path);
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