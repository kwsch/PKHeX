using System;

namespace PKHeX.Core;

public sealed class BlueberryClubRoom9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public BlueberrySupportBoard9 SupportBoard => new(block.Data.AsMemory(0, 0x38));

    public BlueberryClubRoomStyle9 CurrentStyle { get => (BlueberryClubRoomStyle9)Data[0xE6C]; set => Data[0xE6C] = (byte)value; }
}

public sealed class BlueberrySupportBoard9(Memory<byte> Data)
{
    private Span<byte> Span => Data.Span;

    private bool GetBoolean(int index) => Span[index] == 1;
    private void SetBoolean(int index, bool value) => Span[index] = value ? (byte)1 : (byte)0;

    public bool BaseballClub1SmugElegantPurchased { get => GetBoolean(0x04); set => SetBoolean(0x04, value); }
    public bool BaseballClub1SmugElegantUnread { get => GetBoolean(0x05); set => SetBoolean(0x05, value); }

    public bool BaseballClub2TwirlingNinjaPurchased { get => GetBoolean(0x06); set => SetBoolean(0x06, value); }
    public bool BaseballClub2TwirlingNinjaUnread { get => GetBoolean(0x07); set => SetBoolean(0x07, value); }

    public bool BaseballClub3ChampionPurchased { get => GetBoolean(0x08); set => SetBoolean(0x08, value); }
    public bool BaseballClub3ChampionUnread { get => GetBoolean(0x09); set => SetBoolean(0x09, value); }

    public bool BaseballClubChangeStylePurchased { get => GetBoolean(0x0A); set => SetBoolean(0x0A, value); }
    public bool BaseballClubChangeStyleUnread { get => GetBoolean(0x0B); set => SetBoolean(0x0B, value); }

    public bool ScienceClubItemPrinterPurchased { get => GetBoolean(0x0C); set => SetBoolean(0x0C, value); }
    public bool ScienceClubItemPrinterUnread { get => GetBoolean(0x0D); set => SetBoolean(0x0D, value); }

    public bool PhotographyClub1NewEffectsPurchased { get => GetBoolean(0x0E); set => SetBoolean(0x0E, value); }
    public bool PhotographyClub1NewEffectsUnread { get => GetBoolean(0x0F); set => SetBoolean(0x0F, value); }

    public bool PhotographyClub2CuteEffectsPurchased { get => GetBoolean(0x10); set => SetBoolean(0x10, value); }
    public bool PhotographyClub2CuteEffectsUnread { get => GetBoolean(0x11); set => SetBoolean(0x11, value); }

    public bool PhotographyClub3CoolEffectsPurchased { get => GetBoolean(0x12); set => SetBoolean(0x12, value); }
    public bool PhotographyClub3CoolEffectsUnread { get => GetBoolean(0x13); set => SetBoolean(0x13, value); }

    public bool PhotographyClub4TouchUpPurchased { get => GetBoolean(0x14); set => SetBoolean(0x14, value); }
    public bool PhotographyClub4TouchUpUnread { get => GetBoolean(0x15); set => SetBoolean(0x15, value); }

    public bool PhotographyClub5LockOnPurchased { get => GetBoolean(0x16); set => SetBoolean(0x16, value); }
    public bool PhotographyClub5LockOnUnread { get => GetBoolean(0x17); set => SetBoolean(0x17, value); }

    public bool ArtClubRemodelPurchased { get => GetBoolean(0x1A); set => SetBoolean(0x1A, value); }
    public bool ArtClubRemodelUnread { get => GetBoolean(0x1B); set => SetBoolean(0x1B, value); }

    public bool ArtClub1FancyDarkPurchased { get => GetBoolean(0x1C); set => SetBoolean(0x1C, value); }
    public bool ArtClub1FancyDarkUnread { get => GetBoolean(0x1D); set => SetBoolean(0x1D, value); }

    public bool ArtClub2ClassicFuturisticPurchased { get => GetBoolean(0x1E); set => SetBoolean(0x1E, value); }
    public bool ArtClub2ClassicFuturisticUnread { get => GetBoolean(0x1F); set => SetBoolean(0x1F, value); }

    public bool ArtClub3GorgeousPurchased { get => GetBoolean(0x20); set => SetBoolean(0x20, value); }
    public bool ArtClub3GorgeousUnread { get => GetBoolean(0x21); set => SetBoolean(0x21, value); }

    public bool MusicClub1NewSpeakerPurchased { get => GetBoolean(0x22); set => SetBoolean(0x22, value); }
    public bool MusicClub1NewSpeakerUnread { get => GetBoolean(0x23); set => SetBoolean(0x23, value); }

    public bool MusicClub2OutdoorAdventuresAlbumPurchased { get => GetBoolean(0x24); set => SetBoolean(0x24, value); }
    public bool MusicClub2OutdoorAdventuresAlbumUnread { get => GetBoolean(0x25); set => SetBoolean(0x25, value); }

    public bool MusicClub3TownLifeAlbumPurchased { get => GetBoolean(0x26); set => SetBoolean(0x26, value); }
    public bool MusicClub3TownLifeAlbumUnread { get => GetBoolean(0x27); set => SetBoolean(0x27, value); }

    public bool MusicClub4FamousDestinationsAlbumPurchased { get => GetBoolean(0x28); set => SetBoolean(0x28, value); }
    public bool MusicClub4FamousDestinationsAlbumUnread { get => GetBoolean(0x29); set => SetBoolean(0x29, value); }

    public bool MusicClub5KitakamiAlbumPurchased { get => GetBoolean(0x2A); set => SetBoolean(0x2A, value); }
    public bool MusicClub5KitakamiAlbumUnread { get => GetBoolean(0x2B); set => SetBoolean(0x2B, value); }

    public bool MusicClub6SpecialSongsAlbumPurchased { get => GetBoolean(0x2C); set => SetBoolean(0x2C, value); }
    public bool MusicClub6SpecialSongsAlbumUnread { get => GetBoolean(0x2D); set => SetBoolean(0x2D, value); }

    public bool TerariumClubBiodiversityCoastalBiomePurchased { get => GetBoolean(0x2E); set => SetBoolean(0x2E, value); }
    public bool TerariumClubBiodiversityCoastalBiomeUnread { get => GetBoolean(0x2F); set => SetBoolean(0x2F, value); }

    public bool TerariumClubBiodiversitySavannaBiomePurchased { get => GetBoolean(0x30); set => SetBoolean(0x30, value); }
    public bool TerariumClubBiodiversitySavannaBiomeUnread { get => GetBoolean(0x31); set => SetBoolean(0x31, value); }

    public bool TerariumClubBiodiversityPolarBiomePurchased { get => GetBoolean(0x32); set => SetBoolean(0x32, value); }
    public bool TerariumClubBiodiversityPolarBiomeUnread { get => GetBoolean(0x33); set => SetBoolean(0x33, value); }

    public bool TerariumClubBiodiversityCanyonBiomePurchased { get => GetBoolean(0x34); set => SetBoolean(0x34, value); }
    public bool TerariumClubBiodiversityCanyonBiomeUnread { get => GetBoolean(0x35); set => SetBoolean(0x35, value); }

    public bool SecretBossPurchased { get => GetBoolean(0x36); set => SetBoolean(0x36, value); }
    public bool SecretBossUnread { get => GetBoolean(0x37); set => SetBoolean(0x37, value); }
}

public enum BlueberryClubRoomStyle9 : byte
{
    Normal = 0,
    Gorgeous = 1,
    Fancy = 2,
    Futuristic = 3,
    Classic = 4,
    Monochrome = 5,
    Natural = 6,
    Dark = 7,
}
