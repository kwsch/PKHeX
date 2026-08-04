using PKHeX.Core;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Factory that generates blank save files for every supported generation using
/// PKHeX.Core's <see cref="BlankSaveFile"/> API. No real save files needed.
/// </summary>
public static class SaveFileFactory
{
    /// <summary>
    /// All game versions that can produce a blank save and a valid PKM for testing.
    /// </summary>
    private static readonly (GameVersion Version, string Label)[] SupportedVersions =
    [
        (GameVersion.RD, "Gen1-Red"),
        (GameVersion.C, "Gen2-Crystal"),
        (GameVersion.E, "Gen3-Emerald"),
        (GameVersion.FR, "Gen3-FireRed"),
        (GameVersion.D, "Gen4-Diamond"),
        (GameVersion.Pt, "Gen4-Platinum"),
        (GameVersion.HG, "Gen4-HeartGold"),
        (GameVersion.B, "Gen5-Black"),
        (GameVersion.W2, "Gen5-White2"),
        (GameVersion.X, "Gen6-X"),
        (GameVersion.OR, "Gen6-OmegaRuby"),
        (GameVersion.SN, "Gen7-Sun"),
        (GameVersion.UM, "Gen7-UltraMoon"),
        (GameVersion.GP, "Gen7b-LetsGoPikachu"),
        (GameVersion.SW, "Gen8-Sword"),
        (GameVersion.BD, "Gen8b-BrilliantDiamond"),
        (GameVersion.PLA, "Gen8a-LegendsArceus"),
        (GameVersion.SL, "Gen9-Scarlet"),
        (GameVersion.ZA, "Gen9a-LegendsZA"),
    ];

    /// <summary>
    /// Creates a blank save file for the given version.
    /// </summary>
    public static SaveFile CreateBlankSave(GameVersion version)
    {
        return BlankSaveFile.Get(version);
    }

    /// <summary>
    /// Creates a blank PKM appropriate for the save file, with species set to Pikachu (25).
    /// </summary>
    public static PKM CreateTestPKM(SaveFile sav, ushort species = 25)
    {
        var pkm = sav.BlankPKM;
        pkm.Species = species;
        pkm.Nickname = SpeciesName.GetSpeciesNameGeneration(species, (int)LanguageID.English, sav.Generation);

        if (pkm is IHyperTrain)
        {
            // Don't set hypertrain; leave default
        }

        // Set minimal valid moves (Tackle = move 33)
        if (sav.Generation >= 3)
        {
            pkm.Move1 = 33;
        }

        return pkm;
    }

    /// <summary>
    /// xUnit MemberData source: yields (SaveFile, PKM, string label) for every supported version.
    /// </summary>
    public static IEnumerable<object[]> AllGenerations()
    {
        foreach (var (version, label) in SupportedVersions)
        {
            SaveFile sav;
            try
            {
                sav = CreateBlankSave(version);
            }
            catch
            {
                // Skip versions that can't create blank saves in this Core build
                continue;
            }

            var pkm = CreateTestPKM(sav);
            yield return [sav, pkm, label];
        }
    }

    /// <summary>
    /// xUnit MemberData source: yields only (SaveFile, string label) for save-level tests.
    /// </summary>
    public static IEnumerable<object[]> AllSaves()
    {
        foreach (var (version, label) in SupportedVersions)
        {
            SaveFile sav;
            try
            {
                sav = CreateBlankSave(version);
            }
            catch
            {
                continue;
            }

            yield return [sav, label];
        }
    }
}
