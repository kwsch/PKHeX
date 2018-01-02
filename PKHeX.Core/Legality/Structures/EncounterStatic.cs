using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Static Encounter Data
    /// </summary>
    /// <remarks>
    /// Static Encounters are fixed position encounters with properties that are not subject to Wild Encounter conditions.
    /// </remarks>
    public class EncounterStatic : IEncounterable, IMoveset, IGeneration, ILocation
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
        public bool? Shiny { get; set; } // false = never, true = always, null = possible
        public int[] Relearn { get; set; } = new int[4];
        public int Gender { get; set; } = -1;
        public int EggLocation { get; set; }
        public Nature Nature { get; set; } = Nature.Random;
        public bool Gift { get; set; }
        public int Ball { get; set; } = 4; // Only checked when is Gift
        public GameVersion Version = GameVersion.Any;
        public int[] IVs { get; set; } = { -1, -1, -1, -1, -1, -1 };
        public bool IV3 { get; set; }
        public int[] Contest { get; set; } = { 0, 0, 0, 0, 0, 0 };
        public int HeldItem { get; set; }
        public int EggCycles { get; set; }

        public bool Fateful { get; set; }
        public bool RibbonWishing { get; set; }
        public bool SkipFormCheck { get; set; }
        public bool Roaming { get; set; }
        public bool EggEncounter => EggLocation > 0;

        protected void CloneArrays()
        {
            // dereference original arrays with new copies
            Moves = (int[])Moves?.Clone();
            Relearn = (int[])Relearn.Clone();
            IVs = (int[])IVs.Clone();
            Contest = (int[])Contest.Clone();
        }
        private EncounterStatic Clone()
        {
            var result = (EncounterStatic)MemberwiseClone();
            result.CloneArrays();
            return result;
        }
        public virtual EncounterStatic Clone(int location)
        {
            var result = Clone();
            result.Location = location;
            return result;
        }
        public EncounterStatic[] Clone(int[] locations)
        {
            EncounterStatic[] Encounters = new EncounterStatic[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Encounters[i] = Clone(locations[i]);
            return Encounters;
        }

        public IEnumerable<EncounterStatic> DreamRadarClone()
        {
            for (int i = 0; i < 8; i++)
                yield return DreamRadarClone(5 * i + 5);  //Level from 5->40 depends on the number of badges
        }
        private EncounterStatic DreamRadarClone(int level)
        {
            var result = Clone();
            result.Level = level;
            result.Location = 30015;// Pokemon Dream Radar
            result.Gift = true;     // Only
            result.Ball = 25;       // Dream Ball
            return result;
        }

        public string Name
        {
            get
            {
                const string game = "Static Encounter";
                if (Version == GameVersion.Any)
                    return game;
                return $"{game} ({Version})";
            }
        }
    }

    public class EncounterStaticTyped : EncounterStatic
    {
        public EncounterType TypeEncounter { get; internal set; } = EncounterType.None;
        private EncounterStaticTyped Clone()
        {
            var result = (EncounterStaticTyped)MemberwiseClone();
            result.CloneArrays();
            return result;
        }
        public override EncounterStatic Clone(int location)
        {
            var result = Clone();
            result.Location = location;
            return result;
        }
    }

    public class EncounterStaticShadow : EncounterStatic
    {
        public EncounterLock[][] Locks { get; internal set; } = new EncounterLock[0][];
        public int Gauge { get; internal set; }
        public bool EReader { get; set; }
        private EncounterStaticShadow Clone()
        {
            var result = (EncounterStaticShadow)MemberwiseClone();
            result.CloneArrays();
            result.CloneLocks();
            return result;
        }

        private void CloneLocks()
        {
            Locks = new EncounterLock[Locks.Length][];
            for (var i = 0; i < Locks.Length; i++)
            {
                Locks[i] = (EncounterLock[])Locks[i].Clone();
                for (int j = 0; j < Locks[i].Length; j++)
                    Locks[i][j] = Locks[i][j].Clone();
            }
        }

        public override EncounterStatic Clone(int location)
        {
            var result = Clone();
            result.Location = location;
            return result;
        }
    }
    public class EncounterStaticPID : EncounterStatic
    {
        public uint PID { get; set; }
        public bool NSparkle { get; set; }
        private EncounterStaticPID Clone()
        {
            var result = (EncounterStaticPID)MemberwiseClone();
            result.CloneArrays();
            return result;
        }
        public override EncounterStatic Clone(int location)
        {
            var result = Clone();
            result.Location = location;
            return result;
        }
    }
}
