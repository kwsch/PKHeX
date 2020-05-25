namespace PKHeX.Core
{
    public sealed class BattleBox6 : SaveBlock
    {
        public BattleBox6(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        private int LockedFlagOffset => Offset + (6 * PokeCrypto.SIZE_6STORED);

        public bool Locked
        {
            get => LockedWiFiTournament || LockedLiveTournament;
            set => LockedWiFiTournament = LockedLiveTournament = value;
        }

        public bool LockedWiFiTournament
        {
            get => (Data[LockedFlagOffset] & 1) != 0;
            set => Data[LockedFlagOffset] = (byte)((Data[Offset + LockedFlagOffset] & ~1) | (value ? 1 : 0));
        }

        public bool LockedLiveTournament
        {
            get => (Data[LockedFlagOffset] & 2) != 0;
            set => Data[LockedFlagOffset] = (byte)((Data[Offset + LockedFlagOffset] & ~2) | (value ? 2 : 0));
        }
    }
}
