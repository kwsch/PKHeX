using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Mainline format for Generation 1 &amp; 2 <see cref="PKM"/> objects.
    /// </summary>
    /// <remarks>This format stores <see cref="PKM.Nickname"/> and <see cref="PKM.OT_Name"/> in buffers separate from the rest of the details.</remarks>
    public abstract class GBPKML : GBPKM
    {
        internal const int STRLEN_J = 6;
        internal const int STRLEN_U = 11;
        public sealed override int OTLength => Japanese ? 5 : 7;
        public sealed override int NickLength => Japanese ? 5 : 10;
        private int StringLength => Japanese ? STRLEN_J : STRLEN_U;
        public sealed override bool Japanese => otname.Length == STRLEN_J;

        internal readonly byte[] otname;
        internal readonly byte[] nick;

        // Trash Bytes
        public sealed override byte[] Nickname_Trash { get => nick; set { if (value.Length == nick.Length) value.CopyTo(nick, 0); } }
        public sealed override byte[] OT_Trash { get => otname; set { if (value.Length == otname.Length) value.CopyTo(otname, 0); } }

        protected GBPKML(int size, bool jp = false) : base(size)
        {
            int strLen = jp ? STRLEN_J : STRLEN_U;

            // initialize string buffers
            otname = new byte[strLen];
            nick = new byte[strLen];
            for (int i = 0; i < otname.Length; i++)
                otname[i] = nick[i] = 0x50;
        }

        protected GBPKML(byte[] data, bool jp = false) : base(data)
        {
            int strLen = jp ? STRLEN_J : STRLEN_U;

            // initialize string buffers
            otname = new byte[strLen];
            nick = new byte[strLen];
            for (int i = 0; i < otname.Length; i++)
                otname[i] = nick[i] = 0x50;
        }

        public override void SetNotNicknamed(int language) => GetNonNickname(language).CopyTo(nick);

        protected override IEnumerable<byte> GetNonNickname(int language)
        {
            var name = SpeciesName.GetSpeciesNameGeneration(Species, language, Format);
            var len = Nickname_Trash.Length;
            var bytes = SetString(name, len);
            var data = bytes.Concat(Enumerable.Repeat((byte)0x50, len - bytes.Length));
            if (!Korean)
                data = data.Select(b => (byte)(b == 0xF2 ? 0xE8 : b)); // Decimal point<->period fix
            return data;
        }

        private byte[] SetString(string value, int maxLength)
        {
            if (Korean)
                return StringConverter2KOR.SetString2KOR(value, maxLength - 1);
            return StringConverter12.SetString1(value, maxLength - 1, Japanese);
        }

        public sealed override string Nickname
        {
            get
            {
                if (Korean)
                    return StringConverter2KOR.GetString2KOR(nick, 0, nick.Length);
                return StringConverter12.GetString1(nick, 0, nick.Length, Japanese);
            }
            set
            {
                if (!IsNicknamed && Nickname == value)
                    return;

                GetStringSpecial(value, StringLength).CopyTo(nick, 0);
            }
        }

        public sealed override string OT_Name
        {
            get
            {
                if (Korean)
                    return StringConverter2KOR.GetString2KOR(otname, 0, otname.Length);
                return StringConverter12.GetString1(otname, 0, otname.Length, Japanese);
            }
            set => GetStringSpecial(value, StringLength).CopyTo(otname, 0);
        }

        private byte[] GetStringSpecial(string value, int length)
        {
            byte[] strdata = SetString(value, length);
            if (nick.All(b => b != 0) || nick[length - 1] != 0x50 || Array.FindIndex(nick, b => b == 0) != strdata.Length - 1)
                return strdata;

            int firstInd = Array.FindIndex(nick, b => b == 0);
            for (int i = firstInd; i < length - 1; i++)
            {
                if (nick[i] != 0)
                    break;
            }

            return strdata.Take(strdata.Length - 1).ToArray();
        }
    }
}
