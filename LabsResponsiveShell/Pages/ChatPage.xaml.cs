using LabsResponsiveShell.Controls;
using LabsResponsiveShell.Data;

namespace LabsResponsiveShell.Pages;

public sealed partial class ChatPage : Page
{
    private static string? s_pendingConversationId;

    private readonly List<ConversationRowHost> _rows = [];
    private MockConversation? _selected;
    private bool _narrowInThread;
    private bool _hydrated;

    public ChatPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
        SizeChanged += OnSizeChanged;
    }

    public static void RequestOpenConversation(string conversationId) => s_pendingConversationId = conversationId;

    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        if (!_hydrated)
        {
            HydrateLists();
            _hydrated = true;
        }

        if (s_pendingConversationId is { } pick)
        {
            var found = MockConversations.Find(pick);
            s_pendingConversationId = null;
            if (found is not null)
            {
                ActivateConversation(found, openNarrowThread: !IsWide());
            }
            else
            {
                SelectDefaultConversation();
            }
        }
        else if (_selected is null)
        {
            SelectDefaultConversation();
        }

        ApplyLayout();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => ApplyLayout();

    private bool IsWide() => ActualWidth >= 900;

    private void HydrateLists()
    {
        foreach (var conv in MockConversations.GetAll())
        {
            var wideRow = new ConversationRowHost { Conversation = conv };
            wideRow.ConversationActivated += OnRowActivated;
            WideRowStack.Children.Add(wideRow);
            _rows.Add(wideRow);

            var narrowRow = new ConversationRowHost { Conversation = conv };
            narrowRow.ConversationActivated += OnRowActivated;
            NarrowRowStack.Children.Add(narrowRow);
            _rows.Add(narrowRow);
        }
    }

    private void SelectDefaultConversation()
    {
        var all = MockConversations.GetAll();
        MockConversation? pick = null;
        for (var i = 0; i < all.Count; i++)
        {
            if (all[i].IsPinned)
            {
                pick = all[i];
                break;
            }
        }

        pick ??= all.Count > 0 ? all[0] : null;
        if (pick is not null)
        {
            ActivateConversation(pick, openNarrowThread: false);
        }
    }

    private void OnRowActivated(object? sender, MockConversation? conv)
    {
        if (conv is null)
        {
            return;
        }

        ActivateConversation(conv, openNarrowThread: !IsWide());
    }

    private void ActivateConversation(MockConversation conv, bool openNarrowThread)
    {
        _selected = conv;
        foreach (var row in _rows)
        {
            row.IsSelected = row.Conversation?.Id == conv.Id;
        }

        if (IsWide())
        {
            _narrowInThread = false;
            WideThreadChrome.Conversation = conv;
            return;
        }

        if (openNarrowThread)
        {
            _narrowInThread = true;
            NarrowListPane.Visibility = Visibility.Collapsed;
            NarrowThreadPane.Visibility = Visibility.Visible;
            _ = NarrowThreadFrame.Navigate(
                typeof(ChatThreadPage),
                new MobileThreadOpenArgs(conv, CloseNarrowThread));
        }
    }

    private void CloseNarrowThread()
    {
        _narrowInThread = false;
        NarrowThreadPane.Visibility = Visibility.Collapsed;
        NarrowListPane.Visibility = Visibility.Visible;
    }

    private void ApplyLayout()
    {
        if (_selected is null)
        {
            return;
        }

        if (IsWide())
        {
            WideChrome.Visibility = Visibility.Visible;
            NarrowListPane.Visibility = Visibility.Collapsed;
            NarrowThreadPane.Visibility = Visibility.Collapsed;
            WideThreadChrome.Conversation = _selected;
            _narrowInThread = false;
            return;
        }

        WideChrome.Visibility = Visibility.Collapsed;
        if (_narrowInThread)
        {
            NarrowListPane.Visibility = Visibility.Collapsed;
            NarrowThreadPane.Visibility = Visibility.Visible;
        }
        else
        {
            NarrowListPane.Visibility = Visibility.Visible;
            NarrowThreadPane.Visibility = Visibility.Collapsed;
        }
    }
}
