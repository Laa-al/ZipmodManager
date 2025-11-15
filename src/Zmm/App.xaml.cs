using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Volo.Abp;

namespace Zmm;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private IAbpApplicationWithInternalServiceProvider? _abpApplication;

    // ReSharper disable once AsyncVoidEventHandlerMethod
    protected override async void OnStartup(StartupEventArgs e)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File(
                $"Logs/{DateTime.Now:yyyy-MM-dd}.txt",
                rollOnFileSizeLimit: true, fileSizeLimitBytes: 10485760))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting WPF host.");

            _abpApplication = await AbpApplicationFactory.CreateAsync<ZmmModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

                options.Services.AddWpfBlazorWebView();
#if DEBUG
                options.Services.AddBlazorWebViewDeveloperTools();
#endif
                options.Services.AddMasaBlazor();
                // Resources.Add("services", options.Services.BuildServiceProvider());
            });

            await _abpApplication.InitializeAsync();

            _abpApplication.Services.GetRequiredService<MainWindow>().Show();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
        }
    }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_abpApplication != null)
        {
            await _abpApplication.ShutdownAsync();
        }

        await Log.CloseAndFlushAsync();
    }
}