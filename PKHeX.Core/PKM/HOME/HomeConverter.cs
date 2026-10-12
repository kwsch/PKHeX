using System;

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
        var side = pkh.DataPK8 ?? new();

        var form = pk3.Form;
        core.Species = species;
        core.Form = form;
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

        // Simplified version of how HOME determines PID.
        // Official logic checks if shiny as PK3, then forces the PID to be that state (with duplicate checks since it's a standard API method).
        var pid = ec;
        if (ShinyUtil.GetShinyXor(pid, id32) is (>= 8 and <= 15))
            pid ^= 0x1000_0000; // Different from the PK5=>PK6 xor, but standard HOME xor.
        core.PID = pid;

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
        core.MetYear = (byte)date.Year;
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

        // PK3 has party HP data, but let's recalculate it instead of assuming PK3's is valid.
        core.Stat_HPCurrent = (ushort)(pi.HP == 1 ? 1 : ((core.IV_HP + (2 * pi.HP) + (core.EV_HP / 4) + 100) * level / 100) + 10);

        // Skipped - Defensive zeroing: Many fields are cleared by the FR/LG conversion path.
        // Our side-data is already zeroed, no need to clear it again.

        // A clean HOME object with an FRX/LGX origin falls back to GameDataPK8 to set up the remainder of the values.
        side.Ball = ball; // Gen 3 ball IDs 1-12 have the same HOME/SWSH IDs.
        // SW/SH won't display the met location correctly, so sanitize to the localized location ID.
        side.MetLocation = LocationsHOME.GetMetSWSH(LocationsHOME.VirtualConsole3, version);
        side.Ability = (ushort)pk3.Ability;
        side.AbilityNumber = (byte)pk3.AbilityNumber; // determined outside the main function, inferred

        // Since the version is remapped on import to SW/SH, it assumes it is battle ready.
        // Need to reset the moves to be SW/SH's.
        PopulateMovesFRLG(side, species, form, level);
    }

    private static void ConvertStrings3(GameDataCore core, PK3 pk3, ushort species, int language)
    {
        // HOME stores strings in the Gen 8 character encoding.
        core.OriginalTrainerName = pk3.OriginalTrainerName;

        if (pk3.IsNicknamed)
        {
            core.Nickname = pk3.Nickname;
            core.IsNicknamed = true;
        }
        else
        {
            core.NicknameTrash.Clear();
            var reset = SpeciesName.GetSpeciesNameImportHOME(species, language, 8);
            StringConverter8.SetString(core.NicknameTrash, reset, reset.Length, StringConverterOption.None);
        }
    }

    private static void PopulateMovesFRLG(GameDataPK8 side, ushort species, byte form, byte level)
    {
        // FR/LG transfers reset the move list to the SWSH learnset for the current level.
        // HOME's stored side data starts with move PP, PP Ups and relearn moves cleared.
        Span<ushort> moves = stackalloc ushort[4];
        ILearnSource source = LearnSource8SWSH.Instance;
        source.SetEncounterMoves(species, form, level, moves);
        side.Move1 = moves[0];
        side.Move2 = moves[1];
        side.Move3 = moves[2];
        side.Move4 = moves[3];

        // Set actual PP values rather than zeroes.
        var pp = MoveInfo8.PP;
        side.Move1_PP = pp[moves[0]];
        side.Move2_PP = pp[moves[1]];
        side.Move3_PP = pp[moves[2]];
        side.Move4_PP = pp[moves[3]];

        // Defensive zeroing, not necessary.
        side.Move1_PPUps = side.Move2_PPUps = side.Move3_PPUps = side.Move4_PPUps = 0;
        side.RelearnMove1 = side.RelearnMove2 = side.RelearnMove3 = side.RelearnMove4 = 0;
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
    /// The disassembly's fateful-encounter helper receives species, not the obedience flag. FR/LG's event-only mythicals are therefore treated as fateful by species.
    /// Assumedly, they'll precondition this check so that R/S origin returns false.
    /// </remarks>
    private static bool GetIsFatefulFRLG(ushort species) => species is
        (int)Species.Mew or
        (int)Species.Lugia or
        (int)Species.HoOh or
        (int)Species.Latias or
        (int)Species.Latios or
        (int)Species.Deoxys;

    // found this in the setup logic, assumedly to clean up trainer/nicknames? Maybe it's for checking if it is nicknamed?
    private static void NormalizeName(ReadOnlySpan<char> source, Span<char> destination)
    {
        // The native routine processes exactly 13 UTF-16 code units.
        for (int i = 0; i < 13; i++)
        {
            var c = source[i];
            if (c is >= 'a' and <= 'z')
                c = (char)(c - 32);
            else if ((c & 0xFFFC) is 0x00C8 or 0x00E8)
                c = 'E';

            destination[i] = c;
        }
    }
}
