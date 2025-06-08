using System.IO;
using System.IO.Compression;
using System.Xml;

namespace Laaal.Models;

public class ZipmodInfo
{
    public string Guid { get; set; } = string.Empty;
    public string? Version { get; set; } = string.Empty;
    public string? Author { get; set; } = string.Empty;
    public string? Game { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UpdateTime { get; set; }
    public bool IsCharaMod { get; set; }
    public bool IsStudioMod { get; set; }
    public bool IsMapMod { get; set; }

    public static bool operator <(ZipmodInfo lft, ZipmodInfo rht)
    {
        if (rht.Version is null) return false;
        if (lft.Version is null) return true;
        if (lft.Version == rht.Version) return false;
        System.Version.TryParse(rht.Version, out var rv);
        if (rv is null) return false;
        System.Version.TryParse(lft.Version, out var lv);
        return lv < rv;
    }

    public static bool operator >(ZipmodInfo lft, ZipmodInfo rht)
    {
        return rht < lft;
    }

    public void MapTo(ZipmodInfo res)
    {
        res.Guid = Guid;
        res.Version = Version;
        res.Author = Author;
        res.Game = Game;
        res.FileSize = FileSize;
        res.UpdateTime = UpdateTime;
        res.IsCharaMod = IsCharaMod;
        res.IsStudioMod = IsStudioMod;
        res.IsMapMod = IsMapMod;
    }

    public static ZipmodInfo Create(string path)
    {
        if (Path.GetExtension(path) is not (".zip" or ".zipmod"))
            throw new Exception($"not a zip file extension {Path.GetExtension(path)}");
        
        using var file = ZipFile.OpenRead(path);
        var entry = file.GetEntry("manifest.xml");

        if (entry is null) throw new Exception("entry does not has mainfest.xml");

        using var stream = entry.Open();
        XmlDocument document = new();
        document.Load(stream);
        var root = document.DocumentElement;
        if (root is null) throw new Exception("mainfest.xml is empty");
        var guid = root.GetFirstTagTextOrDefault("guid");
        if (guid is null) throw new Exception("entry does not has guid");

        FileInfo fileInfo = new(path);
        ZipmodInfo res = new()
        {
            Guid = guid,
            Author = root.GetFirstTagTextOrDefault("author"),
            Version = root.GetFirstTagTextOrDefault("version"),
            IsCharaMod = file.GetEntry("abdata/chara/") is not null,
            IsStudioMod = file.GetEntry("abdata/studio/") is not null,
            IsMapMod = file.GetEntry("abdata/map/") is not null,
            FileSize = fileInfo.Length,
            UpdateTime = fileInfo.LastWriteTime,
        };

        foreach (XmlNode node in root.GetElementsByTagName("game"))
        {
            if (string.Equals(node.InnerText, "Honey Select 2", StringComparison.OrdinalIgnoreCase))
            {
                res.Game = "Honey Select 2";
                break;
            }
        }

        return res;
    }
}