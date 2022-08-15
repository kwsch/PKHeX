using System;
using static PKHeX.Core.RibbonParseFlags;

namespace PKHeX.Core;

public readonly record struct RibbonResult
{
    private readonly byte Value;
    private readonly RibbonParseFlags Flags;

    public bool IsRequired => (Flags & Required) != 0;
    private RibbonParseFlags Type => Flags & ~Required;

    public RibbonResult(RibbonIndex index, bool required = false) { Value = (byte)index; Flags = required ? Required : Mainline; }
    public RibbonResult(RibbonIndex3 index, bool required = false) { Value = (byte)index; Flags = required ? Required3 : Index3; }
    public RibbonResult(RibbonIndex4 index, bool required = false) { Value = (byte)index; Flags = required ? Required4 : Index4; }
    public bool Equals(RibbonIndex index) => Type == Mainline && Value == (byte)index;
    public bool Equals(RibbonIndex3 index) => Type == Index3 && Value == (byte)index;
    public bool Equals(RibbonIndex4 index) => Type == Index4 && Value == (byte)index;

    public string Name => Type switch
    {
        Mainline => ((RibbonIndex)Value).GetPropertyName(),
        Index3 => ((RibbonIndex3)Value).GetPropertyName(),
        Index4 => ((RibbonIndex4)Value).GetPropertyName(),
        _ => throw new ArgumentOutOfRangeException(),
    };

    public void Fix(RibbonVerifierArguments args)
    {
        switch (Type)
        {
            case Mainline: ((RibbonIndex)Value).Fix(args, IsRequired); break;
            case Index3: ((RibbonIndex3)Value).Fix(args, IsRequired); break;
            case Index4: ((RibbonIndex4)Value).Fix(args, IsRequired); break;
            default: throw new ArgumentOutOfRangeException();
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

    Required = 0x80,
    Required3 = Required | Index3,
    Required4 = Required | Index4,
}
