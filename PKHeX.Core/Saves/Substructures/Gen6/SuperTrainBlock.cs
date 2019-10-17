namespace PKHeX.Core
{
    public class SuperTrainBlock : SaveBlock
    {
        public SuperTrainBlock(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public SuperTrainBlock(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        // Structure:
        // 8 bytes ??? (flags?)
        // float[48] recordScore1; // 0x08, 4byte/entry
        // float[48] recordScore2; // 0xC8, 4byte/entry
        // SS-F-G[48] recordHolder1; // 0x188, 4byte/entry
        // SS-F-G[48] recordHolder2; // 0x248, 4byte/entry
        // byte[12] bags? // 0x308
        // u32 ??? // 0x314

        // 0x318 total size

        // SS-F-G = u16 species, u8 form, u8 gender
    }
}