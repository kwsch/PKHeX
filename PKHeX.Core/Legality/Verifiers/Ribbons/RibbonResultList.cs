using System;

namespace PKHeX.Core;

public ref struct RibbonResultList
{
    private readonly Span<RibbonResult> Span;
    public int Count { get; private set; }

    public RibbonResultList(Span<RibbonResult> span)
    {
        Span = span;
        Count = 0;
    }

    private void Add(RibbonResult item)
    {
        Span[Count] = item;
        ++Count;
    }

    public void Add(RibbonIndex index, bool required = false) => Add(new(index, required));
    public void Add(RibbonIndex3 index, bool required = false) => Add(new(index, required));
    public void Add(RibbonIndex4 index, bool required = false) => Add(new(index, required));
}
