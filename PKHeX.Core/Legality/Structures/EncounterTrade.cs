namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data
    /// </summary>
    /// <remarks>
    /// Trade data is fixed level in all cases except for the first few generations of games.
    /// </remarks>
    public class EncounterTrade : IEncounterable, IMoveset, IGeneration, ILocation
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
        public int EggLocation { get; set; }
        public bool EvolveOnTrade { get; set; }
        public int Ball { get; set; } = 4;
        public int CurrentLevel { get; set; } = -1;

        private const string _name = "In-game Trade";
        public string Name => _name;
        public bool Fateful { get; set; }
        public bool IsNicknamed { get; set; } = true;

        public static readonly int[] DefaultMetLocation = 
        {
            0, 126, 254, 2001, 30002, 30001, 30001,
        };
    }

    /// <summary>
    /// Trade Encounter data with a fixed PID.
    /// </summary>
    public class EncounterTradePID : EncounterTrade
    {
        public uint PID;
    }

    /// <summary>
    /// Trade Encounter data with a fixed Catch Rate
    /// </summary>
    /// <remarks>
    /// Generation 1 specific value used in detecting unmodified/untraded Generation 1 Trade Encounter data.
    /// </remarks>
    public class EncounterTradeCatchRate : EncounterTrade
    {
        public uint Catch_Rate;
    }
}
