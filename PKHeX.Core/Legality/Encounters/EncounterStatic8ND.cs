using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Distributed Data)
    /// </summary>
    public sealed class EncounterStatic8ND : EncounterStatic, IGigantamax, IDynamaxLevel
    {
        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }
        public override int Location { get => SharedNest; set { } }

        public EncounterStatic8ND(byte lvl, byte dyna, byte flawless)
        {
            Level = lvl;
            DynamaxLevel = dyna;
            FlawlessIVCount = flawless;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc == SharedNest || EncounterArea8.IsWildArea8(loc);
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            if (Version != GameVersion.SWSH && pkm.Version != (int)Version && pkm.Met_Location != SharedNest)
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

            return false;
        }
    }
}