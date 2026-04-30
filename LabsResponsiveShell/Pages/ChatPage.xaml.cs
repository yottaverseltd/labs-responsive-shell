using System.ComponentModel;

namespace LabsResponsiveShell.Pages;

public sealed partial class ChatPage : Page
{
    internal ChatViewModel ViewModel { get; } = new();

    public ChatPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => ApplyLayout();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => ApplyLayout();

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsNarrow() && ViewModel.Selected is not null)
        {
            NarrowList.Visibility = Visibility.Collapsed;
            NarrowThread.Visibility = Visibility.Visible;
        }
    }

    private void OnBackToList(object sender, RoutedEventArgs e)
    {
        ViewModel.Selected = null;
        NarrowThread.Visibility = Visibility.Collapsed;
        NarrowList.Visibility = Visibility.Visible;
    }

    private void ApplyLayout()
    {
        if (IsNarrow())
        {
            WideLayout.Visibility = Visibility.Collapsed;
            if (ViewModel.Selected is null)
            {
                NarrowList.Visibility = Visibility.Visible;
                NarrowThread.Visibility = Visibility.Collapsed;
            }
            else
            {
                NarrowList.Visibility = Visibility.Collapsed;
                NarrowThread.Visibility = Visibility.Visible;
            }
        }
        else
        {
            WideLayout.Visibility = Visibility.Visible;
            NarrowList.Visibility = Visibility.Collapsed;
            NarrowThread.Visibility = Visibility.Collapsed;
        }
    }

    private bool IsNarrow() => ActualWidth < 768;
}

internal sealed class ChatViewModel : INotifyPropertyChanged
{
    public IReadOnlyList<Conversation> Conversations { get; } = ChatMocks.Build();

    private Conversation? _selected;
    public Conversation? Selected
    {
        get => _selected;
        set
        {
            if (ReferenceEquals(_selected, value))
            {
                return;
            }

            _selected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class Conversation
{
    public string Name { get; init; } = string.Empty;
    public string Preview { get; init; } = string.Empty;
    public string When { get; init; } = string.Empty;
    public IReadOnlyList<Message> Messages { get; init; } = [];
}

public sealed class Message
{
    public string Body { get; init; } = string.Empty;
    public string When { get; init; } = string.Empty;
    public bool IsFromMe { get; init; }

    public HorizontalAlignment Alignment =>
        IsFromMe ? HorizontalAlignment.Right : HorizontalAlignment.Left;

    public Microsoft.UI.Xaml.Media.Brush BubbleBrush =>
        Application.Current.Resources[IsFromMe ? "AccentMutedBrush" : "SurfaceElevatedBrush"] as Microsoft.UI.Xaml.Media.Brush
        ?? new Microsoft.UI.Xaml.Media.SolidColorBrush();
}

internal static class ChatMocks
{
    public static IReadOnlyList<Conversation> Build() =>
    [
        new Conversation
        {
            Name = "Priya Raman",
            Preview = "ok the schema works. Shipping the migration tonight.",
            When = "12:41",
            Messages =
            [
                new Message { Body = "Could you eyeball the migration order once?", When = "12:36", IsFromMe = false },
                new Message { Body = "Yes. The FK on user_id has to wait until the backfill completes.", When = "12:38", IsFromMe = true },
                new Message { Body = "ok the schema works. Shipping the migration tonight.", When = "12:41", IsFromMe = false },
            ],
        },
        new Conversation
        {
            Name = "Design review",
            Preview = "Aurora tokens are in. Dropped glass behind the rail only.",
            When = "11:02",
            Messages =
            [
                new Message { Body = "The accent felt too hot at 100%. Pulled back to 70.", When = "10:58", IsFromMe = false },
                new Message { Body = "Agree. The muted variant reads better on the narrow layout.", When = "11:00", IsFromMe = true },
                new Message { Body = "Aurora tokens are in. Dropped glass behind the rail only.", When = "11:02", IsFromMe = false },
            ],
        },
        new Conversation
        {
            Name = "Kai",
            Preview = "lunch?",
            When = "Yesterday",
            Messages =
            [
                new Message { Body = "lunch?", When = "Yesterday", IsFromMe = false },
                new Message { Body = "1pm, the usual spot.", When = "Yesterday", IsFromMe = true },
            ],
        },
    ];
}
