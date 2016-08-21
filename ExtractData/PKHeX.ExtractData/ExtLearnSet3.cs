using System;
using System.IO;
using System.Text;

namespace PKHeX.ExtractData
{
    class ExtLearnSet3 : ExtLearnSet4
    {
        internal override GenerationData GData => Generation3Data.Create();

        const string gba_firered_rom = "Pokemon - FireRed Version (USA).gba";
        const string gba_leafgreen_rom = "Pokemon - LeafGreen Version (USA).gba";
        const string gba_emerald_rom = "Pokemon - Emerald Version (USA, Europe).gba";
        const string gba_ruby_rom = "Pokemon - Ruby Version (USA).gba";
        const string gba_sapphire_rom = "Pokemon - Sapphire Version (USA).gba";

        const int OffSet_FR_LevelUpData = 0x257494; //25D7B4 - 6320
        const int OffSet_LG_LevelUpData = 0x257470; //25D794
        const int OffSet_R_LevelUpData = 0x201928; //0x2018A8 + 64 * 2; //207BC8
        const int OffSet_S_LevelUpData = 0x2018B8; //0x201838 + 64 * 2; //207B58
        const int OffSet_E_LevelUpData = 0x3230DC; //0x32305C + 64*2; //32937C
        
        public override void ExtractLevelUpMovesData(string log_folder,string source_folder, string out_folder)
        {
            byte[][] FR_Source_Data = ReadGBAROM(gba_firered_rom,   source_folder, OffSet_FR_LevelUpData);
            byte[][] LG_Source_Data = ReadGBAROM(gba_leafgreen_rom, source_folder, OffSet_LG_LevelUpData);
            byte[][] R_Source_Data  = ReadGBAROM(gba_ruby_rom,      source_folder, OffSet_R_LevelUpData);
            byte[][] S_Source_Data  = ReadGBAROM(gba_sapphire_rom,  source_folder, OffSet_S_LevelUpData);
            byte[][] E_Source_Data  = ReadGBAROM(gba_emerald_rom,   source_folder, OffSet_E_LevelUpData);

            /*
            Check if data is the same. Between FR and LG the differents are Deoxys....and Dugtrio, two level 1 moves are in different order, not a real difference
            Ruby and Sapphire do not have any differences
            Emerald with Ruby and Sapphire is only different with Deoxys
            Fire Red and Leaf Green have many differences with R/S/E
             */
            bool FR_LG_Equal = ExtractUtils.CheckArrayEqual(FR_Source_Data, LG_Source_Data);
            bool R_S_Equal = ExtractUtils.CheckArrayEqual(R_Source_Data, S_Source_Data);
            bool R_E_Equal = ExtractUtils.CheckArrayEqual(R_Source_Data, E_Source_Data);

            Learnset[] R_Output_Data  = TransformLearnset(R_Source_Data);
            Learnset[] S_Output_Data  = TransformLearnset(S_Source_Data);
            Learnset[] E_Output_Data  = TransformLearnset(E_Source_Data);
            Learnset[] FR_Output_Data = TransformLearnset(FR_Source_Data);
            Learnset[] LG_Output_Data = TransformLearnset(LG_Source_Data);

            TraceLearnSetIndex(log_folder, "learnset_index_ru.log", R_Output_Data);
            TraceLearnSetIndex(log_folder, "learnset_index_sa.log", S_Output_Data);
            TraceLearnSetIndex(log_folder, "learnset_index_em.log", E_Output_Data);
            TraceLearnSetIndex(log_folder, "learnset_index_fr.log", FR_Output_Data);
            TraceLearnSetIndex(log_folder, "learnset_index_lg.log", LG_Output_Data);

            R_Output_Data  = GData.TransformDexOrder(R_Output_Data);
            S_Output_Data  = GData.TransformDexOrder(S_Output_Data);
            E_Output_Data  = GData.TransformDexOrder(E_Output_Data);
            FR_Output_Data = GData.TransformDexOrder(FR_Output_Data);
            LG_Output_Data = GData.TransformDexOrder(LG_Output_Data);

            TraceLearnSet(log_folder, "learnset_dexnum_ru.log", R_Output_Data);
            TraceLearnSet(log_folder, "learnset_dexnum_sa.log", S_Output_Data);
            TraceLearnSet(log_folder, "learnset_dexnum_em.log", E_Output_Data);
            TraceLearnSet(log_folder, "learnset_dexnum_fr.log", FR_Output_Data);
            TraceLearnSet(log_folder, "learnset_dexnum_lg.log", LG_Output_Data);

            bool FR_LG_Equal_386 = ExtractUtils.CheckArrayEqual(FR_Output_Data, LG_Output_Data, new int[] { 51, 386 });
            bool R_E_Equal_386 = ExtractUtils.CheckArrayEqual(R_Output_Data, E_Output_Data, new int[] { 386 });

            //Extract LearnSet to file
            byte[] RS_FileData = PackLearnSetData("rs", R_Output_Data);
            byte[] FR_FileData = PackLearnSetData("fr", FR_Output_Data);
            byte[] LG_FileData = PackLearnSetData("lg", LG_Output_Data);
            byte[] E_FileData  = PackLearnSetData("em", E_Output_Data);

            Directory.CreateDirectory(out_folder);
            File.WriteAllBytes(out_folder + "lvlmove_rs.pkl", RS_FileData);
            File.WriteAllBytes(out_folder + "lvlmove_fr.pkl", FR_FileData);
            File.WriteAllBytes(out_folder + "lvlmove_lg.pkl", LG_FileData);
            File.WriteAllBytes(out_folder + "lvlmove_e.pkl", E_FileData);

            //Test the files are correct
            Learnset[] RS_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_rs.pkl"), "rs"));
            Learnset[] FR_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_fr.pkl"), "fr"));
            Learnset[] LG_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_lg.pkl"), "lg"));
            Learnset[] E_Check_Data  = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_e.pkl"), "em"));

            //Check that the fileS that has been writen and then readed are equal than the generated ones
            string diff_rs = string.Empty;
            string diff_fr = string.Empty;
            string diff_lg = string.Empty;
            string diff_e = string.Empty;
            bool check_rs = CheckEquals(RS_Check_Data, RS_Check_Data, out diff_rs);
            if (!check_rs)
                throw new Exception("Check Ruby Sapphire LearnSet." + diff_rs);
            bool check_fr = CheckEquals(FR_Output_Data, FR_Check_Data, out diff_fr);
            if (!check_fr)
                throw new Exception("Check Fire Red LearnSet." + diff_fr);
            bool check_lg = CheckEquals(LG_Output_Data, LG_Check_Data, out diff_lg);
            if (!check_lg)
                throw new Exception("Check Leaf Green LearnSet." + diff_lg);
            bool check_e = CheckEquals(E_Output_Data, E_Check_Data, out diff_e);
            if (!check_e)
                throw new Exception("Check Emerald LearnSet." + diff_lg);
        }

        internal byte[][] ReadGBAROM(string filename, string folder,int OffSet_LevelUpData)
        {
            byte[][] OutputData = new byte[GData.MaxSpeciesIndexGeneration+1][];

            int[] OffsetTable = new int[GData.MaxSpeciesIndexGeneration+1];
            OffsetTable[0] = OffSet_LevelUpData;

            using (var s = new FileStream(folder + filename,FileMode.Open))
            using (var br = new BinaryReader(s))
            {
                br.BaseStream.Seek(OffSet_LevelUpData, SeekOrigin.Begin);
                for (int i = 1; i < OffsetTable.Length; i++)
                {
                   ushort value= br.ReadUInt16();
                   while(value!= 0xFFFF)
                        value = br.ReadUInt16();
             
                   OffsetTable[i] = (int)br.BaseStream.Position;
                }
                OutputData[0] = new byte[0]; //GBA data start with learnset from pokemon index 1, there is no data for the index number 0
                for (int i = 0; i < OffsetTable.Length-1; i++)
                {
                    br.BaseStream.Seek(OffsetTable[i], SeekOrigin.Begin);
                    OutputData[i+1] = br.ReadBytes((int)(OffsetTable[i + 1] - OffsetTable[i] - 2));
                }
            }
            return OutputData;
        }


        internal void TraceLearnSetIndex(string folder, string filename, Learnset[] LearnData)
        {
            string[] species = Util.getSpeciesList("en");
            string[] moves = Util.getMovesList("en");
            StringBuilder log = new StringBuilder();
            for (int i = 0; i < LearnData.Length; i++)
            {
                int dexnumber = TransformSpeciesIndex.getG4Species(i);
                if (dexnumber > 0)
                    log.AppendLine("SPECIE " + species[dexnumber]);
                else
                    log.AppendLine("UNUSED SLOT SPECIE");
                log.AppendLine("INDEX " + i + " DEX NUMBER " + dexnumber);
                for (int j = 0; j < LearnData[i].Count; j++)
                {
                    log.AppendLine(moves[LearnData[i].Moves[j]] + " at level " + LearnData[i].Levels[j]);
                }
                log.AppendLine();
            }

            File.WriteAllText(folder + filename, log.ToString());
        }

        internal override void TraceLearnSet(string folder, string filename, Learnset[] LearnData)
        {
            string[] species = Util.getSpeciesList("en");
            string[] moves = Util.getMovesList("en");
            StringBuilder log = new StringBuilder();
            for (int i = 0; i < LearnData.Length; i++)
            {
                int dexnumber = i;
                int indexnumber = TransformSpeciesIndex.getG3Species(i);
                if (dexnumber > 0)
                    log.AppendLine("SPECIE " + species[dexnumber]);
                else
                    log.AppendLine("UNUSED SLOT SPECIE ");
                log.AppendLine("INDEX " + indexnumber + " DEX NUMBER " + dexnumber);
                for (int j = 0; j < LearnData[i].Count; j++)
                {
                    log.AppendLine(moves[LearnData[i].Moves[j]] + " at level " + LearnData[i].Levels[j]);
                }
                log.AppendLine();
            }

            File.WriteAllText(folder + filename, log.ToString());
        }
    }
}
