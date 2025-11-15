using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Zmm.EntityFrameworkCore;

public class ZmmMigrateDbContextFactory: IDesignTimeDbContextFactory<ZmmMigrateDbContext>
{
    public ZmmMigrateDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<ZmmMigrateDbContext>();

        builder = builder.UseSqlite(configuration["ConnectionStrings:Default"]);
        
        return new ZmmMigrateDbContext(builder.Options);
    }


    protected IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}