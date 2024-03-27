using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 Trade Encounter
/// </summary>
public sealed record EncounterTrade2 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IFixedGender, IFixedIVSet, IEncounterConvertible<PK2>
{
    public byte Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public ushort Location => Locations.LinkTrade2NPC;
    public GameVersion Version => GameVersion.GSC;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => true;

    public byte Form => 0;

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public byte LevelMin => Level;
    public byte LevelMax => 100;

    private string[] TrainerNames { get; }
    private string[] Nicknames { get; }

    public byte Gender { get; init; }
    public byte OTGender { get; init; }
    public IndividualValueSet IVs { get; init; }
    public ushort Species { get; }
    public byte Level { get; }
    public ushort TID16 { get; }

    public EncounterTrade2(ReadOnlySpan<string[]> names, byte index, ushort species, byte level, ushort tid16)
    {
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        Species = species;
        Level = level;
        TID16 = tid16;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        // Prefer to generate as Crystal, as it will include met data.
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, GameVersion.C);
        var isJapanese = lang == (int)LanguageID.Japanese;
        var pi = PersonalTable.C[Species];
        var pk = new PK2(isJapanese)
        {
            Species = Species,
            CurrentLevel = Level,

            MetLocation = Location,

            Nickname = Nicknames[lang],
            OriginalTrainerName = TrainerNames[lang],
            OriginalTrainerFriendship = pi.BaseFriendship,
        };

        if (IVs.IsSpecified)
        {
            pk.DV16 = EncounterUtil.GetDV16(IVs);
            pk.OriginalTrainerGender = OTGender;
            pk.TID16 = TID16;
        }
        else
        {
            pk.DV16 = EncounterUtil.GetRandomDVs(Util.Rand);
            pk.TID16 = tr.TID16;
        }

        EncounterUtil.SetEncounterMoves(pk, Version, Level);

        pk.ResetPartyStats();

        return pk;
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (evo.LevelMax < LevelMin)
            return false;
        if (pk.Format <= 2)
        {
            // Gender is tied to IVs. Don't bother checking Gender, just check IVs.
            if (IVs.IsSpecified)
            {
                if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
                    return false;
                if (pk.TID16 != TID16)
                    return false;
            }
            if (pk is ICaughtData2 { CaughtData: not 0 } c)
            {
                if (c.MetLocation != Locations.LinkTrade2NPC)
                    return false;
                if (c.MetLevel != 0)
                    return false;
                if (IVs.IsSpecified && c.OriginalTrainerGender != OTGender)
                    return false;
            }
        }
        else // 7+
        {
            // require male except if transferred from GS
            if (pk.VC1 && pk.OriginalTrainerGender != 0)
                return false;
            if (IVs.IsSpecified)
            {
                if (pk.Gender != Gender)
                    return false;
                if (pk.TID16 != TID16)
                    return false;
            }
        }
        return true;
    }

    // Italian and English share the same OT name for Spearow, but different nicknames.

    private static int DetectOriginalLanguage(ReadOnlySpan<char> text, ReadOnlySpan<string> all)
    {
        for (int i = 1; i < all.Length; i++)
        {
            if (text.SequenceEqual(all[i]))
                return i;
        }
        return -1;
    }

    private bool IsTrainerNicknameCorrect(PKM pk)
    {
        var language = DetectOriginalLanguage(pk.Nickname, Nicknames);
        if (language == -1)
            return false;

        if ((language == (int)LanguageID.Japanese) != pk.Japanese)
            return false;
        if ((language == (int)LanguageID.Korean) != pk.Korean)
            return false;

        ReadOnlySpan<char> ot = pk.OriginalTrainerName;
        if (language == 4 && Species is (ushort)Shuckle)
        {
            // Same nickname, different OT name for Italian and Spanish.
            if (ot is "MANIA")
                return true;
            language = 7; // Might be remapped.
        }
        var expect = pk.Format < 7 ? TrainerNames[language] : GetExpectedOT(Species, language, pk.Language);
        return ot.SequenceEqual(expect);
    }

    private string GetExpectedOT(ushort species, int language, int pkLanguage) => species switch
    {
        // Can't transfer verbatim with Spanish origin glyphs to French VC.
        (int)Voltorb when language == (int)LanguageID.Spanish && pkLanguage == (int)LanguageID.French => "FALCçN", // FALCÁN
        (int)Shuckle when language == (int)LanguageID.Spanish && pkLanguage == (int)LanguageID.French => "MANôA", // MANÍA
        _ => TrainerNames[language],
    };

    // Already required for encounter matching.
    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (!IsTrainerNicknameCorrect(pk))
            return EncounterMatchRating.DeferredErrors;
        return EncounterMatchRating.Match;
    }

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int _)
    {
        var language = DetectOriginalLanguage(pk.Nickname, Nicknames);
        if (language == -1)
        {
            // Be generous and check the trainer name as well. The Nickname will be flagged as incorrect via a separate check.
            if (DetectOriginalLanguage(trainer, TrainerNames) == -1)
                return false;
        }

        if ((language == (int)LanguageID.Japanese) != pk.Japanese)
            return false;
        if ((language == (int)LanguageID.Korean) != pk.Korean)
            return false;

        ReadOnlySpan<char> ot = pk.OriginalTrainerName;
        if (language == 4 && Species is (ushort)Shuckle)
        {
            // Same nickname, different OT name for Italian and Spanish.
            if (ot is "MANIA")
                return true;
            language = 7; // Might be remapped.
        }
        var expect = pk.Format < 7 ? TrainerNames[language] : GetExpectedOT(Species, language, pk.Language);
        return ot.SequenceEqual(expect);
    }

    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int _)
    {
        var language = DetectOriginalLanguage(nickname, Nicknames);
        if (language == -1)
            return false;

        if ((language == (int)LanguageID.Japanese) != pk.Japanese)
            return false;
        if ((language == (int)LanguageID.Korean) != pk.Korean)
            return false;

        // Nickname matches an expected nickname via the first called method.
        return true;
    }

    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

    #endregion
}
