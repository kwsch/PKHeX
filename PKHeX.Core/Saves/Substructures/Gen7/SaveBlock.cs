using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Base class for a savegame data reader.
    /// </summary>
    public abstract class SaveBlock
    {
        [Browsable(false)]
        public int Offset { get; protected set; }

        public readonly byte[] Data;
        protected readonly SaveFile SAV;
        protected SaveBlock(SaveFile sav) => Data = (SAV = sav).Data;

        protected SaveBlock(SaveFile sav, byte[] data)
        {
            SAV = sav;
            Data = data;
        }
    }
}