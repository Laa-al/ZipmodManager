using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Zmm.Zipmods;

public class ZipmodFileDto : EntityDto<Guid>, IZipmodFile
{
    [MaxLength(256)]
    public string Path { get; set; } = string.Empty;

    public Guid InfoId { get; set; }
    public ZipmodInfoDto Info { get; set; } = null!;
}