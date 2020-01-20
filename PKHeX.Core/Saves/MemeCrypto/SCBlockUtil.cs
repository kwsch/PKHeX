using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    public static class SCBlockUtil
    {
        public static void ExportAllBlocksAsSingleFile(IReadOnlyList<SCBlock> blocks, string path, bool dataOnly = true, bool key = true, bool typeInfo = true, bool fakeHeader = true)
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

        public static string GetBlockFileNameWithoutExtension(SCBlock block)
        {
            var key = block.Key;
            var name = $"{key:X8}";
            if (block.HasValue())
                name += $" {block.GetValue()}";
            return name;
        }

        public static string GetBlockSummary(SCBlock b)
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

        public static List<string> ImportBlocksFromFolder(string path, SAV8SWSH sav)
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