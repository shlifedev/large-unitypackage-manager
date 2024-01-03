namespace SyncDir;

public static class IO
{
    /// <summary>
    /// 파일을 스트림으로 씁니다. 
    /// </summary>
    /// <param name="sourceFile">원본 파일 위치</param>
    /// <param name="destinationFile">대상 파일 위치</param>
    /// <param name="bufferSize">버퍼 사이즈 (기본값 1, 1은 1MB입니다.)</param>
    public static async Task CopyFileWithStreamAsync(string sourceFile, string destinationFile, int bufferSize = 1)
    {
        string dirPath = Path.GetDirectoryName(destinationFile);
        string fileName = Path.GetFileNameWithoutExtension(destinationFile);
        string tempFile = Path.Combine(dirPath, $"{fileName}_{DateTime.Now.Ticks}.tmp");
        using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
        using (var tempStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
        {
            var buffer = new byte[1024 * 1024 * bufferSize]; // 1MB buffer
            int bytesRead;
            long totalRead = 0;
            while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await tempStream.WriteAsync(buffer, 0, bytesRead); 
            }
        } 
        File.Move(tempFile, destinationFile, true);
        File.Delete(tempFile); // Clean up the temp file
    }
}