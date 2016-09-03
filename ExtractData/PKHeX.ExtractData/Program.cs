using System;
using System.IO;

namespace PKHeX.ExtractData
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime Start = DateTime.Now;
            string folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (Path.GetFileName(folder).ToLower() == "debug")
                folder = Directory.GetParent(folder).FullName;
            if (Path.GetFileName(folder).ToLower() == "release")
                folder = Directory.GetParent(folder).FullName;
            if (Path.GetFileName(folder).ToLower() == "bin")
                folder = Directory.GetParent(folder).FullName;
            string log_folder = folder + "\\Logs\\";
            if (!Directory.Exists(log_folder))
                Directory.CreateDirectory(log_folder);

            ExtTMHM3 TMHM3 = new ExtTMHM3();
            ExtLearnSet3 Learn3Data = new ExtLearnSet3();
            ExtMoveTutors3 MoveTutors3 = new ExtMoveTutors3();
            ExtEggMoves3 EggMoves3Data = new ExtEggMoves3();

            string gba_folder = folder + "\\GBA\\";
            string nds_folder = folder + "\\NDS\\";
            string output_folder = folder + "\\Output\\";

            TMHM3.ExtractTMHM(log_folder, gba_folder, output_folder);
            MoveTutors3.ExtractMoveTutors(log_folder, gba_folder, output_folder);
            Learn3Data.ExtractLevelUpMovesData(log_folder, gba_folder, output_folder);
            EggMoves3Data.ExtractEggMovesData(log_folder, gba_folder, output_folder);

            ExtLearnSet4 Learn4Data = new ExtLearnSet4();
            ExtEggMoves4 EggMoves4Data = new ExtEggMoves4();
            ExtMoveTutors4 MoveTutors4 = new ExtMoveTutors4();

            MoveTutors4.ExtractMoveTutors4(log_folder, nds_folder, output_folder);
            Learn4Data.ExtractLevelUpMovesData(log_folder, nds_folder, output_folder);
            EggMoves4Data.ExtractEggMovesData(log_folder, nds_folder, output_folder);

            ExtLearnSet5 Learn5Data = new ExtLearnSet5();
            Learn5Data.ExtractLevelUpMovesData(log_folder, nds_folder, output_folder);

            DateTime End = DateTime.Now;
            TimeSpan Diff = End - Start;
        }
    }
}
