using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Laaal.Services;
using Serilog;
using Serilog.Events;

namespace Laaal;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        Log.Information("Starting WPF host.");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        serviceCollection.AddWpfBlazorWebView();
        serviceCollection.AddMasaBlazor();

        serviceCollection.AddSingleton<RemoteModService>();
        serviceCollection.AddSingleton<DownloadService>();
        serviceCollection.AddSingleton<LocalModService>();
        serviceCollection.AddSingleton<ModAnalizeService>();
        serviceCollection.AddSingleton<SideLoaderModService>();

#if DEBUG
		serviceCollection.AddBlazorWebViewDeveloperTools();
#endif

        Resources.Add("services", serviceCollection.BuildServiceProvider());
    }
}
