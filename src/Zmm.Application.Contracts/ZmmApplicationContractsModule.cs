using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Zmm;

[DependsOn(
    typeof(ZmmDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
    )]
public class ZmmApplicationContractsModule : AbpModule
{

}
