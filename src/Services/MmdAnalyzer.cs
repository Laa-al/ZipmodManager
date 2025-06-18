using System.IO;
using System.Text;
using Laaal.Models;
using Microsoft.Extensions.Logging;

namespace Laaal.Services;

public class MmdAnalyzer(ILoggerFactory factory)
{
    private ILogger Logger { get; } = factory.CreateLogger<MmdAnalyzer>();


    public void SimplyFileName()
    {
    }


    public void SimplyFileNames(string path)
    {
        var groups = CreateGroupsFromPath(path);

        foreach (var group in groups)
        {
            MoveInfoFile(group, group.MotionFiles, "Motion");
            MoveInfoFile(group, group.MorphFiles, "Morph");
            MoveInfoFile(group, group.CameraFiles, "Camera");
            MoveFile(group, group.MusicFiles, "Music", "wav");
        }

        return;

        void MoveInfoFile(MmdGroup group, List<MmdInfo> infos, string prefix)
        {
            var names = infos.Select(u => u.FileName).ToList();
            MoveFile(group, names, prefix, "vmd");
            for (var i = 0; i < names.Count; i++)
            {
                infos[i].FileName = names[i];
            }
        }

        void MoveFile(MmdGroup group, List<string> names, string prefix, string suffix)
        {
            for (var i = 0; i < names.Count; i++)
            {
                var fileName = prefix + i + "." + suffix + ".cache";
                var name = names[i];
                var srcPath = Path.Combine(group.Path, name);
                var dstPath = Path.Combine(group.Path, fileName);
                File.Move(srcPath, dstPath);
                names[i] = fileName;
            }

            for (var i = 0; i < names.Count; i++)
            {
                var fileName = prefix + i + "." + suffix;
                var name = names[i];
                var srcPath = Path.Combine(group.Path, name);
                var dstPath = Path.Combine(group.Path, fileName);
                File.Move(srcPath, dstPath);
            }
        }
    }


    public void CreateListFile(string path, string rootPath, string listName, bool sorted)
    {
        var groups = CreateGroupsFromPath(path);
        if (sorted)
            groups = groups.OrderBy(u => u.DanceName).ToList();
        using var fs = File.OpenWrite(Path.Combine(rootPath, listName));
        using var sw = new StreamWriter(fs);
        foreach (var group in groups)
        {
            Logger.LogInformation("Start write dance {danceName}", group.DanceName);
            sw.WriteLine($"[{group.DanceName}]");
            var relativePath = Path.GetRelativePath(rootPath, group.Path);
            if (relativePath.StartsWith('\\'))
                relativePath = relativePath[1..];

            sw.Write($"basefolder={relativePath}");
            if (!relativePath.EndsWith('\\'))
                sw.Write('\\');
            sw.WriteLine();


            sw.WriteLine($"DancerNumber={group.DancerNum}");

            for (var i = 0; i < group.DancerNum; i++)
            {
                sw.WriteLine($"Dancer{i}Motion={group.MotionFiles[i].FileName}");

                if (group.MorphFiles.Count > i &&
                    group.MorphFiles[i] is { Morph: > 0 } morph)
                    sw.WriteLine($"Dancer{i}Morph={morph.FileName}");
                else if (group.MotionFiles[i] is { Morph: > 0 } motion)
                    sw.WriteLine($"Dancer{i}Morph={motion.FileName}");
            }

            for (var i = 0; i < group.CameraFiles.Count; i++)
            {
                var info = group.CameraFiles[i];
                sw.WriteLine($"Camera{i}={info.FileName}");
            }

            sw.WriteLine($"Music={group.MusicFiles.First()}");
            sw.WriteLine();
        }
    }

    public List<MmdGroup> CreateGroupsFromPath(string path)
    {
        List<MmdGroup> res = [];
        if (!Directory.Exists(path)) return res;

        Queue<string> folders = [];
        folders.Enqueue(path);

        while (folders.TryDequeue(out var folder))
        {
            try
            {
                var group = CreateGroupFromPath(folder);
                if (group is not null)
                {
                    Logger.LogInformation("Start analyze dance {folder}", folder);
                    res.Add(group);
                }
                else
                {
                    Logger.LogInformation("Start analyze folder {folder}", folder);
                    var subFolders = Directory.GetDirectories(folder);
                    foreach (var subFolder in subFolders)
                        folders.Enqueue(subFolder);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Analyze folder failed. {folder}", folder);
            }
        }

        return res;
    }

    private MmdInfo CreateInfoFromPath(string path)
    {
        var fileName = Path.GetFileName(path);

        using var stream = File.OpenRead(path);
        using BinaryReader reader = new(stream);

        var vmdVersion = Encoding.ASCII.GetString(reader.ReadBytes(30));

        var version2 = vmdVersion.StartsWith("Vocaloid Motion Data 0002");

        _ = Encoding.ASCII.GetString(reader.ReadBytes(version2 ? 20 : 10));

        return new MmdInfo(fileName,
            ReadByte(111), ReadByte(23),
            ReadByte(61), ReadByte(28)
        );

        int ReadByte(int length)
        {
            if (reader.BaseStream.Length <= reader.BaseStream.Position)
            {
                return 0;
            }

            var ret = (int)reader.ReadUInt32();
            if (ret > 0
                && ret * length <= reader.BaseStream.Length - reader.BaseStream.Position)
            {
                reader.BaseStream.Seek(ret * length, SeekOrigin.Current);
            }

            return ret;
        }
    }

    private MmdGroup? CreateGroupFromPath(string folder)
    {
        var dancerNum = 1;
        if (folder.EndsWith('P'))
        {
            if (int.TryParse(folder[^3..^1], out var count))
            {
                dancerNum = count;
            }
        }

        var danceName = Path.GetFileName(folder)
            .Replace("[", null).Replace("]", null);
        var fileNames = Directory.GetFiles(folder);

        if (!fileNames.Any(u => u.EndsWith(".vmd",
                StringComparison.OrdinalIgnoreCase))) return null;


        MmdGroup group = new(folder, danceName, dancerNum);

        foreach (var fileName in fileNames)
        {
            if (fileName.EndsWith(".wav"))
            {
                group.MusicFiles.Add(Path.GetFileName(fileName));
            }
            else if (fileName.EndsWith(".vmd"))
            {
                var info = CreateInfoFromPath(fileName);
                if (info.Motion > 0)
                {
                    group.MotionFiles.Add(info);
                }
                else if (info.Morph > 0)
                {
                    group.MorphFiles.Add(info);
                }
                else if (info.Camera > 0)
                {
                    group.CameraFiles.Add(info);
                }
            }
        }

        if (!group.MusicFiles.Any())
            return null;

        group.MotionFiles.Sort((u, v) => v.Motion - u.Motion);
        group.MorphFiles.Sort((u, v) => v.Morph - u.Morph);
        group.CameraFiles.Sort((u, v) => v.Camera - u.Camera);
        group.DancerNum = Math.Min(group.MotionFiles.Count, group.DancerNum);
        return group;
    }
}