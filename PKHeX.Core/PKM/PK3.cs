using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary> Generation 3 <see cref="PKM"/> format. </summary>
    public sealed class PK3 : G3PKM
    {
        private static readonly ushort[] Unused =
        {
            0x2A, 0x2B
        };

        public override int SIZE_PARTY => PokeCrypto.SIZE_3PARTY;
        public override int SIZE_STORED => PokeCrypto.SIZE_3STORED;
        public override int Format => 3;
        public override PersonalInfo PersonalInfo => PersonalTable.RS[Species];

        public override IReadOnlyList<ushort> ExtraBytes => Unused;

        public override byte[] Data { get; }
        public PK3() => Data = new byte[PokeCrypto.SIZE_3PARTY];

        public PK3(byte[] data)
        {
            PokeCrypto.DecryptIfEncrypted3(ref data);
            if (data.Length != PokeCrypto.SIZE_3PARTY)
                Array.Resize(ref data, PokeCrypto.SIZE_3PARTY);
            Data = data;
        }

        public override PKM Clone()
        {
            // Don't use the byte[] constructor, the DecryptIfEncrypted call is based on checksum.
            // An invalid checksum will shuffle the data; we already know it's un-shuffled. Set up manually.
            var pk = new PK3 {Identifier = Identifier};
            Data.CopyTo(pk.Data, 0);
            return pk;
        }

        private string GetString(int Offset, int Count) => StringConverter3.GetString3(Data, Offset, Count, Japanese);
        private byte[] SetString(string value, int maxLength) => StringConverter3.SetString3(value, maxLength, Japanese);

        private const string EggNameJapanese = "タマゴ";

        // Trash Bytes
        public override byte[] Nickname_Trash { get => GetData(0x08, 10); set { if (value.Length == 10) value.CopyTo(Data, 0x08); } }
        public override byte[] OT_Trash { get => GetData(0x14, 7); set { if (value.Length == 7) value.CopyTo(Data, 0x14); } }

        // At top for System.Reflection execution order hack

        // 0x20 Intro
        public override uint PID { get => BitConverter.ToUInt32(Data, 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, 0x00); }
        public override int TID { get => BitConverter.ToUInt16(Data, 0x04); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x04); }
        public override int SID { get => BitConverter.ToUInt16(Data, 0x06); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x06); }
        public override string Nickname { get => GetString(0x08, 10); set => SetString(IsEgg ? EggNameJapanese : value, 10).CopyTo(Data, 0x08); }
        public override int Language { get => Data[0x12]; set => Data[0x12] = (byte)value; }
        public bool FlagIsBadEgg   { get => (Data[0x13] & 1) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~1) | (value ? 1 : 0)); }
        public bool FlagHasSpecies { get => (Data[0x13] & 2) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~2) | (value ? 2 : 0)); }
        public bool FlagIsEgg      { get => (Data[0x13] & 4) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~4) | (value ? 4 : 0)); }
        public override string OT_Name { get => GetString(0x14, 7); set => SetString(value, 7).CopyTo(Data, 0x14); }
        public override int MarkValue { get => SwapBits(Data[0x1B], 1, 2); protected set => Data[0x1B] = (byte)SwapBits(value, 1, 2); }
        public override ushort Checksum { get => BitConverter.ToUInt16(Data, 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1C); }
        public override ushort Sanity { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1E); }

        #region Block A
        public int SpeciesID3 { get => BitConverter.ToUInt16(Data, 0x20); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x20); } // raw access

        public override int Species
        {
            get => SpeciesConverter.GetG4Species(SpeciesID3);
            set
            {
                SpeciesID3 = SpeciesConverter.GetG3Species(value);
                FlagHasSpecies = Species > 0;
            }
        }

        public override int SpriteItem => ItemConverter.GetG4Item((ushort)HeldItem);
        public override int HeldItem { get => BitConverter.ToUInt16(Data, 0x22); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); }

        public override uint EXP { get => BitConverter.ToUInt32(Data, 0x24); set => BitConverter.GetBytes(value).CopyTo(Data, 0x24); }
        private byte PPUps { get => Data[0x28]; set => Data[0x28] = value; }
        public override int Move1_PPUps { get => (PPUps >> 0) & 3; set => PPUps = (byte)((PPUps & ~(3 << 0)) | value << 0); }
        public override int Move2_PPUps { get => (PPUps >> 2) & 3; set => PPUps = (byte)((PPUps & ~(3 << 2)) | value << 2); }
        public override int Move3_PPUps { get => (PPUps >> 4) & 3; set => PPUps = (byte)((PPUps & ~(3 << 4)) | value << 4); }
        public override int Move4_PPUps { get => (PPUps >> 6) & 3; set => PPUps = (byte)((PPUps & ~(3 << 6)) | value << 6); }
        public override int OT_Friendship { get => Data[0x29]; set => Data[0x29] = (byte)value; }
        // Unused 0x2A 0x2B
        #endregion

        #region Block B
        public override int Move1 { get => BitConverter.ToUInt16(Data, 0x2C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2C); }
        public override int Move2 { get => BitConverter.ToUInt16(Data, 0x2E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E); }
        public override int Move3 { get => BitConverter.ToUInt16(Data, 0x30); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x30); }
        public override int Move4 { get => BitConverter.ToUInt16(Data, 0x32); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x32); }
        public override int Move1_PP { get => Data[0x34]; set => Data[0x34] = (byte)value; }
        public override int Move2_PP { get => Data[0x35]; set => Data[0x35] = (byte)value; }
        public override int Move3_PP { get => Data[0x36]; set => Data[0x36] = (byte)value; }
        public override int Move4_PP { get => Data[0x37]; set => Data[0x37] = (byte)value; }
        #endregion

        #region Block C
        public override int EV_HP { get => Data[0x38]; set => Data[0x38] = (byte)value; }
        public override int EV_ATK { get => Data[0x39]; set => Data[0x39] = (byte)value; }
        public override int EV_DEF { get => Data[0x3A]; set => Data[0x3A] = (byte)value; }
        public override int EV_SPE { get => Data[0x3B]; set => Data[0x3B] = (byte)value; }
        public override int EV_SPA { get => Data[0x3C]; set => Data[0x3C] = (byte)value; }
        public override int EV_SPD { get => Data[0x3D]; set => Data[0x3D] = (byte)value; }
        public override int CNT_Cool { get => Data[0x3E]; set => Data[0x3E] = (byte)value; }
        public override int CNT_Beauty { get => Data[0x3F]; set => Data[0x3F] = (byte)value; }
        public override int CNT_Cute { get => Data[0x40]; set => Data[0x40] = (byte)value; }
        public override int CNT_Smart { get => Data[0x41]; set => Data[0x41] = (byte)value; }
        public override int CNT_Tough { get => Data[0x42]; set => Data[0x42] = (byte)value; }
        public override int CNT_Sheen { get => Data[0x43]; set => Data[0x43] = (byte)value; }
        #endregion

        #region Block D
        private byte PKRS { get => Data[0x44]; set => Data[0x44] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | value << 4); }
        public override int Met_Location { get => Data[0x45]; set => Data[0x45] = (byte)value; }
        // Origins
        private ushort Origins { get => BitConverter.ToUInt16(Data, 0x46); set => BitConverter.GetBytes(value).CopyTo(Data, 0x46); }
        public override int Met_Level { get => Origins & 0x7F; set => Origins = (ushort)((Origins & ~0x7F) | value); }
        public override int Version { get => (Origins >> 7) & 0xF; set => Origins = (ushort)((Origins & ~0x780) | ((value & 0xF) << 7)); }
        public override int Ball { get => (Origins >> 11) & 0xF; set => Origins = (ushort)((Origins & ~0x7800) | ((value & 0xF) << 11)); }
        public override int OT_Gender { get => (Origins >> 15) & 1; set => Origins = (ushort)((Origins & ~(1 << 15)) | ((value & 1) << 15)); }

        public uint IV32 { get => BitConverter.ToUInt32(Data, 0x48); set => BitConverter.GetBytes(value).CopyTo(Data, 0x48); }
        public override int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
        public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
        public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
        public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
        public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
        public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }

        public override bool IsEgg
        {
            get => ((IV32 >> 30) & 1) == 1;
            set
            {
                IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0);
                FlagIsEgg = value;
                if (value)
                {
                    Nickname = EggNameJapanese;
                    Language = (int) LanguageID.Japanese;
                }
            }
        }

        public override bool AbilityBit { get => IV32 >> 31 == 1; set => IV32 = (IV32 & 0x7FFFFFFF) | (value ? 1u << 31 : 0u); }

        private uint RIB0 { get => BitConverter.ToUInt32(Data, 0x4C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x4C); }
        public override int RibbonCountG3Cool        { get => (int)(RIB0 >> 00) & 7; set => RIB0 = ((RIB0 & ~(7u << 00)) | (uint)(value & 7) << 00); }
        public override int RibbonCountG3Beauty      { get => (int)(RIB0 >> 03) & 7; set => RIB0 = ((RIB0 & ~(7u << 03)) | (uint)(value & 7) << 03); }
        public override int RibbonCountG3Cute        { get => (int)(RIB0 >> 06) & 7; set => RIB0 = ((RIB0 & ~(7u << 06)) | (uint)(value & 7) << 06); }
        public override int RibbonCountG3Smart       { get => (int)(RIB0 >> 09) & 7; set => RIB0 = ((RIB0 & ~(7u << 09)) | (uint)(value & 7) << 09); }
        public override int RibbonCountG3Tough       { get => (int)(RIB0 >> 12) & 7; set => RIB0 = ((RIB0 & ~(7u << 12)) | (uint)(value & 7) << 12); }
        public override bool RibbonChampionG3Hoenn   { get => (RIB0 & (1 << 15)) == 1 << 15; set => RIB0 = ((RIB0 & ~(1u << 15)) | (value ? 1u << 15 : 0u)); }
        public override bool RibbonWinning           { get => (RIB0 & (1 << 16)) == 1 << 16; set => RIB0 = ((RIB0 & ~(1u << 16)) | (value ? 1u << 16 : 0u)); }
        public override bool RibbonVictory           { get => (RIB0 & (1 << 17)) == 1 << 17; set => RIB0 = ((RIB0 & ~(1u << 17)) | (value ? 1u << 17 : 0u)); }
        public override bool RibbonArtist            { get => (RIB0 & (1 << 18)) == 1 << 18; set => RIB0 = ((RIB0 & ~(1u << 18)) | (value ? 1u << 18 : 0u)); }
        public override bool RibbonEffort            { get => (RIB0 & (1 << 19)) == 1 << 19; set => RIB0 = ((RIB0 & ~(1u << 19)) | (value ? 1u << 19 : 0u)); }
        public override bool RibbonChampionBattle    { get => (RIB0 & (1 << 20)) == 1 << 20; set => RIB0 = ((RIB0 & ~(1u << 20)) | (value ? 1u << 20 : 0u)); }
        public override bool RibbonChampionRegional  { get => (RIB0 & (1 << 21)) == 1 << 21; set => RIB0 = ((RIB0 & ~(1u << 21)) | (value ? 1u << 21 : 0u)); }
        public override bool RibbonChampionNational  { get => (RIB0 & (1 << 22)) == 1 << 22; set => RIB0 = ((RIB0 & ~(1u << 22)) | (value ? 1u << 22 : 0u)); }
        public override bool RibbonCountry           { get => (RIB0 & (1 << 23)) == 1 << 23; set => RIB0 = ((RIB0 & ~(1u << 23)) | (value ? 1u << 23 : 0u)); }
        public override bool RibbonNational          { get => (RIB0 & (1 << 24)) == 1 << 24; set => RIB0 = ((RIB0 & ~(1u << 24)) | (value ? 1u << 24 : 0u)); }
        public override bool RibbonEarth             { get => (RIB0 & (1 << 25)) == 1 << 25; set => RIB0 = ((RIB0 & ~(1u << 25)) | (value ? 1u << 25 : 0u)); }
        public override bool RibbonWorld             { get => (RIB0 & (1 << 26)) == 1 << 26; set => RIB0 = ((RIB0 & ~(1u << 26)) | (value ? 1u << 26 : 0u)); }
        public override bool Unused1 { get => (RIB0 & (1 << 27)) == 1 << 27; set => RIB0 = ((RIB0 & ~(1u << 27)) | (value ? 1u << 27 : 0u)); }
        public override bool Unused2 { get => (RIB0 & (1 << 28)) == 1 << 28; set => RIB0 = ((RIB0 & ~(1u << 28)) | (value ? 1u << 28 : 0u)); }
        public override bool Unused3 { get => (RIB0 & (1 << 29)) == 1 << 29; set => RIB0 = ((RIB0 & ~(1u << 29)) | (value ? 1u << 29 : 0u)); }
        public override bool Unused4 { get => (RIB0 & (1 << 30)) == 1 << 30; set => RIB0 = ((RIB0 & ~(1u << 30)) | (value ? 1u << 30 : 0u)); }
        public override bool FatefulEncounter { get => RIB0 >> 31 == 1; set => RIB0 = (RIB0 & ~(1 << 31)) | (uint)(value ? 1 << 31 : 0); }
        #endregion

        #region Battle Stats
        public override int Status_Condition { get => BitConverter.ToInt32(Data, 0x50); set => BitConverter.GetBytes(value).CopyTo(Data, 0x50); }
        public override int Stat_Level { get => Data[0x54]; set => Data[0x54] = (byte)value; }
        public sbyte HeldMailID { get => (sbyte)Data[0x55]; set => Data[0x55] = (byte)value; }
        public override int Stat_HPCurrent { get => BitConverter.ToUInt16(Data, 0x56); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x56); }
        public override int Stat_HPMax { get => BitConverter.ToUInt16(Data, 0x58); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x58); }
        public override int Stat_ATK { get => BitConverter.ToUInt16(Data, 0x5A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5A); }
        public override int Stat_DEF { get => BitConverter.ToUInt16(Data, 0x5C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5C); }
        public override int Stat_SPE { get => BitConverter.ToUInt16(Data, 0x5E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5E); }
        public override int Stat_SPA { get => BitConverter.ToUInt16(Data, 0x60); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x60); }
        public override int Stat_SPD { get => BitConverter.ToUInt16(Data, 0x62); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x62); }
        #endregion

        // Generated Attributes
        public override bool Japanese => IsEgg || Language == (int)LanguageID.Japanese;

        protected override byte[] Encrypt()
        {
            RefreshChecksum();
            return PokeCrypto.EncryptArray3(Data);
        }

        public override void RefreshChecksum()
        {
            FlagIsBadEgg = false;
            base.RefreshChecksum();
        }

        public PK4 ConvertToPK4()
        {
            PK4 pk4 = new PK4 // Convert away!
            {
                PID = PID,
                Species = Species,
                TID = TID,
                SID = SID,
                EXP = IsEgg ? Experience.GetEXP(5, PersonalInfo.EXPGrowth) : EXP,
                Gender = PKX.GetGenderFromPID(Species, PID),
                AltForm = AltForm,
                // IsEgg = false, -- already false
                OT_Friendship = 70,
                Markings = Markings,
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
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                IV_HP = IV_HP,
                IV_ATK = IV_ATK,
                IV_DEF = IV_DEF,
                IV_SPA = IV_SPA,
                IV_SPD = IV_SPD,
                IV_SPE = IV_SPE,
                Ability = Ability,
                Version = Version,
                Ball = Ball,
                PKRS_Strain = PKRS_Strain,
                PKRS_Days = PKRS_Days,
                OT_Gender = OT_Gender,
                MetDate = DateTime.Now,
                Met_Level = CurrentLevel,
                Met_Location = Locations.Transfer3, // Pal Park

                RibbonChampionG3Hoenn = RibbonChampionG3Hoenn,
                RibbonWinning = RibbonWinning,
                RibbonVictory = RibbonVictory,
                RibbonArtist = RibbonArtist,
                RibbonEffort = RibbonEffort,
                RibbonChampionBattle = RibbonChampionBattle,
                RibbonChampionRegional = RibbonChampionRegional,
                RibbonChampionNational = RibbonChampionNational,
                RibbonCountry = RibbonCountry,
                RibbonNational = RibbonNational,
                RibbonEarth = RibbonEarth,
                RibbonWorld = RibbonWorld,

                // byte -> bool contest ribbons
                RibbonG3Cool         = RibbonCountG3Cool > 0,
                RibbonG3CoolSuper    = RibbonCountG3Cool > 1,
                RibbonG3CoolHyper    = RibbonCountG3Cool > 2,
                RibbonG3CoolMaster   = RibbonCountG3Cool > 3,

                RibbonG3Beauty       = RibbonCountG3Beauty > 0,
                RibbonG3BeautySuper  = RibbonCountG3Beauty > 1,
                RibbonG3BeautyHyper  = RibbonCountG3Beauty > 2,
                RibbonG3BeautyMaster = RibbonCountG3Beauty > 3,

                RibbonG3Cute         = RibbonCountG3Cute > 0,
                RibbonG3CuteSuper    = RibbonCountG3Cute > 1,
                RibbonG3CuteHyper    = RibbonCountG3Cute > 2,
                RibbonG3CuteMaster   = RibbonCountG3Cute > 3,

                RibbonG3Smart        = RibbonCountG3Smart > 0,
                RibbonG3SmartSuper   = RibbonCountG3Smart > 1,
                RibbonG3SmartHyper   = RibbonCountG3Smart > 2,
                RibbonG3SmartMaster  = RibbonCountG3Smart > 3,

                RibbonG3Tough        = RibbonCountG3Tough > 0,
                RibbonG3ToughSuper   = RibbonCountG3Tough > 1,
                RibbonG3ToughHyper   = RibbonCountG3Tough > 2,
                RibbonG3ToughMaster  = RibbonCountG3Tough > 3,

                FatefulEncounter = FatefulEncounter,
            };

            // Yay for reusing string buffers! The game allocates a buffer and reuses it when creating strings.
            // Trash from the {unknown source} is currently in buffer. Set it to the Nickname region.
            var trash = StringConverter345.G4TransferTrashBytes;
            if (pk4.Language < trash.Length)
                trash[pk4.Language].CopyTo(pk4.Data, 0x48 + 4);
            pk4.Nickname = IsEgg ? SpeciesName.GetSpeciesNameGeneration(pk4.Species, pk4.Language, 4) : Nickname;
            pk4.IsNicknamed = !IsEgg && IsNicknamed;

            // Trash from the current string (Nickname) is in our string buffer. Slap the OT name over-top.
            Buffer.BlockCopy(pk4.Data, 0x48, pk4.Data, 0x68, 0x10);
            pk4.OT_Name = OT_Name;

            if (HeldItem > 0)
            {
                ushort item = ItemConverter.GetG4Item((ushort)HeldItem);
                if (ItemConverter.IsItemTransferable34(item))
                    pk4.HeldItem = item;
            }

            // Remove HM moves
            int[] newMoves = pk4.Moves;
            for (int i = 0; i < 4; i++)
            {
                if (Legal.HM_3.Contains(newMoves[i]))
                    newMoves[i] = 0;
            }

            pk4.Moves = newMoves;
            pk4.FixMoves();
            pk4.HealPP();

            pk4.RefreshChecksum();
            return pk4;
        }

        public XK3 ConvertToXK3()
        {
            var pk = ConvertTo<XK3>();
            pk.ResetPartyStats();
            return pk;
        }

        public CK3 ConvertToCK3()
        {
            var pk = ConvertTo<CK3>();
            pk.ResetPartyStats();
            return pk;
        }
    }
}
