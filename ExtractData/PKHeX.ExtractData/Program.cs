using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PKHeX.ExtractData
{
    class Program
    {
        static void Main(string[] args)
        {
            string folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (Path.GetFileName(folder).ToLower() == "debug")
                folder = Directory.GetParent(folder).FullName;
            if (Path.GetFileName(folder).ToLower() == "release")
                folder = Directory.GetParent(folder).FullName;
            if (Path.GetFileName(folder).ToLower() == "bin")
                folder = Directory.GetParent(folder).FullName;
            if (!Directory.Exists(folder)) return;
            if (!Directory.Exists(folder + "\\Logs\\")) Directory.CreateDirectory(folder + "\\Logs\\");

            ExtTMHM3 TMHM3 = new ExtTMHM3();
            ExtLearnSet3 Learn3Data = new ExtLearnSet3();
            ExtMoveTutors3 MoveTutors3 = new ExtMoveTutors3();
            ExtEggMoves3 EggMoves3Data = new ExtEggMoves3();
            
            TMHM3.ExtractTMHM(folder + "\\Logs\\", folder + "\\GBA\\", folder + "\\Output\\");
            MoveTutors3.ExtractMoveTutors(folder + "\\Logs\\", folder + "\\GBA\\", folder + "\\Output\\");
            Learn3Data.ExtractLevelUpMovesData(folder + "\\Logs\\", folder + "\\GBA\\", folder + "\\Output\\");
            EggMoves3Data.ExtractEggMovesData(folder + "\\Logs\\", folder + "\\GBA\\", folder + "\\Output\\");

            ExtLearnSet4 Learn4Data = new ExtLearnSet4();
            ExtEggMoves4 EggMoves4Data = new ExtEggMoves4();
            ExtMoveTutors4 MoveTutors4 = new ExtMoveTutors4();

            MoveTutors4.ExtractMoveTutors4(folder + "\\Logs\\", folder + "\\NDS\\", folder + "\\Output\\");
            Learn4Data.ExtractLevelUpMovesData(folder + "\\Logs\\", folder + "\\NDS\\", folder + "\\Output\\");
            EggMoves4Data.ExtractEggMovesData(folder + "\\Logs\\", folder + "\\NDS\\", folder + "\\Output\\");

            ExtLearnSet5 Learn5Data = new ExtLearnSet5();
            Learn5Data.ExtractLevelUpMovesData(folder + "\\Logs\\", folder + "\\NDS\\", folder + "\\Output\\");
        }
    }
}
