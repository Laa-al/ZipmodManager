namespace Zmm;

public static class Utils
{
    public static string GetSizeString(double length)
    {
        var size = length;
        var unit = "byte";
        if (size > 1024)
        {
            size /= 1024;
            unit = "K";
        }

        if (size > 1024)
        {
            size /= 1024;
            unit = "M";
        }

        if (size > 1024)
        {
            size /= 1024;
            unit = "G";
        }

        return $"{size:N}{unit}";
    }
}