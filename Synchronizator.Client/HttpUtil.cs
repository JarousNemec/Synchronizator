using System.Net.Http.Json;
using Synchronizator.Database.Tables;

namespace Synchronizator.Client;

public static class HttpUtil
{
    public static async Task<List<ApplicationUserHasFile>?> GetStateOfRemoteSynchronizedFiles(string token, string url)
    {
        List<ApplicationUserHasFile>? remoteData = null;
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("token", token);
            string address = url + "/sync/GetRemoteState";
            remoteData = await client.GetFromJsonAsync<List<ApplicationUserHasFile>>(address);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Chybová hláška {e.Message}");
        }

        return remoteData;
    }
}