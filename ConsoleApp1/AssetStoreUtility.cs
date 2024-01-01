using System.Text.Json;

namespace ConsoleApp1;


public class AssetStoreUtilityConfig
{
    public string APIKey { get; set; }
    public string EngineID { get; set; }
}
 
public class AssetStoreUtility
{
    public static async Task<string> SearchGoogle(string query)
    {
        string configContent = File.ReadAllText("config.json");
        AssetStoreUtilityConfig config = JsonSerializer.Deserialize<AssetStoreUtilityConfig>(configContent); 
    
        string url = $"https://www.googleapis.com/customsearch/v1?key={config.APIKey}&cx={config.EngineID}&q={Uri.EscapeDataString(query)}"; 
        using (var httpClient = new HttpClient())
        {
            try
            {
                var json = await httpClient.GetStringAsync(url);
                var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                var firstItem = root.GetProperty("items")[0];
                firstItem.TryGetProperty("link", out var link);

                if (link.ToString().StartsWith("https://assetstore.unity.com/packages"))
                {
                    return link.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
    
    
}