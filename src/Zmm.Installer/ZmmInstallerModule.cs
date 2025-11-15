using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Zmm;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class ZmmInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ZmmInstallerModule>();
        });
    }
}
