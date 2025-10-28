using System;
using System.ComponentModel;

namespace PKHeX.Core;

public sealed class Box8(SaveFile sav, SCBlock block) : SaveBlock<SaveFile>(sav, block.Raw)
{
    [Browsable(false)] public new Memory<byte> Raw => base.Raw;
}
