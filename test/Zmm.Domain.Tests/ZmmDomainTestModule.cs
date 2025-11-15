using Volo.Abp.Modularity;
using Zmm.EntityFrameworkCore;

namespace Zmm;

[DependsOn(
    typeof(ZmmDomainModule),
    typeof(ZmmEntityFrameworkCoreTestBase)
)]
public class ZmmDomainTestModule : AbpModule
{

}
