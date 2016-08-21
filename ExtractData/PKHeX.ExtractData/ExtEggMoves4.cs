using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.ExtractData
{
    public class ExtEggMoves4
    {
        internal virtual GenerationData GData => Generation4Data.Create();

        //EggMove data of generation 4 fames are stored in the following files, before launching this program 
        //it should be extracted from the NDS rom with PPSE
        const string p_filename = "Pearl\\overlay\\overlay_0005.bin";
        const string d_filename = "Diamond\\overlay\\overlay_0005.bin";
        const string pt_filename = "Platinum\\overlay\\overlay_0005.bin";
        const string hg_filename = "HeartGold\\root\\a\\2\\2\\9";
        const string ss_filename = "SoulSilver\\root\\a\\2\\2\\9";

        const int NDS_DP_EggMove_Offset = 0x00020620;
        const int NDS_Pt_EggMove_Offset = 0x0002922A;
        const int NDS_HGSS_EggMove_Offset = 0x0000003C;
        //The dex number of pokemon that have egg moves are stored adding 4E20 (20000) to the internal number of the pokemon
        //For example Bulbasaur 1 becomes 4E21, charmander 4 becomes 4E24 and so on
        protected const int Diff_EggIndex_Dex = 0x4E20;
      
        protected static ushort[] SpeciesEggIndex = null;

        static ushort[] SpeciesEggIndexG4 => new ushort[]
        {
            0x4E21, //BULBASAUR
            0x4E24, //CHARMANDER
            0x4E27, //SQUIRTLE
            0x4E30, //PIDGEY
            0x4E33, //RATTATA
            0x4E35, //SPEAROW
            0x4E37, //EKANS
            0x4E3B, //SANDSHREW
            0x4E3D, //NIDORAN (F)
            0x4E40, //NIDORAN (M
            0x4E45, //VULPIX
            0x4E49, //ZUBAT
            0x4E4B, //ODDISH
            0x4E4E, //PARAS
            0x4E50, //VENONAT
            0x4E52, //DIGLETT
            0x4E54, //MEOWTH
            0x4E56, //PSYDUCK
            0x4E58, //MANKEY
            0x4E5A, //GROWLITHE
            0x4E5C, //POLIWAG
            0x4E5F, //ABRA
            0x4E62, //MACHOP
            0x4E65, //BELLSPROUT
            0x4E68, //TENTACOOL
            0x4E6A, //GEODUDE
            0x4E6D, //PONYTA
            0x4E6F, //SLOWPOKE
            0x4E73, //FARFETCH'D
            0x4E74, //DODUO
            0x4E76, //SEEL
            0x4E78, //GRIMER
            0x4E7A, //SHELLDER
            0x4E7C, //GASTLY
            0x4E7F, //ONIX
            0x4E80, //DROWZEE
            0x4E82, //KRABBY
            0x4E86, //EXEGGCUTE
            0x4E88, //CUBONE
            0x4E8C, //LICKITUNG
            0x4E8D, //KOFFING
            0x4E8F, //RHYHORN
            0x4E91, //CHANSEY
            0x4E92, //TANGELA
            0x4E93, //KANGASKHAN
            0x4E94, //HORSEA
            0x4E96, //GOLDEEN
            0x4E9A, //MR MIME
            0x4E9B, //SCYTHER
            0x4E9F, //PINSIR
            0x4EA3, //LAPRAS
            0x4EA5, //EEVEE
            0x4EAA, //OMANYTE
            0x4EAC, //KABUTO
            0x4EAE, //AERODACTYL
            0x4EAF, //SNORLAX
            0x4EB3, //DRATINI
            0x4EB8, //CHIKORITA
            0x4EBB, //CYNDAQUIL
            0x4EBE, //TOTODILE
            0x4EC1, //SENTRET
            0x4EC3, //HOOTHOOT
            0x4EC5, //LEDYBA
            0x4EC7, //SPINARAK
            0x4ECA, //CHINCHOU
            0x4ECC, //PICHU
            0x4ECD, //CLEFFA
            0x4ECE, //IGGLYBUFF
            0x4ECF, //TOGEPI
            0x4ED1, //NATU
            0x4ED3, //MAREEP
            0x4ED7, //MARILL
            0x4ED9, //SUDOWOODO
            0x4EDB, //HOPPIP
            0x4EDE, //AIPOM
            0x4EDF, //SUNKERN
            0x4EE1, //YANMA
            0x4EE2, //WOOPER
            0x4EE6, //MURKROW
            0x4EE8, //MISDREAVUS
            0x4EEB, //GIRAFARIG
            0x4EEC, //PINECO
            0x4EEE, //DUNSPARCE
            0x4EEF, //GLIGAR
            0x4EF1, //SNUBBULL
            0x4EF3, //QWILFISH
            0x4EF5, //SHUCKLE
            0x4EF6, //HERACROSS
            0x4EF7, //SNEASEL
            0x4EF8, //TEDDIURSA
            0x4EFA, //SLUGMA
            0x4EFC, //SWINUB
            0x4EFE, //CORSOLA
            0x4EFF, //REMORAID
            0x4F01, //MANTINE
            0x4F02, //DELIBIRD
            0x4F03, //SKARMORY
            0x4F04, //HOUNDOUR
            0x4F07, //PHANPY
            0x4F0A, //STANTLER
            0x4F0C, //TYROGUE
            0x4F0E, //SMOOCHUM
            0x4F0F, //ELEKID
            0x4F10, //MAGBY
            0x4F11, //MILTANK
            0x4F16, //LARVITAR
            0x4F1C, //TREECKO
            0x4F1F, //TORCHIC
            0x4F22, //MUDKIP
            0x4F25, //POOCHYENA
            0x4F27, //ZIGZAGOON
            0x4F2E, //LOTAD
            0x4F31, //SEEDOT
            0x4F34, //TAILLOW
            0x4F36, //WINGULL
            0x4F38, //RALTS
            0x4F3B, //SURSKIT
            0x4F3D, //SHROOMISH
            0x4F3F, //SLAKOTH
            0x4F42, //NINCADA
            0x4F45, //WHISMUR
            0x4F48, //MAKUHITA
            0x4F4A, //AZURILL
            0x4F4B, //NOSEPASS
            0x4F4C, //SKITTY
            0x4F4E, //SABLEYE
            0x4F4F, //MAWILE
            0x4F50, //ARON
            0x4F53, //MEDITITE
            0x4F55, //ELECTRIKE
            0x4F57, //PLUSLE
            0x4F58, //MINUN
            0x4F59, //VOLBEAT
            0x4F5A, //ILLUMISE
            0x4F5B, //ROSELIA
            0x4F5C, //GULPIN
            0x4F5E, //CARVANHA
            0x4F60, //WAILMER
            0x4F62, //NUMEL
            0x4F64, //TORKOAL
            0x4F65, //SPOINK
            0x4F67, //SPINDA
            0x4F68, //TRAPINCH
            0x4F6B, //CACNEA
            0x4F6D, //SWABLU
            0x4F6F, //ZANGOOSE
            0x4F70, //SEVIPER
            0x4F73, //BARBOACH
            0x4F75, //CORPHISH
            0x4F79, //LILEEP
            0x4F7B, //ANORITH
            0x4F7D, //FEEBAS
            0x4F7F, //CASTFORM
            0x4F80, //KECLEON
            0x4F81, //SHUPPET
            0x4F83, //DUSKULL
            0x4F85, //TROPIUS
            0x4F86, //CHIMECHO
            0x4F87, //ABSOL
            0x4F89, //SNORUNT
            0x4F8B, //SPHEAL
            0x4F8E, //CLAMPERL
            0x4F91, //RELICANTH
            0x4F92, //LUVDISC
            0x4F93, //BAGON
            0x4FA3, //TURTWIG
            0x4FA6, //CHIMCHAR
            0x4FA9, //PIPLUP
            0x4FAC, //STARLY
            0x4FAF, //BIDOOF
            0x4FB3, //SHINX
            0x4FB6, //BUDEW
            0x4FB8, //CRANIDOS
            0x4FBA, //SHIELDON
            0x4FC1, //PACHIRISU
            0x4FC2, //BUIZEL
            0x4FC4, //CHERUBI
            0x4FC6, //SHELLOS
            0x4FC9, //DRIFLOON
            0x4FCB, //BUNEARY
            0x4FCF, //GLAMEOW
            0x4FD1, //CHINGLING
            0x4FD2, //STUNKY
            0x4FD6, //BONSLY
            0x4FD7, //MIME JR
            0x4FD8, //HAPPINY
            0x4FD9, //CHATOT
            0x4FDA, //SPIRITOMB
            0x4FDB, //GIBLE
            0x4FDE, //MUNCHLAX
            0x4FDF, //RIOLU
            0x4FE1, //HIPPOPOTAS
            0x4FE3, //SKORUPI
            0x4FE5, //CROAGUNK
            0x4FE7, //CARNIVINE
            0x4FE8, //FINNEON
            0x4FEA, //MANTYKE
            0x4FEB //SNOVER
        };

        /// <summary>
        /// Extract egg moves data from a NDS rom extracted file 
        /// </summary>
        /// <param name="filename">bin file with egg moves extracted from NDS rom</param>
        /// <param name="offset">offset where egg moves data start in the file</param>
        /// <returns>Aray of array of ints with every egg move that a pokemon can learn, order by species egg index in the rom data</returns>
        public int[][] ReadRawDataEggMoves(string filename, int offset)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("File Not Exits");

            int current = -1;
            int[][] Moves = new int[SpeciesEggIndex.Length][];

            List<int> CurrentSpeciesMoves = null;
            using (var s = new FileStream(filename, FileMode.Open))
            using (var br = new BinaryReader(s))
            {
                br.BaseStream.Seek(offset, SeekOrigin.Begin);
                while (true)
                {
                    ushort value = br.ReadUInt16();
                    string svalue = value.ToString("X");
                    if (current < SpeciesEggIndex.Length - 1 && value == SpeciesEggIndex[current + 1])
                    {
                        if (current >= 0)
                            Moves[current] = CurrentSpeciesMoves.ToArray();
                        CurrentSpeciesMoves = new List<int>();
                        current++;
                    }
                    else if (SpeciesEggIndex.Contains(value))
                    {
                        throw new Exception($"Invalid File Data. Specie {value - Diff_EggIndex_Dex} found in incorrect order in egg moves table.");
                    }
                    else if (value == 0xFFFF)
                    {
                        Moves[current] = CurrentSpeciesMoves.ToArray();
                        break;
                    }
                    else if (value > SpeciesEggIndex[0])
                    {
                        throw new Exception($"Invalid File Data. Specie {value - Diff_EggIndex_Dex} should not exits in egg moves table.");
                    }
                    else
                    {
                        CurrentSpeciesMoves.Add(value);
                    }
                }
            };

            return Moves;
        }


        //Calculate if the one eggmove list contains the other list
        //Return true if the first list have the same species as the second list and every
        //species of the first list learn all the moves of the same species in the second list
        public bool CalculateEggMovesContains(EggMoves[] EggMoves1, EggMoves[] EggMoves2)
        {
            for (int specie = 0; specie < EggMoves1.Length; specie++)
            {
                if (EggMoves1[specie].Count < EggMoves2[specie].Count)
                    return false;

                for (int countmove = 0; countmove < EggMoves2[specie].Moves.Length; countmove++)
                {
                    if (EggMoves1[specie].Count < EggMoves2[specie].Count)
                        return false;
                    int MoveDPPt = EggMoves2[specie].Moves[countmove];
                    if (!EggMoves1[specie].Moves.Any(m => m == MoveDPPt))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Transform the egg moves in raw data format into an EggMoves of array in PKHeX format
        /// </summary>
        /// <param name="Moves">raw data, only with data of species who can learn egg moves</param>
        /// <param name="Species">max number of species from the current generation</param>
        /// <returns>EggMoves array in pkhex format</returns>
        public EggMoves[] GenerateEggMoves(int[][] Moves, int Species)
        {
            EggMoves[] EggMovesData = new EggMoves[Species+1];
            int current_egg = 0;
            for (int i = 0; i < EggMovesData.Length; i++)
            {
                if (current_egg < SpeciesEggIndex.Length && i == SpeciesEggIndex[current_egg] - Diff_EggIndex_Dex)
                {
                    EggMovesData[i] = new EggMoves(Moves[current_egg]);
                    current_egg++;
                }
                else //No egg moves data for this specie
                    EggMovesData[i] = new EggMoves(new byte[0]);
            }
            return EggMovesData;
        }

        //Pack the EggMove data into an array of bytes to save to a file
        internal byte[] PackEggMovesData(string Header, EggMoves[] EggsData)
        {
            using (var s = new MemoryStream())
            using (var bw = new BinaryWriter(s))
            {
                bw.Write(Header.ToCharArray());
                bw.Write((ushort)EggsData.Length);
                uint offset = (uint)(4 + (EggsData.Length * 4) + 4);
                for (int i = 0; i < EggsData.Length; i++)
                {
                    bw.Write(offset);
                    offset += GetLength(EggsData[i]);
                }
                bw.Write(offset);
                for (int i = 0; i < EggsData.Length; i++)
                {
                    bw.Write((ushort)EggsData[i].Count);
                    for (int j = 0; j < EggsData[i].Moves.Length; j++)
                    {
                        bw.Write((ushort)EggsData[i].Moves[j]);
                    }
                }
                return s.ToArray();
            }
        }

        internal uint GetLength(EggMoves EggsData)
        {
            //For species is 2 bytes for moves count and 2 bytes per move
            return (uint)(EggsData.Moves.Length * 2 + 2);
        }

        //Compare if two array of EggMoves are equals
        internal bool CheckEqualsEggMoves(EggMoves[] EggMoves_Generated, EggMoves[] EggMoves_Readed, out string diff)
        {
            if (EggMoves_Generated.Length != EggMoves_Readed.Length)
            {
                diff = $"Different Lenght. Generated {EggMoves_Generated.Length } Readed {EggMoves_Readed.Length }";
                return false;
            }
            for (int i = 0; i < EggMoves_Generated.Length; i++)
            {
                if (EggMoves_Generated[i].Count != EggMoves_Readed[i].Count)
                {
                    diff = $"Different Lenght in element {i}. Generated {EggMoves_Generated[i].Count } Readed {EggMoves_Readed[i].Count}";
                    return false;
                }
                for (int j = 0; j < EggMoves_Generated[i].Count; j++)
                {
                    if (EggMoves_Generated[i].Moves[j] != EggMoves_Readed[i].Moves[j])
                    {
                        diff = $"Different move {j} in element {i}.";
                        diff += $" Generated {EggMoves_Generated[i].Moves[j] } Readed {EggMoves_Readed[i].Moves[j] }";
                        return false;
                    }
                }
            }

            diff = string.Empty;
            return true;
        }

        //Main function of the class
        public virtual void ExtractEggMovesData(string log_folder,string source_folder, string out_folder)
        {
            SpeciesEggIndex = SpeciesEggIndexG4;

            //Extract egg move data from NDS roms
            //The code is expected to have the roms already unpacked in the folders
            int[][] Source_EggMovesD =  ReadRawDataEggMoves(source_folder + p_filename, NDS_DP_EggMove_Offset);
            int[][] Source_EggMovesP =  ReadRawDataEggMoves(source_folder + d_filename, NDS_DP_EggMove_Offset);
            int[][] Source_EggMovesPt = ReadRawDataEggMoves(source_folder + pt_filename, NDS_Pt_EggMove_Offset);
            int[][] Source_EggMovesHG = ReadRawDataEggMoves(source_folder + hg_filename, NDS_HGSS_EggMove_Offset);
            int[][] Source_EggMovesSS = ReadRawDataEggMoves(source_folder + ss_filename, NDS_HGSS_EggMove_Offset);

            //A small check. Diamond, Pearl and Platinum should be equals. HG and SS should be equal, we end with two different set of possible egg moves
            bool DP_Equals      = ExtractUtils.CheckArrayEqual(Source_EggMovesD, Source_EggMovesP);
            bool DP_Pt_Equals   = ExtractUtils.CheckArrayEqual(Source_EggMovesD, Source_EggMovesPt);
            bool HGSS_Equals    = ExtractUtils.CheckArrayEqual(Source_EggMovesHG, Source_EggMovesSS);
            bool DP_HGSS_Equals = ExtractUtils.CheckArrayEqual(Source_EggMovesD, Source_EggMovesHG);

            //Transform egg moves data to PKHex format
            EggMoves[] EggMovesDPPt = GenerateEggMoves(Source_EggMovesD,  GData.MaxSpeciesIndexGeneration);
            EggMoves[] EggMovesHGSS = GenerateEggMoves(Source_EggMovesHG, GData.MaxSpeciesIndexGeneration);

            //Check if every egg move from HGSS include every egg move.
            bool EggHGSIncludeDPPt = CalculateEggMovesContains(EggMovesHGSS, EggMovesDPPt);
            //Its included, there is no need to calculate two different set off egg breedings
            // that means we only need the breeding data from HGSS. The EggMoves table, with egg moves for every species without fathers data
            // is enought to detect if a DPPt egg has born with a HGSS exclusive move, to analye illegal egg move combinatios we will use HGSS breeding data 

            byte[] DPPt_EggFileData = PackEggMovesData("dp", EggMovesDPPt);
            byte[] HGSS_EggFileData = PackEggMovesData("hs", EggMovesHGSS);
            
            Directory.CreateDirectory(out_folder);
            File.WriteAllBytes(out_folder + "eggmoves_dppt.pkl", DPPt_EggFileData);
            File.WriteAllBytes(out_folder + "eggmoves_hgss.pkl", HGSS_EggFileData);

            EggMoves[] DPPt_Egg_Check_Data = EggMoves.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "eggmoves_dppt.pkl"), "dp"));
            EggMoves[] HGSS_EggCheck_Data  = EggMoves.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "eggmoves_hgss.pkl"), "hs"));

            string diff_dppt = string.Empty;
            string diff_hgss = string.Empty;
            bool check_dppt = CheckEqualsEggMoves(EggMovesDPPt, DPPt_Egg_Check_Data, out diff_dppt);
            if (!check_dppt)
                throw new Exception("Check Diamond Pearl Platinum Egg Moves Data." + diff_dppt);
            bool check_hgss = CheckEqualsEggMoves(EggMovesHGSS, HGSS_EggCheck_Data, out diff_hgss);
            if (!check_hgss)
                throw new Exception("Check Hearth Gold Soul Silver Egg Moves Data." + diff_hgss);
        }

    }
}
