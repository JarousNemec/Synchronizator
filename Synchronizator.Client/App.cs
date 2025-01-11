using System.Net.Http.Json;
using Synchronizator.Database.Tables;
using Synchronizator.Lib.Services;

namespace Synchronizator.Client;

public class App
{
    private bool _run = true;
    private const string SYNC_PATH = "./tosync";
    private HttpService _httpService = new();

    public void Run()
    {
        while (_run)
        {
            StaticDialogs.PrintMainMenu();
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                continue;
            switch (input.Trim().ToUpper())
            {
                case "LOCAL":
                    Console.Clear();
                    PrintSyncFolderContent();
                    StaticDialogs.PrintContinue();
                    break;
                case "REMOTE":
                    Console.Clear();
                    GetUrlAndTokenFromUser(out string? token, out string? url);
                    _ = PrintRemoteContent(token, url).Result;
                    StaticDialogs.PrintContinue();
                    break;
                case "SYNC":
                    Console.Clear();
                    SynchronizeFiles();
                    StaticDialogs.PrintContinue();
                    break;
                case "INFO":
                    StaticDialogs.PrintInfo();
                    break;
            }
        }
    }

    private async void SynchronizeFiles()
    {
        var localFiles = PrintSyncFolderContent();
        GetUrlAndTokenFromUser(out string? token, out string? url);
        // var token = "4c6ff02c-5a9a-4146-95cb-c2096f2b3d23";
        // var url = "http://localhost:5273";
        var remoteFiles = await PrintRemoteContent(token, url);
        
        var toUpload =
            localFiles.Where(path =>
                remoteFiles.All(remote => Path.GetFileName(path) != Path.GetFileName(remote.DiskPath))).ToList();
        var toDownload =
            remoteFiles.Where(remote =>
                localFiles.All(path => Path.GetFileName(remote.DiskPath) != Path.GetFileName(path))).ToList();

        if (toUpload.Count == 0 && toDownload.Count == 0 )
        {
            Console.WriteLine("Nebyli nalezeny zadne soubory k synchronizaci");
            return;
        }
        
        foreach (var file in toDownload)
        {
            new Thread(()=>_httpService.DownloadFile(url, file.DiskPath, token, Path.Combine(SYNC_PATH, Path.GetFileName(file.DiskPath)))).Start();
        }

        foreach (var file in toUpload)
        {
            new Thread(() => _httpService.UploadFile(url, token, file)).Start();
        }

        Console.WriteLine("Workers started...");
    }

    private string[] PrintSyncFolderContent()
    {
        string[] files = [];
        if (!Path.Exists(SYNC_PATH))
            Console.WriteLine(
                $"Synchronizační adresář {SYNC_PATH} neexistuje vytvořte ho prosím v rootu souborového systému aplikace");
        else
        {
            files = Directory.GetFiles(SYNC_PATH);
            if (files.Length == 0)
                Console.WriteLine("Adresář neobsahuje žádné soubory");
            else
            {
                Console.WriteLine("Lokální adresář obsahuje:");
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                }
            }
        }

        return files;
    }

    private async Task<List<ApplicationUserHasFile>?> PrintRemoteContent(string token, string url)
    {
        List<ApplicationUserHasFile>? remoteData;

        remoteData = await HttpUtil.GetStateOfRemoteSynchronizedFiles(token, url);

        if (remoteData == null)
        {
            Console.WriteLine("Chyba při stahování dat se serveru...");
        }
        else if (remoteData.Count == 0)
        {
            Console.WriteLine("Na serveru nejsou nahrané žádné soubory...");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Na serveru jsou nahrané tyto soubory");
            foreach (var file in remoteData)
            {
                Console.WriteLine($"{file.DiskPath} - naposledy upraveno {file.LastUpdated}");
            }
        }

        return remoteData;
    }


    private static void GetUrlAndTokenFromUser(out string? token, out string? url)
    {
        urlAgain:
        Console.WriteLine(
            "Zadejte url adresu synchronizacniho serveru (v podobnem tvaru jako http://www.idnes.cz:10000):");
        url = Console.ReadLine();
        if (string.IsNullOrEmpty(url))
            goto urlAgain;
        tokenAgain:
        Console.WriteLine("Zadejte synchronizační token, který jste získali ve webovém rozhraní:");
        token = Console.ReadLine();
        if (string.IsNullOrEmpty(token))
            goto tokenAgain;
    }
}