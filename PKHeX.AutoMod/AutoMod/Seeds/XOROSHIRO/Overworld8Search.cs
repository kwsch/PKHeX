using System;
using System.Collections.Generic;

namespace PKHeX.Core.AutoMod;

public static class Overworld8Search
{
    private static readonly Dictionary<int, uint> ZeroFlawless = new()
    {
        { ComputeIV32([31, 31, 31, 31, 00, 00]), 0x005DC65E },
        { ComputeIV32([31, 00, 31, 00, 31, 31]), 0x022F7135 },
        { ComputeIV32([31, 31, 31, 31, 00, 31]), 0x025FF1EE },
        { ComputeIV32([31, 31, 31, 00, 31, 00]), 0x02EDF501 },
        { ComputeIV32([31, 00, 00, 31, 00, 31]), 0x03FAFB14 },
        { ComputeIV32([31, 31, 00, 31, 00, 31]), 0x04DD6D04 },
        { ComputeIV32([00, 31, 31, 31, 00, 00]), 0x06A148C5 },
        { ComputeIV32([00, 31, 00, 00, 31, 00]), 0x10620EFD },
        { ComputeIV32([00, 31, 00, 00, 00, 31]), 0x11E8259D },
        { ComputeIV32([00, 00, 31, 31, 00, 00]), 0x12E909F7 },
        { ComputeIV32([00, 31, 31, 00, 31, 00]), 0x12E92CD7 },
        { ComputeIV32([00, 00, 31, 00, 00, 31]), 0x13839BCB },
        { ComputeIV32([31, 00, 00, 00, 31, 00]), 0x13D576E4 },
        { ComputeIV32([00, 31, 31, 00, 31, 31]), 0x14B37E3E },
        { ComputeIV32([00, 31, 31, 00, 00, 31]), 0x14C0A213 },
        { ComputeIV32([31, 00, 31, 31, 00, 31]), 0x153A707E },
        { ComputeIV32([31, 00, 00, 00, 00, 00]), 0x1645B6A1 },
        { ComputeIV32([00, 00, 00, 00, 00, 00]), 0x17033091 },
        { ComputeIV32([00, 00, 00, 00, 31, 00]), 0x17CE97AD },
        { ComputeIV32([00, 31, 00, 31, 00, 31]), 0x17DC94D2 },
        { ComputeIV32([00, 00, 31, 00, 31, 31]), 0x17E99D74 },
        { ComputeIV32([00, 00, 00, 31, 00, 00]), 0x19EF2793 },
        { ComputeIV32([31, 00, 00, 31, 31, 31]), 0x1E220AD7 },
        { ComputeIV32([00, 00, 31, 00, 00, 00]), 0x1EB65910 },
        { ComputeIV32([00, 31, 31, 31, 31, 00]), 0x1EEE3969 },
        { ComputeIV32([31, 00, 31, 31, 31, 00]), 0x200DA840 },
        { ComputeIV32([31, 00, 31, 00, 00, 00]), 0x255E3CBB },
        { ComputeIV32([31, 31, 00, 00, 31, 31]), 0x25CF4634 },
        { ComputeIV32([31, 31, 00, 00, 00, 31]), 0x2751DAA8 },
        { ComputeIV32([00, 00, 31, 31, 00, 31]), 0x289BB6E9 },
        { ComputeIV32([31, 00, 31, 00, 31, 00]), 0x2C0ABDFE },
        { ComputeIV32([00, 00, 31, 31, 31, 31]), 0x2F5E32DD },
        { ComputeIV32([00, 00, 00, 00, 31, 31]), 0x3353493F },
        { ComputeIV32([00, 31, 00, 31, 31, 00]), 0x343BFB03 },
        { ComputeIV32([00, 00, 00, 31, 31, 00]), 0x34470002 },
        { ComputeIV32([31, 31, 31, 00, 00, 31]), 0x367029A7 },
        { ComputeIV32([31, 00, 00, 00, 00, 31]), 0x3A804E4A },
        { ComputeIV32([00, 00, 31, 00, 31, 00]), 0x3C3EC3D3 },
        { ComputeIV32([31, 31, 00, 00, 00, 00]), 0x3E01AA74 },
        { ComputeIV32([31, 00, 00, 00, 31, 31]), 0x3F5D423D },
        { ComputeIV32([00, 31, 00, 31, 00, 00]), 0x4416AB5D },
        { ComputeIV32([31, 00, 31, 31, 31, 31]), 0x44E87F35 },
        { ComputeIV32([00, 31, 00, 00, 31, 31]), 0x47491E8D },
        { ComputeIV32([31, 31, 00, 31, 31, 00]), 0x474F427F },
        { ComputeIV32([00, 00, 00, 31, 00, 31]), 0x4C7EEEC6 },
        { ComputeIV32([00, 00, 00, 31, 31, 31]), 0x5249D5CB },
        { ComputeIV32([00, 31, 00, 00, 00, 00]), 0x5C87F2CD },
        { ComputeIV32([31, 00, 31, 00, 00, 31]), 0x5FB0018E },
        { ComputeIV32([31, 00, 00, 31, 00, 00]), 0x6AAF1D30 },
        { ComputeIV32([31, 31, 00, 00, 31, 00]), 0x72E45644 },
        { ComputeIV32([31, 00, 00, 31, 31, 00]), 0x73F7C8F3 },
        { ComputeIV32([31, 31, 31, 00, 31, 31]), 0x76352687 },
        { ComputeIV32([00, 31, 31, 00, 00, 00]), 0x84E8D2B1 },
        { ComputeIV32([31, 31, 31, 31, 31, 00]), 0x857FA070 },
        { ComputeIV32([31, 00, 31, 31, 00, 00]), 0x8E15BF56 },
        { ComputeIV32([00, 00, 31, 31, 31, 00]), 0xA64E68D0 },
        { ComputeIV32([00, 00, 00, 00, 00, 31]), 0xB49628C0 },
        { ComputeIV32([00, 31, 00, 31, 31, 31]), 0xD85E0341 },
        { ComputeIV32([31, 31, 31, 00, 00, 00]), 0xE66E8A08 },
    };

    private static readonly Dictionary<int, uint> OneFlawless = new()
    {
        { ComputeIV32([00, 00, 31, 31, 00, 00]), 0x0002129F },
        { ComputeIV32([31, 31, 00, 00, 31, 31]), 0x0006C34D },
        { ComputeIV32([00, 31, 00, 00, 31, 31]), 0x0007A8C9 },
        { ComputeIV32([00, 00, 31, 31, 00, 31]), 0x000E0890 },
        { ComputeIV32([31, 31, 00, 00, 00, 00]), 0x00126D28 },
        { ComputeIV32([00, 00, 00, 00, 31, 31]), 0x00238A05 },
        { ComputeIV32([31, 31, 31, 31, 00, 00]), 0x0023A752 },
        { ComputeIV32([00, 00, 00, 00, 00, 31]), 0x00291559 },
        { ComputeIV32([31, 00, 31, 31, 31, 31]), 0x005BCE03 },
        { ComputeIV32([00, 31, 00, 00, 00, 31]), 0x00650B0F },
        { ComputeIV32([31, 00, 00, 00, 00, 31]), 0x0074868B },
        { ComputeIV32([00, 00, 31, 31, 31, 00]), 0x0090A638 },
        { ComputeIV32([00, 00, 31, 00, 31, 31]), 0x00A55015 },
        { ComputeIV32([31, 31, 00, 00, 00, 31]), 0x00C908B7 },
        { ComputeIV32([31, 31, 00, 31, 31, 31]), 0x00D60A65 },
        { ComputeIV32([31, 00, 31, 31, 00, 31]), 0x00E58195 },
        { ComputeIV32([31, 00, 00, 31, 31, 00]), 0x00FDA1C5 },
        { ComputeIV32([00, 31, 31, 00, 31, 00]), 0x01050034 },
        { ComputeIV32([31, 00, 31, 00, 00, 31]), 0x01060136 },
        { ComputeIV32([31, 00, 00, 00, 31, 31]), 0x0122E6BB },
        { ComputeIV32([00, 00, 00, 00, 31, 00]), 0x015FADB9 },
        { ComputeIV32([00, 00, 00, 31, 31, 00]), 0x016D6004 },
        { ComputeIV32([31, 00, 31, 00, 00, 00]), 0x018F6491 },
        { ComputeIV32([31, 00, 00, 00, 31, 00]), 0x0194D69D },
        { ComputeIV32([31, 00, 31, 31, 31, 00]), 0x01A92A80 },
        { ComputeIV32([31, 00, 31, 31, 00, 00]), 0x01AF64E2 },
        { ComputeIV32([31, 00, 00, 31, 31, 31]), 0x01AF7672 },
        { ComputeIV32([00, 31, 00, 31, 00, 31]), 0x01B775E6 },
        { ComputeIV32([00, 31, 00, 00, 00, 00]), 0x01C3FE0F },
        { ComputeIV32([31, 31, 00, 31, 31, 00]), 0x01C49C3B },
        { ComputeIV32([31, 00, 00, 31, 00, 31]), 0x01DCAA5F },
        { ComputeIV32([00, 31, 31, 31, 31, 31]), 0x01E534D6 },
        { ComputeIV32([31, 31, 31, 31, 00, 31]), 0x0211DB78 },
        { ComputeIV32([31, 31, 31, 31, 31, 00]), 0x0220C33A },
        { ComputeIV32([31, 31, 31, 00, 31, 31]), 0x0235487F },
        { ComputeIV32([31, 31, 00, 31, 00, 31]), 0x0247C78A },
        { ComputeIV32([31, 31, 00, 00, 31, 00]), 0x02859531 },
        { ComputeIV32([00, 31, 00, 31, 31, 31]), 0x028BBA5A },
        { ComputeIV32([00, 00, 00, 31, 31, 31]), 0x029B17D5 },
        { ComputeIV32([00, 31, 31, 31, 31, 00]), 0x02BA712A },
        { ComputeIV32([00, 31, 00, 31, 00, 00]), 0x02EAF126 },
        { ComputeIV32([00, 00, 00, 31, 00, 31]), 0x033A5BDE },
        { ComputeIV32([00, 31, 31, 31, 00, 31]), 0x03514D6C },
        { ComputeIV32([00, 31, 31, 00, 00, 31]), 0x0373C3E3 },
        { ComputeIV32([00, 31, 31, 00, 31, 31]), 0x0375AAE1 },
        { ComputeIV32([31, 00, 31, 00, 31, 00]), 0x03AA4EE8 },
        { ComputeIV32([31, 31, 00, 31, 00, 00]), 0x03C7F853 },
        { ComputeIV32([00, 31, 31, 31, 00, 00]), 0x04104680 },
        { ComputeIV32([00, 00, 31, 00, 31, 00]), 0x04112DC7 },
        { ComputeIV32([00, 00, 31, 31, 31, 31]), 0x041B98AD },
        { ComputeIV32([31, 31, 31, 00, 00, 31]), 0x04486D1B },
        { ComputeIV32([00, 31, 00, 31, 31, 00]), 0x04CFF417 },
        { ComputeIV32([31, 31, 31, 31, 31, 31]), 0x052D6808 },
        { ComputeIV32([31, 31, 31, 00, 31, 00]), 0x053B05B5 },
        { ComputeIV32([00, 00, 31, 00, 00, 31]), 0x05775C6E },
        { ComputeIV32([31, 00, 00, 31, 00, 00]), 0x07D6BC01 },
        { ComputeIV32([31, 00, 31, 00, 31, 31]), 0x08EE45AE },
        { ComputeIV32([00, 31, 31, 00, 00, 00]), 0x094EA84E },
        { ComputeIV32([31, 31, 31, 00, 00, 00]), 0x0C0296B9 },
        { ComputeIV32([00, 31, 00, 00, 31, 00]), 0x0D56ACBA },
        { ComputeIV32([31, 00, 00, 00, 00, 00]), 0x17033091 },
        { ComputeIV32([00, 00, 31, 00, 00, 00]), 0x1CEEE0C8 },
        { ComputeIV32([00, 00, 00, 31, 00, 00]), 0x3582A115 },
    };

    private static readonly Dictionary<int, uint> TwoFlawless = new()
    {
        { ComputeIV32([31, 00, 31, 31, 31, 31]), 0x00009BEC },
        { ComputeIV32([00, 31, 31, 00, 31, 31]), 0x00010626 },
        { ComputeIV32([00, 31, 00, 00, 00, 31]), 0x0001D41E },
        { ComputeIV32([31, 31, 31, 31, 00, 31]), 0x000209B0 },
        { ComputeIV32([31, 00, 31, 31, 00, 00]), 0x0002129F },
        { ComputeIV32([00, 00, 31, 31, 31, 00]), 0x00022A74 },
        { ComputeIV32([00, 00, 31, 00, 31, 31]), 0x000386C4 },
        { ComputeIV32([31, 00, 31, 31, 00, 31]), 0x00038F55 },
        { ComputeIV32([31, 31, 00, 31, 00, 31]), 0x0004B200 },
        { ComputeIV32([31, 31, 00, 00, 31, 31]), 0x0007A8C9 },
        { ComputeIV32([31, 31, 31, 00, 31, 31]), 0x00085349 },
        { ComputeIV32([31, 31, 31, 31, 31, 31]), 0x000A1428 },
        { ComputeIV32([31, 31, 00, 31, 31, 31]), 0x000C0774 },
        { ComputeIV32([00, 31, 00, 31, 31, 31]), 0x000CEE68 },
        { ComputeIV32([31, 31, 00, 31, 31, 00]), 0x000E1494 },
        { ComputeIV32([31, 31, 31, 00, 00, 31]), 0x000E671B },
        { ComputeIV32([31, 00, 00, 31, 31, 00]), 0x000FFAD1 },
        { ComputeIV32([31, 00, 31, 00, 31, 00]), 0x00111562 },
        { ComputeIV32([00, 31, 31, 31, 31, 31]), 0x00118B68 },
        { ComputeIV32([00, 31, 31, 31, 00, 31]), 0x0011A238 },
        { ComputeIV32([31, 31, 00, 00, 00, 31]), 0x0012224C },
        { ComputeIV32([31, 31, 00, 31, 00, 00]), 0x00151F19 },
        { ComputeIV32([31, 00, 00, 31, 31, 31]), 0x001648C6 },
        { ComputeIV32([00, 00, 31, 00, 31, 00]), 0x00170CD7 },
        { ComputeIV32([31, 31, 00, 00, 31, 00]), 0x0019D0BD },
        { ComputeIV32([31, 00, 31, 00, 00, 31]), 0x001B54B4 },
        { ComputeIV32([00, 31, 31, 00, 00, 00]), 0x001B7953 },
        { ComputeIV32([00, 31, 00, 00, 31, 31]), 0x001D4371 },
        { ComputeIV32([31, 31, 31, 31, 31, 00]), 0x001DD7BC },
        { ComputeIV32([00, 31, 31, 31, 31, 00]), 0x002049E7 },
        { ComputeIV32([31, 00, 00, 31, 00, 00]), 0x00216E4D },
        { ComputeIV32([31, 00, 00, 00, 31, 31]), 0x00238A05 },
        { ComputeIV32([00, 00, 31, 31, 31, 31]), 0x0026AAEE },
        { ComputeIV32([31, 00, 00, 00, 00, 31]), 0x00291559 },
        { ComputeIV32([31, 00, 00, 31, 00, 31]), 0x002A3DC1 },
        { ComputeIV32([31, 00, 31, 00, 31, 31]), 0x002AEF1D },
        { ComputeIV32([00, 31, 00, 31, 31, 00]), 0x00346DD7 },
        { ComputeIV32([00, 31, 00, 31, 00, 31]), 0x00388EB6 },
        { ComputeIV32([00, 00, 00, 00, 31, 31]), 0x003F6E2F },
        { ComputeIV32([00, 31, 31, 00, 00, 31]), 0x00413759 },
        { ComputeIV32([31, 31, 31, 31, 00, 00]), 0x004FB16D },
        { ComputeIV32([31, 31, 31, 00, 31, 00]), 0x00571436 },
        { ComputeIV32([31, 00, 31, 31, 31, 00]), 0x005CDD03 },
        { ComputeIV32([00, 00, 00, 31, 31, 31]), 0x005E61D3 },
        { ComputeIV32([00, 31, 00, 00, 31, 00]), 0x00643D42 },
        { ComputeIV32([00, 00, 31, 31, 00, 00]), 0x00661FB6 },
        { ComputeIV32([00, 00, 31, 31, 00, 31]), 0x0071D252 },
        { ComputeIV32([00, 31, 00, 31, 00, 00]), 0x007C7FC5 },
        { ComputeIV32([00, 31, 31, 31, 00, 00]), 0x00932FDD },
        { ComputeIV32([00, 31, 31, 00, 31, 00]), 0x009A1433 },
        { ComputeIV32([31, 00, 31, 00, 00, 00]), 0x00B9D520 },
        { ComputeIV32([00, 00, 31, 00, 00, 31]), 0x012F3F0A },
        { ComputeIV32([31, 31, 31, 00, 00, 00]), 0x014AACA3 },
        { ComputeIV32([31, 00, 00, 00, 31, 00]), 0x015FADB9 },
        { ComputeIV32([31, 31, 00, 00, 00, 00]), 0x01772901 },
        { ComputeIV32([00, 00, 00, 31, 00, 31]), 0x0241D345 },
        { ComputeIV32([00, 00, 00, 31, 31, 00]), 0x0394F8DC },
    };

    private static readonly Dictionary<int, uint> ThreeFlawless = new()
    {
        { ComputeIV32([31, 31, 31, 00, 31, 00]), 0x000033F7 },
        { ComputeIV32([31, 31, 31, 00, 00, 31]), 0x0000443F },
        { ComputeIV32([31, 31, 31, 31, 00, 00]), 0x00004548 },
        { ComputeIV32([31, 31, 00, 00, 31, 31]), 0x00005695 },
        { ComputeIV32([31, 00, 00, 31, 31, 31]), 0x00009783 },
        { ComputeIV32([31, 00, 31, 00, 31, 31]), 0x00009B1B },
        { ComputeIV32([31, 31, 31, 31, 31, 31]), 0x00009C23 },
        { ComputeIV32([31, 31, 31, 00, 31, 31]), 0x0000D0E6 },
        { ComputeIV32([31, 31, 00, 00, 00, 31]), 0x0000E6BA },
        { ComputeIV32([31, 31, 00, 31, 00, 31]), 0x0000F110 },
        { ComputeIV32([00, 00, 00, 31, 31, 31]), 0x0000F2E2 },
        { ComputeIV32([31, 00, 31, 31, 31, 31]), 0x0000F998 },
        { ComputeIV32([00, 00, 31, 31, 31, 00]), 0x00011060 },
        { ComputeIV32([31, 00, 00, 31, 31, 00]), 0x00013060 },
        { ComputeIV32([00, 31, 00, 31, 31, 31]), 0x00015A01 },
        { ComputeIV32([31, 00, 00, 31, 00, 31]), 0x00015D54 },
        { ComputeIV32([00, 31, 31, 31, 31, 31]), 0x00018823 },
        { ComputeIV32([00, 31, 31, 31, 00, 31]), 0x00020D0A },
        { ComputeIV32([31, 00, 31, 31, 00, 31]), 0x0002231D },
        { ComputeIV32([31, 00, 31, 31, 31, 00]), 0x00022A74 },
        { ComputeIV32([00, 00, 31, 31, 31, 31]), 0x00022DD5 },
        { ComputeIV32([00, 00, 31, 31, 00, 31]), 0x0002467F },
        { ComputeIV32([31, 31, 31, 31, 00, 31]), 0x00025536 },
        { ComputeIV32([31, 31, 31, 31, 31, 00]), 0x0002DC93 },
        { ComputeIV32([31, 31, 00, 31, 31, 31]), 0x000315E8 },
        { ComputeIV32([31, 31, 31, 00, 00, 00]), 0x0003398E },
        { ComputeIV32([00, 31, 31, 00, 31, 31]), 0x00034C3E },
        { ComputeIV32([00, 31, 31, 31, 31, 00]), 0x00038EBB },
        { ComputeIV32([00, 31, 31, 00, 31, 00]), 0x0003A8BF },
        { ComputeIV32([00, 31, 31, 31, 00, 00]), 0x0006A85F },
        { ComputeIV32([00, 00, 31, 00, 31, 31]), 0x0006C940 },
        { ComputeIV32([31, 00, 31, 00, 00, 31]), 0x0007670C },
        { ComputeIV32([31, 31, 00, 31, 00, 00]), 0x00078B1D },
        { ComputeIV32([31, 00, 00, 00, 31, 31]), 0x0008A45E },
        { ComputeIV32([00, 31, 00, 31, 31, 00]), 0x0008CE15 },
        { ComputeIV32([31, 31, 00, 31, 31, 00]), 0x00097342 },
        { ComputeIV32([31, 00, 31, 00, 31, 00]), 0x000F40D0 },
        { ComputeIV32([31, 00, 31, 31, 00, 00]), 0x001138E6 },
        { ComputeIV32([00, 31, 00, 00, 31, 31]), 0x001354BC },
        { ComputeIV32([00, 31, 00, 31, 00, 31]), 0x0016D9D8 },
        { ComputeIV32([00, 31, 31, 00, 00, 31]), 0x001B2914 },
        { ComputeIV32([31, 31, 00, 00, 31, 00]), 0x002D61F4 },
    };

    private static readonly Dictionary<int, uint> FourFlawless = new()
    {
        { ComputeIV32([31, 00, 31, 31, 31, 31]), 0x6ACF03B6 },
        { ComputeIV32([31, 31, 31, 31, 00, 31]), 0x6ACF049D },
        { ComputeIV32([00, 31, 31, 00, 31, 31]), 0x6ACF0757 },
        { ComputeIV32([31, 31, 31, 31, 31, 31]), 0x6ACF0908 },
        { ComputeIV32([31, 31, 31, 00, 31, 31]), 0x6ACF0C45 },
        { ComputeIV32([31, 31, 00, 31, 31, 31]), 0x6ACF0DFD },
        { ComputeIV32([31, 31, 31, 31, 31, 00]), 0x6ACF0F18 },
        { ComputeIV32([31, 00, 31, 31, 00, 31]), 0x6ACF1229 },
        { ComputeIV32([31, 00, 00, 31, 31, 31]), 0x6ACF128D },
        { ComputeIV32([00, 31, 31, 31, 31, 31]), 0x6ACF1644 },
        { ComputeIV32([00, 31, 31, 31, 00, 31]), 0x6ACF191B },
        { ComputeIV32([31, 31, 31, 31, 00, 00]), 0x6ACF1B9F },
        { ComputeIV32([31, 31, 31, 00, 00, 31]), 0x6ACF2B7D },
        { ComputeIV32([31, 31, 00, 00, 31, 31]), 0x6ACF2B81 },
        { ComputeIV32([00, 31, 00, 31, 31, 31]), 0x6ACF2BB9 },
        { ComputeIV32([31, 00, 31, 00, 31, 31]), 0x6ACF2EAD },
        { ComputeIV32([00, 31, 31, 31, 31, 00]), 0x6ACF31B5 },
        { ComputeIV32([31, 31, 31, 00, 31, 00]), 0x6ACF33CB },
        { ComputeIV32([31, 31, 00, 31, 31, 00]), 0x6ACF4FE4 },
        { ComputeIV32([31, 00, 31, 31, 31, 00]), 0x6ACF5BC2 },
        { ComputeIV32([00, 00, 31, 31, 31, 31]), 0x6ACF829A },
        { ComputeIV32([31, 31, 00, 31, 00, 31]), 0x6ACFC2BC },
    };

    private static readonly Dictionary<int, uint> FiveFlawless = new()
    {
        { ComputeIV32([31, 31, 31, 31, 31, 31]), 0x112AAE7B },
        { ComputeIV32([00, 31, 31, 31, 31, 31]), 0x112AAE98 },
        { ComputeIV32([31, 00, 31, 31, 31, 31]), 0x112AAEAB },
        { ComputeIV32([31, 31, 31, 31, 31, 00]), 0x112AAEB3 },
        { ComputeIV32([31, 31, 31, 31, 00, 31]), 0x112AAF49 },
        { ComputeIV32([31, 31, 00, 31, 31, 31]), 0x112AAF7F },
        { ComputeIV32([31, 31, 31, 00, 31, 31]), 0x112AB13B },
    };

    private static int GetWildSeedFromIV8(ReadOnlySpan<int> fixedCountIVs, int iv32, out uint seed)
    {
        foreach (int i in fixedCountIVs)
        {
            if (CheckValidSeed(i, iv32, out seed))
                return i;
        }
        seed = 0;
        return -1;
    }

    private static int GetWildSeedFromIV8(int fixedivs, int iv32, out uint seed)
    {
        return CheckValidSeed(fixedivs, iv32, out seed) ? fixedivs : -1;
    }

    private static bool CheckValidSeed(int fixedCount, int iv32, out uint seed)
    {
        var seeds = GetSeedSet(fixedCount);
        return seeds.TryGetValue(iv32, out seed);
    }

    private static Dictionary<int, uint> GetSeedSet(int fixedCount) => fixedCount switch
    {
        1 => OneFlawless,
        2 => TwoFlawless,
        3 => ThreeFlawless,
        4 => FourFlawless,
        5 => FiveFlawless,
        _ => ZeroFlawless,
    };

    private static ReadOnlySpan<int> FlawlessWild8 => [0, 2, 3];

    public static int GetFlawlessIVCount(IEncounterTemplate enc, ReadOnlySpan<int> ivs, out uint seed)
    {
        var iv32 = ComputeIV32Swapped(ivs);
        seed = 0;
        return enc switch
        {
            EncounterSlot8 => GetWildSeedFromIV8(FlawlessWild8, iv32, out seed),
            EncounterStatic8 s => GetWildSeedFromIV8(s.FlawlessIVCount, iv32, out seed),
            _ => -1,
        };
    }

    private static int ComputeIV32(ReadOnlySpan<byte> arr)
    {
        int result = 0;
        for (int i = 0; i < arr.Length; i++)
            result |= arr[i] << (i * 5);

        return result;
    }

    private static int ComputeIV32Swapped(ReadOnlySpan<int> ivs)
    {
        // { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD } (original order)
        // { IV_HP, IV_ATK, IV_DEF, IV_SPA, IV_SPD, IV_SPE } (corrected order)
        int result = 0;
        result |= ivs[0] << (5 * 0);
        result |= ivs[1] << (5 * 1);
        result |= ivs[2] << (5 * 2);
        result |= ivs[4] << (5 * 3);
        result |= ivs[5] << (5 * 4);
        result |= ivs[3] << (5 * 5);
        return result;
    }
}
