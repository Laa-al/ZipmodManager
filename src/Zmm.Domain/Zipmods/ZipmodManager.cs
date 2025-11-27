using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Zmm.Downloads;

namespace Zmm.Zipmods;

public class ZipmodManager(
    IRepository<ZipmodInfo, Guid> infoRepository,
    IRepository<ZipmodFile, Guid> fileRepository,
    IRepository<ZipmodLink, Guid> linkRepository,
    IOptions<ZmmOptions> options,
    DownloadManager downloadManager,
    IGuidGenerator generator
) : DomainService
{
    public virtual async Task<ZipmodInfo> LoadInfoFromPathAsync(string path)
    {
        using var file = ZipFile.OpenRead(path);
        var entry = file.GetEntry("manifest.xml");

        if (entry is null) throw new Exception("entry does not has mainfest.xml");

        await using var stream = entry.Open();
        XmlDocument document = new();
        document.Load(stream);
        var root = document.DocumentElement;
        if (root is null) throw new Exception("mainfest.xml is empty");
        var identifier = root.GetFirstTagTextOrDefault("guid");
        if (identifier is null) throw new Exception("entry does not has guid");
        string content;
        {
            var sb = new StringBuilder();
            var csvs = file.Entries.Where(u => u.Name.EndsWith(".csv"));
            foreach (var csv in csvs)
            {
                await using var s = csv.Open();
                using var sr = new StreamReader(s);
                var str = await sr.ReadToEndAsync();
                sb.Append(str);
                sb.Append("\r\n");
            }

            content = sb.ToString();
        }
        string? game = null;

        foreach (XmlNode node in root.GetElementsByTagName("game"))
        {
            if (string.Equals(node.InnerText, "Honey Select 2", StringComparison.OrdinalIgnoreCase))
            {
                game = "Honey Select 2";
                break;
            }
        }

        FileInfo fileInfo = new(path);

        ZipmodInfo res = new(generator.Create())
        {
            Identifier = identifier,
            Author = root.GetFirstTagTextOrDefault("author"),
            Version = root.GetFirstTagTextOrDefault("version"),
            IsCharaMod = file.GetEntry("abdata/chara/") is not null,
            IsStudioMod = file.GetEntry("abdata/studio/") is not null,
            IsMapMod = file.GetEntry("abdata/map/") is not null,
            FileSize = fileInfo.Length,
            UpdateTime = fileInfo.LastWriteTime,
            Content = content,
            Game = game
        };

        return await CreateOrUpdateInfoAsync(res);
    }

    [UnitOfWork]
    public virtual async Task<ZipmodFile> LoadFileFromPathAsync(string path)
    {
        if (!Path.IsPathFullyQualified(path))
        {
            path = Path.GetFullPath(path);
        }

        var file = await fileRepository.FindAsync(u => u.Path == path);

        if (file is null)
        {
            var info = await LoadInfoFromPathAsync(path);
            file = new ZipmodFile(generator.Create(), path, info.Id);
            await fileRepository.InsertAsync(file);
        }

        return file;
    }

    [UnitOfWork]
    public virtual async Task<ZipmodInfo> CreateOrUpdateInfoAsync(ZipmodInfo info)
    {
        var exist = await infoRepository.FirstOrDefaultAsync(u =>
            u.Identifier == info.Identifier && u.Version == info.Version && u.Author == info.Author);

        info.Files.Clear();
        info.Links.Clear();
        
        if (exist is null)
            return await infoRepository.InsertAsync(info);

        exist.Identifier = info.Identifier;
        exist.Author = info.Author;
        exist.Version = info.Version;
        exist.IsCharaMod = info.IsCharaMod;
        exist.IsStudioMod = info.IsStudioMod;
        exist.IsMapMod = info.IsMapMod;
        exist.FileSize = info.FileSize;
        exist.UpdateTime = info.UpdateTime;
        exist.Content = info.Content;

        return exist;
    }

    [UnitOfWork]
    public virtual async Task CreateLinkAsync(ZipmodLink link)
    {
        if (link.Info is not null)
        {
            var info = await CreateOrUpdateInfoAsync(link.Info);
            link.Info = null;
            link.InfoId = info.Id;
        }

        var entity = await linkRepository.FindAsync(u => u.DownloadUri == link.DownloadUri);
        if (entity is null)
        {
            await linkRepository.InsertAsync(link);
        }
        else
        {
            entity.Name = link.Name;
            entity.Description = link.Description;
            entity.Size = link.Size;
            entity.UploadTime = link.UploadTime;
        }
    }

    [UnitOfWork]
    public virtual async Task MoveFileToPathAsync(Guid id, string path)
    {
        var file = await fileRepository.GetAsync(u => u.Id == id);
        path = Path.Combine(path, file.Info.GetRelativePath());
        if (!File.Exists(file.Path))
        {
            await fileRepository.DeleteAsync(file);
            return;
        }

        if (!File.Exists(path))
        {
            await MoveFileDirectlyAsync(file, path);
            return;
        }

        var targetFile = await LoadFileFromPathAsync(path);

        if (targetFile.Path == file.Path) return;

        var result = file.Info.CompareTo(targetFile.Info);

        if (result == 0)
        {
            File.Delete(file.Path);
            await fileRepository.DeleteAsync(file);
            return;
        }

        var cachePath = Path.Combine(
            options.Value.CacheModPath,
            targetFile.Info.GetRelativePathWithVersion());

        if (file.Info.CompareTo(targetFile.Info) > 0)
        {
            targetFile.MoveToPath(cachePath);
        }
        else
        {
            path = cachePath;
        }

        await MoveFileDirectlyAsync(file, path);
    }

    protected virtual async Task MoveFileDirectlyAsync(ZipmodFile file, string path)
    {
        if (file.MoveToPath(path))
        {
            await fileRepository.UpdateAsync(file);
        }
        else
        {
            await DeleteFileAsync(file);
        }
    }

    public virtual async Task DeleteFileAsync(ZipmodFile file)
    {
        await fileRepository.DeleteAsync(file);

        var info = file.Info;
        if (info.Files.All(u => u.Id == file.Id) && info.Links.Count == 0)
            await infoRepository.DeleteAsync(info);
    }

    [UnitOfWork]
    public virtual async Task DeleteLinkAsync(ZipmodLink link)
    {
        await linkRepository.DeleteAsync(link);
        var info = link.Info;

        if (info is not null &&
            info.Links.All(u => u.Id == link.Id) && info.Files.Count == 0)
            await infoRepository.DeleteAsync(info);
    }

    [UnitOfWork]
    public async Task DownloadModAsync(Guid id, string path, bool checkExist)
    {
        var link = await linkRepository.FindAsync(id);

        if (link is null) return;

        if (link.InfoId is not null && checkExist)
        {
            var info = await infoRepository.FindAsync(link.InfoId.Value);
            if (info is not null)
            {
                foreach (var file in info.Files)
                {
                    if (File.Exists(file.Path)) return;
                }
            }
            else
            {
                link.InfoId = null;
            }
        }

        await downloadManager.CreateAsync(link.DownloadUri,
            path,
            link.Id.ToString("N"),
            "zipmod"
        );
    }
}