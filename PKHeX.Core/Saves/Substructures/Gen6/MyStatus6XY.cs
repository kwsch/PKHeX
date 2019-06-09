using System;

namespace PKHeX.Core
{
    public sealed class MyStatus6XY : MyStatus6
    {
        public MyStatus6XY(SaveFile sav, int offset) : base(sav, offset) { }

        public TrainerFashion6 Fashion
        {
            get => TrainerFashion6.GetFashion(SAV.Data, Offset + 0x30, SAV.Gender);
            set => value.Write(Data, Offset + 0x30);
        }

        public string OT_Nick
        {
            get => SAV.GetString(Offset + 0x62, SAV6.ShortStringLength / 2);
            set => SAV.SetData(SAV.SetString(value, SAV6.ShortStringLength / 2), Offset + 0x62);
        }

        public short EyeColor
        {
            get => BitConverter.ToInt16(Data, Offset + 0x148);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x148);
        }
    }
}