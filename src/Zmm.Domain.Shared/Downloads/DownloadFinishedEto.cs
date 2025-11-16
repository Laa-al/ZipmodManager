using System;
using System.Net;

namespace Zmm.Downloads;

public class DownloadFinishedEto : IDownloadTask
{
    public Guid Id { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public required string Label { get; set; }
    public required string FileName { get; set; }
    public required string FolderPath { get; set; }
    public required Uri Uri { get; set; }
    public int CurrentSize { get; set; }
    public DownloadStatus Status { get; set; }
}