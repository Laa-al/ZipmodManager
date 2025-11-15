using System.Collections.Generic;

namespace Zmm.Mmds;

public class MmdGroup(string path, string danceName, int dancerNum)
{
    public List<MmdInfo> MotionFiles { get; } = [];
    public List<MmdInfo> MorphFiles { get; } = [];
    public List<MmdInfo> CameraFiles { get; } = [];
    public List<string> MusicFiles { get; } = [];
    public string Path { get; } = path;
    public string DanceName { get; } = danceName;
    public int DancerNum { get; set; } = dancerNum;
}