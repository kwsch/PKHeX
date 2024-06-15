using static PKHeX.Core.LegalityCheckStrings;
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
            return GetInvalid(LFormArgumentHigh);

        return (Species)pk.Species switch
        {
            // Transfer Edge Cases -- Bank wipes the form but keeps old FormArgument value.
            Furfrou when pk is { Context: EntityContext.Gen7, Form: 0 } &&
                                 ((enc.Generation == 6 && f.FormArgument <= byte.MaxValue) || IsFormArgumentDayCounterValid(f, 5, true))
                => GetValid(LFormArgumentValid),

            Furfrou when pk.Form != 0 => !IsFormArgumentDayCounterValid(f, 5, true) ? GetInvalid(LFormArgumentInvalid) : GetValid(LFormArgumentValid),
            Hoopa when pk.Form == 1 => data.Info.EvoChainsAllGens switch
            {
                { HasVisitedGen9: true } when arg == 0 => GetValid(LFormArgumentValid), // Value not applied on form change, and reset when reverted.
                { HasVisitedGen6: true } when IsFormArgumentDayCounterValid(f, 3) => GetValid(LFormArgumentValid), // 0-3 via OR/AS
                { HasVisitedGen7: true } when IsFormArgumentDayCounterValid(f, 3) && f.FormArgumentRemain != 0 => GetValid(LFormArgumentValid), // 1-3 via Gen7
                _ => GetInvalid(LFormArgumentInvalid),
            },
            Yamask when pk.Form == 1 => arg switch
            {
                not 0 when pk.IsEgg => GetInvalid(LFormArgumentNotAllowed),
                > 9_999 => GetInvalid(LFormArgumentHigh),
                _ => GetValid(LFormArgumentValid),
            },
            Basculin when pk.Form is 2 => arg switch
            {
                not 0 when pk.IsEgg => GetInvalid(LFormArgumentNotAllowed),
                > 9_999 => GetInvalid(LFormArgumentHigh),
                _ => GetValid(LFormArgumentValid),
            },
            Qwilfish when pk.Form is 1 => arg switch
            {
                not 0 when pk.IsEgg => GetInvalid(LFormArgumentNotAllowed),
                not 0 when pk.CurrentLevel < 25 => GetInvalid(LFormArgumentHigh), // Can't get requisite move
                > 9_999 => GetInvalid(LFormArgumentHigh),
                _ => GetValid(LFormArgumentValid),
            },
            Overqwil => arg switch
            {
                > 9_999 => GetInvalid(LFormArgumentHigh),
                0 when enc.Species == (ushort)Overqwil => GetValid(LFormArgumentValid),
                < 20 when !data.Info.EvoChainsAllGens.HasVisitedGen9 || pk.CurrentLevel < (pk is IHomeTrack { HasTracker: true } ? 15 : 28) => GetInvalid(LFormArgumentLow),
                >= 20 when !data.Info.EvoChainsAllGens.HasVisitedPLA || pk.CurrentLevel < 25 => GetInvalid(LFormArgumentLow),
                _ when pk is IHomeTrack { HasTracker: false } and PA8 { CurrentLevel: < 25 } => GetInvalid(LEvoInvalid),
                _ => GetValid(LFormArgumentValid),
            },
            Stantler => arg switch
            {
                not 0 when pk.IsEgg => GetInvalid(LFormArgumentNotAllowed),
                not 0 when pk.CurrentLevel < 31 => GetInvalid(LFormArgumentHigh),
                > 9_999 => GetInvalid(LFormArgumentHigh),
                _ => arg == 0 || HasVisitedPLA(data, Stantler) ? GetValid(LFormArgumentValid) : GetInvalid(LFormArgumentNotAllowed),
            },
            Primeape => arg switch
            {
                > 9_999 => GetInvalid(LFormArgumentHigh),
                _ => arg == 0 || HasVisitedSV(data, Primeape) ? GetValid(LFormArgumentValid) : GetInvalid(LFormArgumentNotAllowed),
            },
            Bisharp => arg switch
            {
                > 9_999 => GetInvalid(LFormArgumentHigh),
                _ => arg == 0 || HasVisitedSV(data, Bisharp) ? GetValid(LFormArgumentValid) : GetInvalid(LFormArgumentNotAllowed),
            },
            Gimmighoul => arg switch
            {
                // When leveled up, the game copies the save file's current coin count to the arg (clamped to <=999). If >=999, evolution is triggered.
                // Without being leveled up at least once, it cannot have a form arg value.
                >= 999 => GetInvalid(LFormArgumentHigh),
                0 => GetValid(LFormArgumentValid),
                _ => pk.CurrentLevel != pk.MetLevel ? GetValid(LFormArgumentValid) : GetInvalid(LFormArgumentNotAllowed),
            },
            Runerigus   => VerifyFormArgumentRange(enc.Species, Runerigus,   arg,  49, 9999),
            Alcremie    => VerifyFormArgumentRange(enc.Species, Alcremie,    arg,   0, (uint)AlcremieDecoration.Ribbon),
            Wyrdeer when enc.Species != (int)Wyrdeer && pk.CurrentLevel < 31 => GetInvalid(LEvoInvalid),
            Wyrdeer     => VerifyFormArgumentRange(enc.Species, Wyrdeer,     arg,  20, 9999),
            Basculegion => VerifyFormArgumentRange(enc.Species, Basculegion, arg, 294, 9999),
            Annihilape  => VerifyFormArgumentRange(enc.Species, Annihilape,  arg,  20, 9999),
            Kingambit   => VerifyFormArgumentRange(enc.Species, Kingambit,   arg,   3, 9999),
            Gholdengo   => VerifyFormArgumentRange(enc.Species, Gholdengo,   arg, 999,  999),
            Koraidon or Miraidon => enc switch
            {
                // Starter Legend has '1' when present in party, to differentiate.
                // Cannot be traded to other games.
                EncounterStatic9 { StarterBoxLegend: true } x when ParseSettings.ActiveTrainer is { } tr && (tr is not SAV9SV sv || sv.Version != x.Version) => GetInvalid(LTradeNotAvailable),
                EncounterStatic9 { StarterBoxLegend: true } => arg switch
                {
                  < EncounterStatic9.RideLegendFormArg => GetInvalid(LFormArgumentLow),
                    EncounterStatic9.RideLegendFormArg => !data.IsStoredSlot(StorageSlotType.Ride) ? GetInvalid(LFormParty) : GetValid(LFormArgumentValid),
                  > EncounterStatic9.RideLegendFormArg => GetInvalid(LFormArgumentHigh),
                },
                _ => arg switch
                {
                    not 0 => GetInvalid(LFormArgumentNotAllowed),
                    _ => GetValid(LFormArgumentValid),
                },
            },
            _ => VerifyFormArgumentNone(pk, f),
        };
    }

    private static bool HasVisitedAs(EvoCriteria[] evos, Species species) => EvolutionHistory.HasVisited(evos, (ushort)species);
    private static bool HasVisitedPLA(LegalityAnalysis data, Species species) => HasVisitedAs(data.Info.EvoChainsAllGens.Gen8a, species);
    private static bool HasVisitedSV(LegalityAnalysis data, Species species) => HasVisitedAs(data.Info.EvoChainsAllGens.Gen9, species);

    /// <summary>
    /// Check if the <see cref="value"/> is within the range of the inclusive <see cref="min"/> and inclusive <see cref="max"/>.
    /// </summary>
    /// <param name="encSpecies">Original species. If evolved, can have a non-zero value.</param>
    /// <param name="check">Current Species</param>
    /// <param name="value">Current Form Argument value</param>
    /// <param name="min">Minimum value allowed</param>
    /// <param name="max">Maximum value allowed</param>
    private CheckResult VerifyFormArgumentRange(ushort encSpecies, Species check, uint value, uint min, uint max)
    {
        // If was never the Form Argument accruing species (never evolved from it), then it must be zero.
        if (encSpecies == (ushort)check)
        {
            if (value == 0)
                return GetValid(LFormArgumentValid);
            return GetInvalid(LFormArgumentNotAllowed);
        }

        // Evolved, must be within the range.
        if (value < min)
            return GetInvalid(LFormArgumentLow);
        if (value > max)
            return GetInvalid(LFormArgumentHigh);
        return GetValid(LFormArgumentValid);
    }

    private CheckResult VerifyFormArgumentNone(PKM pk, IFormArgument f)
    {
        if (pk is not PK6 pk6)
        {
            if (f.FormArgument != 0)
            {
                if (pk is { Species: (int)Furfrou, Form: 0 } && (f.FormArgument & ~0xFF_00_00u) == 0)
                    return GetValid(LFormArgumentValid);
                return GetInvalid(LFormArgumentNotAllowed);
            }
            return GetValid(LFormArgumentValid);
        }

        if (f.FormArgument != 0)
        {
            if (pk is { Species: (int)Furfrou, Form: 0 } && (f.FormArgument & ~0xFFu) == 0)
                return GetValid(LFormArgumentValid);
            return GetInvalid(LFormArgumentNotAllowed);
        }

        // Stored separately from main form argument value
        if (pk6.FormArgumentRemain != 0)
            return GetInvalid(LFormArgumentNotAllowed);
        if (pk6.FormArgumentElapsed != 0)
            return GetInvalid(LFormArgumentNotAllowed);

        return GetValid(LFormArgumentValid);
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
