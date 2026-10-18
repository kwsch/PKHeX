using System;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Long-jump converters from past formats to the current <see cref="PKH"/> format.
/// </summary>
public static class HomeConverter
{
    /// <summary>
    /// Upgrades a Generation 3 Pokémon into HOME's core data and SWSH side data.
    /// The caller is expected to pass a fresh/prepared <see cref="PKH"/> with no conflicting side-game data.
    /// </summary>
    public static void UpgradeFromPastFormat(PK3 pk3, PKH pkh)
    {
        // The HOME conversion path represented by this routine is specifically FR/LG.
        // Try to roughly match the disassembly; some liberty has been taken to sanitize rather than throw, and skip any no-ops.
        // The order of setup might also be different from the disassembly, but the end result should be equivalent.
        var version = GetTransferVersion3(pk3.Version);

        // Bad egg flags(?) or error codes: invalid version, species, language, marking, ball.
        // Rather than throw exceptions or return a bad egg, skip conversion.
        // For the values we can sanitize, we do so below. For the values we cannot sanitize, we skip conversion.
        if (version is 0)
            return;

        var species = pk3.Species;
        if (species == 0)
            return;

        // Convenience: sanitize them (my preference)
        var ball = pk3.Ball;
        if (ball is 0 or > (byte)Ball.Premier)
            ball = (byte)Ball.Poke; // official logic returns ball:=0, and bad-egg's if 0.

        var language = pk3.Language;
        if (language is not (1 or 2 or 3 or 4 or 5 or 7))
            language = 2;

        var core = pkh.Core;
        var side = pkh.InitializeAsTransfer();

        var form = pk3.Form;
        core.Species = species;
        core.Form = form;
        core.EXP = pk3.EXP;
        uint ec = pk3.PID;
        core.EncryptionConstant = ec;
        core.Nature = (Nature)(ec % 25);
        core.StatAlignment = core.Nature;
        core.Gender = pk3.Gender; // Keep the original gender when the PID is corrected below.
        core.Form = form;
        core.FatefulEncounter = GetIsFatefulFRLG(species);
        core.Version = version;
        core.Language = language;
        var id32 = core.ID32 = pk3.ID32;

        core.PID = GetTransferPID(ec, id32, out _);

        // HOME uses dual-bit marking colors; Gen 3 has four boolean markings.
        for (int i = 0; i < 4; i++)
            core.SetMarking(i, pk3.GetMarking(i) ? MarkingColor.Blue : MarkingColor.None);

        core.EV_HP  = Math.Min(EffortValues.Max252, pk3.EV_HP);
        core.EV_ATK = Math.Min(EffortValues.Max252, pk3.EV_ATK);
        core.EV_DEF = Math.Min(EffortValues.Max252, pk3.EV_DEF);
        core.EV_SPE = Math.Min(EffortValues.Max252, pk3.EV_SPE);
        core.EV_SPA = Math.Min(EffortValues.Max252, pk3.EV_SPA);
        core.EV_SPD = Math.Min(EffortValues.Max252, pk3.EV_SPD);

        core.ContestCool = pk3.ContestCool;
        core.ContestBeauty = pk3.ContestBeauty;
        core.ContestCute = pk3.ContestCute;
        core.ContestSmart = pk3.ContestSmart;
        core.ContestTough = pk3.ContestTough;
        core.ContestSheen = pk3.ContestSheen;

        core.IV_HP = pk3.IV_HP;
        core.IV_ATK = pk3.IV_ATK;
        core.IV_DEF = pk3.IV_DEF;
        core.IV_SPE = pk3.IV_SPE;
        core.IV_SPA = pk3.IV_SPA;
        core.IV_SPD = pk3.IV_SPD;
        // IsEgg is always false; can't transfer eggs.

        core.RibbonChampionG3 = pk3.RibbonChampionG3;
        core.RibbonArtist = pk3.RibbonArtist;
        core.RibbonEffort = pk3.RibbonEffort;
        core.RibbonChampionBattle = pk3.RibbonChampionBattle;
        core.RibbonChampionRegional = pk3.RibbonChampionRegional;
        core.RibbonChampionNational = pk3.RibbonChampionNational;
        core.RibbonCountry = pk3.RibbonCountry;
        core.RibbonNational = pk3.RibbonNational;
        core.RibbonEarth = pk3.RibbonEarth;
        core.RibbonWorld = pk3.RibbonWorld;
        core.RibbonCountMemoryContest = pk3.RibbonCountMemoryContest;
        core.RibbonCountMemoryBattle = pk3.RibbonCountMemoryBattle;

        // HOME generates fresh size scalars for this conversion rather than preserving computed Gen 3 size data.
        core.HeightScalar = PokeSizeUtil.GetRandomScalar();
        core.WeightScalar = PokeSizeUtil.GetRandomScalar();

        ConvertStrings3(core, pk3, species, language);

        // Server decides date, as local time for the user.
        var date = EncounterDate.GetDate(GameConsole.NX);
        core.MetYear = (byte)(date.Year - 2000);
        core.MetMonth = (byte)date.Month;
        core.MetDay = (byte)date.Day;
        // Eggs (and any zero met level) need a real HOME met level.
        // This is different from the PK3->PK4 conversion, which just sets the current level!
        var metLevel = pk3.MetLevel;
        if (metLevel == 0)
            metLevel = 5;
        core.MetLevel = metLevel;

        var pi = PersonalTable.SWSH[species, form];
        var level = Experience.GetLevel(core.EXP, pi.EXPGrowth);
        if (level == 100) // HOME doesn't sanity check within the converter, but we'll do so ourselves.
            pkh.EXP = Experience.GetEXP(level, pi.EXPGrowth);

        // PK3 has party HP data, but let's recalculate it instead of assuming PK3's is valid.
        core.Stat_HPCurrent = (ushort)(pi.HP == 1 ? 1 : ((core.IV_HP + (2 * pi.HP) + (core.EV_HP / 4) + 100) * level / 100) + 10);

        // Skipped - Defensive zeroing: Many fields are cleared by the FR/LG conversion path.
        // Our side-data is already zeroed, no need to clear it again.

        // A clean HOME object with an FRX/LGX origin falls back to GameDataPK8 to set up the remainder of the values.
        side.Ball = ball; // Gen 3 ball IDs 1-12 work fine with SW/SH.
        side.MetLocation = LocationsHOME.VirtualConsole3;
        side.Ability = (ushort)pk3.Ability;
        side.AbilityNumber = (byte)pk3.AbilityNumber; // determined outside the main function, inferred

        // Since the version is remapped on import to SW/SH, it assumes it is battle ready.
        // Need to reset the moves to be SW/SH's.
        PopulateMovesFRLG(side, species, form, level);
    }

    private static void ConvertStrings3(GameDataCore core, PK3 pk3, ushort species, int language)
    {
        ConvertTrainerName3(core, pk3.OriginalTrainerTrash, language);
        ConvertNickname3(core, pk3.NicknameTrash, species, language);
    }

    private static void ConvertTrainerName3(GameDataCore core, ReadOnlySpan<byte> src, int language)
    {
        var span = core.OriginalTrainerTrash;
        var dest = MemoryMarshal.Cast<byte, char>(span);
        // HOME stores strings in the Gen 8 character encoding.
        _ = StringConverter3.LoadStringHOME(src, dest, language);
        if (BitConverter.IsLittleEndian)
            return;
        var self = MemoryMarshal.Cast<char, ushort>(dest);
        System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(self, self);
    }

    private static void ConvertNickname3(GameDataCore core, ReadOnlySpan<byte> src, ushort species, int language)
    {
        var span = core.NicknameTrash;
        var dest = MemoryMarshal.Cast<byte, char>(span);
        int length = StringConverter3.LoadStringHOME(src, dest, language);
        dest = dest[.. length];
        if (!BitConverter.IsLittleEndian)
        {
            var self = MemoryMarshal.Cast<char, ushort>(dest);
            System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(self, self);
        }

        // Check if it is nicknamed with the current language context (not friendly to traded-between-languages).
        var future = SpeciesName.GetSpeciesName(species, language);
        var isNicknamed = CheckIsNicknamed(dest, future);
        if (isNicknamed) // Keep the transferred string.
            core.IsNicknamed = true;
        else // Reset to the original species name (decapitalized), clear zero.
            StringConverter8.SetString(span, future, future.Length);
    }

    private static bool CheckIsNicknamed(ReadOnlySpan<char> currentText, ReadOnlySpan<char> speciesName)
    {
        // Check the real species name after normalizing (uppercase, no accents).
        // Eager check for length.
        // +1 length due to null terminator.
        if (currentText.Length != speciesName.Length + 1)
            return false;

        // HOME performs an eager check assuming normalization isn't required
        // Decapitalized species names applied as nicknames will thus be reverted.
        if (currentText[..^1].SequenceEqual(speciesName))
            return true;

        // Might be nicknamed; normalize and check again.
        Span<char> tmp = stackalloc char[speciesName.Length + 1];
        speciesName.CopyTo(tmp);
        NormalizeName(tmp, tmp);
        return !currentText.SequenceEqual(tmp);
    }

    private static void PopulateMovesFRLG(GameDataPK8 side, ushort species, byte form, byte level)
    {
        // FR/LG transfers reset the move list to the SW/SH learnset for the current level.
        // HOME's stored side data starts with move PP, PP Ups and relearn moves cleared.
        Span<ushort> moves = stackalloc ushort[4];
        var learn = LearnSource8SWSH.Instance.GetLearnsetHOME(species, form);
        learn.SetEncounterMoves(level, moves);
        side.Move1 = moves[0];
        side.Move2 = moves[1];
        side.Move3 = moves[2];
        side.Move4 = moves[3];

        // PP, PP Ups, and relearn moves are already cleared by the default constructor of GameDataPK8.
        // HOME does not set current PP for the moves it just applied; they're left 0.
        // When transferring to a new game, the PP for that format is healed to the correct value.
        // Same issue as in un-transferred GO entities.
    }

    private static GameVersion GetTransferVersion3(GameVersion version) => version switch
    {
        GameVersion.FR => GameVersion.FRX,
        GameVersion.LG => GameVersion.LGX,
        _ => 0,
    };

    /// <summary>
    /// Determines if a given species is considered a fateful encounter in the context of FR/LG transfers to HOME.
    /// </summary>
    /// <param name="species"></param>
    /// <remarks>
    /// The disassembly's fateful-encounter helper receives species, not the obedience flag.
    /// FR/LG's event-only mythical species are therefore treated as fateful by species.
    /// Assumedly, they'll precondition this check so that R/S origin returns false.
    /// </remarks>
    private static bool GetIsFatefulFRLG(ushort species) => species is
        (int)Species.Mew or
        (int)Species.Lugia or
        (int)Species.HoOh or
        (int)Species.Latias or
        (int)Species.Latios or
        (int)Species.Deoxys;

    // For checking if it is nicknamed, the disassembly normalizes the name to uppercase and replaces accented E's with E.
    private static void NormalizeName(ReadOnlySpan<char> source, Span<char> destination)
    {
        // The native routine processes exactly 13 UTF-16 code units.
        for (int i = 0; i < source.Length; i++)
        {
            var c = source[i];
            if (c is >= 'a' and <= 'z')
                c = (char)(c - 32);
            else if ((c & 0xFFFC) is 0x00C8 or 0x00E8)
                c = 'E';

            destination[i] = c;
        }
    }

    // Simplified version of how HOME determines PID.
    // Official logic checks if shiny as PK3, then forces the PID to be that state (with duplicate checks since it's a standard API method).
    public static uint GetTransferPID(uint ec, uint id32, out bool bitflip)
    {
        var pid = ec;
        if (ShinyUtil.GetShinyXor(pid, id32) is (>= 8 and <= 15))
        {
            pid ^= 0x1000_0000; // Different from the PK5=>PK6 xor, but standard HOME xor.
            bitflip = true;
        }
        bitflip = false;
        return pid;
    }
}
