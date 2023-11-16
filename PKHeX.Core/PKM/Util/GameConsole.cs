namespace PKHeX.Core;

/// <summary>
/// Hardware console types.
/// </summary>
/// <remarks>
/// Related to <see cref="EntityContext"/>; no need to specify side-game consoles like the N64 as they're tied to the mainline console.
/// Console revisions (like Game Boy Color) or 3DS-XL are not included, again, only care about console limitations that run the games.
/// </remarks>
public enum GameConsole : byte
{
    /// <summary> Invalid console type. </summary>
    None,

    /// <summary> Nintendo GameBoy </summary>
    GB,
    /// <summary> Nintendo GameBoy Advance </summary>
    GBA,
    /// <summary> Nintendo GameCube </summary>
    GC = GBA,
    /// <summary> Nintendo DS </summary>
    NDS,
    /// <summary> Nintendo 3DS </summary>
    _3DS,
    /// <summary> Nintendo Switch </summary>
    NX,
}
