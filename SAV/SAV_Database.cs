using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Database : Form
    {
        private string DatabasePath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "db");
        private Main m_parent;
        public SAV_Database(Main f1)
        {
            m_parent = f1;
            InitializeComponent();

            // Load initial database
            Database.Add(new DatabaseList
            {
                Version = 0,
                Title = "Misc",
                Description = "Individual pk6 files present in the db/sav.",
            });

            // Load databases
            foreach (string file in Directory.GetFiles(DatabasePath))
            {
                if ((new FileInfo(file)).Extension == ".pk6")
                    Database[0].Slot.Add(new PK6(File.ReadAllBytes(file), file));
                else
                    loadDatabase(File.ReadAllBytes(file));
            }
            for (int i = 0; i < 930; i++)
            {
                PK6 pk6 = new PK6(PKX.decryptArray(Main.savefile.Skip(Main.SaveGame.Box + 0xE8*i).Take(0xE8).ToArray()), "Boxes");
                if (pk6.Species != 0)
                    Database[0].Slot.Add(pk6);
            }

            testQuery();
            testUnique();
        }

        private void testQuery()
        {
            var query = from db in Database
                        select db.Slot.Where(p => p.Move1 == 1).ToArray();

            var result = query.ToArray();
            if (!result[0].Any())
                return;

            var any = result[0][0].Data;
            m_parent.populateFields(any);
        }

        private void testUnique()
        {
            var query = from db in Database
                select db.Slot.GroupBy(p => (p.Checksum + p.EncryptionConstant + p.Species)) // Unique criteria
                .Select(grp => grp.First()).ToArray();

            var result = query.ToArray();
            if (!result[0].Any())
                return;

            var any = result[0][0].Data;
            m_parent.populateFields(any);
        }
        private void loadDatabase(byte[] data)
        {
            var db = new DatabaseList(data);
            if (db.Slot.Count > 0)
                Database.Add(db);
        }
        private class DatabaseList
        {
            public List<PK6> Slot = new List<PK6>();
            public int Version;
            private bool Unicode;
            public byte[] Unused;
            public string Title;
            public string Description;

            public DatabaseList() { }
            public DatabaseList(byte[] data)
            {
                if ((data.Length < 0x100 + 0xE8) || (data.Length - 0x100)%0xE8 != 0)
                    return;

                Version = BitConverter.ToInt32(data, 0);
                Unicode = data[0x5] == 1;
                Unused = data.Skip(4).Take(0xB).ToArray();

                if (Unicode)
                {
                    Title = Encoding.Unicode.GetString(data, 0x10, 0x30).Trim();
                    Description = Encoding.Unicode.GetString(data, 0x40, 0x60).Trim();
                }
                else
                {
                    Title = Encoding.ASCII.GetString(data, 0x10, 0x30).Trim();
                    Description = Encoding.ASCII.GetString(data, 0x40, 0x60).Trim();
                }

                int count = (data.Length - 0x100)/0xE8;
                for (int i = 0; i < count; i++)
                    Slot.Add(new PK6(data.Skip(0x100 + i * 0xE8).Take(0xE8).ToArray()));
            }
            public byte[] Write()
            {
                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(Version);
                    bw.Write(Unused);

                    byte[] title = Unicode ? Encoding.Unicode.GetBytes(Title) : Encoding.ASCII.GetBytes(Title);
                    Array.Resize(ref title, 0x30);
                    bw.Write(title);
                    
                    byte[] desc = Unicode ? Encoding.Unicode.GetBytes(Description) : Encoding.ASCII.GetBytes(Description);
                    Array.Resize(ref title, 0x60);
                    bw.Write(desc);

                    foreach (var pk6 in Slot)
                        bw.Write(pk6.Data.Take(0xE8).ToArray());

                    return ms.ToArray();
                }
            }
        }
        List<DatabaseList> Database = new List<DatabaseList>();

        private void openDB(object sender, EventArgs e)
        {
            if (Directory.Exists(DatabasePath))
                Process.Start("explorer.exe", @DatabasePath);
        }
    }
}
