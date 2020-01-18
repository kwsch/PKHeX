using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_BlockDump8 : Form
    {
        private readonly SAV8SWSH SAV;
        private SCBlock CurrentBlock;

        public SAV_BlockDump8(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV8SWSH)sav;

            var blocks = SAV.AllBlocks.Select((z, i) => new ComboItem($"{z.Key:X8} - {i:0000} {z.Type}", (int)z.Key));
            CB_Key.InitializeBinding();
            CB_Key.DataSource = blocks.ToArray();
        }

        private void CB_Key_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = (uint)WinFormsUtil.GetIndex(CB_Key);
            CurrentBlock = SAV.Blocks.GetBlock(key);
            L_Detail_R.Text = GetBlockSummary(CurrentBlock);
        }

        private void B_ExportAll_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            var path = fbd.SelectedPath;
            var blocks = SAV.AllBlocks;
            ExportAllBlocks(blocks, path);
        }

        private static void ExportAllBlocks(IEnumerable<SCBlock> blocks, string path)
        {
            foreach (var b in blocks.Where(z => z.Data.Length != 0))
                File.WriteAllBytes(Path.Combine(path, $"{GetBlockFileNameWithoutExtension(b)}.bin"), b.Data);
        }

        private void B_ImportFolder_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            var failed = ImportBlocksFromFolder(fbd.SelectedPath, SAV);
            if (failed.Count != 0)
            {
                var msg = string.Join(Environment.NewLine, failed);
                WinFormsUtil.Error("Failed to import:", msg);
            }
        }

        private void B_ImportCurrent_Click(object sender, EventArgs e) => ImportSelectBlock(CurrentBlock);
        private void B_ExportCurrent_Click(object sender, EventArgs e) => ExportSelectBlock(CurrentBlock);

        private void B_ExportAllSingle_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog { FileName = "raw.bin" };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            var blocks = SAV.Blocks.BlockInfo;
            ExportAllBlocksAsSingleFile(blocks, sfd.FileName, CHK_DataOnly.Checked, CHK_Key.Checked, CHK_Type.Checked, CHK_FakeHeader.Checked);
        }

        private static void ExportSelectBlock(SCBlock block)
        {
            var name = GetBlockFileNameWithoutExtension(block);
            using var sfd = new SaveFileDialog {FileName = $"{name}.bin"};
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            File.WriteAllBytes(sfd.FileName, block.Data);
        }

        private static void ImportSelectBlock(SCBlock blockTarget)
        {
            var key = blockTarget.Key;
            var data = blockTarget.Data;
            using var ofd = new OpenFileDialog {FileName = $"{key:X8}.bin"};
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            var path = ofd.FileName;
            var file = new FileInfo(path);
            if (file.Length != data.Length)
            {
                WinFormsUtil.Error(string.Format(MessageStrings.MsgFileSize, $"0x{file.Length:X8}"));
                return;
            }

            var bytes = File.ReadAllBytes(path);
            bytes.CopyTo(data, 0);
        }

        private static void ExportAllBlocksAsSingleFile(IReadOnlyList<SCBlock> blocks, string path, bool dataOnly = true, bool key = true, bool typeInfo = true, bool fakeHeader = true)
        {
            using var stream = new MemoryStream();
            using var bw = new BinaryWriter(stream);

            if (fakeHeader)
            {
                for (int i = 0; i < blocks.Count; i++)
                    blocks[i].ID = (uint)i;
            }

            var iterate = dataOnly ? blocks.Where(z => z.Data.Length != 0) : blocks;
            foreach (var b in iterate)
            {
                if (fakeHeader)
                    bw.Write($"BLOCK{b.ID:0000} {b.Key:X8}");
                if (key)
                    bw.Write(b.Key);
                if (typeInfo)
                {
                    bw.Write((byte)b.Type);
                    bw.Write((byte)b.SubType);
                }
                bw.Write(b.Data);
            }
            var data = stream.ToArray(); // SwishCrypto.GetDecryptedRawData(blocks); for raw encrypted
            File.WriteAllBytes(path, data);
        }

        private static string GetBlockFileNameWithoutExtension(SCBlock block)
        {
            var key = block.Key;
            var name = $"{key:X8}";
            if (block.HasValue())
                name += $" {block.GetValue()}";
            return name;
        }

        private static string GetBlockSummary(SCBlock b)
        {
            var sb = new StringBuilder();
            sb.Append("Key: ").AppendFormat("{0:X8}", b.Key).AppendLine();
            sb.Append("Type: ").Append(b.Type).AppendLine();
            if (b.Data.Length != 0)
                sb.Append("Length: ").AppendFormat("{0:X8}", b.Data.Length).AppendLine();

            if (b.SubType != 0)
                sb.Append("SubType: ").Append(b.SubType).AppendLine();
            else if (b.HasValue())
                sb.Append("Value: ").Append(b.GetValue()).AppendLine();

            return sb.ToString();
        }

        private static List<string> ImportBlocksFromFolder(string path, SAV8SWSH sav)
        {
            var failed = new List<string>();
            var files = Directory.EnumerateFiles(path);
            foreach (var f in files)
            {
                var fn = Path.GetFileNameWithoutExtension(f);

                // Trim off Value summary if present
                var space = fn.IndexOf(' ');
                if (space >= 0)
                    fn = fn.Substring(0, space);

                var hex = Util.GetHexValue(fn);
                try
                {
                    var block = sav.Blocks.GetBlock(hex);
                    var len = block.Data.Length;
                    var fi = new FileInfo(f);
                    if (fi.Length != len)
                    {
                        failed.Add(fn);
                        continue;
                    }

                    var data = File.ReadAllBytes(f);
                    data.CopyTo(block.Data, 0);
                }
                catch
                {
                    failed.Add(fn);
                }
            }

            return failed;
        }
    }
}
