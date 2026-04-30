using Microsoft.UI.Xaml.Media.Animation;

namespace LabsResponsiveShell.Motion;

public static class MotionPrimitives
{
    // why: these reads are live; ReducedMotion.Set swaps the effective duration between plays
    public static TimeSpan Fast => ReducedMotion.IsReducedMotion ? TimeSpan.Zero : TimeSpan.FromMilliseconds(120);

    public static TimeSpan Default => ReducedMotion.IsReducedMotion ? TimeSpan.Zero : TimeSpan.FromMilliseconds(200);

    public static TimeSpan Slow => ReducedMotion.IsReducedMotion ? TimeSpan.Zero : TimeSpan.FromMilliseconds(320);

    public static void PlayMount(FrameworkElement element)
    {
        if (element is null)
        {
            return;
        }

        var duration = Default;
        var translate = EnsureTranslate(element);

        if (duration == TimeSpan.Zero)
        {
            element.Opacity = 1;
            translate.Y = 0;
            return;
        }

        element.Opacity = 0;
        translate.Y = 12;

        var storyboard = new Storyboard();

        var opacityAnim = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(duration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(opacityAnim, element);
        Storyboard.SetTargetProperty(opacityAnim, "Opacity");

        var translateAnim = new DoubleAnimation
        {
            From = 12,
            To = 0,
            Duration = new Duration(duration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(translateAnim, translate);
        Storyboard.SetTargetProperty(translateAnim, "Y");

        storyboard.Children.Add(opacityAnim);
        storyboard.Children.Add(translateAnim);
        storyboard.Begin();
    }

    public static void PlayIndicatorSlide(UIElement element, bool appear)
    {
        if (element is null)
        {
            return;
        }

        var duration = Default;
        var translate = EnsureTranslate(element);

        if (duration == TimeSpan.Zero)
        {
            element.Opacity = appear ? 1 : 0;
            translate.Y = 0;
            return;
        }

        var fromOpacity = element.Opacity;
        var toOpacity = appear ? 1.0 : 0.0;
        var fromY = appear ? -4.0 : 0.0;
        var toY = appear ? 0.0 : 4.0;

        translate.Y = fromY;

        var storyboard = new Storyboard();

        var opacityAnim = new DoubleAnimation
        {
            From = fromOpacity,
            To = toOpacity,
            Duration = new Duration(duration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(opacityAnim, element);
        Storyboard.SetTargetProperty(opacityAnim, "Opacity");

        var translateAnim = new DoubleAnimation
        {
            From = fromY,
            To = toY,
            Duration = new Duration(duration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(translateAnim, translate);
        Storyboard.SetTargetProperty(translateAnim, "Y");

        storyboard.Children.Add(opacityAnim);
        storyboard.Children.Add(translateAnim);
        storyboard.Begin();
    }

    private static TranslateTransform EnsureTranslate(UIElement element)
    {
        if (element.RenderTransform is TranslateTransform existing)
        {
            return existing;
        }

        var fresh = new TranslateTransform();
        element.RenderTransform = fresh;
        return fresh;
    }
}
