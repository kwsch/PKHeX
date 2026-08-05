using Avalonia.Controls;

namespace PKHeX.Android;

/// <summary>Owns the Android activity and single-view host references used by Android adapters.</summary>
internal static class AndroidHostContext
{
    public static MainActivity? Activity { get; private set; }
    public static MainView? MainView { get; private set; }

    /// <summary>The live overlay host, so the activity's Back handling can dismiss overlays.</summary>
    public static AndroidWindowService? WindowService { get; private set; }

    public static void SetActivity(MainActivity activity) => Activity = activity;

    public static void SetMainView(MainView view) => MainView = view;

    public static void SetWindowService(AndroidWindowService service) => WindowService = service;

    public static TopLevel? GetTopLevel()
        => MainView is null ? null : TopLevel.GetTopLevel(MainView);
}
