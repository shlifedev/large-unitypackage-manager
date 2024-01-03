using System;
using System.IO;
using System.Collections.Generic;
using SyncDir;

class Program
{
    private static SemaphoreSlim semaphore = new SemaphoreSlim(4);  
    
    
    static async Task Main()
    { 
        string pathA = @"F:\Sync"; // 경로 A 
        string pathB = @"Y:\Sync"; // 경로 B  
        var temp = new LocalDiskToNetworkDiskSync(4);
        await temp.SyncAsync(pathA, pathB); 
    }

 
    
 
 
 
    
}