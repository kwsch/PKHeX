using System;
using static PKHeX.Core.RibbonParseFlags;

namespace PKHeX.Core;

/// <summary>
/// Stores info about if a ribbon is valid or not.
/// </summary>
public readonly record struct RibbonResult
{
    // TWO BYTES USED
    private readonly byte Value;
    private readonly RibbonParseFlags Flags;

    /// <summary>
    /// True: ribbon is missing -- should have the ribbon.
    /// False: ribbon is invalid -- should not have the ribbon.
    /// </summary>
    public bool IsMissing => (Flags & Missing) != 0;

    private RibbonParseFlags Type => Flags & ~Missing;

    public RibbonResult(RibbonIndex index, bool missing = false)  { Value = (byte)index; Flags = missing ? MissingM : Mainline; }
    public RibbonResult(RibbonIndex3 index, bool missing = false) { Value = (byte)index; Flags = missing ? Missing3 : Index3; }
    public RibbonResult(RibbonIndex4 index, bool missing = false) { Value = (byte)index; Flags = missing ? Missing4 : Index4; }
    public bool Equals(RibbonIndex index) => Type == Mainline && Value == (byte)index;
    public bool Equals(RibbonIndex3 index) => Type == Index3 && Value == (byte)index;
    public bool Equals(RibbonIndex4 index) => Type == Index4 && Value == (byte)index;

    /// <summary>
    /// Property Name of the ribbon based on the source code's getters/setters, allowing lookup for localization.
    /// </summary>
    public string PropertyName => Type switch
    {
        Mainline => ((RibbonIndex)Value).GetPropertyName(),
        Index3 => ((RibbonIndex3)Value).GetPropertyName(),
        Index4 => ((RibbonIndex4)Value).GetPropertyName(),
        _ => throw new ArgumentException($"Invalid type: {Type}"),
    };

    /// <summary>
    /// Updates the ribbon state depending on the <see cref="args"/> and <see cref="IsMissing"/> state.
    /// </summary>
    public void Fix(in RibbonVerifierArguments args)
    {
        switch (Type)
        {
            case Mainline: ((RibbonIndex)Value).Fix(args, IsMissing); break;
            case Index3: ((RibbonIndex3)Value).Fix(args, IsMissing); break;
            case Index4: ((RibbonIndex4)Value).Fix(args, IsMissing); break;
            default: throw new ArgumentException($"Invalid type: {Type}");
        }
    }
}

[Flags]
public enum RibbonParseFlags : byte
{
    None = 0,
    Mainline,
    Index3,
    Index4,

    Missing = 0x80,
    MissingM = Missing | Mainline,
    Missing3 = Missing | Index3,
    Missing4 = Missing | Index4,
}
