using System;
using System.Linq;

namespace PKHeX
{
    public class PK1 : PKM
    {
        // Internal use only
        protected internal byte[] otname;
        protected internal byte[] nick;
        public override PersonalInfo PersonalInfo => PersonalTable.RBY[Species];

        public byte[] OT_Name_Raw => (byte[])otname.Clone();
        public byte[] Nickname_Raw => (byte[])nick.Clone();
        public override bool Valid => Species <= 151 && (Data[0] == 0 || Species != 0);

        public sealed override int SIZE_PARTY => PKX.SIZE_1PARTY;
        public override int SIZE_STORED => PKX.SIZE_1STORED;
        internal const int STRLEN_J = 6;
        internal const int STRLEN_U = 11;
        private int StringLength => Japanese ? STRLEN_J : STRLEN_U;

        public override int Format => 1;

        public bool Japanese => otname.Length == STRLEN_J;

        public override string FileName => $"{Species.ToString("000")} - {Nickname} - {SaveUtil.ccitt16(Encrypt()).ToString("X4")}.{Extension}";

        public PK1(byte[] decryptedData = null, string ident = null, bool jp = false)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
            int strLen = jp ? STRLEN_J : STRLEN_U;
            otname = Enumerable.Repeat((byte) 0x50, strLen).ToArray();
            nick = Enumerable.Repeat((byte) 0x50, strLen).ToArray();
        }

        public override PKM Clone()
        {
            PK1 new_pk1 = new PK1(Data, Identifier, Japanese);
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
                    throw new ArgumentOutOfRangeException($"Nickname {value} too long for given PK1");
                if (nick.Any(b => b == 0) && nick[StringLength - 1] == 0x50 && Array.FindIndex(nick, b => b == 0) == strdata.Length - 1) // Handle JP Mew event with grace
                {
                    int firstInd = Array.FindIndex(nick, b => b == 0);
                    for (int i = firstInd; i < StringLength - 1; i++)
                        if (nick[i] != 0)
                            break;
                    strdata = strdata.Take(strdata.Length - 1).ToArray();
                }
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
                    throw new ArgumentOutOfRangeException($"OT Name {value} too long for given PK1");
                if (otname.Any(b => b == 0) && otname[StringLength - 1] == 0x50 && Array.FindIndex(otname, b => b == 0) == strdata.Length - 1) // Handle JP Mew event with grace
                {
                    int firstInd = Array.FindIndex(otname, b => b == 0);
                    for (int i = firstInd; i < StringLength - 1; i++)
                        if (otname[i] != 0)
                            break;
                    strdata = strdata.Take(strdata.Length - 1).ToArray();
                }
                strdata.CopyTo(otname, 0);
            }
        }

        public override byte[] Encrypt()
        {
            // Oh god this is such total abuse of what this method is meant to do
            // Please forgive me
            return new PokemonList1(this).GetBytes();
        }

        // Please forgive me.
        public override byte[] EncryptedPartyData => Encrypt().ToArray();
        public override byte[] EncryptedBoxData => Encrypt().ToArray();
        public override byte[] DecryptedBoxData => Encrypt().ToArray();
        public override byte[] DecryptedPartyData => Encrypt().ToArray();

        public override bool IsNicknamed
        {
            get
            {
                string spName = PKX.getSpeciesName(Species, Japanese ? 1 : 2).ToUpper();
                spName = spName.Replace(" ", ""); // Gen I/II didn't have a space for Mr. Mime
                return !nick.SequenceEqual(
                        PKX.setG1Str(spName, Japanese)
                            .Concat(Enumerable.Repeat((byte) 0x50, StringLength - spName.Length - 1))
                            .Select(b => (byte)(b == 0xF2 ? 0xE8 : b)));
            }
            set { }
        }

        public void setNotNicknamed()
        {
            string spName = PKX.getSpeciesName(Species, Japanese ? 1 : 2).ToUpper();
            spName = spName.Replace(" ", ""); // Gen I/II didn't have a space for Mr. Mime
            nick = PKX.setG1Str(spName, Japanese)
                      .Concat(Enumerable.Repeat((byte)0x50, StringLength - spName.Length - 1))
                      .Select(b => (byte)(b == 0xF2 ? 0xE8 : b)) // Decimal point<->period fix
                      .ToArray();
        }


        #region Stored Attributes
        public override int Species
        {
            get { return PKX.getG1Species(Data[0]); }
            set
            {
                int currentRate = PersonalInfo.CatchRate;
                Data[0] = (byte)PKX.setG1Species(value);

                // Before updating catch rate, check if non-standard
                if (Catch_Rate == currentRate)
                    Catch_Rate = PersonalInfo.CatchRate;
                Type_A = PersonalInfo.Types[0];
                Type_B = PersonalInfo.Types[1];
            }
        }

        public override int Stat_HPCurrent { get { return BigEndian.ToUInt16(Data, 0x1); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x1); } }
        public int Status_Condition { get { return Data[4]; } set { Data[4] = (byte)value; } }
        public int Type_A { get { return Data[5]; } set { Data[5] = (byte)value; } }
        public int Type_B { get { return Data[6]; } set { Data[6] = (byte)value; } }
        public int Catch_Rate { get { return Data[7]; } set { Data[7] = (byte)value; } }
        public override int Move1 { get { return Data[8]; } set { Data[8] = (byte) value; } }
        public override int Move2 { get { return Data[9]; } set { Data[9] = (byte)value; } }
        public override int Move3 { get { return Data[10]; } set { Data[10] = (byte)value; } }
        public override int Move4 { get { return Data[11]; } set { Data[11] = (byte)value; } }
        public override int TID { get { return BigEndian.ToUInt16(Data, 0xC); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xC); } }
        public override uint EXP
        {
            get { return (BigEndian.ToUInt32(Data, 0xE) >> 8) & 0x00FFFFFF; }
            set { Array.Copy(BigEndian.GetBytes((value << 8) & 0xFFFFFF00), 0, Data, 0xE, 3); }
        }
        public override int EV_HP { get { return BigEndian.ToUInt16(Data, 0x11); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x11); } }
        public override int EV_ATK { get { return BigEndian.ToUInt16(Data, 0x13); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x13); } }
        public override int EV_DEF { get { return BigEndian.ToUInt16(Data, 0x15); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x15); } }
        public override int EV_SPE { get { return BigEndian.ToUInt16(Data, 0x17); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x17); } }
        public int EV_SPC { get { return BigEndian.ToUInt16(Data, 0x19); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x19); } }
        public override int EV_SPA { get { return EV_SPC; } set { EV_SPC = value; } }
        public override int EV_SPD { get { return EV_SPC; } set { } }
        public ushort DV16 { get { return BigEndian.ToUInt16(Data, 0x1B); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x1B); } }
        public override int IV_HP { get { return ((IV_ATK & 1) << 3) | ((IV_DEF & 1) << 2) | ((IV_SPE & 1) << 1) | ((IV_SPC & 1) << 0); } set { } }
        public override int IV_ATK { get { return (DV16 >> 12) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 12)) | (ushort)((value > 0xF ? 0xF : value) << 12)); } }
        public override int IV_DEF { get { return (DV16 >> 8) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 8)) | (ushort)((value > 0xF ? 0xF : value) << 8)); } }
        public override int IV_SPE { get { return (DV16 >> 4) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 4)) | (ushort)((value > 0xF ? 0xF : value) << 4)); } }
        public int IV_SPC { get { return (DV16 >> 0) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 0)) | (ushort)((value > 0xF ? 0xF : value) << 0)); } }
        public override int IV_SPA { get { return IV_SPC; } set { IV_SPC = value; } }
        public override int IV_SPD { get { return IV_SPC; } set { } }
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
        public override int Stat_HPMax { get { return BigEndian.ToUInt16(Data, 0x22); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x22); } }
        public override int Stat_ATK { get { return BigEndian.ToUInt16(Data, 0x24); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x24); } }
        public override int Stat_DEF { get { return BigEndian.ToUInt16(Data, 0x26); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x26); } }
        public override int Stat_SPE { get { return BigEndian.ToUInt16(Data, 0x28); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x28); } }
        public int Stat_SPC { get { return BigEndian.ToUInt16(Data, 0x2A); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2A); } }
        // Leave SPA and SPD as alias for SPC
        public override int Stat_SPA { get { return Stat_SPC; } set { Stat_SPC = value; } }
        public override int Stat_SPD { get { return Stat_SPC; } set { } }
        #endregion

        public override ushort[] getStats(PersonalInfo p)
        {
            ushort[] Stats = new ushort[6];
            for (int i = 0; i < Stats.Length; i++)
            {
                ushort L = (ushort)Stat_Level;
                ushort B = (ushort)p.Stats[i];
                ushort I = (ushort)IVs[i];
                ushort E = // Fixed formula via http://www.smogon.com/ingame/guides/rby_gsc_stats
                    (ushort)Math.Floor(Math.Min(255, Math.Floor(Math.Sqrt(Math.Max(0, EVs[i] - 1)) + 1)) / 4.0);
                Stats[i] = (ushort)Math.Floor((2 * (B + I) + E) * L / 100.0 + 5);
            }
            Stats[0] += (ushort)(5 + Stat_Level); // HP

            return Stats;
        }

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

        public override bool CanHoldItem(ushort[] ValidArray)
        {
            return false;
        }

        public override ushort Sanity { get { return 0; } set { } }

        public override bool ChecksumValid => true;
        public override ushort Checksum { get { return 0; } set { } }
        public override int Language { get { return 0; } set { } }
        public override bool FatefulEncounter { get { return false; } set { } }
        public override int TSV => 0x0000;
        public override int PSV => 0xFFFF;
        public override int Characteristic => -1;
        public override int MarkValue { get { return 0; } protected set { } }
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

        public PK2 convertToPK2()
        {
            PK2 pk2 = new PK2(null, Identifier, Japanese) {Species = Species};
            Array.Copy(Data, 0x7, pk2.Data, 0x1, 0x1A);
            // https://github.com/pret/pokecrystal/blob/master/engine/link.asm#L1132
            if (!Legal.HeldItems_GSC.Contains((ushort)pk2.HeldItem)) 
                switch (pk2.HeldItem)
                {
                    case 0x19:
                        pk2.HeldItem = 0x92; // Leftovers
                        break;
                    case 0x2D:
                        pk2.HeldItem = 0x53; // Bitter Berry
                        break;
                    case 0x32:
                        pk2.HeldItem = 0xAE; // Leftovers
                        break;
                    case 0x5A:
                    case 0x64:
                    case 0x78:
                    case 0x87:
                    case 0xBE:
                    case 0xC3:
                    case 0xDC:
                    case 0xFA:
                    case 0xFF:
                        pk2.HeldItem = 0xAD; // Berry
                        break;
                }
            pk2.CurrentFriendship = pk2.PersonalInfo.BaseFriendship;
            // Pokerus = 0
            // Caught Data = 0
            pk2.Stat_Level = PKX.getLevel(Species, EXP);
            Array.Copy(otname, 0, pk2.otname, 0, otname.Length);
            Array.Copy(nick, 0, pk2.nick, 0, nick.Length);

            return pk2;
        }
    }

    public class PokemonList1
    {
        private const int CAPACITY_DAYCARE = 1;
        private const int CAPACITY_PARTY = 6;
        private const int CAPACITY_STORED = 20;
        private const int CAPACITY_STORED_JP = 30;

        private readonly bool Japanese;

        private int StringLength => Japanese ? PK1.STRLEN_J : PK1.STRLEN_U;

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
                Pokemon[i] = new PK1(dat, null, jp)
                {
                    otname = Data.Skip(base_ofs + Capacity*Entry_Size + StringLength*i).Take(StringLength).ToArray(),
                    nick = Data.Skip(base_ofs + Capacity*Entry_Size + StringLength*Capacity + StringLength*i)
                            .Take(StringLength).ToArray()
                };
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
            if (Pokemon.Any(pk => pk.Species == 0))
                Count = (byte) Array.FindIndex(Pokemon, pk => pk.Species == 0);
            else
                Count = Capacity;
            for (int i = 0; i < Count; i++)
            {
                Data[1 + i] = (byte)PKX.setG1Species(Pokemon[i].Species);
                Array.Copy(Pokemon[i].Data, 0, Data, 2 + Capacity + Entry_Size * i, Entry_Size);
                Array.Copy(Pokemon[i].OT_Name_Raw, 0, Data, 2 + Capacity + Capacity * Entry_Size + StringLength * i, StringLength);
                Array.Copy(Pokemon[i].Nickname_Raw, 0, Data, 2 + Capacity + Capacity * Entry_Size + StringLength * Capacity + StringLength * i, StringLength);
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
