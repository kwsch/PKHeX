using PKHeX.Core;

namespace PKHeX.Avalonia.Tests.Harness;

/// <summary>
/// Deterministic in-memory save fixtures for the headless harness. Builds a blank Gen-3 (Emerald)
/// save with a known Pokémon in box 0, slot 0 — so a smoke test always has a save with a
/// guaranteed-occupied slot, independent of whether the downloaded <c>Tests/savefiles</c> corpus is
/// present or what it contains.
/// </summary>
/// <remarks>
/// The save is adopted in-memory via <see cref="HeadlessAppFixture.LoadSaveInstance"/> rather than
/// written to disk and reloaded: PKHeX.Core cannot re-serialize a <em>blank</em> SAV3 (its sector
/// metadata isn't initialized, so <c>SAV3.Write()</c> throws). Adopting the instance still drives the
/// exact same <c>MainWindowViewModel</c> load pipeline that a file open does. The file-based load path
/// is covered by <c>AppBoots_SaveLoads_BoxGridPopulates</c> against a real committed save.
/// </remarks>
public static class HeadlessSaveFixtures
{
    /// <summary>
    /// A blank Emerald save with <paramref name="species"/> (default Pikachu) in box 0/slot 0.
    /// </summary>
    public static SaveFile CreatePopulatedEmeraldSave(ushort species = (ushort)Core.Species.Pikachu)
    {
        var sav = BlankSaveFile.Get(GameVersion.E);

        var pk = sav.BlankPKM;
        pk.Species = species;
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(species, (int)LanguageID.English, sav.Generation);
        pk.Move1 = 33; // Tackle — keeps the entity parseable by summary/legality code paths
        sav.SetBoxSlotAtIndex(pk, 0, 0);

        return sav;
    }
}
