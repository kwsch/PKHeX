namespace PKHeX.Core
{
    public enum BelugaBlockIndex
    {
        /* 00 @ 0x00000, len = 0x00D90 */ MyItem,
        /* 01 @ 0x00E00, len = 0x00200 */ _01,
        /* 02 @ 0x01000, len = 0x00168 */ MyStatus,
        /* 03 @ 0x01200, len = 0x01800 */ EventWork,
        /* 04 @ 0x02A00, len = 0x020E8 */ Zukan,
        /* 05 @ 0x04C00, len = 0x00930 */ Misc, // rival stuff
        /* 06 @ 0x05600, len = 0x00004 */ ConfigSave,
        /* 07 @ 0x05800, len = 0x00130 */ PlayerGeoLocation,
        /* 08 @ 0x05A00, len = 0x00012 */ PokeListHeader,
        /* 09 @ 0x05C00, len = 0x3F7A0 */ PokeListPokemon,
        /* 10 @ 0x45400, len = 0x00008 */ PlayTime,
        /* 11 @ 0x45600, len = 0x00E90 */ WB7Record,
        /* 12 @ 0x46600, len = 0x010A4 */ CaptureRecord,
        /* 13 @ 0x47800, len = 0x000F0 */ _13,
        /* 14 @ 0x47A00, len = 0x06010 */ _14,
        /* 15 @ 0x4DC00, len = 0x00200 */ _15, // stuff containing data about recent captures?
        /* 16 @ 0x4DE00, len = 0x00098 */ _16,
        /* 17 @ 0x4E000, len = 0x00068 */ _17,
        /* 18 @ 0x4E200, len = 0x69780 */ GoParkEntities,
        /* 19 @ 0xB7A00, len = 0x000B0 */ GoGoParkNames,
        /* 20 @ 0xB7C00, len = 0x00940 */ _20, // Go Park Names

        Record,
    }
}
