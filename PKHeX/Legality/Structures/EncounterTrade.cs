namespace PKHeX.Core
{
    public class EncounterTrade : IEncounterable
    {
        public int Species { get; set; }
        public int Level;

        public int Location = 30001;
        public int Ability = 0;
        public Nature Nature = Nature.Random;
        public int TID;
        public int SID = 0;
        public int[] IVs = { -1, -1, -1, -1, -1, -1 };
        public int[] Moves;
        public int Form = 0;
        public bool Shiny = false;
        public int Gender = -1;
        public int OTGender = -1;

        public string Name => "In-game Trade";
    }
}
