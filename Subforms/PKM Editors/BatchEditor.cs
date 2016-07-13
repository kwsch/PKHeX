using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class BatchEditor : Form
    {
        public BatchEditor()
        {
            InitializeComponent();
            DragDrop += tabMain_DragDrop;
            DragEnter += tabMain_DragEnter;
        }

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
            { Util.Alert("Currently executing instruction list."); return; }

            if (RTB_Instructions.Lines.Any(line => line.Length == 0))
            { Util.Error("Line length error in instruction list."); return; }

            runBackgroundWorker();
        }

        private BackgroundWorker b = new BackgroundWorker { WorkerReportsProgress = true };
        private void runBackgroundWorker()
        {
            FLP_RB.Enabled = RTB_Instructions.Enabled = B_Go.Enabled = false;

            var Filters = getFilters().ToList();
            var Instructions = getInstructions().ToList();
            b = new BackgroundWorker {WorkerReportsProgress = true};

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
                    processFolder(files, Filters, Instructions);
                }
            };
            b.ProgressChanged += (sender, e) =>
            {
                setProgressBar(e.ProgressPercentage);
            };
            b.RunWorkerCompleted += (sender, e) => {
                Util.Alert(result);
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
        private string result = "";
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
            int ctr = 0;
            for (int i = 0; i < data.Length; i++)
            {
                var pkm = data[i];
                if (ProcessPKM(pkm, Filters, Instructions))
                    ctr++;
                b.ReportProgress(i);
            }

            Main.SAV.BoxData = data;
            result = $"Modified {ctr}/{data.Length} files.";
        }
        private void processFolder(string[] files, List<StringInstruction> Filters, List<StringInstruction> Instructions)
        {
            int ctr = 0;
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
                if (ProcessPKM(pkm, Filters, Instructions))
                    ctr++;

                File.WriteAllBytes(file, pkm.DecryptedBoxData);
                b.ReportProgress(i);
            }
            result = $"Modified {ctr}/{files.Length} files.";
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

        // Utility Methods
        private class StringInstruction
        {
            public string PropertyName;
            public string PropertyValue;
            public bool Evaluator;
        }
        private static bool ProcessPKM(PKM PKM, IEnumerable<StringInstruction> Filters, IEnumerable<StringInstruction> Instructions)
        {
            if (!PKM.ChecksumValid)
                return false;
            if (PKM.Species == 0)
                return false;
            foreach (var cmd in Filters)
            {
                try
                {
                    if (ReflectUtil.GetValueEquals(PKM, cmd.PropertyName, cmd.PropertyValue) != cmd.Evaluator)
                        return false;
                }
                catch
                {
                    Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}.");
                    return false;
                }
            }

            foreach (var cmd in Instructions)
            {
                try { ReflectUtil.SetValue(PKM, cmd.PropertyName, cmd.PropertyValue); }
                catch { Console.WriteLine($"Unable to set {cmd.PropertyName} to {cmd.PropertyValue}."); }
            }
            return true;
        }

        private static class ReflectUtil
        {
            public static bool GetValueEquals<T>(T obj, string propertyName, object value)
            {
                PropertyInfo pi = typeof(T).GetProperty(propertyName);
                var v = pi.GetValue(obj, null);
                var c = Convert.ChangeType(value, pi.PropertyType);
                return v.Equals(c);
            }
            public static void SetValue<T>(T obj, string propertyName, object value)
            {
                PropertyInfo pi = typeof(T).GetProperty(propertyName);
                pi.SetValue(obj, Convert.ChangeType(value, pi.PropertyType), null);
            }
        }
    }
}
