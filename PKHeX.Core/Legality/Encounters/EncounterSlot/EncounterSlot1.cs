using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen1"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot1(EncounterArea1 Parent, ushort Species, byte LevelMin, byte LevelMax, byte SlotNumber) : IEncounterConvertible<PK1>, EncounterSlot, INumberedSlot, ILevelRange
{
    public int Generation => 1;
    public EntityContext Context => EntityContext.Gen1;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public byte Form => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;

#region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);

    public PK1 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK1 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var isJapanese = lang == (int)LanguageID.Japanese;
        var pk = new PK1(isJapanese)
        {
            Species = Species,
            CurrentLevel = LevelMin,
            Catch_Rate = EncounterUtil1.GetWildCatchRate(Version, Species),
            DV16 = EncounterUtil1.GetRandomDVs(Util.Rand),

            Language = lang,
            OT_Name = tr.OT,
            TID16 = tr.TID16,
        };

        (pk.Type1, pk.Type2) = EncounterUtil1.GetTypes(Version, Species);

        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

        pk.ResetPartyStats();
        return pk;
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Handled by Area
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
    #endregion
}

public static class EncounterUtil1
{
    public const int FormDynamic = FormVivillon;
    public const byte FormVivillon = 30;
    public const byte FormRandom = 31;

    public static void SetRandomIVs(this GBPKM pk, Random rand) => pk.DV16 = GetRandomDVs(rand);

    public static ushort GetRandomDVs(Random rand) => (ushort)rand.Next(ushort.MaxValue + 1);

    public static byte GetWildCatchRate(GameVersion version, ushort species) => (byte)(version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB)[species].CatchRate;
    public static (byte Type1, byte Type2) GetTypes(GameVersion version, ushort species)
    {
        var pt = version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        var pi = pt[species];
        return (pi.Type1, pi.Type2);
    }

    public static void SetEncounterMoves<T>(T pk, GameVersion version, int level) where T : PKM
    {
        Span<ushort> moves = stackalloc ushort[4];
        var source = GameData.GetLearnSource(version);
        source.SetEncounterMoves(pk.Species, 0, level, moves);
        pk.SetMoves(moves);
        pk.SetMaximumPPCurrent(moves);
    }
}
