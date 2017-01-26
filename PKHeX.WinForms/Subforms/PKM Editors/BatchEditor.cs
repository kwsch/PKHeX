using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class BatchEditor : Form
    {
        public BatchEditor(PKM pk)
        {
            InitializeComponent();
            pkm = pk;
            DragDrop += tabMain_DragDrop;
            DragEnter += tabMain_DragEnter;

            CB_Format.Items.Clear();
            CB_Format.Items.Add("All");
            foreach (Type t in types) CB_Format.Items.Add(t.Name.ToLower());
            CB_Format.Items.Add("Any");

            CB_Format.SelectedIndex = CB_Require.SelectedIndex = 0;
            new ToolTip().SetToolTip(CB_Property, "Property of a given PKM to modify.");
            new ToolTip().SetToolTip(L_PropType, "PropertyType of the currently loaded PKM in the main window.");
            new ToolTip().SetToolTip(L_PropValue, "PropertyValue of the currently loaded PKM in the main window.");
        }
        private static string[][] getPropArray()
        {
            var p = new string[types.Length][];
            for (int i = 0; i < p.Length; i++)
                p[i] = ReflectUtil.getPropertiesCanWritePublic(types[i]).ToArray();

            IEnumerable<string> all = p.SelectMany(prop => prop).Distinct();
            IEnumerable<string> any = p[0];
            for (int i = 1; i < p.Length; i++)
                any = any.Union(p[i]);

            var p1 = new string[types.Length + 2][];
            Array.Copy(p, 0, p1, 1, p.Length);
            p1[0] = all.ToArray();
            p1[p1.Length-1] = any.ToArray();

            return p1;
        }

        private readonly PKM pkm;
        private const string CONST_RAND = "$rand";
        private const string CONST_SHINY = "$shiny";
        private int currentFormat = -1;
        private static readonly Type[] types = {typeof (PK7), typeof (PK6), typeof (PK5), typeof (PK4), typeof (PK3)};
        private static readonly string[][] properties = getPropArray();

        // GUI Methods
        private void B_Open_Click(object sender, EventArgs e)
        {
            if (!B_Go.Enabled) return;
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            TB_Folder.Text = fbd.SelectedPath;
            TB_Folder.Visible = true;
        }
        private void B_SAV_Click(object sender, EventArgs e)
        {
            TB_Folder.Text = "";
            TB_Folder.Visible = false;
        }
        private void B_Go_Click(object sender, EventArgs e)
        {
            if (b.IsBusy)
            { WinFormsUtil.Alert("Currently executing instruction list."); return; }

            if (RTB_Instructions.Lines.Any(line => line.Length == 0))
            { WinFormsUtil.Error("Line length error in instruction list."); return; }

            runBackgroundWorker();
        }

        private BackgroundWorker b = new BackgroundWorker { WorkerReportsProgress = true };
        private void runBackgroundWorker()
        {
            var Filters = getFilters().ToList();
            if (Filters.Any(z => string.IsNullOrWhiteSpace(z.PropertyValue)))
            { WinFormsUtil.Error("Empty Filter Value detected."); return; }

            var Instructions = getInstructions().ToList();
            var emptyVal = Instructions.Where(z => string.IsNullOrWhiteSpace(z.PropertyValue)).ToArray();
            if (emptyVal.Any())
            {
                string props = string.Join(", ", emptyVal.Select(z => z.PropertyName));
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, 
                    $"Empty Property Value{(emptyVal.Length > 1 ? "s" : "")} detected:" + Environment.NewLine + props,
                    "Continue?"))
                    return;
            }

            string destPath = "";
            if (RB_Path.Checked)
            {
                WinFormsUtil.Alert("Please select the folder where the files will be saved to.", "This can be the same folder as the source of PKM files.");
                var fbd = new FolderBrowserDialog();
                var dr = fbd.ShowDialog();
                if (dr != DialogResult.OK)
                    return;

                destPath = fbd.SelectedPath;
            }

            FLP_RB.Enabled = RTB_Instructions.Enabled = B_Go.Enabled = false;

            b = new BackgroundWorker {WorkerReportsProgress = true};
            screenStrings(Filters);
            screenStrings(Instructions);

            b.DoWork += (sender, e) => {
                if (RB_SAV.Checked)
                {
                    var data = Main.SAV.BoxData;
                    setupProgressBar(data.Length);
                    processSAV(data, Filters, Instructions);
                }
                else
                {
                    var files = Directory.GetFiles(TB_Folder.Text, "*", SearchOption.AllDirectories);
                    setupProgressBar(files.Length);
                    processFolder(files, Filters, Instructions, destPath);
                }
            };
            b.ProgressChanged += (sender, e) =>
            {
                setProgressBar(e.ProgressPercentage);
            };
            b.RunWorkerCompleted += (sender, e) => {
                string result = $"Modified {ctr}/{len} files.";
                if (err > 0)
                    result += Environment.NewLine + $"{err} files ignored due to an internal error.";
                WinFormsUtil.Alert(result);
                FLP_RB.Enabled = RTB_Instructions.Enabled = B_Go.Enabled = true;
                setupProgressBar(0);
            };
            b.RunWorkerAsync();
        }

        // Progress Bar
        private void setupProgressBar(int count)
        {
            MethodInvoker mi = () => { PB_Show.Minimum = 0; PB_Show.Step = 1; PB_Show.Value = 0; PB_Show.Maximum = count; };
            if (PB_Show.InvokeRequired)
                PB_Show.Invoke(mi);
            else
                mi.Invoke();
        }
        private void setProgressBar(int i)
        {
            if (PB_Show.InvokeRequired)
                PB_Show.Invoke((MethodInvoker)(() => PB_Show.Value = i));
            else { PB_Show.Value = i; }
        }
        
        // Mass Editing
        private int ctr, len, err;
        private IEnumerable<StringInstruction> getFilters()
        {
            var raw =
                RTB_Instructions.Lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Where(line => new[] {'!','='}.Contains(line[0]));

            return from line in raw
                   let eval = line[0] == '='
                   let split = line.Substring(1).Split('=')
                   where split.Length == 2 && !string.IsNullOrWhiteSpace(split[0])
                   select new StringInstruction {PropertyName = split[0], PropertyValue = split[1], Evaluator = eval};
        }
        private IEnumerable<StringInstruction> getInstructions()
        {
            var raw =
                RTB_Instructions.Lines
                    .Where(line => !string.IsNullOrEmpty(line))
                    .Where(line => new[] {'.'}.Contains(line[0]))
                    .Select(line => line.Substring(1));

            return from line in raw
                   select line.Split('=') into split
                   where split.Length == 2
                   select new StringInstruction { PropertyName = split[0], PropertyValue = split[1] };
        }
        private void processSAV(PKM[] data, List<StringInstruction> Filters, List<StringInstruction> Instructions)
        {
            len = err = ctr = 0;
            for (int i = 0; i < data.Length; i++)
            {
                var pkm = data[i];
                if (!pkm.Valid || pkm.Locked)
                {
                    b.ReportProgress(i);
                    continue;
                }

                ModifyResult r = ProcessPKM(pkm, Filters, Instructions);
                if (r != ModifyResult.Invalid)
                    len++;
                if (r == ModifyResult.Error)
                    err++;
                if (r == ModifyResult.Modified)
                {
                    if (pkm.Species != 0)
                        pkm.RefreshChecksum();
                    ctr++;
                }

                b.ReportProgress(i);
            }

            Main.SAV.BoxData = data;
        }
        private void processFolder(string[] files, List<StringInstruction> Filters, List<StringInstruction> Instructions, string destPath)
        {
            len = err = ctr = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if (!PKX.getIsPKM(new FileInfo(file).Length))
                {
                    b.ReportProgress(i);
                    continue;
                }

                byte[] data = File.ReadAllBytes(file);
                var pkm = PKMConverter.getPKMfromBytes(data);
                
                if (!pkm.Valid)
                {
                    b.ReportProgress(i);
                    continue;
                }

                ModifyResult r = ProcessPKM(pkm, Filters, Instructions);
                if (r != ModifyResult.Invalid)
                    len++;
                if (r == ModifyResult.Error)
                    err++;
                if (r == ModifyResult.Modified)
                {
                    if (pkm.Species > 0)
                    {
                        pkm.RefreshChecksum();
                        File.WriteAllBytes(Path.Combine(destPath, Path.GetFileName(file)), pkm.DecryptedBoxData);
                        ctr++;
                    }
                }

                b.ReportProgress(i);
            }
        }
        public static void screenStrings(IEnumerable<StringInstruction> il)
        {
            foreach (var i in il.Where(i => !i.PropertyValue.All(char.IsDigit)))
            {
                string pv = i.PropertyValue;
                if (pv.StartsWith("$") && pv.Contains(','))
                {
                    string str = pv.Substring(1);
                    var split = str.Split(',');
                    int.TryParse(split[0], out i.Min);
                    int.TryParse(split[1], out i.Max);

                    if (i.Min == i.Max)
                    {
                        i.PropertyValue = i.Min.ToString();
                        Console.WriteLine(i.PropertyName + " randomization range Min/Max same?");
                    }
                    else
                        i.Random = true;
                }

                switch (i.PropertyName)
                {
                    case nameof(PKM.Species): i.setScreenedValue(GameInfo.Strings.specieslist); continue;
                    case nameof(PKM.HeldItem): i.setScreenedValue(GameInfo.Strings.itemlist); continue;
                    case nameof(PKM.Ability): i.setScreenedValue(GameInfo.Strings.abilitylist); continue;
                    case nameof(PKM.Nature): i.setScreenedValue(GameInfo.Strings.natures); continue;
                    case nameof(PKM.Ball): i.setScreenedValue(GameInfo.Strings.balllist); continue;
                    case nameof(PKM.Move1):
                    case nameof(PKM.Move2):
                    case nameof(PKM.Move3):
                    case nameof(PKM.Move4):
                    case nameof(PKM.RelearnMove1):
                    case nameof(PKM.RelearnMove2):
                    case nameof(PKM.RelearnMove3):
                    case nameof(PKM.RelearnMove4):
                        i.setScreenedValue(GameInfo.Strings.movelist); continue;
                }
            }
        }
        
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (!Directory.Exists(files[0])) return;

            TB_Folder.Text = files[0];
            TB_Folder.Visible = true;
            RB_SAV.Checked = false;
            RB_Path.Checked = true;
        }
        private void CB_Property_SelectedIndexChanged(object sender, EventArgs e)
        {
            L_PropType.Text = getPropertyType(CB_Property.Text);
            L_PropValue.Text = pkm.GetType().HasProperty(CB_Property.Text)
                ? ReflectUtil.GetValue(pkm, CB_Property.Text).ToString()
                : "";
        }
        private string getPropertyType(string propertyName)
        {
            int typeIndex = CB_Format.SelectedIndex;
            
            if (typeIndex == 0) // All
                return types[0].GetProperty(propertyName).PropertyType.Name;

            if (typeIndex == properties.Length - 1) // Any
                foreach (var p in types.Select(t => t.GetProperty(propertyName)).Where(p => p != null))
                    return p.PropertyType.Name;
            
            return types[typeIndex - 1].GetProperty(propertyName).PropertyType.Name;
        }

        // Utility Methods
        public class StringInstruction
        {
            public string PropertyName;
            public string PropertyValue;
            public bool Evaluator;
            public void setScreenedValue(string[] arr)
            {
                int index = Array.IndexOf(arr, PropertyValue);
                PropertyValue = index > -1 ? index.ToString() : PropertyValue;
            }

            // Extra Functionality
            public bool Random;
            public int Min, Max;
            public int RandomValue => Util.rand.Next(Min, Max + 1);
        }
        private enum ModifyResult
        {
            Invalid,
            Error,
            Filtered,
            Modified,
        }
        private static ModifyResult ProcessPKM(PKM PKM, IEnumerable<StringInstruction> Filters, IEnumerable<StringInstruction> Instructions)
        {
            if (!PKM.ChecksumValid || PKM.Species == 0)
                return ModifyResult.Invalid;

            Type pkm = PKM.GetType();

            foreach (var cmd in Filters)
            {
                try
                {
                    if (!pkm.HasProperty(cmd.PropertyName))
                        return ModifyResult.Filtered;
                    if (ReflectUtil.GetValueEquals(PKM, cmd.PropertyName, cmd.PropertyValue) != cmd.Evaluator)
                        return ModifyResult.Filtered;
                }
                catch
                {
                    Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}.");
                    return ModifyResult.Filtered;
                }
            }

            ModifyResult result = ModifyResult.Error;
            foreach (var cmd in Instructions)
            {
                try
                {
                    if (cmd.PropertyName == nameof(PKM.MetDate))
                        PKM.MetDate = DateTime.ParseExact(cmd.PropertyValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    else if (cmd.PropertyName == nameof(PKM.EggMetDate))
                        PKM.EggMetDate = DateTime.ParseExact(cmd.PropertyValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == CONST_RAND)
                        ReflectUtil.SetValue(PKM, cmd.PropertyName, Util.rnd32().ToString());
                    else if ((cmd.PropertyName == nameof(PKM.Ability) || cmd.PropertyName == nameof(PKM.AbilityNumber)) && cmd.PropertyValue.StartsWith("$"))
                        PKM.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30);
                    else if(cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_RAND)
                        PKM.setPIDGender(PKM.Gender);
                    else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == nameof(PKM.PID))
                        PKM.EncryptionConstant = PKM.PID;
                    else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_SHINY)
                        PKM.setShinyPID();
                    else if (cmd.PropertyName == nameof(PKM.Species) && cmd.PropertyValue == "0")
                        PKM.Data = new byte[PKM.Data.Length];
                    else if (cmd.PropertyName.StartsWith("IV") && cmd.PropertyValue == CONST_RAND)
                        setRandomIVs(PKM, cmd);
                    else if (cmd.Random)
                        ReflectUtil.SetValue(PKM, cmd.PropertyName, cmd.RandomValue);
                    else
                        ReflectUtil.SetValue(PKM, cmd.PropertyName, cmd.PropertyValue);

                    result = ModifyResult.Modified;
                }
                catch { Console.WriteLine($"Unable to set {cmd.PropertyName} to {cmd.PropertyValue}."); }
            }
            return result;
        }
        private static void setRandomIVs(PKM PKM, StringInstruction cmd)
        {
            int MaxIV = PKM.Format <= 2 ? 15 : 31;
            if (cmd.PropertyName == "IVs")
            {
                bool IV3 = Legal.Legends.Contains(PKM.Species) || Legal.SubLegends.Contains(PKM.Species);
                int[] IVs = new int[6];
                do
                {
                    for (int i = 0; i < 6; i++)
                        IVs[i] = (int)(Util.rnd32() & MaxIV);
                } while (IV3 && IVs.Where(i => i == MaxIV).Count() < 3);
                ReflectUtil.SetValue(PKM, cmd.PropertyName, IVs);
            }
            else
                ReflectUtil.SetValue(PKM, cmd.PropertyName, Util.rnd32() & MaxIV);
        }

        private void B_Add_Click(object sender, EventArgs e)
        {
            if (CB_Property.SelectedIndex < 0)
            { WinFormsUtil.Alert("Invalid property selected."); return; }

            char[] prefix = {'.', '=', '!'};
            string s = prefix[CB_Require.SelectedIndex] + CB_Property.Items[CB_Property.SelectedIndex].ToString() + "=";
            if (RTB_Instructions.Lines.Length != 0 && RTB_Instructions.Lines.Last().Length > 0)
                s = Environment.NewLine + s;

            RTB_Instructions.AppendText(s);
        }

        private void CB_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentFormat == CB_Format.SelectedIndex)
                return;

            int format = CB_Format.SelectedIndex;
            CB_Property.Items.Clear();
            CB_Property.Items.AddRange(properties[format]);
            CB_Property.SelectedIndex = 0;
            currentFormat = format;
        }
    }
}
