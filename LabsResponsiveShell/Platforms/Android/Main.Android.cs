using System;
using Android.App;
using Android.Runtime;

namespace LabsResponsiveShell.Droid;

[Application(
    Label = "@string/ApplicationName",
    Icon = "@mipmap/icon",
    LargeHeap = true,
    HardwareAccelerated = true,
    Theme = "@style/AppTheme",
    UsesCleartextTraffic = true)]
public class Application : Microsoft.UI.Xaml.NativeApplication
{
    public Application(IntPtr javaReference, JniHandleOwnership transfer)
        : base(() => new global::LabsResponsiveShell.App(), javaReference, transfer)
    {
    }
}
