using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Notes about the next format
    /// </summary>
    public class PB7 : PKM, IAwakened
    {
        public static readonly byte[] ExtraBytes =
        {
            0x2A, // Old Marking Value (PelagoEventStatus)
            0x36, 0x37, // Unused Ribbons
            0x58, 0x59, 0x73, 0x90, 0x91, 0x9E, 0x9F, 0xA0, 0xA1, 0xA7, 0xAA, 0xAB, 0xAC, 0xAD, 0xC8, 0xC9, 0xE4, 0xE5, 0xE6, 0xE7
        };

        public override int SIZE_PARTY => SIZE;
        public override int SIZE_STORED => SIZE;
        private const int SIZE = 260;
        public override int Format => 7;
        public override PersonalInfo PersonalInfo => PersonalTable.GG.GetFormeEntry(Species, AltForm);

        public PB7(byte[] decryptedData = null, string ident = null)
        {
            Data = decryptedData ?? new byte[SIZE];
            PKMConverter.CheckEncrypted(ref Data, 7);
            Identifier = ident;
            if (Data.Length != SIZE)
                Array.Resize(ref Data, SIZE);
        }

        public override PKM Clone() => new PB7((byte[])Data.Clone(), Identifier);

        private string GetString(int Offset, int Count) => StringConverter.GetString7(Data, Offset, Count);
        private byte[] SetString(string value, int maxLength, bool chinese = false) => StringConverter.SetString7b(value, maxLength, Language, chinese: chinese);

        protected override ushort CalculateChecksum()
        {
            ushort chk = 0;
            for (int i = 8; i < 0xE8; i += 2)
                chk += BitConverter.ToUInt16(Data, i);
            return chk;
        }

        // Trash Bytes
        public override byte[] Nickname_Trash { get => GetData(0x40, 24); set { if (value?.Length == 24) value.CopyTo(Data, 0x40); } }
        public override byte[] HT_Trash { get => GetData(0x78, 24); set { if (value?.Length == 24) value.CopyTo(Data, 0x78); } }
        public override byte[] OT_Trash { get => GetData(0xB0, 24); set { if (value?.Length == 24) value.CopyTo(Data, 0xB0); } }

        // Structure
        #region Block A
        public override uint EncryptionConstant
        {
            get => BitConverter.ToUInt32(Data, 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x00);
        }

        public override ushort Sanity
        {
            get => BitConverter.ToUInt16(Data, 0x04);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x04);
        }

        public override ushort Checksum
        {
            get => BitConverter.ToUInt16(Data, 0x06);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x06);
        }

        public override int Species
        {
            get => BitConverter.ToUInt16(Data, 0x08);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x08);
        }

        public override int HeldItem
        {
            get => BitConverter.ToUInt16(Data, 0x0A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A);
        }

        public override int TID
        {
            get => BitConverter.ToUInt16(Data, 0x0C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0C);
        }

        public override int SID
        {
            get => BitConverter.ToUInt16(Data, 0x0E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0E);
        }

        public override uint EXP
        {
            get => BitConverter.ToUInt32(Data, 0x10);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x10);
        }

        public override int Ability { get => Data[0x14]; set => Data[0x14] = (byte)value; }
        public override int AbilityNumber { get => Data[0x15] & 7; set => Data[0x15] = (byte)((Data[0x15] & ~7) | (value & 7)); }
        public bool Favorite { get => (Data[0x15] & 8) != 0; set => Data[0x15] = (byte)((Data[0x15] & ~8) | ((value ? 1 : 0) << 3)); }
        public override int MarkValue { get => BitConverter.ToUInt16(Data, 0x16); protected set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x16); }

        public override uint PID
        {
            get => BitConverter.ToUInt32(Data, 0x18);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x18);
        }

        public override int Nature { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
        public override bool FatefulEncounter { get => (Data[0x1D] & 1) == 1; set => Data[0x1D] = (byte)((Data[0x1D] & ~0x01) | (value ? 1 : 0)); }
        public override int Gender { get => (Data[0x1D] >> 1) & 0x3; set => Data[0x1D] = (byte)((Data[0x1D] & ~0x06) | (value << 1)); }
        public override int AltForm { get => Data[0x1D] >> 3; set => Data[0x1D] = (byte)((Data[0x1D] & 0x07) | (value << 3)); }
        public override int EV_HP { get => Data[0x1E]; set => Data[0x1E] = (byte)value; }
        public override int EV_ATK { get => Data[0x1F]; set => Data[0x1F] = (byte)value; }
        public override int EV_DEF { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public override int EV_SPE { get => Data[0x21]; set => Data[0x21] = (byte)value; }
        public override int EV_SPA { get => Data[0x22]; set => Data[0x22] = (byte)value; }
        public override int EV_SPD { get => Data[0x23]; set => Data[0x23] = (byte)value; }
        public int AV_HP { get => Data[0x24]; set => Data[0x24] = (byte)value; }
        public int AV_ATK { get => Data[0x25]; set => Data[0x25] = (byte)value; }
        public int AV_DEF { get => Data[0x26]; set => Data[0x26] = (byte)value; }
        public int AV_SPE { get => Data[0x27]; set => Data[0x27] = (byte)value; }
        public int AV_SPA { get => Data[0x28]; set => Data[0x28] = (byte)value; }
        public int AV_SPD { get => Data[0x29]; set => Data[0x29] = (byte)value; }
        public byte ResortEventStatus { get => Data[0x2A]; set => Data[0x2A] = value; }
        private byte PKRS { get => Data[0x2B]; set => Data[0x2B] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | value << 4); }
        public float HeightAbsolute { get => BitConverter.ToSingle(Data, 0x2C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x2C); }
        private uint RIB0 { get => BitConverter.ToUInt32(Data, 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, 0x30); }
        private uint RIB1 { get => BitConverter.ToUInt32(Data, 0x34); set => BitConverter.GetBytes(value).CopyTo(Data, 0x34); }
        public bool RibbonChampionKalos         { get => (RIB0 & (1 << 00)) == 1 << 00; set => RIB0 = ((RIB0 & ~(1u << 00)) | (value ? 1u << 00 : 0)); }
        public bool RibbonChampionG3Hoenn       { get => (RIB0 & (1 << 01)) == 1 << 01; set => RIB0 = ((RIB0 & ~(1u << 01)) | (value ? 1u << 01 : 0)); }
        public bool RibbonChampionSinnoh        { get => (RIB0 & (1 << 02)) == 1 << 02; set => RIB0 = ((RIB0 & ~(1u << 02)) | (value ? 1u << 02 : 0)); }
        public bool RibbonBestFriends           { get => (RIB0 & (1 << 03)) == 1 << 03; set => RIB0 = ((RIB0 & ~(1u << 03)) | (value ? 1u << 03 : 0)); }
        public bool RibbonTraining              { get => (RIB0 & (1 << 04)) == 1 << 04; set => RIB0 = ((RIB0 & ~(1u << 04)) | (value ? 1u << 04 : 0)); }
        public bool RibbonBattlerSkillful       { get => (RIB0 & (1 << 05)) == 1 << 05; set => RIB0 = ((RIB0 & ~(1u << 05)) | (value ? 1u << 05 : 0)); }
        public bool RibbonBattlerExpert         { get => (RIB0 & (1 << 06)) == 1 << 06; set => RIB0 = ((RIB0 & ~(1u << 06)) | (value ? 1u << 06 : 0)); }
        public bool RibbonEffort                { get => (RIB0 & (1 << 07)) == 1 << 07; set => RIB0 = ((RIB0 & ~(1u << 07)) | (value ? 1u << 07 : 0)); }
        public bool RibbonAlert                 { get => (RIB0 & (1 << 08)) == 1 << 08; set => RIB0 = ((RIB0 & ~(1u << 08)) | (value ? 1u << 08 : 0)); }
        public bool RibbonShock                 { get => (RIB0 & (1 << 09)) == 1 << 09; set => RIB0 = ((RIB0 & ~(1u << 09)) | (value ? 1u << 09 : 0)); }
        public bool RibbonDowncast              { get => (RIB0 & (1 << 10)) == 1 << 10; set => RIB0 = ((RIB0 & ~(1u << 10)) | (value ? 1u << 10 : 0)); }
        public bool RibbonCareless              { get => (RIB0 & (1 << 11)) == 1 << 11; set => RIB0 = ((RIB0 & ~(1u << 11)) | (value ? 1u << 11 : 0)); }
        public bool RibbonRelax                 { get => (RIB0 & (1 << 12)) == 1 << 12; set => RIB0 = ((RIB0 & ~(1u << 12)) | (value ? 1u << 12 : 0)); }
        public bool RibbonSnooze                { get => (RIB0 & (1 << 13)) == 1 << 13; set => RIB0 = ((RIB0 & ~(1u << 13)) | (value ? 1u << 13 : 0)); }
        public bool RibbonSmile                 { get => (RIB0 & (1 << 14)) == 1 << 14; set => RIB0 = ((RIB0 & ~(1u << 14)) | (value ? 1u << 14 : 0)); }
        public bool RibbonGorgeous              { get => (RIB0 & (1 << 15)) == 1 << 15; set => RIB0 = ((RIB0 & ~(1u << 15)) | (value ? 1u << 15 : 0)); }
        public bool RibbonRoyal                 { get => (RIB0 & (1 << 16)) == 1 << 16; set => RIB0 = ((RIB0 & ~(1u << 16)) | (value ? 1u << 16 : 0)); }
        public bool RibbonGorgeousRoyal         { get => (RIB0 & (1 << 17)) == 1 << 17; set => RIB0 = ((RIB0 & ~(1u << 17)) | (value ? 1u << 17 : 0)); }
        public bool RibbonArtist                { get => (RIB0 & (1 << 18)) == 1 << 18; set => RIB0 = ((RIB0 & ~(1u << 18)) | (value ? 1u << 18 : 0)); }
        public bool RibbonFootprint             { get => (RIB0 & (1 << 19)) == 1 << 19; set => RIB0 = ((RIB0 & ~(1u << 19)) | (value ? 1u << 19 : 0)); }
        public bool RibbonRecord                { get => (RIB0 & (1 << 20)) == 1 << 20; set => RIB0 = ((RIB0 & ~(1u << 20)) | (value ? 1u << 20 : 0)); }
        public bool RibbonLegend                { get => (RIB0 & (1 << 21)) == 1 << 21; set => RIB0 = ((RIB0 & ~(1u << 21)) | (value ? 1u << 21 : 0)); }
        public bool RibbonCountry               { get => (RIB0 & (1 << 22)) == 1 << 22; set => RIB0 = ((RIB0 & ~(1u << 22)) | (value ? 1u << 22 : 0)); }
        public bool RibbonNational              { get => (RIB0 & (1 << 23)) == 1 << 23; set => RIB0 = ((RIB0 & ~(1u << 23)) | (value ? 1u << 23 : 0)); }
        public bool RibbonEarth                 { get => (RIB0 & (1 << 24)) == 1 << 24; set => RIB0 = ((RIB0 & ~(1u << 24)) | (value ? 1u << 24 : 0)); }
        public bool RibbonWorld                 { get => (RIB0 & (1 << 25)) == 1 << 25; set => RIB0 = ((RIB0 & ~(1u << 25)) | (value ? 1u << 25 : 0)); }
        public bool RibbonClassic               { get => (RIB0 & (1 << 26)) == 1 << 26; set => RIB0 = ((RIB0 & ~(1u << 26)) | (value ? 1u << 26 : 0)); }
        public bool RibbonPremier               { get => (RIB0 & (1 << 27)) == 1 << 27; set => RIB0 = ((RIB0 & ~(1u << 27)) | (value ? 1u << 27 : 0)); }
        public bool RibbonEvent                 { get => (RIB0 & (1 << 28)) == 1 << 28; set => RIB0 = ((RIB0 & ~(1u << 28)) | (value ? 1u << 28 : 0)); }
        public bool RibbonBirthday              { get => (RIB0 & (1 << 29)) == 1 << 29; set => RIB0 = ((RIB0 & ~(1u << 29)) | (value ? 1u << 29 : 0)); }
        public bool RibbonSpecial               { get => (RIB0 & (1 << 30)) == 1 << 30; set => RIB0 = ((RIB0 & ~(1u << 30)) | (value ? 1u << 30 : 0)); }
        public bool RibbonSouvenir              { get => (RIB0 & (1 << 31)) == 1 << 31; set => RIB0 = ((RIB0 & ~(1u << 31)) | (value ? 1u << 31 : 0)); }
        public bool RibbonWishing               { get => (RIB1 & (1 << 00)) == 1 << 00; set => RIB1 = ((RIB1 & ~(1u << 00)) | (value ? 1u << 00 : 0)); }
        public bool RibbonChampionBattle        { get => (RIB1 & (1 << 01)) == 1 << 01; set => RIB1 = ((RIB1 & ~(1u << 01)) | (value ? 1u << 01 : 0)); }
        public bool RibbonChampionRegional      { get => (RIB1 & (1 << 02)) == 1 << 02; set => RIB1 = ((RIB1 & ~(1u << 02)) | (value ? 1u << 02 : 0)); }
        public bool RibbonChampionNational      { get => (RIB1 & (1 << 03)) == 1 << 03; set => RIB1 = ((RIB1 & ~(1u << 03)) | (value ? 1u << 03 : 0)); }
        public bool RibbonChampionWorld         { get => (RIB1 & (1 << 04)) == 1 << 04; set => RIB1 = ((RIB1 & ~(1u << 04)) | (value ? 1u << 04 : 0)); }
        public bool RIB4_5                      { get => (RIB1 & (1 << 05)) == 1 << 05; set => RIB1 = ((RIB1 & ~(1u << 05)) | (value ? 1u << 05 : 0)); } // Unused
        public bool RIB4_6                      { get => (RIB1 & (1 << 06)) == 1 << 06; set => RIB1 = ((RIB1 & ~(1u << 06)) | (value ? 1u << 06 : 0)); } // Unused
        public bool RibbonChampionG6Hoenn       { get => (RIB1 & (1 << 07)) == 1 << 07; set => RIB1 = ((RIB1 & ~(1u << 07)) | (value ? 1u << 07 : 0)); }
        public bool RibbonContestStar           { get => (RIB1 & (1 << 08)) == 1 << 08; set => RIB1 = ((RIB1 & ~(1u << 08)) | (value ? 1u << 08 : 0)); }
        public bool RibbonMasterCoolness        { get => (RIB1 & (1 << 09)) == 1 << 09; set => RIB1 = ((RIB1 & ~(1u << 09)) | (value ? 1u << 09 : 0)); }
        public bool RibbonMasterBeauty          { get => (RIB1 & (1 << 10)) == 1 << 10; set => RIB1 = ((RIB1 & ~(1u << 10)) | (value ? 1u << 10 : 0)); }
        public bool RibbonMasterCuteness        { get => (RIB1 & (1 << 11)) == 1 << 11; set => RIB1 = ((RIB1 & ~(1u << 11)) | (value ? 1u << 11 : 0)); }
        public bool RibbonMasterCleverness      { get => (RIB1 & (1 << 12)) == 1 << 12; set => RIB1 = ((RIB1 & ~(1u << 12)) | (value ? 1u << 12 : 0)); }
        public bool RibbonMasterToughness       { get => (RIB1 & (1 << 13)) == 1 << 13; set => RIB1 = ((RIB1 & ~(1u << 13)) | (value ? 1u << 13 : 0)); }
        public bool RibbonChampionAlola         { get => (RIB1 & (1 << 14)) == 1 << 14; set => RIB1 = ((RIB1 & ~(1u << 14)) | (value ? 1u << 14 : 0)); }
        public bool RibbonBattleRoyale          { get => (RIB1 & (1 << 15)) == 1 << 15; set => RIB1 = ((RIB1 & ~(1u << 15)) | (value ? 1u << 15 : 0)); }
        public bool RibbonBattleTreeGreat       { get => (RIB1 & (1 << 16)) == 1 << 16; set => RIB1 = ((RIB1 & ~(1u << 16)) | (value ? 1u << 16 : 0)); }
        public bool RibbonBattleTreeMaster      { get => (RIB1 & (1 << 17)) == 1 << 17; set => RIB1 = ((RIB1 & ~(1u << 17)) | (value ? 1u << 17 : 0)); }
        public bool RIB6_2                      { get => (RIB1 & (1 << 18)) == 1 << 18; set => RIB1 = ((RIB1 & ~(1u << 18)) | (value ? 1u << 18 : 0)); } // Unused
        public bool RIB6_3                      { get => (RIB1 & (1 << 19)) == 1 << 19; set => RIB1 = ((RIB1 & ~(1u << 19)) | (value ? 1u << 19 : 0)); } // Unused
        public bool RIB6_4                      { get => (RIB1 & (1 << 20)) == 1 << 20; set => RIB1 = ((RIB1 & ~(1u << 20)) | (value ? 1u << 20 : 0)); } // Unused
        public bool RIB6_5                      { get => (RIB1 & (1 << 21)) == 1 << 21; set => RIB1 = ((RIB1 & ~(1u << 21)) | (value ? 1u << 21 : 0)); } // Unused
        public bool RIB6_6                      { get => (RIB1 & (1 << 22)) == 1 << 22; set => RIB1 = ((RIB1 & ~(1u << 22)) | (value ? 1u << 22 : 0)); } // Unused
        public bool RIB6_7                      { get => (RIB1 & (1 << 23)) == 1 << 23; set => RIB1 = ((RIB1 & ~(1u << 23)) | (value ? 1u << 23 : 0)); } // Unused
        public int RibbonCountMemoryContest { get => Data[0x38]; set => Data[0x38] = (byte)value; }
        public int RibbonCountMemoryBattle { get => Data[0x39]; set => Data[0x39] = (byte)value; }
        public int HeightScalar { get => Data[0x3A]; set => Data[0x3A] = (byte)value; }
        public int WeightScalar { get => Data[0x3B]; set => Data[0x3B] = (byte)value; }
        public uint FormDuration { get => BitConverter.ToUInt32(Data, 0x3C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x3C); }
        #endregion
        #region Block B
        public override string Nickname
        {
            get => GetString(0x40, 24);
            set
            {
                if (!IsNicknamed)
                {
                    int lang = PKX.GetSpeciesNameLanguage(Species, value, 7, Language);
                    if (lang == 9 || lang == 10)
                    {
                        StringConverter.SetString7(value, 12, lang, chinese: true).CopyTo(Data, 0x40);
                        return;
                    }
                }
                SetString(value, 12).CopyTo(Data, 0x40);
            }
        }

        public override int Move1
        {
            get => BitConverter.ToUInt16(Data, 0x5A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5A);
        }

        public override int Move2
        {
            get => BitConverter.ToUInt16(Data, 0x5C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5C);
        }

        public override int Move3
        {
            get => BitConverter.ToUInt16(Data, 0x5E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5E);
        }

        public override int Move4
        {
            get => BitConverter.ToUInt16(Data, 0x60);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x60);
        }

        public override int Move1_PP { get => Data[0x62]; set => Data[0x62] = (byte)value; }
        public override int Move2_PP { get => Data[0x63]; set => Data[0x63] = (byte)value; }
        public override int Move3_PP { get => Data[0x64]; set => Data[0x64] = (byte)value; }
        public override int Move4_PP { get => Data[0x65]; set => Data[0x65] = (byte)value; }
        public override int Move1_PPUps { get => Data[0x66]; set => Data[0x66] = (byte)value; }
        public override int Move2_PPUps { get => Data[0x67]; set => Data[0x67] = (byte)value; }
        public override int Move3_PPUps { get => Data[0x68]; set => Data[0x68] = (byte)value; }
        public override int Move4_PPUps { get => Data[0x69]; set => Data[0x69] = (byte)value; }

        public override int RelearnMove1
        {
            get => BitConverter.ToUInt16(Data, 0x6A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x6A);
        }

        public override int RelearnMove2
        {
            get => BitConverter.ToUInt16(Data, 0x6C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x6C);
        }

        public override int RelearnMove3
        {
            get => BitConverter.ToUInt16(Data, 0x6E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x6E);
        }

        public override int RelearnMove4
        {
            get => BitConverter.ToUInt16(Data, 0x70);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x70);
        }

        public byte _0x72 { get => Data[0x72]; set => Data[0x72] = value; }
        public byte _0x73 { get => Data[0x73]; set => Data[0x73] = value; }
        private uint IV32 { get => BitConverter.ToUInt32(Data, 0x74); set => BitConverter.GetBytes(value).CopyTo(Data, 0x74); }
        public override int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); }
        public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); }
        public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); }
        public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); }
        public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); }
        public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); }
        public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (uint)((IV32 & ~0x40000000) | (uint)(value ? 0x40000000 : 0)); }
        public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFF) | (value ? 0x80000000 : 0); }
        #endregion
        #region Block C
        public override string HT_Name { get => GetString(0x78, 24); set => SetString(value, 12).CopyTo(Data, 0x78); }
        public override int HT_Gender { get => Data[0x92]; set => Data[0x92] = (byte)value; }
        public override int CurrentHandler { get => Data[0x93]; set => Data[0x93] = (byte)value; }
        public int Geo1_Region { get => Data[0x94]; set => Data[0x94] = (byte)value; }
        public int Geo1_Country { get => Data[0x95]; set => Data[0x95] = (byte)value; }
        public int Geo2_Region { get => Data[0x96]; set => Data[0x96] = (byte)value; }
        public int Geo2_Country { get => Data[0x97]; set => Data[0x97] = (byte)value; }
        public int Geo3_Region { get => Data[0x98]; set => Data[0x98] = (byte)value; }
        public int Geo3_Country { get => Data[0x99]; set => Data[0x99] = (byte)value; }
        public int Geo4_Region { get => Data[0x9A]; set => Data[0x9A] = (byte)value; }
        public int Geo4_Country { get => Data[0x9B]; set => Data[0x9B] = (byte)value; }
        public int Geo5_Region { get => Data[0x9C]; set => Data[0x9C] = (byte)value; }
        public int Geo5_Country { get => Data[0x9D]; set => Data[0x9D] = (byte)value; }
        public byte _0x9E { get => Data[0x9E]; set => Data[0x9E] = value; }
        public byte _0x9F { get => Data[0x9F]; set => Data[0x9F] = value; }
        public byte _0xA0 { get => Data[0xA0]; set => Data[0xA0] = value; }
        public byte _0xA1 { get => Data[0xA1]; set => Data[0xA1] = value; }
        public override int HT_Friendship { get => Data[0xA2]; set => Data[0xA2] = (byte)value; }
        public override int HT_Affection { get => Data[0xA3]; set => Data[0xA3] = (byte)value; }
        public override int HT_Intensity { get => Data[0xA4]; set => Data[0xA4] = (byte)value; }
        public override int HT_Memory { get => Data[0xA5]; set => Data[0xA5] = (byte)value; }
        public override int HT_Feeling { get => Data[0xA6]; set => Data[0xA6] = (byte)value; }
        public byte _0xA7 { get => Data[0xA7]; set => Data[0xA7] = value; }
        public override int HT_TextVar { get => BitConverter.ToUInt16(Data, 0xA8); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA8); }
        public byte _0xAA { get => Data[0xAA]; set => Data[0xAA] = value; }
        public byte _0xAB { get => Data[0xAB]; set => Data[0xAB] = value; }
        public byte FieldEventFatigue1 { get => Data[0xAC]; set => Data[0xAC] = value; }
        public byte FieldEventFatigue2 { get => Data[0xAD]; set => Data[0xAD] = value; }
        public override byte Fullness { get => Data[0xAE]; set => Data[0xAE] = value; }
        public override byte Enjoyment { get => Data[0xAF]; set => Data[0xAF] = value; }
        #endregion
        #region Block D
        public override string OT_Name { get => GetString(0xB0, 24); set => SetString(value, 12).CopyTo(Data, 0xB0); }
        public override int OT_Friendship { get => Data[0xCA]; set => Data[0xCA] = (byte)value; }
        public int _0xCB { get => Data[0xCB]; set => Data[0xCB] = (byte)value; }
        public int _0xCC { get => Data[0xCC]; set => Data[0xCC] = (byte)value; }
        public int _0xCD { get => Data[0xCD]; set => Data[0xCD] = (byte)value; }
        public int _0xCE { get => Data[0xCE]; set => Data[0xCE] = (byte)value; }
        public int _0xCF { get => Data[0xCF]; set => Data[0xCF] = (byte)value; }
        public int _0xD0 { get => Data[0xD0]; set => Data[0xD0] = (byte)value; }
        public override int Egg_Year { get => Data[0xD1]; set => Data[0xD1] = (byte)value; }
        public override int Egg_Month { get => Data[0xD2]; set => Data[0xD2] = (byte)value; }
        public override int Egg_Day { get => Data[0xD3]; set => Data[0xD3] = (byte)value; }
        public override int Met_Year { get => Data[0xD4]; set => Data[0xD4] = (byte)value; }
        public override int Met_Month { get => Data[0xD5]; set => Data[0xD5] = (byte)value; }
        public override int Met_Day { get => Data[0xD6]; set => Data[0xD6] = (byte)value; }
        public int _0xD7 { get => Data[0xD7]; set => Data[0xD7] = (byte)value; }
        public override int Egg_Location { get => BitConverter.ToUInt16(Data, 0xD8); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xD8); }
        public override int Met_Location { get => BitConverter.ToUInt16(Data, 0xDA); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDA); }
        public override int Ball { get => Data[0xDC]; set => Data[0xDC] = (byte)value; }
        public override int Met_Level { get => Data[0xDD] & ~0x80; set => Data[0xDD] = (byte)((Data[0xDD] & 0x80) | value); }
        public override int OT_Gender { get => Data[0xDD] >> 7; set => Data[0xDD] = (byte)((Data[0xDD] & ~0x80) | (value << 7)); }
        public int HyperTrainFlags { get => Data[0xDE]; set => Data[0xDE] = (byte)value; }
        public bool HT_HP { get => ((HyperTrainFlags >> 0) & 1) == 1; set => HyperTrainFlags =  (HyperTrainFlags & ~(1 << 0)) | ((value ? 1 : 0) << 0); }
        public bool HT_ATK { get => ((HyperTrainFlags >> 1) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 1)) | ((value ? 1 : 0) << 1); }
        public bool HT_DEF { get => ((HyperTrainFlags >> 2) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 2)) | ((value ? 1 : 0) << 2); }
        public bool HT_SPA { get => ((HyperTrainFlags >> 3) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 3)) | ((value ? 1 : 0) << 3); }
        public bool HT_SPD { get => ((HyperTrainFlags >> 4) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 4)) | ((value ? 1 : 0) << 4); }
        public bool HT_SPE { get => ((HyperTrainFlags >> 5) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 5)) | ((value ? 1 : 0) << 5); }
        public override int Version { get => Data[0xDF]; set => Data[0xDF] = (byte)value; }
        public int _0xE0 { get => Data[0xE0]; set => Data[0xE0] = (byte)value; }
        public int _0xE1 { get => Data[0xE1]; set => Data[0xE1] = (byte)value; }
        public int _0xE2 { get => Data[0xE2]; set => Data[0xE2] = (byte)value; }
        public override int Language { get => Data[0xE3]; set => Data[0xE3] = (byte)value; }
        public float WeightAbsolute { get => BitConverter.ToSingle(Data, 0xE4); set => BitConverter.GetBytes(value).CopyTo(Data, 0xE4); }
        #endregion
        #region Battle Stats
        public int Status { get => BitConverter.ToInt32(Data, 0xE8); set => BitConverter.GetBytes(value).CopyTo(Data, 0xE8); }
        public override int Stat_Level { get => Data[0xEC]; set => Data[0xEC] = (byte)value; }
        public byte DirtType { get => Data[0xED]; set => Data[0xED] = value; }
        public byte DirtLocation { get => Data[0xEE]; set => Data[0xEE] = value; }
        // 0xEF unused
        public override int Stat_HPCurrent { get => BitConverter.ToUInt16(Data, 0xF0); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xF0); }
        public override int Stat_HPMax { get => BitConverter.ToUInt16(Data, 0xF2); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xF2); }
        public override int Stat_ATK { get => BitConverter.ToUInt16(Data, 0xF4); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xF4); }
        public override int Stat_DEF { get => BitConverter.ToUInt16(Data, 0xF6); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xF6); }
        public override int Stat_SPE { get => BitConverter.ToUInt16(Data, 0xF8); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xF8); }
        public override int Stat_SPA { get => BitConverter.ToUInt16(Data, 0xFA); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xFA); }
        public override int Stat_SPD { get => BitConverter.ToUInt16(Data, 0xFC); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xFC); }
        public int Stat_CP { get => BitConverter.ToUInt16(Data, 0xFE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xFE); }
        public bool Stat_Mega { get => Data[0x100] != 0; set => Data[0x100] = (byte)(value ? 1 : 0); }
        public int Stat_MegaForm { get => Data[0x101]; set => Data[0x101] = (byte)value; }
        // 102/103 unused
        #endregion

        // Simple Generated Attributes
        public override int CurrentFriendship
        {
            get => CurrentHandler == 0 ? OT_Friendship : HT_Friendship;
            set { if (CurrentHandler == 0) OT_Friendship = value; else HT_Friendship = value; }
        }

        public int OppositeFriendship
        {
            get => CurrentHandler == 1 ? OT_Friendship : HT_Friendship;
            set { if (CurrentHandler == 1) OT_Friendship = value; else HT_Friendship = value; }
        }

        public override int PSV => (int)((PID >> 16 ^ (PID & 0xFFFF)) >> 4);
        public override int TSV => (TID ^ SID) >> 4;
        public bool IsUntradedEvent6 => Geo1_Country == 0 && Geo1_Region == 0 && Met_Location / 10000 == 4 && Gen6;
        public override bool IsUntraded => Data[0x78] == 0 && Data[0x78 + 1] == 0 && Format == GenNumber; // immediately terminated HT_Name data (\0)

        // Complex Generated Attributes
        public override int Characteristic
        {
            get
            {
                // Characteristic with EC%6
                int pm6 = (int)(EncryptionConstant % 6); // EC MOD 6
                int maxIV = IVs.Max();
                int pm6stat = 0;

                for (int i = 0; i < 6; i++)
                {
                    if (IVs[pm6stat = pm6++ % 6] == maxIV)
                        break;
                }

                return (pm6stat * 5) + (maxIV % 5);
            }
        }

        public override int[] Markings
        {
            get
            {
                int[] marks = new int[8];
                int val = MarkValue;
                for (int i = 0; i < marks.Length; i++)
                    marks[i] = ((val >> (i * 2)) & 3) % 3;
                return marks;
            }
            set
            {
                if (value.Length > 8)
                    return;
                int v = 0;
                for (int i = 0; i < value.Length; i++)
                    v |= (value[i] % 3) << (i * 2);
                MarkValue = v;
            }
        }

        // Methods
        protected override byte[] Encrypt()
        {
            RefreshChecksum();
            return PKX.EncryptArray(Data);
        }

        // General User-error Fixes
        public void FixRelearn()
        {
            while (true)
            {
                if (RelearnMove4 != 0 && RelearnMove3 == 0)
                {
                    RelearnMove3 = RelearnMove4;
                    RelearnMove4 = 0;
                }
                if (RelearnMove3 != 0 && RelearnMove2 == 0)
                {
                    RelearnMove2 = RelearnMove3;
                    RelearnMove3 = 0;
                    continue;
                }
                if (RelearnMove2 != 0 && RelearnMove1 == 0)
                {
                    RelearnMove1 = RelearnMove2;
                    RelearnMove2 = 0;
                    continue;
                }
                break;
            }
        }

        public void FixMemories()
        {
            if (IsUntraded)
                HT_Friendship = HT_Affection = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0;

            Geo1_Region = Geo1_Country > 0 ? Geo1_Region : 0;
            Geo2_Region = Geo2_Country > 0 ? Geo2_Region : 0;
            Geo3_Region = Geo3_Country > 0 ? Geo3_Region : 0;
            Geo4_Region = Geo4_Country > 0 ? Geo4_Region : 0;
            Geo5_Region = Geo5_Country > 0 ? Geo5_Region : 0;

            while (true)
            {
                if (Geo5_Country != 0 && Geo4_Country == 0)
                {
                    Geo4_Country = Geo5_Country;
                    Geo4_Region = Geo5_Region;
                    Geo5_Country = Geo5_Region = 0;
                }
                if (Geo4_Country != 0 && Geo3_Country == 0)
                {
                    Geo3_Country = Geo4_Country;
                    Geo3_Region = Geo4_Region;
                    Geo4_Country = Geo4_Region = 0;
                    continue;
                }
                if (Geo3_Country != 0 && Geo2_Country == 0)
                {
                    Geo2_Country = Geo3_Country;
                    Geo2_Region = Geo3_Region;
                    Geo3_Country = Geo3_Region = 0;
                    continue;
                }
                if (Geo2_Country != 0 && Geo1_Country == 0)
                {
                    Geo1_Country = Geo2_Country;
                    Geo1_Region = Geo2_Region;
                    Geo2_Country = Geo2_Region = 0;
                    continue;
                }
                break;
            }
        }

        // Synthetic Trading Logic
        public void Trade(string SAV_Trainer, int SAV_TID, int SAV_SID, int SAV_COUNTRY, int SAV_REGION, int SAV_GENDER, bool Bank, int Day = 1, int Month = 1, int Year = 2015)
        {
            // Eggs do not have any modifications done if they are traded
            if (IsEgg && !(SAV_Trainer == OT_Name && SAV_TID == TID && SAV_SID == SID && SAV_GENDER == OT_Gender))
                SetLinkTradeEgg(Day, Month, Year);
            // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
            else if (!TradeOT(SAV_Trainer, SAV_TID, SAV_SID, SAV_COUNTRY, SAV_REGION, SAV_GENDER))
                TradeHT(SAV_Trainer, SAV_COUNTRY, SAV_REGION, SAV_GENDER, Bank);
        }

        private bool TradeOT(string SAV_Trainer, int SAV_TID, int SAV_SID, int SAV_COUNTRY, int SAV_REGION, int SAV_GENDER)
        {
            // Check to see if the OT matches the SAV's OT info.
            if (!(SAV_Trainer == OT_Name && SAV_TID == TID && SAV_SID == SID && SAV_GENDER == OT_Gender))
                return false;

            CurrentHandler = 0;
            if (!IsUntraded && (SAV_COUNTRY != Geo1_Country || SAV_REGION != Geo1_Region))
                TradeGeoLocation(SAV_COUNTRY, SAV_REGION);

            return true;
        }

        private void TradeHT(string SAV_Trainer, int SAV_COUNTRY, int SAV_REGION, int SAV_GENDER, bool Bank)
        {
            if (SAV_Trainer != HT_Name || SAV_GENDER != HT_Gender || (Geo1_Country == 0 && Geo1_Region == 0 && !IsUntradedEvent6))
                TradeGeoLocation(SAV_COUNTRY, SAV_REGION);

            CurrentHandler = 1;
            if (HT_Name != SAV_Trainer)
            {
                HT_Friendship = PersonalInfo.BaseFriendship;
                HT_Affection = 0;
            }
            HT_Name = SAV_Trainer;
            HT_Gender = SAV_GENDER;

            // Make a memory if no memory already exists. Pretty terrible way of doing this but I'd rather not overwrite existing memories.
            if (HT_Memory == 0)
                TradeMemory(Bank);
        }

        // Misc Updates
        private void TradeGeoLocation(int GeoCountry, int GeoRegion)
        {
            // No geolocations are set, ever! -- except for bank. Don't set them anyway.
        }

        public void TradeMemory(bool Bank)
        {
            // no bank?
        }

        // Legality Properties
        public override bool WasLink => Met_Location == 30011;
        public override bool WasEvent => (Met_Location > 40000 && Met_Location < 50000) || FatefulEncounter;
        public override bool WasEventEgg => GenNumber < 5 ? base.WasEventEgg : ((Egg_Location > 40000 && Egg_Location < 50000) || (FatefulEncounter && Egg_Location == 30002)) && Met_Level == 1;

        // Maximums
        public override int MaxMoveID => Legal.MaxMoveID_7b;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7b;
        public override int MaxAbilityID => Legal.MaxAbilityID_7_USUM;
        public override int MaxItemID => Legal.MaxItemID_7_USUM;
        public override int MaxBallID => Legal.MaxBallID_7;
        public override int MaxGameID => Legal.MaxGameID_7;
        public override int MaxIV => 31;
        public override int MaxEV => 252;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override ushort[] GetStats(PersonalInfo p)
        {
            return CalculateStatsBeluga(p);
        }

        public ushort[] CalculateStatsBeluga(PersonalInfo p)
        {
            int level = CurrentLevel;
            int nature = Nature;
            int friend = CurrentFriendship; // stats +10% depending on friendship!
            int scalar = (int)(((friend / 255.0f / 10.0f) + 1.0f) * 100.0f);
            ushort[] Stats =
            {
                (ushort)(AV_HP  + GetStat(p.HP,  HT_HP  ? 31 : IV_HP,  level) + 10 + level),
                (ushort)(AV_ATK + (scalar * GetStat(p.ATK, HT_ATK ? 31 : IV_ATK, level, nature, 0) / 100)),
                (ushort)(AV_DEF + (scalar * GetStat(p.DEF, HT_DEF ? 31 : IV_DEF, level, nature, 1) / 100)),
                (ushort)(AV_SPE + (scalar * GetStat(p.SPE, HT_SPE ? 31 : IV_SPE, level, nature, 4) / 100)),
                (ushort)(AV_SPA + (scalar * GetStat(p.SPA, HT_SPA ? 31 : IV_SPA, level, nature, 2) / 100)),
                (ushort)(AV_SPD + (scalar * GetStat(p.SPD, HT_SPD ? 31 : IV_SPD, level, nature, 3) / 100)),
            };
            if (Species == 292)
                Stats[0] = 1;
            return Stats;
        }

        /// <summary>
        /// Gets the initial stat value based on the base stat value, IV, and current level.
        /// </summary>
        /// <param name="baseStat"><see cref="PersonalInfo"/> stat.</param>
        /// <param name="iv">Current IV, already accounted for Hyper Training</param>
        /// <param name="level">Current Level</param>
        /// <returns>Initial Stat</returns>
        private static int GetStat(int baseStat, int iv, int level) => (iv + (2 * baseStat)) * level / 100;

        /// <summary>
        /// Gets the initial stat value with nature amplification applied. Used for all stats except HP.
        /// </summary>
        /// <param name="baseStat"><see cref="PersonalInfo"/> stat.</param>
        /// <param name="iv">Current IV, already accounted for Hyper Training</param>
        /// <param name="level">Current Level</param>
        /// <param name="nature"><see cref="PKM.Nature"/></param>
        /// <param name="statIndex">Stat amp index in the nature amp table</param>
        /// <returns>Initial Stat with nature amplification applied.</returns>
        private static int GetStat(int baseStat, int iv, int level, int nature, int statIndex)
        {
            int initial = GetStat(baseStat, iv, level) + 5;
            return AmplifyStat(nature, statIndex, initial);
        }

        private static int AmplifyStat(int nature, int index, int initial)
        {
            switch (AbilityAmpTable[(5 * nature) + index])
            {
                case 1: return 110 * initial / 100; // 110%
                case -1: return 90 * initial / 100; // 90%
                default: return initial;            // 100%
            }
        }

        private static readonly sbyte[] AbilityAmpTable =
        {
            0, 0, 0, 0, 0, // Hardy
            1,-1, 0, 0, 0, // Lonely
            1, 0, 0, 0,-1, // Brave
            1, 0,-1, 0, 0, // Adamant
            1, 0, 0,-1, 0, // Naughty
           -1, 1, 0, 0, 0, // Bold
            0, 0, 0, 0, 0, // Docile
            0, 1, 0, 0,-1, // Relaxed
            0, 1,-1, 0, 0, // Impish
            0, 1, 0,-1, 0, // Lax
           -1, 0, 0, 0, 1, // Timid
            0,-1, 0, 0, 1, // Hasty
            0, 0, 0, 0, 0, // Serious
            0, 0,-1, 0, 1, // Jolly
            0, 0, 0,-1, 1, // Naive
           -1, 0, 1, 0, 0, // Modest
            0,-1, 1, 0, 0, // Mild
            0, 0, 1, 0,-1, // Quiet
            0, 0, 0, 0, 0, // Bashful
            0, 0, 1,-1, 0, // Rash
           -1, 0, 0, 1, 0, // Calm
            0,-1, 0, 1, 0, // Gentle
            0, 0, 0, 1,-1, // Sassy
            0, 0,-1, 1, 0, // Careful
            0, 0, 0, 0, 0, // Quirky
        };

        public int CalcCP => Math.Min(10000, AwakeCP + BaseCP);

        public int BaseCP
        {
            get
            {
                var p = PersonalInfo;
                int level = CurrentLevel;
                int nature = Nature;
                int friend = CurrentFriendship; // stats +10% depending on friendship!
                int scalar = (int)(((friend / 255.0f / 10.0f) + 1.0f) * 100.0f);

                // Calculate stats for all, then sum together.
                // HP is not overriden to 1 like a regular stat calc for Shedinja.
                var statSum =
                    (ushort)GetStat(p.HP, HT_HP ? 31 : IV_HP, level) + 10 + level
                    + (ushort)(GetStat(p.ATK, HT_ATK ? 31 : IV_ATK, level, nature, 0) * scalar / 100)
                    + (ushort)(GetStat(p.DEF, HT_DEF ? 31 : IV_DEF, level, nature, 1) * scalar / 100)
                    + (ushort)(GetStat(p.SPE, HT_SPE ? 31 : IV_SPE, level, nature, 4) * scalar / 100)
                    + (ushort)(GetStat(p.SPA, HT_SPA ? 31 : IV_SPA, level, nature, 2) * scalar / 100)
                    + (ushort)(GetStat(p.SPD, HT_SPD ? 31 : IV_SPD, level, nature, 3) * scalar / 100);

                return (int)((statSum * 6f * level) / 100f);
            }
        }

        public int AwakeCP
        {
            get
            {
                var sum = this.AwakeningSum(); // aHP + aATK + aDEF + aSPA + aSPD + aSPE;
                if (sum == 0)
                    return 0;
                var lvl = CurrentLevel;
                // var scalar = ((lvl * 4.0f) / 100.0f) + 2.0f;
                var scalar = ((lvl * 4.0m) / 100.0m) + 2.0m; // they don't use decimal but c# rounding mode
                return (int)(sum * scalar);
            }
        }

        // ReSharper disable RedundantCast
        // Casts are as per the game code; they may seem redundant but every bit of precision matters?
        // This still doesn't precisely match :( -- just use a tolerance check when updating.
        // If anyone can figure out how to get all precision to match, feel free to update :)
        public float HeightRatio => (float)((float)((float)(byte)HeightScalar / 255.0f) * 0.8f) + 0.6f;
        public float WeightRatio => (float)((float)((float)((float)((float)(byte) WeightScalar / 255.0f) * 0.4f) + 0.8f));

        public float CalcHeightAbsolute => HeightRatio * (float)PersonalInfo.Height;
        public float CalcWeightAbsolute => HeightRatio * (float)(WeightRatio * (float)PersonalInfo.Weight);

        public void ResetCP() => Stat_CP = CalcCP;

        public void ResetHeight()
        {
            var current = HeightAbsolute;
            var updated = CalcHeightAbsolute;
            if (Math.Abs(current - updated) > 0.0001f)
                HeightAbsolute = updated;
        }

        public void ResetWeight()
        {
            var current = WeightAbsolute;
            var updated = CalcWeightAbsolute;
            if (Math.Abs(current - updated) > 0.0001f)
                WeightAbsolute = updated;
        }

        public void ResetCalculatedValues()
        {
            ResetCP();
            ResetHeight();
            ResetWeight();
        }
    }
}