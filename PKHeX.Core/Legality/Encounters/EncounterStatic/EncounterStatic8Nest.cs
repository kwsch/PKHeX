using System;
using static PKHeX.Core.Encounters8Nest;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Raid)
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public abstract record EncounterStatic8Nest<T>(GameVersion Version) : EncounterStatic(Version), IGigantamax, IDynamaxLevel where T : EncounterStatic8Nest<T>
{
    public sealed override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8;
    public static Func<PKM, T, bool>? VerifyCorrelation { private get; set; }
    public static Action<PKM, T, EncounterCriteria>? GenerateData { private get; set; }

    public bool CanGigantamax { get; set; }
    public byte DynamaxLevel { get; set; }
    public override int Location { get => SharedNest; init { } }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk is PK8 d && d.DynamaxLevel < DynamaxLevel)
            return false;

        // Required Ability
        if (Ability == OnlyHidden && pk.AbilityNumber != 4)
            return false; // H

        if (Version != GameVersion.SWSH && pk.Version != (int)Version && pk.Met_Location != SharedNest)
            return false;

        if (VerifyCorrelation != null && !VerifyCorrelation(pk, (T)this))
            return false;

        if (pk is IRibbonSetMark8 m8 && m8.HasMark())
            return false;
        if (pk.Species == (int)Core.Species.Shedinja && pk is IRibbonSetAffixed { AffixedRibbon: >= (int)RibbonIndex.MarkLunchtime })
            return false;

        return base.IsMatchExact(pk, evo);
    }

    protected sealed override EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Ability != Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not OnlyHidden && !AbilityVerifier.CanAbilityPatch(8, PersonalTable.SWSH.GetFormEntry(Species, Form), pk.Species))
                    return EncounterMatchRating.DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                if (Ability is OnlyFirst or OnlySecond && !AbilityVerifier.CanAbilityCapsule(8, PersonalTable.SWSH.GetFormEntry(Species, Form)))
                    return EncounterMatchRating.DeferredErrors;
            }
        }

        return base.IsMatchDeferred(pk);
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        if (pk is PK8 and IGigantamax g && g.CanGigantamax != CanGigantamax && !g.CanToggleGigantamax(pk.Species, pk.Form, Species, Form))
            return true;
        if (Species == (int)Core.Species.Alcremie && pk is IFormArgument { FormArgument: not 0 })
            return true;
        if (Species == (int)Core.Species.Runerigus && pk is IFormArgument { FormArgument: not 0 })
            return true;

        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        return base.IsMatchPartial(pk);
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        if (GenerateData == null)
            pk.SetRandomEC();
    }

    protected sealed override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        if (GenerateData != null)
        {
            GenerateData(pk, (T)this, criteria);
            return;
        }

        base.SetPINGA(pk, criteria);
        if (Species == (int) Core.Species.Toxtricity)
        {
            while (true)
            {
                var result = EvolutionMethod.GetAmpLowKeyResult(pk.Nature);
                if (result == pk.Form)
                    break;
                pk.Nature = Util.Rand.Next(25);
            }

            // Might be originally generated with a Neutral nature, then above logic changes to another.
            // Realign the stat nature to Serious mint.
            if (pk.Nature != pk.StatNature && ((Nature)pk.StatNature).IsNeutral())
                pk.StatNature = (int)Nature.Serious;
        }
        var pid = pk.PID;
        RaidRNG.ForceShinyState(pk, Shiny == Shiny.Always, ref pid);
        pk.PID = pid;
    }
}
