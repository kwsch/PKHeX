using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Localizing Memory strings
/// </summary>
/// <remarks>
/// <see cref="IMemoryOT"/> and <see cref="IMemoryHT"/> parameters build strings.
/// </remarks>
public sealed class MemoryStrings
{
    private readonly GameStrings s;

    public MemoryStrings(GameStrings strings)
    {
        s = strings;
        memories = new Lazy<List<ComboItem>>(GetMemories);
        none = new Lazy<List<ComboItem>>(() => Util.GetCBList([string.Empty]));
        species = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.specieslist));
        item6 = new Lazy<List<ComboItem>>(() => GetItems(EntityContext.Gen6));
        item8 = new Lazy<List<ComboItem>>(() => GetItems(EntityContext.Gen8));
        genloc = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.genloc));
        moves = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.movelist));
        specific = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.Gen6.Met0, Locations6.Met0));
    }

    private List<ComboItem> GetItems(EntityContext context)
    {
        var permit = Memories.GetMemoryItemParams(context);
        var asInt = permit.ToArray();
        return Util.GetCBList(s.itemlist, asInt);
    }

    private readonly Lazy<List<ComboItem>> memories;
    private readonly Lazy<List<ComboItem>> none, species, item6, item8, genloc, moves, specific;

    public List<ComboItem> Memory => memories.Value;
    public List<ComboItem> None => none.Value;
    public List<ComboItem> Moves => moves.Value;
    public List<ComboItem> Items6 => item6.Value;
    public List<ComboItem> Items8 => item8.Value;
    public List<ComboItem> GeneralLocations => genloc.Value;
    public List<ComboItem> SpecificLocations => specific.Value;
    public List<ComboItem> Species => species.Value;

    private List<ComboItem> GetMemories()
    {
        var mems = s.memories.AsSpan(0);
        var list = new List<ComboItem> {new(mems[0], 0)}; // none at top
        Util.AddCBWithOffset(list, mems[1..], 1); // sorted the rest
        return list;
    }

    public ReadOnlySpan<string> GetMemoryQualities() => s.intensity;
    public ReadOnlySpan<string> GetMemoryFeelings(int memoryGen) => memoryGen >= 8 ? s.feeling8 : s.feeling6;

    public List<ComboItem> GetArgumentStrings(MemoryArgType type, int memoryGen) => type switch
    {
        MemoryArgType.Species => Species,
        MemoryArgType.GeneralLocation => GeneralLocations,
        MemoryArgType.Item => memoryGen == 6 ? Items6 : Items8,
        MemoryArgType.Move => Moves,
        MemoryArgType.SpecificLocation => SpecificLocations,
        _ => None,
    };
}
