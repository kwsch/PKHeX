using System;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class SAV2 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {PlayTimeString}].bak";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";
        public override string[] PKMExtensions => PKM.Extensions.Where(f => 
        {
            int gen = f.Last() - 0x30;
            if (Korean)
                return gen == 2;
            return 1 <= gen && gen <= 2;
        }).ToArray();

        public SAV2(byte[] data = null, GameVersion versionOverride = GameVersion.Any)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G2RAW_U] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            if (data == null)
                Version = GameVersion.C;
            else if (versionOverride != GameVersion.Any)
                Version = versionOverride;
            else
                Version = SaveUtil.GetIsG2SAV(Data);

            if (Version == GameVersion.Invalid)
                return;

            Japanese = SaveUtil.GetIsG2SAVJ(Data) != GameVersion.Invalid;
            if (Japanese && Data.Length < SaveUtil.SIZE_G2RAW_J)
                Array.Resize(ref Data, SaveUtil.SIZE_G2RAW_J);
            if (!Japanese)
                Korean = SaveUtil.GetIsG2SAVK(Data) != GameVersion.Invalid;

            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED);
            Party = GetPartyOffset(0);
            
            Personal = Version == GameVersion.GS ? PersonalTable.GS : PersonalTable.C;

            Offsets = new SAV2Offsets(this);

            LegalItems = Legal.Pouch_Items_GSC;
            LegalBalls = Legal.Pouch_Ball_GSC;
            LegalKeyItems = Version == GameVersion.C ? Legal.Pouch_Key_C : Legal.Pouch_Key_GS;
            LegalTMHMs = Legal.Pouch_TMHM_GSC;
            HeldItems = Legal.HeldItems_GSC;
            
            // Stash boxes after the save file's end.
            byte[] TempBox = new byte[SIZE_STOREDBOX];
            for (int i = 0; i < BoxCount; i++)
            {
                if (i < (Japanese ? 6 : 7))
                    Array.Copy(Data, 0x4000 + i * (TempBox.Length + 2), TempBox, 0, TempBox.Length);
                else
                    Array.Copy(Data, 0x6000 + (i - (Japanese ? 6 : 7)) * (TempBox.Length + 2), TempBox, 0, TempBox.Length);
                PokemonList2 PL2 = new PokemonList2(TempBox, Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);
                for (int j = 0; j < PL2.Pokemon.Length; j++)
                {
                    if (j < PL2.Count)
                    {
                        byte[] pkDat = new PokemonList2(PL2[j]).GetBytes();
                        pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + i * SIZE_BOX + j * SIZE_STORED);
                    }
                    else
                    {
                        byte[] pkDat = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Single, Japanese)];
                        pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + i * SIZE_BOX + j * SIZE_STORED);
                    }
                }
            }

            Array.Copy(Data, Offsets.CurrentBox, TempBox, 0, TempBox.Length);
            PokemonList2 curBoxPL = new PokemonList2(TempBox, Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);
            for (int i = 0; i < curBoxPL.Pokemon.Length; i++)
            {
                if (i < curBoxPL.Count)
                {
                    byte[] pkDat = new PokemonList2(curBoxPL[i]).GetBytes();
                    pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + CurrentBox * SIZE_BOX + i * SIZE_STORED);
                }
                else
                {
                    byte[] pkDat = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Single, Japanese)];
                    pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + CurrentBox * SIZE_BOX + i * SIZE_STORED);
                }
            }

            byte[] TempParty = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Party, Japanese)];
            Array.Copy(Data, Offsets.Party, TempParty, 0, TempParty.Length);
            PokemonList2 partyList = new PokemonList2(TempParty, PokemonList2.CapacityType.Party, Japanese);
            for (int i = 0; i < partyList.Pokemon.Length; i++)
            {
                if (i < partyList.Count)
                {
                    byte[] pkDat = new PokemonList2(partyList[i]).GetBytes();
                    pkDat.CopyTo(Data, GetPartyOffset(i));
                }
                else
                {
                    byte[] pkDat = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Single, Japanese)];
                    pkDat.CopyTo(Data, GetPartyOffset(i));
                }
            }

            // Daycare currently undocumented for all Gen II games.

            // Enable Pokedex editing
            PokeDex = 0;

            if (!Exportable)
                ClearBoxes();
        }

        private const int SIZE_RESERVED = 0x8000; // unpacked box data
        public bool Korean { get; }
        private readonly SAV2Offsets Offsets;

        protected override byte[] Write(bool DSV)
        {
            for (int i = 0; i < BoxCount; i++)
            {
                PokemonList2 boxPL = new PokemonList2(Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);
                int slot = 0;
                for (int j = 0; j < boxPL.Pokemon.Length; j++)
                {
                    PK2 boxPK = (PK2) GetPKM(GetData(GetBoxOffset(i) + j*SIZE_STORED, SIZE_STORED));
                    if (boxPK.Species > 0)
                        boxPL[slot++] = boxPK;
                }
                if (i < (Japanese ? 6 : 7))
                    boxPL.GetBytes().CopyTo(Data, 0x4000 + i * (SIZE_STOREDBOX + 2));
                else
                    boxPL.GetBytes().CopyTo(Data, 0x6000 + (i - (Japanese ? 6 : 7)) * (SIZE_STOREDBOX + 2));
                if (i == CurrentBox)
                    boxPL.GetBytes().CopyTo(Data, Offsets.CurrentBox);
            }

            PokemonList2 partyPL = new PokemonList2(PokemonList2.CapacityType.Party, Japanese);
            int pSlot = 0;
            for (int i = 0; i < 6; i++)
            {
                PK2 partyPK = (PK2)GetPKM(GetData(GetPartyOffset(i), SIZE_STORED));
                if (partyPK.Species > 0)
                    partyPL[pSlot++] = partyPK;
            }
            partyPL.GetBytes().CopyTo(Data, Offsets.Party);

            SetChecksums();
            if (Japanese)
            {
                switch (Version)
                {
                    case GameVersion.GS: Array.Copy(Data, Offsets.Trainer1, Data, 0x7209, 0xC83); break;
                    case GameVersion.C:  Array.Copy(Data, Offsets.Trainer1, Data, 0x7209, 0xADA); break;
                }
            }
            else if (Korean)
            {
                // Calculate oddball checksum
                ushort sum = 0;
                ushort[][] offsetpairs =
                {
                    new ushort[] {0x106B, 0x1533},
                    new ushort[] {0x1534, 0x1A12},
                    new ushort[] {0x1A13, 0x1C38},
                    new ushort[] {0x3DD8, 0x3F79},
                    new ushort[] {0x7E39, 0x7E6A},
                };
                foreach (ushort[] p in offsetpairs)
                    for (int i = p[0]; i < p[1]; i++)
                        sum += Data[i];
                BitConverter.GetBytes(sum).CopyTo(Data, 0x7E6B);
            }
            else
            {
                switch (Version)
                {
                    case GameVersion.GS:
                        Array.Copy(Data, 0x2009, Data, 0x15C7, 0x222F - 0x2009);
                        Array.Copy(Data, 0x222F, Data, 0x3D69, 0x23D9 - 0x222F);
                        Array.Copy(Data, 0x23D9, Data, 0x0C6B, 0x2856 - 0x23D9);
                        Array.Copy(Data, 0x2856, Data, 0x7E39, 0x288A - 0x2856);
                        Array.Copy(Data, 0x288A, Data, 0x10E8, 0x2D69 - 0x288A);
                        break;
                    case GameVersion.C:
                        Array.Copy(Data, 0x2009, Data, 0x1209, 0xB7A);
                        break;
                }
            }
            byte[] outData = new byte[Data.Length - SIZE_RESERVED];
            Array.Copy(Data, outData, outData.Length);
            return outData;
        }

        // Configuration
        public override SaveFile Clone() { return new SAV2(Write(DSV: false)); }

        public override int SIZE_STORED => Japanese ? PKX.SIZE_2JLIST : PKX.SIZE_2ULIST;
        protected override int SIZE_PARTY => Japanese ? PKX.SIZE_2JLIST : PKX.SIZE_2ULIST;
        public override PKM BlankPKM => new PK2(null, null, Japanese);
        public override Type PKMType => typeof(PK2);

        private int SIZE_BOX => BoxSlotCount*SIZE_STORED;
        private int SIZE_STOREDBOX => PokemonList2.GetDataLength(Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);

        public override int MaxMoveID => Legal.MaxMoveID_2;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_2;
        public override int MaxAbilityID => Legal.MaxAbilityID_2;
        public override int MaxItemID => Legal.MaxItemID_2;
        public override int MaxBallID => 0; // unused
        public override int MaxGameID => 99; // unused
        public override int MaxMoney => 999999;
        public override int MaxCoins => 9999;

        public override int BoxCount => Japanese ? 9 : 14;
        public override int MaxEV => 65535;
        public override int MaxIV => 15;
        public override int Generation => 2;
        protected override int GiftCountMax => 0;
        public override int OTLength => Japanese ? 5 : 7;
        public override int NickLength => Japanese ? 5 : 10;
        public override int BoxSlotCount => Japanese ? 30 : 20;

        public override bool HasParty => true;
        
        // Checksums
        private ushort GetChecksum()
        {
            return (ushort)Data.Skip(Offsets.Trainer1).Take(Offsets.AccumulatedChecksumEnd - Offsets.Trainer1 + 1).Sum(a => a);
        }
        protected override void SetChecksums()
        {
            ushort accum = GetChecksum();
            BitConverter.GetBytes(accum).CopyTo(Data, Offsets.OverallChecksumPosition);
            BitConverter.GetBytes(accum).CopyTo(Data, Offsets.OverallChecksumPosition2);
        }
        public override bool ChecksumsValid
        {
            get
            {
                ushort accum = GetChecksum();
                ushort actual = BitConverter.ToUInt16(Data, Offsets.OverallChecksumPosition);
                return accum == actual;
            }
        }

        public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";

        // Trainer Info
        public override GameVersion Version { get; protected set; }

        public override string OT
        {
            get => GetString(Offsets.Trainer1 + 2, OTLength);
            set => SetString(value, OTLength).CopyTo(Data, Offsets.Trainer1 + 2);
        }
        public override int Gender
        {
            get => Version == GameVersion.C ? Data[Offsets.Gender] : 0;
            set
            {
                if (Version != GameVersion.C)
                    return;
                Data[Offsets.Gender] = (byte) value;
                Data[Offsets.Palette] = (byte) value;
            }
        }
        public override ushort TID
        {
            get => BigEndian.ToUInt16(Data, Offsets.Trainer1); set => BigEndian.GetBytes(value).CopyTo(Data, Offsets.Trainer1);
        }
        public override ushort SID
        {
            get => 0;
            set { }
        }
        public override int PlayedHours
        {
            get => BigEndian.ToUInt16(Data, Offsets.TimePlayed);
            set => BigEndian.GetBytes((ushort)value).CopyTo(Data, Offsets.TimePlayed);
        }
        public override int PlayedMinutes
        {
            get => Data[Offsets.TimePlayed + 2];
            set => Data[Offsets.TimePlayed + 2] = (byte)value;
        }
        public override int PlayedSeconds
        {
            get => Data[Offsets.TimePlayed + 3];
            set => Data[Offsets.TimePlayed + 3] = (byte)value;
        }

        public int Badges
        {
            get => BitConverter.ToUInt16(Data, Offsets.JohtoBadges);
            set { if (value < 0) return; BitConverter.GetBytes(value).CopyTo(Data, Offsets.JohtoBadges); }
        }
        private byte Options
        {
            get => Data[Offsets.Options];
            set => Data[Offsets.Options] = value;
        }
        public bool BattleEffects
        {
            get => (Options & 0x80) == 0;
            set => Options = (byte)((Options & 0x7F) | (value ? 0 : 0x80));
        }
        public bool BattleStyleSwitch
        {
            get => (Options & 0x40) == 0;
            set => Options = (byte)((Options & 0xBF) | (value ? 0 : 0x40));
        }
        public int Sound
        {
            get => (Options & 0x30) >> 4;
            set
            {
                var new_sound = value;
                if (new_sound > 0)
                    new_sound = 2; // Stereo
                if (new_sound < 0)
                    new_sound = 0; // Mono
                Options = (byte)((Options & 0xCF) | (new_sound << 4));
            }
        }
        public int TextSpeed
        {
            get => Options & 0x7; set
            {
                var new_speed = value;
                if (new_speed > 7)
                    new_speed = 7;
                if (new_speed < 0)
                    new_speed = 0;
                Options = (byte)((Options & 0xF8) | new_speed);
            }
        }
        public override uint Money
        {
            get => BigEndian.ToUInt32(Data, Offsets.Money - 1) & 0xFFFFFF;
            set
            {
                byte[] data = BigEndian.GetBytes((uint) Math.Min(value, MaxMoney));
                Array.Copy(data, 1, Data, Offsets.Money, 3);
            }
        }
        public uint Coin
        {
            get => BigEndian.ToUInt16(Data, Offsets.Money + 7);
            set
            {
                value = (ushort)Math.Min(value, MaxCoins);
                BigEndian.GetBytes((ushort)value).CopyTo(Data, Offsets.Money + 7);
            }
        }

        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs;
        public override InventoryPouch[] Inventory
        {
            get
            {
                var pouch = new[]
                {
                    new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 99, Offsets.PouchTMHM, 57),
                    new InventoryPouch(InventoryType.Items, LegalItems, 99, Offsets.PouchItem, 20),
                    new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 99, Offsets.PouchKey, 26),
                    new InventoryPouch(InventoryType.Balls, LegalBalls, 99, Offsets.PouchBall, 12),
                    new InventoryPouch(InventoryType.PCItems, LegalItems.Concat(LegalKeyItems).Concat(LegalBalls).Concat(LegalTMHMs).ToArray(), 99, Offsets.PouchPC, 50)
                };
                foreach (var p in pouch)
                {
                    p.GetPouchG1(ref Data);
                }
                return pouch;
            }
            set
            {
                foreach (var p in value)
                {
                    int ofs = 0;
                    for (int i = 0; i < p.Count; i++)
                    {
                        while (p.Items[ofs].Count == 0)
                            ofs++;
                        p.Items[i] = p.Items[ofs++];
                    }
                    while (ofs < p.Items.Length)
                        p.Items[ofs++] = new InventoryItem { Count = 0, Index = 0 };
                    p.SetPouchG1(ref Data);
                }
            }
        }
        public override int GetDaycareSlotOffset(int loc, int slot)
        {
            return Daycare;
        }
        public override uint? GetDaycareEXP(int loc, int slot)
        {
            return null;
        }
        public override bool? IsDaycareOccupied(int loc, int slot)
        {
            return null;
        }
        public override void SetDaycareEXP(int loc, int slot, uint EXP)
        {

        }
        public override void SetDaycareOccupied(int loc, int slot, bool occupied)
        {

        }

        // Storage
        public override int PartyCount
        {
            get => Data[Offsets.Party]; protected set => Data[Offsets.Party] = (byte)value;
        }
        public override int GetBoxOffset(int box)
        {
            return Data.Length - SIZE_RESERVED + box * SIZE_BOX;
        }
        public override int GetPartyOffset(int slot)
        {
            return Data.Length - SIZE_RESERVED + BoxCount * SIZE_BOX + slot * SIZE_STORED;
        }
        public override int CurrentBox
        {
            get => Data[Offsets.CurrentBoxIndex] & 0x7F; set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.OtherCurrentBox] & 0x80) | (value & 0x7F));
        }

        public override string GetBoxName(int box)
        {
            int len = Korean ? 17 : 9;
            return GetString(Offsets.BoxNames + box * len, len);
        }
        public override void SetBoxName(int box, string value)
        {
            // Don't allow for custom box names
        }

        public override PKM GetPKM(byte[] data)
        {
            if (data.Length == SIZE_STORED)
                return new PokemonList2(data, PokemonList2.CapacityType.Single, Japanese)[0];
            return new PK2(data);
        }
        public override byte[] DecryptPKM(byte[] data)
        {
            return data;
        }

        // Pokédex
        protected override void SetDex(PKM pkm)
        {
            int species = pkm.Species;
            if (!CanSetDex(species))
                return;

            SetCaught(pkm.Species, true);
            SetSeen(pkm.Species, true);
        }
        private bool CanSetDex(int species)
        {
            if (species <= 0)
                return false;
            if (species > MaxSpeciesID)
                return false;
            if (Version == GameVersion.Unknown)
                return false;
            return true;
        }
        public override void SetSeen(int species, bool seen)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));

            if (seen)
                Data[Offsets.PokedexSeen + ofs] |= bitval;
            else
                Data[Offsets.PokedexSeen + ofs] &= (byte)~bitval;
        }
        public override void SetCaught(int species, bool caught)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));

            if (!caught)
            {
                // Clear the Captured Flag
                Data[Offsets.PokedexCaught + ofs] &= (byte)~bitval;
                return;
            }

            // Set the Captured Flag
            Data[Offsets.PokedexCaught + ofs] |= bitval;
            if (species == 201)
                SetUnownFormFlags();
        }
        private void SetUnownFormFlags()
        {
            // Give all Unown caught to prevent a crash on pokedex view
            for (int i = 1; i <= 26; i++)
                Data[Offsets.PokedexSeen + 0x1F + i] = (byte)i;
        }
        public override bool GetSeen(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));
            // Get the Seen Flag
            return (Data[Offsets.PokedexSeen + ofs] & bitval) != 0;
        }
        public override bool GetCaught(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));
            // Get the Caught Flag
            return (Data[Offsets.PokedexCaught + ofs] & bitval) != 0;
        }

        public override string GetString(int Offset, int Count)
        {
            if (Korean)
                return StringConverter.GetString2KOR(Data, Offset, Count);
            return StringConverter.GetString1(Data, Offset, Count, Japanese);
        }
        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (Korean)
                return StringConverter.SetString2KOR(value, maxLength);
            return StringConverter.SetString1(value, maxLength, Japanese);
        }
    }
}
