using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Zmm.Downloads;

namespace Zmm.Zipmods;

public class ModDownloadAppService(
    IRepository<ZipmodLink, Guid> linkRepository,
    DownloadManager downloadManager
) : ZmmAppService, IModDownloadAppService
{
    public async Task<ModDownloadDto> GetAsync(Guid id)
    {
        var task = downloadManager.GetOrDefault(id);
        var res = new ModDownloadDto();
        await FillInfoWithTaskAsync(res, task);
        return res;
    }

    public async Task<PagedResultDto<ModDownloadDto>> GetListAsync(ModDownloadRequestInput input)
    {
        var query = downloadManager.GetQueryable();


        query = query
                .WhereIf(!input.Path.IsNullOrEmpty(), u =>
                    u.FolderPath.Contains(input.Path!) || u.FileName.Contains(input.Path!))
                .WhereIf(input.Status is not null, u => u.Status == input.Status)
            ;

        var totalCount = query.Count();
        
        if (!input.Sorting.IsNullOrEmpty())
        {
            query = query.OrderBy(input.Sorting);
        }

        query = query
            .PageBy(input.SkipCount, input.MaxResultCount);

        var result = new List<ModDownloadDto>();
        foreach (var task in query)
        {
            var mod = new ModDownloadDto();
            await FillInfoWithTaskAsync(mod, task);
            result.Add(mod);
        }

        return new PagedResultDto<ModDownloadDto>
        {
            Items = result,
            TotalCount = totalCount
        };
    }

    protected async Task FillInfoWithTaskAsync(ModDownloadDto dto, DownloadTask? task)
    {
        if (task is null) return;

        var link = await linkRepository.FindAsync(u => u.DownloadUri == task.Uri);

        dto.Path = Path.Combine(task.FolderPath, task.FileName);
        dto.DownloadUrl = task.Uri;
        dto.Status = task.Status;
        dto.DownloadId = task.Id;
        dto.DownloadSize = task.CurrentSize;
        dto.TotalSize = (int?)link?.Info?.FileSize ?? 0;
    }

    public Task<ModDownloadSummaryDto> GetSummaryAsync()
    {
        var query = downloadManager.GetQueryable();
        int totalCount = query.Count();
        int countInQueue = query.Count(u => u.Status == DownloadStatus.Downloaded);
        return Task.FromResult(new ModDownloadSummaryDto
        {
            TotalCount = totalCount,
            CountInQueue = countInQueue
        });
    }

    public async Task RedownloadModAsync(Guid itemDownloadId)
    {
        var task = downloadManager.GetOrDefault(itemDownloadId);
        if (task is not null)
        {
            task.Status = DownloadStatus.Wait;
            await downloadManager.StartAsync(task);
        }
    }
}