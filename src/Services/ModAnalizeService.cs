using Laaal.Models;

namespace Laaal.Services;

public class ModAnalizeService
{
    public List<ZipmodInfo> List { get; } = [];


    public Task<ZipmodInfo> GetOrAddInfo(ZipmodInfo info)
    {
        return Task.Run(() =>
        {
            ZipmodInfo? res;
            lock (List)
            {
                res = List.FirstOrDefault(u => u.Guid == info.Guid && u.Version == info.Version);

                if (res is null)
                {
                    res = info;
                    List.Add(info);
                }
                else
                {
                    info.MapTo(res);
                }
            }

            return res;
        });
    }
}