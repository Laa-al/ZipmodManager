using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Zmm.Zipmods;

public class ZipmodLink : AggregateRoot<Guid>, IZipmodLink
{
    protected ZipmodLink()
    {
    }

    public ZipmodLink(Guid id, Uri downloadUri) : base(id)
    {
        DownloadUri = downloadUri;
    }

    [MaxLength(256)]
    public Uri DownloadUri { get; set; } = null!;

    [MaxLength(128)]
    public string? Name { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }

    [MaxLength(64)]
    public string? Size { get; set; }

    public DateTime UploadTime { get; set; }

    public bool IsInvalid { get; set; }

    public Guid? InfoId { get; set; }
    public virtual ZipmodInfo? Info { get; set; }
}