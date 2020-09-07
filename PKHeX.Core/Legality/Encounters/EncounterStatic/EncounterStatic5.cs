using System.Linq;

namespace PKHeX.Core
{
    public class EncounterStatic5 : EncounterStatic
    {
        public sealed override int Generation => 5;
        public bool Roaming { get; set; }

        public sealed override bool IsMatchDeferred(PKM pkm)
        {
            if (pkm.FatefulEncounter != Fateful)
                return true;
            if (Ability == 4 && pkm.AbilityNumber != 4) // BW/2 Jellicent collision with wild surf slot, resolved by duplicating the encounter with any abil
                return true;
            return false;
        }

        protected sealed override bool IsMatchLocation(PKM pk)
        {
            if (!Roaming)
                return base.IsMatchLocation(pk);
            return Roaming_MetLocation_BW.Contains(pk.Met_Location);
        }

        private static readonly int[] Roaming_MetLocation_BW =
        {
            25,26,27,28, // Route 12, 13, 14, 15 Night latter half
            15,16,31,    // Route 2, 3, 18 Morning
            17,18,29,    // Route 4, 5, 16 Daytime
            19,20,21,    // Route 6, 7, 8 Evening
            22,23,24,    // Route 9, 10, 11 Night former half
        };
    }

    /// <summary>
    /// Generation 5 Dream Radar gift encounters
    /// </summary>
    public sealed class EncounterStatic5DR : EncounterStatic5
    {
        public EncounterStatic5DR(int species, int form, int abilityIndex = 4)
        {
            Species = species;
            Form = form;
            Ability = abilityIndex;
            Location = 30015;
            Gift = true;
            Ball = 25;
            Level = 5; // to 40
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            // Level from 5->40 depends on the number of badges
            var met = pkm.Met_Level;
            if (met % 5 != 0)
                return false;
            return (uint) (met - 5) <= 35; // 5 <= x <= 40
        }
    }
}
