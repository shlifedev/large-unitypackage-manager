using System;
using System.IO;
using System.Collections.Generic;
using SyncDir;

class Program
{  
    static async Task Main()
    { 
        string pathA = @"F:\Sync"; // 경로 A 
        string pathB = @"Y:\Sync"; // 경로 B  
        var temp = new LocalDiskToNetworkDiskSync(12); 
        await temp.SyncAsync(pathA, pathB); 
        Console.WriteLine("Program End, delete temporary"); 
        await IO.DeleteTempFilesAsync(pathB);
    }

 
    
 
 
 
 
}