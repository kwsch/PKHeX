using System;
using System.IO;

namespace BLZ
{

    /* 
     * Transformed to C# from the following Java code
     https://github.com/samfin/evil-randomizer/blob/master/src/cuecompressors/BLZCoder.java
    */
    /*----------------------------------------------------------------------------*/
    /*--  blz.c - Bottom LZ coding for Nintendo GBA/DS                          --*/
    /*--  Copyright (C) 2011 CUE                                                --*/
    /*--                                                                        --*/
    /*--  Ported to Java by Dabomstew under the terms of the GPL:				--*/
    /*--                                                                        --*/
    /*--  This program is free software: you can redistribute it and/or modify  --*/
    /*--  it under the terms of the GNU General Public License as published by  --*/
    /*--  the Free Software Foundation, either version 3 of the License, or     --*/
    /*--  (at your option) any later version.                                   --*/
    /*--                                                                        --*/
    /*--  This program is distributed in the hope that it will be useful,       --*/
    /*--  but WITHOUT ANY WARRANTY; without even the implied warranty of        --*/
    /*--  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          --*/
    /*--  GNU General Public License for more details.                          --*/
    /*--                                                                        --*/
    /*--  You should have received a copy of the GNU General Public License     --*/
    /*--  along with this program. If not, see <http://www.gnu.org/licenses/>.  --*/
    /*----------------------------------------------------------------------------*/

    public class BLZCoder
    {

        /**
         * @param args
         */
        public static void main(string[] args)
        {
            new BLZCoder(args);
        }

        private static readonly int CMD_DECODE = 0;
        private static readonly int CMD_ENCODE = 1;

        private static readonly int BLZ_NORMAL = 0;
        private static readonly int BLZ_BEST = 1;

        private static readonly int BLZ_SHIFT = 1;
        private static readonly int BLZ_MASK = 0x80;

        private static readonly int BLZ_THRESHOLD = 2;
        private static readonly int BLZ_N = 0x1002;
        private static readonly int BLZ_F = 0x12;

        private static readonly int RAW_MINIM = 0;
        private static readonly int RAW_MAXIM = 0x00FFFFFF;

        private static readonly int BLZ_MINIM = 4;
        private static readonly int BLZ_MAXIM = 0x01400000;

        private bool arm9;
        public BLZCoder()
        {
        }

        public BLZCoder(string[] args)
        {
            int cmd, mode = 0, arg;

            Title();

            if (args.Length < 1)
            {
                Usage();
            }

            if (args[0].Equals("-d",StringComparison.InvariantCultureIgnoreCase))
            {
                cmd = CMD_DECODE;
            }
            else if (args[0].Equals("-en", StringComparison.InvariantCultureIgnoreCase)
                  || args[0].Equals("-en9", StringComparison.InvariantCultureIgnoreCase))
            {
                cmd = CMD_ENCODE;
                mode = BLZ_NORMAL;
            }
            else if (args[0].Equals("-eo", StringComparison.InvariantCultureIgnoreCase)
                  || args[0].Equals("-eo9", StringComparison.InvariantCultureIgnoreCase))
            {
                cmd = CMD_ENCODE;
                mode = BLZ_BEST;
            }
            else
            {
                EXIT("Command not supported\n");
                return;
            }

            if (args.Length < 2)
            {
                EXIT("Filename not specified\n");
            }

            if (cmd == CMD_DECODE)
            {
                for (arg = 1; arg < args.Length; arg++)
                    BLZ_Decode(args[arg]);
            }
            else if (cmd == CMD_ENCODE)
            { 
                arm9 = (args[0].Length > 3 && args[0][3] == '9');
                for (arg = 1; arg < args.Length; arg++)
                    BLZ_Encode(args[arg], mode);
            }

            //Console.Write("\nDone\n");
        }

        private static int TripleShift(int n, int s)
        {
            if (n >= 0)
                return n >> s;
            return (n >> s) + (2 << ~s);
        }

        private void Usage()
        {
            EXIT("Usage: BLZ command filename [filename [...]]\n" + "\n"
                    + "command:\n" + "  -d ....... decode 'filename'\n"
                    + "  -en[9] ... encode 'filename', normal mode\n"
                    + "  -eo[9] ... encode 'filename', optimal mode (LZ-CUE)\n"
                    + "\n"
                    + "* '9' compress an ARM9 file with 0x4000 bytes decoded\n"
                    + "* multiple filenames and wildcards are permitted\n"
                    + "* the original file is overwritten with the new file\n"
                    + "* this codification is used in the DS overlay files\n");

        }

        private void Title()
        {
            //Console.Write("\n");
            //Console.Write("BLZ - (c) CUE 2011\n");
            //Console.Write("Bottom LZ coding for Nintendo GBA/DS\n");
            //Console.Write("\n");
        }

        public void EXIT(string text)
        {
            //Console.Write(text);
            //System.exit(0);
        }

        private int[] Load(string filename, int min, int max)
        {
            try
            {
                FileStream fis = File.OpenRead(filename);
                int fs = (int)new FileInfo(filename).Length;
                if (fs < min || fs > max)
                {
                    fis.Close();
                    EXIT("\nFile size error\n");
                    return null;
                }
                byte[] fbOrig = new byte[fs + 3];
                fis.Read(fbOrig, 0, fs);
                fis.Close();
                int[] fb = new int[fs + 3];
                for (int i = 0; i < fs; i++)
                {
                    fb[i] = fbOrig[i] & 0xFF;
                }
                return fb;
            }
            catch (IOException e)
            {
                EXIT("\nFile read error\n");
                return null;
            }
        }

        private void Save(string filename, int[] buffer, int Length)
        {
            try
            {
                FileStream fos = File.OpenWrite(filename);
                byte[] write = new byte[Length];
                for (int i = 0; i < Length; i++)
                {
                    write[i] = (byte)buffer[i];
                }
                fos.Write(write,0, write.Length);
                fos.Close();
            }
            catch (IOException e)
            {
                EXIT("\nFile write error\n");
            }

        }
        public void BLZ_Decode(string filename)
        {
            BLZ_Decode(filename, filename);
        }

        public void BLZ_Decode(string source_filename, string output_filename)
        {
            int[] pak_buffer, raw_buffer;
            int pak, raw, pak_end, raw_end;
            int pak_len, raw_len, len, pos, inc_len, hdr_len, enc_len, dec_len;
            int flags = 0, mask;
            long startTime = DateTime.Now.Ticks;

            //Console.Write("- decoding '%s'", source_filename);

            pak_buffer = Load(source_filename, BLZ_MINIM, BLZ_MAXIM);
            pak_len = pak_buffer.Length - 3;

            inc_len = readUnsigned(pak_buffer, pak_len - 4);
            if (inc_len < 1)
            {
                //Console.Write(", WARNING: not coded file!");
                enc_len = 0;
                dec_len = pak_len;
                pak_len = 0;
                raw_len = dec_len;
            }
            else
            {
                if (pak_len < 8)
                {
                    EXIT("\nFile has a bad header\n");
                    return;
                }
                hdr_len = pak_buffer[pak_len - 5];
                if (hdr_len < 8 || hdr_len > 0xB)
                {
                    EXIT("\nBad header Length\n");
                    return;
                }
                if (pak_len <= hdr_len)
                {
                    EXIT("\nBad Length\n");
                    return;
                }
                enc_len = readUnsigned(pak_buffer, pak_len - 8) & 0x00FFFFFF;
                dec_len = pak_len - enc_len;
                pak_len = enc_len - hdr_len;
                raw_len = dec_len + enc_len + inc_len;
                if (raw_len > RAW_MAXIM)
                {
                    EXIT("\nBad decoded Length\n");
                    return;
                }
            }

            raw_buffer = new int[raw_len];

            pak = 0;
            raw = 0;
            pak_end = dec_len + pak_len;
            raw_end = raw_len;

            for (len = 0; len < dec_len; len++)
            {
                raw_buffer[raw++] = pak_buffer[pak++];
            }

            BLZ_Invert(pak_buffer, dec_len, pak_len);

            mask = 0;

            while (raw < raw_end)
            {
                //if ((mask = (mask >>> BLZ_SHIFT)) == 0)
                if ((mask = TripleShift(mask, BLZ_SHIFT)) == 0)
                {
                    if (pak == pak_end)
                    {
                        break;
                    }
                    flags = pak_buffer[pak++];
                    mask = BLZ_MASK;
                }

                if ((flags & mask) == 0)
                {
                    if (pak == pak_end)
                    {
                        break;
                    }
                    raw_buffer[raw++] = pak_buffer[pak++];
                }
                else
                {
                    if ((pak + 1) >= pak_end)
                    {
                        break;
                    }
                    pos = pak_buffer[pak++] << 8;
                    pos |= pak_buffer[pak++];
                    // len = (pos >>> 12) + BLZ_THRESHOLD + 1;
                    len = TripleShift(pos, 12) + BLZ_THRESHOLD + 1;
                    if (raw + len > raw_end)
                    {
                        //Console.Write(", WARNING: wrong decoded Length!");
                        len = raw_end - raw;
                    }
                    pos = (pos & 0xFFF) + 3;
                    while ((len--) > 0)
                    {
                        int charHere = raw_buffer[raw - pos];
                        raw_buffer[raw++] = charHere;
                    }
                }
            }

            BLZ_Invert(raw_buffer, dec_len, raw_len - dec_len);

            raw_len = raw;

            if (raw != raw_end)
            {
                //Console.Write(", WARNING: unexpected end of encoded file!");
            }

            Save(output_filename, raw_buffer, raw_len);

            //Console.Write(" - done, time="
            //        + (DateTime.Now.Ticks - startTime) + "ms");

            //Console.Write("\n");
        }

        private int readUnsigned(int[] buffer, int offset)
        {
            return buffer[offset] | (buffer[offset + 1] << 8)
                    | (buffer[offset + 2] << 16)
                    | ((buffer[offset + 3] & 0x7F) << 24);
        }

        private void writeUnsigned(int[] buffer, int offset, int value)
        {
            buffer[offset] = value & 0xFF;
            buffer[offset + 1] = (value >> 8) & 0xFF;
            buffer[offset + 2] = (value >> 16) & 0xFF;
            buffer[offset + 3] = (value >> 24) & 0x7F;
        }

        private int new_len;

        public void BLZ_Encode(string filename, int mode)
        {
            int[] raw_buffer, pak_buffer, new_buffer;
            int raw_len, pak_len;
            long startTime = DateTime.Now.Ticks;
            new_len = 0;

            //Console.Write("- encoding '%s'", filename);

            raw_buffer = Load(filename, RAW_MINIM, RAW_MAXIM);
            raw_len = raw_buffer.Length - 3;

            pak_buffer = null;
            pak_len = BLZ_MAXIM + 1;

            new_buffer = BLZ_Code(raw_buffer, raw_len, mode);

            if (new_len < pak_len)
            {
                pak_buffer = new_buffer;
                pak_len = new_len;
            }

            Save(filename, pak_buffer, pak_len);

            //Console.Write(" - done, time="
            //        + (DateTime.Now.Ticks - startTime) + "ms");

            //Console.Write("\n");
        }

        private int[] BLZ_Code(int[] raw_buffer, int raw_len, int best)
        {
            int[] pak_buffer, tmp;
            int pak, raw, raw_end, flg = 0;
            int pak_len, inc_len, hdr_len, enc_len, len;
            int len_best, pos_best = 0, len_next, pos_next = 0, len_post, pos_post = 0;
            int pak_tmp, raw_tmp, raw_new;
            int mask;

            pak_tmp = 0;
            raw_tmp = raw_len;

            pak_len = raw_len + ((raw_len + 7) / 8) + 11;
            pak_buffer = new int[pak_len];

            raw_new = raw_len;

            if (arm9)
            {
                // We don't do any of the checks here
                // Presume that we actually are using an arm9
                raw_new -= 0x4000;
            }

            BLZ_Invert(raw_buffer, 0, raw_len);

            pak = 0;
            raw = 0;
            raw_end = raw_new;

            mask = 0;
            while (raw < raw_end)
            {
                //if ((mask = (mask >>> BLZ_SHIFT)) == 0)
                if ((mask = TripleShift(mask, BLZ_SHIFT)) == 0)
                {
                    pak_buffer[(flg = pak++)] = 0;
                    mask = BLZ_MASK;
                }

                SearchPair sl1 = SEARCH(pos_best, raw_buffer, raw, raw_end);
                len_best = sl1.l;
                pos_best = sl1.p;

                // LZ-CUE optimization start
                if (best == BLZ_BEST)
                {
                    if (len_best > BLZ_THRESHOLD)
                    {
                        if (raw + len_best < raw_end)
                        {
                            raw += len_best;
                            SearchPair sl2 = SEARCH(pos_next, raw_buffer, raw,
                                    raw_end);
                            len_next = sl2.l;
                            pos_next = sl2.p;
                            raw -= (len_best - 1);
                            SearchPair sl3 = SEARCH(pos_post, raw_buffer, raw,
                                    raw_end);
                            len_post = sl3.l;
                            pos_post = sl3.p;
                            raw--;

                            if (len_next <= BLZ_THRESHOLD)
                            {
                                len_next = 1;
                            }
                            if (len_post <= BLZ_THRESHOLD)
                            {
                                len_post = 1;
                            }
                            if ((len_best + len_next) <= (1 + len_post))
                            {
                                len_best = 1;
                            }
                        }
                    }
                }
                // LZ-CUE optimization end
                pak_buffer[flg] = (pak_buffer[flg] << 1);
                if (len_best > BLZ_THRESHOLD)
                {
                    raw += len_best;
                    pak_buffer[flg] |= 1;
                    pak_buffer[pak++] = ((len_best - (BLZ_THRESHOLD + 1)) << 4)
                            //| ((pos_best - 3) >>> 8);
                            | TripleShift((pos_best - 3) , 8);
                    pak_buffer[pak++] = (pos_best - 3) & 0xFF;
                }
                else
                {
                    pak_buffer[pak++] = raw_buffer[raw++];
                }

                if (pak + raw_len - raw < pak_tmp + raw_tmp)
                {
                    pak_tmp = pak;
                    raw_tmp = raw_len - raw;
                }
            }

            while ((mask > 0) && (mask != 1))
            {
                //mask = (mask >>> BLZ_SHIFT);
                mask = TripleShift(mask , BLZ_SHIFT);
                pak_buffer[flg] = pak_buffer[flg] << 1;
            }

            pak_len = pak;

            BLZ_Invert(raw_buffer, 0, raw_len);
            BLZ_Invert(pak_buffer, 0, pak_len);

            if (pak_tmp == 0
                    || (raw_len + 4 < ((pak_tmp + raw_tmp + 3) & 0xFFFFFFFC) + 8))
            {
                pak = 0;
                raw = 0;
                raw_end = raw_len;

                while (raw < raw_end)
                {
                    pak_buffer[pak] = raw_buffer[raw];
                }

                while ((pak & 3) > 0)
                {
                    pak_buffer[pak++] = 0;
                }

                pak_buffer[pak++] = 0;
                pak_buffer[pak++] = 0;
                pak_buffer[pak++] = 0;
                pak_buffer[pak++] = 0;
            }
            else
            {
                tmp = new int[raw_tmp + pak_tmp + 11];
                for (len = 0; len < raw_tmp; len++)
                {
                    tmp[len] = raw_buffer[len];
                }
                for (len = 0; len < pak_tmp; len++)
                {
                    tmp[raw_tmp + len] = pak_buffer[len + pak_len - pak_tmp];
                }

                pak = 0;
                pak_buffer = tmp;

                pak = raw_tmp + pak_tmp;

                enc_len = pak_tmp;
                hdr_len = 8;
                inc_len = raw_len - pak_tmp - raw_tmp;

                while ((pak & 3) > 0)
                {
                    pak_buffer[pak++] = 0xFF;
                    hdr_len++;
                }

                writeUnsigned(pak_buffer, pak, enc_len + hdr_len);
                pak += 3;
                pak_buffer[pak++] = hdr_len;
                writeUnsigned(pak_buffer, pak, inc_len - hdr_len);
                pak += 4;

            }
            new_len = pak;
            return pak_buffer;
        }

        private class SearchPair
        {
            public int l;
            public int p;

            public SearchPair(int _l, int _p)
            {
                this.l = _l;
                this.p = _p;
            }
        }

        private SearchPair SEARCH(int p, int[] raw_buffer, int raw, int raw_end)
        {
            int l = BLZ_THRESHOLD;
            int max = (raw >= BLZ_N) ? BLZ_N : raw;
            for (int pos = 3; pos <= max; pos++)
            {
                int len;
                for (len = 0; len < BLZ_F; len++)
                {
                    if (raw + len == raw_end)
                    {
                        break;
                    }
                    if (len >= pos)
                    {
                        break;
                    }
                    if (raw_buffer[raw + len] != raw_buffer[raw + len - pos])
                    {
                        break;
                    }
                }

                if (len > l)
                {
                    p = pos;
                    if ((l = len) == BLZ_F)
                    {
                        break;
                    }
                }
            }
            return new SearchPair(l, p);
        }

        private void BLZ_Invert(int[] buffer, int offset, int Length)
        {
            int bottom, ch;

            bottom = offset + Length - 1;

            while (offset < bottom)
            {
                ch = buffer[offset];
                buffer[offset++] = buffer[bottom];
                buffer[bottom--] = ch;
            }
        }

    }
}
