using LabsResponsiveShell.Data;
using LabsResponsiveShell.Motion;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI.Text;

namespace LabsResponsiveShell.Controls;

public sealed partial class ConversationRowHost : UserControl
{
    public static readonly DependencyProperty ConversationProperty = DependencyProperty.Register(
        nameof(Conversation),
        typeof(MockConversation),
        typeof(ConversationRowHost),
        new PropertyMetadata(null, OnConversationChanged));

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(ConversationRowHost),
        new PropertyMetadata(false, OnIsSelectedChanged));

    private bool _hover;

    public event EventHandler<MockConversation?>? ConversationActivated;

    public ConversationRowHost()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public MockConversation? Conversation
    {
        get => (MockConversation?)GetValue(ConversationProperty);
        set => SetValue(ConversationProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private static void OnConversationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ConversationRowHost)d).ApplyConversation();
    }

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ConversationRowHost)d).ApplySelectionAccent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        PointerEntered += RowPointerEntered;
        PointerExited += RowPointerExited;
        RowRoot.PointerReleased += RowRootOnPointerReleased;
        ApplyConversation();
        ApplySelectionAccent();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        PointerEntered -= RowPointerEntered;
        PointerExited -= RowPointerExited;
        RowRoot.PointerReleased -= RowRootOnPointerReleased;
    }

    private void RowRootOnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (Conversation is { } c)
        {
            ConversationActivated?.Invoke(this, c);
        }
    }

    private void RowPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _hover = true;
        RefreshBackground();
    }

    private void RowPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _hover = false;
        RefreshBackground();
    }

    private void ApplyConversation()
    {
        var conv = Conversation;
        if (conv is null)
        {
            return;
        }

        TitleBlock.Text = conv.Title;
        PreviewBlock.Text = conv.IsTyping ? "typing\u2026" : conv.Preview;
        PreviewBlock.FontStyle = conv.IsTyping ? FontStyle.Italic : FontStyle.Normal;
        WhenBlock.Text = conv.RelativeTimeLabel;
        PinGlyph.Visibility = conv.IsPinned ? Visibility.Visible : Visibility.Collapsed;

        var initials = InitialsFormatter.FromTitle(conv.Title);
        AvatarInitials.Text = initials;

        var idx = AvatarPalette.IndexFor(conv.Title);
        AvatarDisk.Fill = (Brush)(Application.Current.Resources[$"AvatarTone{idx}Brush"] ?? new SolidColorBrush());

        if (conv.IsGroup || conv.Presence == PresenceGlyph.None)
        {
            PresenceDot.Visibility = Visibility.Collapsed;
        }
        else
        {
            PresenceDot.Visibility = Visibility.Visible;
            PresenceDot.Fill = conv.Presence == PresenceGlyph.Online
                ? (Brush)(Application.Current.Resources["PresenceOnlineBrush"] ?? new SolidColorBrush())
                : (Brush)(Application.Current.Resources["PresenceAwayBrush"] ?? new SolidColorBrush());
        }

        if (conv.UnreadCount > 0)
        {
            UnreadPill.Visibility = Visibility.Visible;
            UnreadCountText.Text = conv.UnreadCount > 99 ? "99+" : $"{conv.UnreadCount}";
        }
        else
        {
            UnreadPill.Visibility = Visibility.Collapsed;
        }

    }

    private void RefreshBackground()
    {
        if (IsSelected)
        {
            RowRoot.Background = (Brush)(Application.Current.Resources["ConversationRowSelectedBrush"]
                ?? new SolidColorBrush());

            return;
        }

        RowRoot.Background = _hover
            ? (Brush)(Application.Current.Resources["HoverBrush"] ?? new SolidColorBrush())
            : (Brush)(Application.Current.Resources["TransparentChromeBrush"] ?? new SolidColorBrush());
    }

    private void ApplySelectionAccent()
    {
        RefreshBackground();
        var fast = ReducedMotion.IsReducedMotion ? TimeSpan.Zero : TimeSpan.FromMilliseconds(120);

        if (IsSelected)
        {
            if (fast == TimeSpan.Zero)
            {
                SelectionAccent.Opacity = 1;
                if (SelectionAccent.RenderTransform is TranslateTransform t0)
                {
                    t0.Y = 0;
                }

                return;
            }

            var slide = new Storyboard();
            var move = new DoubleAnimation
            {
                From = 6,
                To = 0,
                Duration = new Duration(fast),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            };
            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(fast),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            };
            var translate = (TranslateTransform)SelectionAccent.RenderTransform;
            Storyboard.SetTarget(move, translate);
            Storyboard.SetTargetProperty(move, "Y");
            Storyboard.SetTarget(fade, SelectionAccent);
            Storyboard.SetTargetProperty(fade, "Opacity");
            slide.Children.Add(move);
            slide.Children.Add(fade);
            slide.Completed += (_, _) => translate.Y = 0;
            slide.Begin();
            return;
        }

        SelectionAccent.Opacity = 0;
        if (SelectionAccent.RenderTransform is TranslateTransform t)
        {
            t.Y = 6;
        }
    }
}