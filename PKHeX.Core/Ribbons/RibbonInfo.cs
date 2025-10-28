using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Provides information about the state of a given ribbon.
/// </summary>
public sealed class RibbonInfo
{
    public const string PropertyPrefix = "Ribbon";

    public readonly string Name;
    public readonly RibbonValueType Type;

    public bool HasRibbon { get; set; }
    public byte RibbonCount { get; set; }

    private RibbonInfo(string name, bool hasRibbon)
    {
        Name = name;
        HasRibbon = hasRibbon;
        Type = RibbonValueType.Boolean;
    }

    private RibbonInfo(string name, byte count)
    {
        Name = name;
        Type = RibbonValueType.Byte;
        RibbonCount = count;
    }

    public int MaxCount
    {
        get
        {
            if (Type is RibbonValueType.Boolean)
                throw new ArgumentOutOfRangeException(nameof(Type));

            return Name switch
            {
                nameof(IRibbonSetMemory6.RibbonCountMemoryContest) => 40,
                nameof(IRibbonSetMemory6.RibbonCountMemoryBattle) => 8,
                _ => 4, // Gen3/4 Contest Ribbons
            };
        }
    }

    /// <summary>
    /// Gets a list of all ribbons available for the entity and their state.
    /// </summary>
    public static List<RibbonInfo> GetRibbonInfo(PKM pk)
    {
        var riblist = new List<RibbonInfo>();
        var names = ReflectUtil.GetPropertiesStartWithPrefix(pk.GetType(), PropertyPrefix);
        foreach (var name in names)
        {
            var value = ReflectUtil.GetValue(pk, name);
            if (value is bool b)
                riblist.Add(new RibbonInfo(name, b));
            else if (value is byte x)
                riblist.Add(new RibbonInfo(name, x));
        }
        return riblist;
    }
}

/// <summary>
/// Type of Ribbon Value
/// </summary>
public enum RibbonValueType
{
    Boolean,
    Byte,
}
