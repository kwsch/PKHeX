namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data
    /// </summary>
    /// <remarks>
    /// Trade data is fixed level in all cases except for the first few generations of games.
    /// </remarks>
    public class EncounterTrade : IEncounterable, IMoveset, IGeneration, ILocation, IContestStats
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
        public int[] IVs { get; set; }
        public int[] Contest { set => this.SetContestStats(value); }
        public int CNT_Cool { get; set; }
        public int CNT_Beauty { get; set; }
        public int CNT_Cute { get; set; }
        public int CNT_Smart { get; set; }
        public int CNT_Tough { get; set; }
        public int CNT_Sheen { get; set; }
        public int Form { get; set; }
        public virtual Shiny Shiny { get; set; } = Shiny.Never;
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

        public string[] Nicknames { get; internal set; }
        public string[] TrainerNames { get; internal set; }
        public string GetNickname(int language) => Nicknames?.Length < language ? Nicknames[language] : null;
        public string GetOT(int language) => TrainerNames?.Length < language ? TrainerNames[language] : null;

        public static readonly int[] DefaultMetLocation = 
        {
            0, 126, 254, 2001, 30002, 30001, 30001,
        };

        public PKM ConvertToPKM(ITrainerInfo SAV) => throw new System.NotImplementedException();
    }
}
