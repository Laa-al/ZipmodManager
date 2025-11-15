using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Zmm.Zipmods;

public interface IModDownloadAppService : IReadOnlyAppService<ModDownloadDto, Guid, ModDownloadRequestInput>
{
    Task<ModDownloadSummaryDto> GetSummaryAsync();
    Task RedownloadModAsync(Guid itemDownloadId);
}