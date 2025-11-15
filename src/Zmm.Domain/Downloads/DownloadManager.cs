using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Zmm.Downloads;

public class DownloadManager(
    IGuidGenerator generator,
    IServiceProvider serviceProvider) : ISingletonDependency
{
    private List<DownloadTask> DownloadHistories { get; } = [];

    public DownloadTask? GetOrDefault(Guid id)
    {
        return DownloadHistories.FirstOrDefault(u => u.Id == id);
    }

    public IQueryable<DownloadTask> GetQueryable()
    {
        return DownloadHistories.AsQueryable();
    }

    public List<Task> CurrentTasks { get; } = [];

    public async Task<DownloadTask> CreateAsync(Uri link, string folderPath, string fileName, string label)
    {
        var task = DownloadHistories.FirstOrDefault(u =>
            u.Uri == link);

        if (task is null)
        {
            task = new DownloadTask
            {
                Id = generator.Create(),
                Label = label,
                FileName = fileName,
                FolderPath = folderPath,
                Uri = link,
                Status = DownloadStatus.Wait
            };
            DownloadHistories.Add(task);
        }
        else
        {
            task.FileName = fileName;
            task.FolderPath = folderPath;
            task.Label = label;
        }

        await StartAsync(task);
        return task;
    }

    public async Task StartAsync(DownloadTask task)
    {
        var manager = serviceProvider.GetRequiredService<IBackgroundJobManager>();
        await manager.EnqueueAsync(new DownloadArgs()
        {
            Id = task.Id
        });
    }
}