namespace PKHeX.Core
{
    public class EncounterTrade : IEncounterable, IMoveset, IGeneration
    {
        public int Species { get; set; }
        public int[] Moves { get; set; }
        public int Level;
        public int LevelMin => Level;
        public int LevelMax => 100;
        public int Generation { get; set; } = -1;

        public int Location = -1;
        public int Ability = 0;
        public Nature Nature = Nature.Random;
        public int TID;
        public int SID = 0;
        public GameVersion Version = GameVersion.Any;
        public int[] IVs = { -1, -1, -1, -1, -1, -1 };
        public int[] Contest = { 0, 0, 0, 0, 0, 0 };
        public int Form = 0;
        public bool Shiny = false;
        public int Gender = -1;
        public int OTGender = -1;
        public bool EggEncounter => false;
        public bool EvolveOnTrade = false;

        public string Name => "In-game Trade";

        public static readonly int[] DefaultMetLocation = 
        {
            254, 2001, 30002, 30001, 30001,
        };
    }

    public class EncounterTradePID : EncounterTrade
    {
        public uint PID;
    }

    public class EncounterTradeCatchRate : EncounterTrade
    {
        public uint Catch_Rate;
    }
}
