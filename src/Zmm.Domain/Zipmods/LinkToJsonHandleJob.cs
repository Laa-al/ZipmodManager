using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

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
            foreach (var zipmodLink in list!)
            {
                if (zipmodLink.Info is { } i)
                {
                    var exist = await infoRepository.FirstOrDefaultAsync(u =>
                        u.Identifier == i.Identifier && u.Version == i.Version && u.Author == i.Author);
                    if (exist is not null)
                    {
                        zipmodLink.Info = exist;
                    }
                    else
                    {
                        await infoRepository.InsertAsync(i);
                    }
                }

                await repository.InsertAsync(zipmodLink, true);
            }
        }
    }
}