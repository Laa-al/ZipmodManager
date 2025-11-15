using System;
using Zmm.Downloads;

namespace Zmm.Zipmods;

public class ModDownloadDto
{
    public Guid DownloadId { get; set; }
    public Uri DownloadUrl { get; set; } = null!;
    public int DownloadSize { get; set; }
    public int TotalSize { get; set; }
    public DownloadStatus Status { get; set; }
    public string Path { get; set; } = "";
}

public class ModDownloadSummaryDto
{
    public int TotalCount { get; set; }
    public int CountInQueue { get; set; }
}