namespace LabsResponsiveShell.Data;

public static class ToastRelay
{
    public static event EventHandler<string>? Requested;

    public static void Show(string message) => Requested?.Invoke(null, message);
}
