using System;
using System.Linq;
using System.Linq.Expressions;

namespace PKHeX
{
    class PK1 : PKM
    {
        // Internal use only
        protected byte[] otname;
        protected byte[] nick;

        public sealed override int SIZE_PARTY => PKX.SIZE_1PARTY;
        public override int SIZE_STORED => PKX.SIZE_1STORED;
        internal const int STRLEN_J = 6;
        internal const int STRLEN_U = 11;
        private int StringLength => Japanese ? STRLEN_J : STRLEN_U;

        public override int Format => 1;

        public bool Japanese => otname.Length == STRLEN_J;

        public PK1(byte[] decryptedData = null, string ident = null, bool jp = false)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
            int strLen = STRLEN_U;
            if (jp)
            {
                strLen = STRLEN_J;
            }
            otname = Enumerable.Repeat((byte) 0x50, strLen).ToArray();
            nick = Enumerable.Repeat((byte) 0x50, strLen).ToArray();
        }

        public override PKM Clone()
        {
            PK1 new_pk1 = new PK1(Data);
            Array.Copy(otname, 0, new_pk1.otname, 0, otname.Length);
            Array.Copy(nick, 0, new_pk1.nick, 0, nick.Length);
            return new_pk1;
        }
        public override string Nickname
        {
            get { return PKX.getG1Str(nick, Japanese); }
            set
            {
                byte[] strdata = PKX.setG1Str(value, Japanese);
                if (strdata.Length > StringLength)
                    throw new ArgumentOutOfRangeException("OT Name too long for given PK1");
                strdata.CopyTo(nick, 0);
            }
        }

        public override string OT_Name
        {
            get { return PKX.getG1Str(otname, Japanese); }
            set
            {
                byte[] strdata = PKX.setG1Str(value, Japanese);
                if (strdata.Length > StringLength)
                    throw new ArgumentOutOfRangeException("OT Name too long for given PK1");
                strdata.CopyTo(otname, 0);
            }
        }

        public override byte[] Encrypt()
        {
            throw new NotImplementedException();
        }

        public override bool IsNicknamed { get { throw new NotImplementedException(); } set { } }

        #region Stored Attributes
        public override int Species
        {
            get { return PKX.getG1Species(Data[0]); }
            set
            {
                // Before updating catch rate, check if Special Yellow Version Pikachu
                if (!(PKX.getG1Species(Data[0]) == 25 && value == 25 && Catch_Rate == 163))
                    Catch_Rate = PersonalTable.RBY[value].CatchRate;
                Data[0] = (byte)PKX.setG1Species(value);
                Type_A = PersonalTable.RBY[value].Types[0];
                Type_B = PersonalTable.RBY[value].Types[1];
            }
        }

        public override int Stat_HPCurrent { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x1)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x1); } }
        public int Status_Condition { get { return Data[4]; } set { Data[4] = (byte)value; } }
        public int Type_A { get { return Data[5]; } set { Data[5] = (byte)value; } }
        public int Type_B { get { return Data[6]; } set { Data[6] = (byte)value; } }
        public int Catch_Rate { get { return Data[7]; } set { Data[7] = (byte)value; } }
        public override int Move1 { get { return Data[8]; } set { Data[8] = (byte) value; } }
        public override int Move2 { get { return Data[9]; } set { Data[9] = (byte)value; } }
        public override int Move3 { get { return Data[10]; } set { Data[10] = (byte)value; } }
        public override int Move4 { get { return Data[11]; } set { Data[11] = (byte)value; } }
        public override int TID { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0xC)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0xC); } }
        public override uint EXP
        {
            get { return Util.SwapEndianness(BitConverter.ToUInt32(Data, 0xE)) & 0x00FFFFFF; }
            set { Array.Copy(BitConverter.GetBytes(Util.SwapEndianness((value << 8) & 0xFFFFFF00)), 0, Data, 0xE, 3); }
        }
        public override int EV_HP { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x11)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x11); } }
        public override int EV_ATK { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x13)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x13); } }
        public override int EV_DEF { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x15)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x15); } }
        public override int EV_SPE { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x17)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x17); } }
        public int EV_SPC { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x19)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x19); } }
        public override int EV_SPA { get { return EV_SPC; } set { EV_SPC = value; } }
        public override int EV_SPD { get { return EV_SPC; } set { EV_SPC = value; } }
        public ushort DV16 { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x1B)); } set { BitConverter.GetBytes(Util.SwapEndianness(value)).CopyTo(Data, 0x1B); } }
        public override int IV_HP { get { return ((IV_ATK & 1) << 3) | ((IV_DEF & 1) << 2) | ((IV_SPD & 1) << 1) | ((IV_SPC & 1) << 0); } set { } }
        public override int IV_ATK { get { return (DV16 >> 12) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 12)) | (ushort)((value > 0xF ? 0xF : value) << 12)); } }
        public override int IV_DEF { get { return (DV16 >> 8) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 8)) | (ushort)((value > 0xF ? 0xF : value) << 8)); } }
        public override int IV_SPE { get { return (DV16 >> 4) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 4)) | (ushort)((value > 0xF ? 0xF : value) << 4)); } }
        public int IV_SPC { get { return (DV16 >> 0) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 0)) | (ushort)((value > 0xF ? 0xF : value) << 0)); } }
        public override int IV_SPA { get { return IV_SPC; } set { IV_SPC = value; } }
        public override int IV_SPD { get { return IV_SPC; } set { IV_SPC = value; } }
        public override int Move1_PP { get { return Data[0x1D] & 0x3F; } set { Data[0x1D] = (byte)((Data[0x1D] & 0xC0) | (value & 0x3F)); } }
        public override int Move2_PP { get { return Data[0x1E] & 0x3F; } set { Data[0x1E] = (byte)((Data[0x1E] & 0xC0) | (value & 0x3F)); } }
        public override int Move3_PP { get { return Data[0x1F] & 0x3F; } set { Data[0x1F] = (byte)((Data[0x1F] & 0xC0) | (value & 0x3F)); } }
        public override int Move4_PP { get { return Data[0x20] & 0x3F; } set { Data[0x20] = (byte)((Data[0x20] & 0xC0) | (value & 0x3F)); } }
        public override int Move1_PPUps { get { return (Data[0x1D] & 0xC0) >> 6; } set { Data[0x1D] = (byte)((Data[0x1D] & 0x3F) | ((value & 0x3) << 6)); } }
        public override int Move2_PPUps { get { return (Data[0x1E] & 0xC0) >> 6; } set { Data[0x1E] = (byte)((Data[0x1E] & 0x3F) | ((value & 0x3) << 6)); } }
        public override int Move3_PPUps { get { return (Data[0x1F] & 0xC0) >> 6; } set { Data[0x1F] = (byte)((Data[0x1F] & 0x3F) | ((value & 0x3) << 6)); } }
        public override int Move4_PPUps { get { return (Data[0x20] & 0xC0) >> 6; } set { Data[0x20] = (byte)((Data[0x20] & 0x3F) | ((value & 0x3) << 6)); } }
        #endregion

        #region Party Attributes
        public override int Stat_Level
        {
            get { return Data[0x21]; }
            set { Data[0x21] = (byte)value; Data[0x3] = (byte)value; }
        }
        public override int Stat_HPMax { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x22)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x22); } }
        public override int Stat_ATK { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x24)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x24); } }
        public override int Stat_DEF { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x26)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x26); } }
        public override int Stat_SPE { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x28)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x28); } }
        public int Stat_SPC { get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x2A)); } set { BitConverter.GetBytes(Util.SwapEndianness((ushort)value)).CopyTo(Data, 0x2A); } }
        // Leave SPA and SPD as alias for SPC
        public override int Stat_SPA { get { return Stat_SPC; } set { Stat_SPC = value; } }
        public override int Stat_SPD { get { return Stat_SPC; } set { Stat_SPC = value; } }
        #endregion

        #region Future, Unused Attributes
        public override bool getGenderIsValid()
        {
            return true;
        }
        public override uint EncryptionConstant { get { return 0; } set { } }
        public override uint PID { get { return 0; } set { } }
        public override int Met_Level { get { return 0; } set { } }
        public override int Nature { get { return 0; } set { } }
        public override int AltForm { get { return 0; } set { } }
        public override bool IsEgg { get { return false; } set { } }
        public override int Gender { get { return 0; } set { } }
        public override int HeldItem { get { return 0; } set { } }
        public override ushort Sanity { get { return 0; } set { } }
        public override ushort Checksum { get { return 0; } set { } }
        public override int Language { get { return 0; } set { } }
        public override bool FatefulEncounter { get { return false; } set { } }
        public override int TSV => 0x0000;
        public override int PSV => 0xFFFF;
        public override int Characteristic => -1;
        public override byte MarkByte { get { return 0; } protected set { } }
        public override int CurrentFriendship { get { return 0; } set { } }
        public override int Ability { get { return 0; } set { } }
        public override int CurrentHandler { get { return 0; } set { } }
        public override int Met_Location { get { return 0; } set { } }
        public override int Egg_Location { get { return 0; } set { } }
        public override int OT_Friendship { get { return 0; } set { } }
        public override int OT_Gender { get { return 0; } set { } }
        public override int Ball { get { return 0; } set { } }
        public override int Version { get { return 0; } set { } }
        public override int SID { get { return 0; } set { } }
        public override int PKRS_Strain { get { return 0; } set { } }
        public override int PKRS_Days { get { return 0; } set { } }
        public override int CNT_Cool { get { return 0; } set { } }
        public override int CNT_Beauty { get { return 0; } set { } }
        public override int CNT_Cute { get { return 0; } set { } }
        public override int CNT_Smart { get { return 0; } set { } }
        public override int CNT_Tough { get { return 0; } set { } }
        public override int CNT_Sheen { get { return 0; } set { } }
        #endregion
    }

    class PokemonList1
    {
        internal const int CAPACITY_DAYCARE = 1;
        internal const int CAPACITY_PARTY = 6;
        internal const int CAPACITY_STORED = 20;
        internal const int CAPACITY_STORED_JP = 30;

        private bool Japanese;

        private int StringLength => Japanese ? PK1.STRLEN_J : PK1.STRLEN_U;

        internal static readonly byte[] EMPTY_LIST = { 0x01, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50 };
        public enum CapacityType
        {
            Daycare = CAPACITY_DAYCARE,
            Party = CAPACITY_PARTY,
            Stored = CAPACITY_STORED,
            StoredJP = CAPACITY_STORED_JP,
            Single
        }

        public static int getEntrySize(CapacityType c)
        {
            return c == CapacityType.Single || c == CapacityType.Party
                ? PKX.SIZE_1PARTY
                : PKX.SIZE_1STORED;
        }

        public static byte getCapacity(CapacityType c)
        {
            return c == CapacityType.Single ? (byte)1 : (byte)c;
        }

        private byte[] getEmptyList(CapacityType c, bool is_JP = false)
        {
            int cap = getCapacity(c);
            return new[] { (byte)0 }.Concat(Enumerable.Repeat((byte)0xFF, cap + 1)).Concat(Enumerable.Repeat((byte)0, getEntrySize(c) * cap)).Concat(Enumerable.Repeat((byte)0x50, (is_JP ? PK1.STRLEN_J : PK1.STRLEN_U) * 2 * cap)).ToArray();
        }

        public PokemonList1(byte[] d, CapacityType c = CapacityType.Single, bool jp = false)
        {
            Japanese = jp;
            Data = d ?? getEmptyList(c, Japanese);
            Capacity = getCapacity(c);
            Entry_Size = getEntrySize(c);

            if (Data.Length != DataSize)
            {
                Array.Resize(ref Data, DataSize);
            }

            Pokemon = new PK1[Capacity];
            for (int i = 0; i < Capacity; i++)
            {
                int base_ofs = 2 + Capacity;
                byte[] dat = Data.Skip(base_ofs + Entry_Size * i).Take(Entry_Size).ToArray();
                Pokemon[i] = new PK1(dat, null, jp);
                Pokemon[i].OT_Name = PKX.getG1Str(Data.Skip(base_ofs + Capacity * Entry_Size + StringLength * i).Take(StringLength).ToArray(), Japanese);
                Pokemon[i].Nickname = PKX.getG1Str(Data.Skip(base_ofs + Capacity * Entry_Size + StringLength * Capacity + StringLength * i).Take(StringLength).ToArray(), Japanese);
            }
        }

        public PokemonList1(CapacityType c = CapacityType.Single, bool jp = false)
            : this(null, c, jp)
        {
            Count = 1;
        }

        public PokemonList1(PK1 pk)
            : this(CapacityType.Single, pk.Japanese)
        {
            this[0] = pk;
            Count = 1;
        }

        private readonly byte[] Data;
        private readonly byte Capacity;
        private readonly int Entry_Size;

        public byte Count
        {
            get { return Data[0]; }
            set { Data[0] = value > Capacity ? Capacity : value; }
        }

        public int GetCapacity()
        {
            return Capacity;
        }

        public readonly PK1[] Pokemon;

        public PK1 this[int i]
        {
            get
            {
                if (i > Capacity || i < 0) throw new IndexOutOfRangeException($"Invalid PokemonList Access: {i}");
                return Pokemon[i];
            }
            set
            {
                if (value == null) return;
                Pokemon[i] = (PK1)value.Clone();
            }
        }

        private void Update()
        {
            for (int i = 0; i < Count; i++)
            {
                Data[1 + i] = (byte)Pokemon[i].Species;
                Array.Copy(Pokemon[i].Data, 0, Data, 2 + Capacity + Entry_Size * i, Entry_Size);
                Array.Copy(PKX.setG1Str(Pokemon[i].OT_Name, Japanese), 0, Data, 2 + Capacity + Capacity * Entry_Size + StringLength * i, StringLength);
                Array.Copy(PKX.setG1Str(Pokemon[i].Nickname, Japanese), 0, Data, 2 + Capacity + Capacity * Entry_Size + StringLength * Capacity + StringLength * i, StringLength);
            }
            Data[1 + Count] = byte.MaxValue;
        }

        public byte[] GetBytes()
        {
            Update();
            return Data;
        }

        private int DataSize => Capacity * (Entry_Size + 1 + 2 * StringLength) + 2;

        public static int GetDataLength(CapacityType c, bool jp = false)
        {
            return getCapacity(c) * (getEntrySize(c) + 1 + 2 * (jp ? PK1.STRLEN_J : PK1.STRLEN_U)) + 2;
        }
    }
}
