using PKHeX.Core;

namespace PKHeX.Avalonia.Tests.Fixtures;

/// <summary>
/// Generates populated save files for game versions where real save files aren't available.
/// Injects Pokemon into boxes and party for realistic testing.
/// </summary>
public static class PopulatedSaveGenerator
{
    private static readonly (GameVersion Version, string FileName)[] GapVersions =
    [
        (GameVersion.GP, "gen7b_letsgopikachu.bin"),
        (GameVersion.SW, "gen8_sword.bin"),
    ];

    /// <summary>
    /// Species to inject into save files for testing.
    /// Covers various edge cases: starters, legendaries, forms.
    /// </summary>
    private static readonly ushort[] TestSpecies = [25, 1, 4, 7, 150, 151]; // Pikachu, Bulbasaur, Charmander, Squirtle, Mewtwo, Mew

    /// <summary>
    /// Generates populated saves for versions without real save files.
    /// Writes them to the savefiles directory. Returns paths of generated files.
    /// </summary>
    public static List<string> GenerateIfMissing(string saveDir)
    {
        var generated = new List<string>();

        foreach (var (version, fileName) in GapVersions)
        {
            var path = Path.Combine(saveDir, fileName);
            if (File.Exists(path))
                continue;

            try
            {
                var sav = GeneratePopulatedSave(version);
                if (sav == null) continue;

                var data = sav.Write().ToArray();
                File.WriteAllBytes(path, data);
                generated.Add(path);
            }
            catch
            {
                // Skip if this version can't generate a save
            }
        }

        return generated;
    }

    /// <summary>
    /// Creates a blank save and populates it with test Pokemon.
    /// </summary>
    public static SaveFile? GeneratePopulatedSave(GameVersion version)
    {
        SaveFile sav;
        try
        {
            sav = BlankSaveFile.Get(version);
        }
        catch
        {
            return null;
        }

        // Set trainer info
        sav.OT = "PKHeXTest";
        sav.Language = (int)LanguageID.English;

        // Populate box slots with test Pokemon
        int slotIdx = 0;
        foreach (var species in TestSpecies)
        {
            if (slotIdx >= sav.BoxSlotCount) break;

            try
            {
                var pk = CreatePKM(sav, species);
                if (pk != null)
                {
                    sav.SetBoxSlotAtIndex(pk, slotIdx);
                    slotIdx++;
                }
            }
            catch
            {
                // Some species may not be valid for this generation
            }
        }

        return sav;
    }

    private static PKM? CreatePKM(SaveFile sav, ushort species)
    {
        var pk = sav.BlankPKM;
        pk.Species = species;

        // Check if species is valid for this save
        if (species > sav.Personal.MaxSpeciesID)
            return null;

        if (!sav.Personal.IsPresentInGame(species, 0))
            return null;

        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(species, (int)LanguageID.English, sav.Generation);

        // Set minimal valid data
        if (sav.Generation >= 3)
            pk.Move1 = 33; // Tackle

        pk.CurrentLevel = 50;

        if (pk is ITrainerID32 tid32)
        {
            tid32.ID32 = sav.ID32;
        }

        pk.OriginalTrainerName = sav.OT;

        return pk;
    }
}
