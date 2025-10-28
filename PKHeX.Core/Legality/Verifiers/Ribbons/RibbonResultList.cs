using System;

namespace PKHeX.Core;

/// <summary>
/// List using a fixed-size span to store results. Only exposes <see cref="Count"/>; the span is hidden, and only accessible if the span was already a known instance.
/// </summary>
public ref struct RibbonResultList(Span<RibbonResult> Span)
{
    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly Span<RibbonResult> Span = Span;

    /// <summary>
    /// Count of results that were added to the span.
    /// </summary>
    public int Count { get; private set; }

    private void Add(RibbonResult item) => Span[Count++] = item;

    public void Add(RibbonIndex index, bool missing = false) => Add(new(index, missing));
    public void Add(RibbonIndex3 index, bool missing = false) => Add(new(index, missing));
    public void Add(RibbonIndex4 index, bool missing = false) => Add(new(index, missing));
}
