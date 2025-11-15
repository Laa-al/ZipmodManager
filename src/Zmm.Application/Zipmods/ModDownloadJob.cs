using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Zmm.Zipmods;

public class ModDownloadJob(
    IRepository<ZipmodLink, Guid> repository,
    ZipmodManager zipmodManager
)
    : AsyncBackgroundJob<ModDownloadArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(ModDownloadArgs input)
    {
        var list = await GetIdListAsync(input);
        foreach (var id in list)
        {
            await zipmodManager.DownloadModAsync(id, input.Path, true);
        }
    }

    [UnitOfWork]
    protected virtual async Task<List<Guid>> GetIdListAsync(ModDownloadArgs input)
    {
        var query = await repository.WithDetailsAsync();

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

        query = query.Where(u => !u.IsInvalid);
        return query.Select(u => u.Id).ToList();
    }
}