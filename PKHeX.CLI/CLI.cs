using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX.CLI
{
    internal class CLI
    {
        private SaveFile SAV;
        private GameVersion game;
        private readonly List<ShowdownSet> showdownSets = new List<ShowdownSet>();
        private readonly List<PKM> ENTITIES = new List<PKM>();
        private readonly List<bool> IsValid = new List<bool>();
        /// <summary>
        /// Loading a <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="path">Path to the savefile.</param>
        public void LoadSavefile(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    string ext = fileInfo.Extension;
                    byte[] input = File.ReadAllBytes(path);
                    object obj = FileUtil.GetSupportedFile(input, ext);
                    if (obj == null)
                    {
                        throw new Exception(path + " was not found or isn't a supported file.");
                    }
                    SAV = (SaveFile)obj;
                    game = (GameVersion)SAV.Game;
                    Console.WriteLine("Savefile \"" + path + "\" successfully loaded.");
                }
            }
            catch
            {
                ShowError(path + " was not found or isn't a supported file.");
            }
        }
        /// <summary>
        /// Loading a file and creates <see cref="ShowdownSet"/>s form them.
        /// </summary>
        /// <param name="path">Path to the file containing showdown data.</param>
        public void LoadShowdown(string path)
        {
            try
            {
                string[] sets = File.ReadAllText(path).Split(new string[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.None);
                if (string.IsNullOrWhiteSpace(sets[0]))
                {
                    throw new Exception("Showdown file was invalid! Aborting loading showdown sets");
                }
                for (int i = 0; i < sets.Length; i++)
                {
                    showdownSets.Add(new ShowdownSet(sets[i]));
                    if (showdownSets[i].InvalidLines.Count() == 0)
                    {
                        Console.WriteLine("Loading showdown set for " + SpeciesName.GetSpeciesName(showdownSets[i].Species, 2));
                    }
                    else
                    {
                        showdownSets.Clear();
                        throw new Exception("Showdown file was invalid! Aborting loading showdown sets");
                    }
                }
                Console.WriteLine("Showdown file was successfully loaded.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        /// <summary>
        /// Creates <see cref="PKM"/> from the showdownsets and checks compatibility with the savefile.
        /// </summary>
        /// <remarks>REMEMBER: Many moves was removed in the 8th gen, so some showdownsets will eventually be incompatible with your savefile.</remarks>
        public void CreatePkM()
        {
            if (SAV == null)
            {
                Console.WriteLine("No savefile loaded yet.");
                return;
            }
            if (showdownSets.Count == 0)
            {
                Console.WriteLine("No showdown set loaded.");
                return;
            }
            foreach (ShowdownSet set in showdownSets)
            {
                PKM pkm = SAV.BlankPKM;
                string speciesname = SpeciesName.GetSpeciesName(set.Species, 2);
                Console.WriteLine("Creating " + speciesname);

                if (Breeding.CanHatchAsEgg(set.Species))
                {
                    EncounterEgg egg = new EncounterEgg(set.Species, set.Form, set.Level, SAV.Generation, game);
                    pkm = egg.ConvertToPKM(SAV);
                }
                else
                {
                    pkm.Species = set.Species;
                    pkm.Form = set.Form;
                    pkm.SetGender(pkm.GetSaneGender());
                    IEncounterable[] encs = EncounterMovesetGenerator.GenerateEncounter(pkm, SAV.Generation).ToArray();
                    if (encs.Length == 0)
                    {
                        // use debut generation for Pokemon that available but not catchable in current generation e.g. Meltan
                        encs = EncounterMovesetGenerator.GenerateEncounter(pkm, pkm.DebutGeneration).ToArray();
                    }
                    foreach (IEncounterable enc in encs)
                    {
                        PKM pk = enc.ConvertToPKM(SAV);
                        // not all Pokemon in database are legal in all games
                        if (new LegalityAnalysis(pk, SAV.Personal).Valid)
                        {
                            pkm = PKMConverter.ConvertToType(pk, SAV.PKMType, out _);
                            if ((pk.Generation != SAV.Generation || pk.GO || pk.GO_HOME || pk.LGPE) && pkm is IBattleVersion b)
                            {
                                b.BattleVersion = (int)game;
                            }
                            break;
                        }
                    }
                }
                pkm.Language = SAV.Language;
                pkm.ApplySetDetails(set);
                LegalityAnalysis la = new LegalityAnalysis(pkm, SAV.Personal);
                string report = la.Report();
                if (report == "Legal!")
                {
                    ENTITIES.Add(pkm);
                    IsValid.Add(true);
                }
                else
                {
                    // setting blank pkm if invalid for better indexing
                    ENTITIES.Add(SAV.BlankPKM);
                    IsValid.Add(false);
                    Console.WriteLine("Warning: " + speciesname + " is invalid!");
                    Console.WriteLine(report);
                    Console.WriteLine("Ignoring " + speciesname);
                }
            }
        }
        /// <summary>
        /// Set a chosen <see cref="PKM"/> to a box.
        /// </summary>
        /// <param name="showdownSetIndex">Index of the showdown set. index of ignored sets a invalid</param>
        /// <param name="box">Box to set the <see cref="PKM"/>.</param>
        /// <param name="slot">Slot of the chosen box to set the <see cref="PKM"/>.</param>
        /// <remarks>You can get the <paramref name="showdownSetIndex"/> by calling getshowdownsets.</remarks>
        public void SetPkm(int showdownSetIndex, int box, int slot)
        {
            string paramName = string.Empty;
            if (showdownSets[showdownSetIndex] == null || !IsValid[showdownSetIndex])
            {
                paramName = nameof(showdownSetIndex);
            }
            if (box < 0 || box > 31)
            {
                paramName = nameof(box);
            }
            if (slot < 0 || slot > 29)
            {
                paramName = nameof(slot);
            }
            try
            {
                if (paramName != string.Empty)
                {
                    throw new Exception(paramName + " is invalid");
                }
                PKM pkm = ENTITIES[showdownSetIndex];
                BoxEdit boxEdit = new BoxEdit(SAV);
                boxEdit.LoadBox(box);
                boxEdit[slot] = pkm;
                Console.WriteLine(SpeciesName.GetSpeciesName(pkm.Species, 2) + " was set to box " + box + " slot " + slot);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        /// <summary>
        /// Shows the loaded showdownsets and if they ignored.
        /// </summary>
        /// <remarks>Showdownsets with correct syntax but illegal Pokemon (e.g. Blastoise with Solar Beam) are ignored. Showdown files with syntax errors are completely discarded.</remarks>
        public void GetShowdownSets()
        {
            if (showdownSets.Count == 0)
            {
                Console.WriteLine("No showdown set loaded.");
                return;
            }
            for (int i = 0; i < showdownSets.Count; i++)
            {
                string ignored = string.Empty;
                if (IsValid.Count > 0 && !IsValid[i])
                {
                    ignored = " [ignored]";
                }
                if (i == 0)
                {
                    Console.WriteLine();
                }
                Console.WriteLine("Showdown Set " + i + ignored);
                Console.WriteLine("--------------------");
                Console.WriteLine(showdownSets[i].Text);
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Write the modified data to the savefile.
        /// </summary>
        /// <param name="path">Path where to save the savefile.</param>
        public void Save(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            ExportFlags flags = SAV.Metadata.GetSuggestedFlags(ext);
            try
            {
                File.WriteAllBytes(path, SAV.Write(flags));
                SAV.Metadata.SetExtraInfo(path);
                Console.WriteLine("Savefile was successfully written to " + path);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            Console.WriteLine("ERROR: " + msg);
        }
    }
}
