using System;
using System.Drawing;
using System.Linq;

namespace PKHeX
{
    public abstract class PKM
    {
        public abstract int SIZE_PARTY { get; }
        public abstract int SIZE_STORED { get; }
        public string Extension => "pk" + Format;

        // Internal Attributes set on creation
        public byte[] Data; // Raw Storage
        public string Identifier; // User or Form Custom Attribute

        public byte[] EncryptedPartyData => Encrypt().Take(SIZE_PARTY).ToArray();
        public byte[] EncryptedBoxData => Encrypt().Take(SIZE_STORED).ToArray();
        public byte[] DecryptedPartyData => Write().Take(SIZE_PARTY).ToArray();
        public byte[] DecryptedBoxData => Write().Take(SIZE_STORED).ToArray();
        
        protected ushort CalculateChecksum()
        {
            ushort chk = 0;
            switch (Format)
            {
                case 3:
                    for (int i = 32; i < SIZE_STORED; i += 2)
                        chk += BitConverter.ToUInt16(Data, i);
                    return chk;
                default: // 4+
                    for (int i = 8; i < SIZE_STORED; i += 2)
                        chk += BitConverter.ToUInt16(Data, i);
                    return chk;
            }
        }
        public abstract byte[] Encrypt();
        public abstract int Format { get; }
        public byte[] Write()
        {
            RefreshChecksum();
            return Data;
        }

        // Surface Properties
        public abstract int Species { get; set; }
        public abstract string Nickname { get; set; }
        public abstract int HeldItem { get; set; }
        public abstract int Gender { get; set; }
        public abstract int Nature { get; set; }
        public abstract int Ability { get; set; }
        public abstract int CurrentFriendship { get; set; }
        public abstract int AltForm { get; set; }
        public abstract bool IsEgg { get; set; }
        public abstract bool IsNicknamed { get; set; }
        public abstract uint EXP { get; set; }
        public abstract int TID { get; set; }
        public abstract string OT_Name { get; set; }
        public abstract int OT_Gender { get; set; }
        public abstract int Ball { get; set; }
        public abstract int Met_Level { get; set; }

        // Battle
        public abstract int Move1 { get; set; }
        public abstract int Move2 { get; set; }
        public abstract int Move3 { get; set; }
        public abstract int Move4 { get; set; }
        public abstract int Move1_PP { get; set; }
        public abstract int Move2_PP { get; set; }
        public abstract int Move3_PP { get; set; }
        public abstract int Move4_PP { get; set; }
        public abstract int Move1_PPUps { get; set; }
        public abstract int Move2_PPUps { get; set; }
        public abstract int Move3_PPUps { get; set; }
        public abstract int Move4_PPUps { get; set; }
        public abstract int EV_HP { get; set; }
        public abstract int EV_ATK { get; set; }
        public abstract int EV_DEF { get; set; }
        public abstract int EV_SPE { get; set; }
        public abstract int EV_SPA { get; set; }
        public abstract int EV_SPD { get; set; }
        public abstract int IV_HP { get; set; }
        public abstract int IV_ATK { get; set; }
        public abstract int IV_DEF { get; set; }
        public abstract int IV_SPE { get; set; }
        public abstract int IV_SPA { get; set; }
        public abstract int IV_SPD { get; set; }
        public abstract int Stat_Level { get; set; }
        public abstract int Stat_HPMax { get; set; }
        public abstract int Stat_HPCurrent { get; set; }
        public abstract int Stat_ATK { get; set; }
        public abstract int Stat_DEF { get; set; }
        public abstract int Stat_SPE { get; set; }
        public abstract int Stat_SPA { get; set; }
        public abstract int Stat_SPD { get; set; }

        // Hidden Properties
        public abstract int Version { get; set; }
        public abstract int SID { get; set; }
        public abstract int PKRS_Strain { get; set; }
        public abstract int PKRS_Days { get; set; }
        public abstract int CNT_Cool { get; set; }
        public abstract int CNT_Beauty { get; set; }
        public abstract int CNT_Cute { get; set; }
        public abstract int CNT_Smart { get; set; }
        public abstract int CNT_Tough { get; set; }
        public abstract int CNT_Sheen { get; set; }

        public abstract uint EncryptionConstant { get; set; }
        public abstract uint PID { get; set; }
        public abstract ushort Sanity { get; set; }
        public abstract ushort Checksum { get; set; }

        // Misc Properties
        public abstract int Language { get; set; }
        public abstract bool FatefulEncounter { get; set; }
        public abstract int TSV { get; }
        public abstract int PSV { get; }
        public abstract int Characteristic { get; }
        public abstract byte MarkByte { get; protected set; }
        public abstract int Met_Location { get; set; }
        public abstract int Egg_Location { get; set; }
        public abstract int OT_Friendship { get; set; }

        // Future Properties
        public virtual int Met_Year { get { return 0; } set { } }
        public virtual int Met_Month { get { return 0; } set { } }
        public virtual int Met_Day { get { return 0; } set { } }
        public virtual int Egg_Year { get { return 0; } set { } }
        public virtual int Egg_Month { get { return 0; } set { } }
        public virtual int Egg_Day { get { return 0; } set { } }
        public virtual int OT_Affection { get { return 0; } set { } }
        public virtual int RelearnMove1 { get { return 0; } set { } }
        public virtual int RelearnMove2 { get { return 0; } set { } }
        public virtual int RelearnMove3 { get { return 0; } set { } }
        public virtual int RelearnMove4 { get { return 0; } set { } }
        public virtual int EncounterType { get { return 0; } set { } }

        // Exposed but not Present in all
        public abstract int CurrentHandler { get; set; }

        // Derived
        public bool IsShiny => TSV == PSV;
        public bool Gen6 => Version >= 24 && Version <= 29;
        public bool XY => Version == (int)GameVersion.X || Version == (int)GameVersion.Y;
        public bool AO => Version == (int)GameVersion.AS || Version == (int)GameVersion.OR;
        public bool SM => Version == (int)GameVersion.SN || Version == (int)GameVersion.MN;
        public bool PtHGSS => new[] {GameVersion.Pt, GameVersion.HG, GameVersion.SS}.Contains((GameVersion)Version);
        public bool Gen5 => Version >= 20 && Version <= 23;
        public bool Gen4 => Version >= 10 && Version < 12 || Version >= 7 && Version <= 8;
        public bool Gen3 => Version >= 1 && Version <= 5 || Version == 15;
        public bool GenU => !(Gen6 || Gen5 || Gen4 || Gen3);
        public bool PKRS_Infected => PKRS_Strain > 0;
        public bool PKRS_Cured => PKRS_Days == 0 && PKRS_Strain > 0;
        public bool ChecksumValid => Checksum == CalculateChecksum();
        public int CurrentLevel => PKX.getLevel(Species, EXP);
        public bool Circle { get { return Markings[0]; } set { Markings[0] = value; } }
        public bool Triangle { get { return Markings[1]; } set { Markings[1] = value; } }
        public bool Square { get { return Markings[2]; } set { Markings[2] = value; } }
        public bool Heart { get { return Markings[3]; } set { Markings[3] = value; } }
        public bool Star { get { return Markings[4]; } set { Markings[4] = value; } }
        public bool Diamond { get { return Markings[5]; } set { Markings[5] = value; } }
        public Image Sprite => PKX.getSprite(this);
        public string ShowdownText => ShowdownSet.getShowdownText(this);
        public string[] QRText => PKX.getQRText(this);
        public string FileName => PKX.getFileName(this);
        public int[] IVs
        {
            get { return new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD }; }
            set
            {
                if (value?.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }
        public int[] EVs
        {
            get { return new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD }; }
            set
            {
                if (value?.Length != 6) return;
                EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
                EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
            }
        }
        public int[] Moves
        {
            get { return new[] { Move1, Move2, Move3, Move4 }; }
            set { if (value?.Length != 4) return; Move1 = value[0]; Move2 = value[1]; Move3 = value[2]; Move4 = value[3]; }
        }

        public bool[] Markings
        {
            get
            {
                bool[] mark = new bool[8];
                for (int i = 0; i < 8; i++)
                    mark[i] = ((MarkByte >> i) & 1) == 1;
                return mark;
            }
            set
            {
                if (value.Length > 8)
                    return;
                byte b = 0;
                for (int i = 0; i < value.Length; i++)
                    b |= (byte)(value[i] ? 1 << i : 0);
                MarkByte = b;
            }
        }

        public int[] CNTs
        {
            get { return new[] { CNT_Cool, CNT_Beauty, CNT_Cute, CNT_Smart, CNT_Tough, CNT_Sheen }; }
            set { if (value?.Length != 6) return; CNT_Cool = value[0]; CNT_Beauty = value[1]; CNT_Cute = value[2]; CNT_Smart = value[3]; CNT_Tough = value[4]; CNT_Sheen = value[5]; }
        }
        public int HPType
        {
            get { return 15 * ((IV_HP & 1) + 2 * (IV_ATK & 1) + 4 * (IV_DEF & 1) + 8 * (IV_SPE & 1) + 16 * (IV_SPA & 1) + 32 * (IV_SPD & 1)) / 63; }
            set
            {
                IV_HP = (IV_HP & ~1) + PKX.hpivs[value, 0];
                IV_ATK = (IV_ATK & ~1) + PKX.hpivs[value, 1];
                IV_DEF = (IV_DEF & ~1) + PKX.hpivs[value, 2];
                IV_SPE = (IV_SPE & ~1) + PKX.hpivs[value, 3];
                IV_SPA = (IV_SPA & ~1) + PKX.hpivs[value, 4];
                IV_SPD = (IV_SPD & ~1) + PKX.hpivs[value, 5];
            }
        }

        // Methods
        public abstract bool getGenderIsValid();
        public void RefreshChecksum() { Checksum = CalculateChecksum(); }
        public void FixMoves()
        {
            if (Move4 != 0 && Move3 == 0)
            {
                Move3 = Move4;
                Move3_PP = Move4_PP;
                Move3_PPUps = Move4_PPUps;
                Move4 = Move4_PP = Move4_PPUps = 0;
            }
            if (Move3 != 0 && Move2 == 0)
            {
                Move2 = Move3;
                Move2_PP = Move3_PP;
                Move2_PPUps = Move3_PPUps;
                Move3 = Move3_PP = Move3_PPUps = 0;
            }
            if (Move2 != 0 && Move1 == 0)
            {
                Move1 = Move2;
                Move1_PP = Move2_PP;
                Move1_PPUps = Move2_PPUps;
                Move2 = Move2_PP = Move2_PPUps = 0;
            }
        }
        public int PotentialRating
        {
            get
            {
                int ivTotal = IVs.Sum();
                if (ivTotal <= 90)
                    return 0;
                if (ivTotal <= 120)
                    return 1;
                return ivTotal <= 150 ? 2 : 3;
            }
        }
        
        public void CalculateStats()
        {
            ushort[] Stats = PKX.getStats(this);
            Stat_HPMax = Stat_HPCurrent = Stats[0];
            Stat_ATK = Stats[1];
            Stat_DEF = Stats[2];
            Stat_SPE = Stats[3];
            Stat_SPA = Stats[4];
            Stat_SPD = Stats[5];
        }

        public abstract PKM Clone();
    }
}
