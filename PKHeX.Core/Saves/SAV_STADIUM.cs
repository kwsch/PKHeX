namespace PKHeX.Core
{
    /// <summary>
    /// Base class for GB Era Stadium files.
    /// </summary>
    public abstract class SAV_STADIUM : SaveFile, ILangDeviantSave
    {
        protected internal sealed override string ShortSummary => $"{OT} ({Version})";
        public sealed override string Extension => ".sav";

        public abstract int SaveRevision { get; }
        public abstract string SaveRevisionString { get; }
        public bool Japanese { get; }
        public bool Korean => false;

        public sealed override int MaxBallID => 0; // unused
        public sealed override int MaxGameID => 99; // unused
        public sealed override int MaxMoney => 999999;
        public sealed override int MaxCoins => 9999;

        /// <summary> If the original input data was swapped endianness. </summary>
        private readonly bool IsPairSwapped;

        protected abstract int TeamCount { get; }
        public sealed override string OT { get; set; }
        public sealed override int Language => Japanese ? 1 : 2;

        protected SAV_STADIUM(byte[] data, bool japanese, bool swap) : base(data)
        {
            Japanese = japanese;
            OT = SaveUtil.GetSafeTrainerName(this, (LanguageID)Language);

            if (!swap)
                return;
            BigEndian.SwapBytes32(Data);
            IsPairSwapped = true;
        }

        protected SAV_STADIUM(bool japanese, int size) : base(size)
        {
            Japanese = japanese;
            OT = SaveUtil.GetSafeTrainerName(this, (LanguageID)Language);
        }

        protected sealed override byte[] DecryptPKM(byte[] data) => data;
        public sealed override int GetPartyOffset(int slot) => -1;
        public override string GetBoxName(int box) => $"Box {box + 1}";
        public sealed override void SetBoxName(int box, string value) { }
        public sealed override bool ChecksumsValid => GetBoxChecksumsValid();
        public sealed override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";
        protected abstract void SetBoxChecksum(int i);
        protected abstract bool GetIsBoxChecksumValid(int i);
        protected sealed override void SetChecksums() => SetBoxChecksums();
        protected abstract void SetBoxMetadata(int i);

        protected void SetBoxChecksums()
        {
            for (int i = 0; i < BoxCount; i++)
            {
                SetBoxMetadata(i);
                SetBoxChecksum(i);
            }
        }

        private bool GetBoxChecksumsValid()
        {
            for (int i = 0; i < BoxCount; i++)
            {
                if (!GetIsBoxChecksumValid(i))
                    return false;
            }
            return true;
        }

        protected sealed override byte[] GetFinalData()
        {
            var result = base.GetFinalData();
            if (IsPairSwapped)
                BigEndian.SwapBytes32(result = (byte[])result.Clone());
            return result;
        }

        public abstract SlotGroup GetTeam(int team);

        public virtual SlotGroup[] GetRegisteredTeams()
        {
            var result = new SlotGroup[TeamCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetTeam(i);
            return result;
        }

        public sealed override string GetString(byte[] data, int offset, int length) => StringConverter12.GetString1(data, offset, length, Japanese);

        public sealed override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter12.SetString1(value, maxLength, Japanese, PadToSize, (byte)PadWith);
        }
    }
}
