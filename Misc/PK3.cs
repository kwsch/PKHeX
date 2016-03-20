using System;
using System.Linq;

namespace PKHeX
{
    public class PK3 // 3rd Generation PKM File
    {
        internal const int SIZE_PARTY = 100;
        internal const int SIZE_STORED = 80;
        internal const int SIZE_BLOCK = 12;

        public PK3(byte[] decryptedData = null, string ident = null)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
        }

        // Internal Attributes set on creation
        public byte[] Data; // Raw Storage
        public string Identifier; // User or Form Custom Attribute

        // 0x20 Intro
        public uint PID { get { return BitConverter.ToUInt32(Data, 0x00); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x00); } }
        public uint EID { get { return BitConverter.ToUInt32(Data, 0x04); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x04); } }
        public ushort TID { get { return BitConverter.ToUInt16(Data, 0x04); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x04); } }
        public ushort SID { get { return BitConverter.ToUInt16(Data, 0x06); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x06); } }
        public string Nickname { 
            get { return PKM.getG3Str(Data.Skip(0x08).Take(10).ToArray(), Japanese); } 
            set { byte[] strdata = PKM.setG3Str(value, Japanese);
                if (strdata.Length > 10) 
                    Array.Resize(ref strdata, 10);
                strdata.CopyTo(Data, 0x08); } }
        public int Language { get { return BitConverter.ToUInt16(Data, 0x12); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x12); } }
        public string OT_Name { 
            get { return PKM.getG3Str(Data.Skip(0x14).Take(7).ToArray(), Japanese); } 
            set { byte[] strdata = PKM.setG3Str(value, Japanese);
                if (strdata.Length > 7) 
                    Array.Resize(ref strdata, 7);
                strdata.CopyTo(Data, 0x14); } }
        private byte Markings { get { return Data[0x1B]; } set { Data[0x1B] = value; } }
        public bool Circle { get { return (Markings & (1 << 0)) == 1 << 0; } set { Markings = (byte)(Markings & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool Square { get { return (Markings & (1 << 1)) == 1 << 1; } set { Markings = (byte)(Markings & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool Triangle { get { return (Markings & (1 << 2)) == 1 << 2; } set { Markings = (byte)(Markings & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool Heart { get { return (Markings & (1 << 3)) == 1 << 3; } set { Markings = (byte)(Markings & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public ushort Checksum { get { return BitConverter.ToUInt16(Data, 0x1C); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1C); } }
        public ushort Sanity { get { return BitConverter.ToUInt16(Data, 0x1E); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1E); } }

        #region Block A
        public int Species { get { return PKM.getG4Species(BitConverter.ToUInt16(Data, 0x20)); } set { BitConverter.GetBytes((ushort)PKM.getG3Species(value)).CopyTo(Data, 0x20); } }
        public ushort HeldItem { get { return BitConverter.ToUInt16(Data, 0x22); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x22); } }
        public uint EXP { get { return BitConverter.ToUInt32(Data, 0x24); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x24); } }
        private byte PPUps { get { return Data[0x28]; } set { Data[0x28] = value; } }
        public int Move1_PPUps { get { return (PPUps >> 0) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 0)) | value); } }
        public int Move2_PPUps { get { return (PPUps >> 2) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 2)) | value); } }
        public int Move3_PPUps { get { return (PPUps >> 4) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 4)) | value); } }
        public int Move4_PPUps { get { return (PPUps >> 6) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 6)) | value); } }
        public int Friendship { get { return Data[0x29]; } set { Data[0x29] = (byte)value; } }
        // Unused 0x2A 0x2B
        #endregion

        #region Block B
        public int Move1 { get { return BitConverter.ToUInt16(Data, 0x2C); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2C); } }
        public int Move2 { get { return BitConverter.ToUInt16(Data, 0x2E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E); } }
        public int Move3 { get { return BitConverter.ToUInt16(Data, 0x30); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x30); } }
        public int Move4 { get { return BitConverter.ToUInt16(Data, 0x32); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x32); } }
        public int Move1_PP { get { return Data[0x34]; } set { Data[0x34] = (byte)value; } }
        public int Move2_PP { get { return Data[0x35]; } set { Data[0x35] = (byte)value; } }
        public int Move3_PP { get { return Data[0x36]; } set { Data[0x36] = (byte)value; } }
        public int Move4_PP { get { return Data[0x37]; } set { Data[0x37] = (byte)value; } }
        #endregion

        #region Block C
        public int EV_HP { get { return Data[0x38]; } set { Data[0x38] = (byte)value; } }
        public int EV_ATK { get { return Data[0x39]; } set { Data[0x39] = (byte)value; } }
        public int EV_DEF { get { return Data[0x3A]; } set { Data[0x3A] = (byte)value; } }
        public int EV_SPE { get { return Data[0x3B]; } set { Data[0x3B] = (byte)value; } }
        public int EV_SPA { get { return Data[0x3C]; } set { Data[0x3C] = (byte)value; } }
        public int EV_SPD { get { return Data[0x3D]; } set { Data[0x3D] = (byte)value; } }
        public int CNT_Cool { get { return Data[0x3E]; } set { Data[0x3E] = (byte)value; } }
        public int CNT_Beauty { get { return Data[0x3F]; } set { Data[0x3F] = (byte)value; } }
        public int CNT_Cute { get { return Data[0x40]; } set { Data[0x40] = (byte)value; } }
        public int CNT_Smart { get { return Data[0x41]; } set { Data[0x41] = (byte)value; } }
        public int CNT_Tough { get { return Data[0x42]; } set { Data[0x42] = (byte)value; } }
        public int CNT_Sheen { get { return Data[0x43]; } set { Data[0x43] = (byte)value; } }
        #endregion

        #region Block D
        private byte PKRS { get { return Data[0x44]; } set { Data[0x44] = value; } }
        public int PKRS_Days { get { return PKRS & 0xF; } set { PKRS = (byte)(PKRS & ~0xF | value); } }
        public int PKRS_Strain { get { return PKRS >> 4; } set { PKRS = (byte)(PKRS & 0xF | value << 4); } }
        public int Met_Location { get { return Data[0x45]; } set { Data[0x45] = (byte)value; } }
        // Origins
        private ushort Origins { get { return BitConverter.ToUInt16(Data, 0x46); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x46); } }
        public int Version { get { return (Origins >> 7) & 0xF; } set { Origins = (ushort)((Origins & ~0x780) | ((value & 0xF) << 7));} }
        public int Pokéball { get { return (Origins >> 11) & 0xF; } set { Origins = (ushort)((Origins & ~0x7800) | ((value & 0xF) << 11)); } }
        public int OT_Gender { get { return (Origins >> 15) & 1; } set { Origins = (ushort)(Origins & ~(1 << 15) | ((value & 1) << 15)); } }

        public uint IV32 { get { return BitConverter.ToUInt32(Data, 0x48); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x48); } }
        public int IV_HP { get { return (int)(IV32 >> 00) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); } }
        public int IV_ATK { get { return (int)(IV32 >> 05) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); } }
        public int IV_DEF { get { return (int)(IV32 >> 10) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); } }
        public int IV_SPE { get { return (int)(IV32 >> 15) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); } }
        public int IV_SPA { get { return (int)(IV32 >> 20) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); } }
        public int IV_SPD { get { return (int)(IV32 >> 25) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); } }
        public bool IsEgg { get { return ((IV32 >> 30) & 1) == 1; } set { IV32 = (uint)((IV32 & ~0x40000000) | (uint)(value ? 0x40000000 : 0)); } }
        public int Ability { get { return (int)((IV32 >> 31) & 1); } set { IV32 = (IV32 & 0x7FFFFFFF) | (value == 1 ? 0x80000000 : 0); } }

        private uint RIB0 { get { return BitConverter.ToUInt32(Data, 0x4C); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x4C); } }
        public int Cool_Ribbons  { get { return (int)(RIB0 >> 00) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 00)) | (uint)(value & 7)); } }
        public int Beauty_Ribbons{ get { return (int)(RIB0 >> 03) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 03)) | (uint)(value & 7)); } }
        public int Cute_Ribbons  { get { return (int)(RIB0 >> 06) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 06)) | (uint)(value & 7)); } }
        public int Smart_Ribbons { get { return (int)(RIB0 >> 09) & 3; } set { RIB0 = (uint)((RIB0 & ~(7 << 09)) | (uint)(value & 7)); } }
        public int Tough_Ribbons { get { return (int)(RIB0 >> 12) & 3; } set { RIB0 = (uint)((RIB0 & ~(7 << 12)) | (uint)(value & 7)); } }
        public bool Champion { get { return (RIB0 & (1 << 15)) == 1 << 15; } set { RIB0 = (uint)(RIB0 & ~(1 << 15) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Winning { get { return (RIB0 & (1 << 16)) == 1 << 16; } set { RIB0 = (uint)(RIB0 & ~(1 << 16) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Victory { get { return (RIB0 & (1 << 17)) == 1 << 17; } set { RIB0 = (uint)(RIB0 & ~(1 << 17) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Artist { get { return (RIB0 & (1 << 18)) == 1 << 18; } set { RIB0 = (uint)(RIB0 & ~(1 << 18) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Effort { get { return (RIB0 & (1 << 19)) == 1 << 19; } set { RIB0 = (uint)(RIB0 & ~(1 << 19) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Special1 { get { return (RIB0 & (1 << 20)) == 1 << 20; } set { RIB0 = (uint)(RIB0 & ~(1 << 20) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Special2 { get { return (RIB0 & (1 << 21)) == 1 << 21; } set { RIB0 = (uint)(RIB0 & ~(1 << 21) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Special3 { get { return (RIB0 & (1 << 22)) == 1 << 22; } set { RIB0 = (uint)(RIB0 & ~(1 << 22) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Special4 { get { return (RIB0 & (1 << 23)) == 1 << 23; } set { RIB0 = (uint)(RIB0 & ~(1 << 23) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Special5 { get { return (RIB0 & (1 << 24)) == 1 << 24; } set { RIB0 = (uint)(RIB0 & ~(1 << 24) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Special6 { get { return (RIB0 & (1 << 25)) == 1 << 25; } set { RIB0 = (uint)(RIB0 & ~(1 << 25) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Special7 { get { return (RIB0 & (1 << 26)) == 1 << 26; } set { RIB0 = (uint)(RIB0 & ~(1 << 26) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Unused1 { get { return (RIB0 & (1 << 27)) == 1 << 27; } set { RIB0 = (uint)(RIB0 & ~(1 << 27) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Unused2 { get { return (RIB0 & (1 << 28)) == 1 << 28; } set { RIB0 = (uint)(RIB0 & ~(1 << 28) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Unused3 { get { return (RIB0 & (1 << 29)) == 1 << 29; } set { RIB0 = (uint)(RIB0 & ~(1 << 29) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Unused4 { get { return (RIB0 & (1 << 30)) == 1 << 30; } set { RIB0 = (uint)(RIB0 & ~(1 << 30) | (uint)(value ? 1 << 0 : 0)); } }
        public bool Obedience { get { return (RIB0 & (1 << 31)) == 1 << 31; } set { RIB0 = (RIB0 & ~(1 << 31)) | (uint)(value ? 1 << 0 : 0); } }
        #endregion

        // Simple Generated Attributes
        public bool Japanese => Language == 1;
        public bool Gen3 => Version >= 1 && Version <= 5 || Version == 15;

        public int[] Moves
        {
            get { return new[] { Move1, Move2, Move3, Move4 }; }
            set
            {
                if (value.Length > 0) Move1 = value[0];
                if (value.Length > 1) Move2 = value[1];
                if (value.Length > 2) Move3 = value[2];
                if (value.Length > 3) Move4 = value[3];
            }
        }

        // Methods
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

        public PK4 convertToPK4()
        {
            DateTime moment = DateTime.Now;
            PK4 pk4 = new PK4 // Convert away!
            {
                PID = PID,
                Species = Species,
                TID = TID,
                SID = SID,
                EXP = IsEgg ? PKX.getEXP(5, Species) : EXP,
                IsEgg = false,
                Friendship = 40,
                Circle = Circle,
                Square = Square,
                Triangle = Triangle,
                Heart = Heart,
                Language = Language,
                EV_HP = EV_HP,
                EV_ATK = EV_ATK,
                EV_DEF = EV_DEF,
                EV_SPA = EV_SPA,
                EV_SPD = EV_SPD,
                EV_SPE = EV_SPE,
                CNT_Cool = CNT_Cool,
                CNT_Beauty = CNT_Beauty,
                CNT_Cute = CNT_Cute,
                CNT_Smart = CNT_Smart,
                CNT_Tough = CNT_Tough,
                CNT_Sheen = CNT_Sheen,
                FatefulEncounter = Obedience,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                Move1_PP = PKX.getMovePP(Move1, Move1_PPUps),
                Move2_PP = PKX.getMovePP(Move2, Move2_PPUps),
                Move3_PP = PKX.getMovePP(Move3, Move3_PPUps),
                Move4_PP = PKX.getMovePP(Move4, Move4_PPUps),
                IV_HP = IV_HP,
                IV_ATK = IV_ATK,
                IV_DEF = IV_DEF,
                IV_SPA = IV_SPA,
                IV_SPD = IV_SPD,
                IV_SPE = IV_SPE,
                Ability = PKM.Gen3Abilities[Species][Ability],
                Version = Version,
                Ball = Pokéball,
                PKRS_Strain = PKRS_Strain,
                PKRS_Days = PKRS_Days,
                OT_Gender = OT_Gender,
                Met_Year = moment.Year - 2000,
                Met_Month = moment.Month,
                Met_Day = moment.Day,
                Met_Location = 0x37, // Pal Park
                RIB6_4 = Champion,
                RIB6_5 = Winning,
                RIB6_6 = Victory,
                RIB6_7 = Artist,
                RIB7_0 = Effort,
                RIB7_1 = Special1, // Battle Champion Ribbon
                RIB7_2 = Special2, // Regional Champion Ribbon
                RIB7_3 = Special3, // National Champion Ribbon
                RIB7_4 = Special4, // Country Ribbon
                RIB7_5 = Special5, // National Ribbon
                RIB7_6 = Special6, // Earth Ribbon
                RIB7_7 = Special7, // World Ribbon
            };

            // Remaining Ribbons
            pk4.RIB4_0 |= Cool_Ribbons > 0;
            pk4.RIB4_1 |= Cool_Ribbons > 1;
            pk4.RIB4_2 |= Cool_Ribbons > 2;
            pk4.RIB4_3 |= Cool_Ribbons > 3;
            pk4.RIB4_4 |= Beauty_Ribbons > 0;
            pk4.RIB4_5 |= Beauty_Ribbons > 1;
            pk4.RIB4_6 |= Beauty_Ribbons > 2;
            pk4.RIB4_7 |= Beauty_Ribbons > 3;
            pk4.RIB5_0 |= Cute_Ribbons > 0;
            pk4.RIB5_1 |= Cute_Ribbons > 1;
            pk4.RIB5_2 |= Cute_Ribbons > 2;
            pk4.RIB5_3 |= Cute_Ribbons > 3;
            pk4.RIB5_4 |= Smart_Ribbons > 0;
            pk4.RIB5_5 |= Smart_Ribbons > 1;
            pk4.RIB5_6 |= Smart_Ribbons > 2;
            pk4.RIB5_7 |= Smart_Ribbons > 3;
            pk4.RIB6_0 |= Tough_Ribbons > 0;
            pk4.RIB6_1 |= Tough_Ribbons > 1;
            pk4.RIB6_2 |= Tough_Ribbons > 2;
            pk4.RIB6_3 |= Tough_Ribbons > 3;

            // Yay for reusing string buffers!
            PKM.G4TransferTrashBytes[pk4.Language].CopyTo(pk4.Data, 0x48 + 4);
            pk4.Nickname = IsEgg ? PKM.getSpeciesName(pk4.Species, pk4.Language) : Nickname;
            Array.Copy(pk4.Data, 0x48, pk4.Data, 0x68, 0x10);
            pk4.OT_Name = OT_Name;
            
            // Set Final Data
            pk4.Met_Level = PKX.getLevel(pk4.Species, pk4.EXP);
            pk4.Gender = PKM.getGender(pk4.Species, pk4.PID);
            pk4.IsNicknamed |= pk4.Nickname != PKM.getSpeciesName(pk4.Species, pk4.Language);

            // Unown Form
            if (Species == 201)
                pk4.AltForm = PKM.getUnownForm(PID);

            // Remove HM moves
            int[] banned = { 15, 19, 57, 70, 148, 249, 127, 291 };
            int[] newMoves = pk4.Moves;
            for (int i = 0; i < 4; i++)
                if (banned.Contains(newMoves[i]))
                    newMoves[i] = 0;
            pk4.Moves = newMoves;
            pk4.FixMoves();

            pk4.RefreshChecksum();
            return pk4;
        }
    }
}
