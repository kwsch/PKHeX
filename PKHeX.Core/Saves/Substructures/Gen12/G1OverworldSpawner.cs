using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace PKHeX.Core
{
    public sealed class G1OverworldSpawner
    {
        private readonly SAV1 SAV;
        private readonly bool[] EventFlags;
        private readonly bool[] SpawnFlags;

        public G1OverworldSpawner(SAV1 sav)
        {
            SAV = sav;
            EventFlags = sav.EventFlags;
            SpawnFlags = sav.EventSpawnFlags;
            bool yellow = SAV.Yellow;

            // FlagPairs set for Red/Blue when appropriate.
            FlagEevee = new FlagPairG1 {SpawnFlag = 0x45};
            FlagAerodactyl = new FlagPairG1 {EventFlag = 0x069, SpawnFlag = 0x34};
            FlagHitmonlee = new FlagPairG1 {EventFlag = 0x356, SpawnFlag = 0x4A};
            FlagHitmonchan = new FlagPairG1 {EventFlag = 0x357, SpawnFlag = 0x4B};
            FlagVoltorb_1 = new FlagPairG1 {EventFlag = 0x461, SpawnFlag = 0x4D};
            FlagVoltorb_2 = new FlagPairG1 {EventFlag = 0x462, SpawnFlag = 0x4E};
            FlagVoltorb_3 = new FlagPairG1 { EventFlag = 0x463, SpawnFlag = 0x4F};
            FlagElectrode_1 = new FlagPairG1 {EventFlag = 0x464, SpawnFlag = 0x50};
            FlagVoltorb_4 = new FlagPairG1 {EventFlag = 0x465, SpawnFlag = 0x51};
            FlagVoltorb_5 = new FlagPairG1 {EventFlag = 0x466, SpawnFlag = 0x52};
            FlagElectrode_2 = new FlagPairG1 {EventFlag = 0x467, SpawnFlag = 0x53};
            FlagVoltorb_6 = new FlagPairG1 {EventFlag = 0x468, SpawnFlag = 0x54};
            FlagZapdos = new FlagPairG1 {EventFlag = 0x469, SpawnFlag = 0x55};
            FlagMoltres = new FlagPairG1 {EventFlag = 0x53E, SpawnFlag = 0x5B};
            FlagKabuto = new FlagPairG1 {EventFlag = 0x57E, SpawnFlag = 0x6D};
            FlagOmanyte = new FlagPairG1 {EventFlag = 0x57F, SpawnFlag = 0x6E};
            FlagMewtwo = new FlagPairG1 {EventFlag = 0x8C1, SpawnFlag = 0xD1};
            FlagArticuno = new FlagPairG1 {EventFlag = 0x9DA, SpawnFlag = 0xE3};

            if (yellow) // slightly different
            {
                FlagKabuto.EventFlag = 0x578;
                FlagAerodactyl.SpawnFlag = 0x33;
                FlagMewtwo.SpawnFlag = 0xD7;
                FlagArticuno.SpawnFlag = 0xEB;
                FlagKabuto.SpawnFlag += 2;
                FlagOmanyte.SpawnFlag += 2;

                FlagBulbasaur = new FlagPairG1 { EventFlag = 0x0A8, SpawnFlag = 0x34 };
                FlagSquirtle = new FlagPairG1 { EventFlag = 0x147 }; // Given by Officer Jenny after badged
                FlagCharmander = new FlagPairG1 { EventFlag = 0x54F }; // Given by Damian, doesn't despawn
            }
        }

        private FlagPairG1 FlagMewtwo { get; }
        private FlagPairG1 FlagArticuno { get; }
        private FlagPairG1 FlagZapdos { get; }
        private FlagPairG1 FlagMoltres { get; }
        private FlagPairG1 FlagVoltorb_1 { get; }
        private FlagPairG1 FlagVoltorb_2 { get; }
        private FlagPairG1 FlagVoltorb_3 { get; }
        private FlagPairG1 FlagVoltorb_4 { get; }
        private FlagPairG1 FlagVoltorb_5 { get; }
        private FlagPairG1 FlagVoltorb_6 { get; }
        private FlagPairG1 FlagElectrode_1 { get; }
        private FlagPairG1 FlagElectrode_2 { get; }
        private FlagPairG1 FlagHitmonchan { get; }
        private FlagPairG1 FlagHitmonlee { get; }
        private FlagPairG1 FlagEevee { get; }
        private FlagPairG1 FlagKabuto { get; }
        private FlagPairG1 FlagOmanyte { get; }
        private FlagPairG1 FlagAerodactyl { get; }
        private FlagPairG1? FlagBulbasaur { get; }
        private FlagPairG1? FlagSquirtle { get; }
        private FlagPairG1? FlagCharmander { get; }

        public void Save()
        {
            SAV.EventFlags = EventFlags;
            SAV.EventSpawnFlags = SpawnFlags;
        }

        public IEnumerable<FlagPairG1Detail> GetFlagPairs()
        {
            var pz = ReflectUtil.GetPropertiesStartWithPrefix(GetType(), "Flag");

            foreach (var pair in pz)
            {
                if (!(ReflectUtil.GetValue(this, pair) is FlagPairG1 p))
                    continue;
                yield return new FlagPairG1Detail(p, pair, EventFlags, SpawnFlags);
            }
        }
    }

    public sealed class FlagPairG1
    {
        internal int SpawnFlag { get; set; }
        internal int EventFlag { get; set; }

        internal FlagPairG1() { }
    }

    public sealed class FlagPairG1Detail
    {
        private readonly FlagPairG1 Backing;

        public readonly string Name;
        internal readonly bool[] Event;
        internal readonly bool[] Spawn;

        public FlagPairG1Detail(FlagPairG1 back, string name, bool[] ev, bool[] spawn)
        {
            Backing = back;
            Name = name;
            Event = ev;
            Spawn = spawn;
        }

        public void Invert() => SetState(!IsDespawned);
        public void Reset() => SetState(false);

        public void SetState(bool despawned)
        {
            if (Backing.EventFlag != 0)
                Event[Backing.EventFlag] = despawned;
            if (Backing.SpawnFlag != 0)
                Spawn[Backing.SpawnFlag] = despawned;
        }

        public bool IsDespawned
        {
            get
            {
                bool result = false;
                if (Backing.EventFlag != 0)
                    result |= Event[Backing.EventFlag];
                if (Backing.SpawnFlag != 0)
                    result |= Spawn[Backing.SpawnFlag];
                return result;
            }
        }
    }
}
