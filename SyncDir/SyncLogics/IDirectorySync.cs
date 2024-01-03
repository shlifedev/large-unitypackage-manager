namespace SyncDir;

public interface IDirectorySync
{
    Task SyncAsync(string source, string dest);
}