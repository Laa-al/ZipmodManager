using System.IO;
using Laaal.Models;
using Microsoft.Extensions.Logging;

namespace Laaal.Services;

public class LocalModService(ModAnalizeService service, ILoggerFactory factory)
{
    private ILogger Logger { get; } = factory.CreateLogger<LocalModService>();
    public List<ZipmodFile> List { get; } = [];

    private Queue<string?> Directories { get; set; } = [];

    private int DirectoryCount { get; set; }

    private bool IsLoading { get; set; }


    public async Task LoadModListAsync(string? path)
    {
        if (IsLoading) return;
        IsLoading = true;

        await CleanListAsync();

        Directories = [];
        Directories.Enqueue(path);
        DirectoryCount = 1;

        while (DirectoryCount > 0)
        {
            if (!Directories.TryDequeue(out path))
            {
                await Task.Delay(100);
                continue;
            }

            if (path is null)
            {
                DirectoryCount--;
                continue;
            }

            _ = LoadFromPathAsync(path).ConfigureAwait(false);
        }


        IsLoading = false;
    }

    private async Task LoadFromPathAsync(string path)
    {
        try
        {
            Logger.LogInformation("start analyze {path}", path);

            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                List.RemoveAll(u => u.Path == file);
                try
                {
                    var zipmodInfo = ZipmodInfo.Create(file);
                    zipmodInfo = await service.GetOrAddInfo(zipmodInfo);
                    var zipmodFile = new ZipmodFile(file, zipmodInfo);

                    lock (List)
                    {
                        List.Add(zipmodFile);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "analyze file failed. path: {path}.", file);
                }
            }

            var directories = Directory.GetDirectories(path);

            foreach (var directory in directories)
            {
                Directories.Enqueue(directory);
                DirectoryCount++;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "analyze failed. path: {path}.", path);
        }
        finally
        {
            DirectoryCount--;
        }

        await Task.CompletedTask;
    }


    public async Task CleanListAsync()
    {
        for (int i = 0; i < List.Count; i++)
        {
            if (File.Exists(List[i].Path)) continue;

            List.RemoveAt(i);
            i--;
        }

        await Task.CompletedTask;
    }

    public async Task MoveToTargetPath(string pathPrefix, string path)
    {
        try
        {
            var files = List.Where(u => u.Path.StartsWith(pathPrefix)).ToList();

            foreach (var file in files)
            {
                var directory = FitPropertyToPath(file.Author);
                if (string.IsNullOrWhiteSpace(directory)) directory = "unknown";
                var filename = FitPropertyToPath(file.Guid);
                await MoveToTargetPath(file, Path.Combine(path, directory, filename + ".zipmod"));
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "move failed. from {prefix} to {target}", pathPrefix, path);
        }
    }

    public async Task MoveToTargetPath(ZipmodFile file, string path)
    {
        if (!File.Exists(file.Path))
        {
            List.Remove(file);
            throw new FileNotFoundException("mode file is moved or deleted;");
        }

        var directory = Path.GetDirectoryName(path)!;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(path))
        {
            try
            {
                var info = ZipmodInfo.Create(path);
                if (info > file.Info)
                {
                    File.Move(file.Path, path, true);
                }
                else
                {
                    File.Delete(file.Path);
                    file.Info = await service.GetOrAddInfo(info);
                }
            }
            catch (Exception)
            {
                File.Move(file.Path, path, true);
            }
        }
        else
        {
            File.Move(file.Path, path, true);
        }

        file.Path = path;

        await Task.CompletedTask;
    }

    private static string? FitPropertyToPath(string? property)
    {
        if (property is null)
        {
            return null;
        }

        List<char> chars = property.ToList();

        for (int i = 0; i < chars.Count; i++)
        {
            char c = chars[i];
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
            int start = chars.IndexOf(lft);
            int end = chars.IndexOf(rht);
            if (start >= 0 && end >= 0 && start < end)
            {
                chars.RemoveRange(start, end - start + 1);
            }
        }
    }
}