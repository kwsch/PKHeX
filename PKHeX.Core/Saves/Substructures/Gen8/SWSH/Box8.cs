namespace PKHeX.Core
{
    public sealed class Box8 : SaveBlock
    {
        public Box8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }
    }
}
