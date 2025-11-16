namespace Zmm.Zipmods;

public class ZipmodFileRequestInput : ZipmodInfoRequestInput
{
    public string? Path { get; set; }

    public override void Clear()
    {
        base.Clear();
        Path = null;
    }
}