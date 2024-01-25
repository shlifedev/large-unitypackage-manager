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
            if (mode == "find-empty")
            {
                if (string.IsNullOrEmpty(dest))
                {
                    throw new Exception("destination not defined. use -d or --dest arg");
                }
                var finded = Utils.FindEmptyMp4AndSrtFiles(dest);
                foreach (var find in finded)
                {
                    Console.WriteLine(find);
                } 
            }
            else if (mode == "drive")
            {
                if (folders != null && folders.Any())
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
    }
}