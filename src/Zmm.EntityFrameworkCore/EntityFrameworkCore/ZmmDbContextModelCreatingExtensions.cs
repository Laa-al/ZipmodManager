using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Zmm.Zipmods;

namespace Zmm.EntityFrameworkCore;

public static class ZmmDbContextModelCreatingExtensions
{
    public static void ConfigureZmm(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<ZipmodInfo>(b =>
        {
            b.ToTable(nameof(ZipmodInfo));
            b.ConfigureByConvention();
        });
        builder.Entity<ZipmodFile>(b =>
        {
            b.ToTable(nameof(ZipmodFile));
            b.ConfigureByConvention();
            b.HasOne(u => u.Info).WithMany(u => u.Files)
                .HasForeignKey(u => u.InfoId);
        });
        builder.Entity<ZipmodLink>(b =>
        {
            b.ToTable(nameof(ZipmodLink));
            b.ConfigureByConvention();
            b.HasOne(u => u.Info).WithMany(u => u.Links)
                .HasForeignKey(u => u.InfoId);
        });
    }
}