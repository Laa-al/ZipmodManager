using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Zmm.Zipmods;

namespace Zmm.EntityFrameworkCore;

public class ZmmMigrateDbContext(DbContextOptions<ZmmMigrateDbContext> options) 
    : AbpDbContext<ZmmMigrateDbContext>(options), IZmmDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureZmm();
    }

    public DbSet<ZipmodFile> ZipmodFiles { get; set; }
    public DbSet<ZipmodLink> ZipmodLinks { get; set; }
    public DbSet<ZipmodInfo> ZipmodInfos { get; set; }
}