using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Container block for Generation 8 saved Rental Teams
    /// </summary>
    public sealed class RentalTeam8 : IRentalTeam<PK8>, IPokeGroup
    {
        public const int SIZE = 0x880;

        private const int LEN_META = 0x56;
        private const int LEN_STORED = PokeCrypto.SIZE_8STORED; // 0x148
        private const int LEN_POKE = PokeCrypto.SIZE_8PARTY; // 0x158
        private const int LEN_PARTYSTAT = LEN_POKE - PokeCrypto.SIZE_8STORED; // 0x10
        private const int COUNT_POKE = 6;

        private const int OFS_META = 0;
        private const int OFS_1 = OFS_META + LEN_META;
        private const int OFS_2 = OFS_1 + LEN_POKE;
        private const int OFS_3 = OFS_2 + LEN_POKE;
        private const int OFS_4 = OFS_3 + LEN_POKE;
        private const int OFS_5 = OFS_4 + LEN_POKE;
        private const int OFS_6 = OFS_5 + LEN_POKE;
        private const int POST_META = OFS_6 + LEN_POKE;

        private readonly byte[] Data;

        public RentalTeam8(byte[] data) => Data = data;

        public PK8 GetSlot(int slot)
        {
            var ofs = GetSlotOffset(slot);
            var data = Data.Slice(ofs, LEN_POKE);
            var pk8 = new PK8(data);
            pk8.ResetPartyStats();
            return pk8;
        }

        public void SetSlot(int slot, PK8 pkm)
        {
            var ofs = GetSlotOffset(slot);
            var data = pkm.EncryptedPartyData;
            // Wipe Party Stats
            Array.Clear(data, LEN_STORED, LEN_PARTYSTAT);
            data.CopyTo(Data, ofs);
        }

        public PK8[] GetTeam()
        {
            var team = new PK8[COUNT_POKE];
            for (int i = 0; i < team.Length; i++)
                team[i] = GetSlot(i);
            return team;
        }

        public void SetTeam(IReadOnlyList<PK8> team)
        {
            for (int i = 0; i < team.Count; i++)
                SetSlot(i, team[i]);
        }

        public static int GetSlotOffset(int slot)
        {
            if ((uint)slot >= COUNT_POKE)
                throw new ArgumentOutOfRangeException(nameof(slot));
            return OFS_1 + (LEN_POKE * slot);
        }

        public Span<byte> GetMetadataStart() => Data.AsSpan(OFS_META, LEN_META);
        public Span<byte> GetMetadataEnd() => Data.AsSpan(POST_META);

        public IEnumerable<PKM> Contents => GetTeam();

        public static bool IsRentalTeam(byte[] data)
        {
            if (data.Length != SIZE)
                return false;
            var team = new RentalTeam8(data).GetTeam();
            return team.All(x => x.ChecksumValid);
        }
    }
}
