using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Zmm.Zipmods;

public interface IZipmodLinkAppService : ICrudAppService<ZipmodLinkDto, Guid, ZipmodLinkRequestInput>
{
    Task DownloadModsAsync(ZipmodLinkRequestInput input, string path);
    Task LoadRemoteModsAsync(string remoteUrl);
    Task ExportModsToJsonAsync(string path);
    Task ImportModsFromJsonAsync(string path);
    Task DownloadModAsync(Guid id);
}