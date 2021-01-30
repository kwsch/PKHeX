using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Record of a received <see cref="WB7"/> file.
    /// </summary>
    /// <remarks>
    /// A full <see cref="WB7"/> is not stored in the <see cref="SAV7b"/> structure, as it is immediately converted to <see cref="PKM"/> upon receiving from server.
    /// The save file just stores a summary of the received data for the user to look back at.
    /// </remarks>
    public sealed class WR7 : DataMysteryGift
    {
        public const int Size = 0x140;
        public override int Generation => 7;

        public WR7() : this(new byte[Size]) { }
        public WR7(byte[] data) : base(data) { }

        public uint Epoch
        {
            get => BitConverter.ToUInt32(Data, 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x00);
        }

        public DateTime Date => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Epoch);

        public override int CardID
        {
            get => BitConverter.ToUInt16(Data, 0x08);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x08);
        }

        public ushort CardType
        {
            get => BitConverter.ToUInt16(Data, 0x0A);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x0A);
        }

        public WR7GiftType GiftType
        {
            get => (WR7GiftType)Data[0x0C];
            set => Data[0x0C] = (byte)value;
        }

        public int ItemCount
        {
            get => Data[0x0D];
            set => Data[0x0D] = (byte)value;
        }

        // unknown: region from 0x10 to 0xFF ?

        public override int Species
        {
            get => BitConverter.ToUInt16(Data, 0x10C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x10C);
        }

        public override bool GiftUsed { get; set; }

        public override int Level // are moves stored? mew has '1' but this could be move
        {
            get => BitConverter.ToUInt16(Data, 0x10E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x10E);
        }

        public override int ItemID { get => BitConverter.ToUInt16(Data, 0x110); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x110); }
        public ushort ItemIDCount { get => BitConverter.ToUInt16(Data, 0x112); set => BitConverter.GetBytes(value).CopyTo(Data, 0x112); }
        public ushort ItemSet2Item  { get => BitConverter.ToUInt16(Data, 0x114); set => BitConverter.GetBytes(value).CopyTo(Data, 0x114); }
        public ushort ItemSet2Count { get => BitConverter.ToUInt16(Data, 0x116); set => BitConverter.GetBytes(value).CopyTo(Data, 0x116); }
        public ushort ItemSet3Item  { get => BitConverter.ToUInt16(Data, 0x118); set => BitConverter.GetBytes(value).CopyTo(Data, 0x118); }
        public ushort ItemSet3Count { get => BitConverter.ToUInt16(Data, 0x11A); set => BitConverter.GetBytes(value).CopyTo(Data, 0x11A); }
        public ushort ItemSet4Item  { get => BitConverter.ToUInt16(Data, 0x11C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x11C); }
        public ushort ItemSet4Count { get => BitConverter.ToUInt16(Data, 0x11E); set => BitConverter.GetBytes(value).CopyTo(Data, 0x11E); }
        public ushort ItemSet5Item  { get => BitConverter.ToUInt16(Data, 0x120); set => BitConverter.GetBytes(value).CopyTo(Data, 0x120); } // struct union overlaps OT Name data, beware!
        public ushort ItemSet5Count { get => BitConverter.ToUInt16(Data, 0x122); set => BitConverter.GetBytes(value).CopyTo(Data, 0x122); }
        public ushort ItemSet6Item  { get => BitConverter.ToUInt16(Data, 0x124); set => BitConverter.GetBytes(value).CopyTo(Data, 0x124); }
        public ushort ItemSet6Count { get => BitConverter.ToUInt16(Data, 0x126); set => BitConverter.GetBytes(value).CopyTo(Data, 0x126); }

        public override int Gender { get; set; }
        public override int Form { get; set; }
        public override int TID { get; set; }
        public override int SID { get; set; }

        public override string OT_Name
        {
            get => StringConverter.GetString7(Data, 0x120, 0x1A);
            set => StringConverter.SetString7b(value, 12, 13).CopyTo(Data, 0x120 + 0xB6); // careful with length
        }

        public LanguageID LanguageReceived
        {
            get => (LanguageID)Data[0x13A];
            set => Data[0x13A] = (byte)value;
        }

        // Mystery Gift implementation, unused.
        public override bool IsMatchExact(PKM pkm, DexLevel evo) => false;
        protected override bool IsMatchDeferred(PKM pkm) => false;
        protected override bool IsMatchPartial(PKM pkm) => false;

        public override int Location { get; set; }
        public override int EggLocation { get; set; }
        public override int Ball { get; set; } = 4;

        public override string CardTitle { get => $"{nameof(WB7)} Record ({OT_Name}) [{LanguageReceived}]"; set { } }

        public override bool IsItem
        {
            get => GiftType == WR7GiftType.Item;
            set
            {
                if (value)
                    GiftType = WR7GiftType.Item;
            }
        }

        public override bool IsPokémon
        {
            get => GiftType == WR7GiftType.Pokemon;
            set
            {
                if (value)
                    GiftType = WR7GiftType.Pokemon;
            }
        }

        public override PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria)
        {
            // this method shouldn't really be called, use the WB7 data not the WR7 data.
            if (!IsPokémon)
                throw new ArgumentException(nameof(IsPokémon));

            // we'll just generate something as close as we can, since we must return something!
            var pk = new PB7();
            sav.ApplyTo(pk);
            if (!GameVersion.GG.Contains((GameVersion) sav.Game))
                pk.Version = (int) GameVersion.GP;

            pk.Species = Species;
            pk.Met_Level = pk.CurrentLevel = Level;
            pk.MetDate = Date;

            return pk; // can't really do much more, just return the rough data
        }
    }
}
