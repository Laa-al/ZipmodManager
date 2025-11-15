using Zmm.Localization;
using Volo.Abp.Application.Services;

namespace Zmm;

public abstract class ZmmAppService : ApplicationService
{
    protected ZmmAppService()
    {
        LocalizationResource = typeof(ZmmResource);
        ObjectMapperContext = typeof(ZmmApplicationModule);
    }
}
