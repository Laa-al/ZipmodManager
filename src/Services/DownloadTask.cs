using System.IO;
using System.Net.Http;
using System.Security.Authentication;

namespace Laaal.Services;

public class DownloadTask
{
    private const int DownloadSize = 1024;
    public string? FileName { get; set; }
    public string? FolderPath { get; set; }
    public string? Uri { get; set; }
    public bool IsStarted { get; private set; }
    public bool IsFinished { get; private set; }

    public bool IsStoped { get; set; }
    public bool IsFailed { get; set; }

    public Func<string, Task> FinishedAction { get; set; } = (str) => Task.CompletedTask;

    public async Task DownloadAsync()
    {
        if (IsStarted) return;

        if (FileName is null || FolderPath is null) return;

        var downloadPath = Path.Combine(FolderPath, FileName + ".download");

        try
        {
            IsStarted = true;
            IsFinished = false;
            IsFailed = false;
            
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            var url = Uri!;
            //获取到文件总大小 通过head请求
            using var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            clientHandler.SslProtocols = SslProtocols.None;

            using var client = new HttpClient(clientHandler);
            var response = await client.GetStreamAsync(url);


            await using (var fileStream = new FileStream(downloadPath, FileMode.Create,
                             FileAccess.Write, FileShare.Read))
            {
                await response.CopyToAsync(fileStream);
            }

            var path = Path.Combine(FolderPath, FileName);

            File.Move(downloadPath, path, true);

            IsFinished = true;
            await FinishedAction(path);
        }
        catch (Exception)
        {
            IsFailed = true;
            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }

            throw;
        }
        finally
        {
            IsStarted = false;
        }
    }
}