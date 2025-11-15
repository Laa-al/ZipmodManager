using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Zmm;

[DependsOn(
    typeof(ZmmApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsModule),
    typeof(AbpAutoMapperModule)
    )]
public class ZmmBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<ZmmBlazorModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<ZmmBlazorAutoMapperProfile>(validate: true);
        });
    }
}
