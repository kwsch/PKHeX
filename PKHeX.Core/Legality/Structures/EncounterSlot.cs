namespace PKHeX.Core
{
    public class EncounterSlotPermissions
    {
        public bool Static { get; set; }
        public bool MagnetPull { get; set; }
        public int StaticCount { get; set; }
        public int MagnetPullCount { get; set; }
        public bool AllowDexNav { get; set; }
        public bool Pressure { get; set; }
        public bool DexNav { get; set; }
        public bool WhiteFlute { get; set; }
        public bool BlackFlute { get; set; }
        public bool IsNormalLead => !(WhiteFlute || BlackFlute || DexNav);
        public bool IsDexNav => AllowDexNav && DexNav;
    }
    /// <summary>
    /// Wild Encounter Slot data
    /// </summary>
    public class EncounterSlot : IEncounterable, IGeneration
    {
        public int Location { get; set; } = -1;
        public int Species { get; set; }
        public int Form { get; set; }
        public int LevelMin { get; set; }
        public int LevelMax { get; set; }
        public SlotType Type { get; set; } = SlotType.Any;
        public EncounterType TypeEncounter { get; set; } = EncounterType.None;
        public int SlotNumber { get; set; }
        public bool EggEncounter => false;
        public int Generation { get; set; } = -1;
        internal EncounterSlotPermissions _perm;
        public EncounterSlotPermissions Permissions => _perm ?? (_perm = new EncounterSlotPermissions());

        public virtual EncounterSlot Clone()
        {
            return new EncounterSlot
            {
                Species = Species,
                LevelMax = LevelMax,
                LevelMin = LevelMin,
                Type = Type,
                SlotNumber = SlotNumber,
                _perm = _perm
            };
        }

        public string Name
        {
            get
            {
                const string wild = "Wild Encounter";
                if (Type == SlotType.Any)
                    return wild;
                return $"{wild} {Type.ToString().Replace("_", " ")}";
            }
        }
    }

    /// <summary>
    /// Generation 1 Wild Encounter Slot data
    /// </summary>
    /// <remarks>
    /// Contains Time data which is present in <see cref="GameVersion.C"/> origin data.
    /// Contains <see cref="GameVersion"/> identification, as this Version value is not stored in <see cref="PK1"/> or <see cref="PK2"/> formats.
    /// </remarks>
    public class EncounterSlot1 : EncounterSlot
    {
        public int Rate;
        internal EncounterTime Time = EncounterTime.Any;
        public GameVersion Version = GameVersion.Any;
        public override EncounterSlot Clone()
        {
            return new EncounterSlot1
            {
                Species = Species,
                LevelMax = LevelMax,
                LevelMin = LevelMin,
                Type = Type,
                SlotNumber = SlotNumber,
                _perm = _perm,
                Rate = Rate,
                Time = Time,
                Generation = Generation,
            };
        }
    }
}
