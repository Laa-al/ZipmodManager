using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Zmm.Zipmods;

public class LoadLocalModsJob(ZipmodManager manager)
    : AsyncBackgroundJob<LoadLocalModsArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(LoadLocalModsArgs args)
    {
        var paths = new Queue<string>();
        paths.Enqueue(args.Path);
        while (paths.Count > 0)
        {
            var path = paths.Dequeue();

            var directories = Directory.GetDirectories(path);

            foreach (var directory in directories)
            {
                paths.Enqueue(directory);
            }

            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                try
                {
                    await manager.LoadFileFromPathAsync(file);
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
            }
        }
    }
}