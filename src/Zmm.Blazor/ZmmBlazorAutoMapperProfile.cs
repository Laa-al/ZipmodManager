using AutoMapper;
using Zmm.Zipmods;

namespace Zmm;

public class ZmmBlazorAutoMapperProfile : Profile
{
    public ZmmBlazorAutoMapperProfile()
    {
        CreateMap<ZipmodFileRequestInput, ZipmodFileRequestInput>();
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
    }
}