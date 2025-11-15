using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Zmm.Localization;

namespace Zmm.Zipmods;

public class ZipmodLinkAppService : CrudAppService<ZipmodLink, ZipmodLinkDto, Guid, ZipmodLinkRequestInput>, IZipmodLinkAppService
{
    private readonly IBackgroundJobManager _jobs;
    private readonly ZipmodManager _zipmodManager;
    private readonly ZmmOptions _options;

    public ZipmodLinkAppService(
        IRepository<ZipmodLink, Guid> repository,
        IBackgroundJobManager jobs,
        ZipmodManager zipmodManager,
        IOptions<ZmmOptions> options
    ) : base(repository)
    {
        _jobs = jobs;
        _zipmodManager = zipmodManager;
        _options = options.Value;
        LocalizationResource = typeof(ZmmResource);
        ObjectMapperContext = typeof(ZmmApplicationModule);
    }

    protected override async Task<IQueryable<ZipmodLink>> CreateFilteredQueryAsync(ZipmodLinkRequestInput input)
    {
        var query = await ReadOnlyRepository.WithDetailsAsync();

        query = query
                .WhereIf(!input.Name.IsNullOrEmpty(), u => u.Name!.Contains(input.Name!))
                .WhereIf(!input.Description.IsNullOrEmpty(), u => u.Description!.Contains(input.Description!))
                .WhereIf(!input.Size.IsNullOrEmpty(), u => u.Size!.Contains(input.Size!))
                .WhereIf(input.UploadTimeStrat is not null, u => u.UploadTime >= input.UploadTimeStrat)
                .WhereIf(input.UploadTimeEnd is not null, u => u.UploadTime <= input.UploadTimeEnd)
                .WhereIf(input.IsInvalid is not null, u => u.IsInvalid == input.IsInvalid)
                .WhereIf(!input.Identifier.IsNullOrEmpty(), u => u.Info!.Identifier.Contains(input.Identifier!))
                .WhereIf(!input.Version.IsNullOrEmpty(), u => u.Info!.Version!.Contains(input.Version!))
                .WhereIf(!input.Author.IsNullOrEmpty(), u => u.Info!.Author!.Contains(input.Author!))
                .WhereIf(!input.Game.IsNullOrEmpty(), u => u.Info!.Game!.Contains(input.Game!))
                .WhereIf(input.IsCharaMod is not null, u => u.Info!.IsCharaMod == input.IsCharaMod)
                .WhereIf(input.IsStudioMod is not null, u => u.Info!.IsStudioMod == input.IsStudioMod)
                .WhereIf(input.IsMapMod is not null, u => u.Info!.IsMapMod == input.IsMapMod)
                .WhereIf(input.UpdateTimeStart is not null, u => u.Info!.UpdateTime >= input.UpdateTimeStart)
                .WhereIf(input.UpdateTimeEnd is not null, u => u.Info!.UpdateTime <= input.UpdateTimeEnd)
            ;

        return query;
    }

    [UnitOfWork]
    public async Task DownloadModsAsync(ZipmodLinkRequestInput input, string path)
    {
        var args = new ModDownloadArgs
        {
            Path = path
        };
        await _jobs.EnqueueAsync(ObjectMapper.Map(input, args));
    }

    public async Task DownloadModAsync(Guid id)
    {
        await _zipmodManager.DownloadModAsync(id, _options.DownloadPath, false);
    }


    [UnitOfWork]
    public async Task LoadRemoteModsAsync(string remoteUrl)
    {
        await _jobs.EnqueueAsync(
            new SideLoaderModScrapeArgs
            {
                StartUri = new Uri(remoteUrl)
            }
        );
    }

    public virtual async Task ExportModsToJsonAsync(string path)
    {
        await _jobs.EnqueueAsync(new LinkToJsonHandleArgs
        {
            Path = path,
            IsExport = true
        });
    }

    [UnitOfWork]
    public virtual async Task ImportModsFromJsonAsync(string path)
    {
        await _jobs.EnqueueAsync(new LinkToJsonHandleArgs
        {
            Path = path,
            IsExport = false
        });
    }
}