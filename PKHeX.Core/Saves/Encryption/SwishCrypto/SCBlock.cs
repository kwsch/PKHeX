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
        public SCTypeCode Type { get; set; }

        /// <summary>
        /// For <see cref="SCTypeCode.Array"/>: What kind of array is it?
        /// </summary>
        public SCTypeCode SubType { get; private set; }

        /// <summary>
        /// Decrypted data for this block.
        /// </summary>
        public byte[] Data = Array.Empty<byte>();

        internal SCBlock(uint key) => Key = key;

        public bool HasValue() => Type > SCTypeCode.Array;
        public object GetValue() => Type.GetValue(Data);
        public void SetValue(object value) => Type.SetValue(Data, value);

        public SCBlock Clone()
        {
            var block = (SCBlock)MemberwiseClone();
            block.Data = (byte[])Data.Clone();
            return block;
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
            var block = new SCBlock(key);
            var xk = new SCXorShift32(key);

            // Parse the block's type
            block.Type = (SCTypeCode)(data[offset++] ^ xk.Next());

            switch (block.Type)
            {
                case SCTypeCode.Bool1:
                case SCTypeCode.Bool2:
                case SCTypeCode.Bool3:
                    // Block types are empty, and have no extra data.
                    Debug.Assert(block.Type != SCTypeCode.Bool3); // invalid type, haven't seen it used yet
                    break;

                case SCTypeCode.Object: // Cast raw bytes to Object
                {
                    var num_bytes = BitConverter.ToUInt32(data, offset) ^ xk.Next32();
                    offset += 4;
                    var arr = new byte[num_bytes];
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = (byte)(data[offset++] ^ xk.Next());
                    block.Data = arr;
                    break;
                }

                case SCTypeCode.Array: // Cast raw bytes to SubType[]
                {
                    var num_entries = BitConverter.ToUInt32(data, offset) ^ xk.Next32();
                    offset += 4;
                    block.SubType = (SCTypeCode)(data[offset++] ^ xk.Next());

                    var num_bytes = num_entries * block.SubType.GetTypeSize();
                    var arr = new byte[num_bytes];
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = (byte)(data[offset++] ^ xk.Next());
                    block.Data = arr;
#if DEBUG
                    Debug.Assert(block.SubType > SCTypeCode.Array || Array.TrueForAll(block.Data, z => z <= 1));
#endif
                    break;
                }

                default: // Single Value Storage
                {
                    var num_bytes = block.Type.GetTypeSize();
                    var arr = new byte[num_bytes];
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = (byte)(data[offset++] ^ xk.Next());
                    block.Data = arr;
                    break;
                }
            }

            return block;
        }
    }
}
