using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Mystery Gift Template File
    /// </summary>
    public sealed class WC6 : DataMysteryGift, IRibbonSetEvent3, IRibbonSetEvent4, ILangNick, IContestStats, IContestStatsMutable, INature, IMemoryOT
    {
        public const int Size = 0x108;
        public const uint EonTicketConst = 0x225D73C2;
        public override int Generation => 6;

        public WC6() : this(new byte[Size]) { }
        public WC6(byte[] data) : base(data) { }

        public int RestrictLanguage { get; set; } // None
        public byte RestrictVersion { get; set; } // Permit All

        public bool CanBeReceivedByVersion(int v)
        {
            if (v < (int)GameVersion.X || v > (int)GameVersion.OR)
                return false;
            if (RestrictVersion == 0)
                return true; // no data
            var bitIndex = v - (int) GameVersion.X;
            var bit = 1 << bitIndex;
            return (RestrictVersion & bit) != 0;
        }

        // General Card Properties
        public override int CardID
        {
            get => BitConverter.ToUInt16(Data, 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0);
        }

        public override string CardTitle
        {
            // Max len 36 char, followed by null terminator
            get => StringConverter.GetString6(Data, 2, 0x4A);
            set => StringConverter.SetString6(value, 36, 37).CopyTo(Data, 2);
        }

        internal uint RawDate
        {
            get => BitConverter.ToUInt32(Data, 0x4C);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x4C);
        }

        private uint Year
        {
            get => RawDate / 10000;
            set => RawDate = SetDate(value, Month, Day);
        }

        private uint Month
        {
            get => RawDate % 10000 / 100;
            set => RawDate = SetDate(Year, value, Day);
        }

        private uint Day
        {
            get => RawDate % 100;
            set => RawDate = SetDate(Year, Month, value);
        }

        public static uint SetDate(uint year, uint month, uint day) => (year * 10000) + (month * 100) + day;

        /// <summary>
        /// Gets or sets the date of the card.
        /// </summary>
        public DateTime? Date
        {
            get
            {
                // Check to see if date is valid
                if (!DateUtil.IsDateValid(Year, Month, Day))
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
        public override bool GiftUsed { get => Data[0x52] >> 1 > 0; set => Data[0x52] = (byte)((Data[0x52] & ~2) | (value ? 2 : 0)); }
        public bool MultiObtain { get => Data[0x53] == 1; set => Data[0x53] = value ? (byte)1 : (byte)0; }

        // Item Properties
        public override bool IsItem { get => CardType == 1; set { if (value) CardType = 1; } }

        public override int ItemID {
            get => BitConverter.ToUInt16(Data, 0x68);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); }

        public override int Quantity {
            get => BitConverter.ToUInt16(Data, 0x70);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x70); }

        // Pokémon Properties
        public override bool IsPokémon { get => CardType == 0; set { if (value) CardType = 0; } }
        public override bool IsShiny => PIDType == Shiny.Always;
        public override int TID { get => BitConverter.ToUInt16(Data, 0x68); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); }
        public override int SID { get => BitConverter.ToUInt16(Data, 0x6A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x6A); }
        public int OriginGame { get => Data[0x6C]; set => Data[0x6C] = (byte)value; }
        public uint EncryptionConstant { get => BitConverter.ToUInt32(Data, 0x70); set => BitConverter.GetBytes(value).CopyTo(Data, 0x70); }
        public override int Ball { get => Data[0x76]; set => Data[0x76] = (byte)value; }
        public override int HeldItem { get => BitConverter.ToUInt16(Data, 0x78); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x78); }

        public int Move1 { get => BitConverter.ToUInt16(Data, 0x7A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7A); }
        public int Move2 { get => BitConverter.ToUInt16(Data, 0x7C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7C); }
        public int Move3 { get => BitConverter.ToUInt16(Data, 0x7E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7E); }
        public int Move4 { get => BitConverter.ToUInt16(Data, 0x80); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x80); }

        public override int Species { get => BitConverter.ToUInt16(Data, 0x82); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x82); }
        public override int Form { get => Data[0x84]; set => Data[0x84] = (byte)value; }
        public int Language { get => Data[0x85]; set => Data[0x85] = (byte)value; }

        public string Nickname
        {
            get => StringConverter.GetString6(Data, 0x86, 0x1A);
            set => StringConverter.SetString6(value, 12, 13).CopyTo(Data, 0x86);
        }

        public int Nature { get => (sbyte)Data[0xA0]; set => Data[0xA0] = (byte)value; }
        public override int Gender { get => Data[0xA1]; set => Data[0xA1] = (byte)value; }
        public override int AbilityType {  get => Data[0xA2]; set => Data[0xA2] = (byte)value; }
        public Shiny PIDType { get => (Shiny)Data[0xA3]; set => Data[0xA3] = (byte)value; }
        public override int EggLocation { get => BitConverter.ToUInt16(Data, 0xA4); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA4); }
        public int MetLocation { get => BitConverter.ToUInt16(Data, 0xA6); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA6); }

        public byte CNT_Cool   { get => Data[0xA9]; set => Data[0xA9] = value; }
        public byte CNT_Beauty { get => Data[0xAA]; set => Data[0xAA] = value; }
        public byte CNT_Cute   { get => Data[0xAB]; set => Data[0xAB] = value; }
        public byte CNT_Smart  { get => Data[0xAC]; set => Data[0xAC] = value; }
        public byte CNT_Tough  { get => Data[0xAD]; set => Data[0xAD] = value; }
        public byte CNT_Sheen  { get => Data[0xAE]; set => Data[0xAE] = value; }

        public int IV_HP { get => Data[0xAF]; set => Data[0xAF] = (byte)value; }
        public int IV_ATK { get => Data[0xB0]; set => Data[0xB0] = (byte)value; }
        public int IV_DEF { get => Data[0xB1]; set => Data[0xB1] = (byte)value; }
        public int IV_SPE { get => Data[0xB2]; set => Data[0xB2] = (byte)value; }
        public int IV_SPA { get => Data[0xB3]; set => Data[0xB3] = (byte)value; }
        public int IV_SPD { get => Data[0xB4]; set => Data[0xB4] = (byte)value; }

        public int OTGender { get => Data[0xB5]; set => Data[0xB5] = (byte)value; }

        public override string OT_Name
        {
            get => StringConverter.GetString6(Data, 0xB6, 0x1A);
            set => StringConverter.SetString6(value, 12, 13).CopyTo(Data, 0xB6);
        }

        public override int Level { get => Data[0xD0]; set => Data[0xD0] = (byte)value; }
        public override bool IsEgg { get => Data[0xD1] == 1; set => Data[0xD1] = value ? (byte)1 : (byte)0; }
        public uint PID { get => BitConverter.ToUInt32(Data, 0xD4); set => BitConverter.GetBytes(value).CopyTo(Data, 0xD4); }

        public int RelearnMove1 { get => BitConverter.ToUInt16(Data, 0xD8); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xD8); }
        public int RelearnMove2 { get => BitConverter.ToUInt16(Data, 0xDA); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDA); }
        public int RelearnMove3 { get => BitConverter.ToUInt16(Data, 0xDC); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDC); }
        public int RelearnMove4 { get => BitConverter.ToUInt16(Data, 0xDE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDE); }

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
        private byte RIB1 { get => Data[0x75]; set => Data[0x75] = value; }
        public bool RibbonChampionBattle   { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonChampionRegional { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonChampionNational { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonCountry          { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonNational         { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonEarth            { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonWorld            { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonEvent            { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonChampionWorld    { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonBirthday         { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonSpecial          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonSouvenir         { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonWishing          { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonClassic          { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonPremier          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RIB1_7                 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

        // Meta Accessible Properties
        public override int[] IVs
        {
            get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
            set
            {
                if (value.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }

        public int[] EVs
        {
            get => new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD };
            set
            {
                if (value.Length != 6) return;
                EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
                EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
            }
        }

        public bool IsNicknamed => Nickname.Length > 0;
        public override int Location { get => MetLocation; set => MetLocation = (ushort)value; }

        public override IReadOnlyList<int> Moves
        {
            get => new[] { Move1, Move2, Move3, Move4 };
            set
            {
                if (value.Count > 0) Move1 = value[0];
                if (value.Count > 1) Move2 = value[1];
                if (value.Count > 2) Move3 = value[2];
                if (value.Count > 3) Move4 = value[3];
            }
        }

        public override IReadOnlyList<int> Relearn
        {
            get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
            set
            {
                if (value.Count > 0) RelearnMove1 = value[0];
                if (value.Count > 1) RelearnMove2 = value[1];
                if (value.Count > 2) RelearnMove3 = value[2];
                if (value.Count > 3) RelearnMove4 = value[3];
            }
        }

        public override PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria)
        {
            if (!IsPokémon)
                throw new ArgumentException(nameof(IsPokémon));

            var rnd = Util.Rand;

            int currentLevel = Level > 0 ? Level : rnd.Next(1, 101);
            var pi = PersonalTable.AO.GetFormEntry(Species, Form);
            PK6 pk = new()
            {
                Species = Species,
                HeldItem = HeldItem,
                TID = TID,
                SID = SID,
                Met_Level = currentLevel,
                Form = Form,
                EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : Util.Rand32(),
                Version = OriginGame != 0 ? OriginGame : sav.Game,
                Language = Language != 0 ? Language : sav.Language,
                Ball = Ball,
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

                OT_Name = OT_Name.Length > 0 ? OT_Name : sav.OT,
                OT_Gender = OTGender != 3 ? OTGender % 2 : sav.Gender,
                HT_Name = OT_Name.Length > 0 ? sav.OT : string.Empty,
                HT_Gender = OT_Name.Length > 0 ? sav.Gender : 0,
                CurrentHandler = OT_Name.Length > 0 ? 1 : 0,

                EXP = Experience.GetEXP(Level, pi.EXPGrowth),

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
                FatefulEncounter = MetLocation != 30011, // Link gifts do not set fateful encounter

                EVs = EVs,
            };

            if (sav is IRegionOrigin o)
            {
                pk.Country = o.Country;
                pk.Region = o.Region;
                pk.ConsoleRegion = o.ConsoleRegion;
            }
            else
            {
                pk.SetDefaultRegionOrigins();
            }

            pk.SetMaximumPPCurrent();

            pk.MetDate = Date ?? DateTime.Now;

            if ((sav.Generation > Generation && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
            {
                // give random valid game
                do { pk.Version = (int)GameVersion.X + rnd.Next(4); }
                while (!CanBeReceivedByVersion(pk.Version));
            }

            if (!IsEgg)
            {
                if (pk.CurrentHandler == 0) // OT
                {
                    pk.OT_Memory = 3;
                    pk.OT_TextVar = 9;
                    pk.OT_Intensity = 1;
                    pk.OT_Feeling = Memories.GetRandomFeeling(pk.OT_Memory, 10); // 0-9
                }
                else
                {
                    pk.HT_Memory = 3;
                    pk.HT_TextVar = 9;
                    pk.HT_Intensity = 1;
                    pk.HT_Feeling = Memories.GetRandomFeeling(pk.HT_Memory, 10); // 0-9
                    pk.HT_Friendship = pk.OT_Friendship;
                }
            }

            pk.IsNicknamed = IsNicknamed;
            pk.Nickname = IsNicknamed ? Nickname : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);

            SetPINGA(pk, criteria);

            if (IsEgg)
                SetEggMetData(pk);
            pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

            pk.RefreshChecksum();
            return pk;
        }

        private void SetEggMetData(PKM pk)
        {
            pk.IsEgg = true;
            pk.EggMetDate = Date;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(0, pk.Language, Generation);
            pk.IsNicknamed = true;
        }

        private void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = PersonalTable.AO.GetFormEntry(Species, Form);
            pk.Nature = (int)criteria.GetNature((Nature)Nature);
            pk.Gender = criteria.GetGender(Gender, pi);
            var av = GetAbilityIndex(criteria);
            pk.RefreshAbility(av);
            SetPID(pk);
            SetIVs(pk);
        }

        private int GetAbilityIndex(EncounterCriteria criteria) => AbilityType switch
        {
            00 or 01 or 02 => AbilityType, // Fixed 0/1/2
            03 or 04 => criteria.GetAbilityFromType(AbilityType), // 0/1 or 0/1/H
            _ => throw new ArgumentException(nameof(AbilityType)),
        };

        private void SetPID(PKM pk)
        {
            switch (PIDType)
            {
                case Shiny.FixedValue: // Specified
                    pk.PID = PID;
                    break;
                case Shiny.Random: // Random
                    pk.PID = Util.Rand32();
                    break;
                case Shiny.Always: // Random Shiny
                    pk.PID = Util.Rand32();
                    pk.PID = (uint)(((pk.TID ^ pk.SID ^ (pk.PID & 0xFFFF)) << 16) | (pk.PID & 0xFFFF));
                    break;
                case Shiny.Never: // Random Nonshiny
                    pk.PID = Util.Rand32();
                    if (pk.IsShiny) pk.PID ^= 0x10000000;
                    break;
            }
        }

        private void SetIVs(PKM pk)
        {
            int[] finalIVs = new int[6];
            var ivflag = Array.Find(IVs, iv => (byte)(iv - 0xFC) < 3);
            var rnd = Util.Rand;
            if (ivflag == 0) // Random IVs
            {
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = IVs[i] > 31 ? rnd.Next(32) : IVs[i];
            }
            else // 1/2/3 perfect IVs
            {
                int IVCount = ivflag - 0xFB;
                do { finalIVs[rnd.Next(6)] = 31; }
                while (finalIVs.Count(iv => iv == 31) < IVCount);
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = finalIVs[i] == 31 ? 31 : rnd.Next(32);
            }
            pk.IVs = finalIVs;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (pkm.Egg_Location == 0) // Not Egg
            {
                if (OTGender != 3)
                {
                    // Skip ID check if ORASDEMO Simulated wc6
                    if (CardID != 0)
                    {
                        if (SID != pkm.SID) return false;
                        if (TID != pkm.TID) return false;
                    }
                    if (OTGender != pkm.OT_Gender) return false;
                }
                if (!string.IsNullOrEmpty(OT_Name) && OT_Name != pkm.OT_Name) return false;
                if (PIDType == Shiny.FixedValue && pkm.PID != PID) return false;
                if (!PIDType.IsValid(pkm)) return false;
                if (OriginGame != 0 && OriginGame != pkm.Version) return false;
                if (EncryptionConstant != 0 && EncryptionConstant != pkm.EncryptionConstant) return false;
                if (Language != 0 && Language != pkm.Language) return false;
            }
            if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pkm.Form, pkm.Format))
                return false;

            if (IsEgg)
            {
                if (EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != Locations.LinkTrade6)
                        return false;
                }
                else if (PIDType == 0 && pkm.IsShiny)
                {
                    return false; // can't be traded away for unshiny
                }

                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }
            else
            {
                if (EggLocation != pkm.Egg_Location) return false;
                if (MetLocation != pkm.Met_Location) return false;
            }

            if (Level != pkm.Met_Level) return false;
            if (Ball != pkm.Ball) return false;
            if (OTGender < 3 && OTGender != pkm.OT_Gender) return false;
            if (Nature != -1 && pkm.Nature != Nature) return false;
            if (Gender != 3 && Gender != pkm.Gender) return false;

            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;

            return true;
        }

        protected override bool IsMatchDeferred(PKM pkm)
        {
            switch (CardID)
            {
                case 0525 when IV_HP == 0xFE: // Diancie was distributed with no IV enforcement & 3IVs
                case 0504 when RibbonClassic != ((IRibbonSetEvent4)pkm).RibbonClassic: // Magmar with/without classic
                    return true;
            }
            return Species != pkm.Species;
        }

        protected override bool IsMatchPartial(PKM pkm)
        {
            if (RestrictLanguage != 0 && RestrictLanguage != pkm.Language)
                return true;
            if (!CanBeReceivedByVersion(pkm.Version))
                return true;
            return false;
        }
    }
}
