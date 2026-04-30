namespace LabsResponsiveShell.Pages;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        DataContext = AppSettings.Current;
    }
}
