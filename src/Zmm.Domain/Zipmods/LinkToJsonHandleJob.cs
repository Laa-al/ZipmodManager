using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Zmm.Zipmods;

public class LinkToJsonHandleJob(
    IRepository<ZipmodInfo, Guid> infoRepository,
    IRepository<ZipmodLink, Guid> repository)
    : AsyncBackgroundJob<LinkToJsonHandleArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(LinkToJsonHandleArgs args)
    {
        if (args.IsExport)
        {
            var query = await repository.GetQueryableAsync();
            await using var fs = new FileStream(args.Path, FileMode.Create);
            await using var sw = new StreamWriter(fs);

            var list = JsonSerializer.Serialize(query);
            await sw.WriteAsync(list);
        }
        else
        {
            await using var fs = new FileStream(args.Path, FileMode.Open);
            var list = JsonSerializer.Deserialize<List<ZipmodLink>>(fs);
            if (list is not null)
            {
                foreach (var link in list)
                {
                    await CreateLinkAsync(link);
                }
            }
        }
    }

    [UnitOfWork]
    protected virtual async Task CreateLinkAsync(ZipmodLink link)
    {
        if (link.Info is { } i)
        {
            var exist = await infoRepository.FirstOrDefaultAsync(u =>
                u.Identifier == i.Identifier && u.Version == i.Version && u.Author == i.Author);
            if (exist is not null)
            {
                link.Info = exist;
            }
        }

        await repository.InsertAsync(link);
    }
}