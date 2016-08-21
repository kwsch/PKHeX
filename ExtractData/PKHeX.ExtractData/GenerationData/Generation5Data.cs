using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.ExtractData
{
    class Generation5Data : GenerationData
    {
        private static Generation5Data G5 = null;

        internal override int Generation => 5;
        private const int MaxMovesGeneration5 = 559;
        private const int MaxSpeciesGeneration5 = 649;
        private const int MaxSpeciesIndexGeneration5 = 709; 
        internal override int MaxSpeciesGeneration => MaxSpeciesGeneration5;
        internal override int MaxSpeciesIndexGeneration => MaxSpeciesIndexGeneration5;
        internal override int MaxMovesGeneration => MaxMovesGeneration5;

        private Generation5Data()
        {

        }

        internal static Generation5Data Create()
        {
            if (G5 == null)
                G5 = new Generation5Data();
            return G5;
        }
    }
}
