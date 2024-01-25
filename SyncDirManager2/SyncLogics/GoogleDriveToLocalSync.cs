using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace SyncDir;

public class GoogleDriveToLocalSync : IDirectorySync
{
    private string FolderId { get; set; }
    private static string[] Scopes = { DriveService.Scope.DriveFile, DriveService.Scope.Drive };
    private string OutDir { get; set; }

    public List<string> DownloadFailedPath = new List<string>();
    public GoogleDriveToLocalSync(string folderId, string outputDir)
    {
        this.OutDir = outputDir;
        this.FolderId = folderId; 
    }

    
    private async Task DownloadAllFiles(DriveService service, string folderId, string localPath)
    { 
        var request = service.Files.List();
        request.Q = $"'{folderId}' in parents";  
        request.Fields = "files(id, name, size, mimeType)"; // Include the 'size' field 
        var result = await request.ExecuteAsync(); 
         
        foreach (var file in result.Files)
        {
            if (file.MimeType == "application/vnd.google-apps.shortcut")
            {  
                FilesResource.GetRequest req = service.Files.Get(file.Id);
                req.Fields = "shortcutDetails/targetId";
                var shortcut = req.ExecuteAsync();
                string targetId = shortcut.Result.ShortcutDetails.TargetId;
                var newLocalPath = Path.Combine(localPath, file.Name);
                Directory.CreateDirectory(newLocalPath);  // Create the directory if it does not exist
                await DownloadAllFiles(service, targetId, newLocalPath);
         
            } 
            if (file.MimeType == "application/vnd.google-apps.folder")
            {
                // This is a folder, we need to recursively download its contents
                var newLocalPath = Path.Combine(localPath, file.Name);
                Directory.CreateDirectory(newLocalPath);  // Create the directory if it does not exist
                await DownloadAllFiles(service, file.Id, newLocalPath);
            }
            else
            {
                Console.WriteLine(file.MimeType);
                await DownloadFile(service, file, localPath); 
            }
        }
    }

    private async Task DownloadFile(DriveService service, Google.Apis.Drive.v3.Data.File file,
        string localPath)
    {
        string localFilePath = Path.Combine(localPath, file.Name);
        string tempFilePath = localFilePath + ".tmpdrive";
        
 
        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
            Console.WriteLine($"임시 파일 삭제 {tempFilePath}");
        }
 

        if (File.Exists(localFilePath) &&(new FileInfo(localFilePath).Length == file.Size))
        {  
            Console.WriteLine(new FileInfo(localFilePath).Length); 
        }
        else
        {
            Console.WriteLine($"다운로드 시작 {localFilePath}"); // Display the name of the file currently being downloaded

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                var downloadRequest = service.Files.Get(file.Id);
                await downloadRequest.DownloadAsync(stream);
            }

            try
            {
                if(File.Exists(localFilePath)){
                    File.Delete(localFilePath);
                }
                File.Move(tempFilePath, localFilePath);
            }
            catch (Exception e)
            {
                await Task.Delay(1000);
                File.Move(tempFilePath, localFilePath);
            }
            finally
            {
                if (!File.Exists(localFilePath))
                {
                    DownloadFailedPath.Add(localFilePath);
                    throw new Exception($"다운로드 실패 {localFilePath}");  
                }
                else
                { 
                    Console.WriteLine(
                        $"다운로드 완료 {localFilePath}"); // Display the name of the file that has completed downloading
                }
            }
 
        }
    }

    public async Task DownloadAllFiles()
    {
        UserCredential credential;


        string credPath = "token.json";
        
        var jsonPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "credentials.json");

        var auth = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromFile(jsonPath).Secrets,
            Scopes,
            "shlifedev",
            CancellationToken.None,
            new FileDataStore(credPath, true));
        credential = auth;

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });

        var req = service.Files.Get(FolderId);
        var parent = await req.ExecuteAsync(); 
        OutDir = Path.Combine(OutDir, parent.Name);  
        await DownloadAllFiles(service, FolderId, OutDir);
    }

    public async Task SyncAsync()
    {
        await DownloadAllFiles();
        Console.WriteLine("Download Failed List...");
        foreach (var log in DownloadFailedPath)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
        }
        Console.ResetColor();
    }
}