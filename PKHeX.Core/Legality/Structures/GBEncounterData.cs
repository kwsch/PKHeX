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
        public readonly GameVersion Game;
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
            Type = GBEncounterType.EggEncounter;
        }
        
        public GBEncounterData(PKM pkm, int gen, IEncounterable enc, GameVersion game)
        {
            Game = game;
            Generation = gen;
            Encounter = enc;
            switch (Encounter)
            {
                case EncounterTrade t:
                    if (pkm.HasOriginalMetLocation && t.Level < pkm.Met_Level)
                        Level = pkm.Met_Level; // Crystal
                    else
                        Level = t.Level;
                        Type = Generation == 2
                            ? GBEncounterType.TradeEncounterG2
                            : GBEncounterType.TradeEncounterG1;
                    break;
                case EncounterStatic s:
                    Level = s.Level;
                    Type = s.Moves != null && s.Moves[0] != 0 && pkm.Moves.Contains(s.Moves[0])
                        ? GBEncounterType.SpecialEncounter
                        : GBEncounterType.StaticEncounter;
                    break;
                case EncounterSlot w:
                    Level = pkm.HasOriginalMetLocation && w.LevelMin >= pkm.Met_Level && pkm.Met_Level <= w.LevelMax
                        ? pkm.Met_Level // Crystal
                        : w.LevelMin;
                    Type = GBEncounterType.WildEncounter;
                    break;
            }
        }
    }
}
