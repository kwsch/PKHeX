namespace PKHeX.Core
{
    public readonly struct MemoryVariableSet
    {
        public readonly string Handler;

        public readonly int MemoryID;
        public readonly int Variable;
        public readonly int Intensity;
        public readonly int Feeling;

        private MemoryVariableSet(string handler, int m, int v, int i, int f)
        {
            Handler = handler;
            MemoryID = m;
            Variable = v;
            Intensity = i;
            Feeling = f;
        }

        public static MemoryVariableSet Read(ITrainerMemories pkm, int handler) => handler switch
        {
            0 => new MemoryVariableSet(LegalityCheckStrings.L_XOT, pkm.OT_Memory, pkm.OT_TextVar, pkm.OT_Intensity, pkm.OT_Feeling), // OT
            1 => new MemoryVariableSet(LegalityCheckStrings.L_XHT, pkm.HT_Memory, pkm.HT_TextVar, pkm.HT_Intensity, pkm.HT_Feeling), // HT
            _ => new MemoryVariableSet(LegalityCheckStrings.L_XOT, 0, 0, 0, 0),
        };

        public bool Equals(MemoryVariableSet v) => v.Handler   == Handler
                                                && v.MemoryID  == MemoryID
                                                && v.Variable  == Variable
                                                && v.Intensity == Intensity
                                                && v.Feeling   == Feeling;

        public override bool Equals(object obj) => obj is MemoryVariableSet v && Equals(v);
        public override int GetHashCode() => -1;
        public static bool operator ==(MemoryVariableSet left, MemoryVariableSet right) => left.Equals(right);
        public static bool operator !=(MemoryVariableSet left, MemoryVariableSet right) => !(left == right);
    }
}
