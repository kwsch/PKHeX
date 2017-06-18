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
    }
    public class EncounterSlot : IEncounterable, IGeneration
    {
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
                return wild + " " + $"{Type.ToString().Replace("_", " ")}";
            }
        }
    }
    public class EncounterSlot1 : EncounterSlot
    {
        public int Rate;
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
                Rate = Rate
            };
        }
    }
}
