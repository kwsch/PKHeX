using System;
using System.Linq;

namespace PKHeX.Core.AutoMod;

/// <summary>
/// Modifications for a <see cref="PKM"/> based on a <see cref="IBattleTemplate"/>
/// </summary>
public static class ShowdownEdits
{
    /// <summary>
    /// Quick Gender Toggle
    /// </summary>
    /// <param name="pk">PKM whose gender needs to be toggled</param>
    /// <param name="set">Showdown Set for Gender reference</param>
    public static void FixGender(this PKM pk, IBattleTemplate set)
    {
        pk.ApplySetGender(set);
        var la = new LegalityAnalysis(pk);
        if (la.Valid)
            return;

        var genderMismatch = la.Results.Any(z => z.Result == LegalityCheckResultCode.PIDGenderMismatch);
        if (genderMismatch)
            pk.Gender = pk.Gender == 0 ? (byte)1 : (byte)0;

        if (pk.Gender is not 0 and not 1)
            pk.Gender = pk.GetSaneGender();
    }

    /// <summary>
    /// Sets the nature of the given <see cref="PKM"/> based on the provided <see cref="IBattleTemplate"/> and <see cref="IEncounterTemplate"/>.
    /// Handles special cases for Toxtricity, neutral natures, and legality checks for generations other than 3 or 4.
    /// </summary>
    /// <param name="pk">The <see cref="PKM"/> to modify.</param>
    /// <param name="set">The <see cref="IBattleTemplate"/> containing the desired nature information.</param>
    /// <param name="enc">The <see cref="IEncounterTemplate"/> providing encounter details for legality checks.</param>
    public static void SetNature(PKM pk, IBattleTemplate set, IEncounterTemplate enc)
    {
        if (pk.Nature == set.Nature || set.Nature == Nature.Random)
            return;
        if (enc.Generation <= 2 && pk.Format >= 7)
        {
            if (pk.MetLevel == 2)
            {
                if (!Experience.IsValidNatureMetLevel2(pk.CurrentLevel, set.Nature) && pk.CurrentLevel != 2)
                    pk.MetLevel = 3;
            }
        }
        var val = set.Nature <= Nature.Quirky ? set.Nature : Nature.Hardy;
        if (pk.Species == (ushort)Species.Toxtricity)
        {
            if (pk.Form == ToxtricityUtil.GetAmpLowKeyResult(val))
                pk.Nature = val; // StatAlignment already set

            if (pk.Format >= 8 && pk.StatAlignment != pk.Nature && pk.StatAlignment != Nature.Serious && (pk.StatAlignment > Nature.Quirky || (int)pk.StatAlignment % 6 == 0)) // Only Serious Mint for Neutral Natures
                pk.StatAlignment = Nature.Serious;

            return;
        }
        if (enc is IEncounter9a || enc is WA9)
        {
            pk.StatAlignment = val;
            if (pk.StatAlignment is 0 or Nature.Docile or Nature.Bashful or >= Nature.Quirky) // Only Serious Mint for Neutral Natures
                pk.StatAlignment = Nature.Serious;
            return;
        }
        pk.SetNature(val);
        if (enc.Generation is not (3 or 4))
        {
            var orig = pk.Nature;
            if (orig == val)
                return;

            var la = new LegalityAnalysis(pk);
            pk.Nature = val;
            var la2 = new LegalityAnalysis(pk);
            var enc1 = la.EncounterMatch;
            var enc2 = la2.EncounterMatch;
            if (enc is not IEncounterEgg && ((!ReferenceEquals(enc1, enc2) && enc1 is not IEncounterEgg) || la2.Results.Any(z => z.Identifier is CheckIdentifier.Nature or CheckIdentifier.Encounter && !z.Valid)))
                pk.Nature = orig;
        }
        if (pk.Format >= 8 && pk.StatAlignment != pk.Nature && pk.StatAlignment is 0 or Nature.Docile or Nature.Bashful or >= Nature.Quirky) // Only Serious Mint for Neutral Natures
            pk.StatAlignment = Nature.Serious;
    }

    /// <summary>
    /// Sets the ability of the given <see cref="PKM"/> based on the provided <see cref="IBattleTemplate"/> and <see cref="AbilityPermission"/> preference.
    /// Handles hidden abilities, ability number selection, and special cases for transferred Pokémon.
    /// </summary>
    /// <param name="pk">The <see cref="PKM"/> to modify.</param>
    /// <param name="set">The <see cref="IBattleTemplate"/> containing the desired ability information.</param>
    /// <param name="preference">The <see cref="AbilityPermission"/> indicating the preferred ability slot.</param>
    public static void SetAbility(PKM pk, IBattleTemplate set, AbilityPermission preference)
    {
        if (pk.ZA)
            return;
        if (pk.Ability != set.Ability)
            pk.RefreshAbility(pk is PK5 { HiddenAbility: true } ? 2 : pk.AbilityNumber >> 1);
        if (pk.Ability != set.Ability && pk.Context >= EntityContext.Gen6 && set.Ability != -1)
            pk.RefreshAbility(pk is PK5 { HiddenAbility: true } ? 2 : pk.PersonalInfo.GetIndexOfAbility(set.Ability));
        if (preference <= 0)
            return;

        var pi = pk.PersonalInfo;
        var pref = preference.GetSingleValue();
        // Set unspecified abilities
        if (set.Ability == -1)
        {
            pk.RefreshAbility(pref);
            if (pk is PK5 pk5 && preference == AbilityPermission.OnlyHidden)
                pk5.HiddenAbility = true;
        }
        // Set preferred ability number if applicable
        if (pref == 2 && pi is IPersonalAbility12H h && h.AbilityH == set.Ability)
            pk.AbilityNumber = (int)preference;
        // 3/4/5 transferred to 6+ will have ability 1 if both abilitynum 1 and 2 are the same. Capsule cant convert 1 -> 2 if the abilities arnt unique
        if (pk is { Format: >= 6, Generation: 3 or 4 or 5, AbilityNumber: not 4 } && pi is IPersonalAbility12 a && a.Ability1 == a.Ability2)
            pk.AbilityNumber = 1;
        if (pk is G3PKM && pi is IPersonalAbility12 b && b.Ability1 == b.Ability2)
            pk.AbilityNumber = 1;
    }

    /// <summary>
    /// Set Species and Level with nickname (Helps with PreEvos)
    /// </summary>
    /// <param name="pk">PKM to modify</param>
    /// <param name="set">Set to use as reference</param>
    /// <param name="form">Form to apply</param>
    /// <param name="enc">Encounter detail</param>
    /// <param name="lang">Language to apply</param>
    public static void SetSpeciesLevel(this PKM pk, IBattleTemplate set, byte form, IEncounterTemplate enc, LanguageID? lang)
    {
        var currentlang = (LanguageID)pk.Language;
        pk.ApplySetGender(set);
        pk.SetRecordFlags(set.Moves); // Set record flags before evolution (TODO: what if middle evolution has exclusive record moves??)

        var evolutionRequired = enc.Species != set.Species;
        var formchange = form != pk.Form;
        var UnownFormSet = pk.Species == (ushort)Species.Unown && enc.Generation is 3 or 4;
        if (evolutionRequired)
            pk.Species = set.Species;
            
        if (formchange && !UnownFormSet)
            pk.Form = form;
        if (enc.Version is GameVersion.BD or GameVersion.SP && pk.Species == (ushort)Species.Unown)
            pk.MetLocation = GetBDSPUnownMetLocation(form);
        if ((evolutionRequired || formchange) && pk is IScaledSizeValue sv)
        {
            sv.HeightAbsolute = sv.CalcHeightAbsolute;
            sv.WeightAbsolute = sv.CalcWeightAbsolute;
        }

        // Don't allow invalid Toxtricity nature, set random Nature first and then StatAlignment later
        if (pk.Species == (int)Species.Toxtricity)
        {
            while (true)
            {
                var result = ToxtricityUtil.GetAmpLowKeyResult(pk.Nature);
                if (result == pk.Form)
                    break;

                pk.Nature = (Nature)Util.Rand.Next(25);
            }
        }

        pk.SetSuggestedFormArgument(pk.Species, pk.Form, pk.Context, new LegalityAnalysis(pk).Info.EvoChainsAllGens, enc.Species);
        if (evolutionRequired || formchange || (pk.Ability != set.Ability && set.Ability != -1))
        {
            var abilitypref = (AbilityPermission)pk.PersonalInfo.GetIndexOfAbility(set.Ability);
            SetAbility(pk, set, abilitypref);
        }
        if (pk.CurrentLevel != set.Level)
            pk.CurrentLevel = set.Level;

        if (pk.MetLevel > pk.CurrentLevel)
            pk.MetLevel = pk.CurrentLevel;

        if (set.Level != 100 && set.Level == enc.LevelMin && pk.Format is 3 or 4)
            pk.EXP = Experience.GetEXP((byte)(enc.LevelMin + 1), PersonalTable.HGSS[enc.Species].EXPGrowth) - 1;

        var finallang = lang ?? currentlang;
        if (finallang == LanguageID.None)
            finallang = LanguageID.English;

        pk.Language = (int)finallang;

        // check if nickname even needs to be updated
        if (set.Nickname.Length == 0 && finallang == currentlang && !evolutionRequired)
            return;

        if (enc is IFixedTrainer { IsFixedTrainer: true })
        {
            if (enc is EncounterTrade1 && pk.Context >= EntityContext.Gen7)
            {
                pk.OriginalTrainerName = (LanguageID)pk.Language switch
                {
                    LanguageID.Japanese => "トレーナー",
                    LanguageID.French => "Dresseur",
                    LanguageID.Italian => "Allenatore",
                    LanguageID.Spanish => "Entrenador",
                    _ => "Trainer",
                };
            }
        }
        // don't bother checking EncounterTrade nicknames for length validity
        if (enc is IFixedNickname { IsFixedNickname: true } et)
        {
            // Nickname matches the requested nickname already
            if (pk.Nickname == set.Nickname)
                return;
            // This should be illegal except Meister Magikarp in BD/SP, however trust the user and set corresponding OT
            var nick = et.GetNickname(pk.Language);
            if (!string.IsNullOrWhiteSpace(nick))
            {
                pk.Nickname = nick;
                return;
            }
        }

        var gen = enc.Generation;
        var maxlen = Legal.GetMaxLengthNickname(gen, finallang);
        var newnick = RegenUtil.MutateNickname(set.Nickname, finallang, pk.Version);
        if (pk.Format < 3 && newnick.Length == 0)
            newnick = SpeciesName.GetSpeciesName(pk.Species, (int)finallang);
        var nickname = newnick.Length > maxlen ? newnick[..maxlen] : newnick;
        if (!WordFilter.IsFiltered(nickname, pk.Context, out _, out _) && newnick != SpeciesName.GetSpeciesName(pk.Species, (int)finallang))
            pk.SetNickname(nickname);
        else
            pk.ClearNickname();
    }

    /// <summary>
    /// Applies specified gender (if it exists. Else choose specied gender)
    /// </summary>
    /// <param name="pk">PKM to modify</param>
    /// <param name="set">Template to grab the set gender</param>
    private static void ApplySetGender(this PKM pk, IBattleTemplate set)
    {
        pk.Gender = set.Gender ?? pk.GetSaneGender();
    }

    /// <summary>
    /// Function to check if there is any PID Gender Mismatch
    /// </summary>
    /// <param name="pkm">PKM to modify</param>
    /// <param name="enc">Base encounter</param>
    /// <returns>boolean indicating if the gender is valid</returns>
    public static bool IsValidGenderPID(this PKM pkm, IEncounterTemplate enc)
    {
        bool genderValid = pkm.IsGenderValid();
        if (!genderValid)
            return IsValidGenderMismatch(pkm);

        // check for mixed->fixed gender incompatibility by checking the gender of the original species
        return !SpeciesCategory.IsFixedGenderFromDual(pkm.Species) || IsValidFixedGenderFromBiGender(pkm, enc.Species);
    }

    /// <summary>
    /// Helper function to check if bigender => fixed gender evolution is valid
    /// </summary>
    /// <param name="pkm">pkm to modify</param>
    /// <param name="original">original species (encounter)</param>
    /// <returns>boolean indicating validaity</returns>
    private static bool IsValidFixedGenderFromBiGender(PKM pkm, ushort original)
    {
        var current = pkm.Gender;
        if (current == 2) // Shedinja, genderless
            return true;

        var gender = EntityGender.GetFromPID(original, pkm.EncryptionConstant);
        return gender == current;
    }

    /// <summary>
    /// Check if a gender mismatch is a valid possibility
    /// </summary>
    /// <param name="pkm">PKM to modify</param>
    /// <returns>boolean indicating validity</returns>
    private static bool IsValidGenderMismatch(PKM pkm) => pkm.Species switch
    {
        // Shedinja evolution gender glitch, should match original Gender
        (int)Species.Shedinja when pkm.Format == 4 => pkm.Gender == EntityGender.GetFromPIDAndRatio(pkm.EncryptionConstant, 0x7F), // 50M-50F
        // Evolved from Azurill after transferring to keep gender
        (int)Species.Marill or (int)Species.Azumarill when pkm.Format >= 6 => pkm.Gender == 1 && (pkm.EncryptionConstant & 0xFF) > 0x3F,
        _ => false,
    };

    /// <summary>
    /// Set Moves for a specific PKM. These should not affect legality after being vetted by GeneratePKMs
    /// </summary>
    /// <param name="pk">PKM to modify</param>
    /// <param name="set">Showdown Set to refer</param>
    /// <param name="enc">Encounter to reference</param>
    public static void SetALMMoves(this PKM pk, IBattleTemplate set, IEncounterTemplate enc)
    {
        // If no moves are requested, just keep the encounter moves
        if (set.Moves[0] != 0)
            pk.SetMoves(set.Moves, Legal.IsPPUpAvailable(pk));
        
        var la = new LegalityAnalysis(pk);
        if (enc is IEncounter9a or WA9 && set.Moves.Count(z=>z != 0) < 4)
        {
            Span<ushort> suggmoves = stackalloc ushort[4];
            la.GetSuggestedCurrentMoves(suggmoves);
            ushort[] movesArr = [.. set.Moves];
            int srcIndex = 0;
            for (int i = 0; i < movesArr.Length; i++)
            {
                if (movesArr[i] == 0)
                {
                    // Skip any source values already in target
                    while (srcIndex < suggmoves.Length && movesArr.Contains(suggmoves[srcIndex]))
                        srcIndex++;

                    if (srcIndex < suggmoves.Length)
                        movesArr[i] = suggmoves[srcIndex];
                }
            }
            pk.SetMoves(movesArr, Legal.IsPPUpAvailable(pk));

        }
        // Remove invalid encounter moves (eg. Kyurem Encounter -> Requested Kyurem black)
        if (set.Moves[0] == 0 && la.Info.Moves.Any(z => z.Judgement == Severity.Invalid))
        {
            Span<ushort> moves = stackalloc ushort[4];
            la.GetSuggestedCurrentMoves(moves);
            pk.SetMoves(moves, pk is not PA8 and not PA9);
            pk.FixMoves();
        }

        if (la.Parsed && !pk.FatefulEncounter)
        {
            // For dexnav. Certain encounters come with "random" relearn moves, and our requested moves might require one of them.
            Span<ushort> moves = stackalloc ushort[4];
            la.GetSuggestedRelearnMoves(moves, enc);
            pk.ClearRelearnMoves();
            pk.SetRelearnMoves(moves);
        }
        la = new LegalityAnalysis(pk);
        if (la.Info.Relearn.Any(z => z.Judgement == Severity.Invalid))
            pk.ClearRelearnMoves();

    }

    public static void SetEggMoves(this PKM pk, IBattleTemplate set, IEncounterTemplate enc)
    {
        var moverequests = set.Moves.Where(z => !pk.Moves.Contains(z) && z != 0).ToList();
        var moves = pk.Moves.Where(z=> z != 0 ).ToList();
        var voltTackle = moverequests.Contains((ushort)Move.VoltTackle) && pk.Species == (ushort)Species.Pichu;
        moverequests.Remove((ushort)Move.VoltTackle);
        for (int i = 0; i < moverequests.Count; i++)
        {
            if (moves.Count == 4)
                moves.RemoveAt(0);
            moves.Add(moverequests[i]);
        }
        if (voltTackle)
        {
            if (moves.Count == 4)
                moves.RemoveAt(0);
            moves.Add((ushort)Move.VoltTackle);
        }
        if (moves.Count != 0)
            pk.SetMoves(moves.ToArray(), false);

        var la = new LegalityAnalysis(pk);
        // Remove invalid encounter moves (eg. Kyurem Encounter -> Requested Kyurem black)
        if (set.Moves[0] == 0 && la.Info.Moves.Any(z => z.Judgement == Severity.Invalid))
        {
            Span<ushort> Moves = stackalloc ushort[4];
            la.GetSuggestedCurrentMoves(Moves);
            pk.SetMoves(Moves, pk is not PA8);
            pk.FixMoves();
        }

        if (la.Parsed && !pk.FatefulEncounter)
        {
            // For dexnav. Certain encounters come with "random" relearn moves, and our requested moves might require one of them.
            Span<ushort> Moves = stackalloc ushort[4];
            la.GetSuggestedRelearnMoves(Moves, enc);
            pk.ClearRelearnMoves();
            pk.SetRelearnMoves(Moves);
        }
        la = new LegalityAnalysis(pk);
        if (la.Info.Relearn.Any(z => z.Judgement == Severity.Invalid))
            pk.ClearRelearnMoves();
    }
    /// <summary>
    /// Set EVs and Items for a specific PKM. These should not affect legality after being vetted by GeneratePKMs
    /// </summary>
    /// <param name="pk">PKM to modify</param>
    /// <param name="set">Showdown Set to refer</param>
    /// <param name="enc">Encounter to reference</param>
    public static void SetEV(this PKM pk, IBattleTemplate set)
    {
        if (pk is IAwakened)
        {
            pk.SetAwakenedValues(set);
            return;
        }
        // In Generation 1/2 Format sets, when EVs are not specified at all, it implies maximum EVs instead!
        // Under this scenario, just apply maximum EVs (65535).
        if (pk is GBPKM gb && set.EVs.All(z => z == 0))
        {
            gb.MaxEVs();
            return;
        }
        if (set.IsChampions)
            pk.SetEVsChampions(set.EVs);
        else
            pk.SetEVs(set.EVs);
    }
    /// <summary>
    /// Convert Champions EVs to regular EVs and set them for the PKM. Champions EVs are on a different scale, so they need to be converted before being applied.
    /// </summary>
    /// <param name="pk"></param>
    /// <param name="evs"></param>
    public static void SetEVsChampions(this PKM pk, ReadOnlySpan<int> evs)
    {
        Span<int> final = stackalloc int[6];
        EffortValues.ConvertFromChampions(evs, final);
        pk.SetEVs(final);
    }
    /// <summary>
    /// Set encounter trade IVs for a specific encounter trade
    /// </summary>
    /// <param name="pk">Pokemon to modify</param>
    public static void SetEncounterTradeIVs(this PKM pk)
    {
        pk.SetRandomIVs(minFlawless: 3);
    }

    /// <summary>
    /// Set held items after sanity checking for forms and invalid items
    /// </summary>
    /// <param name="pk">Pokemon to modify</param>
    /// <param name="set">IBattleset to grab the item</param>
    public static void SetHeldItem(this PKM pk, IBattleTemplate set)
    {
        pk.ApplyHeldItem(set.HeldItem, set.Context);
        pk.FixInvalidFormItems(); // arceus, silvally, giratina, genesect fix
        if (!ItemRestrictions.IsHeldItemAllowed(pk) || pk is PB7)
            pk.HeldItem = 0; // Remove the item if the item is illegal in its generation
    }

    /// <summary>
    /// Fix invalid form items
    /// </summary>
    /// <param name="pk">Pokemon to modify</param>
    private static void FixInvalidFormItems(this PKM pk)
    {
        // Ignore games where items don't exist in the first place. They would still allow forms
        if (pk.LA)
            return;

        switch ((Species)pk.Species)
        {
            case Species.Arceus:
            case Species.Silvally:
            case Species.Genesect:
                bool valid = FormItem.TryGetForm(pk.Species, pk.HeldItem, pk.Format, out byte pkform);
                if (!valid)
                    break;

                pk.HeldItem = pk.Form != pkform ? 0 : pk.HeldItem;
                pk.Form = pk.Form != pkform ? (byte)0 : pkform;
                break;
            case Species.Giratina
                when pk is { Form: 1, HeldItem: not 112 and not 1779 }:
                if (pk.Context >= EntityContext.Gen9)
                    pk.HeldItem = 1779;
                else
                    pk.HeldItem = 112;

                break;
            case Species.Dialga when pk is { Form: 1, HeldItem: not 1777 }:
                pk.HeldItem = 1777;
                break;
            case Species.Palkia when pk is { Form: 1, HeldItem: not 1778 }:
                pk.HeldItem = 1778;
                break;
            case Species.Ogerpon:
                if (pk.Form != 0)
                    pk.HeldItem = FormItem.GetItemOgerpon(pk.Form);
                break;
        }
    }

    public static MoveType GetValidOpergonTeraType(byte form) => (form & 3) switch
    {
        0 => MoveType.Grass,
        1 => MoveType.Water,
        2 => MoveType.Fire,
        3 => MoveType.Rock,
        _ => (MoveType)TeraTypeUtil.OverrideNone,
    };
    public static ushort GetBDSPUnownMetLocation(byte form)
    {
        return (form) switch
        {
            < 3 or > 5 and < 8 or > 8 and < 13 or > 13 and < 17 or > 17 and < 26 => 227,
            3 => 240,
            4 => 239,
            5 => 229,
            8 => 237,
            13 => 238,
            17 => 231,
            > 25 => 225,
        };
    }
}
