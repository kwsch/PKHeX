using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary> Generation 6 <see cref="PKM"/> format. </summary>
public abstract class G6PKM : PKM, ISanityChecksum, IHandlerUpdate
{
    public override int SIZE_PARTY => PokeCrypto.SIZE_6PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_6STORED;
    protected G6PKM(byte[] data) : base(data) { }
    protected G6PKM([ConstantExpected] int size) : base(size) { }

    // Trash Bytes
    public sealed override Span<byte> NicknameTrash => Data.AsSpan(0x40, 26);
    public sealed override Span<byte> HandlingTrainerTrash => Data.AsSpan(0x78, 26);
    public sealed override Span<byte> OriginalTrainerTrash => Data.AsSpan(0xB0, 26);
    public override int TrashCharCountTrainer => 13;
    public override int TrashCharCountNickname => 13;

    public abstract ushort Sanity { get; set; }
    public abstract ushort Checksum { get; set; }
    public sealed override void RefreshChecksum() => Checksum = CalculateChecksum();
    public sealed override bool ChecksumValid => CalculateChecksum() == Checksum;
    public sealed override bool Valid { get => Sanity == 0 && ChecksumValid; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

    private ushort CalculateChecksum() => Checksums.Add16(Data.AsSpan()[8..PokeCrypto.SIZE_6STORED]);

    // Simple Generated Attributes
    public sealed override byte CurrentFriendship
    {
        get => CurrentHandler == 0 ? OriginalTrainerFriendship : HandlingTrainerFriendship;
        set { if (CurrentHandler == 0) OriginalTrainerFriendship = value; else HandlingTrainerFriendship = value; }
    }

    public byte OppositeFriendship
    {
        get => CurrentHandler == 1 ? OriginalTrainerFriendship : HandlingTrainerFriendship;
        set { if (CurrentHandler == 1) OriginalTrainerFriendship = value; else HandlingTrainerFriendship = value; }
    }

    public sealed override uint PSV => ((PID >> 16) ^ (PID & 0xFFFF)) >> 4;
    public sealed override uint TSV => (uint)(TID16 ^ SID16) >> 4;
    public sealed override bool IsUntraded => Data[0x78] == 0 && Data[0x78 + 1] == 0 && Format == Generation; // immediately terminated HandlingTrainerName data (\0)

    // Complex Generated Attributes
    public abstract uint IV32 { get; set; }
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
    public bool BelongsTo(ITrainerInfo tr)
    {
        if (tr.Version != Version)
            return false;
        if (tr.ID32 != ID32)
            return false;
        if (tr.Gender != OriginalTrainerGender)
            return false;

        Span<char> ot = stackalloc char[MaxStringLengthTrainer];
        int len = LoadString(OriginalTrainerTrash, ot);
        return ot[..len].SequenceEqual(tr.OT);
    }

    public void UpdateHandler(ITrainerInfo tr)
    {
        if (IsEgg)
        {
            // Eggs do not have any modifications done if they are traded
            // Apply link trade data, only if it left the OT (ignore if dumped & imported, or cloned, etc.)
            const ushort location = Locations.LinkTrade6;
            if (MetLocation != location && !BelongsTo(tr))
            {
                var date = EncounterDate.GetDate3DS();
                SetLinkTradeEgg(date.Day, date.Month, date.Year, location);
            }
            return;
        }

        // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
        var handler = CurrentHandler;
        if (!TradeOT(tr))
            TradeHT(tr);
        if (handler == CurrentHandler)
            return; // Logic updated Friendship
        // Copy over the Friendship Value only under certain circumstances
        if (HasMove((int)Move.Return) || HasMove((int)Move.Frustration))
            CurrentFriendship = OppositeFriendship;
    }

    protected abstract bool TradeOT(ITrainerInfo tr);
    protected abstract void TradeHT(ITrainerInfo tr);

    // Maximums
    public sealed override int MaxIV => 31;
    public sealed override int MaxEV => EffortValues.Max252;
    public sealed override int MaxStringLengthTrainer => 12;
    public sealed override int MaxStringLengthNickname => 12;
}

public interface ISuperTrain
{
    uint SuperTrainBitFlags { get; set; }
    bool SecretSuperTrainingUnlocked { get; set; }
    bool SecretSuperTrainingComplete { get; set; }
    int SuperTrainingMedalCount(int lowBitCount = 30);
}
