namespace LabsResponsiveShell.Shell;

public sealed partial class IdentityChip : UserControl
{
    public IdentityChip()
    {
        InitializeComponent();
        DataContext = AppSettings.Current;
    }
}
