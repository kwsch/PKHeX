using System;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    public sealed class WC6 : MysteryGift, IRibbonSet1, IRibbonSet2
    {
        public const int Size = 0x108;
        public const int SizeFull = 0x310;
        public const uint EonTicketConst = 0x225D73C2;
        public override int Format => 6;

        public WC6(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[Size]);
            if (Data.Length == SizeFull)
            {
                if (Data[0x205] == 0)
                    Data = new byte[Data.Length]; // Invalidate
                Data = Data.Skip(SizeFull - Size).ToArray();
                DateTime now = DateTime.Now;
                Year = (uint)now.Year;
                Month = (uint)now.Month;
                Day = (uint)now.Day;
            }
            if (Year < 2000)
                Data = new byte[Data.Length]; // Invalidate
        }
        
        // General Card Properties
        public override int CardID {
            get => BitConverter.ToUInt16(Data, 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0); }
        public override string CardTitle { // Max len 36 char, followed by null terminator
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, 2, 72));
            set => Encoding.Unicode.GetBytes(value.PadRight(36, '\0')).CopyTo(Data, 2); }
        private uint RawDate {
            get => BitConverter.ToUInt32(Data, 0x4C);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x4C); }
        private uint Year {
            get => RawDate / 10000;
            set => RawDate = value * 10000 + RawDate % 10000; }
        private uint Month {
            get => RawDate % 10000 / 100;
            set => RawDate = Year * 10000 + value * 100 + RawDate % 100; }
        private uint Day {
            get => RawDate % 100;
            set => RawDate = Year * 10000 + Month * 100 + value; }

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

                return new DateTime((int)Year, (int)Month, (int)Day);
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

        public int CardLocation { get => Data[0x50]; set => Data[0x50] = (byte)value; }

        public int CardType { get => Data[0x51]; set => Data[0x51] = (byte)value; }
        public override bool GiftUsed { get => Data[0x52] >> 1 > 0; set => Data[0x52] = (byte)(Data[0x52] & ~2 | (value ? 2 : 0)); }
        public bool MultiObtain { get => Data[0x53] == 1; set => Data[0x53] = (byte)(value ? 1 : 0); }

        // Item Properties
        public override bool IsItem { get => CardType == 1; set { if (value) CardType = 1; } }
        public override int Item {
            get => BitConverter.ToUInt16(Data, 0x68);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); }
        public override int Quantity {
            get => BitConverter.ToUInt16(Data, 0x70);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x70); }
        
        // Pokémon Properties
        public override bool IsPokémon { get => CardType == 0; set { if (value) CardType = 0; } }
        public override bool IsShiny => PIDType == 2;
        public int TID {
            get => BitConverter.ToUInt16(Data, 0x68);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); }
        public int SID {
            get => BitConverter.ToUInt16(Data, 0x6A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x6A); }
        public int OriginGame {
            get => Data[0x6C];
            set => Data[0x6C] = (byte)value; }
        public uint EncryptionConstant {
            get => BitConverter.ToUInt32(Data, 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x70); }
        public override int Ball {
            get => Data[0x76];
            set => Data[0x76] = (byte)value; }
        public override int HeldItem {
            get => BitConverter.ToUInt16(Data, 0x78);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x78); }
        public int Move1 {
            get => BitConverter.ToUInt16(Data, 0x7A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7A); }
        public int Move2 {
            get => BitConverter.ToUInt16(Data, 0x7C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7C); }
        public int Move3 {
            get => BitConverter.ToUInt16(Data, 0x7E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7E); }
        public int Move4 {
            get => BitConverter.ToUInt16(Data, 0x80);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x80); }
        public override int Species {
            get => BitConverter.ToUInt16(Data, 0x82);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x82); }
        public int Form {
            get => Data[0x84];
            set => Data[0x84] = (byte)value; }
        public int Language {
            get => Data[0x85];
            set => Data[0x85] = (byte)value; }
        public string Nickname {
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x86, 0x1A));
            set => Encoding.Unicode.GetBytes(value.PadRight(12 + 1, '\0')).CopyTo(Data, 0x86); }
        public int Nature {
            get => Data[0xA0];
            set => Data[0xA0] = (byte)value; }
        public int Gender {
            get => Data[0xA1];
            set => Data[0xA1] = (byte)value; }
        public int AbilityType {
            get => Data[0xA2];
            set => Data[0xA2] = (byte)value; }
        public int PIDType {
            get => Data[0xA3];
            set => Data[0xA3] = (byte)value; }
        public int EggLocation {
            get => BitConverter.ToUInt16(Data, 0xA4);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA4); }
        public int MetLocation  {
            get => BitConverter.ToUInt16(Data, 0xA6);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA6); }

        public int CNT_Cool { get => Data[0xA9]; set => Data[0xA9] = (byte)value; }
        public int CNT_Beauty { get => Data[0xAA]; set => Data[0xAA] = (byte)value; }
        public int CNT_Cute { get => Data[0xAB]; set => Data[0xAB] = (byte)value; }
        public int CNT_Smart { get => Data[0xAC]; set => Data[0xAC] = (byte)value; }
        public int CNT_Tough { get => Data[0xAD]; set => Data[0xAD] = (byte)value; }
        public int CNT_Sheen { get => Data[0xAE]; set => Data[0xAE] = (byte)value; }

        public int IV_HP { get => Data[0xAF]; set => Data[0xAF] = (byte)value; }
        public int IV_ATK { get => Data[0xB0]; set => Data[0xB0] = (byte)value; }
        public int IV_DEF { get => Data[0xB1]; set => Data[0xB1] = (byte)value; }
        public int IV_SPE { get => Data[0xB2]; set => Data[0xB2] = (byte)value; }
        public int IV_SPA { get => Data[0xB3]; set => Data[0xB3] = (byte)value; }
        public int IV_SPD { get => Data[0xB4]; set => Data[0xB4] = (byte)value; }

        public int OTGender { get => Data[0xB5]; set => Data[0xB5] = (byte)value; }
        public string OT {
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0xB6, 0x1A)); set => Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, 0xB6);
        }
        public override int Level { get => Data[0xD0]; set => Data[0xD0] = (byte)value; }
        public override bool IsEgg { get => Data[0xD1] == 1; set => Data[0xD1] = (byte)(value ? 1 : 0); }
        public uint PID {
            get => BitConverter.ToUInt32(Data, 0xD4); set => BitConverter.GetBytes(value).CopyTo(Data, 0xD4);
        }
        public int RelearnMove1 {
            get => BitConverter.ToUInt16(Data, 0xD8); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xD8);
        }
        public int RelearnMove2 {
            get => BitConverter.ToUInt16(Data, 0xDA); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDA);
        }
        public int RelearnMove3 {
            get => BitConverter.ToUInt16(Data, 0xDC); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDC);
        }
        public int RelearnMove4 {
            get => BitConverter.ToUInt16(Data, 0xDE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDE);
        }
        public int OT_Intensity { get => Data[0xE0]; set => Data[0xE0] = (byte)value; }
        public int OT_Memory { get => Data[0xE1]; set => Data[0xE1] = (byte)value; }
        public int OT_TextVar { get => BitConverter.ToUInt16(Data, 0xE2); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xE2); }
        public int OT_Feeling { get => Data[0xE4]; set => Data[0xE4] = (byte)value; }

        public int EV_HP { get => Data[0xE5]; set => Data[0xE5] = (byte)value; }
        public int EV_ATK { get => Data[0xE6]; set => Data[0xE6] = (byte)value; }
        public int EV_DEF { get => Data[0xE7]; set => Data[0xE7] = (byte)value; }
        public int EV_SPE { get => Data[0xE8]; set => Data[0xE8] = (byte)value; }
        public int EV_SPA { get => Data[0xE9]; set => Data[0xE9] = (byte)value; }
        public int EV_SPD { get => Data[0xEA]; set => Data[0xEA] = (byte)value; }

        private byte RIB0 { get => Data[0x74]; set => Data[0x74] = value; }
        public bool RibbonChampionBattle { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); } // Battle Champ Ribbon
        public bool RibbonChampionRegional { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); } // Regional Champ Ribbon
        public bool RibbonChampionNational { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); } // National Champ Ribbon
        public bool RibbonCountry { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); } // Country Ribbon
        public bool RibbonNational { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); } // National Ribbon
        public bool RibbonEarth { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); } // Earth Ribbon
        public bool RibbonWorld { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); } // World Ribbon
        public bool RibbonEvent { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); } // Event Ribbon
        private byte RIB1 { get => Data[0x75]; set => Data[0x75] = value; }
        public bool RibbonChampionWorld { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } // World Champ Ribbon
        public bool RibbonBirthday { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } // Birthday Ribbon
        public bool RibbonSpecial { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } // Special Ribbon
        public bool RibbonSouvenir { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } // Souvenir Ribbon
        public bool RibbonWishing { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } // Wishing Ribbon
        public bool RibbonClassic { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } // Classic Ribbon
        public bool RibbonPremier { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } // Premier Ribbon
        public bool RIB1_7 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } // Empty

        // Meta Accessible Properties
        public int[] IVs
        {
            get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
            set
            {
                if (value?.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }
        public int[] EVs
        {
            get => new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD };
            set
            {
                if (value?.Length != 6) return;
                EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
                EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
            }
        }
        public bool IsNicknamed => Nickname.Length > 0;

        public override int[] Moves
        {
            get => new[] { Move1, Move2, Move3, Move4 };
            set
            {
                if (value.Length > 0) Move1 = value[0];
                if (value.Length > 1) Move2 = value[1];
                if (value.Length > 2) Move3 = value[2];
                if (value.Length > 3) Move4 = value[3];
            }
        }
        public override int[] RelearnMoves
        {
            get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
            set
            {
                if (value.Length > 0) RelearnMove1 = value[0];
                if (value.Length > 1) RelearnMove2 = value[1];
                if (value.Length > 2) RelearnMove3 = value[2];
                if (value.Length > 3) RelearnMove4 = value[3];
            }
        }

        public override PKM convertToPKM(SaveFile SAV)
        {
            if (!IsPokémon)
                return null;

            int currentLevel = Level > 0 ? Level : (int)(Util.rnd32()%100 + 1);
            var pi = PersonalTable.AO[Species];
            PK6 pk = new PK6
            {
                Species = Species,
                HeldItem = HeldItem,
                TID = TID,
                SID = SID,
                Met_Level = currentLevel,
                Nature = Nature != 0xFF ? Nature : (int)(Util.rnd32() % 25),
                Gender = Gender != 3 ? Gender : pi.RandomGender,
                AltForm = Form,
                EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : Util.rnd32(),
                Version = OriginGame != 0 ? OriginGame : SAV.Game,
                Language = Language != 0 ? Language : SAV.Language,
                Ball = Ball,
                Country = SAV.Country,
                Region = SAV.SubRegion,
                ConsoleRegion = SAV.ConsoleRegion,
                Move1 = Move1, Move2 = Move2, Move3 = Move3, Move4 = Move4,
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
                
                OT_Friendship = pi.BaseFriendship,
                OT_Intensity = OT_Intensity,
                OT_Memory = OT_Memory,
                OT_TextVar = OT_TextVar,
                OT_Feeling = OT_Feeling,
                FatefulEncounter = true,

                EVs = EVs,
            };
            pk.Move1_PP = pk.getMovePP(Move1, 0);
            pk.Move2_PP = pk.getMovePP(Move2, 0);
            pk.Move3_PP = pk.getMovePP(Move3, 0);
            pk.Move4_PP = pk.getMovePP(Move4, 0);

            pk.MetDate = Date ?? DateTime.Now;

            if (SAV.Generation > 6 && OriginGame == 0) // Gen7
            {
                pk.Version = (int)GameVersion.OR;
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
                    do { // 3 Perfect IVs
                    for (int i = 0; i < 6; i++)
                        finalIVs[i] = IVs[i] > 31 ? (int)(Util.rnd32() & 0x1F) : IVs[i];
                    } while (finalIVs.Count(r => r == 31) < 3); // 3*31
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
            pk.Ability = PersonalTable.AO.getAbilities(Species, pk.AltForm)[av];
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
                    pk.PID = Util.rnd32();
                    if ((uint)(((TID ^ SID ^ (pk.PID & 0xFFFF)) << 16) + (pk.PID & 0xFFFF)) < 16) pk.PID ^= 0x10000000;
                    break;
            }

            if (IsEgg)
            {
                pk.IsEgg = true;
                pk.EggMetDate = Date;
            }

            pk.RefreshChecksum();
            return pk;
        }
    }
}
