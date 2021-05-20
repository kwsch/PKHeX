using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for recognizing .gci save files.
    /// </summary>
    public class SaveHandlerGCI : ISaveHandler
    {
        private const int headerSize = 0x40;
        private const int SIZE_G3BOXGCI  = headerSize + SaveUtil.SIZE_G3BOX; // GCI data
        private const int SIZE_G3COLOGCI = headerSize + SaveUtil.SIZE_G3COLO; // GCI data
        private const int SIZE_G3XDGCI   = headerSize + SaveUtil.SIZE_G3XD; // GCI data

        private static readonly string[] HEADER_COLO  = { "GC6J", "GC6E", "GC6P" }; // NTSC-J, NTSC-U, PAL
        private static readonly string[] HEADER_XD    = { "GXXJ", "GXXE", "GXXP" }; // NTSC-J, NTSC-U, PAL
        private static readonly string[] HEADER_RSBOX = { "GPXJ", "GPXE", "GPXP" }; // NTSC-J, NTSC-U, PAL

        private static bool IsGameMatchHeader(IEnumerable<string> headers, byte[] data) => headers.Contains(Encoding.ASCII.GetString(data, 0, 4));

        public bool IsRecognized(int size) => size is SIZE_G3BOXGCI or SIZE_G3COLOGCI or SIZE_G3XDGCI;

        public SaveHandlerSplitResult? TrySplit(byte[] input)
        {
            switch (input.Length)
            {
                case SIZE_G3COLOGCI when IsGameMatchHeader(HEADER_COLO , input):
                case SIZE_G3XDGCI   when IsGameMatchHeader(HEADER_XD   , input):
                case SIZE_G3BOXGCI  when IsGameMatchHeader(HEADER_RSBOX, input):
                    break;
                default:
                    return null;
            }

            byte[] header = input.Slice(0, headerSize);
            input = input.SliceEnd(headerSize);

            return new SaveHandlerSplitResult(input, header, Array.Empty<byte>());
        }

        /// <summary>
        /// Checks if the game code is one of the recognizable versions.
        /// </summary>
        /// <param name="gameCode">4 character game code string</param>
        /// <returns>Magic version ID enumeration; <see cref="GameVersion.Unknown"/> if no match.</returns>
        public static GameVersion GetGameCode(string gameCode)
        {
            if (HEADER_COLO.Contains(gameCode))
                return GameVersion.COLO;
            if (HEADER_XD.Contains(gameCode))
                return GameVersion.XD;
            if (HEADER_RSBOX.Contains(gameCode))
                return GameVersion.RSBOX;

            return GameVersion.Unknown;
        }
    }
}
