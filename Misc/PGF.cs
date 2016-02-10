using System;
using System.Text;

namespace PKHeX
{
    public class PGF
    {
        internal const int Size = 0xCC;

        public byte[] Data;
        public PGF(byte[] data = null)
        {
            Data = data ?? new byte[Size];
        }
        
        public ushort TID { get { return BitConverter.ToUInt16(Data, 0x00); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x00); } }
        public ushort SID { get { return BitConverter.ToUInt16(Data, 0x02); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x02); } }
        public int OriginGame { get { return Data[0x04]; } set { Data[0x04] = (byte)value; } }
        // Unused 0x05 0x06, 0x07
        public uint PID { get { return BitConverter.ToUInt32(Data, 0x08); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x08); } }

        private byte RIB0 { get { return Data[0x0C]; } set { Data[0x0C] = value; } }
        public bool RIB0_0 { get { return (RIB0 & (1 << 0)) == 1 << 0; } set { RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Country Ribbon
        public bool RIB0_1 { get { return (RIB0 & (1 << 1)) == 1 << 1; } set { RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // National Ribbon
        public bool RIB0_2 { get { return (RIB0 & (1 << 2)) == 1 << 2; } set { RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Earth Ribbon
        public bool RIB0_3 { get { return (RIB0 & (1 << 3)) == 1 << 3; } set { RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // World Ribbon
        public bool RIB0_4 { get { return (RIB0 & (1 << 4)) == 1 << 4; } set { RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Classic Ribbon
        public bool RIB0_5 { get { return (RIB0 & (1 << 5)) == 1 << 5; } set { RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Premier Ribbon
        public bool RIB0_6 { get { return (RIB0 & (1 << 6)) == 1 << 6; } set { RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Event Ribbon
        public bool RIB0_7 { get { return (RIB0 & (1 << 7)) == 1 << 7; } set { RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Birthday Ribbon
        private byte RIB1 { get { return Data[0x0D]; } set { Data[0x0D] = value; } }
        public bool RIB1_0 { get { return (RIB1 & (1 << 0)) == 1 << 0; } set { RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Special Ribbon
        public bool RIB1_1 { get { return (RIB1 & (1 << 1)) == 1 << 1; } set { RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Souvenir Ribbon
        public bool RIB1_2 { get { return (RIB1 & (1 << 2)) == 1 << 2; } set { RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Wishing Ribbon
        public bool RIB1_3 { get { return (RIB1 & (1 << 3)) == 1 << 3; } set { RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Battle Champ Ribbon
        public bool RIB1_4 { get { return (RIB1 & (1 << 4)) == 1 << 4; } set { RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Regional Champ Ribbon
        public bool RIB1_5 { get { return (RIB1 & (1 << 5)) == 1 << 5; } set { RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // National Champ Ribbon
        public bool RIB1_6 { get { return (RIB1 & (1 << 6)) == 1 << 6; } set { RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // World Champ Ribbon
        public bool RIB1_7 { get { return (RIB1 & (1 << 7)) == 1 << 7; } set { RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Empty

        public int Pokéball { get { return Data[0x0E]; } set { Data[0x0E] = (byte)value; } }
        public int HeldItem { get { return BitConverter.ToUInt16(Data, 0x10); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x10); } }
        public int Move1 { get { return BitConverter.ToUInt16(Data, 0x12); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x12); } }
        public int Move2 { get { return BitConverter.ToUInt16(Data, 0x14); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x14); } }
        public int Move3 { get { return BitConverter.ToUInt16(Data, 0x16); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x16); } }
        public int Move4 { get { return BitConverter.ToUInt16(Data, 0x18); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x18); } }
        public int Species { get { return BitConverter.ToUInt16(Data, 0x1A); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1A); } }
        public int Form { get { return Data[0x1C]; } set { Data[0x1C] = (byte)value; } }
        public int Language { get { return Data[0x1D]; } set { Data[0x1D] = (byte)value; } }
        public string Nickname
        {
            get { return PKM.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x1E, 0x16)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(0xB, (char)0xFFFF)).CopyTo(Data, 0x1E); }
        }
        public int Nature { get { return Data[0x34]; } set { Data[0x34] = (byte)value; } }
        public int Gender { get { return Data[0x35]; } set { Data[0x35] = (byte)value; } }
        public int AbilityType { get { return Data[0x36]; } set { Data[0x36] = (byte)value; } }
        public int PIDType { get { return Data[0x37]; } set { Data[0x37] = (byte)value; } }
        public ushort EggLocation { get { return BitConverter.ToUInt16(Data, 0x38); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x38); } }
        public ushort MetLocation { get { return BitConverter.ToUInt16(Data, 0x3A); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x3A); } }
        public int MetLevel { get { return Data[0x3C]; } set { Data[0x3C] = (byte)value; } }
        public int CNT_Cool { get { return Data[0x3D]; } set { Data[0x3D] = (byte)value; } }
        public int CNT_Beauty { get { return Data[0x3E]; } set { Data[0x3E] = (byte)value; } }
        public int CNT_Cute { get { return Data[0x3F]; } set { Data[0x3F] = (byte)value; } }
        public int CNT_Smart { get { return Data[0x40]; } set { Data[0x40] = (byte)value; } }
        public int CNT_Tough { get { return Data[0x41]; } set { Data[0x41] = (byte)value; } }
        public int CNT_Sheen { get { return Data[0x42]; } set { Data[0x42] = (byte)value; } }
        public int IV_HP { get { return Data[0x43]; } set { Data[0x43] = (byte)value; } }
        public int IV_ATK { get { return Data[0x44]; } set { Data[0x44] = (byte)value; } }
        public int IV_DEF { get { return Data[0x45]; } set { Data[0x45] = (byte)value; } }
        public int IV_SPE { get { return Data[0x46]; } set { Data[0x46] = (byte)value; } }
        public int IV_SPA { get { return Data[0x47]; } set { Data[0x47] = (byte)value; } }
        public int IV_SPD { get { return Data[0x48]; } set { Data[0x48] = (byte)value; } }
        // Unused 0x49
        public string OT {
            get { return PKM.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x4A, 0x10)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(0x08, (char)0xFFFF)).CopyTo(Data, 0x4A); } }
        public int OTGender { get { return Data[0x5A]; } set { Data[0x5A] = (byte)value; } }
        public int Level { get { return Data[0x5B]; } set { Data[0x5C] = (byte)value; } }
        public bool IsEgg { get { return Data[0x5C] == 1; } set { Data[0x5C] = (byte)(value ? 1 : 0); } }
        // Unused 0x5D 0x5E 0x5F

        // Card Attributes
        public ushort Item { get { return BitConverter.ToUInt16(Data, 0x00); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x00); } }
        
        public ushort Year { get { return BitConverter.ToUInt16(Data, 0xAE); } set { BitConverter.GetBytes(value).CopyTo(Data, 0xAE); } }
        public byte Month { get { return Data[0xAD]; } set { Data[0xAD] = value; } }
        public byte Day { get { return Data[0xAC]; } set { Data[0xAC] = value; } }
        public int CardID
        {
            get { return BitConverter.ToUInt16(Data, 0xB0); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xB0); }
        }
        public int CardLocation { get { return Data[0xB2]; } set { Data[0xB2] = (byte)value; } }
        public int CardType { get { return Data[0xB3]; } set { Data[0xB3] = (byte)value; } }
        public bool GiftUsed { get { return Data[0xB4] >> 1 > 0; } set { Data[0xB4] = (byte)(Data[0xB4] & ~2 | (value ? 2 : 0)); } }
        public bool MultiObtain { get { return Data[0xB4] == 1; } set { Data[0xB4] = (byte)(value ? 1 : 0); } }
        
        // Meta Accessible Properties
        public int[] IVs => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
        public bool IsNicknamed => Nickname.Length > 0;

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
        public bool IsPokémon { get { return CardType == 1; } set { if (value) CardType = 1; } }
        public bool IsItem { get { return CardType == 2; } set { if (value) CardType = 2; } }
        public bool IsPower { get { return CardType == 3; } set { if (value) CardType = 3; } }

        public PK5 convertToPK5(SAV6 SAV)
        {
            if (!IsPokémon)
                return null;

            DateTime dt = DateTime.Now;
            if (Day == 0)
            {
                Day = (byte)dt.Day;
                Month = (byte)dt.Month;
                Year = (byte)dt.Year;
            }
            int currentLevel = Level > 0 ? Level : (int)(Util.rnd32() % 100 + 1);
            PK5 pk = new PK5
            {
                Species = Species,
                HeldItem = HeldItem,
                Met_Level = currentLevel,
                Nature = Nature != 0xFF ? Nature : (int)(Util.rnd32() % 25),
                Gender = PKX.Personal[Species].Gender == 255 ? 2 : (Gender != 2 ? Gender : PKX.Personal[Species].RandomGender),
                AltForm = Form,
                Version = OriginGame == 0 ? new[] {20, 21, 22, 23}[Util.rnd32() & 0x3] : OriginGame,
                Language = Language == 0 ? SAV.Language : Language,
                Ball = Pokéball,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PP = PKX.getBasePP(Move1),
                Move2_PP = PKX.getBasePP(Move2),
                Move3_PP = PKX.getBasePP(Move3),
                Move4_PP = PKX.getBasePP(Move4),
                Met_Location = MetLocation,
                Met_Day = Day,
                Met_Month = Month,
                Met_Year = Year - 2000,
                Egg_Location = EggLocation,
                CNT_Cool = CNT_Cool,
                CNT_Beauty = CNT_Beauty,
                CNT_Cute = CNT_Cute,
                CNT_Smart = CNT_Smart,
                CNT_Tough = CNT_Tough,
                CNT_Sheen = CNT_Sheen,

                EXP = PKX.getEXP(Level, Species),

                // Ribbons
                RIB7_4 = RIB0_0, // Country Ribbon
                RIB7_5 = RIB0_1, // National Ribbon
                RIB7_6 = RIB0_2, // Earth Ribbon
                RIB7_7 = RIB0_3, // World Ribbon
                RIB3_2 = RIB0_4, // Classic Ribbon
                RIB3_3 = RIB0_5, // Premier Ribbon
                RIB2_3 = RIB0_6, // Event Ribbon
                RIB2_6 = RIB0_7, // Birthday Ribbon

                RIB2_7 = RIB1_0, // Special Ribbon
                RIB3_0 = RIB1_1, // Souvenir Ribbon
                RIB3_1 = RIB1_2, // Wishing Ribbon
                RIB7_1 = RIB1_3, // Battle Champ Ribbon
                RIB7_2 = RIB1_4, // Regional Champ Ribbon
                RIB7_3 = RIB1_5, // National Champ Ribbon
                RIB2_5 = RIB1_6, // World Champ Ribbon

                Friendship = PKX.getBaseFriendship(Species),
                FatefulEncounter = true,
            };
            if (OTGender == 3) // User's
            {
                pk.TID = 12345;
                pk.SID = 54321;
                pk.OT_Name = "PKHeX";
                pk.OT_Gender = 1; // Red PKHeX OT
            }
            else
            {
                pk.TID = TID;
                pk.SID = SID;
                pk.OT_Name = OT.Length > 0 ? OT : "PKHeX";
                pk.OT_Gender = OTGender % 2; // %2 just in case?
            }
            pk.IsNicknamed = IsNicknamed;
            pk.Nickname = IsNicknamed ? Nickname : PKX.getSpeciesName(Species, pk.Language);

            // More 'complex' logic to determine final values

            // Dumb way to generate random IVs.
            int[] finalIVs = new int[6];
            for (int i = 0; i < IVs.Length; i++)
                finalIVs[i] = IVs[i] == 0xFF ? (int)(Util.rnd32() & 0x1F) : IVs[i];
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
                    av = (int)(Util.rnd32() % (AbilityType - 1));
                    break;
            }
            pk.HiddenAbility = av == 2;
            pk.Ability = PKX.Personal[PKX.Personal[Species].FormeIndex(Species, pk.AltForm)].Abilities[av];

            if (PID != 0) 
                pk.PID = PID;
            else
            {
                pk.PID = Util.rnd32();
                // Force Ability
                if (av == 0) pk.PID &= 0xFFFEFFFF; else pk.PID |= 0x10000;
                // Force Gender
                do { pk.PID = (pk.PID & 0xFFFFFF00) | Util.rnd32() & 0xFF; } while (!pk.getGenderIsValid());
                if (PIDType == 2) // Force Shiny
                {
                    uint gb = pk.PID & 0xFF;
                    pk.PID = (uint)(((gb ^ pk.TID ^ pk.SID) & 0xFFFE) << 16) | gb;
                    if (av == 0) pk.PID &= 0xFFFEFFFE; else pk.PID |= 0x10001;
                }
                else if (PIDType != 1) // Force Not Shiny
                {
                    if (((pk.PID >> 16) ^ (pk.PID & 0xffff) ^ pk.SID ^ pk.TID) < 8)
                        pk.PID ^= 0x80000000;
                }
            }

            if (IsEgg)
            {
                // pk.IsEgg = true;
                pk.Egg_Day = Day;
                pk.Egg_Month = Month;
                pk.Egg_Year = Year - 2000;
                // Force hatch
                pk.IsEgg = false;
                pk.Met_Location = 4; // Nuvema Town
            }

            pk.RefreshChecksum();
            return pk;
        }
    }
}
