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
        public sealed override Span<byte> Nickname_Trash => RawNickname;
        public sealed override Span<byte> OT_Trash => RawOT;

        protected GBPKML(int size, bool jp = false) : base(size)
        {
            int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

            // initialize string buffers
            RawOT = new byte[strLen];
            RawNickname = new byte[strLen];
            RawOT.AsSpan().Fill(StringConverter12.G1TerminatorCode);
            RawNickname.AsSpan().Fill(StringConverter12.G1TerminatorCode);
        }

        protected GBPKML(byte[] data, bool jp = false) : base(data)
        {
            int strLen = jp ? StringLengthJapanese : StringLengthNotJapan;

            // initialize string buffers
            RawOT = new byte[strLen];
            RawNickname = new byte[strLen];
            RawOT.AsSpan().Fill(StringConverter12.G1TerminatorCode);
            RawNickname.AsSpan().Fill(StringConverter12.G1TerminatorCode);
        }

        public override void SetNotNicknamed(int language) => GetNonNickname(language).CopyTo(RawNickname);

        protected override byte[] GetNonNickname(int language)
        {
            var name = SpeciesName.GetSpeciesNameGeneration(Species, language, Format);
            var len = Nickname_Trash.Length;
            byte[] data = new byte[len];
            SetString(name.AsSpan(), data, len, StringConverterOption.Clear50);
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

        private int SetString(ReadOnlySpan<char> value, Span<byte> destBuffer, int maxLength, StringConverterOption option = StringConverterOption.None)
        {
            if (Korean)
                return StringConverter2KOR.SetString(destBuffer, value, maxLength, option);
            return StringConverter12.SetString(destBuffer, value, maxLength, Japanese, option);
        }

        public sealed override string Nickname
        {
            get
            {
                if (Korean)
                    return StringConverter2KOR.GetString(RawNickname);
                return StringConverter12.GetString(RawNickname, Japanese);
            }
            set
            {
                if (!IsNicknamed && Nickname == value)
                    return;

                SetStringKeepTerminatorStyle(value.AsSpan(), RawNickname);
            }
        }

        public sealed override string OT_Name
        {
            get
            {
                if (Korean)
                    return StringConverter2KOR.GetString(RawOT.AsSpan());
                return StringConverter12.GetString(RawOT.AsSpan(), Japanese);
            }
            set
            {
                if (value == OT_Name)
                    return;
                SetStringKeepTerminatorStyle(value.AsSpan(), RawOT);
            }
        }

        private void SetStringKeepTerminatorStyle(ReadOnlySpan<char> value, Span<byte> exist)
        {
            // Reset the destination buffer based on the termination style of the existing string.
            bool zeroed = exist.IndexOf((byte)0) != -1;
            byte fill = zeroed ? (byte)0 : StringConverter12.G1TerminatorCode;
            for (int i = 0; i < exist.Length; i++)
                exist[i] = fill;

            int finalLength = Math.Min(value.Length + 1, exist.Length);
            SetString(value, exist, finalLength);
        }
    }
}
