using LabsResponsiveShell.Data;

namespace LabsResponsiveShell.Pages;

/// <summary>Mobile drill-in wrapper with an explicit Back affordance.</summary>
public sealed partial class ChatThreadPage : Page
{
    private MobileThreadOpenArgs? _args;

    public ChatThreadPage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Disabled;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is not MobileThreadOpenArgs mo)
        {
            return;
        }

        _args = mo;
        Chrome.Conversation = mo.Conversation;
    }

    private void OnBackClick(object sender, RoutedEventArgs e) => _args?.RequestClose();
}
