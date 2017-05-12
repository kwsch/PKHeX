using System.Linq;

namespace PKHeX.Core
{
    public enum GBEncounterType
    {
        TradeEncounterG1 = 1,
        StaticEncounter = 3,
        WildEncounter = 2,
        EggEncounter = 9,
        TradeEncounterG2 = 10,
        SpecialEncounter = 20,
    }

    public class GBEncounterData
    {
        public readonly int Level;
        public int MoveLevel;
        public readonly int Species;
        public bool Gen2 => Generation == 2;
        public bool Gen1 => Generation == 1;
        public readonly int Generation;
        public readonly bool WasEgg;
        public readonly GBEncounterType Type;
        public readonly object Encounter;

        // Egg encounter
        public GBEncounterData(int species)
        {
            Generation = 2;
            Type = GBEncounterType.EggEncounter;
            Level = 5;
            WasEgg = true;
            Species = species;
        }
        
        public GBEncounterData(PKM pkm, int gen, object enc)
        {
            Generation = gen;
            Encounter = enc;
            WasEgg = false;
            if (Encounter is EncounterTrade)
            {
                var trade = (EncounterTrade)Encounter;
                Species = trade.Species;
                if (pkm.HasOriginalMetLocation && trade.Level < pkm.Met_Level)
                    Level = pkm.Met_Level; // Crystal
                else
                    Level = trade.Level;
                if (Generation == 2)
                    Type = GBEncounterType.TradeEncounterG2;
                else
                    Type = GBEncounterType.TradeEncounterG1;
            }
            else if (Encounter is EncounterStatic)
            {
                var statc = (EncounterStatic)Encounter;
                Species = statc.Species;
                Level = statc.Level;
                if (statc.Moves != null && statc.Moves[0] != 0 && pkm.Moves.Contains(statc.Moves[0]))
                    Type = GBEncounterType.SpecialEncounter;
                else
                    Type = GBEncounterType.StaticEncounter;
            }
            else if (Encounter is EncounterSlot1)
            {
                var slot = (EncounterSlot1)Encounter;
                Species = slot.Species;
                if (pkm.HasOriginalMetLocation && slot.LevelMin >= pkm.Met_Level && pkm.Met_Level <= slot.LevelMax)
                    Level = pkm.Met_Level; // Crystal
                else
                    Level = slot.LevelMin;
                Type = GBEncounterType.WildEncounter;
            }
            MoveLevel = Level;
        }
    }

}
