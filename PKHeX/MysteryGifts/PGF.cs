using System;
using System.Text;

namespace PKHeX
{
    public sealed class PGF : MysteryGift
    {
        internal const int Size = 0xCC;
        public override int Format => 5;

        public PGF(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[Size]);
            if (data == null) Data = new byte[Size];
            else Data = (byte[])data.Clone();
        }
        
        public ushort TID { get { return BitConverter.ToUInt16(Data, 0x00); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x00); } }
        public ushort SID { get { return BitConverter.ToUInt16(Data, 0x02); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x02); } }
        public int OriginGame { get { return Data[0x04]; } set { Data[0x04] = (byte)value; } }
        // Unused 0x05 0x06, 0x07
        public uint PID { get { return BitConverter.ToUInt32(Data, 0x08); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x08); } }

        private byte RIB0 { get { return Data[0x0C]; } set { Data[0x0C] = value; } }
        public bool RibbonCountry { get { return (RIB0 & (1 << 0)) == 1 << 0; } set { RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Country Ribbon
        public bool RibbonNational { get { return (RIB0 & (1 << 1)) == 1 << 1; } set { RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // National Ribbon
        public bool RibbonEarth { get { return (RIB0 & (1 << 2)) == 1 << 2; } set { RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Earth Ribbon
        public bool RibbonWorld { get { return (RIB0 & (1 << 3)) == 1 << 3; } set { RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // World Ribbon
        public bool RibbonClassic { get { return (RIB0 & (1 << 4)) == 1 << 4; } set { RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Classic Ribbon
        public bool RibbonPremier { get { return (RIB0 & (1 << 5)) == 1 << 5; } set { RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Premier Ribbon
        public bool RibbonEvent { get { return (RIB0 & (1 << 6)) == 1 << 6; } set { RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Event Ribbon
        public bool RibbonBirthday { get { return (RIB0 & (1 << 7)) == 1 << 7; } set { RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Birthday Ribbon
        private byte RIB1 { get { return Data[0x0D]; } set { Data[0x0D] = value; } }
        public bool RibbonSpecial { get { return (RIB1 & (1 << 0)) == 1 << 0; } set { RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Special Ribbon
        public bool RibbonSouvenir { get { return (RIB1 & (1 << 1)) == 1 << 1; } set { RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Souvenir Ribbon
        public bool RibbonWishing { get { return (RIB1 & (1 << 2)) == 1 << 2; } set { RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Wishing Ribbon
        public bool RibbonChampionBattle { get { return (RIB1 & (1 << 3)) == 1 << 3; } set { RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Battle Champ Ribbon
        public bool RibbonChampionRegional { get { return (RIB1 & (1 << 4)) == 1 << 4; } set { RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Regional Champ Ribbon
        public bool RibbonChampionNational { get { return (RIB1 & (1 << 5)) == 1 << 5; } set { RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // National Champ Ribbon
        public bool RibbonChampionWorld { get { return (RIB1 & (1 << 6)) == 1 << 6; } set { RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // World Champ Ribbon
        public bool RIB1_7 { get { return (RIB1 & (1 << 7)) == 1 << 7; } set { RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Empty

        public override int Ball { get { return Data[0x0E]; } set { Data[0x0E] = (byte)value; } }
        public override int HeldItem { get { return BitConverter.ToUInt16(Data, 0x10); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x10); } }
        public int Move1 { get { return BitConverter.ToUInt16(Data, 0x12); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x12); } }
        public int Move2 { get { return BitConverter.ToUInt16(Data, 0x14); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x14); } }
        public int Move3 { get { return BitConverter.ToUInt16(Data, 0x16); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x16); } }
        public int Move4 { get { return BitConverter.ToUInt16(Data, 0x18); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x18); } }
        public override int Species { get { return BitConverter.ToUInt16(Data, 0x1A); }
        }
        public int Form { get { return Data[0x1C]; } set { Data[0x1C] = (byte)value; } }
        public int Language { get { return Data[0x1D]; } set { Data[0x1D] = (byte)value; } }
        public string Nickname
        {
            get { return PKX.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x1E, 0x16)); }
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
            get { return PKX.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x4A, 0x10)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(0x08, (char)0xFFFF)).CopyTo(Data, 0x4A); } }
        public int OTGender { get { return Data[0x5A]; } set { Data[0x5A] = (byte)value; } }
        public override int Level { get { return Data[0x5B]; } set { Data[0x5C] = (byte)value; } }
        public override bool IsEgg { get { return Data[0x5C] == 1; } set { Data[0x5C] = (byte)(value ? 1 : 0); } }
        // Unused 0x5D 0x5E 0x5F
        public override string CardTitle
        {
            get { return PKX.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x60, 0x4A)); }
            set { Encoding.Unicode.GetBytes((value + '\uFFFF').PadRight(0x4A/2, '\0')).CopyTo(Data, 0x60); }
        }

        // Card Attributes
        public override int Item { get { return BitConverter.ToUInt16(Data, 0x00); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x00); } }
        
        private ushort Year { get { return BitConverter.ToUInt16(Data, 0xAE); } set { BitConverter.GetBytes(value).CopyTo(Data, 0xAE); } }
        private byte Month { get { return Data[0xAD]; } set { Data[0xAD] = value; } }
        private byte Day { get { return Data[0xAC]; } set { Data[0xAC] = value; } }

        /// <summary>
        /// Gets or sets the date of the card.
        /// </summary>
        public DateTime? Date
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(Year, Month, Day))
                    return null;

                return new DateTime(Year, Month, Day);
            }
            set
            {
                if (value.HasValue)
                {
                    // Only update the properties if a value is provided.
                    Year = (ushort)value.Value.Year;
                    Month = (byte)value.Value.Month;
                    Day = (byte)value.Value.Day;
                }
                else
                {
                    // Clear the Met Date.
                    // If code tries to access MetDate again, null will be returned.
                    Year = 0;
                    Month = 0;
                    Day = 0;
                }
            }
        }


        public override int CardID
        {
            get { return BitConverter.ToUInt16(Data, 0xB0); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xB0); }
        }
        public int CardLocation { get { return Data[0xB2]; } set { Data[0xB2] = (byte)value; } }
        public int CardType { get { return Data[0xB3]; } set { Data[0xB3] = (byte)value; } }
        public override bool GiftUsed { get { return Data[0xB4] >> 1 > 0; } set { Data[0xB4] = (byte)(Data[0xB4] & ~2 | (value ? 2 : 0)); } }
        public bool MultiObtain { get { return Data[0xB4] == 1; } set { Data[0xB4] = (byte)(value ? 1 : 0); } }
        
        // Meta Accessible Properties
        public int[] IVs => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
        public bool IsNicknamed => Nickname.Length > 0;

        public override int[] Moves
        {
            get { return new[] { Move1, Move2, Move3, Move4 }; }
        }
        public override bool IsPokémon { get { return CardType == 1; } set { if (value) CardType = 1; } }
        public override bool IsItem { get { return CardType == 2; } set { if (value) CardType = 2; } }
        public bool IsPower { get { return CardType == 3; } set { if (value) CardType = 3; } }

        public override PKM convertToPKM(SaveFile SAV)
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
                Gender = PersonalTable.B2W2[Species].Gender == 255 ? 2 : (Gender != 2 ? Gender : PersonalTable.B2W2[Species].RandomGender),
                AltForm = Form,
                Version = OriginGame == 0 ? new[] {20, 21, 22, 23}[Util.rnd32() & 0x3] : OriginGame,
                Language = Language == 0 ? SAV.Language : Language,
                Ball = Ball,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Met_Location = MetLocation,
                MetDate = Date,
                Egg_Location = EggLocation,
                CNT_Cool = CNT_Cool,
                CNT_Beauty = CNT_Beauty,
                CNT_Cute = CNT_Cute,
                CNT_Smart = CNT_Smart,
                CNT_Tough = CNT_Tough,
                CNT_Sheen = CNT_Sheen,

                EXP = PKX.getEXP(Level, Species),

                // Ribbons
                RibbonCountry = RibbonCountry,
                RibbonNational = RibbonNational,
                RibbonEarth = RibbonEarth,
                RibbonWorld = RibbonWorld,
                RibbonClassic = RibbonClassic,
                RibbonPremier = RibbonPremier,
                RibbonEvent = RibbonEvent,
                RibbonBirthday = RibbonBirthday,

                RibbonSpecial = RibbonSpecial,
                RibbonSouvenir = RibbonSouvenir,
                RibbonWishing = RibbonWishing,
                RibbonChampionBattle = RibbonChampionBattle,
                RibbonChampionRegional = RibbonChampionRegional,
                RibbonChampionNational = RibbonChampionNational,
                RibbonChampionWorld = RibbonChampionWorld,

                OT_Friendship = PersonalTable.B2W2[Species].BaseFriendship,
                FatefulEncounter = true,
            };
            pk.Move1_PP = pk.getMovePP(Move1, 0);
            pk.Move2_PP = pk.getMovePP(Move2, 0);
            pk.Move3_PP = pk.getMovePP(Move3, 0);
            pk.Move4_PP = pk.getMovePP(Move4, 0);
            if (OTGender == 3) // User's
            {
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
                pk.OT_Name = SAV.OT;
                pk.OT_Gender = 1; // Red PKHeX OT
            }
            else
            {
                pk.TID = IsEgg ? SAV.TID : TID;
                pk.SID = IsEgg ? SAV.SID : SID;
                pk.OT_Name = OT.Length > 0 ? OT : SAV.OT;
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
            pk.Ability = PersonalTable.B2W2.getAbilities(Species, pk.AltForm)[av];

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
                pk.EggMetDate = Date;
                // Force hatch
                pk.IsEgg = false;
                pk.Met_Location = 4; // Nuvema Town
            }

            pk.RefreshChecksum();
            return pk;
        }
    }
}
