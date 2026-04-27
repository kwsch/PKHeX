using PKHeX.Core;
using Xunit;

namespace PKHeX.Core.Tests.Saves.Gen3;

public class MirageIslandTests
{
    [Fact]
    public void SaveBlock3LargeE_MirageIslandValue_ReadWrite()
    {
        var block = new SaveBlock3LargeE(new byte[0x3D88]);

        block.MirageIslandValue = 0xAA80;

        Assert.Equal((ushort)0xAA80, block.MirageIslandValue);
    }

    [Fact]
    public void SaveBlock3LargeRS_MirageIslandValue_ReadWrite()
    {
        var block = new SaveBlock3LargeRS(new byte[0x3B4C]);

        block.MirageIslandValue = 0xAA80;

        Assert.Equal((ushort)0xAA80, block.MirageIslandValue);
    }

    [Fact]
    public void SAV3E_TrySetMirageIslandFromFirstPartySlot_ReturnsFalse_WhenPartyIsEmpty()
    {
        var sav = new SAV3E();

        sav.LargeBlock.PartyCount = 0;

        Assert.False(sav.TrySetMirageIslandFromFirstPartySlot());
    }

    [Fact]
    public void SAV3RS_TrySetMirageIslandFromFirstPartySlot_ReturnsFalse_WhenPartyIsEmpty()
    {
        var sav = new SAV3RS();

        sav.LargeBlock.PartyCount = 0;

        Assert.False(sav.TrySetMirageIslandFromFirstPartySlot());
    }

    [Fact]
    public void SAV3E_TrySetMirageIslandFromFirstPartySlot_SetsLow16BitsOfPID()
    {
        var sav = new SAV3E();
        var pk = CreateTestPK3(0x4603AA80);

        SetFirstPartySlot(sav, pk);

        var result = sav.TrySetMirageIslandFromFirstPartySlot();

        Assert.True(result);
        Assert.Equal((ushort)0xAA80, sav.MirageIslandValue);
    }

    [Fact]
    public void SAV3RS_TrySetMirageIslandFromFirstPartySlot_SetsLow16BitsOfPID()
    {
        var sav = new SAV3RS();
        var pk = CreateTestPK3(0x4603AA80);

        SetFirstPartySlot(sav, pk);

        var result = sav.TrySetMirageIslandFromFirstPartySlot();

        Assert.True(result);
        Assert.Equal((ushort)0xAA80, sav.MirageIslandValue);
    }

    private static PK3 CreateTestPK3(uint pid)
    {
        var pk = new PK3
        {
            Species = (ushort)Species.Treecko,
            PID = pid,
        };

        pk.RefreshChecksum();
        return pk;
    }

    private static void SetFirstPartySlot(SAV3 sav, PK3 pk)
    {
        sav.LargeBlock.PartyCount = 1;
        pk.Data.CopyTo(sav.LargeBlock.PartyBuffer[..sav.SIZE_PARTY]);
    }
}
