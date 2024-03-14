using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Entrée Forest
/// </summary>
public sealed class EntreeForest(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    /// <summary> Areas 1 through 8 have 20 slots. </summary>
    private const byte Count18 = 20;

    /// <summary> 9th Area has only 10 slots. </summary>
    private const byte Count9 = 10;

    private const int TotalSlots = Count18 + (3 * 8 * Count18) + (3 * Count9); // 530

    /// <summary> Areas 3 through 8 can be unlocked (set a value 0 to 6). </summary>
    private const byte MaxUnlock38Areas = 6;

    private const int EncryptionSeedOffset = SIZE - 4; // 0x84C
    public const int SIZE = 0x850;

    private Span<byte> DataRegion => Data[..EncryptionSeedOffset]; // 0x84C

    private bool IsDecrypted;
    public void EndAccess() => EnsureDecrypted(false);
    public void StartAccess() => EnsureDecrypted();
    public void EnsureDecrypted(bool state = true)
    {
        if (IsDecrypted == state)
            return;
        PokeCrypto.CryptArray(DataRegion, EncryptionSeed);
        IsDecrypted = state;
    }

    /// <summary>
    /// Gets all Entree Slot data.
    /// </summary>
    public EntreeSlot[] Slots
    {
        get
        {
            EnsureDecrypted();
            var slots = new EntreeSlot[TotalSlots];
            for (int i = 0; i < slots.Length; i++)
                slots[i] = new EntreeSlot(Raw.Slice(i * EntreeSlot.SIZE, EntreeSlot.SIZE)) { Area = GetSlotArea(i) };
            return slots;
        }
    }

    /// <summary>
    /// Determines if the 9th Area is available to enter.
    /// </summary>
    public bool Unlock9thArea
    {
        get => Data[0x848] == 1;
        set => Data[0x848] = value ? (byte)1 : (byte)0;
    }

    /// <summary>
    /// Determines how many extra areas are available to enter. Areas 1 &amp; 2 are already available by default.
    /// </summary>
    public int Unlock38Areas
    {
        get => Data[0x849] & 7;
        set => Data[0x849] = (byte)((Data[0x849] & ~7) | Math.Min(MaxUnlock38Areas, value));
    }

    public uint EncryptionSeed
    {
        get => ReadUInt32LittleEndian(Data[EncryptionSeedOffset..]);
        private set => WriteUInt32LittleEndian(Data[EncryptionSeedOffset..], value);
    }

    public void UnlockAllAreas()
    {
        Unlock38Areas = MaxUnlock38Areas;
        Unlock9thArea = true;
    }

    public void DeleteAll()
    {
        foreach (var e in Slots)
            e.Delete();
    }

    private static EntreeForestArea GetSlotArea(int index)
    {
        if (index < Count18)
            return EntreeForestArea.Deepest;
        index -= Count18;

        const int slots9 = 3 * Count9;
        if (index < slots9)
            return EntreeForestArea.Ninth | GetSlotPosition(index / Count9);
        index -= slots9;

        const int slots18 = 3 * Count18;
        int area = index / slots18;
        if (area >= 8)
            throw new ArgumentOutOfRangeException(nameof(index));
        index %= slots18;

        return (EntreeForestArea)((int)EntreeForestArea.First << area) | GetSlotPosition(index / Count18);
    }

    private static EntreeForestArea GetSlotPosition(int index) => index switch
    {
        0 => EntreeForestArea.Center,
        1 => EntreeForestArea.Left,
        2 => EntreeForestArea.Right,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };
}
