using System;

namespace PKHeX.Core
{
    /// <summary>
    /// XY specific features for <see cref="MyStatus6"/>
    /// </summary>
    /// <remarks>These properties are technically included in OR/AS but they are unused; assumed backwards compatibility for communications with XY</remarks>
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
            get => SAV.GetString(Offset + 0x62, SAV6.ShortStringLength);
            set => SAV.SetData(SAV.SetString(value, 12), Offset + 0x62);
        }

        public short EyeColor
        {
            get => BitConverter.ToInt16(Data, Offset + 0x148);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x148);
        }
    }
}