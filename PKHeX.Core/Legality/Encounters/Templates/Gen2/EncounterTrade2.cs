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

    private bool IsTrainerNicknameCorrect(PKM pk)
    {
        // Italian and English share the same OT name for Spearow, but different nicknames. Others are like this, so we need to check both.
        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        Span<char> nickname = stackalloc char[pk.TrashCharCountNickname];
        len = pk.LoadString(pk.NicknameTrash, nickname);
        nickname = nickname[..len];

        var lang = DetectLanguage(pk, trainer, nickname);
        return lang != -1;
    }

    private int DetectLanguage(PKM pk, ReadOnlySpan<char> trainer, ReadOnlySpan<char> nickname)
    {
        if (pk.Japanese)
        {
            if (!nickname.SequenceEqual(Nicknames[(int)LanguageID.Japanese]))
                return -1;
            if (!trainer.SequenceEqual(TrainerNames[(int)LanguageID.Japanese]))
                return -1;
            return (int)LanguageID.Japanese;
        }
        if (pk.Korean)
        {
            if (!nickname.SequenceEqual(Nicknames[(int)LanguageID.Korean]))
                return -1;
            if (!trainer.SequenceEqual(TrainerNames[(int)LanguageID.Korean]))
                return -1;
            return (int)LanguageID.Korean;
        }

        for (int i = 2; i < TrainerNames.Length; i++)
        {
            if (i == (int)LanguageID.UNUSED_6)
                continue;
            if (!nickname.SequenceEqual(Nicknames[i]))
                continue;
            if (IsTrainerMatchExact(pk, trainer, i))
                return i;
        }
        return -1;
    }

    private int DetectLanguageNickname(PKM pk, ReadOnlySpan<char> nickname)
    {
        if (pk.Japanese)
        {
            if (!nickname.SequenceEqual(Nicknames[(int)LanguageID.Japanese]))
                return -1;
            return (int)LanguageID.Japanese;
        }
        if (pk.Korean)
        {
            if (!nickname.SequenceEqual(Nicknames[(int)LanguageID.Korean]))
                return -1;
            return (int)LanguageID.Korean;
        }

        for (int i = 2; i < Nicknames.Length; i++)
        {
            if (i == (int)LanguageID.UNUSED_6)
                continue;
            if (nickname.SequenceEqual(Nicknames[i]))
                return i;
        }
        return -1;
    }

    private int DetectLanguageTrainer(PKM pk, ReadOnlySpan<char> trainer)
    {
        if (pk.Japanese)
        {
            if (!trainer.SequenceEqual(TrainerNames[(int)LanguageID.Japanese]))
                return -1;
            return (int)LanguageID.Japanese;
        }
        if (pk.Korean)
        {
            if (!trainer.SequenceEqual(TrainerNames[(int)LanguageID.Korean]))
                return -1;
            return (int)LanguageID.Korean;
        }

        for (int i = 2; i < TrainerNames.Length; i++)
        {
            if (i == (int)LanguageID.UNUSED_6)
                continue;
            if (IsTrainerMatchExact(pk, trainer, i))
                return i;
        }
        return -1;
    }

    private bool IsTrainerMatchExact(PKM pk, ReadOnlySpan<char> trainer, int language)
    {
        var expect = pk.Format < 7 ? TrainerNames[language] : GetExpectedOT(Species, language, pk.Language);
        return trainer.SequenceEqual(expect);
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

    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int _)
    {
        // Match both.
        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];
        var lang = DetectLanguage(pk, trainer, nickname);
        if (lang != -1)
            return true;

        // Both don't match, but maybe one does. Since Trainer is flagged separately, we can flag Nickname if Trainer matches.
        if (DetectLanguageTrainer(pk, trainer) != -1)
            return false; // Trainer matches any possible language, flag nickname.
        if (DetectLanguageNickname(pk, nickname) == -1)
            return false; // Trainer doesn't match any language, nickname doesn't match either.
        return true; // Trainer doesn't match any language, but nickname does.
    }

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int _)
    {
        // Match only trainer.
        var lang = DetectLanguageTrainer(pk, trainer);
        return lang != -1;
    }

    public string GetNickname(int language) => (uint)language < Nicknames.Length ? Nicknames[language] : Nicknames[0];

    #endregion
}
