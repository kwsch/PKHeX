namespace PKHeX.Core
{
    public class EncounterLink : IEncounterable
    {
        public int Species { get; set; }
        public int Level { get; set; }
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Location { get; set; } = 30011;
        public int Ability { get; set; } = 1;
        public int Ball { get; set; } = 4; // Pokéball
        public bool Classic { get; set; } = true;
        public bool Fateful { get; set; } = false;
        public int[] RelearnMoves = new int[4];
        public bool? Shiny { get; set; } = false;
        public bool OT { get; set; } = true; // Receiver is OT?
        public bool EggEncounter => false;

        public bool XY { get; set; }
        public bool ORAS { get; set; }

        public string Name => "Pokémon Link Gift";
    }
}
