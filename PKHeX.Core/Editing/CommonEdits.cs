using System;

namespace PKHeX.Core;

/// <summary>
/// Contains extension logic for modifying <see cref="PKM"/> data.
/// </summary>
public static class CommonEdits
{
    /// <summary>
    /// Setting which enables/disables automatic manipulation of <see cref="IAppliedMarkings"/> when importing from a <see cref="IBattleTemplate"/>.
    /// </summary>
    public static bool ShowdownSetIVMarkings { get; set; } = true;

    /// <summary>
    /// Setting which causes the <see cref="PKM.StatNature"/> to the <see cref="PKM.Nature"/> in Gen8+ formats.
    /// </summary>
    public static bool ShowdownSetBehaviorNature { get; set; }

    /// <summary>
    /// Sets the <see cref="PKM.Nickname"/> to the provided value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="nick"><see cref="PKM.Nickname"/> to set. If no nickname is provided, the <see cref="PKM.Nickname"/> is set to the default value for its current language and format.</param>
    public static void SetNickname(this PKM pk, string nick)
    {
        if (nick.Length == 0)
        {
            pk.ClearNickname();
            return;
        }
        pk.IsNicknamed = true;
        pk.Nickname = nick;
    }

    /// <summary>
    /// Clears the <see cref="PKM.Nickname"/> to the default value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static string ClearNickname(this PKM pk)
    {
        pk.IsNicknamed = false;
        string nick = SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, pk.Format);
        pk.Nickname = nick;
        if (pk is GBPKM pk12)
            pk12.SetNotNicknamed();
        return nick;
    }

    /// <summary>
    /// Sets the <see cref="PKM.Ability"/> value by sanity checking the provided <see cref="PKM.Ability"/> against the possible pool of abilities.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="abilityID">Desired <see cref="Ability"/> value to set.</param>
    public static void SetAbility(this PKM pk, int abilityID)
    {
        if (abilityID < 0)
            return;
        var index = pk.PersonalInfo.GetIndexOfAbility(abilityID);
        index = Math.Max(0, index);
        pk.SetAbilityIndex(index);
    }

    /// <summary>
    /// Sets the <see cref="PKM.Ability"/> value based on the provided ability index (0-2)
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Desired <see cref="PKM.AbilityNumber"/> (shifted by 1) to set.</param>
    public static void SetAbilityIndex(this PKM pk, int index)
    {
        if (pk is PK5 pk5 && index == 2)
            pk5.HiddenAbility = true;
        else if (pk.Format <= 5)
            pk.PID = EntityPID.GetRandomPID(Util.Rand, pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Form, (uint)(index * 0x10001));
        pk.RefreshAbility(index);
    }

    /// <summary>
    /// Sets a Random <see cref="PKM.EncryptionConstant"/> value. The <see cref="PKM.EncryptionConstant"/> is not updated if the value should match the <see cref="PKM.PID"/> instead.
    /// </summary>
    /// <remarks>Accounts for Wurmple evolutions.</remarks>
    /// <param name="pk">Pokémon to modify.</param>
    public static void SetRandomEC(this PKM pk)
    {
        var gen = pk.Generation;
        if (gen is 3 or 4 or 5)
        {
            pk.EncryptionConstant = pk.PID;
            return;
        }
        pk.EncryptionConstant = GetComplicatedEC(pk);
    }

    /// <summary>
    /// Sets the <see cref="PKM.IsShiny"/> derived value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="shiny">Desired <see cref="PKM.IsShiny"/> state to set.</param>
    public static bool SetIsShiny(this PKM pk, bool shiny) => shiny ? SetShiny(pk) : pk.SetUnshiny();

    /// <summary>
    /// Makes a <see cref="PKM"/> shiny.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="type">Shiny type to force. Only use Always* or Random</param>
    /// <returns>Returns true if the <see cref="PKM"/> data was modified.</returns>
    public static bool SetShiny(PKM pk, Shiny type = Shiny.Random)
    {
        if (pk.IsShiny && type.IsValid(pk))
            return false;

        if (type == Shiny.Random || pk.FatefulEncounter || pk.Version == GameVersion.GO || pk.Format <= 2)
        {
            pk.SetShiny();
            return true;
        }

        do { pk.SetShiny(); }
        while (!type.IsValid(pk));

        return true;
    }

    /// <summary>
    /// Makes a <see cref="PKM"/> not-shiny.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <returns>Returns true if the <see cref="PKM"/> data was modified.</returns>
    public static bool SetUnshiny(this PKM pk)
    {
        if (!pk.IsShiny)
            return false;

        pk.SetPIDGender(pk.Gender);
        return true;
    }

    /// <summary>
    /// Sets the <see cref="PKM.Nature"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Nature"/> value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="nature">Desired <see cref="PKM.Nature"/> value to set.</param>
    public static void SetNature(this PKM pk, Nature nature)
    {
        if (!nature.IsFixed())
            nature = 0; // default valid

        var format = pk.Format;
        if (format >= 8)
            pk.StatNature = nature;
        else if (format is 3 or 4)
            pk.SetPIDNature(nature);
        else
            pk.Nature = nature;
    }

    /// <summary>
    /// Copies <see cref="IBattleTemplate"/> details to the <see cref="PKM"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="set"><see cref="IBattleTemplate"/> details to copy from.</param>
    public static void ApplySetDetails(this PKM pk, IBattleTemplate set)
    {
        pk.Species = Math.Min(pk.MaxSpeciesID, set.Species);
        pk.Form = set.Form;

        ReadOnlySpan<ushort> moves = set.Moves;
        if (moves[0] != 0)
            pk.SetMoves(moves, true);
        if (Legal.IsPPUpAvailable(pk))
            pk.SetMaximumPPUps(moves);

        pk.ApplyHeldItem(set.HeldItem, set.Context);
        pk.CurrentLevel = set.Level;
        pk.CurrentFriendship = set.Friendship;

        ReadOnlySpan<int> ivs = set.IVs;
        ReadOnlySpan<int> evs = set.EVs;
        pk.SetIVs(ivs);

        if (pk is GBPKM gb)
        {
            // In Generation 1/2 Format sets, when IVs are not specified with a Hidden Power set, we might not have the hidden power type.
            // Under this scenario, just force the Hidden Power type.
            if (moves.Contains((ushort)Move.HiddenPower) && gb.HPType != set.HiddenPowerType)
            {
                if (ivs.ContainsAny(30, 31))
                    gb.SetHiddenPower(set.HiddenPowerType);
            }

            // In Generation 1/2 Format sets, when EVs are not specified at all, it implies maximum EVs instead!
            // Under this scenario, just apply maximum EVs (65535).
            if (!evs.ContainsAnyExcept(0))
                gb.MaxEVs();
            else
                gb.SetEVs(evs);
        }
        else
        {
            pk.SetEVs(evs);
        }

        // IVs have no side effects such as hidden power type in gen 8
        // therefore all specified IVs are deliberate and should not be Hyper Trained for Pokémon met in gen 8
        if (pk.Generation < 8)
            pk.SetSuggestedHyperTrainingData(ivs);

        if (ShowdownSetIVMarkings)
            pk.SetMarkings();

        pk.SetNickname(set.Nickname);
        pk.SetSaneGender(set.Gender);

        if (pk.Format >= 3)
        {
            pk.SetAbility(set.Ability);
            pk.SetNature(set.Nature);
        }

        pk.SetIsShiny(set.Shiny);
        pk.SetRandomEC();

        if (pk is IAwakened a)
        {
            a.SetSuggestedAwakenedValues(pk);
            if (pk is PB7 b)
            {
                for (int i = 0; i < 6; i++)
                    b.SetEV(i, 0);
                b.ResetCalculatedValues();
            }
        }
        if (pk is IGanbaru g)
            g.SetSuggestedGanbaruValues(pk);

        if (pk is IGigantamax c)
            c.CanGigantamax = set.CanGigantamax;
        if (pk is IDynamaxLevel d)
            d.DynamaxLevel = d.GetSuggestedDynamaxLevel(pk, requested: set.DynamaxLevel);
        if (pk is ITeraType tera)
        {
            var type = set.TeraType == MoveType.Any ? (MoveType)pk.PersonalInfo.Type1 : set.TeraType;
            tera.SetTeraType(type);
        }

        if (pk is IMoveShop8Mastery s)
            s.SetMoveShopFlags(set.Moves, pk);

        if (ShowdownSetBehaviorNature && pk.Format >= 8)
            pk.Nature = pk.StatNature;

        var legal = new LegalityAnalysis(pk);
        if (pk is ITechRecord t)
        {
            t.ClearRecordFlags();
            t.SetRecordFlags(set.Moves, legal.Info.EvoChainsAllGens.Get(pk.Context));
        }
        if (legal.Parsed && !MoveResult.AllValid(legal.Info.Relearn))
            pk.SetRelearnMoves(legal);
        pk.ResetPartyStats();
        pk.RefreshChecksum();
    }

    /// <summary>
    /// Sets the <see cref="PKM.HeldItem"/> value depending on the current format and the provided item index &amp; format.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="item">Held Item to apply</param>
    /// <param name="context">Format required for importing</param>
    public static void ApplyHeldItem(this PKM pk, int item, EntityContext context)
    {
        item = ItemConverter.GetItemForFormat(item, context, pk.Context);
        pk.HeldItem = ((uint)item > pk.MaxItemID) ? 0 : item;
    }

    /// <summary>
    /// Sets one of the <see cref="EffortValues"/> based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Index to set to</param>
    /// <param name="value">Value to set</param>
    public static int SetEV(this PKM pk, int index, int value) => index switch
    {
        0 => pk.EV_HP = value,
        1 => pk.EV_ATK = value,
        2 => pk.EV_DEF = value,
        3 => pk.EV_SPE = value,
        4 => pk.EV_SPA = value,
        5 => pk.EV_SPD = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    /// <summary>
    /// Sets one of the <see cref="PKM.IVs"/> based on its index within the array.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Index to set to</param>
    /// <param name="value">Value to set</param>
    public static int SetIV(this PKM pk, int index, int value) => index switch
    {
        0 => pk.IV_HP = value,
        1 => pk.IV_ATK = value,
        2 => pk.IV_DEF = value,
        3 => pk.IV_SPE = value,
        4 => pk.IV_SPA = value,
        5 => pk.IV_SPD = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    /// <summary>
    /// Fetches the highest value the provided <see cref="EffortValues"/> index can be while considering others.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Index to fetch for</param>
    /// <returns>Highest value the value can be.</returns>
    public static int GetMaximumEV(this PKM pk, int index)
    {
        if (pk.Format < 3)
            return EffortValues.Max12;

        var sum = pk.EVTotal - pk.GetEV(index);
        int remaining = EffortValues.Max510 - sum;
        return Math.Clamp(remaining, 0, EffortValues.Max252);
    }

    /// <summary>
    /// Fetches the highest value the provided <see cref="PKM.IVs"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Index to fetch for</param>
    /// <param name="allow30">Causes the returned value to be dropped down -1 if the value is already at a maximum.</param>
    /// <returns>Highest value the value can be.</returns>
    public static int GetMaximumIV(this PKM pk, int index, bool allow30 = false)
    {
        if (pk.GetIV(index) == pk.MaxIV && allow30)
            return pk.MaxIV - 1;
        return pk.MaxIV;
    }

    /// <summary>
    /// Force hatches a PKM by applying the current species name and a valid Met Location from the origin game.
    /// </summary>
    /// <param name="pk">PKM to apply hatch details to</param>
    /// <param name="tr">Trainer to force hatch with if Version is not currently set.</param>
    /// <param name="reHatch">Re-hatch already hatched <see cref="PKM"/> inputs</param>
    public static void ForceHatchPKM(this PKM pk, ITrainerInfo? tr = null, bool reHatch = false)
    {
        if (!pk.IsEgg && !reHatch)
            return;
        pk.IsEgg = false;
        pk.ClearNickname();
        pk.OriginalTrainerFriendship = Math.Min(pk.OriginalTrainerFriendship, EggStateLegality.GetEggHatchFriendship(pk.Context));
        if (pk.IsTradedEgg)
            pk.EggLocation = pk.MetLocation;
        if (pk.Version == 0)
            pk.Version = EggStateLegality.GetEggHatchVersion(pk, tr?.Version ?? RecentTrainerCache.Version);
        var loc = EncounterSuggestion.GetSuggestedEggMetLocation(pk);
        if (loc != EncounterSuggestion.LocationNone)
            pk.MetLocation = loc;
        if (pk.Format >= 4)
            pk.MetDate = EncounterDate.GetDate(pk.Context.GetConsole());
        if (pk.Gen6)
            pk.SetHatchMemory6();
    }

    /// <summary>
    /// Force hatches a PKM by applying the current species name and a valid Met Location from the origin game.
    /// </summary>
    /// <param name="pk">PKM to apply hatch details to</param>
    /// <param name="origin">Game the egg originated from</param>
    /// <param name="dest">Game the egg is currently present on</param>
    public static void SetEggMetData(this PKM pk, GameVersion origin, GameVersion dest)
    {
        if (pk.Format < 4)
            return;

        var console = pk.Context.GetConsole();
        var date = EncounterDate.GetDate(console);
        var today = pk.MetDate = date;
        bool traded = origin != dest;
        pk.EggLocation = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(pk.Generation, origin, traded);
        pk.EggMetDate = today;
    }

    /// <summary>
    /// Maximizes the <see cref="PKM.CurrentFriendship"/>. If the <see cref="PKM.IsEgg"/>, the hatch counter is set to 1.
    /// </summary>
    /// <param name="pk">PKM to apply hatch details to</param>
    public static void MaximizeFriendship(this PKM pk)
    {
        if (pk.IsEgg)
            pk.OriginalTrainerFriendship = 1;
        else
            pk.CurrentFriendship = byte.MaxValue;
        if (pk is ICombatPower pb)
            pb.ResetCP();
    }

    /// <summary>
    /// Maximizes the <see cref="PKM.CurrentLevel"/>. If the <see cref="PKM.IsEgg"/>, the <see cref="PKM"/> is ignored.
    /// </summary>
    /// <param name="pk">PKM to apply hatch details to</param>
    public static void MaximizeLevel(this PKM pk)
    {
        if (pk.IsEgg)
            return;
        pk.CurrentLevel = 100;
        if (pk is ICombatPower pb)
            pb.ResetCP();
    }

    /// <summary>
    /// Sets the <see cref="PKM.Nickname"/> to its default value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="la">Precomputed optional</param>
    public static void SetDefaultNickname(this PKM pk, LegalityAnalysis la)
    {
        if (la is { Parsed: true, EncounterOriginal: IFixedNickname {IsFixedNickname: true} t })
            pk.SetNickname(t.GetNickname(pk.Language));
        else
            pk.ClearNickname();
    }

    /// <summary>
    /// Sets the <see cref="PKM.Nickname"/> to its default value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static void SetDefaultNickname(this PKM pk) => pk.SetDefaultNickname(new LegalityAnalysis(pk));

    // Extensions
    /// <summary>
    /// Gets the Location Name for the <see cref="PKM"/>
    /// </summary>
    /// <param name="pk">PKM to fetch data for</param>
    /// <param name="eggmet">Location requested is the egg obtained location, not met location.</param>
    /// <returns>Location string</returns>
    public static string GetLocationString(this PKM pk, bool eggmet)
    {
        if (pk.Format < 2)
            return string.Empty;

        ushort location = eggmet ? pk.EggLocation : pk.MetLocation;
        return GameInfo.GetLocationName(eggmet, location, pk.Format, pk.Generation, pk.Version);
    }

    /// <summary>
    /// Gets a <see cref="PKM.EncryptionConstant"/> to match the requested option.
    /// </summary>
    public static uint GetComplicatedEC(ISpeciesForm pk, char option = default)
    {
        var species = pk.Species;
        var form = pk.Form;
        return GetComplicatedEC(species, form, option);
    }

    /// <inheritdoc cref="GetComplicatedEC(ISpeciesForm,char)"/>
    public static uint GetComplicatedEC(ushort species, byte form, char option = default)
    {
        var rng = Util.Rand;
        uint rand = rng.Rand32();
        uint mod, noise;
        if (species is >= (int)Species.Wurmple and <= (int)Species.Dustox)
        {
            mod = 10;
            bool lower = option is '0' or 'B' or 'S' || WurmpleUtil.GetWurmpleEvoGroup(species) == 0;
            noise = (lower ? 0u : 5u) + (uint)rng.Next(0, 5);
        }
        else if (species is (int)Species.Dunsparce or (int)Species.Dudunsparce or (int)Species.Tandemaus or (int)Species.Maushold)
        {
            mod = 100;
            noise = species switch
            {
                // Retain requisite correlation to allow for evolving into this species too.
                (int)Species.Dudunsparce => form == 1 ? 0 : (uint)rng.Next(1, 100), // 3 Segment
                (int)Species.Maushold => form == 0 ? 0 : (uint)rng.Next(1, 100), // Family of 3

                // Otherwise, check if one is preferred, and if not, just make it the more common outcome.
                _ => option switch
                {
                    '0' or '3' => 0u,
                    _ => (uint)rng.Next(1, 100),
                },
            };
        }
        else if (option is >= '0' and <= '5')
        {
            mod = 6;
            noise = (uint)(option - '0');
        }
        else
        {
            return rand;
        }
        return unchecked(rand - (rand % mod) + noise);
    }
}
