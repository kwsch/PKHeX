using System;
using System.Linq;

namespace PKHeX
{
    /* Big thanks to Grovyle91's Pokémon Mystery Gift Editor, from which the structure was referenced.
     * http://projectpokemon.org/forums/member.php?829-Grovyle91
     * http://projectpokemon.org/forums/showthread.php?6524
     * See also: http://tccphreak.shiny-clique.net/debugger/pcdfiles.htm
     */
    public sealed class PCD : MysteryGift
    {
        internal const int Size = 0x358; // 856
        public override int Format => 4;
        public override int Level
        {
            get { return Gift.Level; }
            set { Gift.Level = value; }
        }
        public override int Ball
        {
            get { return Gift.Ball; }
            set { Gift.Ball = value; }
        }

        public PCD(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[Size]);

            byte[] giftData = new byte[PGT.Size];
            Array.Copy(Data, 0, giftData, 0, PGT.Size);
            Gift = new PGT(giftData);

            Information = new byte[Data.Length - PGT.Size];
            Array.Copy(Data, PGT.Size, Information, 0, Information.Length);
        }
        public readonly PGT Gift;
        public override object Content => Gift.PK;

        public readonly byte[] Information;
        public override bool GiftUsed { get { return Gift.GiftUsed; } set { Gift.GiftUsed = value; } }
        public override bool IsPokémon { get { return Gift.IsPokémon; } set { Gift.IsPokémon = value; } }
        public override bool IsItem { get { return Gift.IsItem; } set { Gift.IsItem = value; } }
        public override int Item { get { return Gift.Item; } set { Gift.Item = value; } }
        public override int CardID
        {
            get { return BitConverter.ToUInt16(Data, 0x150); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x150); }
        }
        public override string CardTitle
        {
            get { return PKX.array2strG4(Data.Skip(0x104).Take(0x48).ToArray()); }
            set
            {
                byte[] data = PKX.str2arrayG4(value);
                int len = data.Length;
                Array.Resize(ref data, 0x48);
                for (int i = 0; i < len; i++)
                    data[i] = 0xFF;
                data.CopyTo(Data, 0x104);
            }
        }

        public override int Species => Gift.PK.Species;
        public override int[] Moves => Gift.PK.Moves;
        public override bool IsShiny => Gift.PK.IsShiny;
        public override bool IsEgg => Gift.PK.IsEgg;
        public override int HeldItem => Gift.PK.HeldItem;

        public override PKM convertToPKM(SaveFile SAV)
        {
            return Gift.convertToPKM(SAV);
        }
    }
    public class PGT : MysteryGift
    {
        internal const int Size = 0x104; // 260
        public override int Format => 4;
        public override int Level
        {
            get { return IsPokémon ? PK.Met_Level : 0; }
            set { if (IsPokémon) PK.Met_Level = value; }
        }
        public override int Ball
        {
            get { return IsPokémon ? PK.Ball : 0; }
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

        public override string CardTitle { get { return "Raw Gift (PGT)"; } set { } }
        public override int CardID { get { return -1; } set { } }
        public override bool GiftUsed { get { return false; } set { } }
        public override object Content => PK;

        public PGT(byte[] data = null)
        {
            refreshData((byte[])(data?.Clone() ?? new byte[Size]));
        }

        private void refreshData(byte[] data)
        {
            byte[] ekdata = new byte[PKX.SIZE_4PARTY];
            Array.Copy(data, 8, ekdata, 0, ekdata.Length);
            bool empty = ekdata.SequenceEqual(new byte[ekdata.Length]);
            PK = new PK4(empty ? ekdata : PKX.decryptArray45(ekdata));

            Unknown = new byte[0x10];
            Array.Copy(data, 0xF4, Unknown, 0, 0x10);
            RAW = data;
        }

        public byte CardType { get { return Data[0]; } set { Data[0] = value; } }
        // Unused 0x01
        public byte Slot { get { return Data[2]; } set { Data[2] = value; } }
        public byte Detail { get { return Data[3]; } set { Data[3] = value; } }
        public override int Item { get { return BitConverter.ToUInt16(Data, 0x4); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x4); } }
        public PK4 PK;
        private byte[] Unknown;
        private byte[] RAW;

        public override byte[] Data
        {
            get { return RAW; }
            set
            {
                refreshData(value);
            }
        }

        private GiftType PGTGiftType { get { return (GiftType)Data[0]; } set {Data[0] = (byte)value; } }
        public bool IsHatched => PGTGiftType == GiftType.Pokémon;
        public override bool IsEgg => PGTGiftType == GiftType.PokémonEgg;
        public bool IsManaphyEgg => PGTGiftType == GiftType.ManaphyEgg;
        public override bool IsItem { get { return PGTGiftType == GiftType.Item; } set { if (value) CardType = (int)GiftType.Item; } }
        public override bool IsPokémon { get { return PGTGiftType == GiftType.Pokémon || PGTGiftType == GiftType.PokémonEgg || PGTGiftType == GiftType.ManaphyEgg; } set { } }

        public override int Species => PK.Species;
        public override int[] Moves => PK.Moves;
        public override bool IsShiny => PK.IsShiny;
        public override int HeldItem => PK.HeldItem;

        public override PKM convertToPKM(SaveFile SAV)
        {
            if (!IsPokémon)
                return null;

            PK4 pk4 = new PK4(PK.Data);
            if (!IsHatched && Detail == 0)
            {
                pk4.OT_Name = SAV.OT;
                pk4.TID = SAV.TID;
                pk4.SID = SAV.SID;
                pk4.OT_Gender = SAV.Gender;
            }
            if (IsManaphyEgg)
            {
                // Since none of this data is populated, fill in default info.
                pk4.Species = 490;
                // Level 1 Moves
                pk4.Move1 = 294;
                pk4.Move2 = 145;
                pk4.Move3 = 346;
                pk4.FatefulEncounter = true;
                pk4.Ball = 4;
                pk4.Version = 10; // Diamond
                pk4.Language = 2; // English
                pk4.Nickname = "MANAPHY";
                pk4.Egg_Location = 1; // Ranger (will be +3000 later)
            }

            // Generate IV
            uint seed = Util.rnd32();
            if (pk4.PID == 1 || IsManaphyEgg) // Create Nonshiny
            {
                uint pid1 = PKX.LCRNG(ref seed) >> 16;
                uint pid2 = PKX.LCRNG(ref seed) >> 16;

                while ((pid1 ^ pid2 ^ pk4.TID ^ pk4.SID) < 8)
                {
                    uint testPID = pid1 | pid2 << 16;

                    // Call the ARNG to change the PID
                    testPID = testPID * 0x6c078965 + 1;

                    pid1 = testPID & 0xFFFF;
                    pid2 = testPID >> 16;
                }
                pk4.PID = pid1 | (pid2 << 16);
            }

            // Generate IVs
            if (pk4.IV32 == 0)
            {
                uint iv1 = PKX.LCRNG(ref seed) >> 16;
                uint iv2 = PKX.LCRNG(ref seed) >> 16;
                pk4.IV32 = (iv1 | iv2 << 16) & 0x3FFFFFFF;
            }

            // Generate Met Info
            if (IsPokémon)
            {
                pk4.Met_Location = pk4.Egg_Location + 3000;
                pk4.Egg_Location = 0;
                pk4.MetDate = DateTime.Now;
                pk4.IsEgg = false;
            }
            else
            {
                pk4.Egg_Location = pk4.Egg_Location + 3000;
                pk4.MetDate = DateTime.Now;
                pk4.IsEgg = false;
                // Met Location is modified when transferred to pk5; don't worry about it.
            }
            if (pk4.Species == 201) // Never will be true; Unown was never distributed.
                pk4.AltForm = PKX.getUnownForm(pk4.PID);
            if (IsEgg || IsManaphyEgg)
                pk4.IsEgg = true;

            pk4.RefreshChecksum();
            return pk4;
        }
    }
}
