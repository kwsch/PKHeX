using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class SecretBaseManager3
    {
        private List<SecretBase3> bases;
        private bool[] slots = new bool[20];

        public SecretBaseManager3(byte[] data)
        {
            bases = new List<SecretBase3>();
            for (int i = 0; i < 20 ; i ++)
            {
                SecretBase3 tmp = new SecretBase3(data.AsMemory(i * SecretBase3.SIZE, SecretBase3.SIZE));
                if (tmp.OriginalTrainerName != string.Empty)
                {
                    slots[i] = true;
                    bases.Add(tmp);
                }
                else
                {
                    slots[i] = false;
                }
            }
        }

        public byte[] Write()
        {
            List<SecretBase3> tmp = bases.ToList();
            byte[] data = new byte[20 * SecretBase3.SIZE];
            for (int i = 0; i < data.Length; i++)
                data[i] = 0x00;
            for (int i = 0; i < 20; i++)
                data.AsSpan(i * 160 + 2, 7).Fill(0xFF);
            for (int i = 0; i < 20; i++)
            {
                if (slots[i])
                    bases[i].BaseData.CopyTo(data.AsSpan(i * SecretBase3.SIZE));
            }
            return data;
        }

        public List<SecretBase3> Bases { get => bases; }

        public int Count { get => bases.Count; }
    }
}
