using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Zmm.Localization;

namespace Zmm.Zipmods;

public class ZipmodFileAppService : CrudAppService<ZipmodFile, ZipmodFileDto, Guid, ZipmodFileRequestInput>,
    IZipmodFileAppService
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

        query = query.Filter(input);

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
        var args = new ModMoveArgs()
        {
            TargetPath = folderPath
        };
        await _jobs.EnqueueAsync(ObjectMapper.Map(input, args));
    }

    public async Task DeleteModAsync(ZipmodFileRequestInput input)
    {
        var query = await ReadOnlyRepository.WithDetailsAsync();
        query = query.Filter(input);

        var list = query.ToList();  
        foreach (var mod in list)
        {
            await _manager.DeleteFileAsync(mod);
            if (File.Exists(mod.Path))
            {
                File.Delete(mod.Path);
            }

            await UnitOfWorkManager.Current!.SaveChangesAsync();
        }
    }
}