using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public partial class WC6
    {
        internal const int Size = 0x108;
        internal const int SizeFull = 0x310;
        internal const uint EonTicketConst = 0x225D73C2;

        public readonly byte[] Data;
        public WC6(byte[] data = null)
        {
            Data = data ?? new byte[Size];
            if (Data.Length == SizeFull)
            {
                Data = Data.Skip(SizeFull - Size).ToArray();
                DateTime now = DateTime.Now;
                Year = (uint)(now.Year - 2000);
                Month = (uint)now.Month;
                Day = (uint)now.Day;
            }
        }

        // General Card Properties
        public int CardID {
            get { return BitConverter.ToUInt16(Data, 0); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0); } }
        public string CardTitle { // Max len 36 char, followed by null terminator
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 2, 72)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(36, '\0')).CopyTo(Data, 2); } }
        private uint Date { 
            get { return BitConverter.ToUInt32(Data, 0x4C); } 
            set { BitConverter.GetBytes(value).CopyTo(Data, 0x4C); } }
        public uint Year {
            get { return Date/10000; }
            set { Date = value*10000 + Date%10000; } }
        public uint Month {
            get { return Date%10000/100; }
            set { Date = Year*10000 + value*100 + Date%100; } }
        public uint Day {
            get { return Date%100; }
            set { Date = Year*10000 + Month*100 + value; } }
        public int CardLocation { get { return Data[0x50]; } set { Data[0x50] = (byte)value; } }

        public int CardType { get { return Data[0x51]; } set { Data[0x51] = (byte)value; } }
        public bool GiftUsed { get { return Data[0x52] >> 1 > 0; } set { Data[0x52] = (byte)(Data[0x52] & ~2 | (value ? 2 : 0)); } }
        public bool MultiObtain { get { return Data[0x53] == 1; } set { Data[0x53] = (byte)(value ? 1 : 0); } }

        // Item Properties
        public bool IsItem { get { return CardType == 1; } set { if (value) CardType = 1; } }
        public int Item {
            get { return BitConverter.ToUInt16(Data, 0x68); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); } }
        public int Quantity {
            get { return BitConverter.ToUInt16(Data, 0x70); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x70); } }
        
        // Pokémon Properties
        public bool IsPokémon { get { return CardType == 0; } set { if (value) CardType = 0; } }
        public int TID { 
            get { return BitConverter.ToUInt16(Data, 0x68); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); } }
        public int SID { 
            get { return BitConverter.ToUInt16(Data, 0x6A); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x6A); } }
        public int OriginGame {
            get { return Data[0x6C]; } 
            set { Data[0x6C] = (byte)value; } }
        public uint EncryptionConstant {
            get { return BitConverter.ToUInt32(Data, 0x70); }
            set { BitConverter.GetBytes(value).CopyTo(Data, 0x70); } }
        public int Pokéball {
            get { return Data[0x76]; } 
            set { Data[0x76] = (byte)value; } }
        public int HeldItem {
            get { return BitConverter.ToUInt16(Data, 0x78); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x78); } }
        public int Move1 {
            get { return BitConverter.ToUInt16(Data, 0x7A); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7A); } }
        public int Move2 {
            get { return BitConverter.ToUInt16(Data, 0x7C); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7C); } }
        public int Move3 {
            get { return BitConverter.ToUInt16(Data, 0x7E); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7E); } }
        public int Move4 {
            get { return BitConverter.ToUInt16(Data, 0x80); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x80); } }
        public int Species {
            get { return BitConverter.ToUInt16(Data, 0x82); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x82); } }
        public int Form {
            get { return Data[0x84]; } 
            set { Data[0x84] = (byte)value; } }
        public int Language {
            get { return Data[0x85]; } 
            set { Data[0x85] = (byte)value; } }
        public string Nickname {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x86, 0x1A)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(12 + 1, '\0')).CopyTo(Data, 0x86); } }
        public int Nature {
            get { return Data[0xA0]; } 
            set { Data[0xA0] = (byte)value; } }
        public int Gender {
            get { return Data[0xA1]; } 
            set { Data[0xA1] = (byte)value; } }
        public int AbilityType {
            get { return Data[0xA2]; } 
            set { Data[0xA2] = (byte)value; } }
        public int PIDType {
            get { return Data[0xA3]; } 
            set { Data[0xA3] = (byte)value; } }
        public int EggLocation {
            get { return BitConverter.ToUInt16(Data, 0xA4); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA4); } }
        public int MetLocation  {
            get { return BitConverter.ToUInt16(Data, 0xA6); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA6); } }

        public int CNT_Cool { get { return Data[0xA9]; } set { Data[0xA9] = (byte)value; } }
        public int CNT_Beauty { get { return Data[0xAA]; } set { Data[0xAA] = (byte)value; } }
        public int CNT_Cute { get { return Data[0xAB]; } set { Data[0xAB] = (byte)value; } }
        public int CNT_Smart { get { return Data[0xAC]; } set { Data[0xAC] = (byte)value; } }
        public int CNT_Tough { get { return Data[0xAD]; } set { Data[0xAD] = (byte)value; } }
        public int CNT_Sheen { get { return Data[0xAE]; } set { Data[0xAE] = (byte)value; } }

        public int IV_HP { get { return Data[0xAF]; } set { Data[0xAF] = (byte)value; } }
        public int IV_ATK { get { return Data[0xB0]; } set { Data[0xB0] = (byte)value; } }
        public int IV_DEF { get { return Data[0xB1]; } set { Data[0xB1] = (byte)value; } }
        public int IV_SPE { get { return Data[0xB2]; } set { Data[0xB2] = (byte)value; } }
        public int IV_SPA { get { return Data[0xB3]; } set { Data[0xB3] = (byte)value; } }
        public int IV_SPD { get { return Data[0xB4]; } set { Data[0xB4] = (byte)value; } }

        public int OTGender { get { return Data[0xB5]; } set { Data[0xB5] = (byte)value; } }
        public string OT {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0xB6, 0x1A)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, 0xB6); } }
        public int Level { get { return Data[0xD0]; } set { Data[0xD0] = (byte)value; } }
        public bool IsEgg { get { return Data[0xD1] == 1; } set { Data[0xD1] = (byte)(value ? 1 : 0); } }
        public uint PID {
            get { return BitConverter.ToUInt32(Data, 0xD4); }
            set { BitConverter.GetBytes(value).CopyTo(Data, 0xD4); } }
        public int RelearnMove1 {
            get { return BitConverter.ToUInt16(Data, 0xD8); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xD8); } }
        public int RelearnMove2 {
            get { return BitConverter.ToUInt16(Data, 0xDA); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDA); } }
        public int RelearnMove3 {
            get { return BitConverter.ToUInt16(Data, 0xDC); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDC); } }
        public int RelearnMove4 {
            get { return BitConverter.ToUInt16(Data, 0xDE); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDE); } }

        private byte RIB0 { get { return Data[0x74]; } set { Data[0x74] = value; } }
        public bool RIB0_0 { get { return (RIB0 & (1 << 0)) == 1 << 0; } set { RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Battle Champ Ribbon
        public bool RIB0_1 { get { return (RIB0 & (1 << 1)) == 1 << 1; } set { RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Regional Champ Ribbon
        public bool RIB0_2 { get { return (RIB0 & (1 << 2)) == 1 << 2; } set { RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // National Champ Ribbon
        public bool RIB0_3 { get { return (RIB0 & (1 << 3)) == 1 << 3; } set { RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Country Ribbon
        public bool RIB0_4 { get { return (RIB0 & (1 << 4)) == 1 << 4; } set { RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // National Ribbon
        public bool RIB0_5 { get { return (RIB0 & (1 << 5)) == 1 << 5; } set { RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Earth Ribbon
        public bool RIB0_6 { get { return (RIB0 & (1 << 6)) == 1 << 6; } set { RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // World Ribbon
        public bool RIB0_7 { get { return (RIB0 & (1 << 7)) == 1 << 7; } set { RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Event Ribbon
        private byte RIB1 { get { return Data[0x75]; } set { Data[0x75] = value; } }
        public bool RIB1_0 { get { return (RIB1 & (1 << 0)) == 1 << 0; } set { RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // World Champ Ribbon
        public bool RIB1_1 { get { return (RIB1 & (1 << 1)) == 1 << 1; } set { RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Birthday Ribbon
        public bool RIB1_2 { get { return (RIB1 & (1 << 2)) == 1 << 2; } set { RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Special Ribbon
        public bool RIB1_3 { get { return (RIB1 & (1 << 3)) == 1 << 3; } set { RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Souvenir Ribbon
        public bool RIB1_4 { get { return (RIB1 & (1 << 4)) == 1 << 4; } set { RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Wishing Ribbon
        public bool RIB1_5 { get { return (RIB1 & (1 << 5)) == 1 << 5; } set { RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Classic Ribbon
        public bool RIB1_6 { get { return (RIB1 & (1 << 6)) == 1 << 6; } set { RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Premier Ribbon
        public bool RIB1_7 { get { return (RIB1 & (1 << 7)) == 1 << 7; } set { RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Empty

        // Meta Accessible Properties
        public int[] IVs => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
        public bool IsNicknamed => Nickname.Length > 0;

        public int[] Moves
        {
            get { return new[] {Move1, Move2, Move3, Move4}; }
            set
            {
                if (value.Length > 0) Move1 = value[0];
                if (value.Length > 1) Move2 = value[1];
                if (value.Length > 2) Move3 = value[2];
                if (value.Length > 3) Move4 = value[3];
            }
        }
        public int[] RelearnMoves
        {
            get { return new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 }; }
            set
            {
                if (value.Length > 0) RelearnMove1 = value[0];
                if (value.Length > 1) RelearnMove2 = value[1];
                if (value.Length > 2) RelearnMove3 = value[2];
                if (value.Length > 3) RelearnMove4 = value[3];
            }
        }

        public PK6 convertToPK6(SAV6 SAV)
        {
            if (!IsPokémon)
                return null;

            int currentLevel = Level > 0 ? Level : (int)(Util.rnd32()%100 + 1);
            PK6 pk = new PK6
            {
                Species = Species,
                HeldItem = HeldItem,
                TID = TID,
                SID = SID,
                Met_Level = currentLevel,
                Nature = Nature != 0xFF ? Nature : (int)(Util.rnd32() % 25),
                Gender = PKX.Personal[Species].Gender == 255 ? 2 : (Gender != 3 ? Gender : PKX.Personal[Species].RandomGender),
                AltForm = Form,
                EncryptionConstant = EncryptionConstant == 0 ? Util.rnd32() : EncryptionConstant,
                Version = OriginGame == 0 ? SAV.Game : OriginGame,
                Language = Language == 0 ? SAV.Language : Language,
                Ball = Pokéball,
                Country = SAV.Country,
                Region = SAV.SubRegion,
                ConsoleRegion = SAV.ConsoleRegion,
                Move1 = Move1, Move2 = Move2, Move3 = Move3, Move4 = Move4,
                Move1_PP = PKX.getBasePP(Move1),
                Move2_PP = PKX.getBasePP(Move2),
                Move3_PP = PKX.getBasePP(Move3),
                Move4_PP = PKX.getBasePP(Move4),
                RelearnMove1 = RelearnMove1, RelearnMove2 = RelearnMove2,
                RelearnMove3 = RelearnMove3, RelearnMove4 = RelearnMove4,
                Met_Location = MetLocation,
                Egg_Location = EggLocation,
                CNT_Cool = CNT_Cool,
                CNT_Beauty = CNT_Beauty,
                CNT_Cute = CNT_Cute,
                CNT_Smart = CNT_Smart,
                CNT_Tough = CNT_Tough,
                CNT_Sheen = CNT_Sheen,

                OT_Name = OT.Length > 0 ? OT : SAV.OT,
                OT_Gender = OTGender != 3 ? OTGender % 2 : SAV.Gender,
                HT_Name = OT.Length > 0 ? SAV.OT : "",
                HT_Gender = OT.Length > 0 ? SAV.Gender : 0,
                CurrentHandler = OT.Length > 0 ? 1 : 0,
                
                EXP = PKX.getEXP(Level, Species),

                // Ribbons
                RIB2_6 = RIB0_3, // Country Ribbon
                RIB2_7 = RIB0_4, // National Ribbon

                RIB3_0 = RIB0_5, // Earth Ribbon
                RIB3_1 = RIB0_6, // World Ribbon
                RIB3_2 = RIB1_5, // Classic Ribbon
                RIB3_3 = RIB1_6, // Premier Ribbon
                RIB3_4 = RIB0_7, // Event Ribbon
                RIB3_5 = RIB1_1, // Birthday Ribbon
                RIB3_6 = RIB1_2, // Special Ribbon
                RIB3_7 = RIB1_3, // Souvenir Ribbon

                RIB4_0 = RIB1_4, // Wishing Ribbon
                RIB4_1 = RIB0_0, // Battle Champ Ribbon
                RIB4_2 = RIB0_1, // Regional Champ Ribbon
                RIB4_3 = RIB0_2, // National Champ Ribbon
                RIB4_4 = RIB1_0, // World Champ Ribbon
                
                OT_Friendship = PKX.getBaseFriendship(Species),
                FatefulEncounter = true,
            };

            if (Day + Month + Year == 0) // No datetime set, typical for wc6full
            {
                DateTime dt = DateTime.Now;
                pk.Met_Day = dt.Day;
                pk.Met_Month = dt.Month;
                pk.Met_Year = dt.Year - 2000;
            }
            else
            {
                pk.Met_Day = (int)Day;
                pk.Met_Month = (int)Month;
                pk.Met_Year = (int)(Year - 2000);
            }

            if (pk.CurrentHandler == 0) // OT
            {
                pk.OT_Memory = 3;
                pk.OT_TextVar = 9;
                pk.OT_Intensity = 1;
                pk.OT_Feeling = Util.rand.Next(0, 9);
            }
            else
            {
                pk.HT_Memory = 3;
                pk.HT_TextVar = 9;
                pk.HT_Intensity = 1;
                pk.HT_Feeling = Util.rand.Next(0, 9);
                pk.HT_Friendship = pk.OT_Friendship;
            }
            pk.IsNicknamed = IsNicknamed;
            pk.Nickname = IsNicknamed ? Nickname : PKX.getSpeciesName(Species, pk.Language);

            // More 'complex' logic to determine final values
            
            // Dumb way to generate random IVs.
            int[] finalIVs = new int[6];
            switch (IVs[0])
            {
                case 0xFE:
                    finalIVs[0] = 31;
                    do { // 31 HP IV, 2 other 31s
                    for (int i = 1; i < 6; i++)
                        finalIVs[i] = IVs[i] > 31 ? (int)(Util.rnd32() & 0x1F) : IVs[i];
                    } while (finalIVs.Count(r => r == 31) < 3); // 31 + 2*31
                    break;
                case 0xFD: 
                    do { // 2 other 31s
                    for (int i = 0; i < 6; i++)
                        finalIVs[i] = IVs[i] > 31 ? (int)(Util.rnd32() & 0x1F) : IVs[i];
                    } while (finalIVs.Count(r => r == 31) < 2); // 2*31
                    break;
                default: // Random IVs
                    for (int i = 0; i < 6; i++)
                        finalIVs[i] = IVs[i] > 31 ? (int)(Util.rnd32() & 0x1F) : IVs[i];
                    break;
            }
            pk.IVs = finalIVs;

            int av = 0;
            switch (AbilityType)
            {
                case 00: // 0 - 0
                case 01: // 1 - 1
                case 02: // 2 - H
                    av = AbilityType;
                    break;
                case 03: // 0/1
                case 04: // 0/1/H
                    av = (int)(Util.rnd32()%(AbilityType - 1));
                    break;
            }
            pk.Ability = PKX.Personal[PKX.Personal[Species].FormeIndex(Species, pk.AltForm)].Abilities[av];
            pk.AbilityNumber = 1 << av;

            switch (PIDType)
            {
                case 00: // Specified
                    pk.PID = PID;
                    break;
                case 01: // Random
                    pk.PID = Util.rnd32();
                    break;
                case 02: // Random Shiny
                    pk.PID = Util.rnd32();
                    pk.PID = (uint)(((TID ^ SID ^ (pk.PID & 0xFFFF)) << 16) + (pk.PID & 0xFFFF));
                    break;
                case 03: // Random Nonshiny
                    do { pk.PID = Util.rnd32(); } while ((uint)(((TID ^ SID ^ (pk.PID & 0xFFFF)) << 16) + (pk.PID & 0xFFFF)) < 16);
                    break;
            }

            if (IsEgg)
            {
                pk.IsEgg = true;
                pk.Egg_Day = (int) Day;
                pk.Egg_Month = (int) Month;
                pk.Egg_Year = (int) Year;
            }

            pk.RefreshChecksum();
            return pk;
        }
    }
}
