using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 Mystery Gift Template File
    /// </summary>
    public sealed class PGF : DataMysteryGift, IRibbonSetEvent3, IRibbonSetEvent4, ILangNick, IContestStats, INature
    {
        public const int Size = 0xCC;
        public override int Format => 5;

        public PGF() : this(new byte[Size]) { }
        public PGF(byte[] data) : base(data) { }

        public override int TID { get => BitConverter.ToUInt16(Data, 0x00); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x00); }
        public override int SID { get => BitConverter.ToUInt16(Data, 0x02); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x02); }
        public int OriginGame { get => Data[0x04]; set => Data[0x04] = (byte)value; }
        // Unused 0x05 0x06, 0x07
        public uint PID { get => BitConverter.ToUInt32(Data, 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, 0x08); }

        private byte RIB0 { get => Data[0x0C]; set => Data[0x0C] = value; }
        private byte RIB1 { get => Data[0x0D]; set => Data[0x0D] = value; }
        public bool RibbonCountry          { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonNational         { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonEarth            { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonWorld            { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonClassic          { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonPremier          { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonEvent            { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonBirthday         { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonSpecial          { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonSouvenir         { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonWishing          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonChampionBattle   { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonChampionRegional { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonChampionNational { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonChampionWorld    { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RIB1_7                 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

        public override int Ball { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
        public override int HeldItem { get => BitConverter.ToUInt16(Data, 0x10); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x10); }
        public int Move1 { get => BitConverter.ToUInt16(Data, 0x12); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x12); }
        public int Move2 { get => BitConverter.ToUInt16(Data, 0x14); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x14); }
        public int Move3 { get => BitConverter.ToUInt16(Data, 0x16); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x16); }
        public int Move4 { get => BitConverter.ToUInt16(Data, 0x18); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x18); }
        public override int Species { get => BitConverter.ToUInt16(Data, 0x1A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1A); }
        public override int Form { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
        public int Language { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }

        public string Nickname
        {
            get => Util.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x1E, 0x16));
            set => Encoding.Unicode.GetBytes(value.PadRight(0xB, (char)0xFFFF)).CopyTo(Data, 0x1E);
        }

        public int Nature { get => (sbyte)Data[0x34]; set => Data[0x34] = (byte)value; }
        public override int Gender { get => Data[0x35]; set => Data[0x35] = (byte)value; }
        public override int AbilityType { get => Data[0x36]; set => Data[0x36] = (byte)value; }
        public int PIDType { get => Data[0x37]; set => Data[0x37] = (byte)value; }
        public override int EggLocation { get => BitConverter.ToUInt16(Data, 0x38); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x38); }
        public ushort MetLocation { get => BitConverter.ToUInt16(Data, 0x3A); set => BitConverter.GetBytes(value).CopyTo(Data, 0x3A); }
        public int MetLevel { get => Data[0x3C]; set => Data[0x3C] = (byte)value; }
        public int CNT_Cool { get => Data[0x3D]; set => Data[0x3D] = (byte)value; }
        public int CNT_Beauty { get => Data[0x3E]; set => Data[0x3E] = (byte)value; }
        public int CNT_Cute { get => Data[0x3F]; set => Data[0x3F] = (byte)value; }
        public int CNT_Smart { get => Data[0x40]; set => Data[0x40] = (byte)value; }
        public int CNT_Tough { get => Data[0x41]; set => Data[0x41] = (byte)value; }
        public int CNT_Sheen { get => Data[0x42]; set => Data[0x42] = (byte)value; }
        public int IV_HP { get => Data[0x43]; set => Data[0x43] = (byte)value; }
        public int IV_ATK { get => Data[0x44]; set => Data[0x44] = (byte)value; }
        public int IV_DEF { get => Data[0x45]; set => Data[0x45] = (byte)value; }
        public int IV_SPE { get => Data[0x46]; set => Data[0x46] = (byte)value; }
        public int IV_SPA { get => Data[0x47]; set => Data[0x47] = (byte)value; }
        public int IV_SPD { get => Data[0x48]; set => Data[0x48] = (byte)value; }
        // Unused 0x49
        public override string OT_Name {
            get => Util.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x4A, 0x10));
            set => Encoding.Unicode.GetBytes(value.PadRight(0x08, (char)0xFFFF)).CopyTo(Data, 0x4A); }

        public int OTGender { get => Data[0x5A]; set => Data[0x5A] = (byte)value; }
        public override int Level { get => Data[0x5B]; set => Data[0x5C] = (byte)value; }
        public override bool IsEgg { get => Data[0x5C] == 1; set => Data[0x5C] = (byte)(value ? 1 : 0); }
        // Unused 0x5D 0x5E 0x5F
        public override string CardTitle
        {
            get => Util.TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x60, 0x4A));
            set => Encoding.Unicode.GetBytes((value + '\uFFFF').PadRight(0x4A / 2, '\0')).CopyTo(Data, 0x60);
        }

        // Card Attributes
        public override int ItemID { get => BitConverter.ToUInt16(Data, 0x00); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x00); }

        private ushort Year { get => BitConverter.ToUInt16(Data, 0xAE); set => BitConverter.GetBytes(value).CopyTo(Data, 0xAE); }
        private byte Month { get => Data[0xAD]; set => Data[0xAD] = value; }
        private byte Day { get => Data[0xAC]; set => Data[0xAC] = value; }

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
            get => BitConverter.ToUInt16(Data, 0xB0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xB0);
        }

        public int CardLocation { get => Data[0xB2]; set => Data[0xB2] = (byte)value; }
        public int CardType { get => Data[0xB3]; set => Data[0xB3] = (byte)value; }
        public override bool GiftUsed { get => Data[0xB4] >> 1 > 0; set => Data[0xB4] = (byte)((Data[0xB4] & ~2) | (value ? 2 : 0)); }
        public bool MultiObtain { get => Data[0xB4] == 1; set => Data[0xB4] = (byte)(value ? 1 : 0); }

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

        public bool IsNicknamed => Nickname.Length > 0;
        public override bool IsShiny => PIDType == 2;
        public override int Location { get => MetLocation; set => MetLocation = (ushort)value; }
        public override IReadOnlyList<int> Moves => new[] { Move1, Move2, Move3, Move4 };
        public override bool IsPokémon { get => CardType == 1; set { if (value) CardType = 1; } }
        public override bool IsItem { get => CardType == 2; set { if (value) CardType = 2; } }
        public bool IsPower { get => CardType == 3; set { if (value) CardType = 3; } }

        public override PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            if (!IsPokémon)
                throw new ArgumentException(nameof(IsPokémon));

            var rnd = Util.Rand;

            var dt = DateTime.Now;
            if (Day == 0)
            {
                Day = (byte)dt.Day;
                Month = (byte)dt.Month;
                Year = (byte)dt.Year;
            }

            int currentLevel = Level > 0 ? Level : rnd.Next(1, 101);
            var pi = PersonalTable.B2W2.GetFormeEntry(Species, Form);
            PK5 pk = new PK5
            {
                Species = Species,
                HeldItem = HeldItem,
                Met_Level = currentLevel,
                Nature = Nature != -1 ? Nature : rnd.Next(25),
                AltForm = Form,
                Version = OriginGame == 0 ? SAV.Game : OriginGame,
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

                EXP = Experience.GetEXP(currentLevel, pi.EXPGrowth),

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

                FatefulEncounter = true,
            };
            if (SAV.Generation > 5 && OriginGame == 0) // Gen6+, give random gen5 game
                pk.Version = (int)GameVersion.W + rnd.Next(4);

            if (Move1 == 0) // No moves defined
                pk.Moves = MoveLevelUp.GetEncounterMoves(Species, Form, Level, (GameVersion)pk.Version);

            pk.SetMaximumPPCurrent();

            if (IsEgg) // User's
            {
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
                pk.OT_Name = SAV.OT;
                pk.OT_Gender = SAV.Gender;
            }
            else // Hardcoded
            {
                pk.TID = TID;
                pk.SID = SID;
                pk.OT_Name = OT_Name;
                pk.OT_Gender = (OTGender == 3 ? SAV.Gender : OTGender) & 1; // some events have variable gender based on receiving SaveFile
            }

            pk.IsNicknamed = IsNicknamed;
            pk.Nickname = IsNicknamed ? Nickname : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Format);

            SetPINGA(pk, criteria);

            if (IsEgg)
                SetEggMetDetails(pk);

            pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

            pk.RefreshChecksum();
            return pk;
        }

        private void SetEggMetDetails(PK5 pk)
        {
            pk.IsEgg = true;
            pk.EggMetDate = Date;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(0, pk.Language, Format);
            pk.IsNicknamed = true;
        }

        private void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = PersonalTable.B2W2.GetFormeEntry(Species, Form);
            pk.Nature = (int)criteria.GetNature((Nature)Nature);
            pk.Gender = pi.Gender == 255 ? 2 : Gender != 2 ? Gender : criteria.GetGender(-1, pi);
            var av = GetAbilityIndex(criteria, pi);
            SetPID(pk, av);
            pk.RefreshAbility(av);
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

        private void SetPID(PKM pk, int av)
        {
            if (PID != 0)
            {
                pk.PID = PID;
                return;
            }

            pk.PID = Util.Rand32();
            // Force Gender
            var rnd = Util.Rand;
            do { pk.PID = (pk.PID & 0xFFFFFF00) | (uint)rnd.Next(0x100); }
            while (!pk.IsGenderValid());

            if (PIDType == 2) // Always
            {
                uint gb = pk.PID & 0xFF;
                pk.PID = PIDGenerator.GetMG5ShinyPID(gb, (uint)av, pk.TID, pk.SID);
            }
            else if (PIDType != 1) // Force Not Shiny
            {
                if (pk.IsShiny)
                    pk.PID ^= 0x10000000;
            }

            if (av == 1)
                pk.PID |= 0x10000;
            else
                pk.PID &= 0xFFFEFFFF;
        }

        private void SetIVs(PKM pk)
        {
            int[] finalIVs = new int[6];
            var rnd = Util.Rand;
            for (int i = 0; i < IVs.Length; i++)
                finalIVs[i] = IVs[i] == 0xFF ? rnd.Next(32) : IVs[i];
            pk.IVs = finalIVs;
        }

        protected override bool IsMatchExact(PKM pkm)
        {
            if (!IsEgg)
            {
                if (SID != pkm.SID) return false;
                if (TID != pkm.TID) return false;
                if (OT_Name != pkm.OT_Name) return false;
                if (OTGender < 3 && OTGender != pkm.OT_Gender) return false;
                if (PID != 0 && pkm.PID != PID) return false;
                if (PIDType == 0 && pkm.IsShiny) return false;
                if (PIDType == 2 && !pkm.IsShiny) return false;
                if (OriginGame != 0 && OriginGame != pkm.Version) return false;
                if (Language != 0 && Language != pkm.Language) return false;

                if (EggLocation != pkm.Egg_Location) return false;
                if (MetLocation != pkm.Met_Location) return false;
            }
            else
            {
                if (EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != Locations.LinkTrade5)
                        return false;
                }
                else if (PIDType == 0 && pkm.IsShiny)
                {
                    return false; // can't be traded away for unshiny
                }

                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }

            if (Form != pkm.AltForm && !Legal.IsFormChangeable(pkm, Species))
                return false;

            if (Level != pkm.Met_Level) return false;
            if (Ball != pkm.Ball) return false;
            if (Nature != -1 && pkm.Nature != Nature)
                return false;
            if (Gender != 2 && Gender != pkm.Gender) return false;

            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;

            return true;
        }

        protected override bool IsMatchDeferred(PKM pkm) => false;
    }
}
