using System;
using PKHeX.Core;
using PKHeX.Infrastructure.LiveHex;

namespace PKHeX.Avalonia.Tests.LiveHex;

/// <summary>
/// Round-trip tests for <see cref="LiveHexBoxAddressing"/> across all three console-RAM addressing
/// modes, using the in-memory <see cref="FakeSwitchMemory"/> console.
/// </summary>
public class LiveHexBoxTransferTests
{
    private static byte[] Pattern(int length)
    {
        var data = new byte[length];
        for (int i = 0; i < length; i++)
            data[i] = (byte)((i * 7) + 3);
        return data;
    }

    [Fact]
    public void HeapFlat_SwSh_write_then_read_roundtrips_at_expected_address()
    {
        var sav = new SAV8SWSH();
        var profile = LiveHexGameProfiles.Resolve(sav, "0100ABF008968000", "1.3.2");
        Assert.NotNull(profile);
        Assert.Equal(BoxAddressingMode.HeapFlat, profile!.Mode);

        int slotSize = sav.SIZE_BOXSLOT, slotCount = sav.BoxSlotCount;
        int boxLen = slotSize * slotCount;
        var data = Pattern(boxLen);
        const int box = 2;

        var mem = new FakeSwitchMemory();
        LiveHexBoxAddressing.WriteBox(mem, profile, box, data, slotSize, slotCount);

        // Data must land at the flat heap offset for that box.
        ulong expected = profile.HeapBoxStart + (ulong)(box * boxLen);
        Assert.Equal(data, mem.GetHeapBytes(expected, boxLen));

        var read = LiveHexBoxAddressing.ReadBox(mem, profile, box, slotSize, slotCount);
        Assert.Equal(data, read);
    }

    [Fact]
    public void PointerContiguous_SV_write_then_read_roundtrips()
    {
        var sav = new SAV9SV();
        var profile = LiveHexGameProfiles.Resolve(sav, "0100A3D008C5C000", "4.0.0");
        Assert.NotNull(profile);
        Assert.Equal(BoxAddressingMode.PointerContiguous, profile!.Mode);

        int slotSize = sav.SIZE_BOXSLOT, slotCount = sav.BoxSlotCount;
        int boxLen = slotSize * slotCount;

        // Install the pointer chain for "[[[[main+47350d8]+1C0]+30]+9D0]".
        var mem = new FakeSwitchMemory { HeapBase = 0x80000 };
        mem.SetMainPointer(0x47350d8, 0x100000);
        mem.SetAbsolutePointer(0x100000 + 0x1C0, 0x200000);
        mem.SetAbsolutePointer(0x200000 + 0x30, 0x300000);
        mem.SetAbsolutePointer(0x300000 + 0x9D0, 0x400000);
        ulong boxBase = 0x400000 - 0x80000; // heap-relative base of Box 1 Slot 1

        var data = Pattern(boxLen);
        const int box = 0;
        LiveHexBoxAddressing.WriteBox(mem, profile, box, data, slotSize, slotCount);
        Assert.Equal(data, mem.GetHeapBytes(boxBase, boxLen));

        var read = LiveHexBoxAddressing.ReadBox(mem, profile, box, slotSize, slotCount);
        Assert.Equal(data, read);
    }

    [Fact]
    public void PointerScatter_BDSP_write_then_read_roundtrips_per_mon()
    {
        var sav = new SAV8BS();
        var profile = LiveHexGameProfiles.Resolve(sav, "0100000011D90000", "1.3.0"); // Brilliant Diamond
        Assert.NotNull(profile);
        Assert.Equal(BoxAddressingMode.PointerScatter, profile!.Mode);

        int slotSize = sav.SIZE_BOXSLOT, slotCount = sav.BoxSlotCount;

        // Install the box-list pointer chain "[[[[main+4C64DC0]+B8]+10]+A0]+20".
        var mem = new FakeSwitchMemory { HeapBase = 0x80000 };
        mem.SetMainPointer(0x4C64DC0, 0x1000000);
        mem.SetAbsolutePointer(0x1000000 + 0xB8, 0x2000000);
        mem.SetAbsolutePointer(0x2000000 + 0x10, 0x3000000);
        mem.SetAbsolutePointer(0x3000000 + 0xA0, 0x4000000);
        ulong listAddr = (0x4000000 + 0x20) - 0x80000; // heap-relative box-list table

        const int box = 0;
        // table[box] -> box object pointer (absolute); mon pointers live at boxObj + 0x20.
        // Low byte must be non-zero: a zero first byte is treated as "box list not loaded".
        ulong boxObjRaw = 0x5000010;
        var table = new byte[profile.ScatterPointerCount * 8];
        System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(table.AsSpan(box * 8), boxObjRaw);
        mem.SetHeapBytes(listAddr, table);

        // Per-mon absolute pointers.
        var monTable = new byte[slotCount * 8];
        var monPtrs = new ulong[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            monPtrs[i] = 0x6000000UL + (ulong)(i * 0x1000);
            System.Buffers.Binary.BinaryPrimitives.WriteUInt64LittleEndian(monTable.AsSpan(i * 8), monPtrs[i]);
        }
        mem.SetAbsoluteBytes(boxObjRaw + 0x20, monTable);

        var data = Pattern(slotSize * slotCount);
        LiveHexBoxAddressing.WriteBox(mem, profile, box, data, slotSize, slotCount);

        // Each mon's bytes must land at its own pointer + 0x20.
        for (int i = 0; i < slotCount; i++)
        {
            var expected = new byte[slotSize];
            Array.Copy(data, i * slotSize, expected, 0, slotSize);
            Assert.Equal(expected, mem.GetAbsoluteBytes(monPtrs[i] + 0x20, slotSize));
        }

        var read = LiveHexBoxAddressing.ReadBox(mem, profile, box, slotSize, slotCount);
        Assert.Equal(data, read);
    }

    [Fact]
    public void Save_box_binary_bridges_console_bytes_into_the_save()
    {
        // The console box byte layout equals SaveFile.GetBoxBinary/SetBoxBinary for supported games.
        var sav = new SAV8SWSH();
        int box = 0;
        var original = sav.GetBoxBinary(box);
        Assert.True(sav.SetBoxBinary(original, box)); // idempotent round-trip through Core
        Assert.Equal(original.Length, sav.SIZE_BOXSLOT * sav.BoxSlotCount);
    }
}
