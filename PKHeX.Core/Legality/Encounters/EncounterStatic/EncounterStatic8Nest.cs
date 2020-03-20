using System;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    public abstract class EncounterStatic8Nest<T> : EncounterStatic, IGigantamax, IDynamaxLevel where T : EncounterStatic8Nest<T>
    {
        public static Func<PKM, T, bool>? VerifyCorrelation { private get; set; }
        public static Action<PKM, T, EncounterCriteria>? GenerateData { private get; set; }

        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }
        public override int Location { get => SharedNest; set { } }

        protected override bool IsMatchLevel(PKM pkm, int lvl)
        {
            if (lvl == Level)
                return true;

            // Check downleveled (20-55)
            if (lvl > Level)
                return false;
            if (lvl < 20 || lvl > 55)
                return false;
            return lvl % 5 == 0;
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            if (Version != GameVersion.SWSH && pkm.Version != (int)Version && pkm.Met_Location != SharedNest)
                return false;

            if (VerifyCorrelation != null && !VerifyCorrelation(pkm, (T)this))
                return false;

            return base.IsMatch(pkm, lvl);
        }

        public override bool IsMatchDeferred(PKM pkm)
        {
            if (base.IsMatchDeferred(pkm))
                return true;
            if (Ability != A4 && pkm.AbilityNumber == 4)
                return true;
            if (pkm is IGigantamax g && g.CanGigantamax != CanGigantamax)
                return true;
            if (Species == (int)Core.Species.Alcremie && pkm is IFormArgument a && a.FormArgument != 0)
                return true;
            if (Species == (int)Core.Species.Runerigus && pkm is IFormArgument r && r.FormArgument != 0)
                return true;

            return false;
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            if (GenerateData != null)
                GenerateData(pk, (T)this, criteria);
            else
                base.SetPINGA(pk, criteria);
        }
    }
}
