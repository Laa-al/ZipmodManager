using System;
using Volo.Abp.Application.Dtos;

namespace Zmm.Zipmods;

public class ZipmodInfoRequestInput:PagedAndSortedResultRequestDto
{
    public string? Identifier { get; set; } 
    public string? Version { get; set; }
    public string? Author { get; set; }
    public string? Game { get; set; }
    public bool? IsCharaMod { get; set; }
    public bool? IsStudioMod { get; set; }
    public bool? IsMapMod { get; set; }
    public DateTime? UpdateTimeStart { get; set; }
    public DateTime? UpdateTimeEnd { get; set; }
}