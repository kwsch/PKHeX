namespace PKHeX.Core
{
    public class EncounterSlot : IEncounterable
    {
        public int Species { get; set; }
        public int Form;
        public int LevelMin;
        public int LevelMax;
        public SlotType Type = SlotType.Any;
        public bool AllowDexNav;
        public bool Pressure;
        public bool DexNav;
        public bool WhiteFlute;
        public bool BlackFlute;
        public bool Normal => !(WhiteFlute || BlackFlute || DexNav);
        public int SlotNumber;
        public EncounterSlot() { }
        public virtual EncounterSlot Clone()
        {
            return new EncounterSlot
            {
                Species = Species,
                AllowDexNav = AllowDexNav,
                LevelMax = LevelMax,
                LevelMin = LevelMin,
                Type = Type,
                Pressure = Pressure,
                SlotNumber = SlotNumber,
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
        public EncounterSlot1() { }
        public override EncounterSlot Clone()
        {
            return new EncounterSlot1
            {
                Species = Species,
                LevelMax = LevelMax,
                LevelMin = LevelMin,
                Type = Type,
                Rate = Rate,
                SlotNumber = SlotNumber,
            };
        }
    }
}
