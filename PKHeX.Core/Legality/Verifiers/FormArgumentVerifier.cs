using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

public sealed class FormArgumentVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Form;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk is not IFormArgument f)
            return;

        var result = VerifyFormArgument(data, f);
        data.AddLine(result);
    }

    private CheckResult VerifyFormArgument(LegalityAnalysis data, IFormArgument f)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;
        var arg = f.FormArgument;

        var unusedMask = pk.Format == 6 ? 0xFFFF_FF00 : 0xFF00_0000;
        if ((arg & unusedMask) != 0)
            return GetInvalid(FormArgumentLEQ_0);

        return (Species)pk.Species switch
        {
            Furfrou => CheckFurfrou(pk, enc, f),
            Hoopa when pk.Form == 1 => CheckHoopa(data, f, arg),
            Yamask when pk.Form == 1 => arg switch
            {
                not 0 when pk.IsEgg => GetInvalid(FormArgumentNotAllowed),
                > 9_999 => GetInvalid(FormArgumentLEQ_0, 9999),
                _ => GetValid(FormArgumentValid),
            },
            Basculin when pk.Form is 2 => arg switch
            {
                not 0 when pk.IsEgg => GetInvalid(FormArgumentNotAllowed),
                > 9_999 => GetInvalid(FormArgumentLEQ_0, 9999),
                _ => GetValid(FormArgumentValid),
            },
            Farfetchd when pk.Form is 1 => CheckFarfetchd(data, pk, arg), // Galar
            Sirfetchd => CheckSirfetchd(data, arg, enc),
            Qwilfish when pk.Form is 1 => CheckQwilfish(data, pk, arg),
            Overqwil => CheckOverqwil(data, pk, arg, enc),
            Stantler => arg switch
            {
                not 0 when pk.IsEgg => GetInvalid(FormArgumentNotAllowed),
                not 0 when pk.CurrentLevel < 31 => GetInvalid(FormArgumentLEQ_0, 0),
                > 9_999 => GetInvalid(FormArgumentLEQ_0, 9999),
                _ => arg == 0 || HasVisitedPLA(data, Stantler) ? GetValid(FormArgumentValid) : GetInvalid(FormArgumentNotAllowed),
            },
            Primeape => CheckPrimeape(data, pk, arg, enc),
            Bisharp => arg switch
            {
                > 9_999 => GetInvalid(FormArgumentLEQ_0, 9999),
                _ => arg == 0 || HasVisitedSV(data, Bisharp) ? GetValid(FormArgumentValid) : GetInvalid(FormArgumentNotAllowed),
            },
            Gimmighoul => CheckGimmighoul(data.Info.EvoChainsAllGens, arg, pk),
            Gholdengo => CheckGholdengo(data.Info.EvoChainsAllGens, arg, enc, pk),

            Runerigus   => VerifyFormArgumentRange(enc.Species, Runerigus,   arg,  49, 9999),
            Alcremie    => VerifyFormArgumentRange(enc.Species, Alcremie,    arg,   0, (ushort)AlcremieDecoration.Ribbon),
            Wyrdeer when enc.Species != (int)Wyrdeer && pk.CurrentLevel < 31 => GetInvalid(EvoInvalid),
            Wyrdeer     => VerifyFormArgumentRange(enc.Species, Wyrdeer,     arg,  20, 9999),
            Basculegion => VerifyFormArgumentRange(enc.Species, Basculegion, arg, 294, 9999),
            Annihilape  => VerifyFormArgumentRange(enc.Species, Annihilape,  arg,  20, 9999),
            Kingambit   => VerifyFormArgumentRange(enc.Species, Kingambit,   arg,   3, 9999),
            Koraidon or Miraidon => enc switch
            {
                // Starter Legend has '1' when present in party, to differentiate.
                // Cannot be traded to other games.
                EncounterStatic9 { StarterBoxLegend: true } x when ParseSettings.ActiveTrainer is { } tr && (tr is not SAV9SV sv || sv.Version != x.Version) => GetInvalid(TradeNotAvailable),
                EncounterStatic9 { StarterBoxLegend: true } => arg switch
                {
                  < EncounterStatic9.RideLegendFormArg => GetInvalid(FormArgumentGEQ_0, EncounterStatic9.RideLegendFormArg),
                    EncounterStatic9.RideLegendFormArg => !data.IsStoredSlot(StorageSlotType.Ride) ? GetInvalid(FormParty) : GetValid(FormArgumentValid),
                  > EncounterStatic9.RideLegendFormArg => GetInvalid(FormArgumentLEQ_0, EncounterStatic9.RideLegendFormArg),
                },
                _ => arg switch
                {
                    not 0 => GetInvalid(FormArgumentNotAllowed),
                    _ => GetValid(FormArgumentValid),
                },
            },
            _ => VerifyFormArgumentNone(pk, f),
        };
    }

    private CheckResult CheckHoopa(LegalityAnalysis data, IFormArgument f, uint arg)
    {
        var history = data.Info.EvoChainsAllGens;
        if (arg == 0)
        {
            if (history.HasVisitedZA) // Value not applied on form change, and reset when reverted.
                return GetValid(FormArgumentValid);
            if (history.HasVisitedGen9) // Value not applied on form change, and reset when reverted.
                return GetValid(FormArgumentValid);
        }
        else
        {
            if (history.HasVisitedGen7 && IsFormArgumentDayCounterValid(f, 3)) // 1-3 via Gen7
                return GetValid(FormArgumentValid);
        }

        var pk = data.Entity;
        if (pk is PK6 pk6)
        {
            // 0-3 via OR/AS
            if (pk6.FormArgument != 0) // 0x3C not used (elapsed streak)
                return GetInvalid(FormArgumentNotAllowed);
            var elapsed = pk6.FormArgumentElapsed;
            var remain = pk6.FormArgumentRemain;
            var sum = elapsed + remain;
            if (sum != 3)
                return GetInvalid(FormArgumentInvalid);
            return GetValid(FormArgumentValid);
        }

        return GetInvalid(FormArgumentInvalid);
    }

    private CheckResult CheckFurfrou(PKM pk, IEncounterTemplate enc, IFormArgument f)
    {
        // Transfer Edge Cases -- Bank wipes the form but keeps old FormArgument value.
        // Gen6: Reverts when deposited.
        // Gen7: Reverts form & arg when withdrawn, reverts form (NOT arg) when deposited in Bank.
        // Gen9a: Doesn't decrease, always 5.
        if (pk is PK6 pk6)
            return CheckFurfrou6(pk6);
        if (pk is PK7 pk7)
            return CheckFurfrou7(pk7, enc);
        if (pk is { GO_HOME: true } && f.FormArgument == 0)
            return GetValid(FormArgumentValid); // GO transfers forget to set Form Argument.
        if (pk is PA9 pa9)
            return CheckFurfrou9a(pa9, enc);

        // Only legal pathways are via methods above.
        return GetInvalid(FormArgumentInvalid);
    }

    private CheckResult CheckFurfrou9a(PA9 pk, IEncounterTemplate enc)
    {
        // Gen6=>Gen7 transfer edge case: Form argument is not cleared when depositing in Bank, but form is.
        if (enc.Generation == 6 && pk is { Form: 0, FormArgument: <= byte.MaxValue })
            return GetValid(FormArgumentValid);

        // Z-A trims set to 5.
        if ((pk.FormArgument == 5) == (pk.Form != 0))
            return GetValid(FormArgumentValid);

        // Bank=>HOME with form 0 forgets to wipe, but Form is reverted.
        if (pk.Form == 0 && enc.Generation <= 7 && IsFormArgumentDayCounterValid(pk, 5, true))
            return GetValid(FormArgumentValid);

        return GetInvalid(FormArgumentInvalid);
    }

    private CheckResult CheckFurfrou7(PK7 pk, IEncounterTemplate enc)
    {
        // Gen6=>Gen7 transfer edge case: Form argument is not cleared when depositing in Bank, but form is.
        if (enc.Generation == 6 && pk is { Form: 0, FormArgument: <= byte.MaxValue })
            return GetValid(FormArgumentValid);

        if (pk is { Form: 0, FormArgument: 0 })
            return GetValid(FormArgumentValid);

        // Depositing into box no longer clears form; they only wipe it when you withdraw.
        // Storing in Bank will revert the form, so any form is valid as long as the day counter values are valid for any trim.
        if (!IsFormArgumentDayCounterValid(pk, 5, true))
            return GetInvalid(FormArgumentInvalid);

        return GetValid(FormArgumentValid);
    }

    private CheckResult CheckFurfrou6(PK6 pk)
    {
        // Can only exist in party.
        // 0x3C: Current streak
        // 0xED: Remaining days
        // 0xEE: Elapsed days (same as current streak)
        var arg = pk.FormArgument;
        // Argument can be anything; depositing drops the form and party stats and forgets to clear the arg.
        if (arg > byte.MaxValue)
            return GetInvalid(FormArgumentLEQ_0, byte.MaxValue);

        // Form can only exist inside party. Checked elsewhere.
        var remain = pk.FormArgumentRemain;
        var elapsed = pk.FormArgumentElapsed;
        if (pk.Form != 0)
        {
            var sum = remain + elapsed;
            if (sum < 5)
                return GetInvalid(FormArgumentInvalid);
            if (elapsed != arg)
                return GetInvalid(FormArgumentInvalid);
        }
        else
        {
            // Party stat values must be zero.
            if (remain != 0 || elapsed != 0)
                return GetInvalid(FormArgumentNotAllowed);
        }

        return GetValid(FormArgumentValid);
    }

    private CheckResult CheckGimmighoul(EvolutionHistory history, uint arg, PKM pk)
    {
        if (arg == 0)
            return GetValid(FormArgumentValid);

        // The only game we can assign a form argument value is in S/V.
        // Z-A evolutions do not set form argument to Gimmighoul.
        if (history.HasVisitedGen9)
        {
            // When leveled up, the game copies the save file's current coin count to the arg (clamped to <=999). If >=999, evolution is triggered (can cancel).
            // Without being leveled up at least once, it cannot have a form arg value.
            if (arg > 999)
                return GetInvalid(FormArgumentLEQ_0, 999);
            if (pk.CurrentLevel == pk.MetLevel)
                return GetInvalid(FormArgumentNotAllowed);

            return GetValid(FormArgumentValid);
        }

        return GetInvalid(FormArgumentNotAllowed);
    }

    private CheckResult CheckGholdengo(EvolutionHistory history, uint arg, IEncounterable enc, PKM pk)
    {
        if (enc.Species == (ushort)Gholdengo)
        {
            if (arg == 0)
                return GetValid(FormArgumentValid);
            return GetInvalid(FormArgumentNotAllowed);
        }

        // Gimmighoul evolved.
        // The only game we can assign a form argument value is in S/V.
        // Z-A evolutions do not set form argument to Gimmighoul.
        var hasVisitedNoArgGame = history.HasVisitedZA;
        if (hasVisitedNoArgGame && arg == 0)
            return GetValid(FormArgumentValid);

        if (history.HasVisitedGen9)
        {
            // When leveled up, the game copies the save file's current coin count to the arg (clamped to <=999). If >=999, evolution is triggered (can cancel).
            // Without being leveled up at least once, it cannot have a form arg value.
            if (arg > 999)
                return GetInvalid(FormArgumentLEQ_0, 999);
            if (pk.CurrentLevel == pk.MetLevel)
                return GetInvalid(FormArgumentNotAllowed);

            if (!hasVisitedNoArgGame && arg != 999) // Evolving without visiting a less-restricted game requires 999.
                return GetInvalid(FormArgumentGEQ_0, 999);
            return GetValid(FormArgumentValid);
        }

        return GetInvalid(FormArgumentNotAllowed);
    }

    private static bool IsFormArgumentValidFurfrou8HOME(IFormArgument f, IEncounterTemplate enc)
    {
        if (f.FormArgument == 0 && enc is { Version: GameVersion.GO })
            return true; // Does not come with a Form Argument.
        return IsFormArgumentDayCounterValid(f, 5, enc.Generation < 8);
    }

    private CheckResult CheckPrimeape(LegalityAnalysis data, PKM pk, uint arg, IEncounterTemplate enc)
    {
        if (arg == 0)
            return GetValid(FormArgumentValid);
        if (arg > 9_999)
            return GetInvalid(FormArgumentLEQ_0, 9999);

        if (HasVisitedSV(data, Primeape) || HasVisitedZA(data, Primeape))
        {
            const ushort move = (ushort)Move.RageFist;
            // Eager check
            if (pk.HasMove(move) || pk.HasRelearnMove(move))
                return GetValid(FormArgumentValid);

            var head = LearnGroupUtil.GetCurrentGroup(pk);
            if (MemoryPermissions.GetCanKnowMove(enc, move, data.Info.EvoChainsAllGens, pk, head))
                return GetValid(FormArgumentValid);
        }
        return GetInvalid(FormArgumentLEQ_0, 0); // Can't increase from 0.
    }

    private CheckResult CheckFarfetchd(LegalityAnalysis data, PKM pk, uint arg)
    {
        if (arg == 0)
            return GetValid(FormArgumentValid);
        if (arg > 9_999)
            return GetInvalid(FormArgumentLEQ_0, 9999);
        if (pk.IsEgg)
            return GetInvalid(FormArgumentNotAllowed);

        var history = data.Info.EvoChainsAllGens;
        if (history.HasVisitedZA) // Can increase.
            return GetValid(FormArgumentValid);
        return GetInvalid(FormArgumentLEQ_0, 0); // Can't increase from 0.
    }

    private CheckResult CheckSirfetchd(LegalityAnalysis data, uint arg, IEncounterTemplate enc)
    {
        var history = data.Info.EvoChainsAllGens;
        if (arg is 0)
        {
            if (enc.Species is (ushort)Sirfetchd)
                return GetValid(FormArgumentValid);
            if (history.HasVisitedGen9 || history.HasVisitedSWSH)
                return GetValid(FormArgumentValid);
        }
        else if (arg > 9999)
        {
            return GetInvalid(FormArgumentLEQ_0, 9999);
        }

        if (history.HasVisitedZA && arg >= 3) // Can increase.
            return GetValid(FormArgumentValid);
        return GetInvalid(FormArgumentLEQ_0, 0); // Can't increase from 0.
    }

    private CheckResult CheckQwilfish(LegalityAnalysis data, PKM pk, uint arg)
    {
        if (arg == 0)
            return GetValid(FormArgumentValid);
        if (arg > 9_999)
            return GetInvalid(FormArgumentLEQ_0, 9999);
        if (pk.IsEgg)
            return GetInvalid(FormArgumentNotAllowed);

        const int lowestLearnBarbBarrageHOME = 15; // level 15 via PLA learnset
        var current = pk.CurrentLevel;
        var history = data.Info.EvoChainsAllGens;
        if (history.HasVisitedPLA)
        {
            const int min = 25; // mastered
            if (current < min)
                return GetInvalid(FormArgumentLEQ_0, 0); // Can't get requisite move
            return GetValid(FormArgumentValid);
        }
        if (history.HasVisitedZA)
        {
            var min = pk is IHomeTrack { HasTracker: false } ? 28 : lowestLearnBarbBarrageHOME;
            if (current < min)
                return GetInvalid(FormArgumentLEQ_0, 0); // Can't get requisite move
            return GetValid(FormArgumentValid);
        }

        return GetInvalid(FormArgumentLEQ_0, 0); // Can't get requisite move
    }

    private CheckResult CheckOverqwil(LegalityAnalysis data, PKM pk, uint arg, IEncounterTemplate enc)
    {
        if (arg is 0)
        {
            if (enc.Species is (ushort)Overqwil)
                return GetValid(FormArgumentValid);
        }
        else if (arg > 9999)
        {
            return GetInvalid(FormArgumentLEQ_0, 9999);
        }

        const int lowestLearnBarbBarrageHOME = 15; // level 15 via PLA learnset
        var history = data.Info.EvoChainsAllGens;
        var current = pk.CurrentLevel;
        if (history.HasVisitedGen9)
        {
            // Evolution requires only knowing the move.
            const ushort move = (ushort)Move.BarbBarrage;
            var hasMove = pk.HasMove(move) || pk.HasRelearnMove(move);
            if (hasMove || current >= (history.HasVisitedPLA ? lowestLearnBarbBarrageHOME : 28))
            {
                if (arg == 0 || history.HasVisitedPLA || history.HasVisitedZA)
                    return GetValid(FormArgumentValid);
            }
        }
        if (history.HasVisitedPLA && arg >= 20)
        {
            // Evolution requires mastering the move and using it.
            if (current >= 25)
                return GetValid(FormArgumentValid);
        }
        if (history.HasVisitedZA && arg >= 20)
        {
            if (current >= 28)
                return GetValid(FormArgumentValid);
        }
        return GetInvalid(EvoInvalid);
    }

    private static bool HasVisitedAs(ReadOnlySpan<EvoCriteria> evos, Species species) => EvolutionHistory.HasVisited(evos, (ushort)species);
    private static bool HasVisitedPLA(LegalityAnalysis data, Species species) => HasVisitedAs(data.Info.EvoChainsAllGens.Gen8a, species);
    private static bool HasVisitedSV(LegalityAnalysis data, Species species) => HasVisitedAs(data.Info.EvoChainsAllGens.Gen9, species);
    private static bool HasVisitedZA(LegalityAnalysis data, Species species) => HasVisitedAs(data.Info.EvoChainsAllGens.Gen9a, species);

    /// <summary>
    /// Check if the <see cref="value"/> is within the range of the inclusive <see cref="min"/> and inclusive <see cref="max"/>.
    /// </summary>
    /// <param name="encSpecies">Original species. If evolved, can have a non-zero value.</param>
    /// <param name="check">Current Species</param>
    /// <param name="value">Current Form Argument value</param>
    /// <param name="min">Minimum value allowed</param>
    /// <param name="max">Maximum value allowed</param>
    private CheckResult VerifyFormArgumentRange(ushort encSpecies, Species check, uint value, [ConstantExpected] ushort min, [ConstantExpected] ushort max)
    {
        // If was never the Form Argument accruing species (never evolved from it), then it must be zero.
        if (encSpecies == (ushort)check)
        {
            if (value == 0)
                return GetValid(FormArgumentValid);
            return GetInvalid(FormArgumentNotAllowed);
        }

        // Evolved, must be within the range.
        if (value < min)
            return GetInvalid(FormArgumentGEQ_0, min);
        if (value > max)
            return GetInvalid(FormArgumentLEQ_0, max);
        return GetValid(FormArgumentValid);
    }

    private CheckResult VerifyFormArgumentNone(PKM pk, IFormArgument f)
    {
        if (pk is not PK6 pk6)
        {
            if (f.FormArgument != 0)
            {
                if (pk is { Species: (int)Furfrou, Form: 0 } && (f.FormArgument & ~0xFF_00_00u) == 0)
                    return GetValid(FormArgumentValid);
                return GetInvalid(FormArgumentNotAllowed);
            }
            return GetValid(FormArgumentValid);
        }

        if (f.FormArgument != 0)
        {
            if (pk is { Species: (int)Furfrou, Form: 0 } && (f.FormArgument & ~0xFFu) == 0)
                return GetValid(FormArgumentValid);
            return GetInvalid(FormArgumentNotAllowed);
        }

        // Stored separately from main form argument value
        if (pk6.FormArgumentRemain != 0)
            return GetInvalid(FormArgumentNotAllowed);
        if (pk6.FormArgumentElapsed != 0)
            return GetInvalid(FormArgumentNotAllowed);

        return GetValid(FormArgumentValid);
    }

    private static bool IsFormArgumentDayCounterValid(IFormArgument f, uint maxSeed, bool canRefresh = false)
    {
        var remain = f.FormArgumentRemain;
        var elapsed = f.FormArgumentElapsed;
        var maxElapsed = f.FormArgumentMaximum;
        if (canRefresh)
        {
            if (maxElapsed < elapsed)
                return false;

            if (remain + elapsed < maxSeed)
                return false;
        }
        else
        {
            if (maxElapsed != 0)
                return false;

            if (remain + elapsed != maxSeed)
                return false;
        }
        if (remain > maxSeed)
            return false;
        return remain != 0;
    }
}
