using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Laaal.Services;

public class DownloadService(ILoggerFactory factory)
{
    private ILogger Logger { get; } = factory.CreateLogger<DownloadService>();
    public List<DownloadTask> AllTasks { get; } = [];

    private int _taskCount;

    public ConcurrentQueue<DownloadTask> NotDownload { get; } = [];


    public void Redownload(DownloadTask task)
    {
        if (!task.IsFailed) return;

        task.IsFailed = false;
        DownloadTask(task).ConfigureAwait(false);
    }

    public void AutoAddTask(DownloadTask task)
    {
        AllTasks.Add(task);
        DownloadTask(task).ConfigureAwait(false);
    }


    private async Task DownloadTask(DownloadTask task)
    {
        if (_taskCount > 16)
        {
            NotDownload.Enqueue(task);
            return;
        }

        _taskCount++;

        try
        {
            await task.DownloadAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "download failed. {Uri}", task.Uri);
        }
        finally
        {
            _taskCount--;

            if (NotDownload.TryDequeue(out var result))
            {
                _ = DownloadTask(result).ConfigureAwait(false);
            }
        }
    }
}