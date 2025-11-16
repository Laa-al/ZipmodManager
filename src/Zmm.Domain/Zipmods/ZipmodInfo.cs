using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Volo.Abp.Domain.Entities;

namespace Zmm.Zipmods;

public class ZipmodInfo : AggregateRoot<Guid>, IZipmodInfo
{
    protected ZipmodInfo()
    {
    }

    public ZipmodInfo(Guid id) : base(id)
    {
        Files = [];
        Links = [];
    }

    [MaxLength(128)] public string Identifier { get; set; } = string.Empty;

    [MaxLength(64)] public string? Version { get; set; }

    [MaxLength(64)] public string? Author { get; set; }

    [MaxLength(64)] public string? Game { get; set; }

    public bool IsCharaMod { get; set; }
    public bool IsStudioMod { get; set; }
    public bool IsMapMod { get; set; }
    public long FileSize { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime UpdateTime { get; set; }
    public virtual List<ZipmodFile> Files { get; set; } = null!;
    public virtual List<ZipmodLink> Links { get; set; } = null!;

    public int CompareTo(ZipmodInfo info)
    {
        if (info == this) return 0;
        if (info.Version == Version) return 0;
        if (info.Version is null) return 1;
        if (Version is null) return -1;
        if (!TryParseVersion(info.Version, out var rv)) return 1;
        if (!TryParseVersion(Version, out var lv)) return 0;
        if (lv == rv) return 0;
        return lv < rv ? -1 : 1;
    }

    private static bool TryParseVersion(string versionStr,
        [NotNullWhen(returnValue: true)] out Version? version)
    {
        if (versionStr.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            versionStr = versionStr[1..];
        return System.Version.TryParse(versionStr, out version);
    }

    public string GetRelativePath()
    {
        var directory = FitPropertyToPath(Author);
        if (string.IsNullOrWhiteSpace(directory)) directory = "unknown";
        var filename = FitPropertyToPath(Identifier);

        return Path.Combine(directory, $"{filename}.zipmod");
    }

    public string GetRelativePathWithVersion()
    {
        var directory = FitPropertyToPath(Author);
        if (string.IsNullOrWhiteSpace(directory)) directory = "unknown";
        var filename = FitPropertyToPath(Identifier);
        var version = FitPropertyToPath(Version);

        return Path.Combine(directory, $"{filename}.{version}.zipmod");
    }

    protected static string? FitPropertyToPath(string? property)
    {
        if (property is null)
        {
            return null;
        }

        var chars = property.ToList();

        for (var i = 0; i < chars.Count; i++)
        {
            var c = chars[i];
            if (c is '\\' or '.'
                or '/' or ':' or '*' or '?'
                or '"' or '<' or '>' or '|')
            {
                chars[i] = '_';
            }
        }

        RemoveBetween('[', ']');
        RemoveBetween('(', ')');

        return string.Join(null, chars).Trim();

        void RemoveBetween(char lft, char rht)
        {
            var start = chars.IndexOf(lft);
            var end = chars.IndexOf(rht);
            if (start >= 0 && end >= 0 && start < end)
            {
                chars.RemoveRange(start, end - start + 1);
            }
        }
    }
}