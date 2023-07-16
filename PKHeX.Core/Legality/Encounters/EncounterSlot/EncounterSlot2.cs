using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen2"/>.
/// </summary>
/// <remarks>
/// Referenced Area object contains Time data which is used for <see cref="GameVersion.C"/> origin data.
/// </remarks>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot2(EncounterArea2 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber) : EncounterSlot, IEncounterConvertible<PK2>, ILevelRange, INumberedSlot
{
    public int Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;

    // we have "Special" bitflag. Strip it out.
    public SlotType SlotType => Parent.Type & (SlotType)0xF;
    public bool IsHeadbutt => SlotType == SlotType.Headbutt;

    private static ReadOnlySpan<byte> TreeIndexes => new byte[]
    {
        02, 04, 05, 08, 11, 12, 14, 15, 18, 20, 21, 25, 26, 34, 37, 38, 39, 91, 92,
    };

    private static ReadOnlySpan<int> Trees => new[]
    {
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
    };

    public bool IsTreeAvailable(ushort trainerID)
    {
        var treeIndex = TreeIndexes.BinarySearch((byte)Location);
        if (treeIndex < 0)
            return false;
        var permissions = Trees[treeIndex];

        var pivot = trainerID % 10;
        var type = Parent.Type;
        return type switch
        {
            SlotType.Headbutt => (permissions & (1 << pivot)) != 0,
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
        var pk = new PK2(isJapanese)
        {
            Species = Species,
            // Form is only Unown and is derived from IVs.
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.C[Species].BaseFriendship,
            DV16 = EncounterUtil1.GetRandomDVs(Util.Rand),

            Language = lang,
            OT_Name = tr.OT,
            TID16 = tr.TID16,
        };

        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);
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

        if (Version == GameVersion.C)
        {
            pk.OT_Gender = tr.Gender;
            pk.Met_Level = LevelMin;
            pk.Met_Location = Location;
            pk.Met_TimeOfDay = Parent.Time.RandomValidTime();
        }
        else
        {
            pk.CaughtData = 0;
        }

        pk.ResetPartyStats();
        return pk;
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Handled by Area
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
    #endregion
}
