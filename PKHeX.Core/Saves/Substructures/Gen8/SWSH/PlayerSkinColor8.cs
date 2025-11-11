using System;

namespace PKHeX.Core;

public enum PlayerSkinColor8
{
    PaleM = 0,
    PaleF = 1,
    DefaultM = 2,
    DefaultF = 3,
    TanM = 4,
    TanF = 5,
    DarkM = 6,
    DarkF = 7,
}

public static class PlayerSkinColor8Extensions
{
    private static ReadOnlySpan<ulong> SkinValues => [
        0xDA95FC34AFA29E9F, // PaleM
        0xD9A6510BD62C8864, // PaleF
        0xDA95FD34AFA2A052, // DefaultM
        0xD9A6540BD62C8D7D, // DefaultF
        0xDA95FB34AFA29CEC, // TanM
        0xD9A6520BD62C8A17, // TanF
        0xDA95FA34AFA29B39, // DarkM
        0xD9A64F0BD62C84FE, // DarkF
    ];

    private static ReadOnlySpan<ulong> HairValues => [
        0xED842387576E5C76, // PaleM
        0xE544083D200E4561, // PaleF
        0x9B49D9CF3A42AAC6, // DefaultM
        0x978457BF2B4C257D, // DefaultF
        0x9B49D9CF3A42AAC6, // TanM
        0x978457BF2B4C257D, // TanF
        0xAD67ECF558C3B381, // DarkM
        0x9BB169EFF522D2E2, // DarkF
    ];

    private static ReadOnlySpan<ulong> BrowValues => [
        0x96F8CEBA797121F2, // PaleM
        0x4C74B1222409821F, // PaleF
        0x17403EAE74639AB2, // DefaultM
        0x7822A8288780EF2F, // DefaultF
        0x17403EAE74639AB2, // TanM
        0x7822A8288780EF2F, // TanF
        0x6DE846C94E4F4A75, // DarkM
        0x9FCB5F8634864224, // DarkF
    ];

    private static ReadOnlySpan<ulong> ContactsValues => [
        0x5CC02A7B63A603CA, // PaleM
        0x6E7DFEB09B91E01F, // PaleF
        0xD85897820DFA751F, // DefaultM
        0xD08819DC1FA6A630, // DefaultF
        0xD85897820DFA751F, // TanM
        0xD08819DC1FA6A630, // TanF
        0xFD48FA3903544730, // DarkM
        0x243A6DC814240ADB, // DarkF
    ];

    private static ReadOnlySpan<ulong> LipsValues => [
        0x74D1BDA4ABD8145C, // PaleM
        0xD1CF0934B368C918, // PaleF
        0x74D1BDA4ABD8145C, // DefaultM
        0xD1CF0C34B368CE31, // DefaultF
        0x74D1BDA4ABD8145C, // TanM
        0xD1CF0A34B368CACB, // TanF
        0x74D1BDA4ABD8145C, // DarkM
        0xD1CF0F34B368D34A, // DarkF
    ];

    private static ReadOnlySpan<ulong> MomSkinValues => [
        0xE2245380DA099773, // PaleM
        0x00ECCDE1E75B6B3F, // PaleF
        0xE2245280DA0995C0, // DefaultM
        0x00ECCCE1E75B698C, // DefaultF
        0xE2245480DA099926, // TanM
        0x00ECCEE1E75B6CF2, // TanF
        0xE2245580DA099AD9, // DarkM
        0x00ECCFE1E75B6EA5, // DarkF
    ];

    extension(PlayerSkinColor8 skinColor)
    {
        public ulong Skin() => SkinValues[(int)skinColor];
        public ulong Hair() => HairValues[(int)skinColor];
        public ulong Brow() => BrowValues[(int)skinColor];
        public ulong Contacts() => ContactsValues[(int)skinColor];
        public ulong Lips() => LipsValues[(int)skinColor];
        public ulong MomSkin() => MomSkinValues[(int)skinColor];
    }

    public static PlayerSkinColor8 GetSkinColorFromSkin(ulong skin) => (PlayerSkinColor8)(SkinValues.IndexOf(skin));
}
