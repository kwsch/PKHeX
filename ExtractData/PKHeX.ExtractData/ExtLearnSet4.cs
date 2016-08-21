using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PKHeX.ExtractData
{
    public class ExtLearnSet4
    {
        internal virtual GenerationData GData => Generation4Data.Create();

        const string D_Folder = "\\Diamond\\root\\poketool\\personal\\";
        const string P_Folder = "\\Pearl\\root\\poketool\\personal\\";
        const string Pt_Folder = "\\Platinum\\root\\poketool\\personal\\";
        const string HG_Folder = "\\HeartGold\\root\\a\\0\\3\\";
        const string SS_Folder = "\\SoulSilver\\root\\a\\0\\3\\";
        const int DP_MaxSpeciesFormsGenartion4 = 501;
        const int HGSSPt_MaxSpeciesFormsGenartion4 = 508;

        public virtual void ExtractLevelUpMovesData(string log_folder, string source_folder,string out_folder)
        {
            //Extract level up moves from the rom files
            byte[][] D_Source_Data  = ReadNarcNDS("wotbl.narc",source_folder + D_Folder, DP_MaxSpeciesFormsGenartion4, true);
            byte[][] P_Source_Data  = ReadNarcNDS("wotbl.narc", source_folder + P_Folder, DP_MaxSpeciesFormsGenartion4, true);
            byte[][] Pt_Source_Data = ReadNarcNDS("wotbl.narc", source_folder + Pt_Folder, HGSSPt_MaxSpeciesFormsGenartion4, true);
            byte[][] HG_Source_Data = ReadNarcNDS("3", source_folder + HG_Folder, HGSSPt_MaxSpeciesFormsGenartion4, true);
            byte[][] SS_Source_Data = ReadNarcNDS("3", source_folder + SS_Folder, HGSSPt_MaxSpeciesFormsGenartion4, true);

            //Check differences. Diamond and Pearl are equal. HG and SS are equal
            //Platinum and HG/SS are different
            //The first 501 from Platinum and D/P are different
            //HG/SS and D/P are different
            bool D_P_Equal   = ExtractUtils.CheckArrayEqual(D_Source_Data , P_Source_Data);
            bool HG_SS_Equal = ExtractUtils.CheckArrayEqual(HG_Source_Data, SS_Source_Data);
            bool HG_Pt_Equal = ExtractUtils.CheckArrayEqual(HG_Source_Data, Pt_Source_Data);
            bool DP_Pt_Equal = ExtractUtils.CheckArrayEqual(D_Source_Data , Pt_Source_Data.Take(501).ToArray());

            //Trabsform Raw data into PKHex format
            Learnset[] DP_Output_Data =   TransformLearnset(D_Source_Data);
            Learnset[] Pt_Output_Data =   TransformLearnset(Pt_Source_Data);
            Learnset[] HGSS_Output_Data = TransformLearnset(HG_Source_Data);

            TraceLearnSet(log_folder, "learnset_dp.log",   DP_Output_Data);
            TraceLearnSet(log_folder, "learnset_pt.log",   Pt_Output_Data);
            TraceLearnSet(log_folder, "learnset_hgss.log", HGSS_Output_Data);

            //Extract LearnSet to file
            byte[] DP_FileData = PackLearnSetData("dp", DP_Output_Data);
            byte[] Pt_FileData = PackLearnSetData("pt", Pt_Output_Data);
            byte[] HGSS_FileData = PackLearnSetData("hs", HGSS_Output_Data);

            Directory.CreateDirectory(out_folder);
            File.WriteAllBytes(out_folder + "lvlmove_dp.pkl", DP_FileData);
            File.WriteAllBytes(out_folder + "lvlmove_pt.pkl", Pt_FileData);
            File.WriteAllBytes(out_folder + "lvlmove_hgss.pkl", HGSS_FileData);

            //Test the fileS are correct
            Learnset[] DP_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_dp.pkl"), "dp"));
            Learnset[] Pt_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_pt.pkl"), "pt"));
            Learnset[] HGSS_Check_Data = Learnset.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "lvlmove_hgss.pkl"), "hs"));

            //Check that the fileS that has been writen and then readed are equal than the generated ones
            string diff_dp = string.Empty;
            string diff_pt = string.Empty;
            string diff_hgss = string.Empty;
            bool check_dp = CheckEquals(DP_Output_Data, DP_Check_Data, out diff_dp);
            if (!check_dp)
                throw new Exception("Check Diamond Pearl LearnSet." + diff_dp);
            bool check_pt = CheckEquals(Pt_Output_Data, Pt_Check_Data, out diff_pt);
            if (!check_pt)
                throw new Exception("Check Platinum LearnSet." + diff_pt);
            bool check_hgss = CheckEquals(HGSS_Output_Data, HGSS_Check_Data, out diff_hgss);
            if (!check_hgss)
                throw new Exception("Check Hearth Gold Soul Silver LearnSet." + diff_hgss);
        }

        internal bool CheckEquals(Learnset[] Learnset_Generated, Learnset[] Learnset_Readed, out string diff)
        {
            if(Learnset_Generated.Length != Learnset_Readed.Length)
            {
                diff = $"Different Lenght. Generated {Learnset_Generated.Length } Readed {Learnset_Readed.Length }";
                return false;
            }
            for(int i=0;i< Learnset_Generated.Length;i++)
            {
                if (Learnset_Generated[i].Count != Learnset_Readed[i].Count)
                {
                    diff = $"Different Lenght in element {i}. Generated {Learnset_Generated[i].Count } Readed {Learnset_Readed[i].Count}";
                    return false;
                }
                for (int j = 0; j < Learnset_Generated[i].Count; j++)
                {
                    if (Learnset_Generated[i].Moves[j]  != Learnset_Readed[i].Moves[j] ||
                        Learnset_Generated[i].Levels[j] != Learnset_Readed[i].Levels[j] )
                    {
                        diff = $"Different position {j} in element {i}. Generated {Learnset_Generated[i].Levels[j] } at {Learnset_Generated[i].Moves[j] } Readed {Learnset_Readed[i].Levels[j] } at {Learnset_Readed[i].Moves[j] }";
                        return false;
                    }
                }
            }

            diff = string.Empty;
            return true;
        }

        /// <summary>
        /// Transform raw level up move data from gen4 game to PKHex format
        /// </summary>
        /// <param name="species"> max species in generation</param>
        /// <param name="Input">raw level up move data </param>
        /// <returns>Learnset data in PKHex format</returns>
        internal virtual Learnset[] TransformLearnset(byte[][] Input)
        {
            Learnset[] Output = new Learnset[Input.Length];

            for (int i = 0; i < Input.Length; i++)
            {
                if (Input[i].Length % 2 == 1)
                    throw new Exception("Invalid input data");

                byte[] OutputLearnset = new byte[Input[i].Length * 2];
        
                //Output[i].Levels = new int[Input[i].Length /2];
                //Output[i].Moves = new int[Input[i].Length / 2];
                //Output[i].Count = Input[i].Length / 2;

                //Raw Format. Two bytes per move and level, bits 0-8 = moveid 9-15 = level
                for (int j = 0; j < Input[i].Length - 1; j += 2)
                {
                    ushort value = BitConverter.ToUInt16(Input[i], j);
                    short move = (short)(value & 0x1FF);
                    short level = (short)(value >> 9);
                    Array.Copy(BitConverter.GetBytes(move), 0, OutputLearnset, j * 2, 2);
                    Array.Copy(BitConverter.GetBytes(level), 0, OutputLearnset, j * 2 + 2, 2);
                }
                Output[i] = new Learnset(OutputLearnset);
            }

            return Output;
        }

       
        /// <summary>
        /// Extract all the bin files extracted from the NARC that contain level up moves information
        /// and then read the level up information from that bin files
        /// </summary>
        /// <param name="NARCFileName">Name of the narc file unextracted</param>
        /// <param name="folder">Folder where the extracted bin files are stored</param>
        /// <param name="countfiles">Spected num of files</param>
        /// <returns>Array of raw data with level up moves information</returns>
        internal byte[][] ReadNarcNDS(string NARCFileName, string folder, int countfiles, bool RemoveTerminator)
        {
            string NarcFIle = folder + NARCFileName;
            string NARCFileNameExtrac = NARCFileName.Replace(".narc", "");
            string ExtractFolder = folder + "Extract\\";
            int count = 0;
            if (Directory.Exists(ExtractFolder))
                 count = Directory.GetFiles(ExtractFolder, "*.bin").Length;
            //Extract bin files inside the NARC file
            if (count != countfiles)
            {
                if (Directory.Exists(ExtractFolder)) Directory.Delete(ExtractFolder,true);
                Directory.CreateDirectory(ExtractFolder);
                AndiNarcReader Reader = new AndiNarcReader();
                Reader.OpenData(NarcFIle);
                if (Reader.FileCount != countfiles)
                    throw new Exception($"Expected {countfiles} inside {NARCFileName} file. Found {Reader.FileCount}");

                for (int i = 0; i < Reader.FileCount; i++)
                    File.WriteAllBytes($"{ExtractFolder + NARCFileNameExtrac}_{i}.bin", Reader.getdataselected(i));
            }

            byte[][] ReadData = new byte[countfiles][];
            count = Directory.GetFiles(ExtractFolder, "*.bin").Length;
            if (count != countfiles)
                throw new Exception($"Expected {countfiles} bin files in folder {folder}. Found {count}");

            for(int filenum =0;filenum < countfiles;filenum ++ )
            {
                string filename = ExtractFolder + NARCFileNameExtrac + $"_{filenum}.bin";
                ReadData[filenum] = File.ReadAllBytes(filename);
                if(RemoveTerminator)
                { 
                    uint foot32 = BitConverter.ToUInt32(ReadData[filenum], ReadData[filenum].Length - 4);
                    if (foot32 == 0x0000FFFF)
                        ReadData[filenum] = ReadData[filenum].Take(ReadData[filenum].Length - 4).ToArray();
                    else
                    {
                        ushort foot16 = BitConverter.ToUInt16(ReadData[filenum], ReadData[filenum].Length - 2);
                        if (foot16 == 0xFFFF)
                            ReadData[filenum] = ReadData[filenum].Take(ReadData[filenum].Length - 2).ToArray();
                    }
                }
            }
            return ReadData;
        }

        internal byte[] PackLearnSetData(string Header, Learnset[] LearnSet)
        {
            using (var s = new MemoryStream())
            using (var bw = new BinaryWriter(s))
            {
                bw.Write(Header.ToCharArray());
                bw.Write((ushort)LearnSet.Length);
                uint offset = (uint)(4 + (LearnSet.Length * 4) + 4);
                for (int i = 0; i < LearnSet.Length; i++)
                {
                    bw.Write(offset);
                    offset += GetLength(LearnSet[i]);
                }
                bw.Write(offset);
                for (int i = 0; i < LearnSet.Length; i++)
                {
                    for (int j = 0; j < LearnSet[i].Count; j++)
                    {
                        bw.Write((short)LearnSet[i].Moves[j]);
                        bw.Write((short)LearnSet[i].Levels[j]);
                    }
                    bw.Write(uint.MaxValue);//0xFFFFFFFF
                }
                return s.ToArray();
            }
        }
             
        internal uint GetLength(Learnset LearnData)
        {
            //For every move two bytes for the move and two for the level plus 4 bytes for terminator FFFFFFFF
            return (uint)(LearnData.Count * 2 * 2 ) + 4;
        }

        internal virtual void TraceLearnSet(string folder, string filename, Learnset[] LearnData)
        {
            TraceLearnSet(folder, filename, LearnData, false);
        }

        internal virtual void TraceLearnSet(string folder,string filename, Learnset[] LearnData, bool B1W1)
        {
            string[] all_moves = Util.getMovesList("en");
            string log = string.Empty;

            for (int i = 0; i < LearnData.Length; i++)
            {
                int dexnumber = i;
                log += ExtractUtils.SpeciesLogText(i, GData, B1W1);
                if (LearnData[dexnumber].Count >0)
                {
                    for (int j = 0; j < LearnData[dexnumber].Count; j++)
                    {
                        log += all_moves[LearnData[dexnumber].Moves[j]] + " at level " + LearnData[dexnumber].Levels[j] + Environment.NewLine;
                    }
                }
                else
                {
                    log += " No level up moves available for this pokemon" + Environment.NewLine;
                }

                log += Environment.NewLine;
            }

            File.WriteAllText(folder + filename, log);
        }
    }
}
