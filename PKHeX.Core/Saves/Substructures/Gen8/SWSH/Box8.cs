namespace PKHeX.Core;

public sealed class Box8 : SaveBlock<SaveFile>
{
    public Box8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }
}
