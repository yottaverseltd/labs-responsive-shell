using System.ComponentModel;
using LabsResponsiveShell.Motion;
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
        WideContentFrame.Navigated += OnFrameNavigated;
        NarrowContentFrame.Navigated += OnFrameNavigated;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Navigate(typeof(HomePage));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppSettings.Current.PropertyChanged -= OnSettingsChanged;
        WideContentFrame.Navigated -= OnFrameNavigated;
        NarrowContentFrame.Navigated -= OnFrameNavigated;
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        if (e.Content is FrameworkElement page)
        {
            MotionPrimitives.PlayMount(page);
        }
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
        // why: setting theme on the window root makes the swap propagate across the whole tree,
        // including the Frame's navigated content on both Desktop and WASM heads
        if (App.MainWindow?.Content is FrameworkElement root)
        {
            root.RequestedTheme = AppSettings.Current.Theme;
        }

        RequestedTheme = AppSettings.Current.Theme;
        FlowDirection = AppSettings.Current.FlowDirection;
    }

    private void OnNavigateHome(object sender, RoutedEventArgs e) => Navigate(typeof(HomePage));

    private void OnNavigateChat(object sender, RoutedEventArgs e) => Navigate(typeof(ChatPage));

    private void OnNavigateSettings(object sender, RoutedEventArgs e) => Navigate(typeof(SettingsPage));

    private void Navigate(Type pageType)
    {
        ActiveFrame.Navigate(pageType);
        UpdateSelection(pageType);
    }

    private void UpdateSelection(Type pageType)
    {
        WideHomeItem.IsSelected = pageType == typeof(HomePage);
        WideChatItem.IsSelected = pageType == typeof(ChatPage);
        WideSettingsItem.IsSelected = pageType == typeof(SettingsPage);

        NarrowHomeItem.IsSelected = pageType == typeof(HomePage);
        NarrowChatItem.IsSelected = pageType == typeof(ChatPage);
        NarrowSettingsItem.IsSelected = pageType == typeof(SettingsPage);
    }
}
