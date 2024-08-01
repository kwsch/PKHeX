using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class KeySystem5(SAV5B2W2 SAV, Memory<byte> raw) : SaveBlock<SAV5B2W2>(SAV, raw)
{
    // 0x00-0x27: Unknown
    private const int OffsetKeysObtained = 0x28; // 5 * sizeof(uint)
    private const int OffsetKeysUnlocked = 0x3C; // 5 * sizeof(uint)
    // 3x selections (Difficulty, City, Chamber) - 3 * sizeof(uint)
    private const int OffsetCrypto = 0x5C;

    // The game uses a simple XOR encryption and hardcoded magic numbers to indicate selections and key status.
    // If the value is not zero, it is encrypted with the XOR key. Compare to the associated magic number.
    // Magic numbers (game code), found in ram: 0x0208FA24
    // Selections - Magic Numbers
    private const uint MagicCityWhiteForest     = 0x34525;
    private const uint MagicCityBlackCity       = 0x11963;
    private const uint MagicDifficultyEasy      = 0x31239;
    private const uint MagicDifficultyNormal    = 0x15657;
    private const uint MagicDifficultyChallenge = 0x49589;
    private const uint MagicMysteryDoorRock     = 0x94525;
    private const uint MagicMysteryDoorIron     = 0x81963;
    private const uint MagicMysteryDoorIceberg  = 0x38569;

    // magic numbers, immediately after ^ in ram. same as below set
    private static ReadOnlySpan<uint> MagicKeyObtained =>
    [
        0x35691, // 0x28 Obtained Key (EasyMode)
        0x18256, // 0x2C Obtained Key (Challenge)
        0x59389, // 0x30 Obtained Key (City)
        0x48292, // 0x34 Obtained Key (Iron)
        0x09892, // 0x38 Obtained Key (Iceberg)
    ];

    private static ReadOnlySpan<uint> MagicKeyUnlocked =>
    [
        0x93389, // 0x3C Unlocked (EasyMode)
        0x22843, // 0x40 Unlocked (Challenge)
        0x34771, // 0x44 Unlocked (City)
        0xAB031, // 0x48 Unlocked (Iron)
        0xB3818, // 0x4C Unlocked (Iceberg)
    ];

    public bool GetIsKeyObtained(KeyType5 key)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)key, (uint)KeyType5.Iceberg);
        var offset = OffsetKeysObtained + (sizeof(uint) * (int)key);
        var expect = MagicKeyObtained[(int)key] ^ Crypto;
        var value = ReadUInt32LittleEndian(Data[offset..]);
        return value == expect;
    }

    public void SetIsKeyObtained(KeyType5 key, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)key, (uint)KeyType5.Iceberg);
        var offset = OffsetKeysObtained + (sizeof(uint) * (int)key);
        var expect = MagicKeyObtained[(int)key] ^ Crypto;
        WriteUInt32LittleEndian(Data[offset..], value ? expect : 0);
    }

    public bool GetIsKeyUnlocked(KeyType5 key)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)key, (uint)KeyType5.Iceberg);
        var offset = OffsetKeysUnlocked + (sizeof(uint) * (int)key);
        var expect = MagicKeyUnlocked[(int)key] ^ Crypto;
        var value = ReadUInt32LittleEndian(Data[offset..]);
        return value == expect;
    }

    public void SetIsKeyUnlocked(KeyType5 key, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)key, (uint)KeyType5.Iceberg);
        var offset = OffsetKeysUnlocked + (sizeof(uint) * (int)key);
        var expect = MagicKeyUnlocked[(int)key] ^ Crypto;
        WriteUInt32LittleEndian(Data[offset..], value ? expect : 0);
    }

    // 0x50 - Difficulty Selected (uses selection's magic number) - 0 if default
    public Difficulty5 ActiveDifficulty
    {
        get
        {
            uint value = ReadUInt32LittleEndian(Data[0x50..]);
            if (value is 0)
                return Difficulty5.Normal;

            var xor = value ^ Crypto;
            return xor switch
            {
                MagicDifficultyEasy => Difficulty5.Easy,
                MagicDifficultyNormal => Difficulty5.Normal,
                MagicDifficultyChallenge => Difficulty5.Challenge,
                _ => Difficulty5.Normal, // default
            };
        }
        set
        {
            if (value is Difficulty5.Normal)
            {
                WriteUInt32LittleEndian(Data[0x50..], 0);
                return;
            }
            uint write = value switch
            {
                Difficulty5.Easy => MagicDifficultyEasy,
                Difficulty5.Challenge => MagicDifficultyChallenge,
                _ => MagicDifficultyNormal, // default
            };
            WriteUInt32LittleEndian(Data[0x50..], write ^ Crypto);
        }
    }

    // 0x54 - City Selected (uses selection's magic number) - 0 if default
    public City5 ActiveCity
    {
        get
        {
            var initial = SAV.Version is GameVersion.W2 ? City5.WhiteForest : City5.BlackCity;
            uint value = ReadUInt32LittleEndian(Data[0x54..]);
            if (value is 0)
                return initial;

            var xor = value ^ Crypto;
            return xor switch
            {
                MagicCityWhiteForest => City5.WhiteForest,
                MagicCityBlackCity => City5.BlackCity,
                _ => initial, // default
            };
        }
        set
        {
            var initial = SAV.Version is GameVersion.W2 ? City5.WhiteForest : City5.BlackCity;
            if (value == initial)
            {
                WriteUInt32LittleEndian(Data[0x54..], 0);
                return;
            }
            uint write = value == City5.WhiteForest ? MagicCityWhiteForest : MagicCityBlackCity;
            WriteUInt32LittleEndian(Data[0x54..], write ^ Crypto);
        }
    }

    // 0x58 - Chamber Selected (uses selection's magic number) - 0 if default
    public Chamber5 ActiveChamber
    {
        get
        {
            uint value = ReadUInt32LittleEndian(Data[0x58..]);
            if (value is 0)
                return Chamber5.Rock;

            var xor = value ^ Crypto;
            return xor switch
            {
                MagicMysteryDoorRock => Chamber5.Rock,
                MagicMysteryDoorIron => Chamber5.Iron,
                MagicMysteryDoorIceberg => Chamber5.Iceberg,
                _ => Chamber5.Rock, // default
            };
        }
        set
        {
            if (value is Chamber5.Rock)
            {
                WriteUInt32LittleEndian(Data[0x58..], 0);
                return;
            }
            uint write = value switch
            {
                Chamber5.Iron => MagicMysteryDoorIron,
                Chamber5.Iceberg => MagicMysteryDoorIceberg,
                _ => MagicMysteryDoorRock, // default
            };
            WriteUInt32LittleEndian(Data[0x58..], write ^ Crypto);
        }
    }

    // This value should be > 0xFFFFF to ensure the magic numbers aren't visible in savedata.
    public uint Crypto
    {
        get => ReadUInt32LittleEndian(Data[OffsetCrypto..]);
        set => WriteUInt32LittleEndian(Data[OffsetCrypto..], value);
    }
}

public enum Difficulty5
{
    Easy = 0,
    Normal = 1,
    Challenge = 2,
}

public enum City5
{
    WhiteForest = 0,
    BlackCity = 1,
}

public enum Chamber5
{
    Rock = 0,
    Iron = 1,
    Iceberg = 2,
}

public enum KeyType5
{
    Easy = 0,
    Challenge = 1,
    City = 2,
    Iron = 3,
    Iceberg = 4,
}
