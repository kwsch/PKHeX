using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.ExtractData
{
    abstract class GenerationData
    {
        internal abstract int Generation { get; }
        internal abstract int MaxSpeciesGeneration { get; }
        internal abstract int MaxSpeciesIndexGeneration { get; }
        internal abstract int MaxMovesGeneration { get; }
        internal virtual Learnset[] TransformDexOrder(Learnset[] SourceData)
        {
            return SourceData;
        }

        internal virtual byte[][] TransformDexOrder(byte[][] SourceData)
        {
            return SourceData;
        }
        internal virtual EggMoves[] TransformDexOrder(EggMoves[] SourceData)
        {
            return SourceData;
        }

    }
}
