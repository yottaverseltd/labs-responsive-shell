using LabsResponsiveShell.Motion;

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
        MotionPrimitives.PlayMount(PreviewStack);
    }
}
