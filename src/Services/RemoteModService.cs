using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using Laaal.Models;
using Microsoft.Extensions.Logging;

namespace Laaal.Services;

public class RemoteModService(ModAnalizeService service, ILoggerFactory factory)
{
    private ILogger Logger { get; } = factory.CreateLogger<RemoteModService>();
    public ConcurrentDictionary<Uri, ZipmodLink> List { get; } = [];

    public Task AddLinkAsync(ZipmodLink link)
    {
        return Task.Run(() =>
        {
            if (link.Uri is null) return;

            List.AddOrUpdate(link.Uri, link, (_, s) =>
            {
                if (link.UploadTime <= s.UploadTime)
                {
                    link.Info = s.Info;
                    link.IsInvalid = s.IsInvalid;
                }
                return link;
            });
        });
    }

    public async Task LoadLinkFromJson(string path)
    {
        try
        {
            await using var fs = new FileStream(path, FileMode.Open);

            var list = JsonSerializer.Deserialize<List<ZipmodLink>>(fs);
            foreach (var zipmodLink in list!)
            {
                if (zipmodLink.Info is not null)
                    zipmodLink.Info = await service.GetOrAddInfo(zipmodLink.Info);
                await AddLinkAsync(zipmodLink);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "load failed. path: {path}", path);
        }
    }

    private DateTime _lastSaveDateTime = DateTime.Now;
    private bool _isSaving;

    public async Task SaveLinkAsJson(string path)
    {
        if (_isSaving) return;
        if (DateTime.Now - _lastSaveDateTime < TimeSpan.FromSeconds(5))
        {
            return;
        }

        _lastSaveDateTime = DateTime.Now;

        _isSaving = true;

        try
        {
            await using var fs = new FileStream(path, FileMode.Create);
            await using var sw = new StreamWriter(fs);

            var list = JsonSerializer.Serialize(List.Values.ToList());
            await sw.WriteAsync(list);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "save failed. path: {path}", path);
        }

        _isSaving = false;
    }

    public async Task OnFinishedDownloadRemote(ZipmodLink link, string path)
    {
        try
        {
            var info = ZipmodInfo.Create(path);
            link.Info = await service.GetOrAddInfo(info);
        }
        catch (Exception)
        {
            link.IsInvalid = true;
        }
    }
}