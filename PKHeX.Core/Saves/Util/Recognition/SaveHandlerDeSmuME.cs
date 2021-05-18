using System;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for recognizing .dsv save files from DeSmuME.
    /// </summary>
    public class SaveHandlerDeSmuME : ISaveHandler
    {
        private const int sizeFooter = 0x7A;
        private const int ExpectedSize = SaveUtil.SIZE_G4RAW + sizeFooter;

        private static readonly byte[] FOOTER_DSV = Encoding.ASCII.GetBytes("|-DESMUME SAVE-|");

        private static bool GetHasFooterDSV(byte[] input)
        {
            var signature = FOOTER_DSV;
            var start = input.Length - signature.Length;
            return input.AsSpan(start).SequenceEqual(signature);
        }

        public bool IsRecognized(int size) => size is ExpectedSize;

        public SaveHandlerSplitResult? TrySplit(byte[] input)
        {
            if (!GetHasFooterDSV(input))
                return null;

            var footer = input.SliceEnd(SaveUtil.SIZE_G4RAW);
            input = input.Slice(0, SaveUtil.SIZE_G4RAW);

            return new SaveHandlerSplitResult(input, Array.Empty<byte>(), footer);
        }
    }
}
