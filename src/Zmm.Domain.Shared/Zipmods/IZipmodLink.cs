using System;

namespace Zmm.Zipmods;

public interface IZipmodLink
{
     Uri DownloadUri { get; set; }

     string? Name { get; set; }

     string? Description { get; set; }

     string? Size { get; set; }

     DateTime UploadTime { get; set; }

     bool IsInvalid { get; set; }
}