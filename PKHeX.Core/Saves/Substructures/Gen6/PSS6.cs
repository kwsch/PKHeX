using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public static class PSS6
{
    private const string Header = "PSS List";
    private static readonly string[] headers = ["PSS Data - Friends", "PSS Data - Acquaintances", "PSS Data - Passerby"];

    public static List<string> GetPSSParse(SAV6 SAV)
    {
        var result = new List<string> {Header};
        int offset = SAV.PSS;
        var data = SAV.Data;
        for (int g = 0; g < 3; g++)
        {
            result.Add("----");
            result.Add(headers[g]);
            result.Add("----");
            // uint count = ReadUInt32LittleEndian(Data.Slice(0x4E20));
            ReadTrainers(result, data, offset, 100);
            offset += 0x5000; // Advance to next block
        }

        return result;
    }

    private static void ReadTrainers(List<string> result, Span<byte> data, int offset, int count)
    {
        int r_offset = offset;
        const int size = 0xC8;
        for (int i = 0; i < count; i++)
        {
            if (!ReadTrainer(result, data.Slice(r_offset, size)))
                break; // No data present here

            if (i > 0)
                result.Add(string.Empty);

            r_offset += size; // Advance to next entry
        }
    }

    private static bool ReadTrainer(List<string> result, ReadOnlySpan<byte> data)
    {
        ulong pssID = ReadUInt64LittleEndian(data);
        if (pssID == 0)
            return false; // no data

        string otname = StringConverter6.GetString(data.Slice(0x08, 0x1A));
        string message = StringConverter6.GetString(data.Slice(0x22, 0x22));

        // Trim terminated

        // uint unk1  = ReadUInt32LittleEndian(data[0x44..]);
        // ulong unk2 = ReadUInt64LittleEndian(data[0x48..]);
        // uint unk3  = ReadUInt32LittleEndian(data[0x50..]);
        // uint unk4  = ReadUInt16LittleEndian(data[0x54..]);
        byte regionID = data[0x56];
        byte countryID = data[0x57];
        // byte birthMonth = data[0x58];
        // byte birthDay = data[0x59];
        var game = (GameVersion)data[0x5A];
        // ulong outfit = ReadUInt64LittleEndian(data.AsSpan(ofs + 0x5C));
        int favpkm = ReadUInt16LittleEndian(data[0x9C..]) & 0x7FF;

        var gamename = GetGameName(game);
        var (country, region) = GeoLocation.GetCountryRegionText(countryID, regionID, GameInfo.CurrentLanguage);
        result.Add($"OT: {otname}");
        result.Add($"Message: {message}");
        result.Add($"Game: {gamename}");
        result.Add($"Country: {country}");
        result.Add($"Region: {region}");
        result.Add($"Favorite: {GameInfo.Strings.specieslist[favpkm]}");
        return false;
    }

    private static string GetGameName(GameVersion game)
    {
        const string unk = "UNKNOWN GAME";
        if (!GameVersion.Gen6.Contains(game))
            return unk;
        var list = GameInfo.Strings.gamelist;
        return list[(byte)game];
    }
}
