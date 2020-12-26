using System;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Raid)
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public abstract record EncounterStatic8Nest<T> : EncounterStatic, IGigantamax, IDynamaxLevel where T : EncounterStatic8Nest<T>
    {
        public sealed override int Generation => 8;
        public static Func<PKM, T, bool>? VerifyCorrelation { private get; set; }
        public static Action<PKM, T, EncounterCriteria>? GenerateData { private get; set; }

        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }
        public override int Location { get => SharedNest; init { } }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            // Required Ability
            if (Ability == 4 && pkm.AbilityNumber != 4)
                return false; // H

            if (Version != GameVersion.SWSH && pkm.Version != (int)Version && pkm.Met_Location != SharedNest)
                return false;

            if (VerifyCorrelation != null && !VerifyCorrelation(pkm, (T)this))
                return false;

            return base.IsMatch(pkm, evo);
        }

        public sealed override bool IsMatchDeferred(PKM pkm)
        {
            if (base.IsMatchDeferred(pkm))
                return true;
            if (pkm is IGigantamax g && g.CanGigantamax != CanGigantamax && !g.CanToggleGigantamax(pkm.Species, pkm.Form, Species, Form))
                return true;
            if (Species == (int)Core.Species.Alcremie && pkm is IFormArgument a && a.FormArgument != 0)
                return true;
            if (Species == (int)Core.Species.Runerigus && pkm is IFormArgument r && r.FormArgument != 0)
                return true;

            if (Ability != -1) // Any
            {
                // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
                if (Ability == 0 && pkm.AbilityNumber == 4)
                    return true; // 0/1
                if (Ability == 1 && pkm.AbilityNumber != 1)
                    return true; // 0
                if (Ability == 2 && pkm.AbilityNumber != 2)
                    return true; // 1
            }

            if (Shiny != Shiny.Random)
            {
                if (Shiny == Shiny.Never && pkm.IsShiny)
                    return true;
                if (Shiny == Shiny.Always && !pkm.IsShiny)
                    return true;
            }

            return false;
        }

        protected sealed override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            if (GenerateData != null)
                GenerateData(pk, (T)this, criteria);
            else
                base.SetPINGA(pk, criteria);
        }
    }
}
