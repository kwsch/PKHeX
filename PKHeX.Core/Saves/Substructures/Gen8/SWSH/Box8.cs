namespace PKHeX.Core;

public sealed class Box8(SaveFile sav, SCBlock block) : SaveBlock<SaveFile>(sav, block.Data);
