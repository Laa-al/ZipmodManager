namespace Laaal.Models;

public class MmdInfo
{
    public int Motion { get; }
    public int Morph { get; }
    public int Camera { get; }
    public int Light { get; }
    public string FileName { get; set; }

    public MmdInfo(string fileName, int motion, int morph, int camera, int light)
    {
        FileName = fileName;
        Motion = motion;
        Morph = morph;
        Camera = camera;
        Light = light;
    }

}