using LabsResponsiveShell.Controls;
using LabsResponsiveShell.Data;
using LabsResponsiveShell.Shell;

namespace LabsResponsiveShell.Pages;

public sealed partial class HomePage : Page
{
    private bool _hydrated;

    public HomePage()
    {
        InitializeComponent();
        DataContext = AppSettings.Current;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_hydrated)
        {
            return;
        }

        foreach (var conv in MockConversations.GetHomeRecent(4))
        {
            var row = new ConversationRowHost { Conversation = conv };
            row.ConversationActivated += OnRecentRowActivated;
            RecentStack.Children.Add(row);
        }

        _hydrated = true;
    }

    private void OnRecentRowActivated(object? sender, MockConversation? conv)
    {
        if (conv is null)
        {
            return;
        }

        ChatPage.RequestOpenConversation(conv.Id);
        AppShell.Current?.NavigateToChat();
    }

    private void OnViewAllClick(object sender, RoutedEventArgs e) =>
        AppShell.Current?.NavigateToChat();

    private void OnQuickActionClick(object sender, RoutedEventArgs e)
    {
        var label = (sender as FrameworkElement)?.Tag as string ?? "action";
        ToastRelay.Show($"{label} is coming in a later lab");
    }
}
