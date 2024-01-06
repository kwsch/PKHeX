using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 Trade Encounter
/// </summary>
public sealed record EncounterTrade2 : IEncounterable, IEncounterMatch, IFixedTrainer, IFixedNickname, IFixedGender, IFixedIVSet, IEncounterConvertible<PK2>
{
    public int Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public int Location => Locations.LinkTrade2NPC;
    public GameVersion Version => GameVersion.GSC;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;
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

            Met_Location = Location,

            Nickname = Nicknames[lang],
            OT_Name = TrainerNames[lang],
            OT_Friendship = pi.BaseFriendship,
        };

        if (IVs.IsSpecified)
        {
            pk.DV16 = EncounterUtil.GetDV16(IVs);
            pk.OT_Gender = OTGender;
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
                if (c.Met_Location != Locations.LinkTrade2NPC)
                    return false;
                if (c.Met_Level != 0)
                    return false;
                if (IVs.IsSpecified && c.OT_Gender != OTGender)
                    return false;
            }
        }
        else // 7+
        {
            // require male except if transferred from GS
            if (pk.VC1 && pk.OT_Gender != 0)
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

    private bool IsTrainerNicknameCorrect(PKM pk)
    {
        var indexOT = GetIndexTrainer(pk.OT_Name, pk);
        if (indexOT == -1)
            return false;
        if (pk.Nickname != Nicknames[indexOT])
            return false;
        return true;
    }

    private int GetIndexTrainer(ReadOnlySpan<char> OT, PKM pk)
    {
        if (pk.Japanese)
            return OT.SequenceEqual(TrainerNames[1]) ? 1 : -1;
        if (pk.Korean)
            return OT.SequenceEqual(TrainerNames[(int)LanguageID.Korean]) ? 2 : -1;

        var lang = GetInternationalLanguageID(OT);
        if (pk.Format < 7)
            return lang;

        if (lang is not (-1 or (int)LanguageID.Spanish))
            return lang;
        return SanityCheckTrainerNameIndex(pk, OT, lang);
    }

    private int SanityCheckTrainerNameIndex(ILangNick pk, ReadOnlySpan<char> OT, int lang) => Species switch
    {
        // Can't transfer verbatim with Spanish origin glyphs to French VC.
        (int)Voltorb when pk.Language == (int)LanguageID.French && OT is "FALCçN" => (int)LanguageID.Spanish, // FALCÁN
        (int)Shuckle when pk.Language == (int)LanguageID.French && OT is "MANôA"  => (int)LanguageID.Spanish, // MANÍA
        _ => lang,
    };

    private int GetInternationalLanguageID(ReadOnlySpan<char> OT)
    {
        const int start = (int)LanguageID.English;
        const int end = (int)LanguageID.Spanish;

        var tr = TrainerNames;
        for (int i = start; i <= end; i++)
        {
            if (OT.SequenceEqual(tr[i]))
                return i;
        }
        return -1;
    }

    // Already required for encounter matching.
    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (!IsTrainerNicknameCorrect(pk))
            return EncounterMatchRating.DeferredErrors;
        return EncounterMatchRating.Match;
    }

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => GetIndexTrainer(trainer, pk) != -1;

    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language)
    {
        var index = GetIndexTrainer(pk.OT_Name, pk);
        if (index == -1)
            return false;
        return nickname.SequenceEqual(Nicknames[index]);
    }

    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

    #endregion
}
