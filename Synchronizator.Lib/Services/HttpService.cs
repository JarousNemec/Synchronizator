using System.Net;
using System.Text;
using Synchronizator.Lib.Models;

namespace Synchronizator.Lib.Services;

public class HttpService
{
    private Semaphore _semaphoreUpload = new Semaphore(3, 3);
    private Semaphore _semaphoreDownload = new Semaphore(3, 3);
    
    public async void UploadFile(string baseUrl, string token, string path)
    {
        var url = baseUrl + "/sync/upload";
        if (!Path.Exists(path))
        {
            Console.WriteLine($"Failed to initialize upload {path} file doesnt exists");
            return;
        }
        
        Console.WriteLine($"Waiting for upload file {path}");
        
        _semaphoreUpload.WaitOne();
        
        Console.WriteLine($"Uploading file {path} with token: {token} to {url}");
        
        var client = new HttpClient();
        try
        {
            await using var stream = File.OpenRead(path);
        
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(stream), "File", Path.GetFileName(path));
            content.Add(new StringContent(token), "Details.UserToken");
            request.Content = content;
            var res = await client.SendAsync(request);
            if(!res.IsSuccessStatusCode)
                Console.WriteLine($"Failed to upload {path} request or server error code:{res.StatusCode}");
            _semaphoreUpload.Release();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to upload file {path} with token {token} to {url} with error: {e.Message}");
        }
        
        Console.WriteLine($"Successfully uploaded file {path}");
        
        
        // https://brokul.dev/sending-files-and-additional-data-using-httpclient-in-net-core
    }

    public async void DownloadFile(string baseUrl, string targetFilename, string token, string path)
    {
        var url = baseUrl + "/sync/Download";
        if (Path.Exists(path))
        {
            Console.WriteLine($"Failed to initialize download file {targetFilename} because file already exists");
            return;
        }

        Console.WriteLine($"Waiting... for download file {targetFilename}");
        
        _semaphoreDownload.WaitOne();
        
        Console.WriteLine($"Downloading file {targetFilename} from {url} with token: {token}");
        
        
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("token", token);
            client.DefaultRequestHeaders.Add("filename", targetFilename);
            var stream = await client.GetStreamAsync(url);
            var file = File.Open(path, FileMode.OpenOrCreate);
            await stream.CopyToAsync(file);
            file.Flush();
            file.Close();
            _semaphoreDownload.Release();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to download file {targetFilename} from {url} with error: {e.Message}");
            return;
        }
        Console.WriteLine($"Saved file {targetFilename} to destination path: {path}");
        
        // https://www.tutorialspoint.com/how-to-download-a-file-from-a-url-in-chash
    }
}