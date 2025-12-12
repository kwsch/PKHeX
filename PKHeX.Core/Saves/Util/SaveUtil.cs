using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using static PKHeX.Core.MessageStrings;
using static PKHeX.Core.SaveFileType;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for <see cref="SaveFile"/> data loading and manipulation.
/// </summary>
public static class SaveUtil
{
    private const int SIZE_G9ZA_100 = 0x2F3284;
    private const int SIZE_G9ZA_102 = 0x2F3289; // +5 for Ranked bug fix boolean block
    private const int SIZE_G9ZA_200 = 0x309FA6; // Mega Dimension DLC

    private const int SIZE_G9_0   = 0x31626F; // 1.0.0 fresh
    private const int SIZE_G9_0a  = 0x31627C; // 1.0.0 after multiplayer
    private const int SIZE_G9_1   = 0x319DB3; // 1.0.1 fresh
    private const int SIZE_G9_1a  = 0x319DC0; // 1.0.1 after multiplayer
    private const int SIZE_G9_3   = 0x319DC3; // 1.1.0 fresh
    private const int SIZE_G9_1Ba = 0x319DD0; // 1.0.1 -> 1.1.0
    private const int SIZE_G9_1A  = 0x31A2C0; // 1.0.0 -> 1.0.1
    private const int SIZE_G9_1Aa = 0x31A2CD; // 1.0.0 -> 1.0.1 -> 1.0.1 after multiplayer
    private const int SIZE_G9_1Ab = 0x31A2DD; // 1.0.0 -> 1.0.1 -> 1.0.1 after multiplayer -> 1.1.0
    private const int SIZE_G9_2   = 0x31A2D0; // 1.0.0 -> 1.1.0

    // 1.2.0: add 0x2C9F; clean upgrade (1.1.0->1.2.0 is same as *1.2.0)
    private const int SIZE_G9_3B1 = SIZE_G9_3A1 - 0xD; // BM
    private const int SIZE_G9_3P1 = SIZE_G9_3B1 + 0x5; // GO (before Multiplayer)
    private const int SIZE_G9_3A1 = 0x31CA6F; //          1.0.1 -> 1.1.0 -> 1.2.0 AM
    private const int SIZE_G9_3G1 = SIZE_G9_3A1 + 0x5; // GO

    private const int SIZE_G9_3B0 = SIZE_G9_3A0 - 0xD; // BM
    private const int SIZE_G9_3P0 = SIZE_G9_3B0 + 0x5; // GO (before Multiplayer)
    private const int SIZE_G9_3A0 = 0x31CF7C; // 1.0.0 -> 1.0.1 -> 1.1.0 -> 1.2.0 AM
    private const int SIZE_G9_3G0 = SIZE_G9_3A0 + 0x5; // GO

    // 2.0.1 (2.0.0 skipped): Teal Mask
    // 3.0.0: The Indigo Disk
    // Both save file sizes have a ton of optional blocks, so just accept a range (min/max) and rely on the hash validity.
    private const int SIZE_G9_DLC1_MIN1 = 0x4329A0;
    private const int SIZE_G9_DLC1_MAX1 = 0x4329C9;
    private const int SIZE_G9_DLC1_MIN2 = 0x432EB6;
    private const int SIZE_G9_DLC1_MAX2 = 0x432ED6;
    private const int SIZE_G9_202 = 0xC8E; // Add 2 blocks (1 obj 0xC80, 1 bool) = 4{key}1{obj}4{len} + 4{key}1{boolT/boolF}
    private const int SIZE_G9_300 = 0x83AD;

    private const int SIZE_G8LA = 0x136DDE;
    private const int SIZE_G8LA_1 = 0x13AD06;

    public const int SIZE_G8BDSP_0 = 0xE9828;
    public const int SIZE_G8BDSP_1 = 0xEDC20;
    public const int SIZE_G8BDSP_2 = 0xEED8C;
    public const int SIZE_G8BDSP_3 = 0xEF0A4;

    private const int SIZE_G8SWSH    = 0x1716B3; // 1.0
    private const int SIZE_G8SWSH_1  = 0x17195E; // 1.0 -> 1.1
    private const int SIZE_G8SWSH_2  = 0x180B19; // 1.0 -> 1.1 -> 1.2
    private const int SIZE_G8SWSH_2B = 0x180AD0; // 1.0 -> 1.2
    private const int SIZE_G8SWSH_3  = 0x1876B1; // 1.0 -> 1.1 -> 1.2 -> 1.3
    private const int SIZE_G8SWSH_3A = 0x187693; // 1.0 -> 1.1 -> 1.3
    private const int SIZE_G8SWSH_3B = 0x187668; // 1.0 -> 1.2 -> 1.3
    private const int SIZE_G8SWSH_3C = 0x18764A; // 1.0 -> 1.3

    public const int SIZE_G7GG = 0x100000;
    public const int SIZE_G7USUM = 0x6CC00;
    public const int SIZE_G7SM = 0x6BE00;
    public const int SIZE_G6XY = 0x65600;
    public const int SIZE_G6ORAS = 0x76000;
    public const int SIZE_G6ORASDEMO = 0x5A00;
    public const int SIZE_G5RAW = 0x80000;
    public const int SIZE_G5BW = 0x24000;
    public const int SIZE_G5B2W2 = 0x26000;
    public const int SIZE_G4BR = 0x380000;
    public const int SIZE_G4RAW = 0x80000;
    public const int SIZE_G3BOX = 0x76000;
    public const int SIZE_G3COLO = 0x60000;
    public const int SIZE_G3XD = 0x56000;
    public const int SIZE_G3RAW = 0x20000;
    public const int SIZE_G3RAWHALF = 0x10000;
    public const int SIZE_G2STAD = 0x20000; // same as G3RAW
    public const int SIZE_G2STADF = 0x1FF00;
    public const int SIZE_G2RAW_U = 0x8000;
    public const int SIZE_G2RAW_J = 0x10000;
    public const int SIZE_G1STAD = 0x20000; // same as G3RAW
    public const int SIZE_G1STADF = 0x1FF00;
    public const int SIZE_G1STADJ = 0x8000; // same as G1RAW
    public const int SIZE_G1RAW = 0x8000;

    // Bank Binaries
    public const int SIZE_G7BANK = 0xACA48;
    public const int SIZE_G4BANK = 0x405C4;
    public const int SIZE_G4RANCH = 0x54000;
    public const int SIZE_G4RANCH_PLAT = 0x7C000;

    private static readonly SaveHandlerGCI DolphinHandler = new();

    /// <summary>
    /// Specialized readers for loading save files from non-standard games (e.g. hacks) or containers.
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public static readonly List<ISaveReader> CustomSaveReaders =
    [
        new ZipReader(),
    ];

    /// <summary>
    /// Pre-formatters for loading save files from non-standard formats (e.g. emulators).
    /// </summary>
    public static readonly List<ISaveHandler> Handlers =
    [
        new SaveHandlerFooterRTC(),
        DolphinHandler,
        new SaveHandlerDeSmuME(),
        new SaveHandlerARDS(),
        new SaveHandlerNSO(),
    ];

    private static bool IsSizeGen9ZA(int length) => length is SIZE_G9ZA_100 or SIZE_G9ZA_102 or SIZE_G9ZA_200;

    private static bool IsSizeGen9SV(int length) => length is
        SIZE_G9_0 or SIZE_G9_0a or
        SIZE_G9_1 or SIZE_G9_1a or SIZE_G9_1A or SIZE_G9_1Aa or SIZE_G9_1Ba or SIZE_G9_1Ab or
        SIZE_G9_2 or
        SIZE_G9_3 or
        SIZE_G9_3A0 or SIZE_G9_3A1 or SIZE_G9_3B0 or SIZE_G9_3B1 or
        SIZE_G9_3G0 or SIZE_G9_3G1 or SIZE_G9_3P0 or SIZE_G9_3P1 or
        // tons of optional blocks, just rely on range(+) and hash validity
        (>= SIZE_G9_DLC1_MIN1 and <= SIZE_G9_DLC1_MAX1 + 100) or
        (>= SIZE_G9_DLC1_MIN2 and <= SIZE_G9_DLC1_MAX2 + 100) or
        (>= SIZE_G9_DLC1_MIN1 + SIZE_G9_202 and <= SIZE_G9_DLC1_MAX1 + SIZE_G9_202 + 100) or
        (>= SIZE_G9_DLC1_MIN2 + SIZE_G9_202 and <= SIZE_G9_DLC1_MAX2 + SIZE_G9_202 + 100) or
        (>= SIZE_G9_DLC1_MIN1 + SIZE_G9_300 and <= SIZE_G9_DLC1_MAX1 + SIZE_G9_300 + 100) or
        (>= SIZE_G9_DLC1_MIN2 + SIZE_G9_300 and <= SIZE_G9_DLC1_MAX2 + SIZE_G9_300 + 100)
    ;

    private static bool IsSizeGen8SWSH(int length) => length is SIZE_G8SWSH
        or SIZE_G8SWSH_1
        or SIZE_G8SWSH_2 or SIZE_G8SWSH_2B
        or SIZE_G8SWSH_3 or SIZE_G8SWSH_3A or SIZE_G8SWSH_3B or SIZE_G8SWSH_3C;

    private static bool IsSizeGen2(int length) => length is SIZE_G2RAW_U or SIZE_G2RAW_J;

    private static bool IsSizeCommonFixed(int length) => length is
        SIZE_G8LA or SIZE_G8LA_1
        or SIZE_G8BDSP_0 or SIZE_G8BDSP_1 or SIZE_G8BDSP_2 or SIZE_G8BDSP_3
        or SIZE_G7SM or SIZE_G7USUM or SIZE_G7GG
        or SIZE_G6XY or SIZE_G6ORAS or SIZE_G6ORASDEMO
        or SIZE_G5RAW or SIZE_G5BW or SIZE_G5B2W2
        or SIZE_G4BR
        or SIZE_G3COLO or SIZE_G3XD or SIZE_G3RAW or SIZE_G3RAWHALF
        or SIZE_G2RAW_U
        or SIZE_G7BANK or SIZE_G4BANK or SIZE_G4RANCH or SIZE_G4RANCH_PLAT;

    /// <summary>Determines the type of the provided save data.</summary>
    /// <param name="data">Save data of which to determine the origins of</param>
    /// <returns>Save file type information including sub-version details, or Invalid if type cannot be determined.</returns>
    private static SaveTypeInfo GetTypeInfo(ReadOnlySpan<byte> data)
    {
        // Mainline
        if (IsG1(data, out var info)) return info;
        if (IsG2(data, out info)) return info;
        if (IsG3(data, out var smallOffset)) return GetVersionG3SAV(data[smallOffset..]);
        if (IsG4DP(data)) return DP;
        if (IsG4Pt(data)) return Pt;
        if (IsG4HGSS(data)) return HGSS;
        if (IsG5BW(data)) return BW;
        if (IsG5B2W2(data)) return B2W2;
        if (IsG6XY(data)) return XY;
        if (IsG6AO(data)) return AO;
        if (IsG6AODemo(data)) return AODemo;
        if (IsG7SM(data)) return SM;
        if (IsG7USUM(data)) return USUM;
        if (IsG7LGPE(data)) return LGPE;
        if (IsG8SWSH(data)) return SWSH;
        if (IsG8BDSP(data)) return BDSP;
        if (IsG8LA(data)) return LA;
        if (IsG9SV(data)) return SV;
        if (IsG9ZA(data)) return ZA;

        // Side-game
        if (IsG3Colosseum(data)) return Colosseum;
        if (IsG3XD(data)) return XD;
        if (IsG3RSBox(data)) return RSBox;
        if (IsG4BR(data)) return BattleRevolution;

        // Adjacent/misc.
        if (IsBank7(data)) return Bulk7; // pokebank
        if (IsBank4(data)) return Bulk4; // pokestock
        if (IsBank3(data)) return Bulk3; // pokestock
        if (IsRanch4(data)) return Ranch;
        if (SAV2Stadium.IsStadium(data)) return Stadium2;
        if (SAV1Stadium.IsStadium(data)) return Stadium1;
        if (SAV1StadiumJ.IsStadium(data)) return Stadium1J;

        return SaveTypeInfo.Invalid;
    }

    private static bool IsG1INT(ReadOnlySpan<byte> data) => HasListAt(data, 0x2F2C, 0x30C0, 20);
    private static bool IsG1JPN(ReadOnlySpan<byte> data) => HasListAt(data, 0x2ED5, 0x302D, 30);
    private static bool IsG2GSINT(ReadOnlySpan<byte> data) => HasListAt(data, 0x288A, 0x2D6C, 20);
    private static bool IsG2GSJPN(ReadOnlySpan<byte> data) => HasListAt(data, 0x2D10, 0x283E, 30);
    private static bool IsG2GSKOR(ReadOnlySpan<byte> data) => HasListAt(data, 0x2DAE, 0x28CC, 20);
    private static bool IsG2CrystalINT(ReadOnlySpan<byte> data) => HasListAt(data, 0x2865, 0x2D10, 20);
    private static bool IsG2CrystalJPN(ReadOnlySpan<byte> data) => HasListAt(data, 0x2D10, 0x281A, 30);
    private static bool HasListAt(ReadOnlySpan<byte> data, [ConstantExpected] int offset1, [ConstantExpected] int offset2, [ConstantExpected] byte maxCount) =>
        IsListValidG12(data, offset1, maxCount) && IsListValidG12(data, offset2, maxCount);

    /// <summary>
    /// Determines if a Gen1/2 Pokémon List is Invalid
    /// </summary>
    /// <param name="data">Save data</param>
    /// <param name="offset">Offset the list starts at</param>
    /// <param name="maxCount">Max count of Pokémon in the list</param>
    /// <returns>True if a valid list, False otherwise</returns>
    private static bool IsListValidG12(ReadOnlySpan<byte> data, int offset, [ConstantExpected] byte maxCount)
    {
        byte count = data[offset];
        return count <= maxCount && data[offset + 1 + count] == 0xFF;
    }

    /// <summary>Checks to see if the data belongs to a Gen1 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <param name="info">Recognized save file type, if any.</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static bool IsG1(ReadOnlySpan<byte> data, out SaveTypeInfo info)
    {
        info = default;
        if (data.Length is not SIZE_G1RAW)
            return false;

        // Check if it's not an international save or a Japanese save
        if (IsG1JPN(data))
        { info = new SaveTypeInfo(RBY, SAV1.IsYellowJPN(data) ? GameVersion.YW : default, LanguageID.Japanese); return true; }
        if (IsG1INT(data))
        { info = new SaveTypeInfo(RBY, SAV1.IsYellowINT(data) ? GameVersion.YW : default); return true; }

        return false;
    }

    /// <summary>Checks to see if the data belongs to a Gen2 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <param name="info">Recognized save file type, if any.</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static bool IsG2(ReadOnlySpan<byte> data, out SaveTypeInfo info)
    {
        info = default;
        if (!IsSizeGen2(data.Length))
            return false;

        // Check if it's not an International, Japanese, or Korean save file
        // International
        if (IsG2GSINT(data))
        { info = new SaveTypeInfo(GSC, GameVersion.GS); return true; }
        if (IsG2CrystalINT(data))
        { info = new SaveTypeInfo(GSC, GameVersion.C); return true; }

        // Japanese
        if (IsG2GSJPN(data))
        { info = new SaveTypeInfo(GSC, GameVersion.GS, LanguageID.Japanese); return true; }
        if (IsG2CrystalJPN(data))
        { info = new SaveTypeInfo(GSC, GameVersion.C, LanguageID.Japanese); return true; }

        // Korean
        if (IsG2GSKOR(data))
        { info = new SaveTypeInfo(GSC, GameVersion.GS, LanguageID.Korean); return true; }
        return false;
    }

    /// <summary>Checks to see if the data belongs to a Gen3 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <param name="smallOffset">Offset to the small sector of the save file, if applicable.</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static bool IsG3(ReadOnlySpan<byte> data, out int smallOffset)
    {
        smallOffset = 0;
        if (data.Length is not SIZE_G3RAW)
            return false;

        // check the save file(s)
        int count = data.Length/SIZE_G3RAWHALF;
        for (int slot = 0; slot < count; slot++)
        {
            if (SAV3.IsAllMainSectorsPresent(data, slot, out smallOffset))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks the input <see cref="data"/> to see which game is for this file.
    /// </summary>
    /// <param name="data">Data to check</param>
    /// <returns>RS, E, or FR/LG.</returns>
    private static SaveFileType GetVersionG3SAV(ReadOnlySpan<byte> data)
    {
        // 0xAC
        // RS: Battle Tower Data, which will never match the FR/LG fixed value.
        // E: Encryption Key
        // FR/LG @ 0xAC has a fixed value (01 00 00 00)
        // RS has battle tower data (variable)
        uint _0xAC = ReadUInt32LittleEndian(data[0xAC..]);
        switch (_0xAC)
        {
            case 1: return FRLG; // fixed value
            case 0: return RS; // save has no battle tower record data
            default:
                // RS data structure only extends 0x890 bytes; check if any data is present afterward.
                var remainder = data[0x890..0xF2C];
                if (remainder.ContainsAnyExcept<byte>(0))
                    return Emerald;
                return RS;
        }
    }

    /// <summary>Checks to see if the data belongs to a Gen3 Box RS save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static bool IsG3RSBox(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G3BOX)
            return false;

        // Verify first checksum
        const int offset = 0x2000;
        var span = data.Slice(offset, 0x1FFC);
        var actual = ReadUInt32BigEndian(span);
        var chk = Checksums.CheckSum16BigInvert(span[4..]);
        return chk == actual;
    }

    /// <summary>Checks to see if the data belongs to a Colosseum save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static bool IsG3Colosseum(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G3COLO)
            return false;

        // Check the intro bytes for each save slot
        const int offset = 0x6000;
        for (int i = 0; i < 3; i++)
        {
            var ofs = offset + (0x1E000 * i);
            if (ReadUInt32LittleEndian(data[ofs..]) != 0x00000101)
                return false;
        }
        return true;
    }

    /// <summary>Checks to see if the data belongs to a Gen3 XD save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static bool IsG3XD(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G3XD)
            return false;

        // Check the intro bytes for each save slot
        const int offset = 0x6000;
        for (int i = 0; i < 2; i++)
        {
            var ofs = offset + (0x28000 * i);
            if ((ReadUInt32LittleEndian(data[ofs..]) & 0xFFFE_FFFF) != 0x00000101)
                return false;
        }
        return true;
    }

    private static bool IsG4DP(ReadOnlySpan<byte> data) => data.Length == SIZE_G4RAW && IsValidGeneralFooter2(data, SAV4DP.GeneralSize);
    private static bool IsG4Pt(ReadOnlySpan<byte> data) => data.Length == SIZE_G4RAW && IsValidGeneralFooter2(data, SAV4Pt.GeneralSize);
    private static bool IsG4HGSS(ReadOnlySpan<byte> data) => data.Length == SIZE_G4RAW && IsValidGeneralFooter2(data, SAV4HGSS.GeneralSize);
    private static bool IsG4BR(ReadOnlySpan<byte> data) => data.Length == SIZE_G4BR && SAV4BR.IsValidSaveFile(data);
    private static bool IsG5BW(ReadOnlySpan<byte> data) => data.Length == SIZE_G5RAW && IsValidFooter5(data, SIZE_G5BW, 0x8C);
    private static bool IsG5B2W2(ReadOnlySpan<byte> data) => data.Length == SIZE_G5RAW && IsValidFooter5(data, SIZE_G5B2W2, 0x94);
    private static bool IsG6XY(ReadOnlySpan<byte> data) => data.Length == SIZE_G6XY && HasSaveFooterBEEF(data);
    private static bool IsG6AO(ReadOnlySpan<byte> data) => data.Length == SIZE_G6ORAS && HasSaveFooterBEEF(data);
    private static bool IsG6AODemo(ReadOnlySpan<byte> data) => data.Length == SIZE_G6ORASDEMO && HasSaveFooterBEEF(data);
    private static bool IsG7SM(ReadOnlySpan<byte> data) => data.Length is (SIZE_G7SM) && HasSaveFooterBEEF(data);
    private static bool IsG7USUM(ReadOnlySpan<byte> data) => data.Length is (SIZE_G7USUM) && HasSaveFooterBEEF(data);

    private static bool IsValidGeneralFooter2(ReadOnlySpan<byte> data, [ConstantExpected] int length)
    {
        // Check the other save -- first save is done to the latter half of the binary.
        // The second save should be all that is needed to check.
        const int generalOffset = 0x40000;
        var general = data.Slice(generalOffset, length);

        // The block footers contain a 32-bit 'size' followed by a 32-bit binary-coded-decimal timestamp
        // Korean saves have a different timestamp from other localizations.
        var size = ReadUInt32LittleEndian(general[^0xC..]);
        if (size != general.Length)
            return false;
        var sdk = ReadUInt32LittleEndian(general[^0x8..]);
        return sdk is SAV4.MAGIC_JAPAN_INTL or SAV4.MAGIC_KOREAN;
    }

    private static bool IsValidFooter5(ReadOnlySpan<byte> data, int mainSize, int infoLength)
    {
        var footer = data.Slice(mainSize - 0x100, infoLength + 0x10);
        var stored = ReadUInt16LittleEndian(footer[^2..]);
        var actual = Checksums.CRC16_CCITT(footer[..infoLength]);
        return stored == actual;
    }

    private static bool HasSaveFooterBEEF(ReadOnlySpan<byte> data) => ReadUInt32LittleEndian(data[^0x1F0..]) == 0x42454546; // BEEF

    /// <summary>Determines if the input data belongs to a <see cref="SAV7b"/> save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static bool IsG7LGPE(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE_G7GG)
            return false;

        data = data[..0xB8800];
        const int actualLength = 0xB8800;
        if (!HasSaveFooterBEEF(data))
            return false;
        if (ReadUInt16LittleEndian(data[(actualLength - 0x200 + 0xB0)..]) != 0x13) // check a block number to double-check
            return false;

        return true;
    }

    private static bool IsG8BDSP(ReadOnlySpan<byte> data) => data.Length switch
    {
        SIZE_G8BDSP_0 => (Gem8Version)ReadUInt32LittleEndian(data) == Gem8Version.V1_0,
        SIZE_G8BDSP_1 => (Gem8Version)ReadUInt32LittleEndian(data) == Gem8Version.V1_1,
        SIZE_G8BDSP_2 => (Gem8Version)ReadUInt32LittleEndian(data) == Gem8Version.V1_2,
        SIZE_G8BDSP_3 => (Gem8Version)ReadUInt32LittleEndian(data) == Gem8Version.V1_3,
        _ => false
    };

    private static bool IsG8LA(ReadOnlySpan<byte> data) => data.Length is SIZE_G8LA or SIZE_G8LA_1 && SwishCrypto.GetIsHashValid(data);
    private static bool IsG8SWSH(ReadOnlySpan<byte> data) => IsSizeGen8SWSH(data.Length) && SwishCrypto.GetIsHashValid(data);
    private static bool IsG9SV(ReadOnlySpan<byte> data) => IsSizeGen9SV(data.Length) && SwishCrypto.GetIsHashValid(data);
    private static bool IsG9ZA(ReadOnlySpan<byte> data) => IsSizeGen9ZA(data.Length) && SwishCrypto.GetIsHashValid(data);

    private static bool IsBank7(ReadOnlySpan<byte> data) => data.Length == SIZE_G7BANK && data[0] != 0;
    private static bool IsBank4(ReadOnlySpan<byte> data) => data.Length == SIZE_G4BANK && ReadUInt32LittleEndian(data[0x3FC00..]) != 0; // box name present
    private static bool IsBank3(ReadOnlySpan<byte> data) => data.Length == SIZE_G4BANK && ReadUInt32LittleEndian(data[0x3FC00..]) == 0; // size collision with ^
    private static bool IsRanchDP(ReadOnlySpan<byte> data) => data.Length == SIZE_G4RANCH && ReadUInt32BigEndian(data[0x22AC..]) != 0;
    private static bool IsRanchPlat(ReadOnlySpan<byte> data) => data.Length == SIZE_G4RANCH_PLAT && ReadUInt32BigEndian(data[0x268C..]) != 0;
    private static bool IsRanch4(ReadOnlySpan<byte> data) => IsRanchDP(data) || IsRanchPlat(data);

    /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
    /// <param name="path">File location from which to create a SaveFile.</param>
    /// <param name="result">Save file loaded from the given path, or null if loading failed.</param>
    public static bool TryGetSaveFile(string path, [NotNullWhen(true)] out SaveFile? result)
    {
        // Many things can go wrong with loading save data (file no longer present toc-tou, or bad save layout).
        try
        {
            var data = File.ReadAllBytes(path);
            if (!TryGetSaveFile(data, out result, path))
                return false;

            result.Metadata.SetExtraInfo(path);
            if (result.Generation <= 3)
                SaveLanguage.TryRevise(result);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            result = null;
            return false;
        }
    }

    /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
    /// <param name="path">File location from which to create a SaveFile.</param>
    /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
    public static SaveFile? GetSaveFile(string path) => TryGetSaveFile(path, out var result) ? result : null;

    /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
    /// <param name="data">Save data from which to create a SaveFile.</param>
    /// <param name="path">File path, may help initialize a non-standard save file format.</param>
    /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
    public static SaveFile? GetSaveFile(Memory<byte> data, string? path = null) => TryGetSaveFile(data, out var result, path) ? result : null;

    /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
    /// <param name="data">Save data from which to create a SaveFile.</param>
    /// <param name="result">Save file loaded from the given data, or null if loading failed.</param>
    /// <param name="path">Optional save file path, may help initialize a non-standard save file format.</param>
    /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
    public static bool TryGetSaveFile(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path = null)
    {
        if (TryGetSaveFileCustom(data, out result, path))
            return true;

        result = GetSaveFileInternal(data);
        if (result is not null)
            return true;

        if (TryGetSaveFileHandler(data, out result, path))
            return true;

        // unrecognized.
        return false;
    }

    /// <summary>
    /// Overrides the detected save type with the manually specified <see cref="SaveTypeInfo"/>.
    /// </summary>
    /// <param name="sav">Save file to override</param>
    /// <param name="toType">Manually specified save type information</param>
    /// <param name="result">Overridden save file, or null if the override failed.</param>
    /// <returns><see langword="true"/> if the override was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryOverride(SaveFile sav, SaveTypeInfo toType, [NotNullWhen(true)] out SaveFile? result)
    {
        try
        {
            var data = sav.Buffer;
            result = GetSaveFileInternal(data, toType);
            if (result is null)
                return false;

            result.Metadata.ShareExtraInfo(sav.Metadata);
            return true;
        }
        catch
        {
            // General failure, probably bad type selected / bad data.
            result = null;
            return false;
        }
    }

    private static bool TryGetSaveFileCustom(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path)
    {
        foreach (var h in CustomSaveReaders)
        {
            if (!h.IsRecognized(data.Length))
                continue;

            if (h.TryRead(data, out result, path))
                return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Tries to get the save file from all existing handlers.
    /// </summary>
    public static bool TryGetSaveFileHandler(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path)
    {
        foreach (var h in Handlers)
        {
            if (TryGetSaveFileHandler(data, out result, path, h))
                return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Tries to get the save file from all existing handlers using a specific <see cref="SaveTypeInfo"/>.
    /// </summary>
    public static bool TryGetSaveFileHandler(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path, SaveTypeInfo typeInfo)
    {
        foreach (var h in Handlers)
        {
            if (TryGetSaveFileHandler(data, out result, path, h, typeInfo))
                return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Tries to get the save file from a specific handler.
    /// </summary>
    public static bool TryGetSaveFileHandler(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path, ISaveHandler h)
    {
        result = null;
        if (!h.IsRecognized(data.Length))
            return false;

        var split = h.TrySplit(data);
        if (split is null)
            return false;

        result = GetSaveFileInternal(split.Data);
        if (result is null)
            return false;

        var meta = result.Metadata;
        meta.SetExtraInfo(split.Header, split.Footer, split.Handler);
        if (path is not null)
            meta.SetExtraInfo(path);
        return true;
    }

    /// <summary>
    /// Tries to get the save file from a specific handler with specified type info.
    /// </summary>
    public static bool TryGetSaveFileHandler(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path, ISaveHandler h, SaveTypeInfo typeInfo)
    {
        result = null;
        if (!h.IsRecognized(data.Length))
            return false;

        var split = h.TrySplit(data);
        if (split is null)
            return false;

        result = GetSaveFileInternal(data, typeInfo);
        if (result is null)
            return false;

        var meta = result.Metadata;
        meta.SetExtraInfo(split.Header, split.Footer, split.Handler);
        if (path is not null)
            meta.SetExtraInfo(path);
        return true;
    }

    private static SaveFile? GetSaveFileInternal(Memory<byte> data)
    {
        var typeInfo = GetTypeInfo(data.Span);
        return GetSaveFileInternal(data, typeInfo);
    }

    private static SaveFile? GetSaveFileInternal(Memory<byte> data, SaveTypeInfo typeInfo) => typeInfo.Type switch
    {
        // Main Games
        RBY => new SAV1(data, typeInfo.Language, typeInfo.SubVersion),
        GSC => new SAV2(data, typeInfo.Language, typeInfo.SubVersion),

        RS => new SAV3RS(data),
        Emerald => new SAV3E(data),
        FRLG => new SAV3FRLG(data),

        DP => new SAV4DP(data),
        Pt => new SAV4Pt(data),
        HGSS => new SAV4HGSS(data),

        BW => new SAV5BW(data),
        B2W2 => new SAV5B2W2(data),

        XY => new SAV6XY(data),
        AO => new SAV6AO(data),
        AODemo => new SAV6AODemo(data),

        SM => new SAV7SM(data),
        USUM => new SAV7USUM(data),
        LGPE => new SAV7b(data),

        SWSH => new SAV8SWSH(data),
        BDSP => new SAV8BS(data),
        LA => new SAV8LA(data),

        SV => new SAV9SV(data),
        ZA => new SAV9ZA(data),

        // Side Games
        Colosseum => new SAV3Colosseum(data),
        XD => new SAV3XD(data),
        RSBox => new SAV3RSBox(data),
        BattleRevolution => new SAV4BR(data),
        Stadium2 => new SAV2Stadium(data),
        Stadium1 => new SAV1Stadium(data),
        Stadium1J => new SAV1StadiumJ(data),

        // Bulk Storage
        Ranch => new SAV4Ranch(data),
        Bulk3 => new Bank3(data),
        Bulk4 => new Bank4(data),
        Bulk7 => Bank7.GetBank7(data),

        // No pattern matched
        _ => null,
    };

    public static bool TryGetSaveFile(SAV3GCMemoryCard memCard, [NotNullWhen(true)] out SaveFile? result)
    {
        // Pre-check for header/footer signatures
        if (memCard.IsNoGameSelected)
            memCard.GetMemoryCardState();

        result = null;
        var peek = memCard.ReadSaveGameData();
        if (peek.Length == 0)
            return false;

        var memory = peek.ToArray();
        var split = DolphinHandler.TrySplit(memory);
        var data = split?.Data ?? memory;

        SaveFile sav;
        switch (memCard.SelectedGameVersion)
        {
            // Side Games
            case Colosseum: sav = new SAV3Colosseum(data) { MemoryCard = memCard }; break;
            case XD: sav = new SAV3XD(data) { MemoryCard = memCard }; break;
            case RSBox: sav = new SAV3RSBox(data) { MemoryCard = memCard }; break;

            // No pattern matched
            default: return false;
        }

        if (split is not null)
            sav.Metadata.SetExtraInfo(split.Header, split.Footer, split.Handler);
        result = sav;
        return true;
    }

    /// <summary>
    /// Retrieves possible save file paths from the provided <see cref="folderPath"/>.
    /// </summary>
    /// <param name="folderPath">Folder to look within</param>
    /// <param name="deep">Search all subfolders</param>
    /// <param name="token">Cancellation token to cancel the operation.</param>
    /// <param name="result">If this function returns true, full path of all <see cref="SaveFile"/> that match criteria. If this function returns false, the error message, or null if the directory could not be found</param>
    /// <param name="ignoreBackups">Option to ignore files with backup names and extensions</param>
    /// <returns>Boolean indicating if the operation was successful.</returns>
    public static bool GetSavesFromFolder(string folderPath, bool deep, CancellationToken token, out IEnumerable<string> result, bool ignoreBackups = true)
    {
        if (!Directory.Exists(folderPath))
        {
            result = [];
            return false;
        }
        try
        {
            var searchOption = deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.EnumerateFiles(folderPath, "*", searchOption)
                .IterateSafe(log: z => System.Diagnostics.Debug.WriteLine(z));
            result = FilterSaveFiles(ignoreBackups, files, token);
            return true;
        }
        catch (Exception ex)
        {
            result =
            [
                MsgFileLoadFailAuto + Environment.NewLine + folderPath,
                MsgFileLoadFailAutoAdvise + Environment.NewLine + MsgFileLoadFailAutoCause,
                ex.Message,
            ];
            return false;
        }
    }

    private static IEnumerable<string> FilterSaveFiles(bool ignoreBackups, IEnumerable<string> files, CancellationToken token)
    {
        foreach (var file in files)
        {
            if (token.IsCancellationRequested)
                yield break;

            if (ignoreBackups && IsBackupFilePath(file))
                continue;

            var size = FileUtil.GetFileSize(file);
            if (!IsSizeValid(size))
                continue;

            yield return file;
        }
    }

    public static bool IsBackupFilePath(ReadOnlySpan<char> path)
    {
        // Gen8+ store main,backup,poke_trade
        var fn = Path.GetFileNameWithoutExtension(path);
        if (fn is "backup")
            return true;

        // Programs storing backups via .bak extension
        var ext = Path.GetExtension(path);
        return ext is ".bak";
    }

    /// <summary>
    /// Determines whether the save data size is valid for automatically detecting saves.
    /// </summary>
    /// <param name="size">Size in bytes of the save data</param>
    /// <returns>A boolean indicating if the save data size is valid.</returns>
    public static bool IsSizeValid(long size) => IsSizeValidNoHandler(size) || IsSizeValidHandler(size) || SAV3GCMemoryCard.IsMemoryCardSize(size);

    /// <summary>
    /// Determines whether the save data size is valid for automatically detecting saves.
    /// </summary>
    /// <remarks>Only checks the <see cref="Handlers"/> list.</remarks>
    public static bool IsSizeValidHandler(long size) => Handlers.Exists(z => z.IsRecognized(size));

    /// <summary>
    /// Determines whether the save data size is valid for automatically detecting saves.
    /// </summary>
    /// <remarks>Does not check the <see cref="Handlers"/> list.</remarks>
    public static bool IsSizeValidNoHandler(long size)
    {
        if ((uint)size > int.MaxValue)
            return false;
        int length = (int)size;
        if (IsSizeGen9ZA(length))
            return true;
        if (IsSizeGen9SV(length))
            return true;
        if (IsSizeGen8SWSH(length))
            return true;
        if (IsSizeCommonFixed(length))
            return true;
        return false;
    }
}
