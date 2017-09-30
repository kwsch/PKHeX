using System;
using System.Linq;

namespace PKHeX.Core
{
    /* Big thanks to Grovyle91's Pokémon Mystery Gift Editor, from which the structure was referenced.
     * http://projectpokemon.org/forums/member.php?829-Grovyle91
     * http://projectpokemon.org/forums/showthread.php?6524
     * See also: http://tccphreak.shiny-clique.net/debugger/pcdfiles.htm
     */
    public sealed class PCD : MysteryGift
    {
        public const int Size = 0x358; // 856
        public override int Format => 4;
        public override int Level
        {
            get => Gift.Level;
            set => Gift.Level = value;
        }
        public override int Ball
        {
            get => Gift.Ball;
            set => Gift.Ball = value;
        }

        public PCD(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[Size]);
        }

        public PGT Gift
        {
            get
            {
                if (_gift != null)
                    return _gift;
                byte[] giftData = new byte[PGT.Size];
                Array.Copy(Data, 0, giftData, 0, PGT.Size);
                return _gift = new PGT(giftData);
            }
            set => (_gift = value)?.Data.CopyTo(Data, 0);
        }
        private PGT _gift;
        public byte[] Information
        {
            get
            {
                var data = new byte[Data.Length - PGT.Size];
                Array.Copy(Data, PGT.Size, data, 0, data.Length);
                return data;
            }
            set => value?.CopyTo(Data, Data.Length - PGT.Size);
        }
        public override object Content => Gift.PK;
        public override bool GiftUsed { get => Gift.GiftUsed; set => Gift.GiftUsed = value; }
        public override bool IsPokémon { get => Gift.IsPokémon; set => Gift.IsPokémon = value; }
        public override bool IsItem { get => Gift.IsItem; set => Gift.IsItem = value; }
        public override int ItemID { get => Gift.ItemID; set => Gift.ItemID = value; }
        public override int CardID
        {
            get => BitConverter.ToUInt16(Data, 0x150);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x150);
        }
        public override string CardTitle
        {
            get => StringConverter.GetString4(Data, 0x104, 0x48);
            set
            {
                byte[] data = StringConverter.SetString4(value, 0x48/2-1, 0x48/2, 0xFFFF);
                int len = data.Length;
                Array.Resize(ref data, 0x48);
                for (int i = 0; i < len; i++)
                    data[i] = 0xFF;
                data.CopyTo(Data, 0x104);
            }
        }
        public ushort CardCompatibility => BitConverter.ToUInt16(Data, 0x14C); // rest of bytes we don't really care about

        public override int Species { get => Gift.IsManaphyEgg ? 490 : Gift.Species; set => Gift.Species = value; }
        public override int[] Moves { get => Gift.Moves; set => Gift.Moves = value; }
        public override int HeldItem { get => Gift.HeldItem; set => Gift.HeldItem = value; }
        public override bool IsShiny => Gift.IsShiny;
        public override bool IsEgg { get => Gift.IsEgg; set => Gift.IsEgg = value; }

        public bool GiftEquals(PGT pgt)
        {
            // Skip over the PGT's "Corresponding PCD Slot" @ 0x02
            byte[] g = pgt.Data;
            byte[] c = Gift.Data;
            if (g.Length != c.Length || g.Length < 3)
                return false;
            for (int i = 0; i < 2; i++)
                if (g[i] != c[i])
                    return false;
            for (int i = 3; i < g.Length; i++)
                if (g[i] != c[i])
                    return false;

            return true;
        }

        public override PKM ConvertToPKM(SaveFile SAV)
        {
            return Gift.ConvertToPKM(SAV);
        }

        public bool CanBeReceivedBy(int pkmVersion) => (CardCompatibility >> pkmVersion & 1) == 1;
    }
    public sealed class PGT : MysteryGift
    {
        public const int Size = 0x104; // 260
        public override int Format => 4;
        public override int Level
        {
            get => IsPokémon ? PK.Met_Level : 0;
            set { if (IsPokémon) PK.Met_Level = value; }
        }
        public override int Ball
        {
            get => IsPokémon ? PK.Ball : 0;
            set { if (IsPokémon) PK.Ball = value; }
        }

        private enum GiftType
        {
            Pokémon = 1,
            PokémonEgg = 2,
            Item = 3,
            Rule = 4,
            Seal = 5,
            Accessory = 6,
            ManaphyEgg = 7,
            MemberCard = 8,
            OaksLetter = 9,
            AzureFlute = 10,
            PokétchApp = 11,
            Ribbon = 12,
            PokéWalkerArea = 14
        }

        public override string CardTitle { get => "Raw Gift (PGT)"; set { } }
        public override int CardID { get => -1; set { } }
        public override bool GiftUsed { get => false; set { } }
        public override object Content => PK;

        public PGT(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[Size]);
        }

        public byte CardType { get => Data[0]; set => Data[0] = value; }
        // Unused 0x01
        public byte Slot { get => Data[2]; set => Data[2] = value; }
        public byte Detail { get => Data[3]; set => Data[3] = value; }
        public override int ItemID { get => BitConverter.ToUInt16(Data, 0x4); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x4); }

        public PK4 PK
        {
            get
            {
                if (_pk != null)
                    return _pk;
                byte[] ekdata = new byte[PKX.SIZE_4PARTY];
                Array.Copy(Data, 8, ekdata, 0, ekdata.Length);
                bool empty = ekdata.SequenceEqual(new byte[ekdata.Length]);
                return _pk = new PK4(empty ? ekdata : PKX.DecryptArray45(ekdata));
            }
            set
            {
                if ((_pk = value) == null)
                    return;

                var pkdata = value.Data.SequenceEqual(new byte[value.Data.Length])
                    ? value.Data
                    : PKX.EncryptArray45(value.Data);
                pkdata.CopyTo(Data, 8);
            }
        }
        private PK4 _pk;

        private GiftType PGTGiftType { get => (GiftType)Data[0]; set => Data[0] = (byte)value; }
        public bool IsHatched => PGTGiftType == GiftType.Pokémon;
        public override bool IsEgg { get => PGTGiftType == GiftType.PokémonEgg; set { if (value) { PGTGiftType = GiftType.PokémonEgg; PK.IsEgg = true; } } }
        public bool IsManaphyEgg { get => PGTGiftType == GiftType.ManaphyEgg; set { if (value) PGTGiftType = GiftType.ManaphyEgg; } }
        public override bool EggEncounter => IsEgg || IsManaphyEgg;
        public override bool IsItem { get => PGTGiftType == GiftType.Item; set { if (value) PGTGiftType = GiftType.Item; } }
        public override bool IsPokémon { get => PGTGiftType == GiftType.Pokémon || PGTGiftType == GiftType.PokémonEgg || PGTGiftType == GiftType.ManaphyEgg; set { } }

        public override int Species { get => IsManaphyEgg ? 490 : PK.Species; set => PK.Species = value; }
        public override int[] Moves { get => PK.Moves; set => PK.Moves = value; }
        public override int HeldItem { get => PK.HeldItem; set => PK.HeldItem = value; }
        public override bool IsShiny => PK.IsShiny;

        public override PKM ConvertToPKM(SaveFile SAV)
        {
            if (!IsPokémon)
                return null;

            PK4 pk4 = new PK4(PK.Data) {Sanity = 0};
            if (!IsHatched && Detail == 0)
            {
                pk4.OT_Name = SAV.OT;
                pk4.TID = SAV.TID;
                pk4.SID = SAV.SID;
                pk4.OT_Gender = SAV.Gender;
                pk4.Language = SAV.Language;
            }
            if (IsManaphyEgg)
            {
                // Since none of this data is populated, fill in default info.
                pk4.Species = 490;
                // Level 1 Moves
                pk4.Move1 = 294;
                pk4.Move2 = 145;
                pk4.Move3 = 346;
                pk4.Ability = pk4.PersonalInfo.Abilities[0];
                pk4.FatefulEncounter = true;
                pk4.Ball = 4;
                pk4.Version = 10; // Diamond
                pk4.Language = 2; // English
                pk4.Nickname = "MANAPHY";
                pk4.Egg_Location = 1; // Ranger (will be +3000 later)
                pk4.Move1_PP = pk4.GetMovePP(pk4.Move1, 0);
                pk4.Move2_PP = pk4.GetMovePP(pk4.Move2, 0);
                pk4.Move3_PP = pk4.GetMovePP(pk4.Move3, 0);
            }

            // Generate IV
            uint seed = Util.Rand32();
            if (pk4.PID == 1) // Create Nonshiny
            {
                uint pid1 = PKX.LCRNG(ref seed) >> 16;
                uint pid2 = PKX.LCRNG(ref seed) >> 16;

                while ((pid1 ^ pid2 ^ pk4.TID ^ pk4.SID) < 8)
                {
                    uint testPID = pid1 | pid2 << 16;

                    // Call the ARNG to change the PID
                    testPID = RNG.ARNG.Next(testPID);

                    pid1 = testPID & 0xFFFF;
                    pid2 = testPID >> 16;
                }
                pk4.PID = pid1 | (pid2 << 16);
            }
            if (!IsManaphyEgg)
                seed = Util.Rand32(); // reseed, do not have method 1 correlation

            // Generate IVs
            if (pk4.IV32 == 0)
            {
                uint iv1 = (PKX.LCRNG(ref seed) >> 16) & 0x7FFF;
                uint iv2 = (PKX.LCRNG(ref seed) >> 16) & 0x7FFF;
                pk4.IV32 = iv1 | iv2 << 15;
            }

            // Generate Met Info
            if (!IsEgg && !IsManaphyEgg)
            {
                pk4.Met_Location = pk4.Egg_Location + 3000;
                pk4.Egg_Location = 0;
                pk4.MetDate = DateTime.Now;
                pk4.IsEgg = false;
            }
            else
            {
                pk4.Egg_Location = pk4.Egg_Location + 3000;
                if (SAV.Generation == 4)
                {
                    pk4.IsEgg = true;
                    pk4.IsNicknamed = false;
                    pk4.Nickname = PKX.GetSpeciesNameGeneration(0, pk4.Language, Format);
                    pk4.MetDate = DateTime.Now;
                }
                else
                {
                    pk4.IsEgg = false;
                    // Met Location is modified when transferred to pk5; don't worry about it.
                    pk4.EggMetDate = DateTime.Now;
                }
                while (pk4.IsShiny)
                    pk4.PID = RNG.ARNG.Next(pk4.PID);
            }
            var pi = pk4.PersonalInfo;
            pk4.CurrentFriendship = pk4.IsEgg ? pi.HatchCycles : pi.BaseFriendship;
            if (pk4.Species == 201) // Never will be true; Unown was never distributed.
                pk4.AltForm = PKX.GetUnownForm(pk4.PID);

            pk4.RefreshChecksum();
            return pk4;
        }
    }
}
