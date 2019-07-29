using System;

namespace PKHeX.Core
{
    public class SAV7SM : SAV7
    {
        public SAV7SM(byte[] data) : base(data, BlocksSM, boSM) => Initialize();
        public SAV7SM() : base(SaveUtil.SIZE_G7SM, BlocksSM, boSM) => Initialize();
        public override SaveFile Clone() => new SAV7SM((byte[])Data.Clone());

        private void Initialize()
        {
            Personal = PersonalTable.SM;
            HeldItems = Legal.HeldItems_SM;

            Items = new MyItem7SM(this, Bag);
            Zukan = new Zukan7(this, PokeDex, PokeDexLanguageFlags);
            Records = new Record6(this, Record, Core.Records.MaxType_SM);
        }

        protected override int EventFlagMax => 3968;
        public override int MaxMoveID => Legal.MaxMoveID_7;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7;
        public override int MaxItemID => Legal.MaxItemID_7;
        public override int MaxAbilityID => Legal.MaxAbilityID_7;

        private const int boSM = SaveUtil.SIZE_G7SM - 0x200;

        public static readonly BlockInfo[] BlocksSM =
        {
            new BlockInfo7 (boSM, 00, 0x00000, 0x00DE0),
            new BlockInfo7 (boSM, 01, 0x00E00, 0x0007C),
            new BlockInfo7 (boSM, 02, 0x01000, 0x00014),
            new BlockInfo7 (boSM, 03, 0x01200, 0x000C0),
            new BlockInfo7 (boSM, 04, 0x01400, 0x0061C),
            new BlockInfo7 (boSM, 05, 0x01C00, 0x00E00),
            new BlockInfo7 (boSM, 06, 0x02A00, 0x00F78),
            new BlockInfo7 (boSM, 07, 0x03A00, 0x00228),
            new BlockInfo7 (boSM, 08, 0x03E00, 0x00104),
            new BlockInfo7 (boSM, 09, 0x04000, 0x00200),
            new BlockInfo7 (boSM, 10, 0x04200, 0x00020),
            new BlockInfo7 (boSM, 11, 0x04400, 0x00004),
            new BlockInfo7 (boSM, 12, 0x04600, 0x00058),
            new BlockInfo7 (boSM, 13, 0x04800, 0x005E6),
            new BlockInfo7 (boSM, 14, 0x04E00, 0x36600),
            new BlockInfo7 (boSM, 15, 0x3B400, 0x0572C),
            new BlockInfo7 (boSM, 16, 0x40C00, 0x00008),
            new BlockInfo7 (boSM, 17, 0x40E00, 0x01080),
            new BlockInfo7 (boSM, 18, 0x42000, 0x01A08),
            new BlockInfo7 (boSM, 19, 0x43C00, 0x06408),
            new BlockInfo7 (boSM, 20, 0x4A200, 0x06408),
            new BlockInfo7 (boSM, 21, 0x50800, 0x03998),
            new BlockInfo7 (boSM, 22, 0x54200, 0x00100),
            new BlockInfo7 (boSM, 23, 0x54400, 0x00100),
            new BlockInfo7 (boSM, 24, 0x54600, 0x10528),
            new BlockInfo7 (boSM, 25, 0x64C00, 0x00204),
            new BlockInfo7 (boSM, 26, 0x65000, 0x00B60),
            new BlockInfo7 (boSM, 27, 0x65C00, 0x03F50),
            new BlockInfo7 (boSM, 28, 0x69C00, 0x00358),
            new BlockInfo7 (boSM, 29, 0x6A000, 0x00728),
            new BlockInfo7 (boSM, 30, 0x6A800, 0x00200),
            new BlockInfo7 (boSM, 31, 0x6AA00, 0x00718),
            new BlockInfo7 (boSM, 32, 0x6B200, 0x001FC),
            new BlockInfo7 (boSM, 33, 0x6B400, 0x00200),
            new BlockInfo7 (boSM, 34, 0x6B600, 0x00120),
            new BlockInfo7 (boSM, 35, 0x6B800, 0x001C8),
            new BlockInfo7 (boSM, 36, 0x6BA00, 0x00200),
        };

        private const ulong MagearnaConst = 0xCBE05F18356504AC;

        public void UpdateMagearnaConstant()
        {
            var flag = GetEventFlag(3100);
            ulong value = flag ? MagearnaConst : 0ul;
            SetData(BitConverter.GetBytes(value), QRSaveData + 0x168);
        }
    }
}