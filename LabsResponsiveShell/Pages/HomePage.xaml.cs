namespace LabsResponsiveShell.Pages;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        DataContext = AppSettings.Current;
    }

    private void OnRemount(object sender, RoutedEventArgs e)
    {
        // why: toggling visibility forces a realize pass; commit 9 attaches a fade animation
        PreviewStack.Visibility = Visibility.Collapsed;
        PreviewStack.Visibility = Visibility.Visible;
    }
}
