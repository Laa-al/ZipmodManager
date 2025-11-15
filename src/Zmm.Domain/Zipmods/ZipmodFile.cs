using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Volo.Abp.Domain.Entities;

namespace Zmm.Zipmods;

public class ZipmodFile : AggregateRoot<Guid>, IZipmodFile
{
    protected ZipmodFile()
    {
    }

    public ZipmodFile(Guid id, string path, Guid infoId) : base(id)
    {
        Path = path;
        InfoId = infoId;
    }

    [MaxLength(256)]
    public string Path { get; set; } = string.Empty;

    public Guid InfoId { get; set; }
    public virtual ZipmodInfo Info { get; set; } = null!;

    public bool MoveToPath(string path)
    {
        if (File.Exists(Path))
        {
            File.Move(Path, path, true);
            Path = path;
            return true;
        }

        return false;
    }
}