using System;

namespace PKHeX.Core;

/// <summary>
/// Storage for the species that was fused into <see cref="Species.Kyurem"/> and <see cref="Species.Necrozma"/>.
/// </summary>
public sealed class Fused8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data)
{
    private const int SizeStored = PokeCrypto.SIZE_8PARTY;

    public static int GetFusedSlotOffset(int slot)
    {
        if ((uint)slot >= 3)
            return -1;
        return SizeStored * slot;
    }

    public Memory<byte> this[int i] => Raw.Slice(GetFusedSlotOffset(i), SizeStored);
    private Span<byte> GetSlotSpan(int index) => Data.Slice(GetFusedSlotOffset(index), SizeStored);
    private PK8 GetStoredSlot(int index) => (PK8)SAV.GetStoredSlot(GetSlotSpan(index));
    private void SetStoredSlot(PK8 pk, int index) => pk.EncryptedBoxData.CopyTo(GetSlotSpan(index));

    public PK8 Kyurem           { get => GetStoredSlot(0); set => SetStoredSlot(value, 0); }
    public PK8 NecrozmaSolgaleo { get => GetStoredSlot(1); set => SetStoredSlot(value, 1); }
    public PK8 NecrozmaLunala   { get => GetStoredSlot(2); set => SetStoredSlot(value, 2); }
}
