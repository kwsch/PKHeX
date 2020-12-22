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
            EventFlags = sav.GetEventFlags();
            SpawnFlags = sav.EventSpawnFlags;
            bool yellow = SAV.Yellow;

            // FlagPairs set for Red/Blue when appropriate.
            FlagEevee = new FlagPairG1(0x45);
            FlagAerodactyl = new FlagPairG1(0x069, 0x34);
            FlagHitmonlee = new FlagPairG1(0x356, 0x4A);
            FlagHitmonchan = new FlagPairG1(0x357, 0x4B);
            FlagVoltorb_1 = new FlagPairG1(0x461, 0x4D);
            FlagVoltorb_2 = new FlagPairG1(0x462, 0x4E);
            FlagVoltorb_3 = new FlagPairG1(0x463, 0x4F);
            FlagElectrode_1 = new FlagPairG1(0x464, 0x50);
            FlagVoltorb_4 = new FlagPairG1(0x465, 0x51);
            FlagVoltorb_5 = new FlagPairG1(0x466, 0x52);
            FlagElectrode_2 = new FlagPairG1(0x467, 0x53);
            FlagVoltorb_6 = new FlagPairG1(0x468, 0x54);
            FlagZapdos = new FlagPairG1(0x469, 0x55);
            FlagMoltres = new FlagPairG1(0x53E, 0x5B);
            FlagKabuto = new FlagPairG1(0x57E, 0x6D);
            FlagOmanyte = new FlagPairG1(0x57F, 0x6E);
            FlagMewtwo = new FlagPairG1(0x8C1, 0xD1);
            FlagArticuno = new FlagPairG1(0x9DA, 0xE3);

            if (yellow) // slightly different
            {
                FlagKabuto = new FlagPairG1(0x578, 0x6D);
                FlagAerodactyl = new FlagPairG1(0x069, 0x33);
                FlagMewtwo = new FlagPairG1(0x8C1, 0xD7);
                FlagArticuno = new FlagPairG1(0x9DA, 0xEB);
                FlagKabuto = new FlagPairG1(0x57E, 0x6F);
                FlagOmanyte = new FlagPairG1(0x57F, 0x70);

                FlagBulbasaur = new FlagPairG1(0x0A8, 0x34);
                FlagSquirtle = new FlagPairG1(0x147, 0); // Given by Officer Jenny after badged
                FlagCharmander = new FlagPairG1(0x54F, 0); // Given by Damian, doesn't despawn
            }
        }

#pragma warning disable IDE0052 // Remove unread private members
        public const string FlagPropertyPrefix = "Flag"; // reflection
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
#pragma warning restore IDE0052 // Remove unread private members

        public void Save()
        {
            SAV.SetEventFlags(EventFlags);
            SAV.EventSpawnFlags = SpawnFlags;
        }

        public IEnumerable<FlagPairG1Detail> GetFlagPairs()
        {
            var pz = ReflectUtil.GetPropertiesStartWithPrefix(GetType(), FlagPropertyPrefix);

            foreach (var pair in pz)
            {
                if (ReflectUtil.GetValue(this, pair) is not FlagPairG1 p)
                    continue;
                yield return new FlagPairG1Detail(p, pair, EventFlags, SpawnFlags);
            }
        }
    }

    public sealed class FlagPairG1
    {
        internal readonly int SpawnFlag;
        internal readonly int EventFlag;

        internal FlagPairG1(int script, int hide) : this(hide) => EventFlag = script;
        internal FlagPairG1(int hide) => SpawnFlag = hide;
    }

    public sealed class FlagPairG1Detail
    {
        private readonly FlagPairG1 Backing;
        public readonly string Name;

        private readonly bool[] Event;
        private readonly bool[] Spawn;

        public FlagPairG1Detail(FlagPairG1 back, string name, bool[] ev, bool[] spawn)
        {
            Backing = back;
            Name = name;
            Event = ev;
            Spawn = spawn;
        }

        public void Invert() => SetState(!IsHidden);
        public void Reset() => SetState(false);

        public void SetState(bool hide)
        {
            if (Backing.EventFlag != 0)
                Event[Backing.EventFlag] = hide;
            if (Backing.SpawnFlag != 0)
                Spawn[Backing.SpawnFlag] = hide;
        }

        public bool IsHidden
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
