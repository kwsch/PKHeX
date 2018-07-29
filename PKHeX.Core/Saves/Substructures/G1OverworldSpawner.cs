using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace PKHeX.Core
{
    public class G1OverworldSpawner
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
            FlagEevee = new FlagPair {SpawnFlag = 0x45};
            FlagAerodactyl = new FlagPair {EventFlag = 0x069, SpawnFlag = 0x34};
            FlagHitmonlee = new FlagPair {EventFlag = 0x356, SpawnFlag = 0x4A};
            FlagHitmonchan = new FlagPair {EventFlag = 0x357, SpawnFlag = 0x4B};
            FlagVoltorb_1 = new FlagPair {EventFlag = 0x461, SpawnFlag = 0x4D};
            FlagVoltorb_2 = new FlagPair {EventFlag = 0x462, SpawnFlag = 0x4E};
            FlagVoltorb_3 = new FlagPair { EventFlag = 0x463, SpawnFlag = 0x4F};
            FlagElectrode_1 = new FlagPair {EventFlag = 0x464, SpawnFlag = 0x50};
            FlagVoltorb_4 = new FlagPair {EventFlag = 0x465, SpawnFlag = 0x51};
            FlagVoltorb_5 = new FlagPair {EventFlag = 0x466, SpawnFlag = 0x52};
            FlagElectrode_2 = new FlagPair {EventFlag = 0x467, SpawnFlag = 0x53};
            FlagVoltorb_6 = new FlagPair {EventFlag = 0x468, SpawnFlag = 0x54};
            FlagZapdos = new FlagPair {EventFlag = 0x469, SpawnFlag = 0x55};
            FlagMoltres = new FlagPair {EventFlag = 0x53E, SpawnFlag = 0x5B};
            FlagKabuto = new FlagPair {EventFlag = 0x57E, SpawnFlag = 0x6D};
            FlagOmanyte = new FlagPair {EventFlag = 0x57F, SpawnFlag = 0x6E};
            FlagMewtwo = new FlagPair {EventFlag = 0x8C1, SpawnFlag = 0xD1};
            FlagArticuno = new FlagPair {EventFlag = 0x9DA, SpawnFlag = 0xE3};

            if (yellow) // slightly different
            {
                FlagKabuto.EventFlag = 0x578;
                FlagAerodactyl.SpawnFlag = 0x33;
                FlagMewtwo.SpawnFlag = 0xD7;
                FlagArticuno.SpawnFlag = 0xEB;
                FlagKabuto.SpawnFlag += 2;
                FlagOmanyte.SpawnFlag += 2;

                FlagBulbasaur = new FlagPair { EventFlag = 0x0A8, SpawnFlag = 0x34 };
                FlagSquirtle = new FlagPair { EventFlag = 0x147 }; // Given by Officer Jenny after badged
                FlagCharmander = new FlagPair { EventFlag = 0x54F }; // Given by Damian, doesn't despawn
            }
        }

        private FlagPair FlagMewtwo { get; }
        private FlagPair FlagArticuno { get; }
        private FlagPair FlagZapdos { get; }
        private FlagPair FlagMoltres { get; }
        private FlagPair FlagVoltorb_1 { get; }
        private FlagPair FlagVoltorb_2 { get; }
        private FlagPair FlagVoltorb_3 { get; }
        private FlagPair FlagVoltorb_4 { get; }
        private FlagPair FlagVoltorb_5 { get; }
        private FlagPair FlagVoltorb_6 { get; }
        private FlagPair FlagElectrode_1 { get; }
        private FlagPair FlagElectrode_2 { get; }
        private FlagPair FlagHitmonchan { get; }
        private FlagPair FlagHitmonlee { get; }
        private FlagPair FlagEevee { get; }
        private FlagPair FlagKabuto { get; }
        private FlagPair FlagOmanyte { get; }
        private FlagPair FlagAerodactyl { get; }
        private FlagPair FlagBulbasaur { get; }
        private FlagPair FlagSquirtle { get; }
        private FlagPair FlagCharmander { get; }

        public class FlagPair
        {
            public string Name { get; internal set; }

            internal int SpawnFlag { get; set; }
            internal int EventFlag { get; set; }
            internal bool[] Event { get; set; }
            internal bool[] Spawn { get; set; }

            public void Invert() => SetState(!IsDespawned);
            public void Reset() => SetState(false);

            public void SetState(bool despawned)
            {
                if (EventFlag != 0)
                    Event[EventFlag] = despawned;
                if (SpawnFlag != 0)
                    Spawn[SpawnFlag] = despawned;
            }

            public bool IsDespawned
            {
                get
                {
                    bool result = false;
                    if (EventFlag != 0)
                        result |= Event[EventFlag];
                    if (SpawnFlag != 0)
                        result |= Spawn[SpawnFlag];
                    return result;
                }
            }

            internal FlagPair() { }
        }

        public void Save()
        {
            SAV.EventFlags = EventFlags;
            SAV.EventSpawnFlags = SpawnFlags;
        }

        public IEnumerable<FlagPair> GetFlagPairs()
        {
            var pz = ReflectUtil.GetPropertiesStartWithPrefix(GetType(), "Flag");

            foreach (var pair in pz)
            {
                if (!(ReflectUtil.GetValue(this, pair) is FlagPair p))
                    continue;
                p.Name = pair;
                p.Event = EventFlags;
                p.Spawn = SpawnFlags;
                yield return p;
            }
        }
    }
}
