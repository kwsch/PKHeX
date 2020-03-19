using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Mystery Gift Template File
    /// </summary>
    public sealed class WB7 : DataMysteryGift, ILangNick, IAwakened, INature
    {
        public const int Size = 0x108;
        public const int SizeFull = 0x310;
        private const int CardStart = SizeFull - Size;

        public override int Format => 7;

        public WB7() : this(new byte[SizeFull]) { }
        public WB7(byte[] data) : base(data) { }

        public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }

        public bool CanBeReceivedByVersion(int v)
        {
            if (v < (int)GameVersion.GP || v > (int)GameVersion.GE)
                return false;
            if (RestrictVersion == 0)
                return true; // no data
            var bitIndex = v - (int)GameVersion.GP;
            var bit = 1 << bitIndex;
            return (RestrictVersion & bit) != 0;
        }

        // General Card Properties
        public override int CardID
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0);
        }

        public override string CardTitle
        {
            // Max len 36 char, followed by null terminator
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 2, 72));
            set => Encoding.Unicode.GetBytes(value.PadRight(36, '\0')).CopyTo(Data, CardStart + 2);
        }

        private uint RawDate
        {
            get => BitConverter.ToUInt32(Data, CardStart + 0x4C);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x4C);
        }

        private uint Year
        {
            get => (RawDate / 10000) + 2000;
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

        private static uint SetDate(uint year, uint month, uint day) => (Math.Max(0, year - 2000) * 10000) + (month * 100) + day;

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

        public int CardLocation { get => Data[CardStart + 0x50]; set => Data[CardStart + 0x50] = (byte)value; }

        public int CardType { get => Data[CardStart + 0x51]; set => Data[CardStart + 0x51] = (byte)value; }
        public byte CardFlags { get => Data[CardStart + 0x52]; set => Data[CardStart + 0x52] = value; }

        public bool GiftRepeatable { get => (CardFlags & 1) == 0; set => CardFlags = (byte)((CardFlags & ~1) | (value ? 0 : 1)); }
        public override bool GiftUsed { get => (CardFlags & 2) == 2; set => CardFlags = (byte)((CardFlags & ~2) | (value ? 2 : 0)); }
        public bool GiftOncePerDay { get => (CardFlags & 4) == 4; set => CardFlags = (byte)((CardFlags & ~4) | (value ? 4 : 0)); }

        public bool MultiObtain { get => Data[CardStart + 0x53] == 1; set => Data[CardStart + 0x53] = (byte)(value ? 1 : 0); }

        // Item Properties
        public override bool IsItem { get => CardType == 1; set { if (value) CardType = 1; } }
        public override int ItemID { get => BitConverter.ToUInt16(Data, CardStart + 0x68); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x68); }
        public int GetItem(int index) => BitConverter.ToUInt16(Data, CardStart + 0x68 + (0x4 * index));
        public void SetItem(int index, ushort item) => BitConverter.GetBytes(item).CopyTo(Data, CardStart + 0x68 + (4 * index));
        public int GetQuantity(int index) => BitConverter.ToUInt16(Data, CardStart + 0x6A + (0x4 * index));
        public void SetQuantity(int index, ushort quantity) => BitConverter.GetBytes(quantity).CopyTo(Data, CardStart + 0x6A + (4 * index));

        public override int Quantity
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x6A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x6A);
        }

        // Pokémon Properties
        public override bool IsPokémon { get => CardType == 0; set { if (value) CardType = 0; } }
        public override bool IsShiny => PIDType == Shiny.Always;

        public override int TID
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x68);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x68);
        }

        public override int SID {
            get => BitConverter.ToUInt16(Data, CardStart + 0x6A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x6A);
        }

        public int OriginGame
        {
            get => BitConverter.ToInt32(Data, CardStart + 0x6C);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x6C);
        }

        public uint EncryptionConstant {
            get => BitConverter.ToUInt32(Data, CardStart + 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x70);
        }

        public override int Ball
        {
            get => Data[CardStart + 0x76];
            set => Data[CardStart + 0x76] = (byte)value; }

        public override int HeldItem // no references
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x78);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x78);
        }

        public int Move1 { get => BitConverter.ToUInt16(Data, CardStart + 0x7A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x7A); }
        public int Move2 { get => BitConverter.ToUInt16(Data, CardStart + 0x7C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x7C); }
        public int Move3 { get => BitConverter.ToUInt16(Data, CardStart + 0x7E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x7E); }
        public int Move4 { get => BitConverter.ToUInt16(Data, CardStart + 0x80); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x80); }
        public override int Species { get => BitConverter.ToUInt16(Data, CardStart + 0x82); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x82); }
        public override int Form { get => Data[CardStart + 0x84]; set => Data[CardStart + 0x84] = (byte)value; }

        // public int Language { get => Data[CardStart + 0x85]; set => Data[CardStart + 0x85] = (byte)value; }

        // public string Nickname
        // {
        //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 0x86, 0x1A));
        //     set => Encoding.Unicode.GetBytes(value.PadRight(12 + 1, '\0')).CopyTo(Data, CardStart + 0x86);
        // }

        public int Nature { get => (sbyte)Data[CardStart + 0xA0]; set => Data[CardStart + 0xA0] = (byte)value; }
        public override int Gender { get => Data[CardStart + 0xA1]; set => Data[CardStart + 0xA1] = (byte)value; }
        public override int AbilityType { get => 3; set => Data[CardStart + 0xA2] = (byte)value; } // no references, always ability 0/1
        public Shiny PIDType { get => (Shiny)Data[CardStart + 0xA3]; set => Data[CardStart + 0xA3] = (byte)value; }
        public override int EggLocation { get => BitConverter.ToUInt16(Data, CardStart + 0xA4); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xA4); }
        public int MetLocation  { get => BitConverter.ToUInt16(Data, CardStart + 0xA6); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xA6); }
        public int MetLevel { get => Data[CardStart + 0xA8]; set => Data[CardStart + 0xA8] = (byte)value; }

        public int IV_HP { get => Data[CardStart + 0xAF]; set => Data[CardStart + 0xAF] = (byte)value; }
        public int IV_ATK { get => Data[CardStart + 0xB0]; set => Data[CardStart + 0xB0] = (byte)value; }
        public int IV_DEF { get => Data[CardStart + 0xB1]; set => Data[CardStart + 0xB1] = (byte)value; }
        public int IV_SPE { get => Data[CardStart + 0xB2]; set => Data[CardStart + 0xB2] = (byte)value; }
        public int IV_SPA { get => Data[CardStart + 0xB3]; set => Data[CardStart + 0xB3] = (byte)value; }
        public int IV_SPD { get => Data[CardStart + 0xB4]; set => Data[CardStart + 0xB4] = (byte)value; }

        public int OTGender { get => Data[CardStart + 0xB5]; set => Data[CardStart + 0xB5] = (byte)value; }

        // public override string OT_Name
        // {
        //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 0xB6, 0x1A));
        //     set => Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, CardStart + 0xB6);
        // }

        public override int Level { get => Data[CardStart + 0xD0]; set => Data[CardStart + 0xD0] = (byte)value; }
        public override bool IsEgg { get => Data[CardStart + 0xD1] == 1; set => Data[CardStart + 0xD1] = (byte)(value ? 1 : 0); }
        public ushort AdditionalItem { get => BitConverter.ToUInt16(Data, CardStart + 0xD2); set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0xD2); }

        public uint PID { get => BitConverter.ToUInt32(Data, 0xD4); set => BitConverter.GetBytes(value).CopyTo(Data, 0xD4); }
        public int RelearnMove1 { get => BitConverter.ToUInt16(Data, CardStart + 0xD8); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xD8); }
        public int RelearnMove2 { get => BitConverter.ToUInt16(Data, CardStart + 0xDA); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xDA); }
        public int RelearnMove3 { get => BitConverter.ToUInt16(Data, CardStart + 0xDC); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xDC); }
        public int RelearnMove4 { get => BitConverter.ToUInt16(Data, CardStart + 0xDE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xDE); }

        public int AV_HP {  get => Data[CardStart + 0xE6]; set => Data[CardStart + 0xE6] = (byte)value; }
        public int AV_ATK { get => Data[CardStart + 0xE7]; set => Data[CardStart + 0xE7] = (byte)value; }
        public int AV_DEF { get => Data[CardStart + 0xE8]; set => Data[CardStart + 0xE8] = (byte)value; }
        public int AV_SPE { get => Data[CardStart + 0xE9]; set => Data[CardStart + 0xE9] = (byte)value; }
        public int AV_SPA { get => Data[CardStart + 0xEA]; set => Data[CardStart + 0xEA] = (byte)value; }
        public int AV_SPD { get => Data[CardStart + 0xEB]; set => Data[CardStart + 0xEB] = (byte)value; }

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

        public bool GetIsNicknamed(int language) => Data[GetNicknameOffset(language)] != 0;

        private static int GetLanguageIndex(int language)
        {
            var lang = (LanguageID) language;
            if (lang < LanguageID.Japanese || lang == LanguageID.UNUSED_6 || lang > LanguageID.ChineseT)
                return (int) LanguageID.English; // fallback
            return lang < LanguageID.UNUSED_6 ? language - 1 : language - 2;
        }

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

        public override string OT_Name { get; set; } = string.Empty;
        public string Nickname => string.Empty;
        public bool IsNicknamed => false;
        public int Language => 2;

        public string GetNickname(int language) => Util.TrimFromZero(Encoding.Unicode.GetString(Data, GetNicknameOffset(language), 0x1A));
        public void SetNickname(int language, string value) => Encoding.Unicode.GetBytes(value.PadRight(0x1A / 2, '\0')).CopyTo(Data, GetNicknameOffset(language));

        public string GetOT(int language) => Util.TrimFromZero(Encoding.Unicode.GetString(Data, GetOTOffset(language), 0x1A));
        public void SetOT(int language, string value) => Encoding.Unicode.GetBytes(value.PadRight(0x1A / 2, '\0')).CopyTo(Data, GetOTOffset(language));

        private int GetNicknameOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0x04 + (index * 0x1A);
        }

        private int GetOTOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0xEE + (index * 0x1A);
        }

        public override PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            if (!IsPokémon)
                throw new ArgumentException(nameof(IsPokémon));

            var rnd = Util.Rand;

            int currentLevel = Level > 0 ? Level : rnd.Next(1, 101);
            int metLevel = MetLevel > 0 ? MetLevel : currentLevel;
            var pi = PersonalTable.GG.GetFormeEntry(Species, Form);
            var OT = GetOT(SAV.Language);

            var pk = new PB7
            {
                Species = Species,
                HeldItem = HeldItem,
                TID = TID,
                SID = SID,
                Met_Level = metLevel,
                AltForm = Form,
                EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : Util.Rand32(),
                Version = OriginGame != 0 ? OriginGame : SAV.Game,
                Language = SAV.Language,
                Ball = Ball,
                Country = SAV.Country,
                Region = SAV.SubRegion,
                ConsoleRegion = SAV.ConsoleRegion,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                RelearnMove1 = RelearnMove1,
                RelearnMove2 = RelearnMove2,
                RelearnMove3 = RelearnMove3,
                RelearnMove4 = RelearnMove4,
                Met_Location = MetLocation,
                Egg_Location = EggLocation,
                AV_HP = AV_HP,
                AV_ATK = AV_ATK,
                AV_DEF = AV_DEF,
                AV_SPE = AV_SPE,
                AV_SPA = AV_SPA,
                AV_SPD = AV_SPD,

                OT_Name = OT.Length > 0 ? OT : SAV.OT,
                OT_Gender = OTGender != 3 ? OTGender % 2 : SAV.Gender,
                HT_Name = OT_Name.Length > 0 ? SAV.OT : string.Empty,
                HT_Gender = OT_Name.Length > 0 ? SAV.Gender : 0,
                CurrentHandler = OT_Name.Length > 0 ? 1 : 0,

                EXP = Experience.GetEXP(currentLevel, pi.EXPGrowth),

                OT_Friendship = pi.BaseFriendship,
                FatefulEncounter = true,
            };
            pk.SetMaximumPPCurrent();

            if ((SAV.Generation > Format && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
            {
                // give random valid game
                do { pk.Version = (int)GameVersion.GP + rnd.Next(2); }
                while (!CanBeReceivedByVersion(pk.Version));
            }

            if (OTGender == 3)
            {
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
            }

            pk.MetDate = Date ?? DateTime.Now;
            pk.IsNicknamed = GetIsNicknamed(pk.Language);
            pk.Nickname = pk.IsNicknamed ? GetNickname(pk.Language) : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Format);

            SetPINGA(pk, criteria);

            if (IsEgg)
                SetEggMetData(pk);
            pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

            pk.HeightScalar = rnd.Next(0x100);
            pk.WeightScalar = rnd.Next(0x100);
            pk.ResetCalculatedValues(); // cp & dimensions

            pk.RefreshChecksum();
            return pk;
        }

        private void SetEggMetData(PKM pk)
        {
            pk.IsEgg = true;
            pk.EggMetDate = Date;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(0, pk.Language, Format);
            pk.IsNicknamed = true;
        }

        private void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = PersonalTable.GG.GetFormeEntry(Species, Form);
            pk.Nature = (int)criteria.GetNature((Nature)Nature);
            pk.Gender = criteria.GetGender(Gender, pi);
            var av = GetAbilityIndex(criteria, pi);
            pk.RefreshAbility(av);
            SetPID(pk);
            SetIVs(pk);
        }

        private int GetAbilityIndex(EncounterCriteria criteria, PersonalInfo pi)
        {
            switch (AbilityType)
            {
                case 00: // 0 - 0
                case 01: // 1 - 1
                case 02: // 2 - H
                    return AbilityType;
                case 03: // 0/1
                case 04: // 0/1/H
                    return criteria.GetAbility(AbilityType, pi); // 3 or 2
                default:
                    throw new ArgumentException(nameof(AbilityType));
            }
        }

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
            var rng = Util.Rand;
            if (ivflag == 0) // Random IVs
            {
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = IVs[i] > 31 ? rng.Next(32) : IVs[i];
            }
            else // 1/2/3 perfect IVs
            {
                int IVCount = ivflag - 0xFB;
                do { finalIVs[rng.Next(6)] = 31; }
                while (finalIVs.Count(iv => iv == 31) < IVCount);
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = finalIVs[i] == 31 ? 31 : rng.Next(32);
            }
            pk.IVs = finalIVs;
        }

        protected override bool IsMatchExact(PKM pkm)
        {
            if (pkm.Egg_Location == 0) // Not Egg
            {
                if (OTGender != 3)
                {
                    if (SID != pkm.SID) return false;
                    if (TID != pkm.TID) return false;
                    if (OTGender != pkm.OT_Gender) return false;
                }
                var OT = GetOT(pkm.Language);
                if (!string.IsNullOrEmpty(OT) && OT != pkm.OT_Name) return false;
                if (OriginGame != 0 && OriginGame != pkm.Version) return false;
                if (EncryptionConstant != 0 && EncryptionConstant != pkm.EncryptionConstant) return false;
            }

            if (Form != pkm.AltForm && !Legal.IsFormChangeable(pkm, Species))
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
                if (!PIDType.IsValid(pkm)) return false;
                if (EggLocation != pkm.Egg_Location) return false;
                if (MetLocation != pkm.Met_Location) return false;
            }

            if (MetLevel != pkm.Met_Level) return false;
            if (Ball != pkm.Ball) return false;
            if (OTGender < 3 && OTGender != pkm.OT_Gender) return false;
            if (Nature != -1 && pkm.Nature != Nature) return false;
            if (Gender != 3 && Gender != pkm.Gender) return false;

            if (pkm is IAwakened s && s.IsAwakeningBelow(this))
                return false;

            return true;
        }

        protected override bool IsMatchDeferred(PKM pkm)
        {
            return pkm.Species == Species;
        }
    }
}
