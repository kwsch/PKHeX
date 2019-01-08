namespace PKHeX.Core
{
    public sealed class EncounterSlotPermissions
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
}