using System.Text.RegularExpressions;

namespace SyncDir;

public class FileNameComparer : IComparer<FileInfo>
{
    public int Compare(FileInfo x, FileInfo y)
    {
        return string.Compare(x.Name, y.Name);
    }
}


public class LocalDiskToNetworkDiskSync : IDirectorySync
{
    private readonly string source;
    private readonly string dest;
    private int notExistCount;
    
    

    /// <summary>
    /// 추후 구현, 이걸 구현하려면 IDirectorySync가 파라미터를 받는 함수를 가져서는 안된다. 그리고 그게 맞다.
    /// </summary>
    private List<IValidator> Validators = new List<IValidator>();

    public LocalDiskToNetworkDiskSync(string source, string dest, int threadCount = 4)
    {
        this.source = source;
        this.dest = dest;
        semaphore = new SemaphoreSlim(threadCount);
    }

    private SemaphoreSlim semaphore;

    

    /// <summary>
    /// 경로를 싱크합니다. LocalDiskToNetworkDiskSync는 source, dest의 두 하위 다이렉토리 개수, 이름이 동일해야합니다.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param> 
    public async Task SyncAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Start File Sync Local => Remote");
        Console.ResetColor();
        
        if (Utils.IsNetworkDrive(source)) 
            throw new Exception($"{source}는 네트워크 드라이브입니다.");
        if (!Utils.IsNetworkDrive(dest))
            throw new Exception($"{dest}는 네트워크 드라이브가 아닙니다.");

        bool allSycned = true;
        if (new SubPathValidator(source, dest).Validate())
        {
            if (!Utils.IsValidDirectoryPath(source) || !Utils.IsValidDirectoryPath(dest))
                throw new ArgumentException($"source 혹은 dest 경로가 존재하지 않습니다.");

            string localPath = source;
            string remotePath = dest; 
           
            // 로컬, 리모트 파일 
            string[] localFiles = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories);
            string[] remoteFiles = Directory.GetFiles(remotePath, "*", SearchOption.AllDirectories);
           
             
            // 원격 파일들.
            var remoteFileInfos = remoteFiles.Select(x => new FileInfo(x));
            
            // 리모트 파일의 인포데이터, relativePath를 키로받음.
            var remoteFileMap = remoteFileInfos.ToDictionary(x => x.FullName.Substring(dest.Length));
        
            
            // 리모트 파일의 해시맵 (relativePath)
            var remoteFileHashMap = new HashSet<string>(remoteFiles.Select(path => path.Substring(dest.Length))); 
          
            
            // 업로드 테스크 리스트 생성
            var tasks = new List<Task>();
            foreach (var fileA in localFiles)
            {
                var relativePath = fileA.Replace(localPath, "");
                var targetPath = Path.Combine(remotePath, relativePath.TrimStart(Path.DirectorySeparatorChar));

                // 파일이 없거나, 파일 사이즈가 다르면 로컬에 있는걸 리모트에 올리도록 함.
                remoteFileMap.TryGetValue(relativePath, out var findedRemotePath); 
                if (!remoteFileHashMap.Contains(relativePath) ||
                    (findedRemotePath != null && findedRemotePath.Length != new FileInfo(fileA).Length))
                { 
                    var targetDirectory = Path.GetDirectoryName(targetPath);
                    if (!Directory.Exists(targetDirectory)) 
                        Directory.CreateDirectory(targetDirectory); 
                    
                    // 단 한번이라도 테스크가 등록되어야 하는경우 파일 일치 플래그 해제
                    allSycned = false;
 
                    await semaphore.WaitAsync();
                    tasks.Add(
                        Task.Run(async () =>
                        {
                            try
                            {
                                await IO.CopyFileWithStreamAsync(fileA, targetPath); 
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                }
            } 
            await Task.WhenAll(tasks);

            if (allSycned)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("이미 모든 파일이 일치합니다.");
                Console.ResetColor();
            }
        } 
    }
}