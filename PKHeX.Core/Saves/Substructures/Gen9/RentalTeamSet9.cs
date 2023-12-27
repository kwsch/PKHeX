using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Save Block for Scarlet/Violet that stores a fixed amount of saved rental teams.
/// </summary>
public sealed class RentalTeamSet9(byte[] Data) : IPokeGroup
{
    public const int SIZE = Count * RentalTeam9.SIZE;
    public const int Count = 5;
    public readonly byte[] Data = Data;

    public RentalTeam9 GetRentalTeam(int index) => RentalTeam9.GetFrom(Data, index);
    public void SetRentalTeam(int index, RentalTeam9 team) => team.WriteTo(Data, index);

    public IEnumerable<PKM> Contents
    {
        get
        {
            for (int i = 0; i < Count; i++)
            {
                var team = GetRentalTeam(i);
                foreach (var pk in team.Contents)
                    yield return pk;
            }
        }
    }

    public static bool IsRentalTeamSet(ReadOnlyMemory<byte> data)
    {
        if (data.Length != SIZE)
            return false;
        for (int i = 0; i < Count; i++)
        {
            var offset = i * RentalTeam9.SIZE;
            var slice = data.Slice(offset, RentalTeam9.SIZE).ToArray();
            if (!RentalTeam9.IsRentalTeam(slice))
                return false;
        }
        return true;
    }
}
