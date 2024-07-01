using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.MessageStrings;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic for <see cref="SaveFile"/> data loading and manipulation.
/// </summary>
public static class SaveUtil
{
    public const int BEEF = 0x42454546;

    public const int SIZE_G9_0   = 0x31626F; // 1.0.0 fresh
    public const int SIZE_G9_0a  = 0x31627C; // 1.0.0 after multiplayer
    public const int SIZE_G9_1   = 0x319DB3; // 1.0.1 fresh
    public const int SIZE_G9_1a  = 0x319DC0; // 1.0.1 after multiplayer
    public const int SIZE_G9_3   = 0x319DC3; // 1.1.0 fresh
    public const int SIZE_G9_1Ba = 0x319DD0; // 1.0.1 -> 1.1.0
    public const int SIZE_G9_1A  = 0x31A2C0; // 1.0.0 -> 1.0.1
    public const int SIZE_G9_1Aa = 0x31A2CD; // 1.0.0 -> 1.0.1 -> 1.0.1 after multiplayer
    public const int SIZE_G9_1Ab = 0x31A2DD; // 1.0.0 -> 1.0.1 -> 1.0.1 after multiplayer -> 1.1.0
    public const int SIZE_G9_2   = 0x31A2D0; // 1.0.0 -> 1.1.0

    // 1.2.0: add 0x2C9F; clean upgrade (1.1.0->1.2.0 is same as *1.2.0)
    public const int SIZE_G9_3A0 = 0x31CF7C; // 1.0.0 -> 1.0.1 -> 1.1.0 -> 1.2.0 AM
    public const int SIZE_G9_3A1 = 0x31CA6F; //          1.0.1 -> 1.1.0 -> 1.2.0 AM
    public const int SIZE_G9_3B0 = SIZE_G9_3A0 - 0xD; // BM
    public const int SIZE_G9_3B1 = SIZE_G9_3A1 - 0xD; // BM
    public const int SIZE_G9_3G0 = SIZE_G9_3A0 + 0x5; // GO
    public const int SIZE_G9_3G1 = SIZE_G9_3A1 + 0x5; // GO
    public const int SIZE_G9_3P0 = SIZE_G9_3B0 + 0x5; // GO (before Multiplayer)
    public const int SIZE_G9_3P1 = SIZE_G9_3B1 + 0x5; // GO (before Multiplayer)

    // 2.0.1 (2.0.0 skipped): Teal Mask
    public const int SIZE_G9_DLC1_B = 0x4329A0;
    public const int SIZE_G9_DLC1_T = 0x4329A5; // +5
    public const int SIZE_G9_DLC1_0 = 0x4329A9; // +4
    public const int SIZE_G9_DLC1_E = 0x4329AD; // +1
    public const int SIZE_G9_DLC1_1 = 0x4329AE; // +1
    public const int SIZE_G9_DLC1_D = 0x4329B2; // +4
    public const int SIZE_G9_DLC1_F = 0x4329B3; // +1
    public const int SIZE_G9_DLC1_A = 0x4329B6; // +3
    public const int SIZE_G9_DLC1_Q = 0x4329B7; // +1
    public const int SIZE_G9_DLC1_2 = 0x4329BB; // +5
    public const int SIZE_G9_DLC1_U = 0x4329BC; // +1
    public const int SIZE_G9_DLC1_W = 0x4329BF; // +4
    public const int SIZE_G9_DLC1_3 = 0x4329C0; // +5
    public const int SIZE_G9_DLC1_9 = 0x4329C4;
    public const int SIZE_G9_DLC1_V = 0x4329C9;
    public const int SIZE_G9_DLC1_4 = 0x432EB6;
    public const int SIZE_G9_DLC1_C = 0x432EBB; // +5
    public const int SIZE_G9_DLC1_5 = 0x432EBF; // +4
    public const int SIZE_G9_DLC1_H = 0x432EC0; // +1
    public const int SIZE_G9_DLC1_6 = 0x432EC3; // +4
    public const int SIZE_G9_DLC1_G = 0x432EC4; // +1
    public const int SIZE_G9_DLC1_7 = 0x432EC8; // +5
    public const int SIZE_G9_DLC1_I = 0x432EC9; // +1
    public const int SIZE_G9_DLC1_R = 0x432ECC; // +1
    public const int SIZE_G9_DLC1_8 = 0x432ECD; // +5
    public const int SIZE_G9_DLC1_Y = 0x432ED1; // +1
    public const int SIZE_G9_DLC1_Z = 0x432ED6; // +5

    public const int SIZE_G8LA = 0x136DDE;
    public const int SIZE_G8LA_1 = 0x13AD06;

    public const int SIZE_G8BDSP = 0xE9828;
    public const int SIZE_G8BDSP_1 = 0xEDC20;
    public const int SIZE_G8BDSP_2 = 0xEED8C;
    public const int SIZE_G8BDSP_3 = 0xEF0A4;

    public const int SIZE_G8SWSH = 0x1716B3; // 1.0
    public const int SIZE_G8SWSH_1 = 0x17195E; // 1.0 -> 1.1
    public const int SIZE_G8SWSH_2 = 0x180B19; // 1.0 -> 1.1 -> 1.2
    public const int SIZE_G8SWSH_2B = 0x180AD0; // 1.0 -> 1.2
    public const int SIZE_G8SWSH_3 = 0x1876B1; // 1.0 -> 1.1 -> 1.2 -> 1.3
    public const int SIZE_G8SWSH_3A = 0x187693; // 1.0 -> 1.1 -> 1.3
    public const int SIZE_G8SWSH_3B = 0x187668; // 1.0 -> 1.2 -> 1.3
    public const int SIZE_G8SWSH_3C = 0x18764A; // 1.0 -> 1.3

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

#if !EXCLUDE_HACKS
    /// <summary>
    /// Specialized readers for loading save files from non-standard games (e.g. hacks).
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public static readonly List<ISaveReader> CustomSaveReaders = [];
#endif

#if !EXCLUDE_EMULATOR_FORMATS
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
#endif

    private const int SIZE_G9_202 = 0xC8E; // Add 2 blocks (1 obj 0xC80, 1 bool) = 4{key}1{obj}4{len} + 4{key}1{boolT/boolF}
    private const int SIZE_G9_300 = 0x83AD;

    private static readonly HashSet<long> SizesSV =
    [
        SIZE_G9_0, SIZE_G9_0a,
        SIZE_G9_1, SIZE_G9_1a,
        SIZE_G9_1A, SIZE_G9_1Aa,
        SIZE_G9_1Ba, SIZE_G9_1Ab,
        SIZE_G9_2, SIZE_G9_3,

        SIZE_G9_3A0, SIZE_G9_3A1,
        SIZE_G9_3B0, SIZE_G9_3B1,
        SIZE_G9_3G0, SIZE_G9_3G1,
        SIZE_G9_3P0, SIZE_G9_3P1,

        SIZE_G9_DLC1_0, SIZE_G9_DLC1_0 + SIZE_G9_202, SIZE_G9_DLC1_0 + SIZE_G9_300,
        SIZE_G9_DLC1_1, SIZE_G9_DLC1_1 + SIZE_G9_202, SIZE_G9_DLC1_1 + SIZE_G9_300,
        SIZE_G9_DLC1_2, SIZE_G9_DLC1_2 + SIZE_G9_202, SIZE_G9_DLC1_2 + SIZE_G9_300,
        SIZE_G9_DLC1_3, SIZE_G9_DLC1_3 + SIZE_G9_202, SIZE_G9_DLC1_3 + SIZE_G9_300,
        SIZE_G9_DLC1_4, SIZE_G9_DLC1_4 + SIZE_G9_202, SIZE_G9_DLC1_4 + SIZE_G9_300,
        SIZE_G9_DLC1_5, SIZE_G9_DLC1_5 + SIZE_G9_202, SIZE_G9_DLC1_5 + SIZE_G9_300,
        SIZE_G9_DLC1_6, SIZE_G9_DLC1_6 + SIZE_G9_202, SIZE_G9_DLC1_6 + SIZE_G9_300,
        SIZE_G9_DLC1_7, SIZE_G9_DLC1_7 + SIZE_G9_202, SIZE_G9_DLC1_7 + SIZE_G9_300,
        SIZE_G9_DLC1_8, SIZE_G9_DLC1_8 + SIZE_G9_202, SIZE_G9_DLC1_8 + SIZE_G9_300,
        SIZE_G9_DLC1_9, SIZE_G9_DLC1_9 + SIZE_G9_202, SIZE_G9_DLC1_9 + SIZE_G9_300,
        SIZE_G9_DLC1_A, SIZE_G9_DLC1_A + SIZE_G9_202, SIZE_G9_DLC1_A + SIZE_G9_300,
        SIZE_G9_DLC1_B, SIZE_G9_DLC1_B + SIZE_G9_202, SIZE_G9_DLC1_B + SIZE_G9_300,
        SIZE_G9_DLC1_C, SIZE_G9_DLC1_C + SIZE_G9_202, SIZE_G9_DLC1_C + SIZE_G9_300,
        SIZE_G9_DLC1_D, SIZE_G9_DLC1_D + SIZE_G9_202, SIZE_G9_DLC1_D + SIZE_G9_300,
        SIZE_G9_DLC1_E, SIZE_G9_DLC1_E + SIZE_G9_202, SIZE_G9_DLC1_E + SIZE_G9_300,
        SIZE_G9_DLC1_F, SIZE_G9_DLC1_F + SIZE_G9_202, SIZE_G9_DLC1_F + SIZE_G9_300,
        SIZE_G9_DLC1_G, SIZE_G9_DLC1_G + SIZE_G9_202, SIZE_G9_DLC1_G + SIZE_G9_300,
        SIZE_G9_DLC1_H, SIZE_G9_DLC1_H + SIZE_G9_202, SIZE_G9_DLC1_H + SIZE_G9_300,
        SIZE_G9_DLC1_I, SIZE_G9_DLC1_I + SIZE_G9_202, SIZE_G9_DLC1_I + SIZE_G9_300,
        SIZE_G9_DLC1_Q, SIZE_G9_DLC1_Q + SIZE_G9_202, SIZE_G9_DLC1_Q + SIZE_G9_300,
        SIZE_G9_DLC1_W, SIZE_G9_DLC1_W + SIZE_G9_202, SIZE_G9_DLC1_W + SIZE_G9_300,
        SIZE_G9_DLC1_R, SIZE_G9_DLC1_R + SIZE_G9_202, SIZE_G9_DLC1_R + SIZE_G9_300,
        SIZE_G9_DLC1_T, SIZE_G9_DLC1_T + SIZE_G9_202, SIZE_G9_DLC1_T + SIZE_G9_300,
        SIZE_G9_DLC1_Y, SIZE_G9_DLC1_Y + SIZE_G9_202, SIZE_G9_DLC1_Y + SIZE_G9_300,
        SIZE_G9_DLC1_U, SIZE_G9_DLC1_U + SIZE_G9_202, SIZE_G9_DLC1_U + SIZE_G9_300,
        SIZE_G9_DLC1_V, SIZE_G9_DLC1_V + SIZE_G9_202, SIZE_G9_DLC1_V + SIZE_G9_300,
        SIZE_G9_DLC1_Z, SIZE_G9_DLC1_Z + SIZE_G9_202, SIZE_G9_DLC1_Z + SIZE_G9_300,
    ];

    private static readonly HashSet<long> SizesSWSH =
    [
        SIZE_G8SWSH, SIZE_G8SWSH_1, SIZE_G8SWSH_2, SIZE_G8SWSH_2B, SIZE_G8SWSH_3, SIZE_G8SWSH_3A, SIZE_G8SWSH_3B, SIZE_G8SWSH_3C,
    ];

    private static readonly HashSet<long> SizesGen2 =
    [
        SIZE_G2RAW_U, SIZE_G2RAW_J,
    ];

    private static readonly HashSet<long> Sizes =
    [
        ..SizesGen2, ..SizesSWSH, ..SizesSV,
        SIZE_G8LA, SIZE_G8LA_1, SIZE_G8BDSP, SIZE_G8BDSP_1, SIZE_G8BDSP_2, SIZE_G8BDSP_3,
        // SizesSWSH covers Gen8 sizes since there's so many
        SIZE_G7SM, SIZE_G7USUM, SIZE_G7GG,
        SIZE_G6XY, SIZE_G6ORAS, SIZE_G6ORASDEMO,
        SIZE_G5RAW, SIZE_G5BW, SIZE_G5B2W2,
        SIZE_G4BR, SIZE_G4RAW,
        SIZE_G3BOX, SIZE_G3COLO, SIZE_G3XD, SIZE_G3RAW, SIZE_G3RAWHALF,
        // SizesGen2 covers Gen2 sizes since there's so many
        SIZE_G1RAW,

        SIZE_G7BANK, SIZE_G4BANK, SIZE_G4RANCH, SIZE_G4RANCH_PLAT,
    ];

    /// <summary>Determines the type of the provided save data.</summary>
    /// <param name="data">Save data of which to determine the origins of</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetSAVType(ReadOnlySpan<byte> data)
    {
        GameVersion version;
        if ((version = GetIsG1SAV(data)) != Invalid)
            return version;
        if ((version = GetIsG2SAV(data)) != Invalid)
            return version;
        if ((version = GetIsG3SAV(data)) != Invalid)
            return version;
        if ((version = GetIsG4SAV(data)) != Invalid)
            return version;
        if ((version = GetIsG5SAV(data)) != Invalid)
            return version;
        if ((version = GetIsG6SAV(data)) != Invalid)
            return version;
        if ((version = GetIsG7SAV(data)) != Invalid)
            return version;

        if (GetIsBelugaSAV(data) != Invalid)
            return GG;
        if (GetIsG3COLOSAV(data) != Invalid)
            return COLO;
        if (GetIsG3XDSAV(data) != Invalid)
            return XD;
        if (GetIsG3BOXSAV(data) != Invalid)
            return RSBOX;
        if (GetIsG4BRSAV(data) != Invalid)
            return BATREV;

        if (GetIsBank7(data)) // pokebank
            return Gen7;
        if (GetIsBank4(data)) // pokestock
            return Gen4;
        if (GetIsBank3(data)) // pokestock
            return Gen3;
        if (GetIsRanch4(data)) // ranch
            return DPPt;
        if (SAV2Stadium.IsStadium(data))
            return Stadium2;
        if (SAV1Stadium.IsStadium(data))
            return Stadium;
        if (SAV1StadiumJ.IsStadium(data))
            return StadiumJ;

        if ((version = GetIsG8SAV(data)) != Invalid)
            return version;
        if ((version = GetIsG8SAV_BDSP(data)) != Invalid)
            return version;
        if ((version = GetIsG8SAV_LA(data)) != Invalid)
            return version;
        if ((version = GetIsG9SAV(data)) != Invalid)
            return version;

        return Invalid;
    }

    /// <summary>
    /// Determines if a Gen1/2 Pokémon List is Invalid
    /// </summary>
    /// <param name="data">Save data</param>
    /// <param name="offset">Offset the list starts at</param>
    /// <param name="listCount">Max count of Pokémon in the list</param>
    /// <returns>True if a valid list, False otherwise</returns>
    private static bool IsG12ListValid(ReadOnlySpan<byte> data, int offset, int listCount)
    {
        byte num_entries = data[offset];
        return num_entries <= listCount && data[offset + 1 + num_entries] == 0xFF;
    }

    /// <summary>Checks to see if the data belongs to a Gen1 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    internal static GameVersion GetIsG1SAV(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G1RAW)
            return Invalid;

        // Check if it's not an american save or a japanese save
        if (!(GetIsG1SAVU(data) || GetIsG1SAVJ(data)))
            return Invalid;
        // I can't actually detect which game version, because it's not stored anywhere.
        // If you can think of anything to do here, please implement :)
        return RBY;
    }

    /// <summary>Checks to see if the data belongs to an International Gen1 save</summary>
    /// <param name="data">Save data of which to determine the region</param>
    /// <returns>True if a valid International save, False otherwise.</returns>
    private static bool GetIsG1SAVU(ReadOnlySpan<byte> data)
    {
        return IsG12ListValid(data, 0x2F2C, 20) && IsG12ListValid(data, 0x30C0, 20);
    }

    /// <summary>Checks to see if the data belongs to a Japanese Gen1 save</summary>
    /// <param name="data">Save data of which to determine the region</param>
    /// <returns>True if a valid Japanese save, False otherwise.</returns>
    internal static bool GetIsG1SAVJ(ReadOnlySpan<byte> data)
    {
        return IsG12ListValid(data, 0x2ED5, 30) && IsG12ListValid(data, 0x302D, 30);
    }

    /// <summary>Checks to see if the data belongs to a Gen2 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    internal static GameVersion GetIsG2SAV(ReadOnlySpan<byte> data)
    {
        if (!SizesGen2.Contains(data.Length))
            return Invalid;

        // Check if it's not an International, Japanese, or Korean save file
        GameVersion result;
        if ((result = GetIsG2SAVU(data)) != Invalid)
            return result;
        if ((result = GetIsG2SAVJ(data)) != Invalid)
            return result;
        if ((result = GetIsG2SAVK(data)) != Invalid)
            return result;
        return Invalid;
    }

    /// <summary>Checks to see if the data belongs to an International (not Japanese or Korean) Gen2 save</summary>
    /// <param name="data">Save data of which to determine the region</param>
    /// <returns>True if a valid International save, False otherwise.</returns>
    private static GameVersion GetIsG2SAVU(ReadOnlySpan<byte> data)
    {
        if (IsG12ListValid(data, 0x288A, 20) && IsG12ListValid(data, 0x2D6C, 20))
            return GS;
        if (IsG12ListValid(data, 0x2865, 20) && IsG12ListValid(data, 0x2D10, 20))
            return C;
        return Invalid;
    }

    /// <summary>Checks to see if the data belongs to a Japanese Gen2 save</summary>
    /// <param name="data">Save data of which to determine the region</param>
    /// <returns>True if a valid Japanese save, False otherwise.</returns>
    internal static GameVersion GetIsG2SAVJ(ReadOnlySpan<byte> data)
    {
        if (!IsG12ListValid(data, 0x2D10, 30))
            return Invalid;
        if (IsG12ListValid(data, 0x283E, 30))
            return GS;
        if (IsG12ListValid(data, 0x281A, 30))
            return C;
        return Invalid;
    }

    /// <summary>Checks to see if the data belongs to a Korean Gen2 save</summary>
    /// <param name="data">Save data of which to determine the region</param>
    /// <returns>True if a valid Korean save, False otherwise.</returns>
    internal static GameVersion GetIsG2SAVK(ReadOnlySpan<byte> data)
    {
        if (IsG12ListValid(data, 0x2DAE, 20) && IsG12ListValid(data, 0x28CC, 20))
            return GS;
        return Invalid;
    }

    /// <summary>Checks to see if the data belongs to a Gen3 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG3SAV(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G3RAW)
            return Invalid;

        // check the save file(s)
        int count = data.Length/SIZE_G3RAWHALF;
        for (int slot = 0; slot < count; slot++)
        {
            if (!SAV3.IsAllMainSectorsPresent(data, slot, out var smallOffset))
                continue;

            // Detect RS/E/FRLG
            return GetVersionG3SAV(data[smallOffset..]);
        }
        return Invalid;
    }

    /// <summary>
    /// Checks the input <see cref="data"/> to see which game is for this file.
    /// </summary>
    /// <param name="data">Data to check</param>
    /// <returns>RS, E, or FR/LG.</returns>
    private static GameVersion GetVersionG3SAV(ReadOnlySpan<byte> data)
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
                    return E;
                return RS;
        }
    }

    /// <summary>Checks to see if the data belongs to a Gen3 Box RS save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG3BOXSAV(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G3BOX)
            return Invalid;

        // Verify first checksum
        const int offset = 0x2000;
        var span = data.Slice(offset, 0x1FFC);
        var actual = ReadUInt32BigEndian(span);
        var chk = Checksums.CheckSum16BigInvert(span[4..]);
        return chk == actual ? RSBOX : Invalid;
    }

    /// <summary>Checks to see if the data belongs to a Colosseum save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG3COLOSAV(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G3COLO)
            return Invalid;

        // Check the intro bytes for each save slot
        const int offset = 0x6000;
        for (int i = 0; i < 3; i++)
        {
            var ofs = offset + (0x1E000 * i);
            if (ReadUInt32LittleEndian(data[ofs..]) != 0x00000101)
                return Invalid;
        }
        return COLO;
    }

    /// <summary>Checks to see if the data belongs to a Gen3 XD save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG3XDSAV(ReadOnlySpan<byte> data)
    {
        if (data.Length is not SIZE_G3XD)
            return Invalid;

        // Check the intro bytes for each save slot
        const int offset = 0x6000;
        for (int i = 0; i < 2; i++)
        {
            var ofs = offset + (0x28000 * i);
            if ((ReadUInt32LittleEndian(data[ofs..]) & 0xFFFE_FFFF) != 0x00000101)
                return Invalid;
        }
        return XD;
    }

    /// <summary>Checks to see if the data belongs to a Gen4 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG4SAV(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE_G4RAW)
            return Invalid;

        // Check the other save -- first save is done to the latter half of the binary.
        // The second save should be all that is needed to check.
        const int generalOffset = 0x40000;
        if (IsValidGeneralFooter(data.Slice(generalOffset, SAV4DP.GeneralSize)))
            return DP;
        if (IsValidGeneralFooter(data.Slice(generalOffset, SAV4Pt.GeneralSize)))
            return Pt;
        if (IsValidGeneralFooter(data.Slice(generalOffset, SAV4HGSS.GeneralSize)))
            return HGSS;

        return Invalid;

        // The block footers contain a 32-bit 'size' followed by a 32-bit binary-coded-decimal timestamp
        // Korean saves have a different timestamp from other localizations.
        static bool IsValidGeneralFooter(ReadOnlySpan<byte> general)
        {
            var size = ReadUInt32LittleEndian(general[^0xC..]);
            if (size != general.Length)
                return false;
            var sdk = ReadUInt32LittleEndian(general[^0x8..]);
            return sdk is SAV4.MAGIC_JAPAN_INTL or SAV4.MAGIC_KOREAN;
        }
    }

    /// <summary>Checks to see if the data belongs to a Gen4 Battle Revolution save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG4BRSAV(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE_G4BR)
            return Invalid;

        byte[] sav = SAV4BR.DecryptPBRSaveData(data);
        return SAV4BR.IsChecksumsValid(sav) ? BATREV : Invalid;
    }

    /// <summary>Checks to see if the data belongs to a Gen5 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG5SAV(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE_G5RAW)
            return Invalid;

        // check the checksum footer block validity; nobody would normally modify this region
        if (IsValidFooter(data, SIZE_G5BW, 0x8C))
            return BW;
        if (IsValidFooter(data, SIZE_G5B2W2, 0x94))
            return B2W2;
        return Invalid;

        static bool IsValidFooter(ReadOnlySpan<byte> data, int mainSize, int infoLength)
        {
            var footer = data.Slice(mainSize - 0x100, infoLength + 0x10);
            ushort stored = ReadUInt16LittleEndian(footer[^2..]);
            ushort actual = Checksums.CRC16_CCITT(footer[..infoLength]);
            return stored == actual;
        }
    }

    /// <summary>Checks to see if the data belongs to a Gen6 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG6SAV(ReadOnlySpan<byte> data)
    {
        if (data.Length is not (SIZE_G6XY or SIZE_G6ORAS or SIZE_G6ORASDEMO))
            return Invalid;

        if (ReadUInt32LittleEndian(data[^0x1F0..]) != BEEF)
            return Invalid;

        return data.Length switch
        {
            SIZE_G6XY => XY,
            SIZE_G6ORAS => ORAS,
            _ => ORASDEMO, // least likely
        };
    }

    /// <summary>Checks to see if the data belongs to a Gen7 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG7SAV(ReadOnlySpan<byte> data)
    {
        if (data.Length is not (SIZE_G7SM or SIZE_G7USUM))
            return Invalid;

        if (ReadUInt32LittleEndian(data[^0x1F0..]) != BEEF)
            return Invalid;

        return data.Length == SIZE_G7SM ? SM : USUM;
    }

    /// <summary>Determines if the input data belongs to a <see cref="SAV7b"/> save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsBelugaSAV(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE_G7GG)
            return Invalid;

        const int actualLength = 0xB8800;
        if (ReadUInt32LittleEndian(data[(actualLength - 0x1F0)..]) != BEEF) // beef table start
            return Invalid;
        if (ReadUInt16LittleEndian(data[(actualLength - 0x200 + 0xB0)..]) != 0x13) // check a block number to double-check
            return Invalid;

        return GG;
    }

    /// <summary>Checks to see if the data belongs to a Gen8 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG8SAV(ReadOnlySpan<byte> data)
    {
        if (!SizesSWSH.Contains(data.Length))
            return Invalid;

        return SwishCrypto.GetIsHashValid(data) ? SWSH : Invalid;
    }

    private static GameVersion GetIsG8SAV_BDSP(ReadOnlySpan<byte> data)
    {
        if (data.Length is not (SIZE_G8BDSP or SIZE_G8BDSP_1 or SIZE_G8BDSP_2 or SIZE_G8BDSP_3))
            return Invalid;

        var version = (Gem8Version)ReadUInt32LittleEndian(data);
        if (version is not (Gem8Version.V1_0 or Gem8Version.V1_1 or Gem8Version.V1_2 or Gem8Version.V1_3))
            return Invalid;

        return BDSP;
    }

    private static GameVersion GetIsG8SAV_LA(ReadOnlySpan<byte> data)
    {
        if (data.Length is not (SIZE_G8LA or SIZE_G8LA_1))
            return Invalid;

        return SwishCrypto.GetIsHashValid(data) ? PLA : Invalid;
    }

    /// <summary>Checks to see if the data belongs to a Gen8 save</summary>
    /// <param name="data">Save data of which to determine the type</param>
    /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
    private static GameVersion GetIsG9SAV(ReadOnlySpan<byte> data)
    {
        if (!SizesSV.Contains(data.Length))
            return Invalid;

        return SwishCrypto.GetIsHashValid(data) ? SV : Invalid;
    }

    private static bool GetIsBank7(ReadOnlySpan<byte> data) => data.Length == SIZE_G7BANK && data[0] != 0;
    private static bool GetIsBank4(ReadOnlySpan<byte> data) => data.Length == SIZE_G4BANK && ReadUInt32LittleEndian(data[0x3FC00..]) != 0; // box name present
    private static bool GetIsBank3(ReadOnlySpan<byte> data) => data.Length == SIZE_G4BANK && ReadUInt32LittleEndian(data[0x3FC00..]) == 0; // size collision with ^
    private static bool GetIsRanchDP(ReadOnlySpan<byte> data) => data.Length == SIZE_G4RANCH && ReadUInt32BigEndian(data[0x22AC..]) != 0;
    private static bool GetIsRanchPlat(ReadOnlySpan<byte> data) => data.Length == SIZE_G4RANCH_PLAT && ReadUInt32BigEndian(data[0x268C..]) != 0;
    private static bool GetIsRanch4(ReadOnlySpan<byte> data) => GetIsRanchDP(data) || GetIsRanchPlat(data);

    /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
    /// <param name="path">File location from which to create a SaveFile.</param>
    /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
    public static SaveFile? GetVariantSAV(string path)
    {
        // Many things can go wrong with loading save data (file no longer present toc-tou, or bad save layout).
        try
        {
            var data = File.ReadAllBytes(path);
            var sav = GetVariantSAV(data, path);
            if (sav is null)
                return null;

            sav.Metadata.SetExtraInfo(path);
            if (sav.Generation <= 3)
                SaveLanguage.TryRevise(sav);
            return sav;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return null;
        }
    }

    /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
    /// <param name="data">Save data from which to create a SaveFile.</param>
    /// <param name="path">Optional save file path, may help initialize a non-standard save file format.</param>
    /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
    public static SaveFile? GetVariantSAV(byte[] data, string? path = null)
    {
#if !EXCLUDE_HACKS
        foreach (var h in CustomSaveReaders)
        {
            if (!h.IsRecognized(data.Length))
                continue;

            var custom = h.ReadSaveFile(data, path);
            if (custom != null)
                return custom;
        }
#endif

        var sav = GetVariantSAVInternal(data);
        if (sav != null)
            return sav;

#if !EXCLUDE_EMULATOR_FORMATS
        foreach (var h in Handlers)
        {
            if (!h.IsRecognized(data.Length))
                continue;

            var split = h.TrySplit(data);
            if (split == null)
                continue;

            sav = GetVariantSAVInternal(split.Data);
            if (sav == null)
                continue;

            var meta = sav.Metadata;
            meta.SetExtraInfo(split.Header, split.Footer, split.Handler);
            if (path is not null)
                meta.SetExtraInfo(path);
            return sav;
        }
#endif

        // unrecognized.
        return null;
    }

    private static SaveFile? GetVariantSAVInternal(byte[] data)
    {
        var type = GetSAVType(data);
        return type switch
        {
            // Main Games
            RBY => new SAV1(data, type),
            GS or C => new SAV2(data, type),

            RS => new SAV3RS(data),
            E => new SAV3E(data),
            FRLG => new SAV3FRLG(data),

            DP => new SAV4DP(data),
            Pt => new SAV4Pt(data),
            HGSS => new SAV4HGSS(data),

            BW => new SAV5BW(data),
            B2W2 => new SAV5B2W2(data),

            XY => new SAV6XY(data),
            ORAS => new SAV6AO(data),
            ORASDEMO => new SAV6AODemo(data),

            SM => new SAV7SM(data),
            USUM => new SAV7USUM(data),
            GG => new SAV7b(data),

            SWSH => new SAV8SWSH(data),
            BDSP => new SAV8BS(data),
            PLA => new SAV8LA(data),

            SV => new SAV9SV(data),

            // Side Games
            COLO => new SAV3Colosseum(data),
            XD => new SAV3XD(data),
            RSBOX => new SAV3RSBox(data),
            BATREV => new SAV4BR(data),
            Stadium2 => new SAV2Stadium(data),
            Stadium => new SAV1Stadium(data),
            StadiumJ => new SAV1StadiumJ(data),

            // Bulk Storage
            Gen3 => new Bank3(data),
            DPPt => new SAV4Ranch(data),
            Gen4 => new Bank4(data),
            Gen7 => Bank7.GetBank7(data),

            // No pattern matched
            _ => null,
        };
    }

    public static SaveFile? GetVariantSAV(SAV3GCMemoryCard memCard)
    {
        // Pre-check for header/footer signatures
        if (memCard.IsNoGameSelected)
            memCard.GetMemoryCardState();
        var memory = memCard.ReadSaveGameData();
        if (memory.Length == 0)
            return null;

        var split = DolphinHandler.TrySplit(memory.Span);
        var data = split != null ? split.Data : memory.ToArray();

        SaveFile sav;
        switch (memCard.SelectedGameVersion)
        {
            // Side Games
            case COLO: sav = new SAV3Colosseum(data) { MemoryCard = memCard }; break;
            case XD: sav = new SAV3XD(data) { MemoryCard = memCard }; break;
            case RSBOX: sav = new SAV3RSBox(data, memCard) { MemoryCard = memCard }; break;

            // No pattern matched
            default: return null;
        }

        if (split != null)
            sav.Metadata.SetExtraInfo(split.Header, split.Footer, split.Handler);
        return sav;
    }

    /// <summary>
    /// Returns a <see cref="LanguageID"/> that feels best for the save file's language.
    /// </summary>
    public static LanguageID GetSafeLanguage(SaveFile? sav) => sav switch
    {
        null => LanguageID.English,
        ILangDeviantSave s => s.Japanese ? LanguageID.Japanese : s.Korean ? LanguageID.Korean : LanguageID.English,
        _ => (uint)sav.Language <= Legal.GetMaxLanguageID(sav.Generation) ? (LanguageID)sav.Language : LanguageID.English,
    };

    /// <summary>
    /// Returns a Trainer Name that feels best for the save file's language.
    /// </summary>
    public static string GetSafeTrainerName(SaveFile? sav, LanguageID lang) => lang switch
    {
        LanguageID.Japanese => sav?.Generation >= 3 ? "ＰＫＨｅＸ" : "1337",
        _ => "PKHeX",
    };

    /// <summary>
    /// Creates an instance of a SaveFile with a blank base.
    /// </summary>
    /// <param name="game">Version to create the save file for.</param>
    /// <param name="trainerName">Trainer Name</param>
    /// <param name="language">Language to initialize with</param>
    /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
    public static SaveFile GetBlankSAV(GameVersion game, string trainerName, LanguageID language = LanguageID.English)
    {
        var sav = GetBlankSAV(game, language);
        sav.Version = game;
        sav.OT = trainerName;
        if (sav.Generation >= 4)
            sav.Language = (int)language;

        // Secondary Properties may not be used but can be filled in as template.
        (uint tid, uint sid) = sav.Generation >= 7 ? (123456u, 1234u) : (12345u, 54321u);
        sav.SetDisplayID(tid, sid);
        sav.Language = (int)language;

        // Only set geolocation data for 3DS titles
        if (sav is IRegionOrigin o)
            o.SetDefaultRegionOrigins((int)language);

        return sav;
    }

    /// <summary>
    /// Creates an instance of a SaveFile with a blank base.
    /// </summary>
    /// <param name="game">Version to create the save file for.</param>
    /// <param name="language">Save file language to initialize for</param>
    /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
    private static SaveFile GetBlankSAV(GameVersion game, LanguageID language) => game switch
    {
        RD or BU or GN or YW or RBY => new SAV1(version: game, game == BU ? LanguageID.Japanese : language),
        StadiumJ => new SAV1StadiumJ(),
        Stadium => new SAV1Stadium(language == LanguageID.Japanese),

        GD or SI or GS => new SAV2(version: GS, language: language),
        C or GSC => new SAV2(version: C, language: language),
        Stadium2 => new SAV2Stadium(language == LanguageID.Japanese),

        R or S or RS => new SAV3RS(language == LanguageID.Japanese),
        E or RSE => new SAV3E(language == LanguageID.Japanese),
        FR or LG or FRLG => new SAV3FRLG(language == LanguageID.Japanese),

        CXD or COLO => new SAV3Colosseum(),
        XD => new SAV3XD(),
        RSBOX => new SAV3RSBox(),

        D or P or DP => new SAV4DP(),
        Pt or DPPt => new SAV4Pt(),
        HG or SS or HGSS => new SAV4HGSS(),
        BATREV => new SAV4BR(),

        B or W or BW => new SAV5BW(),
        B2 or W2 or B2W2 => new SAV5B2W2(),

        X or Y or XY => new SAV6XY(),
        ORASDEMO => new SAV6AODemo(),
        OR or AS or ORAS => new SAV6AO(),

        SN or MN or SM => new SAV7SM(),
        US or UM or USUM => new SAV7USUM(),
        GP or GE or GG or GO => new SAV7b(),

        SW or SH or SWSH => new SAV8SWSH(),
        BD or SP or BDSP => new SAV8BS(),
        PLA => new SAV8LA(),

        SL or VL or SV => new SAV9SV(),

        _ => throw new ArgumentOutOfRangeException(nameof(game)),
    };

    /// <summary>
    /// Creates an instance of a SaveFile with a blank base.
    /// </summary>
    /// <param name="context">Context of the Save File.</param>
    /// <param name="trainerName">Trainer Name</param>
    /// <param name="language">Save file language to initialize for</param>
    /// <returns>Save File for that generation.</returns>
    public static SaveFile GetBlankSAV(EntityContext context, string trainerName, LanguageID language = LanguageID.English)
    {
        var version = context.GetSingleGameVersion();
        return GetBlankSAV(version, trainerName, language);
    }

    /// <summary>
    /// Retrieves possible save file paths from the provided <see cref="folderPath"/>.
    /// </summary>
    /// <param name="folderPath">Folder to look within</param>
    /// <param name="deep">Search all subfolders</param>
    /// <param name="result">If this function returns true, full path of all <see cref="SaveFile"/> that match criteria. If this function returns false, the error message, or null if the directory could not be found</param>
    /// <param name="ignoreBackups">Option to ignore files with backup names and extensions</param>
    /// <returns>Boolean indicating if the operation was successful.</returns>
    public static bool GetSavesFromFolder(string folderPath, bool deep, out IEnumerable<string> result, bool ignoreBackups = true)
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
            result = FilterSaveFiles(ignoreBackups, files);
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

    private static IEnumerable<string> FilterSaveFiles(bool ignoreBackups, IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            if (ignoreBackups && IsBackup(file))
                continue;

            var size = FileUtil.GetFileSize(file);
            if (!IsSizeValid(size))
                continue;

            yield return file;
        }
    }

    public static bool IsBackup(ReadOnlySpan<char> path)
    {
        var fn = Path.GetFileNameWithoutExtension(path);
        if (fn is "backup")
            return true;

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
    public static bool IsSizeValidHandler(long size) => Handlers.Any(z => z.IsRecognized(size));

    /// <summary>
    /// Determines whether the save data size is valid for automatically detecting saves.
    /// </summary>
    /// <remarks>Does not check the <see cref="Handlers"/> list.</remarks>
    public static bool IsSizeValidNoHandler(long size) => Sizes.Contains(size);
}
