using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace MasterplanXP.UI.Android
{
    [Activity(
        Label = "MasterplanXP.UI.Android",
        Theme = "@style/MyTheme.NoActionBar",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : AvaloniaMainActivity
    {
    }
}
