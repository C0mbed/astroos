// src/AstroOS.UI/App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using AstroOS.UI.Services.Interfaces;
using AstroOS.UI.ViewModels;
#if DEBUG
using AstroOS.UI.Services.Mock;
#endif

namespace AstroOS.UI;

/// <summary>
/// Application entry point. Hosts the WinUI 3 window and DI container.
/// </summary>
public partial class App : Application
{
    private MainWindow? _window;

    /// <summary>Application-wide service provider. Available after the App constructor completes.</summary>
    public IServiceProvider Services { get; }

    public App()
    {
        InitializeComponent();
        Services = BuildServices();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow(Services);
        _window.Activate();
    }

    private static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

#if DEBUG
        // Mock services for visual verification — replaced by real implementations at release.
        services.AddSingleton<IConnectionService,  MockConnectionService>();
        services.AddSingleton<ISafetyService,      MockSafetyService>();
        services.AddSingleton<ISequencerService,   MockSequencerService>();
        services.AddSingleton<ISiteProfileService, MockSiteProfileService>();
#endif

        services.AddTransient<StatusBarViewModel>();

        return services.BuildServiceProvider();
    }
}
