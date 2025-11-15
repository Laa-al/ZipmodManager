namespace Zmm;

public static class ZmmDbProperties
{
    public static string DbTablePrefix { get; set; } = "Zmm";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "Zmm";
}
