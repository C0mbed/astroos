// src/AstroOS.UI/MainWindow.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using AstroOS.UI.ViewModels;

namespace AstroOS.UI;

/// <summary>
/// The application's single root window.
/// TODO(forge): Implement full App Shell layout — Titlebar, Nav Rail,
///              Active Pane, Device Panel, Log Strip per app-shell-tech-spec.md
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow(IServiceProvider services)
    {
        InitializeComponent();
        AppWindow.Resize(new Windows.Graphics.SizeInt32(1024, 700));

        // Resolve and wire the Status Bar ViewModel. The StatusBar code-behind
        // (StatusBar.xaml.cs) forwards ConnectionWidget's DataContext from this VM.
        StatusBarControl.DataContext = services.GetRequiredService<StatusBarViewModel>();
    }
}
