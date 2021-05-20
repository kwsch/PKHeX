using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class BV3 : BattleVideo
    {
        internal const int SIZE = 0xF80;
        public override int Generation => 3;

        public override IReadOnlyList<PKM> BattlePKMs => PlayerTeams.SelectMany(z => z).ToArray();

        public readonly byte[] Data;

        internal new static bool IsValid(byte[] data)
        {
            if (data.Length != SIZE)
                return false;
            var chk = BitConverter.ToUInt32(data, SIZE - 4);
            if (chk > 0xF7080)
                return false; // max if all are FF
            var expect = GetChecksum8(data);
            return chk == expect;
        }

        public BV3(byte[] data) => Data = (byte[])data.Clone();
        public BV3() : this(new byte[SIZE]) { }

        public IReadOnlyList<PK3[]> PlayerTeams
        {
            get => new[]
            {
                GetTeam(0, 0),
                GetTeam(0, 6 * PokeCrypto.SIZE_3PARTY),
            };
            set
            {
                SetTeam(value[0], 0);
                SetTeam(value[1], 6 * PokeCrypto.SIZE_3PARTY);
            }
        }

        public PK3[] GetTeam(int teamIndex, int ofs)
        {
            var team = new PK3[6];
            for (int p = 0; p < 6; p++)
            {
                int offset = ofs + (PokeCrypto.SIZE_3PARTY * p);
                team[p] = new PK3(Data.Slice(offset, PokeCrypto.SIZE_3PARTY)) { Identifier = $"Team {teamIndex}, Slot {p}" };
            }

            return team;
        }

        public void SetTeam(IReadOnlyList<PK3> team, int ofs)
        {
            for (int p = 0; p < 6; p++)
            {
                int offset = ofs + (PokeCrypto.SIZE_3PARTY * p);
                team[p].EncryptedPartyData.CopyTo(Data, offset);
            }
        }

        // 0x4B0 - string3[4][8] Trainer Names
        // 0x4D0 - u8[4] Trainer Genders
        // 0x4D4 - u32[4] Trainer IDs
        // 0x4E4 - u8[4] Trainer Languages

        public uint Seed
        {
            get => BitConverter.ToUInt32(Data, 0x4E8);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x4E8);
        }

        public uint Mode
        {
            get => BitConverter.ToUInt32(Data, 0x4EC);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x4EC);
        }

        // ...

        public uint Checksum
        {
            get => BitConverter.ToUInt32(Data, SIZE - 4);
            set => BitConverter.GetBytes(value).CopyTo(Data, SIZE - 4);
        }

        public bool IsChecksumValid() => Checksum == GetChecksum8(Data);

        public static uint GetChecksum8(byte[] data)
        {
            uint result = 0;
            for (int i = 0; i < data.Length - 4; i++)
                result += data[i];
            return result;
        }
    }
}
