using System;

namespace PKHeX.Core
{
    public abstract class Encount5 : SaveBlock
    {
        protected Encount5(SAV5 SAV, int offset) : base(SAV) => Offset = offset;

        public abstract uint SwarmSeed { get; set; }
        public abstract uint SwarmMaxCountModulo { get; }

        public uint SwarmIndex
        {
            get => SwarmSeed % SwarmMaxCountModulo;
            set
            {
                value %= SwarmMaxCountModulo;
                while (SwarmIndex != value)
                    ++SwarmSeed;
            }
        }
    }

    public sealed class Encount5BW : Encount5
    {
        public Encount5BW(SAV5BW SAV, int offset) : base(SAV, offset) { }

        public override uint SwarmSeed { get => BitConverter.ToUInt32(Data, Offset + 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x30); }
        public override uint SwarmMaxCountModulo => 17;
    }

    public sealed class Encount5B2W2 : Encount5
    {
        public Encount5B2W2(SAV5B2W2 SAV, int offset) : base(SAV, offset) { }

        public override uint SwarmSeed { get => BitConverter.ToUInt32(Data, Offset + 0x2C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x2C); }
        public override uint SwarmMaxCountModulo => 19;
    }
}
