using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Zmm.Zipmods;

namespace Zmm.EntityFrameworkCore;

[ConnectionStringName(ZmmDbProperties.ConnectionStringName)]
public interface IZmmDbContext : IEfCoreDbContext
{
    DbSet<ZipmodFile> ZipmodFiles { get; }
    DbSet<ZipmodLink> ZipmodLinks { get; }
    DbSet<ZipmodInfo> ZipmodInfos { get; }
}