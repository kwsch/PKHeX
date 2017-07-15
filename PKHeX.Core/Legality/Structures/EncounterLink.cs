namespace PKHeX.Core
{
    public class EncounterLink : IEncounterable, IRibbonSetEvent4
    {
        public int Species { get; set; }
        public int Level { get; set; }
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Location { get; set; } = 30011;
        public int Ability { get; set; } = 1;
        public int Ball { get; set; } = 4; // Pokéball
        public bool Fateful { get; set; } = false;
        public int[] RelearnMoves = new int[4];
        public bool? Shiny { get; set; } = false;
        public bool OT { get; set; } = true; // Receiver is OT?
        public bool EggEncounter => false;

        public bool XY { get; set; }
        public bool ORAS { get; set; }

        public string Name => "Pokémon Link Gift";

        public bool RibbonClassic { get; set; } = true;

        // Unused
        public bool RibbonWishing { get; set; }
        public bool RibbonPremier { get; set; }
        public bool RibbonEvent { get; set; }
        public bool RibbonBirthday { get; set; }
        public bool RibbonSpecial { get; set; }
        public bool RibbonWorld { get; set; }
        public bool RibbonChampionWorld { get; set; }
        public bool RibbonSouvenir { get; set; }
    }
}
