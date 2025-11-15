using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;

namespace Zmm.Downloads;

public class DownloadJob(
    DownloadManager manager,
    IOptions<ZmmOptions> options,
    ILogger<DownloadJob> logger,
    ILocalEventBus eventBus)
    : AsyncBackgroundJob<DownloadArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(DownloadArgs args)
    {
        var task = manager.GetOrDefault(args.Id);
        if (task is null) return;

        if (task.Status == DownloadStatus.Downloading)
        {
            logger.LogWarning("task is already started.");
            return;
        }

        if (task.Status == DownloadStatus.Downloaded)
        {
            logger.LogWarning("task is already downloaded.");
            return;
        }

        var h = ProcessDownloadAsync(task);
        manager.CurrentTasks.RemoveAll(u => u.IsCompleted || u.IsCanceled || u.IsFaulted);
        manager.CurrentTasks.Add(h);
        if (manager.CurrentTasks.Count >= options.Value.MaxThreadCount)
        {
            await Task.WhenAny(manager.CurrentTasks);
        }
    }

    protected async Task ProcessDownloadAsync(DownloadTask task)
    {
        var downloadPath = Path.Combine(task.FolderPath, task.FileName + ".download");
        var statusCode = HttpStatusCode.OK;
        try
        {
            task.Status = DownloadStatus.Downloading;
            if (!Directory.Exists(task.FolderPath))
            {
                Directory.CreateDirectory(task.FolderPath);
            }

            var url = task.Uri;

            using var client = new HttpClient();
            await using var stream = await client.GetStreamAsync(url);
            await using (var fileStream = new FileStream(downloadPath, FileMode.Create,
                             FileAccess.Write, FileShare.Read))
            {
                task.CurrentSize = 0;

                var progress = stream.CopyToAsync(fileStream);

                while (progress.Status is TaskStatus.Running or
                       TaskStatus.WaitingToRun or TaskStatus.WaitingForActivation)
                {
                    task.CurrentSize = (int)fileStream.Length;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                await progress;
                task.CurrentSize = (int)fileStream.Length;
            }

            var path = Path.Combine(task.FolderPath, task.FileName);

            File.Move(downloadPath, path, true);

            task.Status = DownloadStatus.Downloaded;
        }
        catch (Exception e)
        {
            task.Status = DownloadStatus.Failed;
            logger.LogException(e);
            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }

            throw;
        }

        await eventBus.PublishAsync(new DownloadFinishedEto
        {
            Id = task.Id,
            StatusCode = statusCode
        });
    }
}