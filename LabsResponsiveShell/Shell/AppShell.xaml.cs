using System.ComponentModel;
using LabsResponsiveShell.Pages;

namespace LabsResponsiveShell.Shell;

public sealed partial class AppShell : Page
{
    public AppShell()
    {
        InitializeComponent();
        ApplyFromSettings();
        AppSettings.Current.PropertyChanged += OnSettingsChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ActiveFrame.Navigate(typeof(HomePage));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppSettings.Current.PropertyChanged -= OnSettingsChanged;
    }

    private Frame ActiveFrame => WideLayout.Visibility == Visibility.Visible
        ? WideContentFrame
        : NarrowContentFrame;

    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppSettings.Theme)
            or nameof(AppSettings.IsDarkTheme)
            or nameof(AppSettings.IsRightToLeft)
            or nameof(AppSettings.FlowDirection))
        {
            ApplyFromSettings();
        }
    }

    private void ApplyFromSettings()
    {
        RequestedTheme = AppSettings.Current.Theme;
        FlowDirection = AppSettings.Current.FlowDirection;
    }

    private void OnNavigateHome(object sender, RoutedEventArgs e) =>
        ActiveFrame.Navigate(typeof(HomePage));

    private void OnNavigateChat(object sender, RoutedEventArgs e) =>
        ActiveFrame.Navigate(typeof(ChatPage));

    private void OnNavigateSettings(object sender, RoutedEventArgs e) =>
        ActiveFrame.Navigate(typeof(SettingsPage));
}
