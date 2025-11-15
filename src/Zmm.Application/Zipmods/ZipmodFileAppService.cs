using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Zmm.Localization;

namespace Zmm.Zipmods;

public class ZipmodFileAppService : CrudAppService<ZipmodFile, ZipmodFileDto, Guid, ZipmodFileRequestInput>, IZipmodFileAppService
{
    private readonly ZipmodManager _manager;
    private readonly IBackgroundJobManager _jobs;

    public ZipmodFileAppService(
        IRepository<ZipmodFile, Guid> repository,
        ZipmodManager manager,
        IBackgroundJobManager jobs
    ) : base(repository)
    {
        _manager = manager;
        _jobs = jobs;
        LocalizationResource = typeof(ZmmResource);
        ObjectMapperContext = typeof(ZmmApplicationModule);
    }

    protected override async Task<IQueryable<ZipmodFile>> CreateFilteredQueryAsync(ZipmodFileRequestInput input)
    {
        var query = await ReadOnlyRepository.WithDetailsAsync(u => u.Info);

        query = query
                .WhereIf(!input.Path.IsNullOrEmpty(), u => u.Path.Contains(input.Path!))
                .WhereIf(!input.Identifier.IsNullOrEmpty(), u => u.Info.Identifier.Contains(input.Identifier!))
                .WhereIf(!input.Version.IsNullOrEmpty(), u => u.Info.Version!.Contains(input.Version!))
                .WhereIf(!input.Author.IsNullOrEmpty(), u => u.Info.Author!.Contains(input.Author!))
                .WhereIf(!input.Game.IsNullOrEmpty(), u => u.Info.Game!.Contains(input.Game!))
                .WhereIf(input.IsCharaMod is not null, u => u.Info.IsCharaMod == input.IsCharaMod)
                .WhereIf(input.IsStudioMod is not null, u => u.Info.IsStudioMod == input.IsStudioMod)
                .WhereIf(input.IsMapMod is not null, u => u.Info.IsMapMod == input.IsMapMod)
                .WhereIf(input.UpdateTimeStart is not null, u => u.Info.UpdateTime >= input.UpdateTimeStart)
                .WhereIf(input.UpdateTimeEnd is not null, u => u.Info.UpdateTime <= input.UpdateTimeEnd)
            ;

        return query;
    }

    public async Task LoadLocalModsAsync(string folderPath)
    {
        await _jobs.EnqueueAsync(new LoadLocalModsArgs()
        {
            Path = folderPath
        });
    }

    [UnitOfWork]
    public virtual async Task MoveLocalModsAsync(ZipmodFileRequestInput input, string folderPath)
    {
        var query = await CreateFilteredQueryAsync(input);

        foreach (var id in query.Select(u => u.Id))
        {
            await _manager.MoveFileToPathAsync(id, folderPath);
        }
    }

    public async Task DeleteModAsync(Guid id)
    {
        var file = await GetEntityByIdAsync(id);

        if (File.Exists(file.Path))
        {
            File.Delete(file.Path);
        }

        await _manager.DeleteFileAsync(file);
    }
}