using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.ExtractData
{
    class Generation4Data : GenerationData
    {
        private static Generation4Data G4 = null;

        internal override int Generation => 4;
        private const int MaxMovesGeneration4 = 467;
        private const int MaxSpeciesGeneration4 = 493;
        private const int MaxSpeciesIndexGeneration4 = 508; //PersonalTable.Pt.Length;
        internal override int MaxSpeciesGeneration => MaxSpeciesGeneration4;
        internal override int MaxSpeciesIndexGeneration => MaxSpeciesIndexGeneration4;
        internal override int MaxMovesGeneration => MaxMovesGeneration4;

        private Generation4Data()
        {

        }

        internal static Generation4Data Create()
        {
            if (G4 == null)
                G4 = new Generation4Data();
            return G4;
        }
    }
}
