using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using System.Reflection;

namespace PKHeX.WinForms
{
    public partial class BatchEditor : Form
    {
        private readonly SaveFile SAV;
        public BatchEditor(PKM pk, SaveFile sav)
        {
            pkmref = pk;
            SAV = sav;
            InitializeComponent();
            DragDrop += TabMain_DragDrop;
            DragEnter += TabMain_DragEnter;

            CB_Format.Items.Clear();
            CB_Format.Items.Add("Any");
            foreach (Type t in types) CB_Format.Items.Add(t.Name.ToLower());
            CB_Format.Items.Add("All");

            CB_Format.SelectedIndex = CB_Require.SelectedIndex = 0;
            new ToolTip().SetToolTip(CB_Property, "Property of a given PKM to modify.");
            new ToolTip().SetToolTip(L_PropType, "PropertyType of the currently loaded PKM in the main window.");
            new ToolTip().SetToolTip(L_PropValue, "PropertyValue of the currently loaded PKM in the main window.");
        }
        private static string[][] GetPropArray()
        {
            var p = new string[types.Length][];
            for (int i = 0; i < p.Length; i++)
                p[i] = ReflectFrameworkUtil.GetPropertiesCanWritePublicDeclared(types[i]).Concat(CustomProperties).OrderBy(a => a).ToArray();

            // Properties for any PKM
            var any = ReflectFrameworkUtil.GetPropertiesCanWritePublic(typeof(PK1)).Concat(p.SelectMany(a => a)).Distinct().OrderBy(a => a).ToArray();
            // Properties shared by all PKM
            var all = p.Aggregate(new HashSet<string>(p.First()), (h, e) => { h.IntersectWith(e); return h; }).OrderBy(a => a).ToArray();

            var p1 = new string[types.Length + 2][];
            Array.Copy(p, 0, p1, 1, p.Length);
            p1[0] = any;
            p1[p1.Length - 1] = all;

            return p1;
        }

        private readonly PKM pkmref;
        private const string CONST_RAND = "$rand";
        private const string CONST_SHINY = "$shiny";
        private const string CONST_SUGGEST = "$suggest";

        private const string PROP_LEGAL = "Legal";
        private static readonly string[] CustomProperties = {PROP_LEGAL};

        private int currentFormat = -1;
        private static readonly Type[] types = 
        {
            typeof (PK7), typeof (PK6), typeof (PK5), typeof (PK4), typeof(BK4),
            typeof (PK3), typeof (XK3), typeof (CK3),
            typeof (PK2), typeof (PK1),
        };
        private static readonly string[][] properties = GetPropArray();

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

            RunBackgroundWorker();
        }
        private void B_Add_Click(object sender, EventArgs e)
        {
            if (CB_Property.SelectedIndex < 0)
            { WinFormsUtil.Alert("Invalid property selected."); return; }

            char[] prefix = { '.', '=', '!' };
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
        private void CB_Property_SelectedIndexChanged(object sender, EventArgs e)
        {
            L_PropType.Text = GetPropertyType(CB_Property.Text);
            if (pkmref.GetType().HasProperty(CB_Property.Text))
            {
                L_PropValue.Text = ReflectFrameworkUtil.GetValue(pkmref, CB_Property.Text).ToString();
                L_PropType.ForeColor = L_PropValue.ForeColor; // reset color
            }
            else // no property, flag
            {
                L_PropValue.Text = string.Empty;
                L_PropType.ForeColor = Color.Red;
            }
        }
        private static void TabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        private void TabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (!Directory.Exists(files[0]))
                return;

            TB_Folder.Text = files[0];
            TB_Folder.Visible = true;
            RB_SAV.Checked = false;
            RB_Path.Checked = true;
        }

        private BackgroundWorker b = new BackgroundWorker { WorkerReportsProgress = true };
        private void RunBackgroundWorker()
        {
            var Filters = StringInstruction.GetFilters(RTB_Instructions.Lines).ToArray();
            if (Filters.Any(z => string.IsNullOrWhiteSpace(z.PropertyValue)))
            { WinFormsUtil.Error("Empty Filter Value detected."); return; }

            var Instructions = StringInstruction.GetInstructions(RTB_Instructions.Lines).ToArray();
            var emptyVal = Instructions.Where(z => string.IsNullOrWhiteSpace(z.PropertyValue)).ToArray();
            if (emptyVal.Any())
            {
                string props = string.Join(", ", emptyVal.Select(z => z.PropertyName));
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, 
                    $"Empty Property Value{(emptyVal.Length > 1 ? "s" : "")} detected:" + Environment.NewLine + props,
                    "Continue?"))
                    return;
            }

            if (!Instructions.Any())
            { WinFormsUtil.Error("No instructions defined."); return; }

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
            ScreenStrings(Filters);
            ScreenStrings(Instructions);

            b.DoWork += (sender, e) => {

                len = err = ctr = 0;
                if (RB_SAV.Checked)
                {
                    if (SAV.HasParty)
                    {
                        var data = SAV.PartyData;
                        SetupProgressBar(data.Length);
                        ProcessSAV(data, Filters, Instructions);
                        SAV.PartyData = data;
                    }
                    if (SAV.HasBox)
                    {
                        var data = SAV.BoxData;
                        SetupProgressBar(data.Length);
                        ProcessSAV(data, Filters, Instructions);
                        SAV.BoxData = data;
                    }
                }
                else
                {
                    var files = Directory.GetFiles(TB_Folder.Text, "*", SearchOption.AllDirectories);
                    SetupProgressBar(files.Length);
                    ProcessFolder(files, Filters, Instructions, destPath);
                }
            };
            b.ProgressChanged += (sender, e) =>
            {
                SetProgressBar(e.ProgressPercentage);
            };
            b.RunWorkerCompleted += (sender, e) => {
                string result = $"Modified {ctr}/{len} files.";
                if (err > 0)
                    result += Environment.NewLine + $"{err} files ignored due to an internal error.";
                WinFormsUtil.Alert(result);
                FLP_RB.Enabled = RTB_Instructions.Enabled = B_Go.Enabled = true;
                SetupProgressBar(0);
            };
            b.RunWorkerAsync();
        }

        // Progress Bar
        private void SetupProgressBar(int count)
        {
            MethodInvoker mi = () => { PB_Show.Minimum = 0; PB_Show.Step = 1; PB_Show.Value = 0; PB_Show.Maximum = count; };
            if (PB_Show.InvokeRequired)
                PB_Show.Invoke(mi);
            else
                mi.Invoke();
        }
        private void SetProgressBar(int i)
        {
            if (PB_Show.InvokeRequired)
                PB_Show.Invoke((MethodInvoker)(() => PB_Show.Value = i));
            else { PB_Show.Value = i; }
        }
        
        // Mass Editing
        private int ctr, len, err;
        private void ProcessSAV(PKM[] data, StringInstruction[] Filters, StringInstruction[] Instructions)
        {
            for (int i = 0; i < data.Length; i++)
            {
                ProcessPKM(data[i], Filters, Instructions);
                b.ReportProgress(i);
            }
        }
        private void ProcessFolder(string[] files, StringInstruction[] Filters, StringInstruction[] Instructions, string destPath)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                var fi = new FileInfo(file);
                if (!PKX.IsPKM(fi.Length))
                {
                    b.ReportProgress(i);
                    continue;
                }

                int format = fi.Extension.Length > 0 ? (fi.Extension.Last() - 0x30) & 7 : SAV.Generation;
                byte[] data = File.ReadAllBytes(file);
                var pkm = PKMConverter.GetPKMfromBytes(data, prefer: format);
                if (ProcessPKM(pkm, Filters, Instructions))
                    File.WriteAllBytes(Path.Combine(destPath, Path.GetFileName(file)), pkm.DecryptedBoxData);

                b.ReportProgress(i);
            }
        }
        private bool ProcessPKM(PKM pkm, IEnumerable<StringInstruction> Filters, IEnumerable<StringInstruction> Instructions)
        {
            if (!pkm.Valid || pkm.Locked)
            {
                len++;
                Console.WriteLine("Skipped a pkm due to disallowed input: " + (pkm.Locked ? "Locked." : "Not Valid."));
                return false;
            }

            ModifyResult r = TryModifyPKM(pkm, Filters, Instructions);
            if (r != ModifyResult.Invalid)
                len++;
            if (r == ModifyResult.Error)
                err++;
            if (r != ModifyResult.Modified)
                return false;
            if (pkm.Species <= 0)
                return false;

            pkm.RefreshChecksum();
            ctr++;
            return true;
        }
        
        private string GetPropertyType(string propertyName)
        {
            if (CustomProperties.Contains(propertyName))
                return "Custom";

            int typeIndex = CB_Format.SelectedIndex;
            
            if (typeIndex == properties.Length - 1) // All
                return types[0].GetProperty(propertyName).PropertyType.Name;

            if (typeIndex == 0) // Any
                foreach (var p in types.Select(t => t.GetProperty(propertyName)).Where(p => p != null))
                    return p.PropertyType.Name;
            
            return types[typeIndex - 1].GetProperty(propertyName).PropertyType.Name;
        }

        // Utility Methods
        private enum ModifyResult
        {
            Invalid,
            Error,
            Filtered,
            Modified,
        }
        public class StringInstruction
        {
            public string PropertyName { get; set; }
            public string PropertyValue { get; set; }
            public bool Evaluator { get; set; }
            public void SetScreenedValue(string[] arr)
            {
                int index = Array.IndexOf(arr, PropertyValue);
                PropertyValue = index > -1 ? index.ToString() : PropertyValue;
            }

            // Extra Functionality
            private int Min, Max;
            public bool Random { get; private set; }
            public int RandomValue => Util.Rand.Next(Min, Max + 1);
            public void SetRandRange(string pv)
            {
                string str = pv.Substring(1);
                var split = str.Split(',');
                int.TryParse(split[0], out Min);
                int.TryParse(split[1], out Max);

                if (Min == Max)
                {
                    PropertyValue = Min.ToString();
                    Console.WriteLine(PropertyName + " randomization range Min/Max same?");
                }
                else
                    Random = true;
            }

            public static IEnumerable<StringInstruction> GetFilters(IEnumerable<string> lines)
            {
                var raw = GetRelevantStrings(lines, new[] { '!', '=' });
                return from line in raw
                    let eval = line[0] == '='
                    let split = line.Substring(1).Split('=')
                    where split.Length == 2 && !string.IsNullOrWhiteSpace(split[0])
                    select new StringInstruction { PropertyName = split[0], PropertyValue = split[1], Evaluator = eval };
            }
            public static IEnumerable<StringInstruction> GetInstructions(IEnumerable<string> lines)
            {
                var raw = GetRelevantStrings(lines, new[] { '.' }).Select(line => line.Substring(1));
                return from line in raw
                    select line.Split('=') into split
                    where split.Length == 2
                    select new StringInstruction { PropertyName = split[0], PropertyValue = split[1] };
            }
            private static IEnumerable<string> GetRelevantStrings(IEnumerable<string> lines, IEnumerable<char> pieces)
            {
                return lines
                    .Where(line => !string.IsNullOrEmpty(line))
                    .Where(line => pieces.Any(z => z == line[0]));
            }
        }
        private sealed class PKMInfo
        {
            internal PKM pkm { get; }
            internal PKMInfo(PKM pk) { pkm = pk; }

            private LegalityAnalysis la;
            private LegalityAnalysis Legality => la ?? (la = new LegalityAnalysis(pkm));

            internal bool Legal => Legality.Valid;
            internal int[] SuggestedRelearn => Legality.GetSuggestedRelearn();
            internal int[] SuggestedMoves => Legality.GetSuggestedMoves(tm: true, tutor: true, reminder: false);
            internal EncounterStatic SuggestedEncounter => Legality.GetSuggestedMetInfo();
        }

        public static void ScreenStrings(IEnumerable<StringInstruction> il)
        {
            foreach (var i in il.Where(i => !i.PropertyValue.All(char.IsDigit)))
            {
                string pv = i.PropertyValue;
                if (pv.StartsWith("$") && pv.Contains(','))
                    i.SetRandRange(pv);

                SetInstructionScreenedValue(i);
            }
        }
        private static void SetInstructionScreenedValue(StringInstruction i)
        {
            switch (i.PropertyName)
            {
                case nameof(PKM.Species): i.SetScreenedValue(GameInfo.Strings.specieslist); return;
                case nameof(PKM.HeldItem): i.SetScreenedValue(GameInfo.Strings.itemlist); return;
                case nameof(PKM.Ability): i.SetScreenedValue(GameInfo.Strings.abilitylist); return;
                case nameof(PKM.Nature): i.SetScreenedValue(GameInfo.Strings.natures); return;
                case nameof(PKM.Ball): i.SetScreenedValue(GameInfo.Strings.balllist); return;
                case nameof(PKM.Move1):
                case nameof(PKM.Move2):
                case nameof(PKM.Move3):
                case nameof(PKM.Move4):
                case nameof(PKM.RelearnMove1):
                case nameof(PKM.RelearnMove2):
                case nameof(PKM.RelearnMove3):
                case nameof(PKM.RelearnMove4):
                    i.SetScreenedValue(GameInfo.Strings.movelist); return;
            }
        }
        private static ModifyResult TryModifyPKM(PKM PKM, IEnumerable<StringInstruction> Filters, IEnumerable<StringInstruction> Instructions)
        {
            if (!PKM.ChecksumValid || PKM.Species == 0)
                return ModifyResult.Invalid;

            Type pkm = PKM.GetType();
            PKMInfo info = new PKMInfo(PKM);

            ModifyResult result = ModifyResult.Error;
            foreach (var cmd in Filters)
            {
                try
                {
                    if (IsPKMFiltered(pkm, cmd, info, out result))
                        return result; // why it was filtered out
                }
                catch { Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}."); }
            }

            foreach (var cmd in Instructions)
            {
                try
                {
                    result = SetPKMProperty(PKM, info, cmd);
                }
                catch { Console.WriteLine($"Unable to set {cmd.PropertyName} to {cmd.PropertyValue}."); }
            }
            return result;
        }
        private static ModifyResult SetPKMProperty(PKM PKM, PKMInfo info, StringInstruction cmd)
        {
            if (cmd.PropertyValue == CONST_SUGGEST)
                return SetSuggestedPKMProperty(PKM, cmd, info)
                    ? ModifyResult.Modified
                    : ModifyResult.Error;

            SetProperty(PKM, cmd);
            return ModifyResult.Modified;
        }
        private static bool IsPKMFiltered(Type pkm, StringInstruction cmd, PKMInfo info, out ModifyResult result)
        {
            result = ModifyResult.Error;
            if (cmd.PropertyName == PROP_LEGAL)
            {
                if (!bool.TryParse(cmd.PropertyValue, out bool legal))
                    return true;
                if (legal == info.Legal == cmd.Evaluator)
                    return false;
                result = ModifyResult.Filtered;
                return true;
            }
            if (!pkm.HasPropertyAll(cmd.PropertyName) 
                || pkm.IsValueEqual(info.pkm, cmd.PropertyName, cmd.PropertyValue) != cmd.Evaluator)
            {
                result = ModifyResult.Filtered;
                return true;
            }
            return false;
        }
        private static bool SetSuggestedPKMProperty(PKM PKM, StringInstruction cmd, PKMInfo info)
        {
            switch (cmd.PropertyName)
            {
                case nameof(PKM.RelearnMoves):
                    PKM.RelearnMoves = info.SuggestedRelearn;
                    return true;
                case nameof(PKM.Met_Location):
                    var encounter = info.SuggestedEncounter;
                    if (encounter == null)
                        return false;

                    int level = encounter.Level;
                    int location = encounter.Location;
                    int minlvl = Legal.GetLowestLevel(PKM, encounter.Species);

                    PKM.Met_Level = level;
                    PKM.Met_Location = location;
                    PKM.CurrentLevel = Math.Max(minlvl, level);

                    return true;

                case nameof(PKM.Moves):
                    var moves = info.SuggestedMoves;
                    Util.Shuffle(moves);
                    Array.Resize(ref moves, 4);
                    PKM.Moves = moves;
                    return true;

                default:
                    return false;
            }
        }
        private static void SetProperty(PKM PKM, StringInstruction cmd)
        {
            if (cmd.PropertyName == nameof(PKM.MetDate))
                PKM.MetDate = DateTime.ParseExact(cmd.PropertyValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            else if (cmd.PropertyName == nameof(PKM.EggMetDate))
                PKM.EggMetDate = DateTime.ParseExact(cmd.PropertyValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == CONST_RAND)
                ReflectFrameworkUtil.SetValue(PKM, cmd.PropertyName, Util.Rand32().ToString());
            else if ((cmd.PropertyName == nameof(PKM.Ability) || cmd.PropertyName == nameof(PKM.AbilityNumber)) && cmd.PropertyValue.StartsWith("$"))
                PKM.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30);
            else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_RAND)
                PKM.SetPIDGender(PKM.Gender);
            else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == nameof(PKM.PID))
                PKM.EncryptionConstant = PKM.PID;
            else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_SHINY)
                PKM.SetShinyPID();
            else if (cmd.PropertyName == nameof(PKM.Species) && cmd.PropertyValue == "0")
                PKM.Data = new byte[PKM.Data.Length];
            else if (cmd.PropertyName.StartsWith("IV") && cmd.PropertyValue == CONST_RAND)
                SetRandomIVs(PKM, cmd);
            else if (cmd.Random)
                ReflectFrameworkUtil.SetValue(PKM, cmd.PropertyName, cmd.RandomValue);
            else if (cmd.PropertyName == nameof(PKM.IsNicknamed) && cmd.PropertyValue.ToLower() == "false")
            { PKM.IsNicknamed = false; PKM.Nickname = PKX.GetSpeciesName(PKM.Species, PKM.Language); }
            else
                ReflectFrameworkUtil.SetValue(PKM, cmd.PropertyName, cmd.PropertyValue);
        }
        private static void SetRandomIVs(PKM PKM, StringInstruction cmd)
        {
            int MaxIV = PKM.Format <= 2 ? 15 : 31;
            if (cmd.PropertyName == "IVs")
            {
                int[] IVs = new int[6];

                for (int i = 0; i < 6; i++)
                    IVs[i] = (int)(Util.Rand32() & MaxIV);
                if (Legal.Legends.Contains(PKM.Species) || Legal.SubLegends.Contains(PKM.Species))
                    for (int i = 0; i < 3; i++)
                        IVs[i] = MaxIV;

                Util.Shuffle(IVs);
                PKM.IVs = IVs;
            }
            else
                ReflectFrameworkUtil.SetValue(PKM, cmd.PropertyName, Util.Rand32() & MaxIV);
        }
    }

    public static class ReflectFrameworkUtil
    {
        public static bool IsValueEqual(this Type t, object obj, string propertyName, object value)
        {
            PropertyInfo pi = t.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var v = pi.GetValue(obj, null);
            var c = ConvertValue(value, pi.PropertyType);
            return v.Equals(c);
        }
        public static void SetValue(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            pi.SetValue(obj, ConvertValue(value, pi.PropertyType), null);
        }

        public static object GetValue(object obj, string propertyName)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            return pi.GetValue(obj, null);
        }

        public static IEnumerable<string> GetPropertiesStartWithPrefix(Type type, string prefix, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            return type.GetProperties(flags)
                .Where(p => p.Name.StartsWith(prefix, StringComparison.Ordinal))
                .Select(p => p.Name);
        }
        public static IEnumerable<string> GetPropertiesCanWritePublic(Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            return type.GetProperties(flags)
                .Where(p => p.CanWrite && p.GetSetMethod(nonPublic: true).IsPublic)
                .Select(p => p.Name);
        }
        public static IEnumerable<string> GetPropertiesCanWritePublicDeclared(Type type)
        {
            return GetPropertiesCanWritePublic(type, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }
        public static bool HasProperty(this Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;
        }
        public static bool HasPropertyAll(this Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        private static object ConvertValue(object value, Type type)
        {
            if (type == typeof(DateTime?)) // Used for PKM.MetDate and other similar properties
            {
                return DateTime.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue)
    ? new DateTime?(dateValue)
    : null;
            }

            // Convert.ChangeType is suitable for most things
            return Convert.ChangeType(value, type);
        }
    }
}
