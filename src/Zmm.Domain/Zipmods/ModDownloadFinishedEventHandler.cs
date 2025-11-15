using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;
using Zmm.Downloads;

namespace Zmm.Zipmods;

public class ModDownloadFinishedEventHandler(
    IRepository<ZipmodLink, Guid> linkRepository,
    DownloadManager downloadManager,
    ZipmodManager zipmodManager)
    : ILocalEventHandler<DownloadFinishedEto>, ITransientDependency
{
    public async Task HandleEventAsync(DownloadFinishedEto eventData)
    {
        var task = downloadManager.GetOrDefault(eventData.Id);
        if (task is not { Label: "zipmod" }) return;

        if (eventData.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
        {
            await DeleteInvalidLinkAsync(task);
            return;
        }

        try
        {
            var link = await linkRepository.FindAsync(u => u.DownloadUri == task.Uri);
            var file = await zipmodManager.LoadFileFromPathAsync(
                Path.Combine(task.FolderPath, task.FileName));
            if (link is not null)
            {
                link.InfoId = file.InfoId;
                await linkRepository.UpdateAsync(link);
            }

            await zipmodManager.MoveFileToPathAsync(file.Id, task.FolderPath);
        }
        catch (Exception)
        {
            var link = await linkRepository.FindAsync(u => u.DownloadUri == task.Uri);
            if (link is not null)
            {
                link.IsInvalid = true;
                await linkRepository.UpdateAsync(link);
            }

            throw;
        }
    }

    [UnitOfWork]
    protected virtual async Task DeleteInvalidLinkAsync(DownloadTask task)
    {
        var link = await linkRepository.FindAsync(u => u.DownloadUri == task.Uri);

        if (link is null) return;

        await zipmodManager.DeleteLinkAsync(link);
    }
}