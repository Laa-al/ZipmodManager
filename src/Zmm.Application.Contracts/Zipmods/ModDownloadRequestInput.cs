using Volo.Abp.Application.Dtos;
using Zmm.Downloads;

namespace Zmm.Zipmods;

public class ModDownloadRequestInput : PagedAndSortedResultRequestDto
{
    public string? Path { get; set; }
    public DownloadStatus? Status { get; set; }
}