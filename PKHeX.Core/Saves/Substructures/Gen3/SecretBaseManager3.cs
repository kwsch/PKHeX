using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public sealed class SecretBaseManager3
{
    public const int BaseCount = 20;
    private readonly bool[] Occupied = new bool[BaseCount];

    private readonly Memory<byte> Data;

    public List<SecretBase3> Bases { get; set; }
    public int Count => Bases.Count;

    public SecretBaseManager3(Memory<byte> data)
    {
        Data = data;
        Bases = new List<SecretBase3>(BaseCount);
        for (int i = 0; i < 20 ; i ++)
        {
            var slice = GetBase(i);
            var tmp = new SecretBase3(slice);
            if (tmp.IsTrainerPresent)
            {
                Occupied[i] = true;
                Bases.Add(tmp);
            }
            else
            {
                Occupied[i] = false;
            }
        }
    }

    private Memory<byte> GetBase(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, BaseCount, nameof(index));
        return Data.Slice(index * SecretBase3.SIZE, SecretBase3.SIZE);
    }

    public void Save() => WriteTo(Data.Span);

    private void WriteTo(Span<byte> data)
    {
        var tmp = Bases.ToList();
        for (int i = 0; i < BaseCount; i++)
        {
            if (!Occupied[i])
            {
                // Initialize: Wipe trainer name of each slot
                data.Slice((i * SecretBase3.SIZE) + 2, 7).Fill(StringConverter3.TerminatorByte);
                continue;
            }
            var tmpbase = tmp[0];
            tmpbase.Data.CopyTo(data[(i * SecretBase3.SIZE)..]);
            tmp.RemoveAt(0);
        }
    }
}
