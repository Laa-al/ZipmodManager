using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Zmm;

[DependsOn(
    typeof(AbpBackgroundJobsModule),
    typeof(AbpDddDomainModule),
    typeof(ZmmDomainSharedModule)
)]
public class ZmmDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        
        Configure<ZmmOptions>(options =>
        {
            var section = configuration.GetSection("Zmm");
            section.Bind(options);
        });
    }
}
