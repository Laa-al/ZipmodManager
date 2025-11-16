using System;

namespace Zmm.Downloads;

public class DownloadTask:IDownloadTask
{
    public Guid Id { get; set; }
    public required string Label { get; set; }
    public required string FileName { get; set; }
    public required string FolderPath { get; set; }
    public required Uri Uri { get; set; }
    public int CurrentSize { get; set; }
    public DownloadStatus Status { get; set; }
}