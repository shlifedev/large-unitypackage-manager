using System;
using System.IO;
using System.Collections.Generic;
using SyncDir;

class Program
{
    static async Task Main(string[] args)
    {
        IDirectorySync sync = null;
#if RELEASE
{
    string source = null;
    string destination = null;

    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "-s":
            case "--source":
                if (i + 1 < args.Length)
                {
                    source = args[++i];
                }
                break;
            case "-d":
            case "--destination":
                if (i + 1 < args.Length)
                {
                    destination = args[++i];
                }
                break;
        }
    }

    if (source == null || destination == null)
    {
        Console.WriteLine("Error: You must specify both - and -");
        Thread.Sleep(1000);
        return;
    }
    else
    {
        var temp = new LocalDiskToNetworkDiskSync(12);
        await temp.SyncAsync(source, destination);

        Console.WriteLine("Program End, Wait Delete Temporary");
        await IO.DeleteTempFilesAsync(destination);
    }
}
#endif

#if DEBUG
        {
           
            string pathA = @"F:\Sync"; // 경로 A 
            string pathB = @"Z:\home\Sync"; // 경로 B  
            var temp = new LocalDiskToNetworkDiskSync(pathA, pathB, 4);
            await temp.SyncAsync();
            
            Console.WriteLine("Program End, delete temporary");
            await IO.DeleteTempFilesAsync(pathB);
        }
#endif
    }
}