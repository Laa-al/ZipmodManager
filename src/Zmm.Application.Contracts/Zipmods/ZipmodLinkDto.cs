using System;
using Volo.Abp.Application.Dtos;

namespace Zmm.Zipmods;

public class ZipmodLinkDto : EntityDto<Guid>, IZipmodLink
{
    public Guid? InfoId { get; set; }
    public Uri DownloadUri { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Size { get; set; }

    public DateTime UploadTime { get; set; }
    public long LinkSize { get; set; }

    public bool IsInvalid { get; set; }

    public ZipmodInfoDto? Info { get; set; }
}