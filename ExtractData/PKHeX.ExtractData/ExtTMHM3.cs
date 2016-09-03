using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.ExtractData
{
    class ExtTMHM3 : ExtMoveTutors4
    {
        internal override GenerationData GData => Generation3Data.Create();

        internal override byte[][] ExtractCompatibilityList(int maxspecies, string filename, int offset, int size)
        {
            byte[][] Byte_Data = new byte[maxspecies+1][];
            using (var s = new FileStream(filename, FileMode.Open))
            using (var br = new BinaryReader(s))
            {
                br.BaseStream.Seek(offset, SeekOrigin.Begin);
                for (int i = 0; i <= maxspecies; i++)
                {
                    Byte_Data[i] = br.ReadBytes(size);
                }
            }

            return Byte_Data;
        }

        protected const string gba_firered_rom = "Pokemon - FireRed Version (USA).gba";
        protected const string gba_leafgreen_rom = "Pokemon - LeafGreen Version (USA).gba";
        protected const string gba_emerald_rom = "Pokemon - Emerald Version (USA, Europe).gba";
        protected const string gba_ruby_rom = "Pokemon - Ruby Version (USA).gba";
        protected const string gba_sapphire_rom = "Pokemon - Sapphire Version (USA).gba";

        const int OffSet_FR_TMHMData = 0x45A5A4;
        const int OffSet_LG_TMHMData = 0x459FC4;
        const int OffSet_R_TMHMData = 0x376504;
        const int OffSet_S_TMHMData = 0x376494; 
        const int OffSet_E_TMHMData = 0x615B94;
        const int TMHMDataSize = 2;

        const int OffSet_FR_TMHMCompatibilty = 0x252BC8; //0x252BD0 - 8
        const int OffSet_LG_TMHMCompatibilty = 0x252BA4; //0x252BAC - 8
        const int OffSet_R_TMHMCompatibilty =  0x1FD0F0; //0x1FD0F8 - 8
        const int OffSet_S_TMHMCompatibilty = 0x1FD080; //0x1FD088 - 8
        const int OffSet_E_TMHMCompatibilty = 0x31E898; //0x31E8A0 - 8
        const int GBA_HMTMComp_Size = 8;

        const int TMMoves_Gen3 = 50;
        const int HMMoves_Gen3 = 8;

        public void ExtractTMHM(string log_folder, string source_folder, string output_folder)
        {
            /*Extract move list Raw data*/
            ushort[] R_TMHM  = ExtractMoveList(source_folder + gba_ruby_rom      ,TMMoves_Gen3 + HMMoves_Gen3, OffSet_R_TMHMData  ,TMHMDataSize, false);
            ushort[] S_TMHM  = ExtractMoveList(source_folder + gba_sapphire_rom  ,TMMoves_Gen3 + HMMoves_Gen3, OffSet_S_TMHMData  ,TMHMDataSize, false);
            ushort[] E_TMHM  = ExtractMoveList(source_folder + gba_emerald_rom   ,TMMoves_Gen3 + HMMoves_Gen3, OffSet_E_TMHMData  ,TMHMDataSize, false);
            ushort[] FR_TMHM = ExtractMoveList(source_folder + gba_firered_rom   ,TMMoves_Gen3 + HMMoves_Gen3, OffSet_FR_TMHMData ,TMHMDataSize, false);
            ushort[] LG_TMHM = ExtractMoveList(source_folder + gba_leafgreen_rom ,TMMoves_Gen3 + HMMoves_Gen3, OffSet_LG_TMHMData ,TMHMDataSize, false);

            /*Data is equal between all GBA games*/
            bool R_S_EqualsMoves   = ExtractUtils.CheckArrayEqual(R_TMHM  ,S_TMHM);
            bool R_E_EqualsMoves   = ExtractUtils.CheckArrayEqual(R_TMHM  ,E_TMHM);
            bool FR_E_EqualsMoves  = ExtractUtils.CheckArrayEqual(FR_TMHM ,E_TMHM);
            bool FR_LG_EqualsMoves = ExtractUtils.CheckArrayEqual(FR_TMHM ,LG_TMHM);

            /*Extract compatibility list Raw data*/
            byte[][] R_Compatibility  = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_ruby_rom, OffSet_R_TMHMCompatibilty, GBA_HMTMComp_Size);
            byte[][] S_Compatibility  = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_sapphire_rom, OffSet_S_TMHMCompatibilty, GBA_HMTMComp_Size);
            byte[][] E_Compatibility  = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_emerald_rom, OffSet_E_TMHMCompatibilty, GBA_HMTMComp_Size);
            byte[][] FR_Compatibility = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_firered_rom, OffSet_FR_TMHMCompatibilty, GBA_HMTMComp_Size);
            byte[][] LG_Compatibility = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_leafgreen_rom, OffSet_LG_TMHMCompatibilty, GBA_HMTMComp_Size);

            /*Transform to bool array*/
            bool[][] R_FlagsCompatibility  = RawFlagsToBoolean(R_Compatibility, TMMoves_Gen3 + HMMoves_Gen3);
            bool[][] S_FlagsCompatibility  = RawFlagsToBoolean(S_Compatibility, TMMoves_Gen3 + HMMoves_Gen3);
            bool[][] E_FlagsCompatibility  = RawFlagsToBoolean(E_Compatibility, TMMoves_Gen3 + HMMoves_Gen3);
            bool[][] FR_FlagsCompatibility = RawFlagsToBoolean(FR_Compatibility, TMMoves_Gen3 + HMMoves_Gen3);
            bool[][] LG_FlagsCompatibility = RawFlagsToBoolean(LG_Compatibility, TMMoves_Gen3 + HMMoves_Gen3);

            /*Data is equal between all GBA games*/
            bool R_S_EqualsComp   = ExtractUtils.CheckArrayEqual(R_Compatibility, S_Compatibility);
            bool FR_LG_EqualsComp = ExtractUtils.CheckArrayEqual(FR_Compatibility, LG_Compatibility);
            bool R_E_EqualsComp   = ExtractUtils.CheckArrayEqual(R_Compatibility, E_Compatibility);
            bool FR_E_EqualsComp  = ExtractUtils.CheckArrayEqual(FR_Compatibility, E_Compatibility);

            /*From here only Emerald data is used*/

            TraceTMHMList(log_folder + "G3_HMTM.log", E_TMHM, TMMoves_Gen3, HMMoves_Gen3);
            TraceTMHMCompatibilityList(log_folder, "G3_HMTM_Index_Comp.log"    , E_TMHM, E_FlagsCompatibility, TMMoves_Gen3, HMMoves_Gen3,true);

            /*Data is stored in gen 3 index number, not in dex number. 
            * Order the data in dex order to have the same order in pkhex like the personal table*/
            E_Compatibility = GData.TransformDexOrder(E_Compatibility);
            E_FlagsCompatibility = RawFlagsToBoolean(E_Compatibility, TMMoves_Gen3 + HMMoves_Gen3);

            TraceTMHMCompatibilityList(log_folder, "G3_HMTM_DexNumber_Comp.log", E_TMHM, E_FlagsCompatibility, TMMoves_Gen3, HMMoves_Gen3, false);

            TMHMTutorMoves[] E_MovesCompatibility = TMHMTutorMoves.getArray(E_Compatibility);

            byte[] TutorsFileData = ExtractUtils.PackTMHMTutorMoves("g3", E_MovesCompatibility, GBA_HMTMComp_Size);
            Directory.CreateDirectory(output_folder);
            File.WriteAllBytes(output_folder + "hmtm_g3.pkl", TutorsFileData);

            //Test the fileS are correct
            TMHMTutorMoves[] Tutors_Check_Data = TMHMTutorMoves.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(output_folder + "hmtm_g3.pkl"), "g3"));
            string diff = string.Empty;
            bool check = ExtractUtils.CheckEquals(E_MovesCompatibility, Tutors_Check_Data, out diff);
            if (!check)
                throw new Exception("Check HM TM Moves Gen 3." + diff);

        }


        public int[][] InheritanceTMHMMoves(bool[][] FlagsCompatibility, ushort[] MoveList)
        {
            int[][] Output_Moves = new int[FlagsCompatibility.Length][];

            for (int i = 0; i < FlagsCompatibility.Length; i++)
            {
                Output_Moves[i] = MoveList.
                            Select((move, index) => new { index = index, move = move }).
                            Where(m => FlagsCompatibility[i][m.index]).
                            Select(m => (int)m.move).
                            ToArray();
            }

            return Output_Moves;
        }

        internal void TraceTMHMList(string filename, ushort[] MoveList, int TMCount, int HMCount)
        {
            StringBuilder log = new StringBuilder();
            string[] all_moves = Util.getMovesList("en");

            for (int i = 0; i < TMCount; i++)
            {
                log.AppendLine($"TM{(i + 1).ToString("00")} Move {MoveList[i]} { all_moves[MoveList[i]]}");
            }
            for (int i = TMCount; i < MoveList.Length; i++)
            {
                log.AppendLine($"HM{(i - TMCount + 1).ToString("00")} Move {MoveList[i]} { all_moves[MoveList[i]]}");
            }

            File.WriteAllText(filename, log.ToString());
        }

        internal void TraceTMHMCompatibilityList(string folder, string filename, ushort[] TMHMMoves, bool[][] MoveList,int TMCount,int HMCount,bool IndexOrder)
        {
            string[] species = Util.getSpeciesList("en");
            string[] all_moves = Util.getMovesList("en");
            StringBuilder log = new StringBuilder();

            for (int i = 0; i < MoveList.Length; i++)
            {
                int indexnumber = IndexOrder ? i : TransformSpeciesIndex.getG3Species(i);
                int dexnumber = IndexOrder ? TransformSpeciesIndex.getG4Species(i) : i;
                if (dexnumber > 0)
                    log.AppendLine("SPECIE " + species[dexnumber]);
                else
                    log.AppendLine("UNUSED SLOT SPECIE");

                log.AppendLine("DEX NUMBER " + dexnumber + " INDEX NUMBER " + indexnumber);
                if (MoveList[dexnumber].Any(x => x))
                {
                    int count = 0;
                    for (int k = 0; k < TMCount; k++)
                    {
                        if (MoveList[dexnumber][k])
                        {
                            log.AppendLine($"TM{(k + 1).ToString("00")} { all_moves[TMHMMoves[k]]}");
                            count++;
                        }
                    }
                    for (int k = TMCount; k < MoveList[dexnumber].Length; k++)
                    {
                        if (MoveList[dexnumber][k])
                        {
                            log.AppendLine($"HM{(k - TMCount + 1).ToString("00")} { all_moves[TMHMMoves[k]]}");
                            count++;
                        }
                    }
                }
                else
                    log.AppendLine(" No TM or HM moves available for this pokemon");

                log.AppendLine();
            }

            File.WriteAllText(folder + filename, log.ToString());
        }

    }
}
