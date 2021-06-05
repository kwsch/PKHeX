using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Mainline format for Generation 1 &amp; 2 <see cref="PKM"/> objects.
    /// </summary>
    /// <remarks>This format stores <see cref="PKM.Nickname"/> and <see cref="PKM.OT_Name"/> in buffers separate from the rest of the details.</remarks>
    public abstract class GBPKML : GBPKM
    {
        internal const int StringLengthJapanese = 6;
        internal const int StringLengthNotJapan = 11;
        public sealed override int OTLength => Japanese ? 5 : 7;
        public sealed override int NickLength => Japanese ? 5 : 10;
        public sealed override bool Japanese => RawOT.Length == StringLengthJapanese;

        internal readonly byte[] RawOT;
        internal readonly byte[] RawNickname;

        // Trash Bytes
        public sealed override byte[] Nickname_Trash { get => RawNickname; set { if (value.Length == RawNickname.Length) value.CopyTo(RawNickname, 0); } }
        public sealed override byte[] OT_Trash { get => RawOT; set { if (value.Length == RawOT.Length) value.CopyTo(RawOT, 0); } }

        protected GBPKML(int size, bool jp = false) : base(size)
        {
            int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

            // initialize string buffers
            RawOT = new byte[strLen];
            RawNickname = new byte[strLen];
            for (int i = 0; i < RawOT.Length; i++)
                RawOT[i] = RawNickname[i] = StringConverter12.G1TerminatorCode;
        }

        protected GBPKML(byte[] data, bool jp = false) : base(data)
        {
            int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

            // initialize string buffers
            RawOT = new byte[strLen];
            RawNickname = new byte[strLen];
            for (int i = 0; i < RawOT.Length; i++)
                RawOT[i] = RawNickname[i] = StringConverter12.G1TerminatorCode;
        }

        public override void SetNotNicknamed(int language) => GetNonNickname(language).CopyTo(RawNickname);

        protected override byte[] GetNonNickname(int language)
        {
            var name = SpeciesName.GetSpeciesNameGeneration(Species, language, Format);
            var len = Nickname_Trash.Length;
            var data = SetString(name, len, len, 0x50);
            if (!Korean)
            {
                // Decimal point<->period fix
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0xF2)
                        data[i] = 0xE8;
                }
            }
            return data;
        }

        private byte[] SetString(string value, int maxLength, int padTo = 0, byte padWith = 0)
        {
            if (Korean)
                return StringConverter2KOR.SetString2KOR(value, maxLength - 1, padTo, padWith);
            return StringConverter12.SetString1(value, maxLength - 1, Japanese, padTo, padWith);
        }

        public sealed override string Nickname
        {
            get
            {
                if (Korean)
                    return StringConverter2KOR.GetString2KOR(RawNickname, 0, RawNickname.Length);
                return StringConverter12.GetString1(RawNickname, 0, RawNickname.Length, Japanese);
            }
            set
            {
                if (!IsNicknamed && Nickname == value)
                    return;

                SetStringKeepTerminatorStyle(value, RawNickname);
            }
        }

        public sealed override string OT_Name
        {
            get
            {
                if (Korean)
                    return StringConverter2KOR.GetString2KOR(RawOT, 0, RawOT.Length);
                return StringConverter12.GetString1(RawOT, 0, RawOT.Length, Japanese);
            }
            set
            {
                if (value == OT_Name)
                    return;
                SetStringKeepTerminatorStyle(value, RawOT);
            }
        }

        private void SetStringKeepTerminatorStyle(string value, byte[] exist)
        {
            // Reset the destination buffer based on the termination style of the existing string.
            bool zeroed = Array.IndexOf(exist, (byte)0) != -1;
            byte fill = zeroed ? (byte)0 : StringConverter12.G1TerminatorCode;
            for (int i = 0; i < exist.Length; i++)
                exist[i] = fill;

            int finalLength = Math.Min(value.Length + 1, exist.Length);
            byte[] strdata = SetString(value, finalLength);
            strdata.CopyTo(exist, 0);
        }
    }
}
