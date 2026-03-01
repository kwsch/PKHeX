using System;
using static PKHeX.Core.Distribution3JPN;

namespace PKHeX.Core;

public enum Distribution3JPN : byte
{
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
}

public static class Gen3PCJP
{
    private const string Tokyo = "トウキョー";
    private const string Yokohama = "ヨコハマ";
    private const string Nagoya = "ナゴヤ";
    private const string Osaka = "オーサカ";
    private const string Fukuoka = "フクオカ";
    private const string Sapporo = "サッポロ";

    private static ReadOnlySpan<ushort> Distro1 => [252, 255, 258];
    private static ReadOnlySpan<ushort> Distro2 => [152, 155, 158];
    private static ReadOnlySpan<ushort> Distro3 => [023, 025, 052, 058, 069, 079, 090, 113, 115, 123, 125, 126, 128, 198, 200, 211, 215, 225, 226];
    private static ReadOnlySpan<ushort> Distro4 => [1, 4, 7];
    private static ReadOnlySpan<ushort> Distro5 => [025, 270, 273, 283, 300, 302, 303, 307, 311, 312, 315, 335, 336, 337, 338, 358];
    private static ReadOnlySpan<ushort> Distro6 => [025, 163, 179, 190, 191, 202, 204, 207, 209, 213, 216, 228, 234, 235];

    public static EncounterGift3JPN[] GetArray()
    {
        var size = Distro1.Length + Distro2.Length + Distro3.Length + Distro4.Length + Distro5.Length + Distro6.Length;
        var arr = new EncounterGift3JPN[size];
        var i = 0;

        AddDistro(arr, ref i, Distro1, First);
        AddDistro(arr, ref i, Distro2, Second);
        AddDistro(arr, ref i, Distro3, Third);
        AddDistro(arr, ref i, Distro4, Fourth);
        AddDistro(arr, ref i, Distro5, Fifth);
        AddDistro(arr, ref i, Distro6, Sixth);
        return arr;

        static void AddDistro(EncounterGift3JPN[] arr, ref int i, ReadOnlySpan<ushort> Distro, Distribution3JPN dist)
        {
            foreach (var species in Distro)
                arr[i++] = new EncounterGift3JPN(species, dist);
        }
    }

    extension(Distribution3JPN dist)
    {
        public ushort GetTrainerID() => dist switch
        {
            First  => 51126,
            Second => 51224,
            Third  => 60114,
            Fourth => 60227,
            Fifth  => 60321,
            Sixth  => 60505,
            _ => throw new ArgumentOutOfRangeException(nameof(dist), dist, null),
        };

        public bool IsTrainerNameValid(ReadOnlySpan<char> name) => dist switch
        {
            Sixth => name is Tokyo or Yokohama or Nagoya or Osaka or Fukuoka,
            _ => name is Tokyo or Yokohama or Nagoya or Osaka or Fukuoka or Sapporo,
        };

        public string GetTrainerName(ushort rand)
        {
            var choice = dist == Sixth ? 5 : 6; // Ignore Sapporo for Sixth
            return GetTrainerNameIndex(rand % choice);
        }
    }

    private static string GetTrainerNameIndex(int index) => index switch
    {
        0 => Tokyo,
        1 => Yokohama,
        2 => Nagoya,
        3 => Osaka,
        4 => Fukuoka,
        5 => Sapporo,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
    };
}
