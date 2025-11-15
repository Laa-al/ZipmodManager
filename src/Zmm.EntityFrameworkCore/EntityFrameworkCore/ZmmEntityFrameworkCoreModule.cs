using Microsoft.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Modularity;
using Zmm.Zipmods;

namespace Zmm.EntityFrameworkCore;

[DependsOn(
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(ZmmDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class ZmmEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ZmmDbContext>(options =>
        {
            options.AddDefaultRepositories<IZmmDbContext>(includeAllEntities: true);

            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
        });

        context.Services.Configure<AbpEntityOptions>(o =>
        {
            o.Entity<ZipmodLink>(e =>
            {
                e.DefaultWithDetailsFunc = q => q
                        .Include(v => v.Info)
                        .ThenInclude(u => u!.Files)
                    ;
            });
            o.Entity<ZipmodFile>(e =>
            {
                e.DefaultWithDetailsFunc = q => q
                        .Include(v => v.Info)
                    ;
            });
            o.Entity<ZipmodInfo>(e =>
            {
                e.DefaultWithDetailsFunc = q => q
                        .Include(v => v.Files)
                        .Include(v => v.Links)
                    ;
            });
        });
    }
}