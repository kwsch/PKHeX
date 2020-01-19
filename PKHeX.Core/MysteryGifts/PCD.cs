using System;
using System.Collections.Generic;

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
    public sealed class PCD : DataMysteryGift
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

        public PCD() : this(new byte[Size]) { }
        public PCD(byte[] data) : base(data) { }

        public override byte[] Write()
        {
            // Ensure PGT content is encrypted
            var clone = (PCD)Clone();
            if (clone.Gift.VerifyPKEncryption())
                clone.Gift = clone.Gift;
            return clone.Data;
        }

        public PGT Gift
        {
            get => _gift ??= new PGT(Data.Slice(0, PGT.Size));
            set => (_gift = value).Data.CopyTo(Data, 0);
        }

        private PGT? _gift;

        public byte[] Information
        {
            get => Data.SliceEnd(PGT.Size);
            set => value.CopyTo(Data, Data.Length - PGT.Size);
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
            get => StringConverter4.GetString4(Data, 0x104, TitleLength);
            set
            {
                byte[] data = StringConverter4.SetString4(value, (TitleLength / 2) - 1, TitleLength / 2, 0xFFFF);
                int len = data.Length;
                Array.Resize(ref data, 0x48);
                for (int i = 0; i < len; i++)
                    data[i] = 0xFF;
                data.CopyTo(Data, 0x104);
            }
        }

        public ushort CardCompatibility => BitConverter.ToUInt16(Data, 0x14C); // rest of bytes we don't really care about

        public override int Species { get => Gift.IsManaphyEgg ? 490 : Gift.Species; set => Gift.Species = value; }
        public override IReadOnlyList<int> Moves { get => Gift.Moves; set => Gift.Moves = value; }
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

        public override PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            return Gift.ConvertToPKM(SAV, criteria);
        }

        public bool CanBeReceivedBy(int pkmVersion) => (CardCompatibility >> pkmVersion & 1) == 1;

        protected override bool IsMatchExact(PKM pkm)
        {
            var wc = Gift.PK;
            if (!wc.IsEgg)
            {
                if (wc.TID != pkm.TID) return false;
                if (wc.SID != pkm.SID) return false;
                if (wc.OT_Name != pkm.OT_Name) return false;
                if (wc.OT_Gender != pkm.OT_Gender) return false;
                if (wc.Language != 0 && wc.Language != pkm.Language) return false;

                if (pkm.Format != 4) // transferred
                {
                    // met location: deferred to general transfer check
                    if (wc.CurrentLevel > pkm.Met_Level) return false;
                }
                else
                {
                    if (wc.Egg_Location + 3000 != pkm.Met_Location) return false;
                    if (wc.CurrentLevel != pkm.Met_Level) return false;
                }
            }
            else // Egg
            {
                if (wc.Egg_Location + 3000 != pkm.Egg_Location && pkm.Egg_Location != Locations.LinkTrade4) // traded
                    return false;
                if (wc.CurrentLevel != pkm.Met_Level)
                    return false;
                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }

            if (wc.AltForm != pkm.AltForm && !Legal.IsFormChangeable(pkm, Species))
                return false;

            if (wc.Ball != pkm.Ball) return false;
            if (wc.OT_Gender < 3 && wc.OT_Gender != pkm.OT_Gender) return false;
            if (wc.PID == 1 && pkm.IsShiny) return false;
            if (wc.Gender != 3 && wc.Gender != pkm.Gender) return false;

            if (pkm is IContestStats s && s.IsContestBelow(wc))
                return false;

            return true;
        }

        protected override bool IsMatchDeferred(PKM pkm)
        {
            if (!CanBeReceivedBy(pkm.Version))
                return false;
            return Species != pkm.Species;
        }
    }
}
