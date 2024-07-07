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

    public static ushort GetTrainerID(this Distribution3JPN dist) => dist switch
    {
        First  => 51126,
        Second => 51224,
        Third  => 60114,
        Fourth => 60227,
        Fifth  => 60321,
        Sixth  => 60505,
        _ => throw new ArgumentOutOfRangeException(nameof(dist), dist, null),
    };

    public static bool IsTrainerNameValid(this Distribution3JPN dist, ReadOnlySpan<char> name) => dist switch
    {
        Sixth => name is Tokyo or Yokohama or Nagoya or Osaka or Fukuoka,
        _ => name is Tokyo or Yokohama or Nagoya or Osaka or Fukuoka or Sapporo,
    };

    public static string GetTrainerName(this Distribution3JPN dist, ushort rand)
    {
        var choice = dist == Sixth ? 5 : 6; // Ignore Sapporo for Sixth
        return GetTrainerNameIndex(rand % choice);
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
