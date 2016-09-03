using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PKHeX
{

    //Source 
    //https://github.com/andibadra/ANDT/blob/master/Andi.Utils/Nitro/Archive/AndiNarcReader.cs
    public class AndiNarcReader
    {
        public struct FileEntry
        {
            public int Ofs;
            public int Size;
        }

        public FileEntry[] FileInformation;
        private MemoryStream FsMemoryStream;
        private FileStream FsFileStream;
        public byte[] CachedData;

        public long MemorySize { get; set; }

        public long SectionNarc { get; set; }

        public int FileCount { get; set; }

        public AndiNarcReader()
        {

        }

        public void SaveData(string path)
        {
            File.WriteAllBytes(path, CachedData);
        }

        public void OpenData(string pathdata)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(pathdata));
            byte[] buffer = new byte[reader.BaseStream.Length];

            reader.BaseStream.Position = 0L;
            reader.Read(buffer, 0, (int)reader.BaseStream.Length);
            reader.Close();

            setData(buffer);
        }

        public void OpenData(byte[] data)
        {
            setData(data);
        }

        private void setData(byte[] datainput)
        {
            this.CachedData = datainput;
            this.FsMemoryStream = new MemoryStream(this.CachedData);

            BinaryReader binaryReader = new BinaryReader((Stream)this.FsMemoryStream);
            byte[] buffer1 = new byte[16];
            binaryReader.Read(buffer1, 0, 16);
            this.MemorySize = (long)BitConverter.ToUInt32(buffer1, 8);
            int HeaderSize = (int)BitConverter.ToInt16(buffer1, 12);
            this.SectionNarc = BitConverter.ToInt16(buffer1, 14);
            this.FsMemoryStream.Seek((long)HeaderSize, SeekOrigin.Begin);

            byte[] buffer2 = new byte[12];
            binaryReader.Read(buffer2, 0, 12);
            int num2 = BitConverter.ToInt32(buffer2, 4);
            this.FileCount = BitConverter.ToInt32(buffer2, 8);
            this.FileInformation = new FileEntry[this.FileCount];

            for (int index = 0; index < this.FileCount; ++index)
            {
                this.FileInformation[index].Ofs = binaryReader.ReadInt32();
                this.FileInformation[index].Size = binaryReader.ReadInt32() - this.FileInformation[index].Ofs;
            }
            this.FsMemoryStream.Seek((long)(HeaderSize + num2), SeekOrigin.Begin);

            byte[] buffer3 = new byte[16];
            binaryReader.Read(buffer3, 0, 16);
            int num3 = BitConverter.ToInt32(buffer3, 4);
            int num4 = HeaderSize + num3 + num2 + 8;
            for (int index = 0; index < this.FileCount; ++index)
                this.FileInformation[index].Ofs += num4;
            this.FsMemoryStream.Close();
        }

        private int OpenEntry(int id)
        {
            this.FsMemoryStream = new MemoryStream(this.CachedData);
            this.FsMemoryStream.Seek((long)this.FileInformation[id].Ofs, SeekOrigin.Begin);
            return 0;
        }

        private void CloseStream()
        {
            FsMemoryStream.Close();
        }

        public byte[] getdata()
        {
            return this.CachedData;
        }

        public byte[] getdataselected(int id)
        {
            this.OpenEntry(id);
            BinaryReader binaryReader = new BinaryReader(this.FsMemoryStream);

            byte[] buffer = new byte[this.FileInformation[id].Size];
            binaryReader.Read(buffer, 0, this.FileInformation[id].Size);
            binaryReader.Close();
            CloseStream();
            return buffer;
        }

        private int WriteCachedData(MemoryStream a)
        {
            BinaryReader binaryReader = new BinaryReader((Stream)a);
            this.CachedData = new byte[binaryReader.BaseStream.Length];
            binaryReader.BaseStream.Position = 0L;
            binaryReader.Read(this.CachedData, 0, (int)binaryReader.BaseStream.Length);
            binaryReader.Close();
            return 0;
        }

        public void ReplaceEntry(int index, int newsize, byte[] replacement)
        {
            int num1 = newsize - this.FileInformation[index].Size;
            this.FsMemoryStream = new MemoryStream();
            FsMemoryStream.Write(this.CachedData, 0, this.CachedData.Length);

            if (num1 > 0)
            {
                this.FsMemoryStream.SetLength(this.FsMemoryStream.Length + num1);
            }

            this.FsMemoryStream.Seek(8L, SeekOrigin.Begin);
            new BinaryWriter((Stream)this.FsMemoryStream).Write((int)this.MemorySize + num1);
            this.WriteCachedData(this.FsMemoryStream);
            CloseStream();

            if (num1 > 0)
            {
                for (int index1 = this.FileCount - 1; index1 > index; --index1)
                {
                    this.FsMemoryStream = new MemoryStream(this.CachedData);
                    this.FsMemoryStream.Seek((long)this.FileInformation[index1].Ofs, SeekOrigin.Begin);
                    BinaryReader binaryReader = new BinaryReader((Stream)this.FsMemoryStream);
                    BinaryWriter binaryWriter = new BinaryWriter((Stream)this.FsMemoryStream);

                    byte[] buffer = new byte[this.FileInformation[index1].Size];
                    binaryReader.Read(buffer, 0, this.FileInformation[index1].Size);
                    this.FsMemoryStream.Seek((long)(-this.FileInformation[index1].Size + num1), SeekOrigin.Current);
                    binaryWriter.Write(buffer, 0, this.FileInformation[index1].Size);
                    this.WriteCachedData(this.FsMemoryStream);
                    this.FsMemoryStream.Close();
                }
                this.FsMemoryStream = new MemoryStream(this.CachedData);
                this.FsMemoryStream.Seek((long)this.FileInformation[index].Ofs, SeekOrigin.Begin);
                new BinaryWriter((Stream)this.FsMemoryStream).Write(replacement);
                this.WriteCachedData(this.FsMemoryStream);
                this.FsMemoryStream.Close();

                for (int index1 = index; index1 < this.FileCount; ++index1)
                {
                    this.FsMemoryStream = new MemoryStream(this.CachedData);
                    this.FsMemoryStream.Seek((long)(28 + index1 * 8), SeekOrigin.Begin);
                    BinaryWriter binaryWriter = new BinaryWriter((Stream)this.FsMemoryStream);
                    BinaryReader binaryReader = new BinaryReader((Stream)this.FsMemoryStream);
                    if (index1 == index)
                    {
                        this.FsMemoryStream.Seek(4L, SeekOrigin.Current);
                        long num2 = (long)binaryReader.ReadUInt32();
                        this.FsMemoryStream.Seek(-4L, SeekOrigin.Current);
                        binaryWriter.Write((int)num2 + num1);
                        this.FileInformation[index].Size += num1;
                    }
                    else
                    {
                        long num2 = (long)binaryReader.ReadUInt32();
                        this.FsMemoryStream.Seek(-4L, SeekOrigin.Current);
                        binaryWriter.Write((int)num2 + num1);
                        long num3 = (long)binaryReader.ReadUInt32();
                        this.FsMemoryStream.Seek(-4L, SeekOrigin.Current);
                        binaryWriter.Write((int)num3 + num1);
                        this.FileInformation[index1].Ofs += num1;
                    }
                    this.WriteCachedData(this.FsMemoryStream);
                    this.FsMemoryStream.Close();
                }
            }
            else if (num1 < 0)
            {
                for (int index1 = index + 1; index1 < this.FileCount; ++index1)
                {
                    this.FsMemoryStream = new MemoryStream(this.CachedData);
                    this.FsMemoryStream.Seek((long)this.FileInformation[index1].Ofs, SeekOrigin.Begin);
                    BinaryReader binaryReader = new BinaryReader((Stream)this.FsMemoryStream);
                    BinaryWriter binaryWriter = new BinaryWriter((Stream)this.FsMemoryStream);
                    byte[] buffer = new byte[this.FileInformation[index1].Size];
                    binaryReader.Read(buffer, 0, this.FileInformation[index1].Size);
                    this.FsMemoryStream.Seek((long)(-this.FileInformation[index1].Size + num1), SeekOrigin.Current);
                    binaryWriter.Write(buffer, 0, this.FileInformation[index1].Size);
                    this.WriteCachedData(this.FsMemoryStream);
                    this.FsMemoryStream.Close();
                }

                this.FsMemoryStream = new MemoryStream(this.CachedData);
                this.FsMemoryStream.Seek((long)this.FileInformation[index].Ofs, SeekOrigin.Begin);
                new BinaryWriter((Stream)this.FsMemoryStream).Write(replacement);
                this.WriteCachedData(this.FsMemoryStream);
                this.FsMemoryStream.Close();

                for (int index1 = index; index1 < this.FileCount; ++index1)
                {
                    this.FsMemoryStream = new MemoryStream(this.CachedData);
                    this.FsMemoryStream.Seek((long)(28 + index1 * 8), SeekOrigin.Begin);
                    BinaryWriter binaryWriter = new BinaryWriter((Stream)this.FsMemoryStream);
                    BinaryReader binaryReader = new BinaryReader((Stream)this.FsMemoryStream);
                    if (index1 == index)
                    {
                        this.FsMemoryStream.Seek(4L, SeekOrigin.Current);
                        long num2 = (long)binaryReader.ReadUInt32();
                        this.FsMemoryStream.Seek(-4L, SeekOrigin.Current);
                        binaryWriter.Write((int)num2 + num1);
                        this.FileInformation[index].Size += num1;
                    }
                    else
                    {
                        long num2 = (long)binaryReader.ReadUInt32();
                        this.FsMemoryStream.Seek(-4L, SeekOrigin.Current);
                        binaryWriter.Write((int)num2 + num1);
                        long num3 = (long)binaryReader.ReadUInt32();
                        this.FsMemoryStream.Seek(-4L, SeekOrigin.Current);
                        binaryWriter.Write((int)num3 + num1);
                        this.FileInformation[index1].Ofs += num1;
                    }
                    this.WriteCachedData(this.FsMemoryStream);
                    this.FsMemoryStream.Close();
                }

                this.FsMemoryStream = new MemoryStream();
                this.FsMemoryStream.Write(this.CachedData, 0, this.CachedData.Length + num1);
                this.WriteCachedData(this.FsMemoryStream);
                this.FsMemoryStream.Close();
            }
            else
            {
                this.FsMemoryStream = new MemoryStream(this.CachedData);
                this.FsMemoryStream.Seek((long)this.FileInformation[index].Ofs, SeekOrigin.Begin);
                new BinaryWriter((Stream)this.FsMemoryStream).Write(replacement);
                this.WriteCachedData(this.FsMemoryStream);
                this.FsMemoryStream.Close();
            }
        }
    }
}
