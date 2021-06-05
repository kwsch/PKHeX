using System;

namespace PKHeX.Core
{
    /// <summary> Generation 2 <see cref="PKM"/> format for <see cref="GameVersion.Stadium2"/>. </summary>
    public sealed class SK2 : GBPKM, ICaughtData2
    {
        public override PersonalInfo PersonalInfo => PersonalTable.C[Species];

        public override bool Valid => Species <= 252;

        public override int SIZE_PARTY => PokeCrypto.SIZE_2STADIUM;
        public override int SIZE_STORED => PokeCrypto.SIZE_2STADIUM;
        private bool IsEncodingJapanese { get; set; }
        public override bool Japanese => IsEncodingJapanese;
        public override bool Korean => false;
        private const int StringLength = 12;

        public override int Format => 2;
        public override int OTLength => StringLength;
        public override int NickLength => StringLength;

        public SK2(bool jp = false) : base(PokeCrypto.SIZE_2STADIUM) => IsEncodingJapanese = jp;
        public SK2(byte[] data) : this(data, IsJapanese(data)) { }
        public SK2(byte[] data, bool jp) : base(data) => IsEncodingJapanese = jp;

        public override PKM Clone() => new SK2((byte[])Data.Clone(), Japanese)
        {
            Identifier = Identifier,
            IsEgg = IsEgg,
        };

        protected override byte[] Encrypt() => Data;

        #region Stored Attributes
        public override int Species { get => Data[0]; set => Data[0] = (byte)value; }
        public override int SpriteItem => ItemConverter.GetItemFuture2((byte)HeldItem);
        public override int HeldItem { get => Data[0x1]; set => Data[0x1] = (byte)value; }
        public override int Move1 { get => Data[2]; set => Data[2] = (byte)value; }
        public override int Move2 { get => Data[3]; set => Data[3] = (byte)value; }
        public override int Move3 { get => Data[4]; set => Data[4] = (byte)value; }
        public override int Move4 { get => Data[5]; set => Data[5] = (byte)value; }
        public override int TID { get => BigEndian.ToUInt16(Data, 6); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 6); }
        public override uint EXP { get => BigEndian.ToUInt32(Data, 8); set => BigEndian.GetBytes(value).CopyTo(Data, 8); } // not 3 bytes like in PK2
        public override int EV_HP { get => BigEndian.ToUInt16(Data, 0x0C); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x0C); }
        public override int EV_ATK { get => BigEndian.ToUInt16(Data, 0x0E); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x0E); }
        public override int EV_DEF { get => BigEndian.ToUInt16(Data, 0x10); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x10); }
        public override int EV_SPE { get => BigEndian.ToUInt16(Data, 0x12); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x12); }
        public override int EV_SPC { get => BigEndian.ToUInt16(Data, 0x14); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x14); }
        public override ushort DV16 { get => BigEndian.ToUInt16(Data, 0x16); set => BigEndian.GetBytes(value).CopyTo(Data, 0x16); }
        public override int Move1_PP { get => Data[0x18] & 0x3F; set => Data[0x18] = (byte)((Data[0x18] & 0xC0) | Math.Min(63, value)); }
        public override int Move2_PP { get => Data[0x19] & 0x3F; set => Data[0x19] = (byte)((Data[0x19] & 0xC0) | Math.Min(63, value)); }
        public override int Move3_PP { get => Data[0x1A] & 0x3F; set => Data[0x1A] = (byte)((Data[0x1A] & 0xC0) | Math.Min(63, value)); }
        public override int Move4_PP { get => Data[0x1B] & 0x3F; set => Data[0x1B] = (byte)((Data[0x1B] & 0xC0) | Math.Min(63, value)); }
        public override int Move1_PPUps { get => (Data[0x18] & 0xC0) >> 6; set => Data[0x18] = (byte)((Data[0x18] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move2_PPUps { get => (Data[0x19] & 0xC0) >> 6; set => Data[0x19] = (byte)((Data[0x19] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move3_PPUps { get => (Data[0x1A] & 0xC0) >> 6; set => Data[0x1A] = (byte)((Data[0x1A] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move4_PPUps { get => (Data[0x1B] & 0xC0) >> 6; set => Data[0x1B] = (byte)((Data[0x1B] & 0x3F) | ((value & 0x3) << 6)); }
        public override int CurrentFriendship { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }

        public override int Stat_Level { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
        public override bool IsEgg { get => (Data[0x1E] & 1) == 1; set => Data[0x1E] = (byte)((Data[0x1E] & ~1) | (value ? 1 : 0)); }

        public bool IsRental
        {
            get => (Data[0x1E] & 4) == 4;
            set
            {
                if (!value)
                {
                    Data[0x1E] &= 0xFB;
                    return;
                }

                Data[0x1E] |= 4;
                OT_Name = string.Empty;
            }
        }

        // 0x1F

        private byte PKRS { get => Data[0x20]; set => Data[0x20] = value; }
        // Crystal only Caught Data
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | value << 4); }

        public int CaughtData { get => BigEndian.ToUInt16(Data, 0x21); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x21); }

        public int Met_TimeOfDay { get => (CaughtData >> 14) & 0x3; set => CaughtData = (CaughtData & 0x3FFF) | ((value & 0x3) << 14); }
        public override int Met_Level { get => (CaughtData >> 8) & 0x3F; set => CaughtData = (CaughtData & 0xC0FF) | ((value & 0x3F) << 8); }
        public override int OT_Gender { get => (CaughtData >> 7) & 1; set => CaughtData = (CaughtData & 0xFF7F) | ((value & 1) << 7); }
        public override int Met_Location { get => CaughtData & 0x7F; set => CaughtData = (CaughtData & 0xFF80) | (value & 0x7F); }

        public override string Nickname { get => GetString(0x24, StringLength); set => StringConverter12.SetString1(value, 12, Japanese).CopyTo(Data, 0x24); }

        public override string OT_Name
        {
            get => GetString(0x30, StringLength);
            set
            {
                if (IsRental)
                {
                    Array.Clear(Data, 0x30, StringLength);
                    return;
                }
                SetString(value, StringLength).CopyTo(Data, 0x30);
            }
        }

        public override byte[] Nickname_Trash { get => GetData(0x24, 12); set { if (value.Length == 12) value.CopyTo(Data, 0x24); } }
        public override byte[] OT_Trash { get => GetData(0x30, 12); set { if (value.Length == 12) value.CopyTo(Data, 0x30); } }
        #endregion

        #region Party Attributes
        public override int Status_Condition { get; set; }
        public override int Stat_HPCurrent { get; set; }
        public override int Stat_HPMax { get; set; }
        public override int Stat_ATK { get; set; }
        public override int Stat_DEF { get; set; }
        public override int Stat_SPE { get; set; }
        public override int Stat_SPA { get; set; }
        public override int Stat_SPD { get; set; }
        #endregion

        public override int OT_Friendship { get => CurrentFriendship; set => CurrentFriendship = value; }
        public override bool HasOriginalMetLocation => CaughtData != 0;
        public override int Version { get => (int)GameVersion.GSC; set { } }

        private string GetString(int offset, int length) => StringConverter12.GetString1(Data, offset, length - 1, Japanese);
        private byte[] SetString(string value, int length) => StringConverter12.SetString1(value, length - 1, Japanese);

        protected override byte[] GetNonNickname(int language)
        {
            var name = SpeciesName.GetSpeciesNameGeneration(Species, language, Format);
            return SetString(name, StringLength);
        }

        public override void SetNotNicknamed(int language)
        {
            var name = SpeciesName.GetSpeciesNameGeneration(Species, language, Format);
            Array.Clear(Data, 0x24, 0xC);
            Nickname = name;
        }

        // Maximums
        public override int MaxMoveID => Legal.MaxMoveID_2;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_2;
        public override int MaxAbilityID => Legal.MaxAbilityID_2;
        public override int MaxItemID => Legal.MaxItemID_2;

        public PK2 ConvertToPK2()
        {
            return new(Japanese)
            {
                Species = Species,
                HeldItem = HeldItem,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                TID = TID,
                EXP = EXP,
                EV_HP = EV_HP,
                EV_ATK  = EV_ATK,
                EV_DEF  = EV_DEF,
                EV_SPE  = EV_SPE,
                EV_SPC = EV_SPC,
                DV16 = DV16,
                Move1_PP = Move1_PP,
                Move2_PP = Move2_PP,
                Move3_PP = Move3_PP,
                Move4_PP = Move4_PP,
                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                CurrentFriendship = CurrentFriendship,
                Stat_Level = Stat_Level,
                IsEgg = IsEgg,
                PKRS_Days = PKRS_Days,
                PKRS_Strain = PKRS_Strain,
                CaughtData = CaughtData,

                // Only copies until first 0x50 terminator, but just copy everything
                Nickname = Nickname,
                OT_Name = IsRental ? Japanese ? "1337" : "PKHeX" : OT_Name,
            };
        }

        private static bool IsJapanese(byte[] data)
        {
            if (!StringConverter12.GetIsG1Japanese(data, 0x24, StringLength))
                return false;
            if (!StringConverter12.GetIsG1Japanese(data, 0x30, StringLength))
                return false;

            for (int i = 6; i < 0xC; i++)
            {
                if (data[0x24 + i] is not (0 or StringConverter12.G1TerminatorCode))
                    return false;
                if (data[0x30 + i] is not (0 or StringConverter12.G1TerminatorCode))
                    return false;
            }
            return true;
        }

        private static bool IsEnglish(byte[] data) => StringConverter12.GetIsG1English(data, 0x24, StringLength) && StringConverter12.GetIsG1English(data, 0x30, StringLength);
        public bool IsPossible(bool japanese) => japanese ? IsJapanese(Data) : IsEnglish(Data);
        public void SwapLanguage() => IsEncodingJapanese ^= true;
    }
}
