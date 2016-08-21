using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.ExtractData
{
    class Generation3Data : GenerationData
    {
        internal override int Generation => 3;
        private const int MaxMovesGeneration3 = 354;
        private const int MaxSpeciesGeneration3 = 386;
        private const int MaxSpeciesIndexGeneration3 = 411; 
        internal override int MaxSpeciesGeneration => MaxSpeciesGeneration3;
        internal override int MaxSpeciesIndexGeneration => MaxSpeciesIndexGeneration3;
        internal override int MaxMovesGeneration => MaxMovesGeneration3;

        private static Generation3Data G3 = null;

        protected Generation3Data()
        {

        }

        public static Generation3Data Create()
        {
            if (G3 == null)
                G3 = new Generation3Data();
            return G3;
        }


        internal override Learnset[] TransformDexOrder(Learnset[] SourceData)
        {
            Learnset[] Output = new Learnset[MaxSpeciesGeneration + 1];

            Output[0] = new Learnset(new byte[0]);
            for (int i = 1; i < SourceData.Length; i++)
                Output[TransformSpeciesIndex.getG4Species(i)] = SourceData[i];

            return Output;
        }

        internal override byte[][] TransformDexOrder(byte[][] SourceData)
        {
            byte[][] Output = new byte[MaxSpeciesGeneration + 1][];

            Output[0] = new byte[0];
            for (int i = 1; i < SourceData.Length; i++)
                Output[TransformSpeciesIndex.getG4Species(i)] = SourceData[i];

            return Output;
        }
        internal override EggMoves[] TransformDexOrder(EggMoves[] SourceData)
        {
            EggMoves[] Output = new EggMoves[MaxSpeciesGeneration + 1];

            Output[0] = new EggMoves(new byte[0]);
            for (int i = 1; i < SourceData.Length; i++)
                Output[TransformSpeciesIndex.getG4Species(i)] = SourceData[i];

            return Output;
        }

    }
}
