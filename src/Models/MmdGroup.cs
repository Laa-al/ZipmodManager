namespace Laaal.Models;

public class MmdGroup
{
    public List<MmdInfo> MotionFiles { get; } = new();
    public List<MmdInfo> MorphFiles { get; } = new();
    public List<MmdInfo> CameraFiles { get; } = new();
    public List<string> MusicFiles { get; } = new();
    public string Path { get; }
    public string DanceName { get; }
    public int DancerNum { get; set; }

    public MmdGroup(string path, string danceName, int dancerNum)
    {
        Path = path;
        DanceName = danceName;
        DancerNum = dancerNum;
    }
}