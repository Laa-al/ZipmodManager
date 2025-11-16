using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Zmm.Zipmods;

public class ZipmodInfoDto : EntityDto<Guid>,IZipmodInfo
{
    public string Identifier { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Author { get; set; }
    public string? Game { get; set; }
    public bool IsCharaMod { get; set; }
    public bool IsStudioMod { get; set; }
    public bool IsMapMod { get; set; }
    public long FileSize { get; set; }
    public string Content { get; set; } = "";
    public DateTime UpdateTime { get; set; }
    public List<ZipmodFileDto> Files { get; set; } = null!;
    public List<ZipmodLinkDto> Links { get; set; } = null!;
}