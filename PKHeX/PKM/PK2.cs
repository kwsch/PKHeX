using System;
using System.Linq;

namespace PKHeX
{
    public class PK2 : PKM
    {
        // Internal use only
        protected internal byte[] otname;
        protected internal byte[] nick;
        public override PersonalInfo PersonalInfo => PersonalTable.C[Species];

        public byte[] OT_Name_Raw => (byte[])otname.Clone();
        public byte[] Nickname_Raw => (byte[])nick.Clone();
        public override bool Valid => Species <= 252;

        public sealed override int SIZE_PARTY => PKX.SIZE_2PARTY;
        public override int SIZE_STORED => PKX.SIZE_2STORED;
        internal const int STRLEN_J = 6;
        internal const int STRLEN_U = 11;
        private int StringLength => Japanese ? STRLEN_J : STRLEN_U;

        public override int Format => 2;

        public bool Japanese => otname.Length == STRLEN_J;

        public override string FileName => $"{Species.ToString("000")} - {Nickname} - {SaveUtil.ccitt16(Encrypt()).ToString("X4")}.{Extension}";

        public PK2(byte[] decryptedData = null, string ident = null, bool jp = false)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
            int strLen = jp ? STRLEN_J : STRLEN_U;
            otname = Enumerable.Repeat((byte) 0x50, strLen).ToArray();
            nick = Enumerable.Repeat((byte) 0x50, strLen).ToArray();

            IsEgg = false; // Egg data stored in Pokemon List.
        }

        public override PKM Clone()
        {
            PK2 new_pk2 = new PK2(Data, Identifier, Japanese);
            Array.Copy(otname, 0, new_pk2.otname, 0, otname.Length);
            Array.Copy(nick, 0, new_pk2.nick, 0, nick.Length);
            new_pk2.IsEgg = IsEgg;
            return new_pk2;
        }
        public override string Nickname
        {
            get { return PKX.getG1Str(nick, Japanese); }
            set
            {
                byte[] strdata = PKX.setG1Str(value, Japanese);
                if (strdata.Length > StringLength)
                    throw new ArgumentOutOfRangeException($"Nickname {value} too long for given PK2");
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
            return new PokemonList2(this).GetBytes();
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
            get { return Data[0]; }
            set
            {
                Data[0] = (byte)value;
            }
        }
        public override int SpriteItem => PKX.getG4Item((byte)HeldItem);
        public override int HeldItem { get { return Data[0x1]; } set { Data[0x1] = (byte)value; } }
        public override int Move1 { get { return Data[2]; } set { Data[2] = (byte) value; } }
        public override int Move2 { get { return Data[3]; } set { Data[3] = (byte)value; } }
        public override int Move3 { get { return Data[4]; } set { Data[4] = (byte)value; } }
        public override int Move4 { get { return Data[5]; } set { Data[5] = (byte)value; } }
        public override int TID { get { return BigEndian.ToUInt16(Data, 6); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 6); } }
        public override uint EXP
        {
            get { return (BigEndian.ToUInt32(Data, 8) >> 8) & 0x00FFFFFF; }
            set { Array.Copy(BigEndian.GetBytes((value << 8) & 0xFFFFFF00), 0, Data, 8, 3); }
        }
        public override int EV_HP { get { return BigEndian.ToUInt16(Data, 0xB); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xB); } }
        public override int EV_ATK { get { return BigEndian.ToUInt16(Data, 0xD); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xD); } }
        public override int EV_DEF { get { return BigEndian.ToUInt16(Data, 0xF); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xF); } }
        public override int EV_SPE { get { return BigEndian.ToUInt16(Data, 0x11); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x11); } }
        public int EV_SPC { get { return BigEndian.ToUInt16(Data, 0x13); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x13); } }
        public override int EV_SPA { get { return EV_SPC; } set { EV_SPC = value; } }
        public override int EV_SPD { get { return EV_SPC; } set { } }
        public ushort DV16 { get { return BigEndian.ToUInt16(Data, 0x15); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x15); } }
        public override int IV_HP { get { return ((IV_ATK & 1) << 3) | ((IV_DEF & 1) << 2) | ((IV_SPE & 1) << 1) | ((IV_SPC & 1) << 0); } set { } }
        public override int IV_ATK { get { return (DV16 >> 12) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 12)) | (ushort)((value > 0xF ? 0xF : value) << 12)); } }
        public override int IV_DEF { get { return (DV16 >> 8) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 8)) | (ushort)((value > 0xF ? 0xF : value) << 8)); } }
        public override int IV_SPE { get { return (DV16 >> 4) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 4)) | (ushort)((value > 0xF ? 0xF : value) << 4)); } }
        public int IV_SPC { get { return (DV16 >> 0) & 0xF; } set { DV16 = (ushort)((DV16 & ~(0xF << 0)) | (ushort)((value > 0xF ? 0xF : value) << 0)); } }
        public override int IV_SPA { get { return IV_SPC; } set { IV_SPC = value; } }
        public override int IV_SPD { get { return IV_SPC; } set { } }
        public override int Move1_PP { get { return Data[0x17] & 0x3F; } set { Data[0x17] = (byte)((Data[0x17] & 0xC0) | (value & 0x3F)); } }
        public override int Move2_PP { get { return Data[0x18] & 0x3F; } set { Data[0x18] = (byte)((Data[0x18] & 0xC0) | (value & 0x3F)); } }
        public override int Move3_PP { get { return Data[0x19] & 0x3F; } set { Data[0x19] = (byte)((Data[0x19] & 0xC0) | (value & 0x3F)); } }
        public override int Move4_PP { get { return Data[0x1A] & 0x3F; } set { Data[0x1A] = (byte)((Data[0x1A] & 0xC0) | (value & 0x3F)); } }
        public override int Move1_PPUps { get { return (Data[0x17] & 0xC0) >> 6; } set { Data[0x17] = (byte)((Data[0x17] & 0x3F) | ((value & 0x3) << 6)); } }
        public override int Move2_PPUps { get { return (Data[0x18] & 0xC0) >> 6; } set { Data[0x18] = (byte)((Data[0x18] & 0x3F) | ((value & 0x3) << 6)); } }
        public override int Move3_PPUps { get { return (Data[0x19] & 0xC0) >> 6; } set { Data[0x19] = (byte)((Data[0x19] & 0x3F) | ((value & 0x3) << 6)); } }
        public override int Move4_PPUps { get { return (Data[0x1A] & 0xC0) >> 6; } set { Data[0x1A] = (byte)((Data[0x1A] & 0x3F) | ((value & 0x3) << 6)); } }
        public override int CurrentFriendship { get { return Data[0x1B]; } set { Data[0x1B] = (byte) value; } }
        private byte PKRS { get { return Data[0x1C]; } set { Data[0x1C] = value; } }
        public override int PKRS_Days { get { return PKRS & 0xF; } set { PKRS = (byte)(PKRS & ~0xF | value); } }
        public override int PKRS_Strain { get { return PKRS >> 4; } set { PKRS = (byte)(PKRS & 0xF | value << 4); } }
        // Crystal only Caught Data
        private int CaughtData { get { return BigEndian.ToUInt16(Data, 0x1D); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x1D); } }
        public int Met_TimeOfDay { get { return (CaughtData >> 14) & 0x3; } set { CaughtData = (CaughtData & 0x3FFF) | ((value & 0x3) << 14); } }
        public override int Met_Level { get { return (CaughtData >> 8) & 0x3F; } set { CaughtData = (CaughtData & 0xC0FF) | ((value & 0x3F) << 8); } }
        public override int OT_Gender { get { return (CaughtData >> 7) & 1; } set { CaughtData = (CaughtData & 0xFFEF) | ((value & 1) << 7); } }
        public override int Met_Location { get { return CaughtData & 0x7F; } set { CaughtData = (CaughtData & 0xFF80) | (value & 0x7F); } }
        
        public override int Stat_Level
        {
            get { return Data[0x1F]; }
            set { Data[0x1F] = (byte)value; }
        }

        #endregion

        #region Party Attributes
        public int Status_Condition { get { return Data[0x20]; } set { Data[0x20] = (byte)value; } }

        public override int Stat_HPCurrent { get { return BigEndian.ToUInt16(Data, 0x22); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x22); } }
        public override int Stat_HPMax { get { return BigEndian.ToUInt16(Data, 0x24); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x24); } }
        public override int Stat_ATK { get { return BigEndian.ToUInt16(Data, 0x26); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x26); } }
        public override int Stat_DEF { get { return BigEndian.ToUInt16(Data, 0x28); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x28); } }
        public override int Stat_SPE { get { return BigEndian.ToUInt16(Data, 0x2A); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2A); } }
        public override int Stat_SPA { get { return BigEndian.ToUInt16(Data, 0x2C); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2C); } }
        public override int Stat_SPD { get { return BigEndian.ToUInt16(Data, 0x2E); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2E); } }
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

        public override bool getGenderIsValid()
        {
            int gv = PersonalInfo.Gender;

            if (gv == 255)
                return Gender == 2;
            if (gv == 254)
                return Gender == 1;
            if (gv == 0)
                return Gender == 0;
            switch (gv)
            {
                case 31:
                    return IV_ATK >= 2 ? Gender == 0 : Gender == 1;
                case 63:
                    return IV_ATK >= 5 ? Gender == 0 : Gender == 1;
                case 127:
                    return IV_ATK >= 7 ? Gender == 0 : Gender == 1;
                case 191:
                    return IV_ATK >= 12 ? Gender == 0 : Gender == 1;
            }
            return false;
        }

        public override bool IsEgg { get; set; }

        public override int Gender
        {
            get
            {
                int gv = PersonalInfo.Gender;
                if (gv == 255)
                    return 2;
                if (gv == 254)
                    return 1;
                if (gv == 0)
                    return 0;
                switch (gv)
                {
                    case 31:
                        return IV_ATK >= 2 ? 0 : 1;
                    case 63:
                        return IV_ATK >= 5 ? 0 : 1;
                    case 127:
                        return IV_ATK >= 7 ? 0 : 1;
                    case 191:
                        return IV_ATK >= 12 ? 0 : 1;
                }
                Console.WriteLine("Unknown Gender value: " + gv);
                return 0;
            }
            set { }
        }

        public bool hasMetData => CaughtData != 0;

        #region Future, Unused Attributes
        public override uint EncryptionConstant { get { return 0; } set { } }
        public override uint PID { get { return 0; } set { } }
        public override int Nature { get { return 0; } set { } }

        public override int AltForm
        {
            get
            {
                if (Species != 201) // Unown
                    return 0;
                else
                {
                    uint formeVal = 0;
                    formeVal |= (uint)((IV_ATK & 0x6) << 5);
                    formeVal |= (uint)((IV_DEF & 0x6) << 3);
                    formeVal |= (uint)((IV_SPE & 0x6) << 1);
                    formeVal |= (uint)((IV_SPC & 0x6) >> 1);
                    return (int)(formeVal / 10);
                }
            }
            set{ }
        }

        public override int HPType
        {
            get { return 4 * (IV_ATK % 4) + (IV_DEF % 4); }
            set
            {

            }
        }
        public override bool IsShiny => IV_DEF == 10 && IV_SPE == 10 && IV_SPC == 10 && (IV_ATK & 2) == 2;
        public override ushort Sanity { get { return 0; } set { } }
        public override bool ChecksumValid => true;
        public override ushort Checksum { get { return 0; } set { } }
        public override int Language { get { return 0; } set { } }
        public override bool FatefulEncounter { get { return false; } set { } }
        public override int TSV => 0x0000;
        public override int PSV => 0xFFFF;
        public override int Characteristic => -1;
        public override int MarkValue { get { return 0; } protected set { } }
        public override int Ability { get { return 0; } set { } }
        public override int CurrentHandler { get { return 0; } set { } }
        public override int Egg_Location { get { return 0; } set { } }
        public override int OT_Friendship { get { return 0; } set { } }
        public override int Ball { get { return 0; } set { } }
        public override int Version { get { return 0; } set { } }
        public override int SID { get { return 0; } set { } }
        public override int CNT_Cool { get { return 0; } set { } }
        public override int CNT_Beauty { get { return 0; } set { } }
        public override int CNT_Cute { get { return 0; } set { } }
        public override int CNT_Smart { get { return 0; } set { } }
        public override int CNT_Tough { get { return 0; } set { } }
        public override int CNT_Sheen { get { return 0; } set { } }
        #endregion

        public PK1 convertToPK1()
        {
            PK1 pk1 = new PK1(null, Identifier, Japanese);
            Array.Copy(Data, 0x1, pk1.Data, 0x7, 0x1A);
            pk1.Species = Species; // This will take care of Typing :)
            pk1.Stat_HPCurrent = Stat_HPCurrent;
            pk1.Stat_Level = Stat_Level;
            // Status = 0
            Array.Copy(otname, 0, pk1.otname, 0, otname.Length);
            Array.Copy(nick, 0, pk1.nick, 0, nick.Length);

            int[] newMoves = pk1.Moves;
            for (int i = 0; i < 4; i++)
                if (newMoves[i] > 165) // not present in Gen 1
                    newMoves[i] = 0;
            pk1.Moves = newMoves;
            pk1.FixMoves();

            return pk1;
        }
    }

    public class PokemonList2
    {
        private const int CAPACITY_DAYCARE = 1;
        private const int CAPACITY_PARTY = 6;
        private const int CAPACITY_STORED = 20;
        private const int CAPACITY_STORED_JP = 30;

        private readonly bool Japanese;

        private int StringLength => Japanese ? PK2.STRLEN_J : PK2.STRLEN_U;

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
                ? PKX.SIZE_2PARTY
                : PKX.SIZE_2STORED;
        }

        public static byte getCapacity(CapacityType c)
        {
            return c == CapacityType.Single ? (byte)1 : (byte)c;
        }

        private byte[] getEmptyList(CapacityType c, bool is_JP = false)
        {
            int cap = getCapacity(c);
            return new[] { (byte)0 }.Concat(Enumerable.Repeat((byte)0xFF, cap + 1)).Concat(Enumerable.Repeat((byte)0, getEntrySize(c) * cap)).Concat(Enumerable.Repeat((byte)0x50, (is_JP ? PK2.STRLEN_J : PK2.STRLEN_U) * 2 * cap)).ToArray();
        }

        public PokemonList2(byte[] d, CapacityType c = CapacityType.Single, bool jp = false)
        {
            Japanese = jp;
            Data = d ?? getEmptyList(c, Japanese);
            Capacity = getCapacity(c);
            Entry_Size = getEntrySize(c);

            if (Data.Length != DataSize)
            {
                Array.Resize(ref Data, DataSize);
            }

            Pokemon = new PK2[Capacity];
            for (int i = 0; i < Capacity; i++)
            {
                int base_ofs = 2 + Capacity;
                byte[] dat = Data.Skip(base_ofs + Entry_Size * i).Take(Entry_Size).ToArray();
                Pokemon[i] = new PK2(dat, null, jp)
                {
                    IsEgg = Data[1 + i] == 0xFD,
                    otname = Data.Skip(base_ofs + Capacity*Entry_Size + StringLength*i).Take(StringLength).ToArray(),
                    nick = Data.Skip(base_ofs + Capacity*Entry_Size + StringLength*Capacity + StringLength*i)
                            .Take(StringLength).ToArray()
                };
            }
        }

        public PokemonList2(CapacityType c = CapacityType.Single, bool jp = false)
            : this(null, c, jp)
        {
            Count = 1;
        }

        public PokemonList2(PK2 pk)
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

        public readonly PK2[] Pokemon;

        public PK2 this[int i]
        {
            get
            {
                if (i > Capacity || i < 0) throw new IndexOutOfRangeException($"Invalid PokemonList Access: {i}");
                return Pokemon[i];
            }
            set
            {
                if (value == null) return;
                Pokemon[i] = (PK2)value.Clone();
            }
        }

        private void Update()
        {
            if (Pokemon.Any(pk => pk.Species == 0))
                Count = (byte)Array.FindIndex(Pokemon, pk => pk.Species == 0);
            else
                Count = Capacity;
            for (int i = 0; i < Count; i++)
            {
                Data[1 + i] = Pokemon[i].IsEgg ? (byte)0xFD : (byte)Pokemon[i].Species;
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
            return getCapacity(c) * (getEntrySize(c) + 1 + 2 * (jp ? PK2.STRLEN_J : PK2.STRLEN_U)) + 2;
        }
    }
}
