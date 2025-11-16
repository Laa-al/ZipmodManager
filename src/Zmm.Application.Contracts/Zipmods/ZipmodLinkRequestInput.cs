using System;

namespace Zmm.Zipmods;

public class ZipmodLinkRequestInput : ZipmodInfoRequestInput
{

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Size { get; set; }

    public DateTime? UploadTimeStrat { get; set; }
    public DateTime? UploadTimeEnd { get; set; }

    public bool? IsDownload { get; set; }
    public bool? IsInvalid { get; set; }

    public override void Clear()
    {
        base.Clear();
        Name = null;
        Description = null;
        Size = null;
        UploadTimeStrat = null;
        UploadTimeEnd = null;
        IsDownload = null;
        IsInvalid = null;
    }
}