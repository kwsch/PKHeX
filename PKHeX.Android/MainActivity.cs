using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls.ApplicationLifetimes;

namespace PKHeX.Android;

[Activity(
    Label = "PKHeX",
    Theme = "@style/MainTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public sealed class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        Log.Info("PKHEX_ANDROID", "MainActivity.OnCreate: before Avalonia base");
        AndroidHostContext.SetActivity(this);
        base.OnCreate(savedInstanceState);
        Log.Info("PKHEX_ANDROID", "MainActivity.OnCreate: after Avalonia base");

        // The Android activity can be recreated while the Avalonia Application survives. Replace
        // the single-view lifetime's root so an old Activity-owned view is never reused.
        if (App.Current?.ApplicationLifetime is ISingleViewApplicationLifetime activity
            && App.Current is App app)
        {
            Log.Info("PKHEX_ANDROID", "MainActivity.OnCreate: replacing single-view root");
            activity.MainView = app.CreateMainView();
        }
        else
        {
            Log.Warn("PKHEX_ANDROID", "MainActivity.OnCreate: no single-view lifetime available");
        }
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        => base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .LogToTrace();
}
