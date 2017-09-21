namespace PKHeX.Core
{
    public class EncounterTrade : IEncounterable, IMoveset, IGeneration
    {
        public int Species { get; set; }
        public int[] Moves { get; set; }
        public int Level { get; set; }
        public int LevelMin => Level;
        public int LevelMax => 100;
        public int Generation { get; set; } = -1;

        public int Location { get; set; } = -1;
        public int Ability { get; set; }
        public Nature Nature = Nature.Random;
        public int TID { get; set; }
        public int SID { get; set; }
        public GameVersion Version { get; set; } = GameVersion.Any;
        public int[] IVs { get; set; } = { -1, -1, -1, -1, -1, -1 };
        public int[] Contest { get; set; } = { 0, 0, 0, 0, 0, 0 };
        public int Form { get; set; }
        public bool Shiny { get; set; } = false;
        public int Gender { get; set; } = -1;
        public int OTGender { get; set; } = -1;
        public bool EggEncounter => false;
        public int Egg_Location { get; set; }
        public bool EvolveOnTrade { get; set; }
        public int Ball { get; set; } = 4;
        public int CurrentLevel { get; set; } = -1;

        public string Name => "In-game Trade";
        public bool Fateful { get; set; }

        public static readonly int[] DefaultMetLocation = 
        {
            0, 126, 254, 2001, 30002, 30001, 30001,
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
