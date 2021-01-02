using System;

namespace PKHeX.Core
{
    /// <summary> Generation 2 <see cref="PKM"/> format. </summary>
    public sealed class PK2 : GBPKML, ICaughtData2
    {
        public override PersonalInfo PersonalInfo => PersonalTable.C[Species];

        public override bool Valid => Species <= 252;

        public override int SIZE_PARTY => PokeCrypto.SIZE_2PARTY;
        public override int SIZE_STORED => PokeCrypto.SIZE_2STORED;
        public override bool Korean => !Japanese && otname[0] <= 0xB;

        public override int Format => 2;

        public PK2(bool jp = false) : base(PokeCrypto.SIZE_2PARTY, jp) { }
        public PK2(byte[] decryptedData, bool jp = false) : base(EnsurePartySize(decryptedData), jp) { }

        private static byte[] EnsurePartySize(byte[] data)
        {
            if (data.Length != PokeCrypto.SIZE_2PARTY)
                Array.Resize(ref data, PokeCrypto.SIZE_2PARTY);
            return data;
        }

        public override PKM Clone() => new PK2((byte[])Data.Clone(), Japanese)
        {
            Identifier = Identifier,
            OT_Trash = otname,
            Nickname_Trash = nick,
            IsEgg = IsEgg,
        };

        protected override byte[] Encrypt() => new PokeList2(this).Write();

        #region Stored Attributes
        public override int Species { get => Data[0]; set => Data[0] = (byte)value; }
        public override int SpriteItem => ItemConverter.GetItemFuture2((byte)HeldItem);
        public override int HeldItem { get => Data[0x1]; set => Data[0x1] = (byte)value; }
        public override int Move1 { get => Data[2]; set => Data[2] = (byte)value; }
        public override int Move2 { get => Data[3]; set => Data[3] = (byte)value; }
        public override int Move3 { get => Data[4]; set => Data[4] = (byte)value; }
        public override int Move4 { get => Data[5]; set => Data[5] = (byte)value; }
        public override int TID { get => BigEndian.ToUInt16(Data, 6); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 6); }
        public override uint EXP { get => BigEndian.ToUInt32(Data, 8) >> 8; set => Array.Copy(BigEndian.GetBytes(value << 8), 0, Data, 8, 3); }
        public override int EV_HP { get => BigEndian.ToUInt16(Data, 0xB); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xB); }
        public override int EV_ATK { get => BigEndian.ToUInt16(Data, 0xD); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xD); }
        public override int EV_DEF { get => BigEndian.ToUInt16(Data, 0xF); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xF); }
        public override int EV_SPE { get => BigEndian.ToUInt16(Data, 0x11); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x11); }
        public override int EV_SPC { get => BigEndian.ToUInt16(Data, 0x13); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x13); }
        public override ushort DV16 { get => BigEndian.ToUInt16(Data, 0x15); set => BigEndian.GetBytes(value).CopyTo(Data, 0x15); }
        public override int Move1_PP { get => Data[0x17] & 0x3F; set => Data[0x17] = (byte)((Data[0x17] & 0xC0) | Math.Min(63, value)); }
        public override int Move2_PP { get => Data[0x18] & 0x3F; set => Data[0x18] = (byte)((Data[0x18] & 0xC0) | Math.Min(63, value)); }
        public override int Move3_PP { get => Data[0x19] & 0x3F; set => Data[0x19] = (byte)((Data[0x19] & 0xC0) | Math.Min(63, value)); }
        public override int Move4_PP { get => Data[0x1A] & 0x3F; set => Data[0x1A] = (byte)((Data[0x1A] & 0xC0) | Math.Min(63, value)); }
        public override int Move1_PPUps { get => (Data[0x17] & 0xC0) >> 6; set => Data[0x17] = (byte)((Data[0x17] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move2_PPUps { get => (Data[0x18] & 0xC0) >> 6; set => Data[0x18] = (byte)((Data[0x18] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move3_PPUps { get => (Data[0x19] & 0xC0) >> 6; set => Data[0x19] = (byte)((Data[0x19] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move4_PPUps { get => (Data[0x1A] & 0xC0) >> 6; set => Data[0x1A] = (byte)((Data[0x1A] & 0x3F) | ((value & 0x3) << 6)); }
        public override int CurrentFriendship { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
        private byte PKRS { get => Data[0x1C]; set => Data[0x1C] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | value << 4); }
        // Crystal only Caught Data
        public int CaughtData { get => BigEndian.ToUInt16(Data, 0x1D); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x1D); }
        public int Met_TimeOfDay { get => (CaughtData >> 14) & 0x3; set => CaughtData = (CaughtData & 0x3FFF) | ((value & 0x3) << 14); }
        public override int Met_Level { get => (CaughtData >> 8) & 0x3F; set => CaughtData = (CaughtData & 0xC0FF) | ((value & 0x3F) << 8); }
        public override int OT_Gender { get => (CaughtData >> 7) & 1; set => CaughtData = (CaughtData & 0xFF7F) | ((value & 1) << 7); }
        public override int Met_Location { get => CaughtData & 0x7F; set => CaughtData = (CaughtData & 0xFF80) | (value & 0x7F); }

        public override int Stat_Level
        {
            get => Data[0x1F];
            set => Data[0x1F] = (byte)value;
        }

        #endregion

        #region Party Attributes
        public override int Status_Condition { get => Data[0x20]; set => Data[0x20] = (byte)value; }

        public override int Stat_HPCurrent { get => BigEndian.ToUInt16(Data, 0x22); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x22); }
        public override int Stat_HPMax { get => BigEndian.ToUInt16(Data, 0x24); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public override int Stat_ATK { get => BigEndian.ToUInt16(Data, 0x26); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x26); }
        public override int Stat_DEF { get => BigEndian.ToUInt16(Data, 0x28); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x28); }
        public override int Stat_SPE { get => BigEndian.ToUInt16(Data, 0x2A); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2A); }
        public override int Stat_SPA { get => BigEndian.ToUInt16(Data, 0x2C); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2C); }
        public override int Stat_SPD { get => BigEndian.ToUInt16(Data, 0x2E); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2E); }
        #endregion

        public override bool IsEgg { get; set; }
        public override int OT_Friendship { get => CurrentFriendship; set => CurrentFriendship = value; }
        public override bool HasOriginalMetLocation => CaughtData != 0;
        public override int Version { get => (int)GameVersion.GSC; set { } }

        // Maximums
        public override int MaxMoveID => Legal.MaxMoveID_2;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_2;
        public override int MaxAbilityID => Legal.MaxAbilityID_2;
        public override int MaxItemID => Legal.MaxItemID_2;

        public PK1 ConvertToPK1()
        {
            PK1 pk1 = new(Japanese) {TradebackStatus = TradebackType.WasTradeback};
            Array.Copy(Data, 0x1, pk1.Data, 0x7, 0x1A);
            pk1.Species = Species; // This will take care of Typing :)

            var lvl = Stat_Level;
            if (lvl == 0) // no party stats (originated from box format), need to regenerate
            {
                pk1.Stat_HPCurrent = GetStat(PersonalInfo.HP, IV_ATK, EV_ATK, Stat_Level);
                pk1.Stat_Level = CurrentLevel;
            }
            else
            {
                pk1.Stat_HPCurrent = Stat_HPCurrent;
                pk1.Stat_Level = Stat_Level;
            }
            // Status = 0
            pk1.OT_Trash = otname;
            pk1.Nickname_Trash = nick;

            pk1.ClearInvalidMoves();

            return pk1;
        }

        public PK7 ConvertToPK7()
        {
            var pk7 = new PK7
            {
                EncryptionConstant = Util.Rand32(),
                Species = Species,
                TID = TID,
                CurrentLevel = CurrentLevel,
                EXP = EXP,
                Met_Level = CurrentLevel,
                Nature = Experience.GetNatureVC(EXP),
                PID = Util.Rand32(),
                Ball = 4,
                MetDate = DateTime.Now,
                Version = HasOriginalMetLocation ? (int)GameVersion.C : (int)GameVersion.SV,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                Met_Location = Locations.Transfer2, // "Johto region", hardcoded.
                Gender = Gender,
                IsNicknamed = false,
                Form = Form,

                CurrentHandler = 1,
                HT_Name = PKMConverter.OT_Name,
                HT_Gender = PKMConverter.OT_Gender,
            };
            PKMConverter.SetConsoleRegionData3DS(pk7);
            PKMConverter.SetFirstCountryRegion(pk7);
            pk7.HealPP();
            var lang = TransferLanguage(PKMConverter.Language);
            pk7.Language = lang;
            pk7.Nickname = SpeciesName.GetSpeciesNameGeneration(pk7.Species, lang, pk7.Format);
            if (otname[0] == StringConverter12.G1TradeOTCode) // In-game Trade
                pk7.OT_Name = StringConverter12.G1TradeOTName[lang];
            pk7.OT_Friendship = pk7.HT_Friendship = PersonalTable.SM[Species].BaseFriendship;

            // IVs
            var special = Species is 151 or 251;
            var new_ivs = new int[6];
            int flawless = special ? 5 : 3;
            var rnd = Util.Rand;
            for (var i = 0; i < new_ivs.Length; i++)
                new_ivs[i] = rnd.Next(32);
            for (var i = 0; i < flawless; i++)
                new_ivs[i] = 31;
            Util.Shuffle(new_ivs);
            pk7.IVs = new_ivs;

            switch (IsShiny ? Shiny.Always : Shiny.Never)
            {
                case Shiny.Always when !pk7.IsShiny: // Force Square
                    pk7.PID = (uint)(((pk7.TID ^ 0 ^ (pk7.PID & 0xFFFF) ^ 0) << 16) | (pk7.PID & 0xFFFF));
                    break;
                case Shiny.Never when pk7.IsShiny: // Force Not Shiny
                    pk7.PID ^= 0x1000_0000;
                    break;
            }

            int abil = Legal.TransferSpeciesDefaultAbilityGen2(Species) ? 0 : 2; // Hidden
            pk7.RefreshAbility(abil); // 0/1/2 (not 1/2/4)

            if (special)
            {
                pk7.FatefulEncounter = true;
            }
            else if (IsNicknamedBank)
            {
                pk7.IsNicknamed = true;
                pk7.Nickname = Korean ? Nickname : StringConverter12Transporter.GetString(nick, Japanese);
            }
            pk7.OT_Name = Korean ? OT_Name : StringConverter12Transporter.GetString(otname, Japanese);
            pk7.OT_Gender = OT_Gender; // Crystal

            pk7.SetTradeMemoryHT(bank: true); // oh no, memories on gen7 pkm

            // Dizzy Punch cannot be transferred
            {
                var index = pk7.GetMoveIndex(146); // Dizzy Punch
                if (index != -1)
                {
                    pk7.SetMove(index, 0);
                    pk7.FixMoves();
                }
            }

            pk7.RefreshChecksum();
            return pk7;
        }

        public SK2 ConvertToSK2()
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
                EV_ATK = EV_ATK,
                EV_DEF = EV_DEF,
                EV_SPE = EV_SPE,
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
                IsEgg = IsEgg,
                Stat_Level = Stat_Level,
                PKRS_Days = PKRS_Days,
                PKRS_Strain = PKRS_Strain,
                CaughtData = CaughtData,

                // Only copies until first 0x50 terminator, but just copy everything
                Nickname = Nickname,
                OT_Name = OT_Name,
            };
        }
    }
}
