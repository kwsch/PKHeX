using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class PSS6
    {
        private const string Header = "PSS List";
        private static readonly string[] headers = { "PSS Data - Friends", "PSS Data - Acquaintances", "PSS Data - Passerby" };

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
                // uint count = BitConverter.ToUInt32(savefile, offset + 0x4E20);
                ReadTrainers(result, data, offset, 100);
                offset += 0x5000; // Advance to next block
            }

            return result;
        }

        private static void ReadTrainers(ICollection<string> result, byte[] data, int offset, int count)
        {
            int r_offset = offset;
            for (int i = 0; i < count; i++)
            {
                if (!ReadTrainer(result, data, r_offset))
                    break; // No data present here

                if (i > 0)
                    result.Add(string.Empty);

                r_offset += 0xC8; // Advance to next entry
            }
        }

        private static bool ReadTrainer(ICollection<string> result, byte[] Data, int ofs)
        {
            ulong pssID = BitConverter.ToUInt64(Data, ofs);
            if (pssID == 0)
                return false; // no data

            string otname = StringConverter.GetString6(Data, ofs + 8, 0x1A);
            string message = StringConverter.GetString6(Data, ofs + 8 + 0x1A, 0x22);

            // Trim terminated

            // uint unk1 = BitConverter.ToUInt32(savefile, r_offset + 0x44);
            // ulong unk2 = BitConverter.ToUInt64(savefile, r_offset + 0x48);
            // uint unk3 = BitConverter.ToUInt32(savefile, r_offset + 0x50);
            // uint unk4 = BitConverter.ToUInt16(savefile, r_offset + 0x54);
            byte regionID = Data[ofs + 0x56];
            byte countryID = Data[ofs + 0x57];
            byte game = Data[ofs + 0x5A];
            // ulong outfit = BitConverter.ToUInt64(savefile, r_offset + 0x5C);
            int favpkm = BitConverter.ToUInt16(Data, ofs + 0x9C) & 0x7FF;

            string gamename = GetGameName(game);
            var (country, region) = GeoLocation.GetCountryRegionText(countryID, regionID, GameInfo.CurrentLanguage);
            result.Add($"OT: {otname}");
            result.Add($"Message: {message}");
            result.Add($"Game: {gamename}");
            result.Add($"Country: {country}");
            result.Add($"Region: {region}");
            result.Add($"Favorite: {GameInfo.Strings.specieslist[favpkm]}");
            return false;
        }

        private static string GetGameName(int game)
        {
            const string unk = "UNKNOWN GAME";
            if (game < 0)
                return unk;
            var list = GameInfo.Strings.gamelist;
            if (game >= list.Length)
                return unk;
            return list[game];
        }
    }
}
