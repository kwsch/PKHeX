using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Provides reflection utility for manipulating blocks, providing block names and value wrapping.
/// </summary>
public sealed class SCBlockMetadata
{
    private readonly Dictionary<string, IDataIndirect> BlockList;
    private readonly Dictionary<uint, string> ValueList;
    private readonly SCBlockAccessor Accessor;

    /// <summary>
    /// Creates a new instance of <see cref="SCBlockMetadata"/> by loading properties and constants declared via reflection.
    /// </summary>
    public SCBlockMetadata(SCBlockAccessor accessor, IEnumerable<string> extraKeyNames, params string[] exclusions)
    {
        var aType = accessor.GetType();

        BlockList = aType.GetAllPropertiesOfType<IDataIndirect>(accessor);
        ValueList = aType.GetAllConstantsOfType<uint>();
        AddExtraKeyNames(ValueList, extraKeyNames);
        if (exclusions.Length != 0)
            ValueList = ValueList.Where(z => !exclusions.Any(z.Value.Contains)).ToDictionary();
        Accessor = accessor;
    }

    /// <summary>
    /// Returns a list of block details, ordered by their type and <see cref="GetSortKey"/>.
    /// </summary>
    public IEnumerable<ComboItem> GetSortedBlockKeyList() => Accessor.BlockInfo
        .Select((z, i) => new ComboItem(GetBlockHint(z, i), (int)z.Key))
        .OrderBy(z => !z.Text.StartsWith('*'))
        .ThenBy(z => GetSortKey(z));

    /// <summary>
    /// Loads names from an external file to the requested <see cref="names"/> list.
    /// </summary>
    /// <remarks>Tab separated text file expected.</remarks>
    /// <param name="names">Currently loaded list of block names</param>
    /// <param name="lines">Tab separated key-value pair list of block names.</param>
    public static void AddExtraKeyNames(Dictionary<uint, string> names, IEnumerable<string> lines)
    {
        foreach (ReadOnlySpan<char> line in lines)
        {
            var split = line.IndexOf('\t');
            if (split < 0)
                continue;
            var hex = line[..split];
            if (!ulong.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var value))
                continue;

            var name = line[(split + 1)..].ToString();
            names.TryAdd((uint)value, name);
        }
    }

    private static string GetSortKey(in ComboItem item)
    {
        var text = item.Text;
        if (text.StartsWith('*'))
            return text;
        // key:X8, " - ", "####", " ", type
        return text[(8 + 3 + 4 + 1)..];
    }

    private string GetBlockHint(SCBlock z, int index)
    {
        var blockName = GetBlockName(z, out _);
        var isBool = z.Type.IsBoolean();
        var type = (isBool ? "Bool" : z.Type.ToString());
        if (blockName != null)
            return $"*{type} {blockName}";
        var result = $"{z.Key:X8} - {index:0000} {type}";
        if (z.Type is SCTypeCode.Object or SCTypeCode.Array)
            result += $" 0x{z.Data.Length:X3}";
        else if (!isBool)
            result += $" {z.GetValue()}";
        return result;
    }

    /// <summary>
    /// Searches the <see cref="BlockList"/> to see if a named Save Block originate from the requested <see cref="block"/>. If no block exists, the logic will check for a named stored-value.
    /// </summary>
    /// <param name="block">Block requesting the name of</param>
    /// <param name="saveBlock">Block that shares the same backing byte array; null if none.</param>
    /// <returns>Name of the block indicating the purpose that it serves in-game.</returns>
    public string? GetBlockName(SCBlock block, out IDataIndirect? saveBlock)
    {
        // See if we have a Block object for this block
        if (block.Data.Length != 0)
        {
            static bool SameBackingBuffer(IDataIndirect d, ReadOnlyMemory<byte> data) => d.Equals(data);
            var obj = BlockList.FirstOrDefault(z => SameBackingBuffer(z.Value, block.Data));
            if (obj is not (null, null))
            {
                saveBlock = obj.Value;
                return obj.Key;
            }
        }

        // See if it's a single-value declaration
        if (ValueList.TryGetValue(block.Key, out var blockName))
        {
            saveBlock = null;
            return blockName;
        }
        saveBlock = null;
        return null;
    }

    /// <summary>
    /// Returns an object that wraps the block with a Value property to get/set via a PropertyGrid/etc. control.
    /// </summary>
    /// <returns>Returns null if no wrapping is supported.</returns>
    public static object? GetEditableBlockObject(SCBlock block) => block.Type switch
    {
        SCTypeCode.Byte => new WrappedValueView<byte>(block, block.GetValue()),
        SCTypeCode.UInt16 => new WrappedValueView<ushort>(block, block.GetValue()),
        SCTypeCode.UInt32 => new WrappedValueView<uint>(block, block.GetValue()),
        SCTypeCode.UInt64 => new WrappedValueView<ulong>(block, block.GetValue()),

        SCTypeCode.SByte => new WrappedValueView<sbyte>(block, block.GetValue()),
        SCTypeCode.Int16 => new WrappedValueView<short>(block, block.GetValue()),
        SCTypeCode.Int32 => new WrappedValueView<int>(block, block.GetValue()),
        SCTypeCode.Int64 => new WrappedValueView<long>(block, block.GetValue()),

        SCTypeCode.Single => new WrappedValueView<float>(block, block.GetValue()),
        SCTypeCode.Double => new WrappedValueView<double>(block, block.GetValue()),

        _ => null,
    };

    private sealed class WrappedValueView<T>(SCBlock Parent, object currentValue) where T : struct
    {
        private T _value = (T)Convert.ChangeType(currentValue, typeof(T));

        [Description("Stored Value for this Block")]
        public T Value
        {
            get => _value;
            set => Parent.SetValue(_value = value);
        }

        // ReSharper disable once UnusedMember.Local
        [Description("Type of Value this Block stores")]
#pragma warning disable CA1822
        public string ValueType => typeof(T).Name;
#pragma warning restore CA1822
    }
}
