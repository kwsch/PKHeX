using System;
using System.Linq;
using System.IO;
using System.Text;

namespace PKHeX.ExtractData
{
    class ExtMoveTutors3 : ExtTMHM3
    {
        internal override GenerationData GData => Generation3Data.Create();

        const int OffSet_FR_TutorsData = 0x459B60;
        const int OffSet_LG_TutorsData = 0x459580;
        const int OffSet_E_TutorsData  = 0x61500C;
        const int GBA_FRLG_MoveTutors_Count = 15;
        const int GBA_E_MoveTutors_Count = 30;
        const int MoveTutorsDataSize = 2;

        const int OffSet_FR_TutorsCompatibilty = 0x459B7E; 
        const int OffSet_LG_TutorsCompatibilty = 0x45959E;
        const int OffSet_E_TutorsCompatibilty = 0x615048; 
        const int GBA_E_TutorsComp_Size = 4;
        const int GBA_FRLG_TutorsComp_Size = 2;

        public void ExtractMoveTutors(string log_folder, string source_folder, string output_folder)
        {
            ushort[] E_MoveTutors   = ExtractMoveList(source_folder + gba_emerald_rom  , GBA_E_MoveTutors_Count   , OffSet_E_TutorsData , MoveTutorsDataSize, false);
            ushort[] FR_MoveTutors  = ExtractMoveList(source_folder + gba_firered_rom  , GBA_FRLG_MoveTutors_Count, OffSet_FR_TutorsData, MoveTutorsDataSize, false);
            ushort[] LG_TMoveTutors = ExtractMoveList(source_folder + gba_leafgreen_rom, GBA_FRLG_MoveTutors_Count, OffSet_LG_TutorsData, MoveTutorsDataSize, false);
            
            /*Moves are equal between FR and LG*/
            bool FR_LG_EqualsMoves = ExtractUtils.CheckArrayEqual(FR_MoveTutors, LG_TMoveTutors);

            byte[][] E_Compatibility  = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_emerald_rom  , OffSet_E_TutorsCompatibilty , GBA_E_TutorsComp_Size);
            byte[][] FRLG_Compatibility = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_firered_rom  , OffSet_FR_TutorsCompatibilty, GBA_FRLG_TutorsComp_Size);
            byte[][] LG_Compatibility = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + gba_leafgreen_rom, OffSet_LG_TutorsCompatibilty, GBA_FRLG_TutorsComp_Size);
            
            bool[][] E_Bool_FlagsCompatibility  = RawFlagsToBoolean(E_Compatibility , GBA_E_MoveTutors_Count);
            bool[][] FR_Bool_FlagsCompatibility = RawFlagsToBoolean(FRLG_Compatibility, GBA_FRLG_MoveTutors_Count);
            bool[][] LG_FlagsCompatibility = RawFlagsToBoolean(LG_Compatibility, GBA_FRLG_MoveTutors_Count);
            
            /*Compatibility is equal between FR and LG*/
            bool FR_LG_EqualsComp = ExtractUtils.CheckArrayEqual(FRLG_Compatibility, LG_Compatibility);

            TraceMoveTutorsList(log_folder + "E_MoveTutors.log", E_MoveTutors);
            TraceMoveTutorsList(log_folder + "FRLG_MoveTutors.log", FR_MoveTutors);
            TraceMoveTutorsCompatibilityList(log_folder, "E_MoveTutors_Index_Comp.log", E_MoveTutors, E_Bool_FlagsCompatibility,true);
            TraceMoveTutorsCompatibilityList(log_folder, "FRLG_MoveTutors_Index_Comp.log", FR_MoveTutors, FR_Bool_FlagsCompatibility, true);

            /*Data is stored in gen 3 index number, not in dex number. 
             * Order the data in dex order to have the same order in pkhex like the personal table*/
            E_Compatibility    = GData.TransformDexOrder(E_Compatibility);
            FRLG_Compatibility = GData.TransformDexOrder(FRLG_Compatibility);
            E_Bool_FlagsCompatibility = RawFlagsToBoolean(E_Compatibility, E_MoveTutors.Length);
            FR_Bool_FlagsCompatibility = RawFlagsToBoolean(FRLG_Compatibility, FR_MoveTutors.Length);

            TMHMTutorMoves[] FRLG_FlagsCompatibility = TMHMTutorMoves.getArray(FRLG_Compatibility);
            TMHMTutorMoves[] E_FlagsCompatibility    = TMHMTutorMoves.getArray(E_Compatibility);

            TraceMoveTutorsCompatibilityList(log_folder, "E_MoveTutors_DexNumber_Comp.log", E_MoveTutors, E_Bool_FlagsCompatibility, false);
            TraceMoveTutorsCompatibilityList(log_folder, "FRLG_MoveTutors_DexNumber_Comp.log", FR_MoveTutors, FR_Bool_FlagsCompatibility, false);

            /* Emerald contains all the info of FR/LG move tutors. Emerald data is enoight to make legal analysis in gen 3*/
            bool TutorsEContains = CheckMoveTutorsContains(E_MoveTutors, FR_MoveTutors, E_Bool_FlagsCompatibility, FR_Bool_FlagsCompatibility);

            byte[] TutorsFileData = ExtractUtils.PackTMHMTutorMoves("g3", E_FlagsCompatibility, GBA_E_TutorsComp_Size);
            Directory.CreateDirectory(output_folder);
            File.WriteAllBytes(output_folder + "tutors_g3.pkl", TutorsFileData);
            
            //Test the fileS are correct
            TMHMTutorMoves[] Tutors_Check_Data = TMHMTutorMoves.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(output_folder + "tutors_g3.pkl"), "g3"));
            string diff = string.Empty;
            bool check = ExtractUtils.CheckEquals(E_FlagsCompatibility, Tutors_Check_Data, out diff);
            if (!check)
                throw new Exception("Check Tutors Move Gen 3." + diff);
        }
        
        protected override int[] TutorCompatibility_BlastBurn => new int[] {3};
        protected override int[] TutorCompatibility_HydroCannon => new int[] {6};
        protected override int[] TutorCompatibility_FrenzyPlant => new int[] {9};
        protected override int[] TutorMove_DracoMeteor => new int[0];
        protected override int[] TutorCompatibility_DracoMeteor => new int[0];

        internal void TraceMoveTutorsCompatibilityList(string folder, string filename, ushort[] MoveTutors, bool[][] MoveList, bool IndexOrder)
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
                    log.AppendLine("UNUSED SLOT SPECIE ");

                log.AppendLine("DEX NUMBER " + dexnumber + " INDEX NUMBER " + indexnumber);
                if (MoveList[dexnumber].Any(x => x))
                {
                    string[] moves = new string[MoveList[dexnumber].Count(x => x)];
                    int count = 0;
                    for (int k = 0; k < MoveList[dexnumber].Length; k++)
                    {
                        if (MoveList[dexnumber][k])
                        {
                            moves[count] = all_moves[MoveTutors[k]] + Environment.NewLine;
                            count++;
                        }
                    }
                    //Print moves ordered to compare with webs like bulbapedia
                    log.Append(moves.OrderBy(m => m).Aggregate((m1, m2) => (m1 + m2)));
                }
                else
                {
                    log.AppendLine(" No move tutors available for this pokemon");
                }

                log.AppendLine();
            }
            File.WriteAllText(folder + filename, log.ToString());
        }

    }
}
