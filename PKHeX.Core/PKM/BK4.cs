using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary> Generation 4 <see cref="PKM"/> format, exclusively for Pokémon Battle Revolution. </summary>
    /// <remarks>
    /// When stored in the save file, these are only shuffled; no xor encryption is performed.
    /// Values are stored in Big Endian format rather than Little Endian. Beware.
    /// </remarks>
    public sealed class BK4 : G4PKM
    {
        private static readonly ushort[] Unused =
        {
            0x42, 0x43, 0x5E, 0x63, 0x64, 0x65, 0x66, 0x67, 0x87
        };

        public override IReadOnlyList<ushort> ExtraBytes => Unused;

        public override int SIZE_PARTY => PokeCrypto.SIZE_4STORED;
        public override int SIZE_STORED => PokeCrypto.SIZE_4STORED;
        public override int Format => 4;
        public override PersonalInfo PersonalInfo => PersonalTable.HGSS[Species];

        public override byte[] DecryptedBoxData => EncryptedBoxData;

        public override bool Valid => ChecksumValid || (Sanity == 0 && Species <= MaxSpeciesID);

        public override byte[] Data { get; }

        public static BK4 ReadUnshuffle(byte[] data)
        {
            var PID = BigEndian.ToUInt32(data, 0);
            uint sv = ((PID & 0x3E000) >> 0xD) % 24;
            var Data = PokeCrypto.ShuffleArray(data, sv, PokeCrypto.SIZE_4BLOCK);
            var result = new BK4(Data);
            result.RefreshChecksum();
            return result;
        }

        public BK4(byte[] data)
        {
            Data = data;
            Sanity = 0x4000;
            ResetPartyStats();
        }

        public BK4() : this(new byte[PokeCrypto.SIZE_4STORED]) { }

        public override PKM Clone() => new BK4((byte[])Data.Clone()){Identifier = Identifier};

        public string GetString(int Offset, int Count) => StringConverter4.GetBEString4(Data, Offset, Count);
        public byte[] SetString(string value, int maxLength) => StringConverter4.SetBEString4(value, maxLength);

        // Structure
        public override uint PID { get => BigEndian.ToUInt32(Data, 0x00); set => BigEndian.GetBytes(value).CopyTo(Data, 0x00); }
        public override ushort Sanity { get => BigEndian.ToUInt16(Data, 0x04); set => BigEndian.GetBytes(value).CopyTo(Data, 0x04); }
        public override ushort Checksum { get => BigEndian.ToUInt16(Data, 0x06); set => BigEndian.GetBytes(value).CopyTo(Data, 0x06); }

        #region Block A
        public override int Species { get => BigEndian.ToUInt16(Data, 0x08); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x08); }
        public override int HeldItem { get => BigEndian.ToUInt16(Data, 0x0A); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x0A); }
        public override int SID { get => BigEndian.ToUInt16(Data, 0x0C); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x0C); }
        public override int TID { get => BigEndian.ToUInt16(Data, 0x0E); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x0E); }

        public override uint EXP
        {
            get => BigEndian.ToUInt32(Data, 0x10);
            set => BigEndian.GetBytes(value).CopyTo(Data, 0x10);
        }

        public override int OT_Friendship { get => Data[0x14]; set => Data[0x14] = (byte)value; }
        public override int Ability { get => Data[0x15]; set => Data[0x15] = (byte)value; }
        public override int MarkValue { get => Data[0x16]; protected set => Data[0x16] = (byte)value; }
        public override int Language { get => Data[0x17]; set => Data[0x17] = (byte)value; }
        public override int EV_HP { get => Data[0x18]; set => Data[0x18] = (byte)value; }
        public override int EV_ATK { get => Data[0x19]; set => Data[0x19] = (byte)value; }
        public override int EV_DEF { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }
        public override int EV_SPE { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
        public override int EV_SPA { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
        public override int EV_SPD { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
        public override int CNT_Cool { get => Data[0x1E]; set => Data[0x1E] = (byte)value; }
        public override int CNT_Beauty { get => Data[0x1F]; set => Data[0x1F] = (byte)value; }
        public override int CNT_Cute { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public override int CNT_Smart { get => Data[0x21]; set => Data[0x21] = (byte)value; }
        public override int CNT_Tough { get => Data[0x22]; set => Data[0x22] = (byte)value; }
        public override int CNT_Sheen { get => Data[0x23]; set => Data[0x23] = (byte)value; }

        private byte RIB3 { get => Data[0x24]; set => Data[0x24] = value; } // Unova 2
        private byte RIB2 { get => Data[0x25]; set => Data[0x25] = value; } // Unova 1
        private byte RIB1 { get => Data[0x26]; set => Data[0x26] = value; } // Sinnoh 2
        private byte RIB0 { get => Data[0x27]; set => Data[0x27] = value; } // Sinnoh 1
        public override bool RibbonChampionSinnoh { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonAbility { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonAbilityGreat { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonAbilityDouble { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonAbilityMulti { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonAbilityPair { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonAbilityWorld { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonAlert { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonShock { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonDowncast { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonCareless { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonRelax { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonSnooze { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonSmile { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonGorgeous { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonRoyal { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonGorgeousRoyal { get => (RIB2 & (1 << 0)) == 1 << 0; set => RIB2 = (byte)((RIB2 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonFootprint { get => (RIB2 & (1 << 1)) == 1 << 1; set => RIB2 = (byte)((RIB2 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonRecord { get => (RIB2 & (1 << 2)) == 1 << 2; set => RIB2 = (byte)((RIB2 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonEvent { get => (RIB2 & (1 << 3)) == 1 << 3; set => RIB2 = (byte)((RIB2 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonLegend { get => (RIB2 & (1 << 4)) == 1 << 4; set => RIB2 = (byte)((RIB2 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonChampionWorld { get => (RIB2 & (1 << 5)) == 1 << 5; set => RIB2 = (byte)((RIB2 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonBirthday { get => (RIB2 & (1 << 6)) == 1 << 6; set => RIB2 = (byte)((RIB2 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonSpecial { get => (RIB2 & (1 << 7)) == 1 << 7; set => RIB2 = (byte)((RIB2 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonSouvenir { get => (RIB3 & (1 << 0)) == 1 << 0; set => RIB3 = (byte)((RIB3 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonWishing { get => (RIB3 & (1 << 1)) == 1 << 1; set => RIB3 = (byte)((RIB3 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonClassic { get => (RIB3 & (1 << 2)) == 1 << 2; set => RIB3 = (byte)((RIB3 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonPremier { get => (RIB3 & (1 << 3)) == 1 << 3; set => RIB3 = (byte)((RIB3 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RIB3_4 { get => (RIB3 & (1 << 4)) == 1 << 4; set => RIB3 = (byte)((RIB3 & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
        public override bool RIB3_5 { get => (RIB3 & (1 << 5)) == 1 << 5; set => RIB3 = (byte)((RIB3 & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
        public override bool RIB3_6 { get => (RIB3 & (1 << 6)) == 1 << 6; set => RIB3 = (byte)((RIB3 & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
        public override bool RIB3_7 { get => (RIB3 & (1 << 7)) == 1 << 7; set => RIB3 = (byte)((RIB3 & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
        #endregion

        #region Block B
        public override int Move1 { get => BigEndian.ToUInt16(Data, 0x28); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x28); }
        public override int Move2 { get => BigEndian.ToUInt16(Data, 0x2A); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2A); }
        public override int Move3 { get => BigEndian.ToUInt16(Data, 0x2C); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2C); }
        public override int Move4 { get => BigEndian.ToUInt16(Data, 0x2E); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2E); }
        public override int Move1_PP { get => Data[0x30]; set => Data[0x30] = (byte)value; }
        public override int Move2_PP { get => Data[0x31]; set => Data[0x31] = (byte)value; }
        public override int Move3_PP { get => Data[0x32]; set => Data[0x32] = (byte)value; }
        public override int Move4_PP { get => Data[0x33]; set => Data[0x33] = (byte)value; }
        public override int Move1_PPUps { get => Data[0x34]; set => Data[0x34] = (byte)value; }
        public override int Move2_PPUps { get => Data[0x35]; set => Data[0x35] = (byte)value; }
        public override int Move3_PPUps { get => Data[0x36]; set => Data[0x36] = (byte)value; }
        public override int Move4_PPUps { get => Data[0x37]; set => Data[0x37] = (byte)value; }
        public uint IV32 { get => BigEndian.ToUInt32(Data, 0x38); set => BigEndian.GetBytes(value).CopyTo(Data, 0x38); }
        public override int IV_SPD { get => (int)(IV32 >> 02) & 0x1F; set => IV32 = ((IV32 & ~(0x1Fu << 02)) | ((value > 31 ? 31u : (uint)value) << 02)); }
        public override int IV_SPA { get => (int)(IV32 >> 07) & 0x1F; set => IV32 = ((IV32 & ~(0x1Fu << 07)) | ((value > 31 ? 31u : (uint)value) << 07)); }
        public override int IV_SPE { get => (int)(IV32 >> 12) & 0x1F; set => IV32 = ((IV32 & ~(0x1Fu << 12)) | ((value > 31 ? 31u : (uint)value) << 12)); }
        public override int IV_DEF { get => (int)(IV32 >> 17) & 0x1F; set => IV32 = ((IV32 & ~(0x1Fu << 17)) | ((value > 31 ? 31u : (uint)value) << 17)); }
        public override int IV_ATK { get => (int)(IV32 >> 22) & 0x1F; set => IV32 = ((IV32 & ~(0x1Fu << 22)) | ((value > 31 ? 31u : (uint)value) << 22)); }
        public override int IV_HP { get => (int)(IV32 >> 27) & 0x1F;  set => IV32 = ((IV32 & ~(0x1Fu << 27)) | ((value > 31 ? 31u : (uint)value) << 27)); }
        public override bool IsNicknamed { get => ((IV32 >> 0) & 1) == 1; set => IV32 = ((IV32 & ~0x00000001u) | (value ? 0x00000001u : 0u)); }
        public override bool IsEgg { get => ((IV32 >> 1) & 1) == 1; set => IV32 = ((IV32 & ~0x00000002u) | (value ? 0x00000002u : 0u)); }

        private byte RIB7 { get => Data[0x3C]; set => Data[0x3C] = value; } // Hoenn 2b
        private byte RIB6 { get => Data[0x3D]; set => Data[0x3D] = value; } // Hoenn 2a
        private byte RIB5 { get => Data[0x3E]; set => Data[0x3E] = value; } // Hoenn 1b
        private byte RIB4 { get => Data[0x3F]; set => Data[0x3F] = value; } // Hoenn 1a
        public override bool RibbonG3Cool { get => (RIB4 & (1 << 0)) == 1 << 0; set => RIB4 = (byte)((RIB4 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonG3CoolSuper { get => (RIB4 & (1 << 1)) == 1 << 1; set => RIB4 = (byte)((RIB4 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonG3CoolHyper { get => (RIB4 & (1 << 2)) == 1 << 2; set => RIB4 = (byte)((RIB4 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonG3CoolMaster { get => (RIB4 & (1 << 3)) == 1 << 3; set => RIB4 = (byte)((RIB4 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonG3Beauty { get => (RIB4 & (1 << 4)) == 1 << 4; set => RIB4 = (byte)((RIB4 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonG3BeautySuper { get => (RIB4 & (1 << 5)) == 1 << 5; set => RIB4 = (byte)((RIB4 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonG3BeautyHyper { get => (RIB4 & (1 << 6)) == 1 << 6; set => RIB4 = (byte)((RIB4 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonG3BeautyMaster { get => (RIB4 & (1 << 7)) == 1 << 7; set => RIB4 = (byte)((RIB4 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonG3Cute { get => (RIB5 & (1 << 0)) == 1 << 0; set => RIB5 = (byte)((RIB5 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonG3CuteSuper { get => (RIB5 & (1 << 1)) == 1 << 1; set => RIB5 = (byte)((RIB5 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonG3CuteHyper { get => (RIB5 & (1 << 2)) == 1 << 2; set => RIB5 = (byte)((RIB5 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonG3CuteMaster { get => (RIB5 & (1 << 3)) == 1 << 3; set => RIB5 = (byte)((RIB5 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonG3Smart { get => (RIB5 & (1 << 4)) == 1 << 4; set => RIB5 = (byte)((RIB5 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonG3SmartSuper { get => (RIB5 & (1 << 5)) == 1 << 5; set => RIB5 = (byte)((RIB5 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonG3SmartHyper { get => (RIB5 & (1 << 6)) == 1 << 6; set => RIB5 = (byte)((RIB5 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonG3SmartMaster { get => (RIB5 & (1 << 7)) == 1 << 7; set => RIB5 = (byte)((RIB5 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonG3Tough { get => (RIB6 & (1 << 0)) == 1 << 0; set => RIB6 = (byte)((RIB6 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonG3ToughSuper { get => (RIB6 & (1 << 1)) == 1 << 1; set => RIB6 = (byte)((RIB6 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonG3ToughHyper { get => (RIB6 & (1 << 2)) == 1 << 2; set => RIB6 = (byte)((RIB6 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonG3ToughMaster { get => (RIB6 & (1 << 3)) == 1 << 3; set => RIB6 = (byte)((RIB6 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonChampionG3Hoenn { get => (RIB6 & (1 << 4)) == 1 << 4; set => RIB6 = (byte)((RIB6 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonWinning { get => (RIB6 & (1 << 5)) == 1 << 5; set => RIB6 = (byte)((RIB6 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonVictory { get => (RIB6 & (1 << 6)) == 1 << 6; set => RIB6 = (byte)((RIB6 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonArtist { get => (RIB6 & (1 << 7)) == 1 << 7; set => RIB6 = (byte)((RIB6 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonEffort { get => (RIB7 & (1 << 0)) == 1 << 0; set => RIB7 = (byte)((RIB7 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonChampionBattle { get => (RIB7 & (1 << 1)) == 1 << 1; set => RIB7 = (byte)((RIB7 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonChampionRegional { get => (RIB7 & (1 << 2)) == 1 << 2; set => RIB7 = (byte)((RIB7 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonChampionNational { get => (RIB7 & (1 << 3)) == 1 << 3; set => RIB7 = (byte)((RIB7 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonCountry { get => (RIB7 & (1 << 4)) == 1 << 4; set => RIB7 = (byte)((RIB7 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonNational { get => (RIB7 & (1 << 5)) == 1 << 5; set => RIB7 = (byte)((RIB7 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonEarth { get => (RIB7 & (1 << 6)) == 1 << 6; set => RIB7 = (byte)((RIB7 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonWorld { get => (RIB7 & (1 << 7)) == 1 << 7; set => RIB7 = (byte)((RIB7 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

        public override bool FatefulEncounter { get => (Data[0x40] & 0x80) == 0x80; set => Data[0x40] = (byte)((Data[0x40] & ~0x80) | (value ? 0x80 : 0)); }
        public override int Gender { get => (Data[0x40] >> 5) & 0x3; set => Data[0x40] = (byte)((Data[0x40] & ~0x60) | ((value & 3) << 5)); }
        public override int AltForm { get => Data[0x40] & 0x1F; set => Data[0x40] = (byte)((Data[0x40] & ~0x1F) | (value & 0x1F)); }
        public override int ShinyLeaf { get => Data[0x41]; set => Data[0x41] = (byte)value; }
        // 0x43-0x47 Unused
        #endregion

        #region Block C
        public override string Nickname { get => GetString(0x48, 24); set => SetString(value, 11).CopyTo(Data, 0x48); }
        // 0x5E unused
        public override int Version { get => Data[0x5F]; set => Data[0x5F] = (byte)value; }
        private byte RIB8 { get => Data[0x60]; set => Data[0x60] = value; } // Sinnoh 3
        private byte RIB9 { get => Data[0x61]; set => Data[0x61] = value; } // Sinnoh 4
        private byte RIBA { get => Data[0x62]; set => Data[0x62] = value; } // Sinnoh 5
        private byte RIBB { get => Data[0x63]; set => Data[0x63] = value; } // Sinnoh 6
        public override bool RibbonG4Cool { get => (RIB8 & (1 << 0)) == 1 << 0; set => RIB8 = (byte)((RIB8 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonG4CoolGreat { get => (RIB8 & (1 << 1)) == 1 << 1; set => RIB8 = (byte)((RIB8 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonG4CoolUltra { get => (RIB8 & (1 << 2)) == 1 << 2; set => RIB8 = (byte)((RIB8 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonG4CoolMaster { get => (RIB8 & (1 << 3)) == 1 << 3; set => RIB8 = (byte)((RIB8 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonG4Beauty { get => (RIB8 & (1 << 4)) == 1 << 4; set => RIB8 = (byte)((RIB8 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonG4BeautyGreat { get => (RIB8 & (1 << 5)) == 1 << 5; set => RIB8 = (byte)((RIB8 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonG4BeautyUltra { get => (RIB8 & (1 << 6)) == 1 << 6; set => RIB8 = (byte)((RIB8 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonG4BeautyMaster { get => (RIB8 & (1 << 7)) == 1 << 7; set => RIB8 = (byte)((RIB8 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonG4Cute { get => (RIB9 & (1 << 0)) == 1 << 0; set => RIB9 = (byte)((RIB9 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonG4CuteGreat { get => (RIB9 & (1 << 1)) == 1 << 1; set => RIB9 = (byte)((RIB9 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonG4CuteUltra { get => (RIB9 & (1 << 2)) == 1 << 2; set => RIB9 = (byte)((RIB9 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonG4CuteMaster { get => (RIB9 & (1 << 3)) == 1 << 3; set => RIB9 = (byte)((RIB9 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RibbonG4Smart { get => (RIB9 & (1 << 4)) == 1 << 4; set => RIB9 = (byte)((RIB9 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public override bool RibbonG4SmartGreat { get => (RIB9 & (1 << 5)) == 1 << 5; set => RIB9 = (byte)((RIB9 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public override bool RibbonG4SmartUltra { get => (RIB9 & (1 << 6)) == 1 << 6; set => RIB9 = (byte)((RIB9 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public override bool RibbonG4SmartMaster { get => (RIB9 & (1 << 7)) == 1 << 7; set => RIB9 = (byte)((RIB9 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public override bool RibbonG4Tough { get => (RIBA & (1 << 0)) == 1 << 0; set => RIBA = (byte)((RIBA & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public override bool RibbonG4ToughGreat { get => (RIBA & (1 << 1)) == 1 << 1; set => RIBA = (byte)((RIBA & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public override bool RibbonG4ToughUltra { get => (RIBA & (1 << 2)) == 1 << 2; set => RIBA = (byte)((RIBA & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public override bool RibbonG4ToughMaster { get => (RIBA & (1 << 3)) == 1 << 3; set => RIBA = (byte)((RIBA & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public override bool RIBA_4 { get => (RIBA & (1 << 4)) == 1 << 4; set => RIBA = (byte)((RIBA & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
        public override bool RIBA_5 { get => (RIBA & (1 << 5)) == 1 << 5; set => RIBA = (byte)((RIBA & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
        public override bool RIBA_6 { get => (RIBA & (1 << 6)) == 1 << 6; set => RIBA = (byte)((RIBA & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
        public override bool RIBA_7 { get => (RIBA & (1 << 7)) == 1 << 7; set => RIBA = (byte)((RIBA & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
        public override bool RIBB_0 { get => (RIBB & (1 << 0)) == 1 << 0; set => RIBB = (byte)((RIBB & ~(1 << 0)) | (value ? 1 << 0 : 0)); } // Unused
        public override bool RIBB_1 { get => (RIBB & (1 << 1)) == 1 << 1; set => RIBB = (byte)((RIBB & ~(1 << 1)) | (value ? 1 << 1 : 0)); } // Unused
        public override bool RIBB_2 { get => (RIBB & (1 << 2)) == 1 << 2; set => RIBB = (byte)((RIBB & ~(1 << 2)) | (value ? 1 << 2 : 0)); } // Unused
        public override bool RIBB_3 { get => (RIBB & (1 << 3)) == 1 << 3; set => RIBB = (byte)((RIBB & ~(1 << 3)) | (value ? 1 << 3 : 0)); } // Unused
        public override bool RIBB_4 { get => (RIBB & (1 << 4)) == 1 << 4; set => RIBB = (byte)((RIBB & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
        public override bool RIBB_5 { get => (RIBB & (1 << 5)) == 1 << 5; set => RIBB = (byte)((RIBB & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
        public override bool RIBB_6 { get => (RIBB & (1 << 6)) == 1 << 6; set => RIBB = (byte)((RIBB & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
        public override bool RIBB_7 { get => (RIBB & (1 << 7)) == 1 << 7; set => RIBB = (byte)((RIBB & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
        // 0x64-0x67 Unused
        #endregion

        #region Block D
        public override string OT_Name { get => GetString(0x68, 16); set => SetString(value, 7).CopyTo(Data, 0x68); }
        public override int Egg_Year { get => Data[0x78]; set => Data[0x78] = (byte)value; }
        public override int Egg_Month { get => Data[0x79]; set => Data[0x79] = (byte)value; }
        public override int Egg_Day { get => Data[0x7A]; set => Data[0x7A] = (byte)value; }
        public override int Met_Year { get => Data[0x7B]; set => Data[0x7B] = (byte)value; }
        public override int Met_Month { get => Data[0x7C]; set => Data[0x7C] = (byte)value; }
        public override int Met_Day { get => Data[0x7D]; set => Data[0x7D] = (byte)value; }

        public override int Egg_Location
        {
            get
            {
                ushort hgssloc = BigEndian.ToUInt16(Data, 0x44);
                if (hgssloc != 0)
                    return hgssloc;
                return BigEndian.ToUInt16(Data, 0x7E);
            }
            set
            {
                if (value == 0)
                {
                    BigEndian.GetBytes((ushort)0).CopyTo(Data, 0x44);
                    BigEndian.GetBytes((ushort)0).CopyTo(Data, 0x7E);
                }
                else if ((value < 2000 && value > 111) || Locations.IsPtHGSSLocationEgg(value))
                {
                    // Met location not in DP, set to Faraway Place
                    BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x44);
                    BigEndian.GetBytes((ushort)0xBBA).CopyTo(Data, 0x7E);
                }
                else
                {
                    int pthgss = PtHGSS ? value : 0; // only set to PtHGSS loc if encountered in game
                    BigEndian.GetBytes((ushort)pthgss).CopyTo(Data, 0x44);
                    BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x7E);
                }
            }
        }

        public override int Met_Location
        {
            get
            {
                ushort hgssloc = BigEndian.ToUInt16(Data, 0x46);
                if (hgssloc != 0)
                    return hgssloc;
                return BigEndian.ToUInt16(Data, 0x80);
            }
            set
            {
                if (value == 0)
                {
                    BigEndian.GetBytes((ushort)0).CopyTo(Data, 0x46);
                    BigEndian.GetBytes((ushort)0).CopyTo(Data, 0x80);
                }
                else if ((value < 2000 && value > 111) || Locations.IsPtHGSSLocationEgg(value))
                {
                    // Met location not in DP, set to Faraway Place
                    BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x46);
                    BigEndian.GetBytes((ushort)0xBBA).CopyTo(Data, 0x80);
                }
                else
                {
                    int pthgss = PtHGSS ? value : 0; // only set to PtHGSS loc if encountered in game
                    BigEndian.GetBytes((ushort)pthgss).CopyTo(Data, 0x46);
                    BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x80);
                }
            }
        }

        private byte PKRS { get => Data[0x82]; set => Data[0x82] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | (value << 4)); }

        public override int Ball
        {
            get => Math.Max(Data[0x86], Data[0x83]);
            // Pokemon obtained in HGSS have the HGSS ball set (@0x86)
            // However, this info is not set when receiving a wondercard!
            // The PGT contains a preformatted PK4 file, which is slightly modified.
            // No HGSS balls were used, and no HGSS ball info is set.

            // Sneaky way = return the higher of the two values.

            set
            {
                // Ball to display in DPPt
                Data[0x83] = (byte)(value <= 0x10 ? value : 4); // Cap at Cherish Ball

                // HGSS Exclusive Balls -- If the user wants to screw things up, let them. Any legality checking could catch hax.
                if (value > 0x10 || (HGSS && !FatefulEncounter))
                    Data[0x86] = (byte)(value <= 0x18 ? value : 4); // Cap at Comp Ball
                else
                    Data[0x86] = 0; // Unused
            }
        }

        public override int Met_Level { get => Data[0x84] >> 1; set => Data[0x84] = (byte)((Data[0x84] & 0x1) | value << 1); }
        public override int OT_Gender { get => Data[0x84] & 1; set => Data[0x84] = (byte)((Data[0x84] & ~0x1) | (value & 1)); }
        public override int EncounterType { get => Data[0x85]; set => Data[0x85] = (byte)value; }
        // Unused 0x87
        #endregion

        // Not stored
        public override int Status_Condition { get; set; }
        public override int Stat_Level { get => CurrentLevel; set {} }
        public override int Stat_HPCurrent { get; set; }
        public override int Stat_HPMax { get; set; }
        public override int Stat_ATK { get; set; }
        public override int Stat_DEF { get; set; }
        public override int Stat_SPE { get; set; }
        public override int Stat_SPA { get; set; }
        public override int Stat_SPD { get; set; }

        // Methods
        protected override ushort CalculateChecksum()
        {
            ushort chk = 0;
            for (int i = 8; i < SIZE_STORED; i += 2)
                chk += BigEndian.ToUInt16(Data, i);
            return chk;
        }

        // Synthetic Trading Logic
        public bool Trade(string SAV_Trainer, int SAV_TID, int SAV_SID, int SAV_GENDER, int Day = 1, int Month = 1, int Year = 2009)
        {
            // Eggs do not have any modifications done if they are traded
            if (IsEgg && !(SAV_Trainer == OT_Name && SAV_TID == TID && SAV_SID == SID && SAV_GENDER == OT_Gender))
            {
                SetLinkTradeEgg(Day, Month, Year, Locations.LinkTrade4);
                return true;
            }
            return false;
        }

        protected override byte[] Encrypt()
        {
            RefreshChecksum();
            return PokeCrypto.ShuffleArray(Data, PokeCrypto.blockPositionInvert[((PID & 0x3E000) >> 0xD)%24], PokeCrypto.SIZE_4BLOCK);
        }

        public PK4 ConvertToPK4()
        {
            PK4 pk4 = ConvertTo<PK4>();
            pk4.RefreshChecksum();
            return pk4;
        }
    }
}