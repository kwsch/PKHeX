using System.Net.Http.Json;
using Microsoft.JSInterop;
using PKHeX.Web.Services.Diagnostics;

namespace PKHeX.Web.Services.Sprites;

/// <summary>Whether sprites are shown.</summary>
public enum SpriteCatalogState
{
    /// <summary>This build was published without the atlas; slots are shown as text.</summary>
    NotIncluded,

    /// <summary>The atlas and its stylesheet are resident; sprites are shown.</summary>
    Loaded,

    /// <summary>The atlas is part of this build but could not be loaded; slots are shown as text.</summary>
    Failed,
}

/// <summary>
/// The sprite atlas, loaded once at startup before any file can be chosen.
/// </summary>
/// <remarks>
/// <para>
/// Same-origin requests still reach the host's logs, so a request for one species' image after a save is opened would tell the host
/// what the save holds. The whole atlas is therefore fetched before the app renders (see <c>Program.cs</c>): the manifest, the generated
/// stylesheet and the one atlas image, identical for every save. Every sprite is then drawn from that resident image; nothing is
/// requested per entity or at all after startup.
/// </para>
/// <para>
/// If any part fails to load, or has not loaded within <see cref="LoadTimeout"/>, the catalog is <see cref="SpriteCatalogState.Failed"/> and
/// no sprite is ever drawn, because drawing one would request the image again. Slots are then shown as text, as in a build without sprites.
/// The app waits for the catalog before it renders, so the time limit keeps a stalled request from leaving it on its loading message.
/// </para>
/// </remarks>
/// <param name="http">Fetches the manifest from the app's own origin.</param>
/// <param name="js">Imports <c>browser.js</c> to preload the stylesheet and atlas.</param>
/// <param name="diagnostics">Records a failed load, redacted, for the diagnostic report; none in tests that do not need it.</param>
public sealed class SpriteCatalog(HttpClient http, IJSRuntime js, DiagnosticLog? diagnostics = null)
{
    /// <summary>
    /// How long the manifest, stylesheet and atlas (about 1.3 MiB together) may take to load before the app starts without sprites.
    /// A request still running then may finish later, but it was made at startup and is the same for every save.
    /// </summary>
    public static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(20);

    /// <summary>Whether sprites are shown.</summary>
    public SpriteCatalogState State { get; private set; } = SpriteCatalogState.NotIncluded;

    /// <summary>The loaded sheet, or null unless <see cref="State"/> is <see cref="SpriteCatalogState.Loaded"/>.</summary>
    public SpriteSheet? Sheet { get; private set; }

    /// <summary>
    /// Loads the atlas when this build includes it; does nothing otherwise. Never throws: a failure leaves <see cref="SpriteCatalogState.Failed"/>.
    /// </summary>
    public Task LoadAsync() => LoadAsync(BuildInfo.SpritesIncluded);

    /// <summary>Loads the atlas when <paramref name="included"/>; for <see cref="LoadAsync()"/>, and for tests of a build with sprites.</summary>
    internal async Task LoadAsync(bool included)
    {
        if (!included)
        {
            return;
        }
        using var timeout = new CancellationTokenSource(LoadTimeout);
        try
        {
            var manifest = await http.GetFromJsonAsync(SpriteSheet.ManifestPath, SpriteManifestJson.Default.SpriteManifest, timeout.Token)
                ?? throw new InvalidDataException("The sprite manifest is empty.");
            var sheet = SpriteSheet.From(manifest);
            await using var module = await js.InvokeAsync<IJSObjectReference>("import", timeout.Token, "./browser.js");
            await module.InvokeVoidAsync("preloadSprites", timeout.Token, sheet.StylesheetPath, sheet.ImagePath);
            Use(sheet);
        }
        catch (Exception e) when (e is not OutOfMemoryException)
        {
            State = SpriteCatalogState.Failed;
            // Recorded, redacted, for the diagnostic report and the console. Nothing about a save exists yet.
            diagnostics?.Record(DiagnosticOperation.Sprites, DiagnosticCode.FromException("sprites.not-loaded", e));
        }
    }

    /// <summary>Makes <paramref name="sheet"/> the loaded sheet. For <see cref="LoadAsync"/> and tests.</summary>
    internal void Use(SpriteSheet sheet)
    {
        Sheet = sheet;
        State = SpriteCatalogState.Loaded;
    }

    /// <summary>The cells of <paramref name="slot"/>'s sprite, or null when sprites are not shown or the slot has none.</summary>
    public SpriteCells? Resolve(SlotSummary slot) => Sheet?.Resolve(slot);
}
