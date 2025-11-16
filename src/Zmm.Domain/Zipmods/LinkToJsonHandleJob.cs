using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Zmm.Zipmods;

public class LinkToJsonHandleJob(
    ZipmodManager manager,
    IRepository<ZipmodLink, Guid> repository)
    : AsyncBackgroundJob<LinkToJsonHandleArgs>, ITransientDependency
{
    private static JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public override async Task ExecuteAsync(LinkToJsonHandleArgs args)
    {
        if (args.IsExport)
        {
            var query = await repository
                .WithDetailsAsync(u => u.Info!);

            await using var fs = new FileStream(args.Path, FileMode.Create);
            await using var sw = new StreamWriter(fs);

            var list = query.ToList();

            var str = JsonSerializer.Serialize(list, _options);
            await sw.WriteAsync(str);
        }
        else
        {
            await using var fs = new FileStream(args.Path, FileMode.Open);
            var list = JsonSerializer.Deserialize<List<ZipmodLink>>(fs);
            if (list is not null)
            {
                foreach (var link in list)
                {
                    await manager.CreateLinkAsync(link);
                }
            }
        }
    }
}