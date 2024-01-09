using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
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
            string mode = null;
            string dest = null;
            string sourceOrFolderId = null;
            string[] folders = null;
            if (args.Length != 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-m":
                        case "--mode":
                            if (i + 1 < args.Length)
                            {
                                mode = args[++i];
                            }

                            break;
                        case "-d":
                        case "--destination":
                            if (i + 1 < args.Length)
                            {
                                dest = args[++i];
                            }

                            break;
                        case "-s":
                        case "--source":
                            if (i + 1 < args.Length)
                            {
                                sourceOrFolderId = args[++i];
                                if (sourceOrFolderId.Contains(","))
                                {
                                    var splits = sourceOrFolderId.Split(",");
                                    splits[0] = splits[0].Replace("(", null);
                                    splits[^1] = splits[^1].Replace(")", null);
                                    folders = splits;
                                }
                            }

                            break;
                    }
                }


                if (sourceOrFolderId == null || dest == null || mode == null)
                {
                    throw new Exception("args is null!!!");
                }

                if (mode == "drive")
                {
                    if (folders != null && folders.Length != 0)
                    {
                        foreach (var folder in folders)
                        {
                            var google = new GoogleDriveToLocalSync(folder, dest);
                            await google.SyncAsync();
                        }

                    }
                    else
                    {
                        var google = new GoogleDriveToLocalSync(sourceOrFolderId, dest);
                        await google.SyncAsync();
                    }

                }
                else if (mode == "sync")
                {
                    var temp = new LocalDiskToNetworkDiskSync(sourceOrFolderId, dest, 4);
                    await temp.SyncAsync();
                    await IO.DeleteTempFilesAsync(dest);
                }
            }
            else
            {
                string pathA = @"Z:\"; // 경로 A 
                string pathB = @"H:\먀옹님의 공유 허브"; // 경로 B  
                var temp = new LocalDiskToNetworkDiskSync(pathA, pathB, 4);
                await temp.SyncAsync();
                await IO.DeleteTempFilesAsync(pathB);
                
            }

            // {
            //     var google = new GoogleDriveToLocalSync("1e1_KFDIUVPVwMXEJczAaXHR9YS3bq83f", @"Z:\home\Sync\Udemy Hub");
            //     await google.SyncAsync();
            // }
            {
                // var google = new GoogleDriveToLocalSync("1u8sHxYYVJI7cqC7CmVIPwqMDcmVydJ7J", @"Z:\home\Sync\Udemy Hub");
                // await google.SyncAsync(); 
            }
       
            // //
 
            // //
            // // Console.WriteLine("Program End, delete temporary");
 
            // await IO.ConvertAllFilesEncoding(pathB, "html", Encoding.UTF8, Encoding.Unicode);
            //
        }
#endif
    }
}