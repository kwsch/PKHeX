using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public abstract class MyItem(SaveFile SAV, Memory<byte> raw) : SaveBlock<SaveFile>(SAV, raw)
{
    public abstract IReadOnlyList<InventoryPouch> Inventory { get; set; }
}
