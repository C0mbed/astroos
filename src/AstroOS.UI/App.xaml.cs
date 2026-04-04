// src/AstroOS.UI/App.xaml.cs
using Microsoft.UI.Xaml;

namespace AstroOS.UI;

/// <summary>
/// Application entry point. Hosts the WinUI 3 window and DI container.
/// TODO(forge): Wire up DI container (Microsoft.Extensions.DependencyInjection),
///              register all project services, and initialise ThemeManager.
/// </summary>
public partial class App : Application
{
    private MainWindow? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
