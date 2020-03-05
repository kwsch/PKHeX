using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Provides reflection utility for manipulating blocks, providing block names and value wrapping.
    /// </summary>
    public class SCBlockMetadata
    {
        private readonly Dictionary<SaveBlock, string> BlockList;
        private readonly Dictionary<uint, string> ValueList;
        private readonly SCBlockAccessor Accessor;

        public SCBlockMetadata(SCBlockAccessor accessor)
        {
            var aType = accessor.GetType();

            BlockList = aType.GetAllPropertiesOfType<SaveBlock>(accessor);
            ValueList = aType.GetAllConstantsOfType<uint>();
            Accessor = accessor;
        }

        public IEnumerable<ComboItem> GetSortedBlockKeyList()
        {
            var list = Accessor.BlockInfo
                .Select((z, i) => new ComboItem(GetBlockHint(z, i), (int)z.Key))
                .OrderBy(z => !z.Text.StartsWith("*"))
                .ThenBy(z => GetSortKey(z));
            return list;
        }

        private static string GetSortKey(in ComboItem item)
        {
            var text = item.Text;
            if (text.StartsWith("*"))
                return text;
            // key:X8, " - ", "####", " ", type
            return text.Substring(8 + 3 + 4 + 1);
        }

        private string GetBlockHint(SCBlock z, int i)
        {
            var blockName = GetBlockName(z, out _);
            var isBool = z.Type.IsBoolean();
            var type = (isBool ? "Bool" : z.Type.ToString());
            if (blockName != null)
                return $"*{type} {blockName}";
            var result = $"{z.Key:X8} - {i:0000} {type}";
            if (z.Type == SCTypeCode.Object || z.Type == SCTypeCode.Array)
                result += $" 0x{z.Data.Length:X3}";
            else if (!isBool)
                result += $" {z.GetValue()}";
            return result;
        }

        public string? GetBlockName(SCBlock block, out SaveBlock? saveBlock)
        {
            // See if we have a Block object for this block
            var obj = BlockList.FirstOrDefault(z => ReferenceEquals(z.Key.Data, block.Data));
            if (obj.Key != null)
            {
                saveBlock = obj.Key;
                return obj.Value;
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
        /// Returns an object that wraps the block with a Value property to get/set via a PropertyGrid/etc control.
        /// </summary>
        /// <returns>Returns null if no wrapping is supported.</returns>
        public static object? GetEditableBlockObject(SCBlock block)
        {
            return block.Type switch
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
        }

        private class WrappedValueView<T> where T : struct
        {
            private readonly SCBlock Parent;
            private T _value;

            [Description("Stored Value for this Block")]
            public T Value
            {
                get => _value;
                set => Parent.SetValue(_value = value);
            }

            [Description("Type of Value this Block stores")]
            public string ValueType => typeof(T).Name;

            public WrappedValueView(SCBlock block, object currentValue)
            {
                Parent = block;
                _value = (T)Convert.ChangeType(currentValue, typeof(T));
            }
        }
    }
}