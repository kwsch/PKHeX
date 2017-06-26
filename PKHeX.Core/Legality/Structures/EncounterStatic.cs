namespace PKHeX.Core
{
    public class EncounterStatic : IEncounterable, IMoveset, IGeneration
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
        public bool NSparkle { get; set; }
        public bool Roaming { get; set; }
        public bool EggEncounter => EggLocation > 0;

        public EncounterStatic[] Clone(int[] locations)
        {
            EncounterStatic[] Encounters = new EncounterStatic[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Encounters[i] = Clone(locations[i]);
            return Encounters;
        }

        public virtual EncounterStatic Clone(int location)
        {
            return new EncounterStatic
            {
                Species = Species,
                Level = Level,
                Location = location,
                Ability = Ability,
                Form = Form,
                Shiny = Shiny,
                Relearn = Relearn,
                Moves = Moves,
                Gender = Gender,
                EggLocation = EggLocation,
                Nature = Nature,
                Gift = Gift,
                Ball = Ball,
                Version = Version,
                IVs = IVs,
                IV3 = IV3,
                Contest = Contest,
                HeldItem = HeldItem,
                Fateful = Fateful,
                RibbonWishing = RibbonWishing,
                SkipFormCheck = SkipFormCheck,
                NSparkle = NSparkle,
                Roaming = Roaming,
                EggCycles = EggCycles,
            };
        }

        public EncounterStatic[] DreamRadarClone()
        {
            EncounterStatic[] Encounters = new EncounterStatic[8];
            for (int i = 0; i < 8; i++)
                Encounters[i] = DreamRadarClone(5 * i + 5);  //Level from 5->40 depends on the number of badage
            return Encounters;
        }

        public EncounterStatic DreamRadarClone(int level)
        {
            return new EncounterStatic
            {
                Species = Species,
                Level = level,
                Location = 30015, //Pokemon Dream Radar
                Ability = Ability,
                Form = Form,
                Shiny = Shiny,
                Relearn = Relearn,
                Moves = Moves,
                Gender = Gender,
                EggLocation = EggLocation,
                Nature = Nature,
                Gift = true,    //Only
                Ball = 25,      //Dream Ball
                Version = Version,
                IVs = IVs,
                IV3 = IV3,
                Contest = Contest,
                HeldItem = HeldItem,
                Fateful = Fateful,
                RibbonWishing = RibbonWishing,
                SkipFormCheck = SkipFormCheck,
                NSparkle = NSparkle,
                Roaming = Roaming,
                EggCycles = EggCycles,
            };
        }

        public string Name
        {
            get
            {
                const string game = "Static Encounter";
                if (Version == GameVersion.Any)
                    return game;
                return game + " " + $"({Version})";
            }
        }
    }

    public class EncounterStaticTyped : EncounterStatic
    {
        public EncounterType TypeEncounter = EncounterType.None;

        public override EncounterStatic Clone(int location)
        {
            return new EncounterStaticTyped
            {
                Species = Species,
                Level = Level,
                Location = location,
                Ability = Ability,
                Form = Form,
                Shiny = Shiny,
                Relearn = Relearn,
                Moves = Moves,
                Gender = Gender,
                EggLocation = EggLocation,
                Nature = Nature,
                Gift = Gift,
                Ball = Ball,
                Version = Version,
                IVs = IVs,
                IV3 = IV3,
                Contest = Contest,
                HeldItem = HeldItem,
                Fateful = Fateful,
                RibbonWishing = RibbonWishing,
                SkipFormCheck = SkipFormCheck,
                NSparkle = NSparkle,
                Roaming = Roaming,
                EggCycles = EggCycles,
                TypeEncounter = TypeEncounter,
            };
        }
    }

    public class EncounterStaticShadow : EncounterStatic
    {
        public EncounterLock[][] Locks = new EncounterLock[0][];
        public int Gauge;
        public bool EReader = false;

        public override EncounterStatic Clone(int location)
        {
            throw new System.NotImplementedException();
        }
    }
}
