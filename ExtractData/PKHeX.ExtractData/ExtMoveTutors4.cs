using System;
using System.Linq;
using System.IO;
using System.Text;

namespace PKHeX.ExtractData
{
    class ExtMoveTutors4
    {
        internal virtual GenerationData GData => Generation4Data.Create();

        const string pt_filename = "Platinum\\overlay\\overlay_0005.bin";
        const string hg_moves_filename = "HeartGold\\overlay\\overlay_0001.bin";
        const string ss_moves_filename = "SoulSilver\\overlay\\overlay_0001.bin";
        const string hg_compatibilty_filename = "HeartGold\\root\\fielddata\\wazaoshie\\waza_oshie.bin";
        const string ss_compatibilty_filename = "SoulSilver\\root\\fielddata\\wazaoshie\\waza_oshie.bin";

        const int NDS_Pt_MoveTutors_Offset = 0x2FF6C;
        const int NDS_Pt_MoveTutors_Size = 12;
        const int NDS_Pt_MoveTutors_Count = 38;
        const int NDS_Pt_MoveTutorsComp_Offset = 0x30134;
        const int NDS_Pt_MoveTutorsComp_Size = 5;
      
        const int NDS_HGSS_MoveTutors_Offset = 0x23AE0; 
        const int NDS_HGSS_MoveTutors_Size = 4;
        const int NDS_HGSS_MoveTutors_Count = 52;

        const int NDS_HGSS_MoveTutorsComp_Size = 8;
        const int NDS_HGSS_MoveTutorsComp_Offset = 0;

        public void ExtractMoveTutors4(string log_folder,string source_folder,string output_folder)
        {
            ushort[] HeartGold_MoveTutors  = ExtractMoveList(source_folder + hg_moves_filename, NDS_HGSS_MoveTutors_Count, NDS_HGSS_MoveTutors_Offset, NDS_HGSS_MoveTutors_Size,true);
            ushort[] SoulSilver_MoveTutors = ExtractMoveList(source_folder + ss_moves_filename, NDS_HGSS_MoveTutors_Count, NDS_HGSS_MoveTutors_Offset, NDS_HGSS_MoveTutors_Size, true);
            ushort[] Platinum_MoveTutors   = ExtractMoveList(source_folder + pt_filename,       NDS_Pt_MoveTutors_Count,   NDS_Pt_MoveTutors_Offset,   NDS_Pt_MoveTutors_Size, false);

            /*Move tutor dada is equals between Heart Gold and Soul Silver*/
            bool HGSS_EqualsMoves= ExtractUtils.CheckArrayEqual(HeartGold_MoveTutors, SoulSilver_MoveTutors);

            TraceMoveTutorsList(log_folder + "PT_MoveTutors.log", Platinum_MoveTutors);
            TraceMoveTutorsList(log_folder + "HGSS_MoveTutors.log", HeartGold_MoveTutors);

            byte[][] Pt_Compatibility = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + pt_filename,              NDS_Pt_MoveTutorsComp_Offset,   NDS_Pt_MoveTutorsComp_Size);
            bool[][] Pt_Bool_FlagsCompatibility = RawFlagsToBoolean(Pt_Compatibility, Platinum_MoveTutors.Length);


            byte[][] HG_Compatibility = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + hg_compatibilty_filename, NDS_HGSS_MoveTutorsComp_Offset, NDS_HGSS_MoveTutorsComp_Size);
            byte[][] SS_Compatibility = ExtractCompatibilityList(GData.MaxSpeciesIndexGeneration, source_folder + ss_compatibilty_filename, NDS_HGSS_MoveTutorsComp_Offset, NDS_HGSS_MoveTutorsComp_Size);

            /*Compatibility data is equals between Heart Gold and Soul Silver */
            bool HGSS_EqualsComp = ExtractUtils.CheckArrayEqual(SS_Compatibility, HG_Compatibility);

            bool[][] HGSS_Bool_FlagsCompatibility = RawFlagsToBoolean(HG_Compatibility, HeartGold_MoveTutors.Length);

            TMHMTutorMoves[] Pt_FlagsCompatibility = TMHMTutorMoves.getArray(Pt_Compatibility);
            TMHMTutorMoves[] HGSS_FlagsCompatibility = TMHMTutorMoves.getArray(HG_Compatibility);

            /*Move tutors compatibility tables do not have record for index number 494 and 495 (Eggs). All pokemon bellow them are 2 positions before its personal table index
             I move them to their position in personal table and create empty data for 494 and 495*/
            /* Size for TMHMTutorMoves is the byte length of bytes in raw format*/
            Pt_FlagsCompatibility = FixData494495(Pt_FlagsCompatibility  , NDS_HGSS_MoveTutorsComp_Size);
            HGSS_FlagsCompatibility = FixData494495(HGSS_FlagsCompatibility, NDS_HGSS_MoveTutorsComp_Size);
            /* Size for bool array is the num of move tutors*/
            Pt_Bool_FlagsCompatibility = FixData494495(Pt_Bool_FlagsCompatibility, Platinum_MoveTutors.Length);
            HGSS_Bool_FlagsCompatibility = FixData494495(HGSS_Bool_FlagsCompatibility, HeartGold_MoveTutors.Length);

            TraceMoveTutorsCompatibilityList(log_folder, "PT_MoveComp.log"  , Platinum_MoveTutors , Pt_Bool_FlagsCompatibility);
            TraceMoveTutorsCompatibilityList(log_folder, "HGSS_MoveComp.log", HeartGold_MoveTutors, HGSS_Bool_FlagsCompatibility);

            /* hg/ss contains all the info of Platinum move tutors. HGSS data is enoight to make legal analysis in gen 4*/
            bool Tutors_HGSS_Contains = CheckMoveTutorsContains(HeartGold_MoveTutors, Platinum_MoveTutors, HGSS_Bool_FlagsCompatibility, Pt_Bool_FlagsCompatibility);

            byte[] TutorsFileData = ExtractUtils.PackTMHMTutorMoves("g4",HGSS_FlagsCompatibility, NDS_HGSS_MoveTutorsComp_Size);
            Directory.CreateDirectory(output_folder);
            File.WriteAllBytes(output_folder + "tutors_g4.pkl", TutorsFileData);

            //Test the fileS are correct
            TMHMTutorMoves[] Tutors_Check_Data = TMHMTutorMoves.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(output_folder + "tutors_g4.pkl"), "g4"));
            string diff = string.Empty;
            bool check = ExtractUtils.CheckEquals(HGSS_FlagsCompatibility, Tutors_Check_Data, out diff);
            if (!check)
                throw new Exception("Check Tutors Move Gen 4." + diff);
        }

        internal ushort[] ExtractMoveList(string filename,int count, int offset, int size,bool Compressed)
        {
            ushort[] Output = new ushort[count];
            byte[] FileData = null;

            if(Compressed) //HG and SS move tutors files are compressed with BLZ format
            {
                BLZ.BLZCoder Coder = new BLZ.BLZCoder();
                Coder.BLZ_Decode(filename, filename + ".dec");
                FileData = File.ReadAllBytes(filename + ".dec");
            }
            else
                FileData = File.ReadAllBytes(filename);

            for (int i = 0; i < count; i++)
            {
                Output[i] = (ushort)readWord(FileData, offset + i * size);
            }
            return Output;
        }

        internal bool[] ReadBytesIntoFlags(byte[] data, int flagLength)
        {
            bool[] flags = new bool[flagLength];
            int sourceLength = data.Length * 8;
            for (int offsetIntoData=0; offsetIntoData<data.Length; offsetIntoData++)
            { 
                int thisByte = data[offsetIntoData] & 0xFF;
                for (int i = 0; i < 8; i++)
                {
                    if (i + offsetIntoData * 8 < flags.Length)
                    { 
                        flags[i + offsetIntoData * 8] = ((thisByte >> i) & 0x01) == 0x01;
                    }
                    else
                    {
                        if (((thisByte >> i) & 0x01) == 0x01)
                            throw new Exception($"Flag {i + offsetIntoData * 8 }set beyond limit. Not expected flags set beyond {flags.Length}");
                    }
                }
            }
            return flags;
        }

        internal virtual byte[][] ExtractCompatibilityList(int maxspecies, string filename, int offset,int size)
        {
            byte[][] Byte_Data = new byte[maxspecies][];
            Byte_Data[0] = new byte[size];

            using (var s = new FileStream(filename,FileMode.Open))
            using (var br = new BinaryReader(s))
            {
                br.BaseStream.Seek(offset, SeekOrigin.Begin);
                for (int i = 1; i < maxspecies - 2; i++)
                {
                    Byte_Data[i] = br.ReadBytes(size);
                }
            }
            Byte_Data[maxspecies - 1] = new byte[size];
            Byte_Data[maxspecies - 2] = new byte[size];

            return Byte_Data;
        }

        internal TMHMTutorMoves[] FixData494495(TMHMTutorMoves[] FlagsData, int FlagSize)
        {
            /*Data beyond max species (494-507) correspond to altered forms
             In the personal table index 494 and index 495 are empty and forms start at 496
             In the move tutors table the empty data is at 506 and 507
             That means 494-505 in flags data are equivalent to 496-507 at personal table
            */
            if(FlagsData.Length <= 508)
            {
                for (int i = 507; i >= 496;i--)
                    FlagsData[i] = FlagsData[i-2];
                FlagsData[494] = new TMHMTutorMoves(new byte[FlagSize]);
                FlagsData[495] = new TMHMTutorMoves(new byte[FlagSize]);
            }

            return FlagsData;
        }

        internal bool[][] FixData494495(bool[][] FlagsData, int FlagSize)
        {
            /*Data beyond max species (494-507) correspond to altered forms
             In the personal table index 494 and index 495 are empty and forms start at 496
             In the move tutors table the empty data is at 506 and 507
             That means 494-505 in flags data are equivalent to 496-507 at personal table
            */
            if (FlagsData.Length <= 508)
            {
                for (int i = 507; i >= 496; i--)
                    FlagsData[i] = FlagsData[i - 2];
                FlagsData[494] = new bool[FlagSize];
                FlagsData[495] = new bool[FlagSize];
            }

            return FlagsData;
        }

        internal bool[][] RawFlagsToBoolean(byte[][] Byte_Data,int CountFlags)
        {
            bool[][] Flags_Data = new bool[Byte_Data.Length][];
            Flags_Data[0] = new bool[CountFlags];
            for (int i = 0; i < Byte_Data.Length; i++)
            {
                Flags_Data[i] = ReadBytesIntoFlags(Byte_Data[i], CountFlags);
            }
            return Flags_Data;
        }

        private int readWord(byte[] data, int offset)
        {
            return (data[offset] & 0xFF) + ((data[offset + 1] & 0xFF) << 8);
        }

        internal void TraceMoveTutorsList(string filename, ushort[] MoveList)
        {
            StringBuilder log = new StringBuilder();
            string[] all_moves = Util.getMovesList("en");
            for (int i = 0; i < MoveList.Length; i++)
            {
                log.AppendLine( $"Index {i} Move {MoveList[i]} { all_moves[MoveList[i]]}");
            }
            File.WriteAllText(filename, log.ToString());
        }
        internal void TraceMoveTutorsCompatibilityList(string folder,string filename,ushort[] MoveTutors, bool[][] MoveList)
        {
            string[] species = Util.getSpeciesList("en");
            string[] all_moves = Util.getMovesList("en");
            StringBuilder log = new StringBuilder();
            for (int i = 0; i < MoveList.Length; i++)
            {
                int dexnumber = i;
                ExtractUtils.SpeciesLogText(ref log, i, GData, false);
                if (MoveList[dexnumber].Any(x=>x))
                {
                    string[] moves = new string[MoveList[dexnumber].Count(x=>x)];
                    int count = 0;
                    for (int k = 0; k < MoveList[dexnumber].Length; k++)
                    {
                        if (MoveList[dexnumber][k])
                        {
                            //log += all_moves[MoveTutors[k]] + Environment.NewLine;
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

        protected virtual int[] TutorMove_BlastBurn => new int[] { 307 };
        protected virtual int[] TutorCompatibility_BlastBurn => new int[] { 3, 154, 254, 389 }; 
        protected virtual int[] TutorMove_HydroCannon => new int[] { 308 };
        protected virtual int[] TutorCompatibility_HydroCannon => new int[] { 6, 157, 257, 392 };
        protected virtual int[] TutorMove_FrenzyPlant => new int[] { 338 };
        protected virtual int[] TutorCompatibility_FrenzyPlant => new int[] { 9, 160, 260, 395 };
        protected virtual int[] TutorMove_DracoMeteor => new int[] { 434 };
        protected virtual int[] TutorCompatibility_DracoMeteor => new int[] { 147, 148, 149, 230, 329, 330, 334, 371, 372, 373, 380, 381, 384, 443, 444, 445, 483, 484, 487 };

        public virtual int[][] LearnDataSpecialMoveTutors(int[][] LearnData)
        {
            //307 Blast Burn. Add to fire starters at stage 3
            for (int i = 0; i < TutorCompatibility_BlastBurn.Length; i++)
                LearnData[TutorCompatibility_BlastBurn[i]].Union(TutorMove_BlastBurn).ToArray();
            //308 Hydro Cannon. Add to water starters at stage 3
            for (int i = 0; i < TutorCompatibility_HydroCannon.Length; i++)
                LearnData[TutorCompatibility_HydroCannon[i]].Union(TutorMove_HydroCannon).ToArray();
            //338 Frenzy Plant. Add to plant starters at stage 3
            for (int i = 0; i < TutorCompatibility_FrenzyPlant.Length; i++)
                LearnData[TutorCompatibility_FrenzyPlant[i]].Union(TutorMove_FrenzyPlant).ToArray();
            //434 Draco Meteor. Add to dragons
            for (int i = 0; i < TutorCompatibility_DracoMeteor.Length; i++)
                LearnData[TutorCompatibility_DracoMeteor[i]].Union(TutorMove_DracoMeteor).ToArray();

            return LearnData;
        }

        public bool CheckMoveTutorsContains(ushort[] MoveTutors1, ushort[] MoveTutors2, bool[][] FlagsCompatibility1, bool[][] FlagsCompatibility2)
        {
            for (int move = 0; move < MoveTutors2.Length; move++ )
                if (MoveTutors1[move] != MoveTutors2[move])
                    return false;

            for (int species = 0; species < FlagsCompatibility2.Length; species++)
            {
                for(int move =0; move < FlagsCompatibility2[species].Length; move++ )
                {
                    if (FlagsCompatibility1[species][move])
                        continue;
                    if (FlagsCompatibility1[species][move]!= FlagsCompatibility2[species][move] )
                        return false;
                }
            }

            return true;
        }

    }
}
