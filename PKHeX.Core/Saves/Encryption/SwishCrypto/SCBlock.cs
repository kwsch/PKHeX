using System;
using System.Diagnostics;
using System.IO;

namespace PKHeX.Core
{
    /// <summary>
    /// Block of <see cref="Data"/> obtained from a <see cref="SwishCrypto"/> encrypted block storage binary.
    /// </summary>
    public sealed class SCBlock
    {
        /// <summary>
        /// Used to encrypt the rest of the block.
        /// </summary>
        public readonly uint Key;

        /// <summary>
        /// What kind of block is it?
        /// </summary>
        public SCTypeCode Type { get; private set; }

        /// <summary>
        /// For <see cref="SCTypeCode.Array"/>: What kind of array is it?
        /// </summary>
        public readonly SCTypeCode SubType;

        /// <summary>
        /// Decrypted data for this block.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// Changes the block's Boolean type. Will throw if the old / new <see cref="Type"/> is not boolean.
        /// </summary>
        /// <param name="value">New boolean type to set.</param>
        /// <remarks>Will throw if the requested block state changes are incorrect.</remarks>
        public void ChangeBooleanType(SCTypeCode value)
        {
            if (Type is not (SCTypeCode.Bool1 or SCTypeCode.Bool2) || value is not (SCTypeCode.Bool1 or SCTypeCode.Bool2))
                throw new InvalidOperationException($"Cannot change {Type} to {value}.");
            Type = value;
        }

        /// <summary>
        /// Replaces the current <see cref="Data"/> with a same-sized array <see cref="value"/>.
        /// </summary>
        /// <param name="value">New array to load (copy from).</param>
        /// <remarks>Will throw if the requested block state changes are incorrect.</remarks>
        public void ChangeData(ReadOnlySpan<byte> value)
        {
            if (value.Length != Data.Length)
                throw new InvalidOperationException($"Cannot change size of {Type} block from {Data.Length} to {value.Length}.");
            value.CopyTo(Data);
        }

        internal SCBlock(uint key, SCTypeCode type) : this(key, type, Array.Empty<byte>())
        {
        }

        internal SCBlock(uint key, SCTypeCode type, byte[] arr)
        {
            Key = key;
            Type = type;
            Data = arr;
        }

        internal SCBlock(uint key, byte[] arr, SCTypeCode subType)
        {
            Key = key;
            Type = SCTypeCode.Array;
            Data = arr;
            SubType = subType;
        }

        public bool HasValue() => Type > SCTypeCode.Array;
        public object GetValue() => Type.GetValue(Data);
        public void SetValue(object value) => Type.SetValue(Data, value);

        public SCBlock Clone()
        {
            if (Data.Length == 0)
                return new SCBlock(Key, Type);
            if (SubType == 0)
                return new SCBlock(Key, Type, (byte[]) Data.Clone());
            return new SCBlock(Key, (byte[])Data.Clone(), SubType);
        }

        /// <summary>
        /// Encrypts the <see cref="Data"/> according to the <see cref="Type"/> and <see cref="SubType"/>.
        /// </summary>
        public void WriteBlock(BinaryWriter bw)
        {
            var xk = new SCXorShift32(Key);
            bw.Write(Key);
            bw.Write((byte)((byte)Type ^ xk.Next()));

            if (Type == SCTypeCode.Object)
            {
                bw.Write((uint)Data.Length ^ xk.Next32());
            }
            else if (Type == SCTypeCode.Array)
            {
                var entries = (uint)(Data.Length / SubType.GetTypeSize());
                bw.Write(entries ^ xk.Next32());
                bw.Write((byte)((byte)SubType ^ xk.Next()));
            }

            foreach (var b in Data)
                bw.Write((byte)(b ^ xk.Next()));
        }

        /// <summary>
        /// Reads a new <see cref="SCBlock"/> object from the <see cref="data"/>, determining the <see cref="Type"/> and <see cref="SubType"/> during read.
        /// </summary>
        /// <param name="data">Decrypted data</param>
        /// <param name="offset">Offset the block is to be read from (modified to offset by the amount of bytes consumed).</param>
        /// <returns>New object containing all info for the block.</returns>
        public static SCBlock ReadFromOffset(byte[] data, ref int offset)
        {
            // Create block, parse its key.
            var key = BitConverter.ToUInt32(data, offset);
            offset += 4;
            var xk = new SCXorShift32(key);

            // Parse the block's type
            //var block = 
            var type = (SCTypeCode)(data[offset++] ^ xk.Next());

            switch (type)
            {
                case SCTypeCode.Bool1:
                case SCTypeCode.Bool2:
                case SCTypeCode.Bool3:
                    // Block types are empty, and have no extra data.
                    Debug.Assert(type != SCTypeCode.Bool3); // invalid type, haven't seen it used yet
                    return new SCBlock(key, type);

                case SCTypeCode.Object: // Cast raw bytes to Object
                {
                    var num_bytes = BitConverter.ToUInt32(data, offset) ^ xk.Next32();
                    offset += 4;
                    var arr = new byte[num_bytes];
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = (byte)(data[offset++] ^ xk.Next());

                    return new SCBlock(key, type, arr);
                }

                case SCTypeCode.Array: // Cast raw bytes to SubType[]
                {
                    var num_entries = BitConverter.ToUInt32(data, offset) ^ xk.Next32();
                    offset += 4;
                    var sub = (SCTypeCode)(data[offset++] ^ xk.Next());

                    var num_bytes = num_entries * sub.GetTypeSize();
                    var arr = new byte[num_bytes];
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = (byte)(data[offset++] ^ xk.Next());
#if DEBUG
                    Debug.Assert(sub > SCTypeCode.Array || Array.TrueForAll(arr, z => z <= 1));
#endif
                    return new SCBlock(key, arr, sub);
                }

                default: // Single Value Storage
                {
                    var num_bytes = type.GetTypeSize();
                    var arr = new byte[num_bytes];
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = (byte)(data[offset++] ^ xk.Next());
                    return new SCBlock(key, type, arr);
                }
            }
        }
    }
}
