using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.RandomCorrelationRating;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Pokéwalker  Encounter
/// </summary>
public sealed record EncounterStatic4Pokewalker(PokewalkerCourse4 Course)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>, IMoveset, IRandomCorrelation, IFixedGender
{
    public byte Generation => 4;
    public EntityContext Context => EntityContext.Gen4;
    public GameVersion Version => GameVersion.HGSS;

    public ushort Location => Locations.PokeWalker4;
    public bool IsShiny => false;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Never;
    public byte Form => 0;
    ushort ILocation.EggLocation => 0;

    public ushort Species { get; }
    public byte Level { get; }
    public byte Gender { get; }
    public Moveset Moves { get; }

    public string Name => $"Pokéwalker Encounter ({Course})";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    private EncounterStatic4Pokewalker(ReadOnlySpan<byte> data, PokewalkerCourse4 course) : this(course)
    {
        Species = ReadUInt16LittleEndian(data);
        Level = data[2];
        Gender = data[3];
        var move1 = ReadUInt16LittleEndian(data[0x4..]);
        var move2 = ReadUInt16LittleEndian(data[0x6..]);
        var move3 = ReadUInt16LittleEndian(data[0x8..]);
        var move4 = ReadUInt16LittleEndian(data[0xA..]);
        Moves = new(move1, move2, move3, move4);
        System.Diagnostics.Debug.Assert(move1 != 0); // All Pokéwalker encounters have at least one move, so this is a good sanity check for the data.
    }

    public static EncounterStatic4Pokewalker[] GetAll(ReadOnlySpan<byte> data)
    {
        const int SlotSize = 0xC;
        const int SlotsPerCourse = PokewalkerRNG.SlotsPerCourse;
        const int SlotCount = SlotsPerCourse * (int)PokewalkerCourse4.MAX_COUNT;
        System.Diagnostics.Debug.Assert(data.Length == SlotCount * SlotSize);

        PokewalkerCourse4 course = 0;
        var result = new EncounterStatic4Pokewalker[SlotCount];
        for (int i = 0, offset = 0; i < result.Length; course++)
        {
            for (int s = 0; s < SlotsPerCourse; s++, offset += SlotSize)
                result[i++] = new(data.Slice(offset, SlotSize), course);
        }
        return result;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage456((LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.HGSS[Species];
        var pk = new PK4
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = version,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,

            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };

        SetPINGA(pk, criteria, pi);
        SetMoves(pk);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetMoves(PK4 pk)
    {
        // Game quirk:
        // Initializing a PK4 from Pokéwalker encounter template:
        // - applies normal level-up moves,
        // - heals the PP,
        // - overwrites the moves from the Pokéwalker encounter template,
        // - heals the PP again
        // Due to the PP healing not updating empty move slots, any full->empty move slot will retain the PP from the initial level-up moves.

        // Initialize moves with default moves, then overwrite with the predefined moves.
        // Immediately after this in overlay_112, it overwrites the default move IDs with the ones sent back by the Pokéwalker.
        // When it reaches PCStorage_PlaceMonInBoxByIndexPair, it calls RestoreBoxMonPP,
        // which only sets the new PP for moves that are not MOVE_NONE, leaving empty slot PP values unmodified.
        const int count = 4;
        Span<ushort> moves = stackalloc ushort[count];
        Span<byte> pp = stackalloc byte[count];
        PopulateMoves(moves, pp);

        pk.Move1 = moves[0];
        pk.Move2 = moves[1];
        pk.Move3 = moves[2];
        pk.Move4 = moves[3];
        pk.Move1_PP = pp[0];
        pk.Move2_PP = pp[1];
        pk.Move3_PP = pp[2];
        pk.Move4_PP = pp[3];
    }

    private void PopulateMoves(Span<ushort> moves, Span<byte> pp)
    {
        LoadInitialMoves(moves);
        LoadInitialPP(moves, pp);
        Moves.CopyTo(moves);
        LoadInitialPP(moves, pp);
    }

    public bool IsPossibleMovesetEmpty(PK4 pk, bool isStored)
    {
        // Easy eager check.
        if (Moves.Move4 != 0)
            return false;

        if (!IsPossibleGlitched(Species, Level))
            return false;

        // If it must have learned a level up move, then it cannot retain its empty-PP-with-value slot.
        var level = GetNextLearnedMoveLevel(Species, pk.Species, Level);
        if (level != NoLevelUpMoves && pk.CurrentLevel > level)
            return false;

        // Get the original moves and PP. For the two encounters that reach here, we don't care about original PP.
        // Moves can be used (decreased PP), and PP Ups can be applied (raising the amount). The glitched slot is always 20.
        const int count = 4;
        Span<ushort> tmp = stackalloc ushort[count];
        Span<byte> pp = stackalloc byte[count];
        PopulateMoves(tmp, pp);

        // Check the current values against the original values.
        return VerifyGlitchedCounts(pk, tmp, isStored);
    }

    private static bool VerifyGlitchedCounts(PK4 pk, Span<ushort> originalMoves, bool isStored)
    {
        // Moves can be deleted, which shifts the move up a slot.
        // Moves can be reordered, which doesn't heal PP for any slot. Can't have empty interstitials (verify in regular moveset checks).
        // Moves that are non-zero can be used in battle, so don't worry about the PP value.

        // If a foreign move was introduced, then it cannot retain its empty-PP-with-value slot.
        bool seenZero = false;
        for (int i = 0; i < 4; i++)
        {
            var move = pk.GetMove(i);
            var actual = GetMovePP(pk, i);

            if (move == 0)
            {
                // Verify the PP value of a zero-move.
                byte expect = seenZero ? (byte)0 : (byte)20; // all glitched slots are 20
                if (actual != expect)
                    return false;
                seenZero = true;
            }
            else
            {
                if (!originalMoves.Contains(move))
                    return false; // foreign move added, cannot retain empty-PP-with-value slot

                // Verify the actual PP while we're here. PP Ups can be used to increase the count.
                var expect = pk.GetMovePP(move, GetMoveUps(pk, i));
                if (isStored) // most common.
                {
                    if (actual != expect)
                        return false;
                }
                else
                {
                    if (actual > expect)
                        return false;
                }
            }
        }
        return true;
    }

    private static int GetMoveUps(PK4 pk, int index) => index switch
    {
        0 => pk.Move1_PPUps,
        1 => pk.Move2_PPUps,
        2 => pk.Move3_PPUps,
        _ => pk.Move4_PPUps,
    };

    private static int GetMovePP(PK4 pk, int index) => index switch
    {
        0 => pk.Move1_PP,
        1 => pk.Move2_PP,
        2 => pk.Move3_PP,
        _ => pk.Move4_PP,
    };

    private void LoadInitialMoves(Span<ushort> tmp)
    {
        ILearnSource source = LearnSource4HGSS.Instance;
        source.SetEncounterMoves(Species, Form, Level, tmp);
    }

    private static void LoadInitialPP(ReadOnlySpan<ushort> moves, Span<byte> pp)
    {
        var table = MoveInfo4.PP;
        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (move != 0)
                pp[i] = table[move];
        }
    }

    private void SetPINGA(PK4 pk, in EncounterCriteria criteria, PersonalInfo4 pi)
    {
        var gender = criteria.GetGender(Gender, pi);
        var nature = (uint)criteria.GetNature();

        var pid = pk.PID = PokewalkerRNG.GetPID(pk.TID16, pk.SID16, nature, pk.Gender = gender, pi.Gender);
        // Cannot force an ability; nature-gender-trainerID only yield fixed PIDs.
        pk.RefreshAbility((int)(pid & 1));
        pk.IV32 = GetIV32(criteria);
    }

    private static uint GetIV32(in EncounterCriteria criteria)
    {
        if (criteria.IsSpecifiedIVsAll()) // Don't trust that the requirements are valid
        {
            criteria.GetCombinedIVs(out var iv1, out var iv2);

            // With Mic Test, any seed can be the seed that immediately generates the IVs.
            var ctr = LCRNGReversal.GetSeedsIVs(stackalloc uint[LCRNG.MaxCountSeedsIV], iv1, iv2);
            if (ctr != 0)
                return criteria.GetCombinedIVs();
        }
        PokewalkerRNG.GetRandomIVs(criteria, out var iv32);
        return iv32;
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
            return false;
        if (!IsMatchGender(pk))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private static bool IsMatchSeed(PKM pk)
    {
        Span<int> ivs = stackalloc int[6];
        pk.GetIVs(ivs);
        var seed = PokewalkerRNG.GetLeastEffortSeed(ivs);
        return seed.Type != PokewalkerSeedType.None;
    }

    private bool IsMatchGender(PKM pk)
    {
        if (pk.Gender == Gender)
            return true;

        // Azurill-F can change to M when evolving in Gen4 (but not in Gen5+) due to Gender Ratio differences.
        if (pk.Species != Species && Species == (ushort)Core.Species.Azurill && Gender == 1)
            return EntityGender.GetFromPID(pk.PID, EntityGender.MF) == Gender;

        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format == 4)
            return pk.MetLocation == Location;
        return true; // transfer location verified later
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return evo.LevelMax >= Level;
        return pk.MetLevel == Level;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        if (pk.IsShiny)
            return EncounterMatchRating.DeferredErrors;

        // Thunder Wave (4 moves) is before the other potential matches. Need to yield the glitched move one first.
        if (Species is (int)Pikachu && Level == 10 && Moves.Move4 != 0 && !pk.HasMove(Moves.Move4))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }

    private static bool IsMatchPartial(PKM pk) => pk.Ball != (byte)Ball.Poke || !IsMatchSeed(pk);
    #endregion

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk)
    {
        if (type is PIDType.Pokewalker)
            return Match;

        // Pokewalker can sometimes be confused with CuteCharm due to the PID creation routine. Double check if it is okay.
        if (type is PIDType.CuteCharm)
            return CuteCharm4.IsCuteCharm(pk, pk.EncryptionConstant) && CuteCharm4.IsValid(this, pk) ? Match : Mismatch;
        return Mismatch;
    }

    public PIDType GetSuggestedCorrelation() => PIDType.Pokewalker;

    private const byte NoLevelUpMoves = 0;

    private static bool IsPossibleGlitched(ushort species, byte lvl) =>
        (species is (int)Pikachu && lvl == 10) ||
        (species is (int)Geodude && lvl == 8);

    private static byte GetNextLearnedMoveLevel(ushort species, ushort currentSpecies, byte lvl) => species switch
    {
        // Mud Slap instead of Mud Sport, no Tackle
        (int)Geodude when lvl == 8 => 11,

        // Can dodge via evolving before the level threshold. No further level-up moves.
        // There's one Pikachu encounter without Thunder Wave at level 10.
        (int)Pikachu when lvl == 10 => currentSpecies != species ? NoLevelUpMoves : (byte)13,

        _ => NoLevelUpMoves, // No further level up moves
    };
}
