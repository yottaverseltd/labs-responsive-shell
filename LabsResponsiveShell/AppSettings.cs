using System.ComponentModel;
using System.Runtime.CompilerServices;
using LabsResponsiveShell.Motion;

namespace LabsResponsiveShell;

public sealed class AppSettings : INotifyPropertyChanged
{
    public static AppSettings Current { get; } = new();

    private ElementTheme _theme = ElementTheme.Dark;
    private bool _rightToLeft;
    private bool _reducedMotion;

    public ElementTheme Theme
    {
        get => _theme;
        set
        {
            if (Set(ref _theme, value))
            {
                Raise(nameof(IsDarkTheme));
            }
        }
    }

    public bool IsDarkTheme
    {
        get => _theme == ElementTheme.Dark;
        set => Theme = value ? ElementTheme.Dark : ElementTheme.Light;
    }

    public bool IsRightToLeft
    {
        get => _rightToLeft;
        set
        {
            if (Set(ref _rightToLeft, value))
            {
                Raise(nameof(FlowDirection));
            }
        }
    }

    public bool IsReducedMotion
    {
        get => _reducedMotion;
        set
        {
            if (Set(ref _reducedMotion, value))
            {
                ReducedMotion.Set(value);
            }
        }
    }

    public FlowDirection FlowDirection =>
        _rightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        Raise(name);
        return true;
    }

    private void Raise(string? name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
