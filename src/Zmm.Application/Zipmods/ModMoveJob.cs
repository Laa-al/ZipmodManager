using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Zmm.Zipmods;

public class ModMoveJob(
    IRepository<ZipmodFile, Guid> repository,
    ZipmodManager zipmodManager
)
    : AsyncBackgroundJob<ModMoveArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(ModMoveArgs input)
    {
        var list = await GetIdListAsync(input);
        foreach (var id in list)
        {
            await zipmodManager.MoveFileToPathAsync(id, input.TargetPath);
        }
    }

    [UnitOfWork]
    protected virtual async Task<List<Guid>> GetIdListAsync(ModMoveArgs input)
    {
        var query = await repository.WithDetailsAsync();

        query = query.Filter(input)
            ;

        return query.Select(u => u.Id).ToList();
    }
}