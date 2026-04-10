// src/AstroOS.UI/Controls/ConnectionWidget.xaml.cs
using System.ComponentModel;
using AstroOS.UI.Models;
using AstroOS.UI.ViewModels;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;

namespace AstroOS.UI.Controls;

/// <summary>
/// Code-behind for the Connection Widget UserControl (Status Bar Zone B).
/// Observes ConnectionWidgetViewModel.State and drives VisualStateManager.
/// All business logic lives in ConnectionWidgetViewModel. This file is pure
/// view wiring: DataContext change → VisualStateManager.GoToState.
/// </summary>
public sealed partial class ConnectionWidget : UserControl
{
    private ConnectionWidgetViewModel? _viewModel;
    private bool _animationsEnabled = true;

    public ConnectionWidget()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    // ── Template ──────────────────────────────────────────────────────────────

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Read Reduce Motion preference from Windows accessibility settings.
        // This respects "Settings → Accessibility → Visual Effects → Animation effects".
        // PLATFORM: WinUI3 — UISettings is read once at control load. For live
        // sensitivity to mid-session changes, subscribe to UISettings.AnimationsEnabledChanged
        // (available in Windows App SDK 1.4+). Deferred to a future accessibility pass.
        var uiSettings = new UISettings();
        _animationsEnabled = uiSettings.AnimationsEnabled;
    }

    // ── DataContext wiring ────────────────────────────────────────────────────

    private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // Unsubscribe from the old ViewModel.
        if (_viewModel is not null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        _viewModel = args.NewValue as ConnectionWidgetViewModel;

        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;

            // Snap to current state without transitions (no "intro" animation on first load).
            ApplyVisualState(_viewModel.State, useTransitions: false);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ConnectionWidgetViewModel.State) && _viewModel is not null)
        {
            // Reduce Motion: pass useTransitions=false to GoToState which skips all
            // VisualTransition Storyboards AND GeneratedDuration interpolation.
            // Status Bar Amendment v1.2.0 §5.3: "Reduce Motion: instant state swap."
            ApplyVisualState(_viewModel.State, useTransitions: _animationsEnabled);
        }
    }

    // ── VisualState application ───────────────────────────────────────────────

    /// <summary>
    /// Maps ConnectionState enum to the corresponding VisualState name and calls
    /// GoToState. State names must match x:Name attributes in ConnectionWidget.xaml.
    /// </summary>
    private void ApplyVisualState(ConnectionState state, bool useTransitions)
    {
        string stateName = state switch
        {
            ConnectionState.AllDisconnected    => "AllDisconnected",
            ConnectionState.Connecting         => "Connecting",
            ConnectionState.PartiallyConnected => "PartiallyConnected",
            ConnectionState.AllConnected       => "AllConnected",
            ConnectionState.SessionReady       => "SessionReady",
            ConnectionState.ErrorPresent       => "ErrorPresent",
            _                                  => "AllDisconnected",
        };

        VisualStateManager.GoToState(this, stateName, useTransitions);
    }
}
