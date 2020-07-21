using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Secret base format for <see cref="GameVersion.ORAS"/>
    /// </summary>
    public sealed class SecretBase6
    {
        private readonly byte[] Data;
        private readonly int Offset;
        public const int SIZE = 0x3E0;

        public int BaseLocation
        {
            get => BitConverter.ToInt16(Data, Offset);
            set => BitConverter.GetBytes((short)value).CopyTo(Data, Offset);
        }

        public SecretBase6(byte[] data, int offset = 0)
        {
            Data = data;
            Offset = offset;
        }

        public string TrainerName
        {
            get => StringConverter.GetString6(Data, Offset + 0x218, 0x1A);
            set => StringConverter.SetString6(TrainerName, 0x1A / 2).CopyTo(Data, Offset + 0x218);
        }

        public string FlavorText1
        {
            get => StringConverter.GetString6(Data, Offset + 0x232 + (0x22 * 0), 0x22);
            set => StringConverter.SetString6(value, 0x22 / 2).CopyTo(Data, Offset + 0x232 + (0x22 * 0));
        }

        public string FlavorText2
        {
            get => StringConverter.GetString6(Data, Offset + 0x232 + (0x22 * 1), 0x22);
            set => StringConverter.SetString6(value, 0x22 / 2).CopyTo(Data, Offset + 0x232 + (0x22 * 1));
        }

        public string Saying1
        {
            get => StringConverter.GetString6(Data, Offset + 0x276 + (0x22 * 0), 0x22);
            set => StringConverter.SetString6(value, 0x22 / 2).CopyTo(Data, Offset + 0x276 + (0x22 * 0));
        }

        public string Saying2
        {
            get => StringConverter.GetString6(Data, Offset + 0x276 + (0x22 * 1), 0x22);
            set => StringConverter.SetString6(value, 0x22 / 2).CopyTo(Data, Offset + 0x276 + (0x22 * 1));
        }

        public string Saying3
        {
            get => StringConverter.GetString6(Data, Offset + 0x276 + (0x22 * 2), 0x22);
            set => StringConverter.SetString6(value, 0x22 / 2).CopyTo(Data, Offset + 0x276 + (0x22 * 2));
        }

        public string Saying4
        {
            get => StringConverter.GetString6(Data, Offset + 0x276 + (0x22 * 3), 0x22);
            set => StringConverter.SetString6(value, 0x22 / 2).CopyTo(Data, Offset + 0x276 + (0x22 * 3));
        }

        public bool IsDummiedBaseLocation => !IsEmpty && BaseLocation < 3;
        public bool IsEmpty => BaseLocation == 0;
    }
}
