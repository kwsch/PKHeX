using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class EggMoves
{
    public readonly int[] Moves;
    protected EggMoves(int[] moves) => Moves = moves;
    public bool GetHasEggMove(int move) => Array.IndexOf(Moves, move) != -1;
}

public sealed class EggMoves2 : EggMoves
{
    private EggMoves2(int[] moves) : base(moves) { }

    public static EggMoves2[] GetArray(ReadOnlySpan<byte> data, int count)
    {
        var entries = new EggMoves2[count + 1];
        var empty = entries[0] = new EggMoves2(Array.Empty<int>());

        int baseOffset = ReadInt16LittleEndian(data) - (count * 2);
        for (int i = 1; i < entries.Length; i++)
        {
            int start = ReadInt16LittleEndian(data[((i - 1) * 2)..]) - baseOffset;
            var slice = data[start..];
            int moveCount = slice.IndexOf((byte)0xFF);
            if (moveCount == 0)
            {
                entries[i] = empty;
                continue;
            }

            int[] moves = new int[moveCount];
            for (int m = 0; m < moves.Length; m++)
                moves[m] = slice[m];

            entries[i] = new EggMoves2(moves);
        }

        return entries;
    }
}

public sealed class EggMoves6 : EggMoves
{
    private EggMoves6(int[] moves) : base(moves) { }

    public static EggMoves6[] GetArray(BinLinkerAccessor entries)
    {
        var result = new EggMoves6[entries.Length];
        var empty = result[0] = new EggMoves6(Array.Empty<int>());
        for (int i = 1; i < result.Length; i++)
        {
            var data = entries[i];
            int count = ReadInt16LittleEndian(data);
            if (count == 0)
            {
                result[i] = empty;
                continue;
            }

            var moves = new int[count];
            var span = data[2..];
            for (int j = 0; j < moves.Length; j++)
                moves[j] = ReadInt16LittleEndian(span[(j * 2)..]);
            result[i] = new EggMoves6(moves);
        }
        return result;
    }
}

public sealed class EggMoves7 : EggMoves
{
    public readonly int FormTableIndex;

    private EggMoves7(int[] moves, int formIndex = 0) : base(moves) => FormTableIndex = formIndex;

    public static EggMoves7[] GetArray(BinLinkerAccessor entries)
    {
        var result = new EggMoves7[entries.Length];
        var empty = result[0] = new EggMoves7(Array.Empty<int>());
        for (int i = 1; i < result.Length; i++)
        {
            var data = entries[i];
            int count = ReadInt16LittleEndian(data[2..]);
            int formIndex = ReadInt16LittleEndian(data);
            if (count == 0)
            {
                // Might need to keep track of the Form Index for unavailable forms pointing to valid forms.
                if (formIndex != 0)
                    result[i] = new EggMoves7(Array.Empty<int>(), formIndex);
                else
                    result[i] = empty;
                continue;
            }

            var moves = new int[count];
            var span = data[4..];
            for (int j = 0; j < moves.Length; j++)
                moves[j] = ReadInt16LittleEndian(span[(j * 2)..]);
            result[i] = new EggMoves7(moves, formIndex);
        }
        return result;
    }
}
