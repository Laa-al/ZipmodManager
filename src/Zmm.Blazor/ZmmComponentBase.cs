using Volo.Abp.AspNetCore.Components;
using Zmm.Localization;

namespace Zmm;

public class ZmmComponentBase : AbpComponentBase
{
    public ZmmComponentBase()
    {
        ObjectMapperContext = typeof(ZmmBlazorModule);
        LocalizationResource = typeof(ZmmResource);
    }
}