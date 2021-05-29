using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary> Generation 1 <see cref="PKM"/> format. </summary>
    public sealed class PK1 : GBPKML
    {
        public override PersonalInfo PersonalInfo => PersonalTable.Y[Species];

        public override bool Valid => Species <= 151 && (Data[0] == 0 || Species != 0);

        public override int SIZE_PARTY => PokeCrypto.SIZE_1PARTY;
        public override int SIZE_STORED => PokeCrypto.SIZE_1STORED;
        public override bool Korean => false;

        public override int Format => 1;

        public PK1(bool jp = false) : base(PokeCrypto.SIZE_1PARTY, jp) { }
        public PK1(byte[] decryptedData, bool jp = false) : base(EnsurePartySize(decryptedData), jp) { }

        private static byte[] EnsurePartySize(byte[] data)
        {
            if (data.Length != PokeCrypto.SIZE_1PARTY)
                Array.Resize(ref data, PokeCrypto.SIZE_1PARTY);
            return data;
        }

        public override PKM Clone() => new PK1((byte[])Data.Clone(), Japanese)
        {
            Identifier = Identifier,
            OT_Trash = RawOT,
            Nickname_Trash = RawNickname,
        };

        protected override byte[] Encrypt() => new PokeList1(this).Write();

        #region Stored Attributes
        public byte SpeciesID1 { get => Data[0]; set => Data[0] = value; } // raw access
        public override int Species { get => SpeciesConverter.GetG1Species(SpeciesID1); set => SetSpeciesValues(value); }
        public override int Stat_HPCurrent { get => BigEndian.ToUInt16(Data, 0x1); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x1); }
        public int Stat_LevelBox { get => Data[3]; set => Data[3] = (byte)value; }
        public override int Status_Condition { get => Data[4]; set => Data[4] = (byte)value; }
        public int Type_A { get => Data[5]; set => Data[5] = (byte)value; }
        public int Type_B { get => Data[6]; set => Data[6] = (byte)value; }
        public int Catch_Rate { get => Data[7]; set => Data[7] = (byte)value; }
        public override int Move1 { get => Data[8]; set => Data[8] = (byte)value; }
        public override int Move2 { get => Data[9]; set => Data[9] = (byte)value; }
        public override int Move3 { get => Data[10]; set => Data[10] = (byte)value; }
        public override int Move4 { get => Data[11]; set => Data[11] = (byte)value; }
        public override int TID { get => BigEndian.ToUInt16(Data, 0xC); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xC); }
        public override uint EXP { get => BigEndian.ToUInt32(Data, 0xE) >> 8; set => Array.Copy(BigEndian.GetBytes(value << 8), 0, Data, 0xE, 3); }
        public override int EV_HP { get => BigEndian.ToUInt16(Data, 0x11); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x11); }
        public override int EV_ATK { get => BigEndian.ToUInt16(Data, 0x13); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x13); }
        public override int EV_DEF { get => BigEndian.ToUInt16(Data, 0x15); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x15); }
        public override int EV_SPE { get => BigEndian.ToUInt16(Data, 0x17); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x17); }
        public override int EV_SPC { get => BigEndian.ToUInt16(Data, 0x19); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x19); }
        public override ushort DV16 { get => BigEndian.ToUInt16(Data, 0x1B); set => BigEndian.GetBytes(value).CopyTo(Data, 0x1B); }
        public override int Move1_PP { get => Data[0x1D] & 0x3F; set => Data[0x1D] = (byte)((Data[0x1D] & 0xC0) | Math.Min(63, value)); }
        public override int Move2_PP { get => Data[0x1E] & 0x3F; set => Data[0x1E] = (byte)((Data[0x1E] & 0xC0) | Math.Min(63, value)); }
        public override int Move3_PP { get => Data[0x1F] & 0x3F; set => Data[0x1F] = (byte)((Data[0x1F] & 0xC0) | Math.Min(63, value)); }
        public override int Move4_PP { get => Data[0x20] & 0x3F; set => Data[0x20] = (byte)((Data[0x20] & 0xC0) | Math.Min(63, value)); }
        public override int Move1_PPUps { get => (Data[0x1D] & 0xC0) >> 6; set => Data[0x1D] = (byte)((Data[0x1D] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move2_PPUps { get => (Data[0x1E] & 0xC0) >> 6; set => Data[0x1E] = (byte)((Data[0x1E] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move3_PPUps { get => (Data[0x1F] & 0xC0) >> 6; set => Data[0x1F] = (byte)((Data[0x1F] & 0x3F) | ((value & 0x3) << 6)); }
        public override int Move4_PPUps { get => (Data[0x20] & 0xC0) >> 6; set => Data[0x20] = (byte)((Data[0x20] & 0x3F) | ((value & 0x3) << 6)); }
        #endregion

        #region Party Attributes
        public override int Stat_Level { get => Data[0x21]; set => Stat_LevelBox = Data[0x21] = (byte)value; }
        public override int Stat_HPMax { get => BigEndian.ToUInt16(Data, 0x22); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x22); }
        public override int Stat_ATK { get => BigEndian.ToUInt16(Data, 0x24); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public override int Stat_DEF { get => BigEndian.ToUInt16(Data, 0x26); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x26); }
        public override int Stat_SPE { get => BigEndian.ToUInt16(Data, 0x28); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x28); }
        public int Stat_SPC { get => BigEndian.ToUInt16(Data, 0x2A); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x2A); }
        // Leave SPA and SPD as alias for SPC
        public override int Stat_SPA { get => Stat_SPC; set => Stat_SPC = value; }
        public override int Stat_SPD { get => Stat_SPC; set { } }
        #endregion

        private static bool IsCatchRateHeldItem(int rate) => ParseSettings.AllowGen1Tradeback && Array.IndexOf(Legal.HeldItems_GSC, (ushort)rate) >= 0;

        private void SetSpeciesValues(int value)
        {
            var updated = SpeciesConverter.SetG1Species(value);
            if (SpeciesID1 == updated)
                return;

            SpeciesID1 = updated;

            Type_A = PersonalInfo.Type1;
            Type_B = PersonalInfo.Type2;

            // Before updating catch rate, check if non-standard
            if (TradebackStatus == TradebackType.WasTradeback)
                return;
            if (IsCatchRateHeldItem(Catch_Rate))
                return;
            if (value == (int)Core.Species.Pikachu && Catch_Rate == 0xA3) // Light Ball (starter)
                return;

            int Rate = Catch_Rate;
            int baseSpecies = EvoBase.GetBaseSpecies(this).Species;
            for (int z = baseSpecies; z <= value; z++)
            {
                if (Rate == PersonalTable.RB[z].CatchRate || Rate == PersonalTable.Y[z].CatchRate)
                    return;
            }
            Catch_Rate = PersonalTable.RB[value].CatchRate;
        }

        public override int Version { get => (int)GameVersion.RBY; set { } }
        public override int PKRS_Strain { get => 0; set { } }
        public override int PKRS_Days { get => 0; set { } }
        public override bool CanHoldItem(IReadOnlyList<ushort> valid) => false;
        public override int Met_Location { get => 0; set { } }
        public override int OT_Gender { get => 0; set { } }
        public override int Met_Level { get => 0; set { } }
        public override int CurrentFriendship { get => 0; set { } }
        public override bool IsEgg { get => false; set { } }
        public override int HeldItem { get => 0; set { } }
        public override int OT_Friendship { get => 0; set { } }

        // Maximums
        public override int MaxMoveID => Legal.MaxMoveID_1;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_1;
        public override int MaxAbilityID => Legal.MaxAbilityID_1;
        public override int MaxItemID => Legal.MaxItemID_1;

        // Extra
        public int Gen2Item => ItemConverter.GetItemFuture1(Catch_Rate);

        public PK2 ConvertToPK2()
        {
            PK2 pk2 = new(Japanese) {Species = Species};
            Array.Copy(Data, 0x7, pk2.Data, 0x1, 0x1A);
            RawOT.CopyTo(pk2.RawOT, 0);
            RawNickname.CopyTo(pk2.RawNickname, 0);

            pk2.HeldItem = Gen2Item;
            pk2.CurrentFriendship = pk2.PersonalInfo.BaseFriendship;
            pk2.Stat_Level = CurrentLevel;

            return pk2;
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
                Version = (int)GameVersion.RD, // Default to red
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                Met_Location = Locations.Transfer1, // "Kanto region", hardcoded.
                Gender = Gender,
                OT_Name = StringConverter12Transporter.GetString(RawOT, Japanese),
                IsNicknamed = false,

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
            if (RawOT[0] == StringConverter12.G1TradeOTCode) // In-game Trade
                pk7.OT_Name = StringConverter12.G1TradeOTName[lang];
            pk7.OT_Friendship = pk7.HT_Friendship = PersonalTable.SM[Species].BaseFriendship;

            // IVs
            var new_ivs = new int[6];
            int flawless = Species == (int)Core.Species.Mew ? 5 : 3;
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

            int abil = Legal.TransferSpeciesDefaultAbilityGen1(Species) ? 0 : 2; // Hidden
            pk7.RefreshAbility(abil); // 0/1/2 (not 1/2/4)

            if (Species == (int)Core.Species.Mew) // Mew gets special treatment.
            {
                pk7.FatefulEncounter = true;
            }
            else if (IsNicknamedBank)
            {
                pk7.IsNicknamed = true;
                pk7.Nickname = StringConverter12Transporter.GetString(RawNickname, Japanese);
            }

            pk7.SetTradeMemoryHT(bank:true); // oh no, memories on gen7 pkm

            pk7.RefreshChecksum();
            return pk7;
        }
    }
}
