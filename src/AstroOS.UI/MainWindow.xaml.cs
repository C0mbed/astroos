// src/AstroOS.UI/MainWindow.xaml.cs
using Microsoft.UI.Xaml;

namespace AstroOS.UI;

/// <summary>
/// The application's single root window.
/// TODO(forge): Implement App Shell layout — Titlebar, Status Bar, Nav Rail,
///              Active Pane, Device Panel, Log Strip per app-shell-tech-spec.md
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
