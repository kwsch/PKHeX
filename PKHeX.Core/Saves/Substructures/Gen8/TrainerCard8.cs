using System;
using System.Text;

namespace PKHeX.Core
{
    public sealed class TrainerCard8 : SaveBlock
    {
        public TrainerCard8(SAV8SWSH sav, SCBlock block) : base (sav, block.Data) { }

        public string OT
        {
            get => SAV.GetString(Data, 0x00, 0x1A);
            set => SAV.SetData(Data, SAV.SetString(value, SAV.OTLength), 0x00);
        }

        public int TrainerID
        {
            get => BitConverter.ToInt32(Data, 0x1C);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), 0x1C);
        }

        public const int RotoRallyScoreMax = 99_999;

        public int RotoRallyScore
        {
            get => BitConverter.ToInt32(Data, 0x28);
            set
            {
                if (value > RotoRallyScoreMax)
                    value = RotoRallyScoreMax;
                var data = BitConverter.GetBytes(value);
                SAV.SetData(Data, data, 0x28);
                // set to the other block since it doesn't have an accessor
                ((SAV8SWSH)SAV).SetValue(SaveBlockAccessorSWSH.KRotoRally, value);
            }
        }

        public string Number
        {
            get => Encoding.ASCII.GetString(Data, 0x39, 3);
            set
            {
                for (int i = 0; i < 3; i++)
                    Data[0x39 + i] = (byte) (value.Length > i ? value[i] : '\0');
                SAV.Edited = true;
            }
        }
    }
}