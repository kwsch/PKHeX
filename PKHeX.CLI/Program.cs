using System;

namespace PKHeX.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CLI cli = new CLI();
            foreach (string arg in args)
            {
                if (arg.Contains("-savefile:"))
                {
                    cli.LoadSavefile(arg.Substring(10));
                }
                else if (arg.Contains("-showdown:"))
                {
                    cli.LoadShowdown(arg.Substring(10));
                }
            }
            do
            {
                string consoleInput = Console.ReadLine();
                string[] cmd = consoleInput.Split(' ');
                if (consoleInput == "exit")
                {
                    break;
                }
                try
                {
                    switch (cmd[0])
                    {
                        case "loadsavefile":
                            cli.LoadSavefile(cmd[1]);
                            break;
                        case "loadshowdown":
                            cli.LoadShowdown(cmd[1]);
                            break;
                        case "createpkm":
                            cli.CreatePkM();
                            break;
                        case "setpkm":
                            cli.SetPkm(int.Parse(cmd[1]), int.Parse(cmd[1]), int.Parse(cmd[2]));
                            break;
                        case "getshowdownsets":
                            cli.GetShowdownSets();
                            break;
                        case "save":
                            cli.Save(cmd[1]);
                            break;
                        default:
                            Console.WriteLine("unknown command");
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine(cmd[0] + " has missing arguments");
                }
            }
            while (true);
        }
    }
}
