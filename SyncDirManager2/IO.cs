using System.Drawing;

namespace SyncDir;

public static class IO
{
    public class FileUploadStatus
    {
        public string FileName { get; set; }
        public long TotalBytes { get; set; }
        public long UploadedBytes { get; set; }
        public double ProgressPercentage
        {
            get { return (double)UploadedBytes / TotalBytes * 100; }
        }
    }
    private static List<FileUploadStatus> currentUploads = new List<FileUploadStatus>();
    public static Task Monitor = null;
    public static void StartUpload(string fileName, long totalBytes)
    { 
        currentUploads.Add(new FileUploadStatus
        {
            FileName = fileName,
            TotalBytes = totalBytes,
            UploadedBytes = 0
        });
    }
     
    public static void PrintCurrentUploads()
    {
        var uploads = IO.GetCurrentUploads();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"현재 {uploads.Count()} 개의 파일이 업로드 중입니다.:");
        Console.ResetColor();

        foreach (var upload in uploads)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"File: {upload.FileName}");
            Console.ResetColor();

            Console.Write(", ");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Total Bytes: {upload.TotalBytes}");
            Console.ResetColor();

            Console.Write(", ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Uploaded Bytes: {upload.UploadedBytes}");
            Console.ResetColor();

            Console.Write(", ");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Progress: {upload.ProgressPercentage}%");
            Console.ResetColor();
        }
    }
    
    public static void UpdateUpload(string fileName, long uploadedBytes)
    {
        var fileUploadStatus = currentUploads.Find(status => status.FileName == fileName);
        if (fileUploadStatus != null)
        {
            fileUploadStatus.UploadedBytes = uploadedBytes;
        }
    }
    
    public static void FinishUpload(string fileName)
    {
        var fileUploadStatus = currentUploads.Find(status => status.FileName == fileName);
        if (fileUploadStatus != null)
        {
            currentUploads.Remove(fileUploadStatus);
        }
    }
    
    public static IReadOnlyList<FileUploadStatus> GetCurrentUploads()
    {
        return currentUploads;
    }
    
    /// <summary>
    /// 파일을 스트림으로 씁니다. 
    /// </summary>
    /// <param name="sourceFile">원본 파일 위치</param>
    /// <param name="destinationFile">대상 파일 위치</param>
    /// <param name="bufferSize">버퍼 사이즈 (기본값 1, 1은 1MB입니다.)</param>
    public static async Task CopyFileWithStreamAsync(string sourceFile, string destinationFile, int bufferSize = 1)
    {
        Console.WriteLine($"{sourceFile} 을(를) {destinationFile} 으로 복사(업로드)합니다."); 
        string dirPath = Path.GetDirectoryName(destinationFile);
        string fileName = Path.GetFileNameWithoutExtension(destinationFile);
        string tempFile = Path.Combine(dirPath, $"{fileName}.{Const.TmpExtensionName}");

        if (File.Exists(tempFile)) 
            File.Delete(tempFile);  
        using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
        using (var tempStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
        { 
            var buffer = new byte[1024 * 1024 * bufferSize]; // 1MB buffer
            int bytesRead;
            long totalBytes = sourceStream.Length;
            // IO.StartUpload(sourceFile, totalBytes);
            while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await tempStream.WriteAsync(buffer, 0, bytesRead); 
                // IO.UpdateUpload(sourceFile, tempStream.Position);
                // IO.PrintCurrentUploads();
            } 
        } 
        File.Move(tempFile, destinationFile, true);
        File.Delete(tempFile); // Clean up the temp file
        // IO.FinishUpload(Path.GetFileName(sourceFile));
        Console.WriteLine($"{sourceFile} 을(를) {destinationFile} 으로 복사(업로드) 완료"); 
    }

    public async static Task DeleteTempFilesAsync(string directoryPath)
    {
        Console.WriteLine($"Run DeleteTempFilesAsync {directoryPath}");
        string[] tempFiles = Directory.GetFiles(directoryPath, $"*.{Const.TmpExtensionName}", SearchOption.AllDirectories);

        var semaphore = new SemaphoreSlim(8); // 8 concurrent tasks at most

        var tasks = tempFiles.Select(async tempFile =>
        {
            await semaphore.WaitAsync();

            try
            {
                File.Delete(tempFile);
                Console.WriteLine($"{tempFile} 파일을 성공적으로 삭제했습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 {tempFile} 삭제 시 오류 발생: {ex}");
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        await Task.WhenAll(tasks);
    }
}