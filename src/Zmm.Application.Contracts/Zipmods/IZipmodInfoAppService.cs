using System;
using Volo.Abp.Application.Services;

namespace Zmm.Zipmods;

public interface IZipmodInfoAppService: ICrudAppService<ZipmodInfoDto, Guid, ZipmodInfoRequestInput>
{
    
}