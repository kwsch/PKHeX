using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls.ApplicationLifetimes;
using AndroidX.Activity;

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

        OnBackPressedDispatcher.AddCallback(this, new OverlayBackCallback(this));
    }

    /// <summary>
    /// Back dismisses the top-most tool/dialog overlay instead of leaving the app, which is what
    /// an Android user expects from a full-screen sheet. Only when nothing is open does Back fall
    /// through to the platform default.
    /// </summary>
    /// <remarks>
    /// This must go through <see cref="OnBackPressedDispatcher"/>, not an <c>OnBackPressed</c>
    /// override: AndroidX registers an OnBackInvokedCallback, so on API 33+ the framework routes
    /// the gesture to the dispatcher and the legacy activity callback is never invoked.
    /// </remarks>
    private sealed class OverlayBackCallback(MainActivity activity) : OnBackPressedCallback(true)
    {
        public override void HandleOnBackPressed()
        {
            if (AndroidHostContext.WindowService?.TryCloseTopOverlay() == true)
            {
                Log.Info("PKHEX_ANDROID", "Back: dismissed overlay");
                return;
            }

            // Nothing of ours to close — step aside and let the platform default run.
            Enabled = false;
            activity.OnBackPressedDispatcher.OnBackPressed();
            Enabled = true;
        }
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        => base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .LogToTrace();
}
