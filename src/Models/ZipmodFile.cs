namespace Laaal.Models;

public class ZipmodFile
{
    public ZipmodFile(string path, ZipmodInfo info)
    {
        Path = path;
        Info = info;
    }

    public string Path { get; set; }

    public string Guid => Info.Guid;
    public string? Version => Info.Version;
    public string? Author => Info.Author;
    public string? Game => Info.Game;
    public long FileSize => Info.FileSize;
    public DateTime UpdateTime => Info.UpdateTime;
    public bool IsCharaMod => Info.IsCharaMod;
    public bool IsStudioMod => Info.IsStudioMod;
    public bool IsMapMod => Info.IsMapMod;
    public ZipmodInfo Info { get; set; }

}