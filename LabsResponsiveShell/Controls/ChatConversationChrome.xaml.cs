using LabsResponsiveShell.Data;
using LabsResponsiveShell.Motion;
using Microsoft.UI.Xaml.Media.Animation;

namespace LabsResponsiveShell.Controls;

public sealed partial class ChatConversationChrome : UserControl
{
    public static readonly DependencyProperty ConversationProperty = DependencyProperty.Register(
        nameof(Conversation),
        typeof(MockConversation),
        typeof(ChatConversationChrome),
        new PropertyMetadata(null, OnConversationChanged));

    public static readonly DependencyProperty BubbleCapWidthProperty = DependencyProperty.Register(
        nameof(BubbleCapWidth),
        typeof(double),
        typeof(ChatConversationChrome),
        new PropertyMetadata(520d));

    private DispatcherTimer? _typingTimer;
    private int _typingPhase;

    public ChatConversationChrome()
    {
        InitializeComponent();
        Loaded += OnLoadedInner;
        Unloaded += OnUnloaded;
        SizeChanged += OnChromeSized;
    }

    private void OnLoadedInner(object sender, RoutedEventArgs e) => ResizeBubbles();

    private void OnUnloaded(object sender, RoutedEventArgs e) => StopTypingPulse();

    private void OnChromeSized(object sender, SizeChangedEventArgs e) => ResizeBubbles();

    public MockConversation? Conversation
    {
        get => (MockConversation?)GetValue(ConversationProperty);
        set => SetValue(ConversationProperty, value);
    }

    public double BubbleCapWidth
    {
        get => (double)GetValue(BubbleCapWidthProperty);
        private set => SetValue(BubbleCapWidthProperty, value);
    }

    private static void OnConversationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ChatConversationChrome)d).RefreshConversation();

    private void ResizeBubbles()
    {
        var w = ActualWidth;
        if (w <= 0)
        {
            return;
        }

        BubbleCapWidth = Math.Min(520, w * 0.68);
    }

    private void RefreshConversation()
    {
        StopTypingPulse();
        ThreadScroller.LayoutUpdated -= OnThreadLayoutOnce;
        ThreadScroller.LayoutUpdated += OnThreadLayoutOnce;

        var conv = Conversation;
        if (conv is null)
        {
            ThreadItems.ItemsSource = null;
            return;
        }

        ThreadItems.ItemsSource = conv.Messages;
        HeaderTitleText.Text = conv.Title;
        HeaderSubtitleText.Text = conv.ThreadPresenceSubtitle;
        HeaderInitials.Text = InitialsFormatter.FromTitle(conv.Title);
        var avatarIdx = AvatarPalette.IndexFor(conv.Title);
        HeaderAvatar.Fill =
            (Brush)(Application.Current.Resources[$"AvatarTone{avatarIdx}Brush"] ?? new SolidColorBrush());

        if (string.IsNullOrWhiteSpace(conv.TypingLine))
        {
            TypingStripe.Visibility = Visibility.Collapsed;
        }
        else
        {
            TypingStripe.Visibility = Visibility.Visible;
            TypingLabel.Text = conv.TypingLine;
        }

        PlayHeaderHaloOnce();
        StartTypingTimerIfNeeded();
    }

    private void OnThreadLayoutOnce(object? sender, object e)
    {
        ThreadScroller.LayoutUpdated -= OnThreadLayoutOnce;
        TryScrollMessagesToEnd();
    }

    private void StopTypingPulse()
    {
        if (_typingTimer is null)
        {
            return;
        }

        _typingTimer.Tick -= TypingTimerOnTick;
        _typingTimer.Stop();
        _typingTimer = null;
    }

    private void StartTypingTimerIfNeeded()
    {
        if (ReducedMotion.IsReducedMotion || TypingStripe.Visibility != Visibility.Visible)
        {
            return;
        }

        _typingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _typingTimer.Tick += TypingTimerOnTick;
        _typingTimer.Start();
    }

    private void TypingTimerOnTick(object? sender, object e)
    {
        _typingPhase = (_typingPhase + 1) % 3;
        const double lo = 0.4;
        const double hi = 0.9;
        TypingDotA.Opacity = _typingPhase == 0 ? hi : lo;
        TypingDotB.Opacity = _typingPhase == 1 ? hi : lo;
        TypingDotC.Opacity = _typingPhase == 2 ? hi : lo;
    }

    private void PlayHeaderHaloOnce()
    {
        HeaderAvatarGlow.RenderTransform ??= new CompositeTransform();
        if (HeaderAvatarGlow.RenderTransform is not CompositeTransform composite)
        {
            return;
        }

        composite.CenterX = 24;
        composite.CenterY = 24;
        composite.ScaleX = composite.ScaleY = 1;

        if (ReducedMotion.IsReducedMotion)
        {
            return;
        }

        // one-shot pulse: expand then settle back to rest in a single 220 ms window
        var pulse = new Storyboard();
        var sx = BuildHaloScaleKeyframes();
        var sy = BuildHaloScaleKeyframes();
        Storyboard.SetTarget(sx, composite);
        Storyboard.SetTarget(sy, composite);
        Storyboard.SetTargetProperty(sx, nameof(CompositeTransform.ScaleX));
        Storyboard.SetTargetProperty(sy, nameof(CompositeTransform.ScaleY));
        pulse.Children.Add(sx);
        pulse.Children.Add(sy);
        pulse.Completed += (_, _) =>
        {
            composite.ScaleX = 1;
            composite.ScaleY = 1;
        };
        pulse.Begin();
    }

    private static DoubleAnimationUsingKeyFrames BuildHaloScaleKeyframes()
    {
        var anim = new DoubleAnimationUsingKeyFrames
        {
            Duration = new Duration(TimeSpan.FromMilliseconds(220)),
        };
        anim.KeyFrames.Add(new LinearDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
            Value = 0.95,
        });
        anim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(110)),
            Value = 1.08,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        });
        anim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(220)),
            Value = 1.0,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn },
        });
        return anim;
    }

    private void TryScrollMessagesToEnd()
    {
        if (!double.IsFinite(ThreadScroller.ScrollableHeight))
        {
            return;
        }

        ThreadScroller.ChangeView(null, ThreadScroller.ScrollableHeight, null, false);
    }

    private void OnDraftChanged(object sender, TextChangedEventArgs e) =>
        SendButton.IsEnabled = !string.IsNullOrWhiteSpace(DraftBox.Text);

    private void OnSendClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DraftBox.Text))
        {
            return;
        }

        DraftBox.Text = string.Empty;
        ToastRelay.Show("Send in a future lab");
        SendButton.IsEnabled = false;
    }
}
