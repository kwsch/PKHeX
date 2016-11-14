namespace PKHeX
{
    public class EncounterLink
    {
        public int Species;
        public int Level;
        public int Location = 30011;
        public int Ability = 1;
        public int Ball = 4; // Pokéball
        public Nature Nature = Nature.Random;
        public int[] IVs = { -1, -1, -1, -1, -1, -1 };
        public int FlawlessIVs = 0;
        public bool Classic = true;
        public bool Fateful = false;
        public int[] RelearnMoves = new int[4];
        public bool? Shiny = false;
        public bool OT = true; // Receiver is OT?

        public bool XY = false;
        public bool ORAS = false;
        public bool SM = false;
    }
}
