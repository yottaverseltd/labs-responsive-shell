using System.ComponentModel;
using LabsResponsiveShell.Data;
using LabsResponsiveShell.Motion;
using LabsResponsiveShell.Pages;
using Microsoft.UI.Xaml.Media.Animation;

namespace LabsResponsiveShell.Shell;

public sealed partial class AppShell : Page
{
    public static AppShell? Current { get; private set; }

    private DispatcherTimer? _toastTimer;
    private Type _routeType = typeof(HomePage);

    public AppShell()
    {
        InitializeComponent();
        Current = this;
        ApplyFromSettings();
        AppSettings.Current.PropertyChanged += OnSettingsChanged;
        ToastRelay.Requested += OnToastRequested;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        RootGrid.SizeChanged += OnRootGridSizeChanged;
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
        ToastRelay.Requested -= OnToastRequested;
        RootGrid.SizeChanged -= OnRootGridSizeChanged;
        WideContentFrame.Navigated -= OnFrameNavigated;
        NarrowContentFrame.Navigated -= OnFrameNavigated;
        if (ReferenceEquals(Current, this))
        {
            Current = null;
        }
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        if (e.Content is FrameworkElement page)
        {
            MotionPrimitives.PlayMount(page);
        }
    }

    private void OnRootGridSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // Keep both frames on the same route when the breakpoint toggles (WASM/desktop resize).
        SyncBothFrames(_routeType);
    }

    private void SyncBothFrames(Type pageType)
    {
        NavigateFrameIfNeeded(WideContentFrame, pageType);
        NavigateFrameIfNeeded(NarrowContentFrame, pageType);
    }

    private static void NavigateFrameIfNeeded(Frame frame, Type pageType)
    {
        if (frame.Content?.GetType() == pageType)
        {
            return;
        }

        frame.Navigate(pageType);
    }

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
        // why: setting theme on the window root propagates across the tree, including frame content
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

    /// <summary>Public hook: Home row taps route here with a pending conversation id.</summary>
    public void NavigateToChat() => Navigate(typeof(ChatPage));

    private void Navigate(Type pageType)
    {
        _routeType = pageType;
        ShowSkeleton();
        SyncBothFrames(pageType);
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

    private void ShowSkeleton()
    {
        SkeletonOverlay.Opacity = 1;
        var hold = ReducedMotion.IsReducedMotion
            ? TimeSpan.FromMilliseconds(120)
            : TimeSpan.FromMilliseconds(200);

        var holdTimer = new DispatcherTimer { Interval = hold };
        holdTimer.Tick += (_, _) =>
        {
            holdTimer.Stop();
            FadeSkeletonOut();
        };
        holdTimer.Start();
    }

    private void FadeSkeletonOut()
    {
        if (ReducedMotion.IsReducedMotion)
        {
            SkeletonOverlay.Opacity = 0;
            return;
        }

        var story = new Storyboard();
        var fade = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(140)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(fade, SkeletonOverlay);
        Storyboard.SetTargetProperty(fade, "Opacity");
        story.Children.Add(fade);
        story.Begin();
    }

    private void OnToastRequested(object? sender, string message)
    {
        ToastText.Text = message;
        if (ToastHost.RenderTransform is not TranslateTransform t)
        {
            t = new TranslateTransform();
            ToastHost.RenderTransform = t;
        }

        _toastTimer?.Stop();

        if (ReducedMotion.IsReducedMotion)
        {
            ToastHost.Opacity = 1;
            t.Y = 0;
        }
        else
        {
            var show = new Storyboard();
            var opacityIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(160)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            };
            Storyboard.SetTarget(opacityIn, ToastHost);
            Storyboard.SetTargetProperty(opacityIn, "Opacity");
            var slideIn = new DoubleAnimation
            {
                From = 16,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            };
            Storyboard.SetTarget(slideIn, t);
            Storyboard.SetTargetProperty(slideIn, "Y");
            show.Children.Add(opacityIn);
            show.Children.Add(slideIn);
            show.Begin();
        }

        _toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _toastTimer.Tick += ToastTimerOnTick;
        _toastTimer.Start();
    }

    private void ToastTimerOnTick(object? sender, object e)
    {
        if (_toastTimer is not null)
        {
            _toastTimer.Tick -= ToastTimerOnTick;
            _toastTimer.Stop();
            _toastTimer = null;
        }

        if (ReducedMotion.IsReducedMotion)
        {
            ToastHost.Opacity = 0;
            return;
        }

        var story = new Storyboard();
        var fade = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(160)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn },
        };
        Storyboard.SetTarget(fade, ToastHost);
        Storyboard.SetTargetProperty(fade, "Opacity");
        story.Children.Add(fade);
        story.Begin();
    }
}
