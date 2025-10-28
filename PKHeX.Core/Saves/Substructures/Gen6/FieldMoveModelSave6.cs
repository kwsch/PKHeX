using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FieldMoveModelSave6(SAV6 sav, Memory<byte> raw) : SaveBlock<SAV6>(sav, raw)
{
    private const int SIZE = 0x108;
    private const int PLAYER_MAGIC = 0x43;
    private const int OFS_MODEL = 0x8;

    private int GetPlayerModelOffset()
    {
        for (int i = 0; i < Data.Length; i += SIZE)
        {
            if (Data[i] == PLAYER_MAGIC)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Model used for the player when the save is loaded.
    /// </summary>
    /// <remarks>
    /// 1: Serena, 2: Calem, 3: Shauna, 4: Tierno, 5: Trevor, ..., 171: May, 172: Brendan
    /// </remarks>
    public int PlayerModel
    {
        get
        {
            var ofs = GetPlayerModelOffset();
            if (ofs == -1)
                return -1;
            return ReadUInt16LittleEndian(Data[(ofs + OFS_MODEL)..]);
        }

        set
        {
            var ofs = GetPlayerModelOffset();
            if (ofs == -1)
                return;
            WriteUInt16LittleEndian(Data[(ofs + OFS_MODEL)..], (ushort)value);
        }
    }

    /// <summary>
    /// Resets the player's model to the expected value based on version and gender.
    /// </summary>
    public void ResetPlayerModel() => PlayerModel = (SAV.Gender == 0 ? 2 : 1) + (SAV is SAV6AO or SAV6AODemo ? 170 : 0);
}
