using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for recognizing .duc save files dumped via an ARDS.
    /// </summary>
    public class SaveHandlerARDS : ISaveHandler
    {
        private const int sizeHeader = 0xA4;
        private const int ExpectedSize = SaveUtil.SIZE_G4RAW + sizeHeader; // 0x800A4

        public bool IsRecognized(int size) => size is ExpectedSize;

        public SaveHandlerSplitResult TrySplit(byte[] input)
        {
            // No authentication to see if it actually is a header; no size collisions expected.
            var header = input.Slice(0, sizeHeader);
            input = input.SliceEnd(sizeHeader);
            return new SaveHandlerSplitResult(input, header, Array.Empty<byte>());
        }
    }
}
