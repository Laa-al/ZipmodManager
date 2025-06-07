using Microsoft.Extensions.Logging;

namespace Laaal.Services;

public class DownloadService(ILoggerFactory factory)
{
    private ILogger Logger { get; } = factory.CreateLogger<DownloadService>();
    public List<DownloadTask> AllTasks { get; } = [];


    public void AutoAddTask(DownloadTask task)
    {
        AllTasks.Add(task);
        DownloadTask(task).ConfigureAwait(false);
    }

    private async Task DownloadTask(DownloadTask task)
    {
        try
        {
            await task.DownloadAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "download failed. {Uri}", task.Uri);
        }
    }
}