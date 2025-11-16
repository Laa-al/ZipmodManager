namespace Zmm.Zipmods;

public class ModMoveArgs : ZipmodFileRequestInput
{
    public required string TargetPath { get; set; }
}