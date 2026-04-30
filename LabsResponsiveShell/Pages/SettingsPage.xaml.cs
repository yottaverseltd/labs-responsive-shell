namespace LabsResponsiveShell.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContext = AppSettings.Current;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var sound = AppSettings.Current.SoundPreset;
        SoundSubtleOption.IsChecked = sound == AuroraSoundPreset.Subtle;
        SoundFullOption.IsChecked = sound == AuroraSoundPreset.Full;
        SoundOffOption.IsChecked = sound == AuroraSoundPreset.Off;

        var lastSeen = AppSettings.Current.LastSeenAudience;
        LastSeenEveryoneOption.IsChecked = lastSeen == LastSeenAudience.Everyone;
        LastSeenContactsOption.IsChecked = lastSeen == LastSeenAudience.Contacts;
        LastSeenNobodyOption.IsChecked = lastSeen == LastSeenAudience.Nobody;
    }

    private void OnSoundChanged(object sender, RoutedEventArgs e)
    {
        AppSettings.Current.SoundPreset = sender == SoundFullOption
            ? AuroraSoundPreset.Full
            : sender == SoundOffOption
                ? AuroraSoundPreset.Off
                : AuroraSoundPreset.Subtle;
    }

    private void OnLastSeenChanged(object sender, RoutedEventArgs e)
    {
        AppSettings.Current.LastSeenAudience = sender == LastSeenEveryoneOption
            ? LastSeenAudience.Everyone
            : sender == LastSeenNobodyOption
                ? LastSeenAudience.Nobody
                : LastSeenAudience.Contacts;
    }
}
