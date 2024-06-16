using System;
using static PKHeX.Core.SlotType2;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen2"/>.
/// </summary>
/// <remarks>
/// Referenced Area object contains Time data which is used for <see cref="GameVersion.C"/> origin data.
/// </remarks>
public sealed record EncounterSlot2(EncounterArea2 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK2>, INumberedSlot, IEncounterFormRandom, IEncounterTime
{
    public byte Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => TransporterLogic.IsHiddenDisallowedVC2(Species) ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType2 Type => Parent.Type;
    public bool IsHeadbutt => Type is Headbutt or HeadbuttSpecial;

    private static ReadOnlySpan<byte> TreeIndexes =>
    [
        02, 04, 05, 08, 11, 12, 14, 15, 18, 20, 21, 25, 26, 34, 37, 38, 39, 91, 92,
    ];

    private static ReadOnlySpan<int> Trees =>
    [
        0x3FF_3FF, // Route 29
        0x0FF_3FF, // Route 30
        0x3FE_3FF, // Route 31
        0x3EE_3FF, // Route 32
        0x240_3FF, // Route 33
        0x37F_3FF, // Azalea Town
        0x3FF_3FF, // Ilex Forest
        0x001_3FE, // Route 34
        0x261_3FF, // Route 35
        0x3FF_3FF, // Route 36
        0x2B9_3FF, // Route 37
        0x3FF_3FF, // Route 38
        0x184_3FF, // Route 39
        0x3FF_3FF, // Route 42
        0x3FF_3FF, // Route 43
        0x3FF_3FF, // Lake of Rage
        0x2FF_3FF, // Route 44
        0x200_1FF, // Route 26
        0x2BB_3FF, // Route 27
    ];

    public bool IsTreeAvailable(ushort trainerID)
    {
        var treeIndex = TreeIndexes.BinarySearch((byte)Location);
        if (treeIndex < 0)
            return false;
        var permissions = Trees[treeIndex];

        var pivot = trainerID % 10;
        return Type switch
        {
            Headbutt => (permissions & (1 << pivot)) != 0,
            /*special*/ _ => (permissions & (1 << (pivot + 12))) != 0,
        };
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);

    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var isJapanese = lang == (int)LanguageID.Japanese;
        var pi = PersonalTable.C[Species];
        var pk = new PK2(isJapanese)
        {
            Species = Species,
            // Form is only Unown and is derived from IVs.
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            DV16 = EncounterUtil.GetRandomDVs(Util.Rand),

            Language = lang,
            OriginalTrainerName = tr.OT,
            TID16 = tr.TID16,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (Version == GameVersion.C)
        {
            pk.OriginalTrainerGender = tr.Gender;
            pk.MetLevel = LevelMin;
            pk.MetLocation = Location;
            pk.MetTimeOfDay = GetRandomTime();
        }

        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        if (IsHeadbutt)
        {
            var id = pk.TID16;
            if (!IsTreeAvailable(id))
            {
                // Get a random TID that satisfies this slot.
                do { id = (ushort)Util.Rand.Next(); }
                while (!IsTreeAvailable(id));
                pk.TID16 = id;
            }
        }

        pk.ResetPartyStats();
        return pk;
    }

    public int GetRandomTime() => Parent.Time.RandomValidTime();
    public EncounterTime EncounterTime => Parent.Time;

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (evo.Form != Form)
        {
            if (Species != (int)Core.Species.Unown || evo.Form >= 26) // Don't yield !? forms
                return false;
        }

        if (pk is not ICaughtData2 {CaughtData: not 0} c2)
            return LevelMin <= evo.LevelMax;

        if (!this.IsLevelWithinRange(c2.MetLevel))
            return false;
        if (!Parent.Time.Contains(c2.MetTimeOfDay))
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsHeadbutt && !IsTreeAvailable(pk.TID16))
            return EncounterMatchRating.DeferredErrors;
        return EncounterMatchRating.Match;
    }
    #endregion
}
