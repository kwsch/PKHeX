using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Mystery Gift Template File
    /// </summary>
    /// <remarks>
    /// Big thanks to Grovyle91's Pokémon Mystery Gift Editor, from which the structure was referenced.
    /// https://projectpokemon.org/home/profile/859-grovyle91/
    /// https://projectpokemon.org/home/forums/topic/5870-pok%C3%A9mon-mystery-gift-editor-v143-now-with-bw-support/
    /// See also: http://tccphreak.shiny-clique.net/debugger/pcdfiles.htm
    /// </remarks>
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
            Data = data ?? new byte[Size];
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

        private const int TitleLength = 0x48;

        public override string CardTitle
        {
            get => StringConverter.GetString4(Data, 0x104, TitleLength);
            set
            {
                byte[] data = StringConverter.SetString4(value, (TitleLength / 2) - 1, TitleLength / 2, 0xFFFF);
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
        public override int Gender { get => Gift.Gender; set => Gift.Gender = value; }
        public override int Form { get => Gift.Form; set => Gift.Form = value; }
        public override int TID { get => Gift.TID; set => Gift.TID = value; }
        public override int SID { get => Gift.SID; set => Gift.SID = value; }
        public override string OT_Name { get => Gift.OT_Name; set => Gift.OT_Name = value; }

        // ILocation overrides
        public override int Location { get => IsEgg ? 0 : Gift.EggLocation + 3000; set { } }
        public override int EggLocation { get => IsEgg ? Gift.EggLocation + 3000 : 0; set { } }

        public bool GiftEquals(PGT pgt)
        {
            // Skip over the PGT's "Corresponding PCD Slot" @ 0x02
            byte[] g = pgt.Data;
            byte[] c = Gift.Data;
            if (g.Length != c.Length || g.Length < 3)
                return false;
            for (int i = 0; i < 2; i++)
            {
                if (g[i] != c[i])
                    return false;
            }

            for (int i = 3; i < g.Length; i++)
            {
                if (g[i] != c[i])
                    return false;
            }

            return true;
        }

        public override PKM ConvertToPKM(ITrainerInfo SAV)
        {
            return Gift.ConvertToPKM(SAV);
        }

        public bool CanBeReceivedBy(int pkmVersion) => (CardCompatibility >> pkmVersion & 1) == 1;
    }
}
