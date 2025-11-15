using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Zmm.Zipmods;

public interface IZipmodFileAppService : ICrudAppService<ZipmodFileDto, Guid, ZipmodFileRequestInput>
{
    Task LoadLocalModsAsync(string folderPath);
    Task MoveLocalModsAsync(ZipmodFileRequestInput input, string folderPath);
    Task DeleteModAsync(Guid id);
}