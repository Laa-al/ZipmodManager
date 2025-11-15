namespace Zmm.Mmds;

public class MmdInfo(string fileName, int motion, int morph, int camera, int light)
{
    public int Motion { get; } = motion;
    public int Morph { get; } = morph;
    public int Camera { get; } = camera;
    public int Light { get; } = light;
    public string FileName { get; set; } = fileName;
}