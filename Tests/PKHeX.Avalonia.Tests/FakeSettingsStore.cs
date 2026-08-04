namespace PKHeX.Avalonia.Tests;

/// <summary>In-memory <see cref="ISettingsStore"/> for tests that don't exercise persistence.</summary>
internal sealed class FakeSettingsStore : ISettingsStore
{
    public AppSettings? Saved { get; private set; }
    public AppSettings ToLoad { get; set; } = new();

    public AppSettings Load() => ToLoad;
    public void Save(AppSettings settings) => Saved = settings;
}
