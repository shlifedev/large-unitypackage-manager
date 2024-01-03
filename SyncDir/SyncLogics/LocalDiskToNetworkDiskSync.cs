namespace SyncDir;

public class LocalDiskToNetworkDiskSync : IDirectorySync
{

 
    /// <summary>
    /// 추후 구현, 이걸 구현하려면 IDirectorySync가 파라미터를 받는 함수를 가져서는 안된다. 그리고 그게 맞다.
    /// </summary>
    private List<IValidator> Validators = new List<IValidator>();
    public LocalDiskToNetworkDiskSync(int threadCount = 4)
    {
        semaphore = new SemaphoreSlim(threadCount); 
    }

    private SemaphoreSlim semaphore;
    /// <summary>
    /// 경로를 싱크합니다. LocalDiskToNetworkDiskSync는 source, dest의 두 하위 다이렉토리 개수, 이름이 동일해야합니다.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param> 
    public async Task SyncAsync(string source, string dest)
    { 
        if (Utils.IsNetworkDrive(source)) throw new Exception($"{source}는 네트워크 드라이브입니다.");
        if (!Utils.IsNetworkDrive(dest)) throw new Exception($"{dest}는 네트워크 드라이브가 아닙니다.");
        
        
        if (new SubPathValidator(source, dest).Validate())
        {


            if (!Utils.IsValidDirectoryPath(source) || !Utils.IsValidDirectoryPath(dest)) 
                throw new ArgumentException($"source 혹은 dest 경로가 존재하지 않습니다."); 

            string pathA = source;
            string pathB = dest;

            string[] filesA = Directory.GetFiles(pathA, "*", SearchOption.AllDirectories);
            string[] filesB = Directory.GetFiles(pathB, "*", SearchOption.AllDirectories);

            var tasks = new List<Task>();

            foreach (var fileA in filesA)
            {
                var relativePath = fileA.Replace(pathA, "");
                var targetPath = Path.Combine(pathB, relativePath.TrimStart(Path.DirectorySeparatorChar));

                if (!filesB.Contains(relativePath))
                {
                    var targetDirectory = Path.GetDirectoryName(targetPath);
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    tasks.Add(IO.CopyFileWithStreamAsync(fileA, targetPath));
                }
            }

            while (tasks.Any())
            {
                // tasks 리스트에서 태스크를 하나씩 빼서 진행하며, 별도 작업이 완료될 때마다 제거합니다.
                Task finished = await Task.WhenAny(tasks);
                tasks.Remove(finished);
            }
        }
        else
        {
            throw new Exception("SubPath가 일치하지 않음");
        }
    }
}