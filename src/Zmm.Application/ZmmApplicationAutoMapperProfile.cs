using AutoMapper;
using Zmm.Zipmods;

namespace Zmm;

public class ZmmApplicationAutoMapperProfile : Profile
{
    public ZmmApplicationAutoMapperProfile()
    {
        CreateMap<ZipmodFile, ZipmodFileDto>().ReverseMap();
        CreateMap<ZipmodInfo, ZipmodInfoDto>().ReverseMap();
        CreateMap<ZipmodLink, ZipmodLinkDto>().ReverseMap();
        CreateMap<ZipmodLinkRequestInput, ModDownloadArgs>().ReverseMap();
        CreateMap<ZipmodFileRequestInput, ModMoveArgs>().ReverseMap();
    }
}