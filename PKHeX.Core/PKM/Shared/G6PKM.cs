using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary> Generation 6 <see cref="PKM"/> format. </summary>
public abstract class G6PKM : PKM, ISanityChecksum
{
    public override int SIZE_PARTY => PokeCrypto.SIZE_6PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_6STORED;
    protected G6PKM(byte[] data) : base(data) { }
    protected G6PKM([ConstantExpected] int size) : base(size) { }

    // Trash Bytes
    public sealed override Span<byte> Nickname_Trash => Data.AsSpan(0x40, 26);
    public sealed override Span<byte> HT_Trash => Data.AsSpan(0x78, 26);
    public sealed override Span<byte> OT_Trash => Data.AsSpan(0xB0, 26);

    public abstract ushort Sanity { get; set; }
    public abstract ushort Checksum { get; set; }
    public sealed override void RefreshChecksum() => Checksum = CalculateChecksum();
    public sealed override bool ChecksumValid => CalculateChecksum() == Checksum;
    public sealed override bool Valid { get => Sanity == 0 && ChecksumValid; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

    private ushort CalculateChecksum() => Checksums.Add16(Data.AsSpan()[8..PokeCrypto.SIZE_6STORED]);

    // Simple Generated Attributes
    public sealed override int CurrentFriendship
    {
        get => CurrentHandler == 0 ? OT_Friendship : HT_Friendship;
        set { if (CurrentHandler == 0) OT_Friendship = value; else HT_Friendship = value; }
    }

    public int OppositeFriendship
    {
        get => CurrentHandler == 1 ? OT_Friendship : HT_Friendship;
        set { if (CurrentHandler == 1) OT_Friendship = value; else HT_Friendship = value; }
    }

    public sealed override uint PSV => ((PID >> 16) ^ (PID & 0xFFFF)) >> 4;
    public sealed override uint TSV => (uint)(TID16 ^ SID16) >> 4;
    public sealed override bool IsUntraded => Data[0x78] == 0 && Data[0x78 + 1] == 0 && Format == Generation; // immediately terminated HT_Name data (\0)

    // Complex Generated Attributes
    protected abstract uint IV32 { get; set; }
    public override int Characteristic => EntityCharacteristic.GetCharacteristic(EncryptionConstant, IV32);

    // Methods
    protected sealed override byte[] Encrypt()
    {
        RefreshChecksum();
        return PokeCrypto.EncryptArray6(Data);
    }

    // General User-error Fixes
    public void FixRelearn()
    {
        while (true)
        {
            if (RelearnMove4 != 0 && RelearnMove3 == 0)
            {
                RelearnMove3 = RelearnMove4;
                RelearnMove4 = 0;
            }
            if (RelearnMove3 != 0 && RelearnMove2 == 0)
            {
                RelearnMove2 = RelearnMove3;
                RelearnMove3 = 0;
                continue;
            }
            if (RelearnMove2 != 0 && RelearnMove1 == 0)
            {
                RelearnMove1 = RelearnMove2;
                RelearnMove2 = 0;
                continue;
            }
            break;
        }
    }

    // Synthetic Trading Logic
    public void Trade(ITrainerInfo tr, int Day = 1, int Month = 1, int Year = 2015)
    {
        if (IsEgg)
        {
            // Eggs do not have any modifications done if they are traded
            // Apply link trade data, only if it left the OT (ignore if dumped & imported, or cloned, etc.)
            if ((tr.TID16 != TID16) || (tr.SID16 != SID16) || (tr.Gender != OT_Gender) || (tr.OT != OT_Name))
                SetLinkTradeEgg(Day, Month, Year, Locations.LinkTrade6);
            return;
        }

        // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
        if (!TradeOT(tr))
            TradeHT(tr);
    }

    protected abstract bool TradeOT(ITrainerInfo tr);
    protected abstract void TradeHT(ITrainerInfo tr);

    // Maximums
    public sealed override int MaxIV => 31;
    public sealed override int MaxEV => EffortValues.Max252;
    public sealed override int MaxStringLengthOT => 12;
    public sealed override int MaxStringLengthNickname => 12;
}

public interface ISuperTrain
{
    uint SuperTrainBitFlags { get; set; }
    bool SecretSuperTrainingUnlocked { get; set; }
    bool SecretSuperTrainingComplete { get; set; }
    int SuperTrainingMedalCount(int lowBitCount = 30);
}
