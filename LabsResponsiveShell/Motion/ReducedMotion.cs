namespace LabsResponsiveShell.Motion;

public static class ReducedMotion
{
    public static bool IsReducedMotion { get; private set; }

    public static event EventHandler? Changed;

    public static void Initialize()
    {
#if __WASM__
        var matches = Uno.Foundation.WebAssemblyRuntime.InvokeJS(
            "window.matchMedia('(prefers-reduced-motion: reduce)').matches ? '1' : '0'");
        Set(matches == "1");
#else
        // why: Windows.UI.ViewManagement.UISettings is implemented on Skia Desktop via Uno
        try
        {
            var settings = new Windows.UI.ViewManagement.UISettings();
            Set(!settings.AnimationsEnabled);
        }
        catch
        {
            Set(false);
        }
#endif
    }

    public static void Set(bool value)
    {
        if (IsReducedMotion == value)
        {
            return;
        }

        IsReducedMotion = value;
        ClampMotionDurations(value);
        Changed?.Invoke(null, EventArgs.Empty);
    }

    private static void ClampMotionDurations(bool reduce)
    {
        if (Application.Current is null)
        {
            return;
        }

        var resources = Application.Current.Resources;
        if (reduce)
        {
            resources["MotionFast"] = new Duration(TimeSpan.Zero);
            resources["MotionDefault"] = new Duration(TimeSpan.Zero);
            resources["MotionSlow"] = new Duration(TimeSpan.Zero);
            resources["MotionTypingLoop"] = new Duration(TimeSpan.Zero);
            resources["MotionSkeletonFade"] = new Duration(TimeSpan.Zero);
            resources["MotionAvatarHalo"] = new Duration(TimeSpan.Zero);
            resources["MotionNavIndicatorSlide"] = new Duration(TimeSpan.Zero);
        }
        else
        {
            resources["MotionFast"] = new Duration(TimeSpan.FromMilliseconds(120));
            resources["MotionDefault"] = new Duration(TimeSpan.FromMilliseconds(200));
            resources["MotionSlow"] = new Duration(TimeSpan.FromMilliseconds(320));
            resources["MotionTypingLoop"] = new Duration(TimeSpan.FromMilliseconds(600));
            resources["MotionSkeletonFade"] = new Duration(TimeSpan.FromMilliseconds(200));
            resources["MotionAvatarHalo"] = new Duration(TimeSpan.FromMilliseconds(220));
            resources["MotionNavIndicatorSlide"] = new Duration(TimeSpan.FromMilliseconds(120));
        }
    }
}
