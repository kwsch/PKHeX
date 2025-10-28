using System;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Header information for the stored <see cref="PKM"/> data.
/// </summary>
/// <remarks>
/// This block simply contains the following:
/// u16 Party Pointers * 6: Indicates which index occupies this slot. <see cref="SLOT_EMPTY"/> if nothing in slot.
/// u16 Starter Pointer: Indicates which index is the starter. <see cref="SLOT_EMPTY"/> if no starter yet.
/// u16 List Count: Points to the next empty slot, and indicates how many slots are stored in the list.
/// </remarks>
public sealed class PokeListHeader : SaveBlock<SAV7b>
{
    public PokeListHeader(SAV7b sav, Memory<byte> raw, bool exportable) : base(sav, raw)
        => PokeListInfo = Initialize(exportable);

    /// <summary>
    /// Raw representation of data, cast to ushort[].
    /// </summary>
    internal readonly int[] PokeListInfo;

    public const int STARTER = 6;
    private const int COUNT = 7;
    private const int MAX_SLOTS = 1000;
    private const int SLOT_EMPTY = 1001;

    private int[] Initialize(bool exportable)
    {
        var info = LoadPointerData();
        if (!exportable)
        {
            info.AsSpan().Fill(SLOT_EMPTY);
        }
        else
        {
            // Count the party slots that are valid
            for (int i = 0; i < 6; i++)
            {
                if (info[i] < MAX_SLOTS)
                    ++_partyCount;
            }
        }
        return info;
    }

    private int _partyCount;

    public int PartyCount
    {
        get => _partyCount;
        set
        {
            if (_partyCount > value)
            {
                for (int i = _partyCount; i < value; i++)
                    ClearPartySlot(i);
            }
            _partyCount = value;
        }
    }

    public bool ClearPartySlot(int slot)
    {
        if (slot >= 6 || PartyCount <= 1)
            return false;

        if (slot > PartyCount)
        {
            slot = PartyCount;
        }
        else if (slot != PartyCount - 1)
        {
            int countShiftDown = PartyCount - 1 - slot;
            Array.Copy(PokeListInfo, slot + 1, PokeListInfo, slot, countShiftDown);
            slot = PartyCount - 1;
        }
        PokeListInfo[slot] = SLOT_EMPTY;
        PartyCount--;
        return true;
    }

    public void RemoveStarter() => StarterIndex = SLOT_EMPTY;

    public int StarterIndex
    {
        get => PokeListInfo[STARTER];
        set
        {
            if ((ushort)value > 1000 && value != SLOT_EMPTY)
                throw new ArgumentOutOfRangeException(nameof(value));
            PokeListInfo[STARTER] = (ushort)value;
        }
    }

    public int Count
    {
        get => ReadUInt16LittleEndian(Data[(COUNT * 2)..]);
        set => WriteUInt16LittleEndian(Data[(COUNT * 2)..], (ushort)value);
    }

    private int[] LoadPointerData()
    {
        var span = Data;
        var list = new int[COUNT];
        for (int i = 0; i < list.Length; i++)
            list[i] = ReadUInt16LittleEndian(span[(i * 2)..]);
        return list;
    }

    private void SetPointerData(ReadOnlySpan<int> vals)
    {
        var span = Data;
        for (int i = 0; i < vals.Length; i++)
            WriteUInt16LittleEndian(span[(i*2)..], (ushort)vals[i]);
        vals.CopyTo(PokeListInfo);
    }

    public int GetPartyOffset(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, 6);
        int position = PokeListInfo[slot];
        return SAV.GetBoxSlotOffset(position);
    }

    private int GetPartyIndex(int slotIndex)
    {
        if ((uint) slotIndex >= MAX_SLOTS)
            return MAX_SLOTS;
        var index = Array.IndexOf(PokeListInfo, slotIndex);
        return index >= 0 ? index : MAX_SLOTS;
    }

    public bool IsParty(int slotIndex) => GetPartyIndex(slotIndex) != MAX_SLOTS;

    public bool CompressStorage()
    {
        // Box Data is stored as a list, instead of an array. Empty interstitial values are not legal.
        // Fix stored slots!
        var arr = PokeListInfo.AsSpan(0, 7);
        var result = SAV.CompressStorage(out int count, arr);
        Debug.Assert(count <= MAX_SLOTS);
        Count = count;
        if (StarterIndex > count && StarterIndex != SLOT_EMPTY)
        {
            // uh oh, we lost the starter! might have been moved out of its proper slot incorrectly.
            var species = SAV.Version == GameVersion.GP ? 25 : 133;
            bool IsStarter(PKM pk) => pk.Species == species && pk.Form != 0;
            var index = SAV.FindSlotIndex(IsStarter, count);
            if (index >= 0)
                arr[6] = index;
        }

        SetPointerData(PokeListInfo);
        return result;
    }
}
