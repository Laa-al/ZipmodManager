using Volo.Abp.BackgroundJobs;

namespace Zmm.Zipmods;

[BackgroundJobName("load-local-mods")]
public class LoadLocalModsArgs
{
    public required string Path { get; set; }
}