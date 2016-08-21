using System;
using System.Linq;
using System.IO;

namespace PKHeX.ExtractData
{
    class ExtractUtils
    {

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
