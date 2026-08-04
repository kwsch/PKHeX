using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using static PKHeX.Core.Species;

namespace PKHeX.Core.AutoMod;

public static class SimpleEdits
{
    // Make PKHeX use our own marking method
    static SimpleEdits() => MarkingApplicator.MarkingMethod = FlagIVsAutoMod;

    internal static ReadOnlySpan<ushort> AlolanOriginForms =>
    [
        019, // Rattata
        020, // Raticate
        027, // Sandshrew
        028, // Sandslash
        037, // Vulpix
        038, // Ninetales
        050, // Diglett
        051, // Dugtrio
        052, // Meowth
        053, // Persian
        074, // Geodude
        075, // Graveler
        076, // Golem
        088, // Grimer
        089, // Muk
    ];

    /// <summary>
    /// Determines if a species/form combination is shiny-locked.
    /// </summary>
    /// <param name="species">The species ID.</param>
    /// <param name="form">The form ID.</param>
    /// <returns>True if shiny-locked, otherwise false.</returns>
    public static bool IsShinyLockedSpeciesForm(ushort species, byte form) => (Species)species switch
    {
        Pikachu => form is not (0 or 8), // Cap Pikachus, Cosplay
        Pichu => form is 1, // Spiky-eared
        Victini or Keldeo => true,
        Scatterbug or Spewpa or Vivillon => form is 19, // Poké Ball
        Hoopa or Volcanion or Cosmog or Cosmoem => true,

        Magearna => true, // Even though a shiny is available via HOME, can't generate as legal.

        Kubfu or Urshifu or Zarude => true,
        Glastrier or Spectrier or Calyrex => true,
        Enamorus => true,
        Gimmighoul => form is 1,

        WoChien or ChienPao or TingLu or ChiYu => true,
        Koraidon or Miraidon => true,

        WalkingWake or IronLeaves => true,
        Okidogi or Munkidori or Fezandipiti => true,
        Ogerpon => true,
        GougingFire or RagingBolt or IronBoulder or IronCrown => true,
        Terapagos => true,
        Pecharunt => true,

        _ => false,
    };

    private static Func<int, int, int> FlagIVsAutoMod(PKM pk)
    {
        return pk.Format < 7 ? GetSimpleMarking : GetComplexMarking;

        // value, index
        static int GetSimpleMarking(int val, int _) => val == 31 ? 1 : 0;
        static int GetComplexMarking(int val, int _) => val switch
        {
            31 => 1,
            1 => 2,
            0 => 2,
            _ => 0,
        };
    }

    /// <summary>
    /// Set Encryption Constant based on PKM Generation
    /// </summary>
    /// <param name="pk">PKM to modify</param>
    /// <param name="enc">Encounter details</param>
    public static void SetEncryptionConstant(this PKM pk, IEncounterTemplate enc)
    {
        if (pk.Format < 6)
            return;

        if (enc is { Species: 658, Form: 1 } || APILegality.IsPIDIVSet(pk, enc)) // Ash-Greninja or raids
            return;

        if (enc.Generation is 3 or 4 or 5)
        {
            var ec = pk.PID;
            pk.EncryptionConstant = ec;
            var pidxor = ((pk.TID16 ^ pk.SID16 ^ (int)(ec & 0xFFFF) ^ (int)(ec >> 16)) & ~0x7) == 8;
            pk.PID = pidxor ? ec ^ 0x80000000 : ec;
            return;
        }
        var wIndex = WurmpleUtil.GetWurmpleEvoGroup(pk.Species);
        if (wIndex != WurmpleEvolution.None)
        {
            pk.EncryptionConstant = WurmpleUtil.GetWurmpleEncryptionConstant(wIndex);
            return;
        }

        if (enc is not ITeraRaid9 && pk is { Species: (ushort)Maushold, Form: 0 } or { Species: (ushort)Dudunsparce, Form: 1 })
        {
            pk.EncryptionConstant = pk.EncryptionConstant / 100 * 100;
            return;
        }
        if (pk.EncryptionConstant != 0)
            return;

        pk.EncryptionConstant = enc is WC8 { PIDType: ShinyType8.FixedValue, EncryptionConstant: 0 } ? 0 : Util.Rand32();
    }

    /// <summary>
    /// Sets shiny value to whatever boolean is specified. Takes in specific shiny as a boolean. Meant for edge case scenarios.
    /// </summary>
    /// <param name="pk">PKM to modify</param>
    /// <param name="isShiny">Shiny value that needs to be set</param>
    /// <param name="enc">Encounter details</param>
    /// <param name="shiny">Shiny Type (star vs square)</param>
    /// <param name="method">PID Method used to determine if Collosseum Eevee needs to be handled</param>
    /// <param name="criteria">Encounter criteria with original shiny request, used for RNG</param>
    public static void SetShinyBoolean(this PKM pk, bool isShiny, IEncounterTemplate enc, Shiny shiny, PIDType method, EncounterCriteria criteria)
    {
        if (IsShinyLockedSpeciesForm(pk.Species, pk.Form)) // Don't modify locked shinies
            return;

        if (pk.IsShiny == isShiny)
            return; // don't mess with stuff if pk is already shiny. Also do not modify for specific shinies (Most likely event shinies)

        if (!isShiny)
        {
            pk.SetUnshiny(); // anti-shiny
            return;
        }

        if (enc is EncounterStatic8N or EncounterStatic8NC or EncounterStatic8ND)
        {
            pk.SetRaidShiny(shiny); //handles SWSH raids
            return;
        }

        if (enc is WC8 { IsHOMEGift: true })
        {
            // Set XOR as 0 so SID comes out as 8 or less, Set TID based on that (kinda like a setshinytid)
            pk.TID16 = (ushort)(0 ^ (pk.PID & 0xFFFF) ^ (pk.PID >> 16));
            pk.SID16 = (ushort)Util.Rand.Next(8);
            return;
        }

        if (enc.Generation > 5 || pk.VC)
        {
            if (enc.Shiny is Shiny.FixedValue or Shiny.Never)
                return;

            while (true)
            {
                pk.SetShiny();
                switch (shiny)
                {
                    case Shiny.AlwaysSquare when pk.ShinyXor != 0:
                    case Shiny.AlwaysStar when pk.ShinyXor == 0:
                        continue;
                }
                return;
            }
        }

        if (enc is MysteryGift mg)
        {
            if (mg.IsEgg || mg is PGT { IsManaphyEgg: true })
            {
                pk.SetShinySID(); // not SID locked
                return;
            }

            pk.SetShiny();
            if (pk.Format < 6)
                return;

            do
            {
                pk.SetShiny();
            } while (IsBit3Set());

            bool IsBit3Set() => ((pk.TID16 ^ pk.SID16 ^ (int)(pk.PID & 0xFFFF) ^ (int)(pk.PID >> 16)) & ~0x7) == 8;
            return;
        }
        if (pk.Version == GameVersion.CXD && method == PIDType.CXD && criteria.Shiny.IsShiny() && pk is XK3 xk && pk.Species != (ushort)Eevee)
            MethodCXD.SetFromIVs(xk, in criteria, xk.PersonalInfo, false);
        TrainerIDVerifier.TryGetShinySID(pk.PID, pk.TID16, pk.Version, out var sid); //gets a valid SID for the given TID and PID for shiny
        pk.SID16 = sid;
        if (isShiny && enc.Generation is 1 or 2)
            pk.SetShiny(); // lol there is no SID in gen 1/2.
        if (enc.Generation != 5)
            return;

        while (true) // gen 5
        {
            pk.PID = EntityPID.GetRandomPID(Util.Rand, pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Form, pk.PID);
            if (shiny == Shiny.AlwaysSquare && pk.ShinyXor != 0)
                continue;

            if (shiny == Shiny.AlwaysStar && pk.ShinyXor == 0)
                continue;

            var isValidGen5SID = pk.SID16 & 1;
            pk.SetShinySID();
            pk.EncryptionConstant = pk.PID;
            var result = (pk.PID & 1) ^ (pk.PID >> 31) ^ (pk.TID16 & 1) ^ (pk.SID16 & 1);
            if ((isValidGen5SID == (pk.SID16 & 1)) && result == 0)
                break;
        }
    }

    /// <summary>
    /// Sets the shiny status for a raid encounter.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="shiny">The shiny type to set.</param>
    public static void SetRaidShiny(this PKM pk, Shiny shiny)
    {
        if (pk.IsShiny)
            return;

        while (true)
        {
            pk.SetShiny();
            if (pk.Format <= 7)
                return;

            var xor = pk.ShinyXor;
            if ((shiny == Shiny.AlwaysStar && xor == 1) || (shiny == Shiny.AlwaysSquare && xor == 0) || ((shiny is Shiny.Always or Shiny.Random) && xor < 2)) // allow xor1 and xor0 for den shinies
                return;
        }
    }

    /// <summary>
    /// Clears all relearn moves for the PKM.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    public static void ClearRelearnMoves(this PKM pk)
    {
        pk.RelearnMove1 = 0;
        pk.RelearnMove2 = 0;
        pk.RelearnMove3 = 0;
        pk.RelearnMove4 = 0;
    }

    /// <summary>
    /// Calculates a shiny PID based on TID, SID, PID, and type.
    /// </summary>
    /// <param name="tid">Trainer ID.</param>
    /// <param name="sid">Secret ID.</param>
    /// <param name="pid">PID value.</param>
    /// <param name="type">Type value.</param>
    /// <returns>The calculated shiny PID.</returns>
    public static uint GetShinyPID(int tid, int sid, uint pid, int type)
    {
        return (uint)(((tid ^ sid ^ (pid & 0xFFFF) ^ type) << 16) | (pid & 0xFFFF));
    }

    /// <summary>
    /// Applies height and weight values to the PKM based on the encounter template.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="enc">The encounter template.</param>
    /// <param name="signed">Apply correlated height/weight/scale values based on PID/EC.</param>
    public static void ApplyHeightWeight(this PKM pk, IEncounterTemplate enc, bool signed = true)
    {
        if (enc is { Generation: < 8, Context: not EntityContext.Gen7b } && pk.Format >= 8) // height and weight don't apply prior to GG
            return;
        if (enc is { Context: EntityContext.Gen9a }) // do not break xoroshiro correlation.
            return;

        if (pk is IScaledSizeValue obj) // Deal with this later -- restrictions on starters/statics/alphas, for now roll with whatever encounter DB provides
        {
            obj.HeightAbsolute = obj.CalcHeightAbsolute;
            obj.WeightAbsolute = obj.CalcWeightAbsolute;
            if (pk is PB7 pb1)
                pb1.ResetCP();

            return;
        }
        if (pk is not IScaledSize size)
            return;

        // fixed height and weight
        bool isFixedScale = enc switch
        {
            EncounterStatic9 { Size: not 0 } => true,
            EncounterTrade8b => true,
            EncounterTrade9 => true,
            EncounterStatic8a { HasFixedHeight: true } => true,
            EncounterStatic8a { HasFixedWeight: true } => true,
            _ => false,
        };
        if (isFixedScale)
            return;

        if (enc is WC8 { IsHOMEGift: true })
            return; // HOME gift. No need to set height and weight

        if (enc is WC9 wc9)
        {
            size.WeightScalar = (byte)wc9.WeightValue;
            size.HeightScalar = (byte)wc9.HeightValue;
            return;
        }

        if (enc is EncounterStatic8N or EncounterStatic8NC or EncounterStatic8ND)
            return;
        if (APILegality.IsPIDIVSet(pk, enc) && enc is not EncounterEgg8b)
            return;

        var height = 0x12;
        var weight = 0x97;
        var scale = 0xFB;
        if (signed)
        {
            if (GameVersion.SWSH.Contains(pk.Version) || GameVersion.BDSP.Contains(pk.Version) || GameVersion.SV.Contains(pk.Version))
            {
                var top = (int)(pk.PID >> 16);
                var bottom = (int)(pk.PID & 0xFFFF);
                height = (top % 0x80) + (bottom % 0x81);
                weight = ((int)(pk.EncryptionConstant >> 16) % 0x80) + ((int)(pk.EncryptionConstant & 0xFFFF) % 0x81);
                scale = ((int)(pk.PID >> 16)*height % 0x80) + ((int)(pk.PID &0xFFFF)*height % 0x81);
            }
            else if (pk.GG)
            {
                height = (int)(pk.PID >> 16) % 0xFF;
                weight = (int)(pk.PID & 0xFFFF) % 0xFF;
                scale = (int)(pk.PID >> 8) % 0xFF;
            }
        }
        else
        {
            height = Util.Rand.Next(255);
            weight = Util.Rand.Next(255);
            scale = Util.Rand.Next(255);
        }
        size.HeightScalar = (byte)height;
        size.WeightScalar = (byte)weight;
        if (pk is IScaledSize3 sz3 && enc is not EncounterFixed9 && sz3.Scale != 128)
            sz3.Scale = (byte)scale;
    }

    /// <summary>
    /// Sets the friendship values for the PKM based on the encounter template.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="enc">The encounter template.</param>
    /// <param name="friendship">The friendship value to set.</param>
    public static void SetFriendship(this PKM pk, IEncounterTemplate enc, byte friendship)
    {
        if (enc.Generation <= 2)
        {
            pk.OriginalTrainerFriendship = (byte)GetBaseFriendship(EntityContext.Gen7, pk.Species, pk.Form); // VC transfers use SM personal info
            return;
        }

        bool wasNeverOriginalTrainer = !HistoryVerifier.GetCanOTHandle(enc, pk, enc.Generation);
        if (wasNeverOriginalTrainer)
        {
            pk.OriginalTrainerFriendship = (byte)GetBaseFriendship(enc);
            pk.HandlingTrainerFriendship = pk.HasMove(218) ? (byte)0 : friendship;
        }
        else
        {
            pk.CurrentFriendship = pk.HasMove(218) ? (byte)0 : friendship;
        }
    }

    /// <summary>
    /// Sets calculated values for Beluga-based PKM.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    public static void SetBelugaValues(this PKM pk)
    {
        if (pk is PB7 pb7)
            pb7.ResetCalculatedValues();
    }

    /// <summary>
    /// Sets Awakened Values (AVs) for a PKM based on a battle template.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="set">The battle template.</param>
    public static void SetAwakenedValues(this PKM pk, IBattleTemplate set)
    {
        if (pk is not PB7 pb7)
            return;

        Span<byte> result = stackalloc byte[6];
        AwakeningUtil.SetExpectedMinimumAVs(pb7, result);

        const byte max = AwakeningUtil.AwakeningMax;
        ReadOnlySpan<int> evs = set.EVs;
        pb7.AV_HP  = (byte)Math.Min(max, Math.Max(result[0], evs[0]));
        pb7.AV_ATK = (byte)Math.Min(max, Math.Max(result[1], evs[1]));
        pb7.AV_DEF = (byte)Math.Min(max, Math.Max(result[2], evs[2]));
        pb7.AV_SPA = (byte)Math.Min(max, Math.Max(result[3], evs[4]));
        pb7.AV_SPD = (byte)Math.Min(max, Math.Max(result[4], evs[5]));
        pb7.AV_SPE = (byte)Math.Min(max, Math.Max(result[5], evs[3]));
    }

    /// <summary>
    /// Sets the handling trainer language for the PKM.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="prefer">Preferred language ID.</param>
    public static void SetHTLanguage(this PKM pk, byte prefer)
    {
        var preferID = (LanguageID)prefer;
        if (preferID is LanguageID.None or LanguageID.UNUSED_6)
            prefer = 2; // prefer english

        if (pk is IHandlerLanguage h)
            h.HandlingTrainerLanguage = prefer;
    }

    /// <summary>
    /// Sets the Gigantamax factor for the PKM based on the battle template and encounter.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="set">The battle template.</param>
    /// <param name="enc">The encounter template.</param>
    public static void SetGigantamaxFactor(this PKM pk, IBattleTemplate set, IEncounterTemplate enc)
    {
        if (pk is not IGigantamax gmax || gmax.CanGigantamax == set.CanGigantamax)
            return;

        if (Gigantamax.CanToggle(pk.Species, pk.Form, enc.Species, enc.Form))
            gmax.CanGigantamax = set.CanGigantamax; // soup hax
    }

    /// <summary>
    /// Sets Dyna/Gimmick values for the PKM based on the battle template.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="set">The battle template.</param>
    public static void SetGimmicks(this PKM pk, IBattleTemplate set)
    {
        if (pk is IDynamaxLevel d)
            d.DynamaxLevel = d.GetSuggestedDynamaxLevel(pk, requested: set.DynamaxLevel);

        if (pk is ITeraType t && set.TeraType != MoveType.Any && t.GetTeraType() != set.TeraType)
            t.SetTeraType(set.TeraType);
    }

    internal static void HyperTrain(this IHyperTrain t, PKM pk, ReadOnlySpan<int> ivs, EncounterCriteria criteria)
    {
        t.HT_HP  = pk.IV_HP  != 31 && criteria.IV_HP is -1;
        t.HT_ATK = pk.IV_ATK != 31 && ivs[1] > 2 && criteria.IV_ATK is -1;
        t.HT_DEF = pk.IV_DEF != 31 && criteria.IV_DEF is -1;
        t.HT_SPA = pk.IV_SPA != 31 && ivs[4] > 2 && criteria.IV_SPA is -1;
        t.HT_SPD = pk.IV_SPD != 31 && criteria.IV_SPD is -1;
        t.HT_SPE = pk.IV_SPE != 31 && ivs[3] > 2 && criteria.IV_SPE is -1;

        if (pk is PB7 pb)
            pb.ResetCP();
    }

    /// <summary>
    /// Sets suggested memories for the PKM based on its type and trade status.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    public static void SetSuggestedMemories(this PKM pk)
    {
        switch (pk)
        {
            case PK9 pk9 when !pk.IsUntraded:
                pk9.ClearMemoriesHT();
                break;
            case PA8 pa8 when !pk.IsUntraded:
                pa8.ClearMemoriesHT();
                break;
            case PB8 pb8 when !pk.IsUntraded:
                pb8.ClearMemoriesHT();
                break;
            case PK8 pk8 when !pk.IsUntraded:
                pk8.SetTradeMemoryHT8();
                break;
            case PK7 pk7 when !pk.IsUntraded:
                pk7.SetTradeMemoryHT6(true);
                break;
            case PK6 pk6 when !pk.IsUntraded:
                pk6.SetTradeMemoryHT6(true);
                break;
        }
    }

    private static int GetBaseFriendship(IEncounterTemplate enc) => enc switch
    {
        IFixedOTFriendship f => f.OriginalTrainerFriendship,
        { Version: GameVersion.BD or GameVersion.SP } => PersonalTable.SWSH.GetFormEntry(enc.Species, enc.Form).BaseFriendship,
        _ => GetBaseFriendship(enc.Context, enc.Species, enc.Form),
    };

    private static int GetBaseFriendship(EntityContext context, ushort species, byte form) => context switch
    {
        EntityContext.Gen1  => PersonalTable.USUM[species].BaseFriendship,
        EntityContext.Gen2  => PersonalTable.USUM[species].BaseFriendship,
        EntityContext.Gen6  => PersonalTable.AO  [species].BaseFriendship,
        EntityContext.Gen7  => PersonalTable.USUM[species].BaseFriendship,
        EntityContext.Gen7b => PersonalTable.GG  [species].BaseFriendship,
        EntityContext.Gen8  => PersonalTable.SWSH[species, form].BaseFriendship,
        EntityContext.Gen8a => PersonalTable.LA  [species, form].BaseFriendship,
        EntityContext.Gen8b => PersonalTable.BDSP[species, form].BaseFriendship,
        EntityContext.Gen9  => PersonalTable.SV  [species, form].BaseFriendship,
        EntityContext.Gen9a => PersonalTable.ZA  [species, form].BaseFriendship,
        _ => throw new IndexOutOfRangeException(),
    };

    /// <summary>
    /// Set TID, SID and OT
    /// </summary>
    /// <param name="pk">PKM to set trainer data to</param>
    /// <param name="trainer">Trainer data</param>
    public static void SetTrainerData(this PKM pk, ITrainerInfo trainer)
    {
        pk.TID16 = trainer.TID16;
        pk.SID16 = pk.Generation >= 3 ? trainer.SID16 : (ushort)0;
        pk.OriginalTrainerName = trainer.OT;
    }

    /// <summary>
    /// Set Handling Trainer data for a given PKM
    /// </summary>
    /// <param name="pk">PKM to modify</param>
    /// <param name="trainer">Trainer to handle the <see cref="pk"/></param>
    /// <param name="enc">Encounter template originated from</param>
    public static void SetHandlerAndMemory(this PKM pk, ITrainerInfo trainer, IEncounterTemplate enc)
    {
        if (TradeRestrictions.IsUntradableEncounter(enc))
            return;
        if (enc.Version == GameVersion.PLA && trainer.Version == GameVersion.PLA) { pk.OriginalTrainerGender = trainer.Gender; } //just use the link cable item HT not required;
        var expect = trainer.IsFromTrainer(pk) ? 0 : 1;
        if (pk.CurrentHandler == expect && expect == 0 && !IsTradeEvolutionRequired((Species)pk.Species, enc))
            return;

        pk.CurrentHandler = 1;
        pk.HandlingTrainerName = trainer.OT;
        pk.HandlingTrainerGender = trainer.Gender;
        pk.SetHTLanguage((byte)trainer.Language);
        pk.SetSuggestedMemories();
    }

    /// <summary>
    /// Set trainer data for a legal PKM
    /// </summary>
    /// <param name="pk">Legal PKM for setting the data</param>
    /// <param name="trainer"></param>
    /// <returns>PKM with the necessary values modified to reflect trainer data changes</returns>
    public static void SetAllTrainerData(this PKM pk, ITrainerInfo trainer)
    {
        pk.SetBelugaValues(); // trainer details changed?

        if (pk is not IGeoTrack gt)
            return;

        if (trainer is not IRegionOrigin o)
        {
            gt.ConsoleRegion = 1; // North America
            gt.Country = 49; // USA
            gt.Region = 7; // California
            return;
        }

        gt.ConsoleRegion = o.ConsoleRegion;
        gt.Country = o.Country;
        gt.Region = o.Region;
        if (pk is PK7 pk7 && pk.Generation <= 2)
            pk7.FixVCRegion();
        else if (pk.Species is (int)Vivillon or (int)Spewpa or (int)Scatterbug)
            pk.FixVivillonRegion();
    }

    /// <summary>
    /// Sets a moveset which is suggested based on calculated legality.
    /// </summary>
    /// <param name="pk">Legal PKM for setting the data</param>
    /// <param name="random">True for Random assortment of legal moves, false if current moves only.</param>
    public static void SetSuggestedMoves(this PKM pk, bool random = false)
    {
        Span<ushort> m = stackalloc ushort[4];
        pk.GetMoveSet(m, random);
        var moves = m.ToArray();
        if (moves.All(z => z == 0))
            return;

        if (pk.Moves.SequenceEqual(moves))
            return;

        pk.SetMoves(moves);
    }

    /// <summary>
    /// Set Dates for date-locked Pokémon
    /// </summary>
    /// <param name="pk">Pokémon file to modify</param>
    /// <param name="enc">encounter used to generate Pokémon file</param>
    public static void SetDateLocks(this PKM pk, IEncounterTemplate enc)
    {
        if (enc is WC8 { IsHOMEGift: true } wc8)
            SetDateLocksWC8(pk, wc8);
    }

    private static void SetDateLocksWC8(PKM pk, WC8 w)
    {
        var locked = w.GetDistributionWindow(out var time);
        if (locked)
            pk.MetDate = time.Start;
    }

    public static bool TryApplyHardcodedSeedWild8(PK8 pk, IEncounterTemplate enc, ReadOnlySpan<int> ivs, Shiny requestedShiny)
    {
        // Don't bother if there is no overworld correlation
        if (enc is not IOverworldCorrelation8 eo)
            return false;

        // Check if a seed exists
        var flawless = Overworld8Search.GetFlawlessIVCount(enc, ivs, out var seed);

        // Ensure requested criteria matches
        if (flawless == -1)
            return false;

        APILegality.FindWildPIDIV8(pk, requestedShiny, flawless, seed);
        return eo.IsOverworldCorrelationCorrect(pk) && requestedShiny switch
        {
            Shiny.AlwaysStar when pk.ShinyXor is 0 or > 15 => false,
            Shiny.Never when pk.ShinyXor < 16 => false,
            _ => true,
        };
    }

    /// <summary>
    /// Checks if a species/form exists in the specified game version.
    /// </summary>
    /// <param name="destVer">Destination game version.</param>
    /// <param name="species">Species ID.</param>
    /// <param name="form">Form ID.</param>
    /// <returns>True if exists, otherwise false.</returns>
    public static bool ExistsInGame(this GameVersion destVer, ushort species, byte form)
    {
        // Don't process if Game is LGPE and requested PKM is not Kanto / Meltan / Melmetal
        // Don't process if Game is SWSH and requested PKM is not from the Galar Dex (Zukan8.DexLookup)
        if (destVer is GameVersion.GP or GameVersion.GE)
            return species is <= 151 or 808 or 809;

        if (GameVersion.SWSH.Contains(destVer))
            return PersonalTable.SWSH.IsPresentInGame(species, form);

        if (GameVersion.PLA.Contains(destVer))
            return PersonalTable.LA.IsPresentInGame(species, form);

        return GameVersion.SV.Contains(destVer) ? PersonalTable.SV.IsPresentInGame(species, form) : (uint)species <= destVer.GetMaxSpeciesID();
    }

    /// <summary>
    /// Applies post-batch fixes to the PKM, such as resetting height and weight.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    public static void ApplyPostBatchFixes(this PKM pk)
    {
        if (pk is IScaledSizeValue sv)
        {
            sv.ResetHeight();
            sv.ResetWeight();
        }
    }
    /// <summary>
    /// Sets the Plus flags for the PKM.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="pi">The PersonalInfo to use.</param>
    /// <param name="seedofMastery">Whether to use Seed of Mastery.</param>
    /// <param name="tms">Whether to use TMs.</param>
    public static void SetPlusFlags(this PKM pk, bool seedofMastery = true, bool tms = true)
    {
        if (pk is IPlusRecord plus && pk.PersonalInfo is IPermitPlus permit)
        {
            plus.ClearPlusFlags(permit.PlusCountTotal);
            plus.SetPlusFlags(permit, new LegalityAnalysis(pk), seedofMastery, tms);
        }
    }
    /// <summary>
    /// Sets record flags for the PKM based on the provided moves.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="moves">Moves to set record flags for.</param>
    public static void SetRecordFlags(this PKM pk, ReadOnlySpan<ushort> moves)
    {
        if (pk is ITechRecord tr and not PA8 and not PA9)
        {
            if (pk.Species == (ushort)Hydrapple)
            {
                ReadOnlySpan<ushort> dc = [(ushort)Move.DragonCheer];
                tr.SetRecordFlags(dc);
            }
            if (moves.Length != 0)
            {
                tr.SetRecordFlags(moves);
            }
            else
            {
                var permit = tr.Permit;
                for (int i = 0; i < permit.RecordCountUsed; i++)
                {
                    if (permit.IsRecordPermitted(i))
                        tr.SetMoveRecordFlag(i);
                }
            }
            return;
        }

        if (pk is IMoveShop8Mastery master)
            master.SetMoveShopFlags(pk);
    }

    /// <summary>
    /// Sets suggested contest stats for the PKM based on the encounter template.
    /// </summary>
    /// <param name="pk">The PKM to modify.</param>
    /// <param name="enc">The encounter template.</param>
    public static void SetSuggestedContestStats(this PKM pk, IEncounterTemplate enc)
    {
        var la = new LegalityAnalysis(pk);
        pk.SetSuggestedContestStats(enc, la.Info.EvoChainsAllGens);
    }

    private static ReadOnlySpan<ushort> ArceusPlateIDs =>
    [
        303, 306, 304, 305, 309, 308, 310, 313, 298, 299, 301, 300, 307, 302, 311, 312, 644,
    ];

    public static ushort? GetArceusHeldItemFromForm(int form) => form is >= 1 and <= 17 ? ArceusPlateIDs[form - 1] : null;

    /// <summary>
    /// Gets the Silvally held item ID from the form.
    /// </summary>
    /// <param name="form">Form index.</param>
    /// <returns>Held item ID or null.</returns>
    public static int? GetSilvallyHeldItemFromForm(int form) => form == 0 ? null : form + 903;

    /// <summary>
    /// Gets the Genesect held item ID from the form.
    /// </summary>
    /// <param name="form">Form index.</param>
    /// <returns>Held item ID or null.</returns>
    public static int? GetGenesectHeldItemFromForm(int form) => form == 0 ? null : form + 115;

    public static bool IsTradeEvolutionRequired(Species species, IEncounterTemplate enc)
    {
        if (enc.Species == (ushort)species) //if its already evolved, doesn't need to be traded;
            return false;
        return species switch
        {
            Alakazam or
            Machamp or
            Golem or
            Gengar or
            Gigalith or
            Conkeldurr or
            Trevenant or
            Gourgeist or
            Politoed or
            Slowking or
            Steelix or
            Scizor or
            Kingdra or
            Porygon2 or
            PorygonZ or
            Rhyperior or
            Electivire or
            Magmortar or
            Dusknoir or
            Huntail or
            Gorebyss or
            Milotic or
            Aromatisse or
            Slurpuff or
            Accelgor or
            Escavalier => true,

            _ => false,
        };
    }
}
