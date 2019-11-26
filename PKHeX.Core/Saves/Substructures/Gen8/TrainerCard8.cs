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

        public string Number
        {
            get => Encoding.ASCII.GetString(Data, 0x39, 3);
            set
            {
                var str = value.Length == 3 ? value : value.Length < 3 ? value.PadLeft(3, '0') : value.Substring(0, 3);
                var bytes = Encoding.ASCII.GetBytes(str.ToCharArray(), 0, 3);
                SAV.SetData(Data, bytes, 0x39);
            }
        }
    }
}