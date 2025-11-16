using System;
using Volo.Abp.Application.Dtos;

namespace Zmm.Zipmods;

public abstract class ZipmodInfoRequestInput : PagedAndSortedResultRequestDto
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
    public int? MinSize { get; set; }
    public int? MaxSize { get; set; }
    public string? Content { get; set; }

    public virtual void Clear()
    {
        Identifier = null;
        Content = null;
        Version = null;
        Author = null;
        Game = null;
        IsCharaMod = null;
        IsStudioMod = null;
        IsMapMod = null;
        UpdateTimeStart = null;
        UpdateTimeEnd = null;
        MinSize = null;
        MaxSize = null;
    }
}