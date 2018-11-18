using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Go Park Entity transferred to <see cref="GameVersion.GG"/>.
    /// </summary>
    public class GP1
    {
        public const int SIZE = 0x1B0;
        public byte[] Data { get; }
        public GP1() => Data = (byte[])Blank.Clone();
        public void WriteTo(byte[] data, int offset) => Data.CopyTo(data, offset);

        public static GP1 FromData(byte[] data, int offset)
        {
            var gpkm = new GP1();
            Array.Copy(data, offset, gpkm.Data, 0, SIZE);
            return gpkm;
        }

        private static readonly byte[] Blank20 =
        {
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F,
            0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x85, 0xEC, 0x33, 0x01,
        };

        public static readonly byte[] Blank = GetBlank();

        public static byte[] GetBlank()
        {
            byte[] data = new byte[SIZE];
            Blank20.CopyTo(data, 0x20);
            return data;
        }

        public string Username1 => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x00, 0x10));
        public string Username2 => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x10, 0x20));

        public int Species => BitConverter.ToInt32(Data, 0x28);
        public float LevelF => BitConverter.ToSingle(Data, 0x30);
        public int Level => Math.Max(1, (int)Math.Round(LevelF));

        public int IV1 => BitConverter.ToInt32(Data, 0x50);
        public int IV2 => BitConverter.ToInt32(Data, 0x54);
        public int IV3 => BitConverter.ToInt32(Data, 0x58);

        public int Gender => Data[0x70] - 1; // M=1, F=2, G=3 ;; shift down by 1.

        public int AltForm => Data[0x72];
        public bool IsShiny => Data[0x73] == 1;

        // https://bulbapedia.bulbagarden.net/wiki/List_of_moves_in_Pok%C3%A9mon_GO
        public int Move1 => BitConverter.ToInt32(Data, 0x74); // uses Go Indexes
        public int Move2 => BitConverter.ToInt32(Data, 0x78); // uses Go Indexes

        public string GeoCityName => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x7C, 0x60)); // dunno length

        public string Nickname => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x12D, 0x20)); // dunno length

        public static readonly string[] Genders = {"M", "F", "-"};
        public string GenderString => (uint) Gender >= Genders.Length ? string.Empty : Genders[Gender];
        public string ShinyString => IsShiny ? "★ " : string.Empty;
        public string FileName => $"{ShinyString}{Nickname} lv{Level} - {IV1:00}.{IV2:00}.{IV3:00}, Move1 {Move1}, Move2 {Move2}.gp1";
        public string FormString => AltForm != 0 ? $"-{AltForm}" : string.Empty;


        public string Dump(IReadOnlyList<string> speciesNames, int index) => $"{index:000} {Nickname} ({speciesNames[Species]}{FormString} {ShinyString}[{GenderString}]) @ lv{Level} - {IV1:00}/{IV2:00}/{IV3:00}, Move1 {Move1}, Move2 {Move2}, Captured in {GeoCityName} by {Username1}.";
    }
}