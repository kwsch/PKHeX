using System.Linq;

namespace PKHeX.Core
{
    public enum GBEncounterType
    {
        EggEncounter,
        WildEncounter,
        StaticEncounter,
        SpecialEncounter,
        TradeEncounterG1,
        TradeEncounterG2,
    }

    public class GBEncounterData : IEncounterable
    {
        public readonly int Level;
        public int MoveLevel;
        public GameVersion Game;
        public readonly int Generation;
        public readonly GBEncounterType Type;
        public readonly IEncounterable Encounter;

        public int Species => Encounter.Species;
        public string Name => Encounter.Name;
        public bool EggEncounter => Encounter.EggEncounter;
        public int LevelMin => Encounter.LevelMin;
        public int LevelMax => Encounter.LevelMax;

        // Egg encounter
        public GBEncounterData(int species, GameVersion game)
        {
            Generation = 2;
            Game = game;
            Encounter = new EncounterEgg { Species = species, Game = game, Level = Level };
        }
        
        public GBEncounterData(PKM pkm, int gen, IEncounterable enc, GameVersion game)
        {
            Game = game;
            Generation = gen;
            Encounter = enc;
            if (Encounter is EncounterTrade trade)
            {
                if (pkm.HasOriginalMetLocation && trade.Level < pkm.Met_Level)
                    Level = pkm.Met_Level; // Crystal
                else
                    Level = trade.Level;
                Type = Generation == 2
                    ? GBEncounterType.TradeEncounterG2
                    : GBEncounterType.TradeEncounterG1;
            }
            else if (Encounter is EncounterStatic statc)
            {
                Level = statc.Level;
                Type = statc.Moves != null && statc.Moves[0] != 0 && pkm.Moves.Contains(statc.Moves[0])
                    ? GBEncounterType.SpecialEncounter
                    : GBEncounterType.StaticEncounter;
            }
            else if (Encounter is EncounterSlot1 slot)
            {
                Level = pkm.HasOriginalMetLocation && slot.LevelMin >= pkm.Met_Level && pkm.Met_Level <= slot.LevelMax
                    ? pkm.Met_Level // Crystal
                    : slot.LevelMin;
                Type = GBEncounterType.WildEncounter;
            }
            MoveLevel = Level;
        }
    }
}
