using System;

namespace PKHeX.Core
{
    public sealed class SecretBase3Team
    {
        private const int O_PID = 0;
        private const int O_Moves = 0x18;
        private const int O_Species = 0x24;
        private const int O_Item = 0x30;
        private const int O_Level = 0x3C;
        private const int O_EV = 0x42;

        private static int GetOffsetPID(int i) => O_PID + (i * 4);
        private static int GetOffsetMove(int i, int move) => O_Moves + (i * 8) + (move * 2);
        private static int GetOffsetSpecies(int i) => O_Species + (i * 2);
        private static int GetOffsetItem(int i) => O_Item + (i * 2);

        public readonly SecretBase3PKM[] Team;
        private readonly byte[] Data;

        public SecretBase3Team(byte[] data)
        {
            Team = new SecretBase3PKM[6];
            for (int i = 0; i < Team.Length; i++)
                Team[i] = GetPKM(i);
            Data = data;
        }

        public byte[] Write()
        {
            for (int i = 0; i < Team.Length; i++)
                SetPKM(i);
            return Data;
        }

        private SecretBase3PKM GetPKM(int i)
        {
            return new()
            {
                PID = BitConverter.ToUInt32(Data, GetOffsetPID(i)),
                Species = BitConverter.ToUInt16(Data, GetOffsetSpecies(i)),
                HeldItem = BitConverter.ToUInt16(Data, GetOffsetItem(i)),
                Move1 = BitConverter.ToUInt16(Data, GetOffsetMove(i, 0)),
                Move2 = BitConverter.ToUInt16(Data, GetOffsetMove(i, 1)),
                Move3 = BitConverter.ToUInt16(Data, GetOffsetMove(i, 2)),
                Move4 = BitConverter.ToUInt16(Data, GetOffsetMove(i, 3)),
                Level = Data[O_Level + i],
                EVAll = Data[O_EV + i],
            };
        }

        private void SetPKM(int i)
        {
            var pk = Team[i];
            BitConverter.GetBytes(pk.PID).CopyTo(Data, GetOffsetPID(i));
            BitConverter.GetBytes((ushort)pk.Species).CopyTo(Data, GetOffsetSpecies(i));
            BitConverter.GetBytes((ushort)pk.HeldItem).CopyTo(Data, GetOffsetItem(i));
            BitConverter.GetBytes((ushort)pk.Move1).CopyTo(Data, GetOffsetMove(i, 0));
            BitConverter.GetBytes((ushort)pk.Move2).CopyTo(Data, GetOffsetMove(i, 1));
            BitConverter.GetBytes((ushort)pk.Move3).CopyTo(Data, GetOffsetMove(i, 2));
            BitConverter.GetBytes((ushort)pk.Move4).CopyTo(Data, GetOffsetMove(i, 3));
            Data[O_Level + i] = (byte) pk.Level;
            Data[O_EV + i] = (byte) pk.EVAll;
        }
    }
}