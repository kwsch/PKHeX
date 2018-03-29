namespace PKHeX.Core
{
    /// <summary>
    /// Static Encounter Data
    /// </summary>
    /// <remarks>
    /// Static Encounters are fixed position encounters with properties that are not subject to Wild Encounter conditions.
    /// </remarks>
    public class EncounterStatic : IEncounterable, IMoveset, IGeneration, ILocation, IContestStats
    {
        public int Species { get; set; }
        public int[] Moves { get; set; }
        public int Level { get; set; }

        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Generation { get; set; } = -1;
        public int Location { get; set; }
        public int Ability { get; set; }
        public int Form { get; set; }
        public virtual Shiny Shiny { get; set; } = Shiny.Random;
        public int[] Relearn { get; set; } = new int[4];
        public int Gender { get; set; } = -1;
        public int EggLocation { get; set; }
        public Nature Nature { get; set; } = Nature.Random;
        public bool Gift { get; set; }
        public int Ball { get; set; } = 4; // Only checked when is Gift
        public GameVersion Version = GameVersion.Any;
        public int[] IVs { get; set; }
        public int FlawlessIVCount { get; internal set; }
        public bool IV3 { set => FlawlessIVCount = value ? 3 : 0; }

        public int[] Contest { set => this.SetContestStats(value); }
        public int CNT_Cool { get; set; }
        public int CNT_Beauty { get; set; }
        public int CNT_Cute { get; set; }
        public int CNT_Smart { get; set; }
        public int CNT_Tough { get; set; }
        public int CNT_Sheen { get; set; }

        public int HeldItem { get; set; }
        public int EggCycles { get; set; }

        public bool Fateful { get; set; }
        public bool RibbonWishing { get; set; }
        public bool SkipFormCheck { get; set; }
        public bool Roaming { get; set; }
        public bool EggEncounter => EggLocation > 0;

        private void CloneArrays()
        {
            // dereference original arrays with new copies
            Moves = (int[])Moves?.Clone();
            Relearn = (int[])Relearn.Clone();
            IVs = (int[])IVs?.Clone();
        }
        internal virtual EncounterStatic Clone()
        {
            var result = (EncounterStatic)MemberwiseClone();
            result.CloneArrays();
            return result;
        }

        private const string _name = "Static Encounter";
        public string Name => Version == GameVersion.Any ? _name : $"{_name} ({Version})";

        public PKM ConvertToPKM(ITrainerInfo SAV) => throw new System.NotImplementedException();
    }
}
