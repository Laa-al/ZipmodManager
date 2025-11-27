using System;

namespace Zmm.Zipmods;

public interface IZipmodInfo
{
     string Identifier { get; set; }
     string? Version { get; set; }
     string? Author { get; set; }
     string? Game { get; set; }
     string Content { get; set; }
     bool IsCharaMod { get; set; }
     bool IsStudioMod { get; set; }
     bool IsMapMod { get; set; }
     long FileSize { get; set; }
     DateTime UpdateTime { get; set; }
}