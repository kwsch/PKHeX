using System;
using System.Linq;
using System.IO;

namespace PKHeX.ExtractData
{
    class ExtLearnSet5 : ExtLearnSet4
    {
        internal override GenerationData GData => Generation5Data.Create();

        const string B1_Folder = "\\Black\\root\\a\\0\\1\\";
        const string W1_Folder = "\\White\\root\\a\\0\\1\\"; //8
        const string B2_Folder = "\\Black 2\\root\\a\\0\\1\\";
        const string W2_Folder = "\\White 2\\root\\a\\0\\1\\"; //8
        const int B1W1_MaxSpeciesFormsGenartion5 = 668;
        const int B2W2_MaxSpeciesFormsGenartion5 = 709;

        public override void ExtractLevelUpMovesData(string log_folder, string source_folder, string out_folder)
        {
            byte[][] B1_Source_Data = ReadNarcNDS("8", source_folder + B1_Folder, B1W1_MaxSpeciesFormsGenartion5, false);
            byte[][] W1_Source_Data = ReadNarcNDS("8", source_folder + W1_Folder, B1W1_MaxSpeciesFormsGenartion5, false);
            byte[][] B2_Source_Data = ReadNarcNDS("8", source_folder + B2_Folder, B2W2_MaxSpeciesFormsGenartion5, false);
            byte[][] W2_Source_Data = ReadNarcNDS("8", source_folder + W2_Folder, B2W2_MaxSpeciesFormsGenartion5, false);
            /*Black and White data is equal*/
            /*Black 2 and White 2 data is equal*/
            bool B1_W1_Equal = ExtractUtils.CheckArrayEqual(B1_Source_Data, W1_Source_Data);
            bool B2_W2_Equal = ExtractUtils.CheckArrayEqual(B2_Source_Data, W2_Source_Data);
            /*The data is different betwen black 1 and white 1 with black 2 and white 2*/
            bool B1_B2_Equal = ExtractUtils.CheckArrayEqual(B1_Source_Data, B2_Source_Data.Take(668).ToArray());
            bool W1_W2_Equal = ExtractUtils.CheckArrayEqual(W1_Source_Data, W2_Source_Data.Take(668).ToArray());

            //Trabsform Raw data into PKHex format
            Learnset[] B1W1_Output_Data = TransformLearnset(B1_Source_Data);
            Learnset[] B2W2_Output_Data = TransformLearnset(B2_Source_Data);

            TraceLearnSet(log_folder, "learnset_bw.log"  , B1W1_Output_Data, true);
            TraceLearnSet(log_folder, "learnset_b2w2.log", B2W2_Output_Data, false);

            //Extract LearnSet to file
            byte[] B1W1_FileData = PackLearnSetData("51", B1W1_Output_Data);
            byte[] B2W2_FileData = PackLearnSetData("52", B2W2_Output_Data);

            Directory.CreateDirectory(out_folder);
            File.WriteAllBytes(out_folder + "lvlmove_bw.pkl", B1W1_FileData);
            File.WriteAllBytes(out_folder + "lvlmove_b2w2.pkl", B2W2_FileData);

            Learnset[] B1W1_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_bw.pkl"), "51"));
            Learnset[] B2W2_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_b2w2.pkl"), "52"));

            //Check that the fileS that has been writen and then readed are equal than the generated ones
            string diff_b1w1 = string.Empty;
            string diff_b2w2 = string.Empty;
            bool check_b1w1 = CheckEquals(B1W1_Output_Data, B1W1_Check_Data, out diff_b1w1);
            if (!check_b1w1)
                throw new Exception("Check Black White LearnSet." + diff_b1w1);
            bool check_b2w2 = CheckEquals(B2W2_Output_Data, B2W2_Check_Data, out diff_b2w2);
            if (!check_b2w2)
                throw new Exception("Check Black 2 White 2 LearnSet." + diff_b2w2);
        }
        
        /// <summary>
        /// Transform raw level up move data from gen5 game to PKHex format
        /// </summary>
        /// <param name="species"> max species in generation</param>
        /// <param name="Input">raw level up move data </param>
        /// <returns>Learnset data in PKHex format</returns>
        internal override Learnset[] TransformLearnset(byte[][] Input)
        {
            //Gen 5 is in the same format used by PKHex
            return Learnset.getArray(Input);
        }
    }
}
