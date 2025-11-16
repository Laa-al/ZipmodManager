using System;

namespace Zmm.Downloads;

public interface IDownloadTask
{
    string Label { get; }
    string FileName { get; }
    string FolderPath { get; }
    Uri Uri { get; }
    int CurrentSize { get; }
    DownloadStatus Status { get; }
}