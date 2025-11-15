using Volo.Abp.Modularity;

namespace Zmm;

[DependsOn(
    typeof(ZmmApplicationModule),
    typeof(ZmmDomainTestModule)
    )]
public class ZmmApplicationTestModule : AbpModule
{

}
