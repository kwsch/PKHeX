namespace PKHeX.Core
{
    public class EncounterSlotPermissions
    {
        public int StaticIndex { get; set; } = -1;
        public int MagnetPullIndex { get; set; } = -1;
        public int StaticCount { get; set; }
        public int MagnetPullCount { get; set; }

        public bool AllowDexNav { get; set; }
        public bool Pressure { get; set; }
        public bool DexNav { get; set; }
        public bool WhiteFlute { get; set; }
        public bool BlackFlute { get; set; }
        public bool IsNormalLead => !(WhiteFlute || BlackFlute || DexNav);
        public bool IsDexNav => AllowDexNav && DexNav;
        public EncounterSlotPermissions Clone() => (EncounterSlotPermissions)MemberwiseClone();
    }
    /// <summary>
    /// Wild Encounter Slot data
    /// </summary>
    public class EncounterSlot : IEncounterable, IGeneration, ILocation
    {
        public int Species { get; set; }
        public int Form { get; set; }
        public int LevelMin { get; set; }
        public int LevelMax { get; set; }
        public SlotType Type { get; set; } = SlotType.Any;
        public EncounterType TypeEncounter { get; set; } = EncounterType.None;
        public int SlotNumber { get; set; }
        public int Generation { get; set; } = -1;
        internal EncounterSlotPermissions _perm;
        public EncounterSlotPermissions Permissions => _perm ?? (_perm = new EncounterSlotPermissions());

        internal EncounterArea Area { get; set; }
        public int Location { get => Area.Location; set { } }
        public bool EggEncounter => false;
        public int EggLocation { get => 0; set { } }

        public EncounterSlot Clone()
        {
            var slot = (EncounterSlot) MemberwiseClone();
            if (_perm != null)
                slot._perm = Permissions.Clone();
            return slot;
        }

        public bool FixedLevel => LevelMin == LevelMax;

        public bool IsMatchStatic(int index, int count) => index == Permissions.StaticIndex && count == Permissions.StaticCount;
        public bool IsMatchMagnet(int index, int count) => index == Permissions.MagnetPullIndex && count == Permissions.MagnetPullCount;

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
    }
    public class EncounterSlotMoves : EncounterSlot, IMoveset
    {
        public int[] Moves { get; set; }
    }
}
