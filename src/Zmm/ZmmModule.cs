using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Zmm.EntityFrameworkCore;

namespace Zmm;

[DependsOn(
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(ZmmEntityFrameworkCoreModule),
    typeof(ZmmApplicationModule),
    typeof(ZmmBlazorModule),
    typeof(AbpAutofacModule)
)]
public class ZmmModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ZmmMigrateDbContext>();
        context.Services.AddSingleton<MainWindow>();
        Configure<AbpDbContextOptions>(options => { options.Configure(configurationContext => { configurationContext.UseSqlite(); }); });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        using var scope = context.ServiceProvider.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ZmmMigrateDbContext>();
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Failed to connect to the database!");
        }
    }
}