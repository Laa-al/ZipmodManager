namespace Laaal.Models;

public class ZipmodLink
{
    public Uri? Uri { get; set; } 

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public DateTime UploadTime { get; set; }

    public ZipmodInfo? Info { get; set; }
}