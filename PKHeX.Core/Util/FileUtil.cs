using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for detecting supported binary object formats.
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// Attempts to get a binary object from the provided path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="reference">Reference savefile used for PC Binary compatibility checks.</param>
        /// <returns>Supported file object reference, null if none found.</returns>
        public static object? GetSupportedFile(string path, SaveFile? reference = null)
        {
            try
            {
                var fi = new FileInfo(path);
                if (IsFileTooBig(fi.Length) || IsFileTooSmall(fi.Length))
                    return null;

                var data = File.ReadAllBytes(path);
                var ext = Path.GetExtension(path);
                return GetSupportedFile(data, ext, reference);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // User input data can be fuzzed; if anything blows up, just fail safely.
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Debug.WriteLine(MessageStrings.MsgFileInUse);
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Attempts to get a binary object from the provided inputs.
        /// </summary>
        /// <param name="data">Binary data for the file.</param>
        /// <param name="ext">File extension used as a hint.</param>
        /// <param name="reference">Reference savefile used for PC Binary compatibility checks.</param>
        /// <returns>Supported file object reference, null if none found.</returns>
        public static object? GetSupportedFile(byte[] data, string ext, SaveFile? reference = null)
        {
            if (TryGetSAV(data, out var sav))
                return sav;
            if (TryGetMemoryCard(data, out var mc))
                return mc;
            if (TryGetPKM(data, out var pk, ext))
                return pk;
            if (TryGetPCBoxBin(data, out IEnumerable<byte[]> pks, reference))
                return pks;
            if (TryGetBattleVideo(data, out var bv))
                return bv;
            if (TryGetMysteryGift(data, out var g, ext))
                return g;
            if (TryGetGP1(data, out var gp))
                return gp;
            if (TryGetBundle(data, out var bundle))
                return bundle;
            return null;
        }

        public static bool IsFileLocked(string path)
        {
            try { return (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0; }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { return true; }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static int GetFileSize(string path)
        {
            try
            {
                var size = new FileInfo(path).Length;
                if (size > int.MaxValue)
                    return -1;
                return (int)size;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { return -1; } // Bad File / Locked
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static bool TryGetGP1(byte[] data, [NotNullWhen(true)] out GP1? gp1)
        {
            gp1 = null;
            if (data.Length != GP1.SIZE || BitConverter.ToUInt32(data, 0x28) == 0)
                return false;
            gp1 = new GP1(data);
            return true;
        }

        private static bool TryGetBundle(byte[] data, [NotNullWhen(true)] out IPokeGroup? result)
        {
            result = null;
            if (RentalTeam8.IsRentalTeam(data))
            {
                result = new RentalTeam8(data);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the length is too big to be a detectable file.
        /// </summary>
        /// <param name="length">File size</param>
        public static bool IsFileTooBig(long length)
        {
            if (length <= 0x10_0000) // 1 MB
                return false;
            if (length > int.MaxValue)
                return true;
            if (SaveUtil.IsSizeValid((int)length))
                return false;
            if (SAV3GCMemoryCard.IsMemoryCardSize(length))
                return false; // pbr/GC have size > 1MB
            return true;
        }

        /// <summary>
        /// Checks if the length is too small to be a detectable file.
        /// </summary>
        /// <param name="length">File size</param>
        public static bool IsFileTooSmall(long length) => length < 0x20; // bigger than PK1

        /// <summary>
        /// Tries to get an <see cref="SaveFile"/> object from the input parameters.
        /// </summary>
        /// <param name="data">Binary data</param>
        /// <param name="sav">Output result</param>
        /// <returns>True if file object reference is valid, false if none found.</returns>
        public static bool TryGetSAV(byte[] data, [NotNullWhen(true)] out SaveFile? sav)
        {
            sav = SaveUtil.GetVariantSAV(data);
            return sav != null;
        }

        /// <summary>
        /// Tries to get an <see cref="SAV3GCMemoryCard"/> object from the input parameters.
        /// </summary>
        /// <param name="data">Binary data</param>
        /// <param name="memcard">Output result</param>
        /// <returns>True if file object reference is valid, false if none found.</returns>
        public static bool TryGetMemoryCard(byte[] data, [NotNullWhen(true)] out SAV3GCMemoryCard? memcard)
        {
            if (!SAV3GCMemoryCard.IsMemoryCardSize(data))
            {
                memcard = null;
                return false;
            }
            memcard = new SAV3GCMemoryCard(data);
            return true;
        }

        /// <summary>
        /// Tries to get an <see cref="PKM"/> object from the input parameters.
        /// </summary>
        /// <param name="data">Binary data</param>
        /// <param name="pk">Output result</param>
        /// <param name="ext">Format hint</param>
        /// <param name="sav">Reference savefile used for PC Binary compatibility checks.</param>
        /// <returns>True if file object reference is valid, false if none found.</returns>
        public static bool TryGetPKM(byte[] data, [NotNullWhen(true)] out PKM? pk, string ext, ITrainerInfo? sav = null)
        {
            if (ext == ".pgt") // size collision with pk6
            {
                pk = null;
                return false;
            }
            var format = PKX.GetPKMFormatFromExtension(ext, sav?.Generation ?? 6);
            pk = PKMConverter.GetPKMfromBytes(data, prefer: format);
            return pk != null;
        }

        /// <summary>
        /// Tries to get an <see cref="IEnumerable{T}"/> object from the input parameters.
        /// </summary>
        /// <param name="data">Binary data</param>
        /// <param name="pkms">Output result</param>
        /// <param name="sav">Reference savefile used for PC Binary compatibility checks.</param>
        /// <returns>True if file object reference is valid, false if none found.</returns>
        public static bool TryGetPCBoxBin(byte[] data, out IEnumerable<byte[]> pkms, SaveFile? sav)
        {
            if (sav == null)
            {
                pkms = Array.Empty<byte[]>();
                return false;
            }
            var length = data.Length;
            if (PKX.IsPKM(length / sav.SlotCount) || PKX.IsPKM(length / sav.BoxSlotCount))
            {
                pkms = ArrayUtil.EnumerateSplit(data, length);
                return true;
            }
            pkms = Array.Empty<byte[]>();
            return false;
        }

        /// <summary>
        /// Tries to get a <see cref="BattleVideo"/> object from the input parameters.
        /// </summary>
        /// <param name="data">Binary data</param>
        /// <param name="bv">Output result</param>
        /// <returns>True if file object reference is valid, false if none found.</returns>
        public static bool TryGetBattleVideo(byte[] data, [NotNullWhen(true)] out BattleVideo? bv)
        {
            bv = BattleVideo.GetVariantBattleVideo(data);
            return bv != null;
        }

        /// <summary>
        /// Tries to get a <see cref="MysteryGift"/> object from the input parameters.
        /// </summary>
        /// <param name="data">Binary data</param>
        /// <param name="mg">Output result</param>
        /// <param name="ext">Format hint</param>
        /// <returns>True if file object reference is valid, false if none found.</returns>
        public static bool TryGetMysteryGift(byte[] data, [NotNullWhen(true)] out MysteryGift? mg, string ext)
        {
            mg = MysteryGift.GetMysteryGift(data, ext);
            return mg != null;
        }

        /// <summary>
        /// Gets a Temp location File Name for the <see cref="PKM"/>.
        /// </summary>
        /// <param name="pk">Data to be exported</param>
        /// <param name="encrypt">Data is to be encrypted</param>
        /// <returns>Path to temporary file location to write to.</returns>
        public static string GetPKMTempFileName(PKM pk, bool encrypt)
        {
            string fn = pk.FileNameWithoutExtension;
            string filename = fn + (encrypt ? $".ek{pk.Format}" : $".{pk.Extension}");

            return Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
        }

        /// <summary>
        /// Gets a <see cref="PKM"/> from the provided <see cref="file"/> path, which is to be loaded to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="file"><see cref="PKM"/> or <see cref="MysteryGift"/> file path.</param>
        /// <param name="sav">Generation Info</param>
        /// <returns>New <see cref="PKM"/> reference from the file.</returns>
        public static PKM? GetSingleFromPath(string file, ITrainerInfo sav)
        {
            var fi = new FileInfo(file);
            if (!fi.Exists)
                return null;
            if (fi.Length == GP1.SIZE && TryGetGP1(File.ReadAllBytes(file), out var gp1))
                return gp1.ConvertToPB7(sav);
            if (!PKX.IsPKM(fi.Length) && !MysteryGift.IsMysteryGift(fi.Length))
                return null;
            var data = File.ReadAllBytes(file);
            var ext = fi.Extension;
            var mg = MysteryGift.GetMysteryGift(data, ext);
            var gift = mg?.ConvertToPKM(sav);
            if (gift != null)
                return gift;
            int prefer = PKX.GetPKMFormatFromExtension(ext, sav.Generation);
            return PKMConverter.GetPKMfromBytes(data, prefer: prefer);
        }
    }
}
