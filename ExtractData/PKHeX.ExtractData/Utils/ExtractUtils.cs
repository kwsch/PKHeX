using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.ExtractData
{
    class ExtractUtils
    {
        internal static ushort[] SearchMovesId(string[] SearchMoves)
        {
            string[] all_moves = Util.getMovesList("en");
            ushort[] Id_SearchMoves = new ushort[SearchMoves.Length];

            for (int i = 0; i < Id_SearchMoves.Length; i++)
            {
                string move_find = SearchMoves[i].Trim();
                if (all_moves.Contains(move_find))
                {
                    Id_SearchMoves[i] = (ushort)all_moves.
                            Select((move, index) => new { index = index, move = move }).
                            Where(x => x.move == move_find).
                            Select(x => x.index).
                            First();
                }
                else
                {
                    object a = new object();
                }
            }

            return Id_SearchMoves;
        }
        internal static ushort SearchMovesId(string SearchMove)
        {
            string[] all_moves = Util.getMovesList("en");
           
            if (all_moves.Contains(SearchMove))
            {
                return (ushort)all_moves.
                        Select((move, index) => new { index = index, move = move }).
                        Where(x => x.move == SearchMove).
                        Select(x => x.index).
                        First();
            }
            else
            {
                return 0;
            }
        }

        internal static int SearchSpecieId(string SearchSpecie)
        {
            string[] all_species= Util.getSpeciesList("en");

            if (all_species.Contains(SearchSpecie))
            {
                return  all_species.
                        Select((name, index) => new { index = index, name = name }).
                        Where(x => x.name == SearchSpecie).
                        Select(x => x.index).
                        First();
            }
            else
            {
                return 0;
            }
        }

        private static string[] getStringList(string txt)
        {
            //object txt = Properties.Resources.ResourceManager.GetObject("text_" + f + "_" + l); // Fetch File, \n to list.
            if (txt == null) return new string[0];
            string[] rawlist = ((string)txt).Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].Trim();
            return rawlist;
        }

        internal static bool HasFormWithTableEntry(int species, PersonalTable Personal)
        {
            for (int j = 0; j < Personal[species].FormeCount; j++)
            {
                if (Personal[species].FormeIndex(species, j) != species)
                    return true;
            }
            return false;
        }

        public static int[][] MoveListClone(int[][] LearnSet1)
        {
            int[][] Output_Moves = new int[LearnSet1.Length][];

            for (int i = 0; i < LearnSet1.Length; i++)
            {
                Output_Moves[i] = (int[])LearnSet1[i].Clone();
            }
            return Output_Moves;
        }

        public static int[][] JoinMoveListData(int[][] LearnSet1, int[][] LearnSet2)
        {
            int[][] Output_Moves = new int[Math.Max(LearnSet1.Length, LearnSet2.Length)][];

            for (int i = 0; i < LearnSet1.Length; i++)
            {
                if (LearnSet1.Length > i && LearnSet2.Length > i)
                    Output_Moves[i] = LearnSet1[i].Union(LearnSet2[i]).ToArray();
                else if (LearnSet1.Length > i)
                    Output_Moves[i] = LearnSet1[i];
                else if (LearnSet2.Length > i)
                    Output_Moves[i] = LearnSet2[i];
            }
            return Output_Moves;
        }

        internal static string[] gendersymbols = { "♂", "♀", "-" };

        internal static bool CheckArrayEqual(byte[][] Data1, byte[][] Data2)
        {
            if (Data1.Length != Data2.Length)
                return false;
            for (int i = 0; i < Data1.Length; i++)
            {
                if (Data1[i].Length != Data2[i].Length)
                    return false;
                if (!Data1[i].SequenceEqual(Data2[i]))
                    return false;
            }
            return true;
        }

        internal static bool CheckArrayEqual(Learnset[] Data1, Learnset[] Data2)
        {
            if (Data1.Length != Data2.Length)
                return false;
            for (int i = 0; i < Data1.Length; i++)
            {
                if (Data1[i].Count != Data2[i].Count)
                    return false;
                if (!Data1[i].Levels.SequenceEqual(Data2[i].Levels))
                    return false;
            }
            return true;
        }

        internal static bool CheckArrayEqual(Learnset[] Data1, Learnset[] Data2,int[] Exclude)
        {
            if (Data1.Length != Data2.Length)
                return false;
            for (int i = 0; i < Data1.Length; i++)
            {
                if (Exclude.Contains(i))
                    continue;
                if (Data1[i].Count != Data2[i].Count)
                    return false;
                if (!Data1[i].Levels.SequenceEqual(Data2[i].Levels))
                    return false;
                if (!Data1[i].Moves.SequenceEqual(Data2[i].Moves))
                    return false;
            }
            return true;
        }

        internal static bool CheckArrayEqual(byte[] Data1, byte[] Data2)
        {
            if (Data1.Length != Data2.Length)
                return false;
            if (!Data1.SequenceEqual(Data2))
                return false;
            return true;
        }

        internal static bool CheckArrayEqual(ushort[] Data1, ushort[] Data2)
        {
            if (Data1.Length != Data2.Length)
                return false;
            if (!Data1.SequenceEqual(Data2))
                return false;
            return true;
        }

        internal static bool CheckArrayEqual(int[][] Data1, int[][] Data2)
        {
            if (Data1.Length != Data2.Length)
                return false;
            for (int i = 0; i < Data1.Length; i++)
            {
                if (Data1[i].Length != Data2[i].Length)
                    return false;
                if (!Data1[i].SequenceEqual(Data2[i]))
                    return false;
            }
            return true;
        }

        static int SearchBytes(byte[] haystack, byte[] needle)
        {
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

        /*First array contains species who have another entry in the personal table and learnset table with different data
        /*Second array related index number above max species of the generation with the index number of its form 0*/
        /*Third array related index number above max species of the generation with the index number of its form in the forms array of the species*/
        internal static readonly int[] Dexnumber_HasForms4 = { 386, 413, 479, 487, 492 };
        internal static readonly int[] FormToDexNumber4 = { 0, 0, 386, 386, 386, 413, 413, 487, 492, 479, 479, 479, 479, 479 };
        internal static readonly int[] FormNumberIndex4 = { 0, 0, 1,   2,   3,   1,   2,   1,   1,   1,   2,   3,   4,   5 };

        internal static readonly int[] Dexnumber_HasForms5_B2W2 = { 351, 386, 413, 479, 487, 492, 550, 555, 641, 642, 645, 646, 647, 648 };
        internal static readonly int[] FormToDexNumber5_B2W2 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 386, 386, 386, 413, 413, 492, 487, 479, 479, 479, 479, 479, 351, 351, 351, 550, 555, 648, 646, 646, 647, 641, 642, 645};
        internal static readonly int[] FormNumberIndex5_B2W2 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,   2,   3,   1,   2,   1,   1,   1,   2,   3,   4,   5,   1,   2,   3,   1,   1,   1,   1,   2,   1,   1,   1,   1  };

        internal static readonly int[] Dexnumber_HasForms5_B1W1 = { 351, 386, 413, 479, 487, 492, 550, 555, 648 };
        internal static readonly int[] FormToDexNumber5_B1W1 = { 386,386,386,413,413,492,487,479,479,479,479,479,351,351,351,550,555,648};
        internal static readonly int[] FormNumberIndex5_B1W1 = { 1,  2,  3,  1,  2,  1,  1,  1,  2,  3,  4,  5,  1,  2,  3,  1,  1,  1  };

        internal static byte[][] unpackMini(byte[] fileData, string identifier)
        {
            using (var s = new MemoryStream(fileData))
            using (var br = new BinaryReader(s))
            {
                if (identifier != new string(br.ReadChars(2)))
                    return null;

                ushort count = br.ReadUInt16();
                byte[][] returnData = new byte[count][];

                uint[] offsets = new uint[count + 1];
                for (int i = 0; i < count; i++)
                    offsets[i] = br.ReadUInt32();

                uint length = br.ReadUInt32();
                offsets[offsets.Length - 1] = length;

                for (int i = 0; i < count; i++)
                {
                    br.BaseStream.Seek(offsets[i], SeekOrigin.Begin);
                    using (MemoryStream dataout = new MemoryStream())
                    {
                        byte[] data = new byte[0];
                        s.CopyTo(dataout, (int)offsets[i]);
                        int len = (int)offsets[i + 1] - (int)offsets[i];

                        if (len != 0)
                        {
                            data = dataout.ToArray();
                            Array.Resize(ref data, len);
                        }
                        returnData[i] = data;
                    }
                }
                return returnData;
            }
        }

        internal static string SpeciesLogText(int indexnumber, GenerationData GData,bool B1W1)
        {
            string[] species = Util.getSpeciesList("en");
            string log = "";
            if (indexnumber == 0)
                log += "UNUSED DATA " + Environment.NewLine;
            else if (indexnumber <= GData.MaxSpeciesGeneration)
                log += "SPECIE " + species[indexnumber] + Environment.NewLine;

            int[] Hasforms = new int[0];
            int[] FormToDexNumber = new int[0];
            int[] FormNumberIndex = new int[0];
            if (GData.Generation == 4)
            {
                Hasforms = Dexnumber_HasForms4;
                FormToDexNumber = FormToDexNumber4;
                FormNumberIndex = FormNumberIndex4;
            }

            if (GData.Generation == 5 && B1W1)
            {
                Hasforms = Dexnumber_HasForms5_B1W1;
                FormToDexNumber = FormToDexNumber5_B1W1;
                FormNumberIndex = FormNumberIndex5_B1W1;
            }

            if (GData.Generation == 5 && !B1W1)
            {
                Hasforms = Dexnumber_HasForms5_B2W2;
                FormToDexNumber = FormToDexNumber5_B2W2;
                FormNumberIndex = FormNumberIndex5_B2W2;
            }

            if (Hasforms.Contains(indexnumber))
            {
                string[] formStrings = PKX.getFormList(indexnumber,
                                         Util.getStringList("types", "en"),
                                         Util.getStringList("forms", "en"), ExtractUtils.gendersymbols);
                log += Environment.NewLine;
                log += "FORM " + formStrings[0] + Environment.NewLine;
            }

            if (indexnumber > GData.MaxSpeciesGeneration)
            {
                if (indexnumber > GData.MaxSpeciesGeneration && FormToDexNumber[indexnumber - GData.MaxSpeciesGeneration - 1] > 0)
                {
                    int realdexnumber = FormToDexNumber[indexnumber - GData.MaxSpeciesGeneration - 1];
                    log += "SPECIE " + species[realdexnumber] + " DEX NUMBER " + realdexnumber + Environment.NewLine;
                    string[] formStrings = PKX.getFormList(realdexnumber,
                                   Util.getStringList("types", "en"),
                                   Util.getStringList("forms", "en"), ExtractUtils.gendersymbols);
                    log += Environment.NewLine;
                    log += "FORM " + formStrings[FormNumberIndex[indexnumber - GData.MaxSpeciesGeneration - 1]] + Environment.NewLine;
                }
                else  //494 495 in GEN 4
                    log += "UNUSED DATA " + Environment.NewLine;
            }

            log += "INDEX NUMBER " + indexnumber + Environment.NewLine;
            return log;
        }

        internal static byte[] PackTMHMTutorMoves(string Header, TMHMTutorMoves[] Moves,int Size)
        {
            using (var s = new MemoryStream())
            using (var bw = new BinaryWriter(s))
            {
                bw.Write(Header.ToCharArray());
                bw.Write((ushort)Moves.Length);
                uint offset = (uint)(4 + (Moves.Length * 4) + 4);
                for (int i = 0; i < Moves.Length; i++)
                {
                    bw.Write(offset);
                    offset += (uint)Size;
                }
                bw.Write(offset);
                for (int i = 0; i < Moves.Length; i++)
                {
                    bw.Write(Moves[i].setBits());
                }
                return s.ToArray();
            }
        }


        internal static bool CheckEquals(TMHMTutorMoves[] Moves_Generated, TMHMTutorMoves[] Moves_Readed, out string diff)
        {
            if (Moves_Generated.Length != Moves_Readed.Length)
            {
                diff = $"Different Lenght. Generated {Moves_Generated.Length } Readed {Moves_Readed.Length }";
                return false;
            }
            for (int i = 0; i < Moves_Generated.Length; i++)
            {
                if (Moves_Generated[i].Compatibility.Length  != Moves_Readed[i].Compatibility.Length)
                {
                    diff = $"Different Lenght in element {i}. Generated {Moves_Generated[i].Compatibility.Length  } Readed {Moves_Readed[i].Compatibility.Length }";
                    return false;
                }
                for (int j = 0; j < Moves_Generated[i].Compatibility.Length; j++)
                {
                    if (Moves_Generated[i].Compatibility[j] != Moves_Readed[i].Compatibility[j])
                    {
                        diff = $"Different value {j} in element {i}. Generated {Moves_Generated[i].Compatibility[j] }  Readed {Moves_Readed[i].Compatibility[j] }";
                        return false;
                    }
                }
            }

            diff = string.Empty;
            return true;
        }
    }
}
