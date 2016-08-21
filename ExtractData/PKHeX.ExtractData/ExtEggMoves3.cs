using System;
using System.Linq;
using System.IO;

namespace PKHeX.ExtractData
{
    class ExtEggMoves3 : ExtEggMoves4
    {
        internal override GenerationData GData => Generation3Data.Create();
        static ushort[] SpeciesEggIndexG3 => new ushort[]
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
            0x4E40, //NIDORAN (M)
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
            0x4F93  //BAGON
        };

        private static void PrepareSpeciesEggIndex()
        {
            SpeciesEggIndex = SpeciesEggIndexG3; 
            //Initially contains the values corresponding to dex number + 4F1C, should be translated to internal index number + 4F1C

            for (int i=0;i<SpeciesEggIndex.Length;i++)
            {
                if (SpeciesEggIndex[i] >= 0x4F1C)//treeko
                    SpeciesEggIndex[i] = (ushort)(TransformSpeciesIndex.getG3Species(SpeciesEggIndex[i] - Diff_EggIndex_Dex) + Diff_EggIndex_Dex);
            }

            SpeciesEggIndex = SpeciesEggIndex.OrderBy(x => x).ToArray();
        }

        const string gba_firered_rom = "Pokemon - FireRed Version (USA).gba";
        const string gba_leafgreen_rom = "Pokemon - LeafGreen Version (USA).gba";
        const string gba_emerald_rom = "Pokemon - Emerald Version (USA, Europe).gba";
        const string gba_ruby_rom = "Pokemon - Ruby Version (USA).gba";
        const string gba_sapphire_rom = "Pokemon - Sapphire Version (USA).gba";

        const int GBA_E_EggMove_Offset  = 0x32ADD8;
        const int GBA_R_EggMove_Offset  = 0x2091DC;
        const int GBA_S_EggMove_Offset  = 0x20916C;
        const int GBA_FR_EggMove_Offset = 0x25EF0C;
        const int GBA_LG_EggMove_Offset = 0x25EEEC;

        public override void ExtractEggMovesData(string log_folder, string source_folder, string out_folder)
        {
            PrepareSpeciesEggIndex();

            //Extract egg move data from NDS roms
            //The code is expected to have the roms already unpacked in the folders
            int[][] Source_EggMovesE  = ReadRawDataEggMoves(source_folder + gba_emerald_rom,   GBA_E_EggMove_Offset);
            int[][] Source_EggMovesFR = ReadRawDataEggMoves(source_folder + gba_firered_rom,   GBA_FR_EggMove_Offset);
            int[][] Source_EggMovesR  = ReadRawDataEggMoves(source_folder + gba_ruby_rom,      GBA_R_EggMove_Offset);
            int[][] Source_EggMovesS  = ReadRawDataEggMoves(source_folder + gba_sapphire_rom,  GBA_S_EggMove_Offset);
            int[][] Source_EggMovesLG = ReadRawDataEggMoves(source_folder + gba_leafgreen_rom, GBA_LG_EggMove_Offset);

            //A small check. Egg moves are the same among all 5 gba games
            bool RS_Equals    = ExtractUtils.CheckArrayEqual(Source_EggMovesR, Source_EggMovesS);
            bool RS_E_Equals  = ExtractUtils.CheckArrayEqual(Source_EggMovesR, Source_EggMovesE);
            bool FR_E_Equals  = ExtractUtils.CheckArrayEqual(Source_EggMovesFR, Source_EggMovesE);
            bool FR_LG_Equals = ExtractUtils.CheckArrayEqual(Source_EggMovesFR, Source_EggMovesLG);

            //Transform egg moves data to PKHex format
            EggMoves[] EggMovesE    = GenerateEggMoves(Source_EggMovesE , GData.MaxSpeciesIndexGeneration);
            EggMovesE = GData.TransformDexOrder(EggMovesE);

            byte[] E_EggFileData = PackEggMovesData("g3", EggMovesE);

            Directory.CreateDirectory(out_folder);
            File.WriteAllBytes(out_folder + "eggmoves_g3.pkl", E_EggFileData);

            EggMoves[] E_Egg_Check_Data = EggMoves.getArray(ExtractUtils.unpackMini(File.ReadAllBytes(out_folder + "eggmoves_g3.pkl"), "g3"));
            string diff_g3 = string.Empty;
            bool check_g3 = CheckEqualsEggMoves(EggMovesE, E_Egg_Check_Data, out diff_g3);
            if (!check_g3)
                throw new Exception("Check Generation 3 Egg Moves Data." + diff_g3);
        }
    }
}
