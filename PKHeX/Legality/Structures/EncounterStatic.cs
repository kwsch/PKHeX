namespace PKHeX.Core
{
    public class EncounterStatic : IEncounterable
    {
        public int Species { get; set; }
        public int Level;

        public int Location = 0;
        public int Ability = 0;
        public int Form = 0;
        public bool? Shiny = null; // false = never, true = always, null = possible
        public int[] Relearn = new int[4];
        public int[] Moves = new int[4];
        public int Gender = -1;
        public int EggLocation = 0;
        public Nature Nature = Nature.Random;
        public bool Gift = false;
        public int Ball = 4; // Gift Only
        public GameVersion Version = GameVersion.Any;
        public int[] IVs = { -1, -1, -1, -1, -1, -1 };
        public bool IV3;
        public int[] Contest = { 0, 0, 0, 0, 0, 0 };
        public int HeldItem { get; set; }

        public bool Fateful = false;
        public bool RibbonWishing = false;
        public bool SkipFormCheck = false;
        public bool NSparkle = false;
        public bool Roaming = false;

        public EncounterStatic[] Clone(int[] locations)
        {
            EncounterStatic[] Encounters = new EncounterStatic[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Encounters[i] = Clone(locations[i]);
            return Encounters;
        }

        public EncounterStatic Clone(int location)
        {
            return new EncounterStatic()
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
                Roaming = Roaming
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
            return new EncounterStatic()
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
                Roaming = Roaming
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
}
