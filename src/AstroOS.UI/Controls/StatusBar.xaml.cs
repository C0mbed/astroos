// src/AstroOS.UI/Controls/StatusBar.xaml.cs
using AstroOS.UI.ViewModels;
using Microsoft.UI.Xaml;

namespace AstroOS.UI.Controls;

/// <summary>
/// Code-behind for the StatusBar UserControl.
/// Minimal: wires the ConnectionWidget's DataContext to
/// StatusBarViewModel.ConnectionWidget when the DataContext is set.
/// All logic lives in StatusBarViewModel and ConnectionWidgetViewModel.
/// </summary>
public sealed partial class StatusBar : UserControl
{
    public StatusBar()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // When a StatusBarViewModel is assigned as our DataContext, wire Zone B's
        // DataContext to its owned ConnectionWidgetViewModel.
        // TODO(forge): Replace manual wiring with container.GetRequiredService<StatusBarViewModel>()
        // once DI is wired up in App.xaml.cs per the App Shell contract.
        if (args.NewValue is StatusBarViewModel vm)
        {
            ConnectionWidgetControl.DataContext = vm.ConnectionWidget;
        }
    }
}
