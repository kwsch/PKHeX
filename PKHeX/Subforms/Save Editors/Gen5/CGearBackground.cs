using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PKHeX
{
    public class CGearBackground
    {
        public const string Extension = "cgb";
        public const string Filter = "C-Gear Background|*.cgb";
        public const int Width = 256; // px
        public const int Height = 192; // px
        public const int SIZE_CGB = 0x2600;

        /* CGearBackground Documentation
        * CGearBackgrounds (.cgb) are tiled images.
        * Tiles are 8x8, and serve as a tileset for building the image.
        * The first 0x2000 bytes are the tile building region.
        * A tile to have two pixels defined in one byte of space.
        * A tile takes up 64 pixels, 32 bytes, 0x20 chunks.
        * The last tile is actually the colors used in the image (16bit).
        * Only 16 colors can be used for the entire image.
        * 255 tiles may be chosen from, as (0x2000-(0x20))/0x20 = 0xFF
        * The last 0x600 bytes are the tiles used.
        * 256/8 = 32, 192/8 = 24
        * 32 * 24 = 0x300
        * The tiles are chosen based on the 16bit index of the tile.
        * 0x300 * 2 = 0x600!
        * 
        * CGearBackgrounds tilemap (when stored) employs odd obfuscation.
        * BW obfuscates by adding 0xA0A0, B2W2 adds 0xA000
        * The obfuscated number is then tweaked by adding 15*(i/17)
        * To reverse, use a similar reverse calculation
        * PSK files are basically raw game rips (obfuscated)
        * CGB files are un-obfuscated.
        * Due to BW and B2W2 using different obfuscation adds, PSK files are incompatible between the versions.
        */
        
        public CGearBackground(byte[] data)
        {
            if (data.Length != SIZE_CGB)
                return;

            byte[] Region1 = data.Take(0x1FE0).ToArray();
            byte[] ColorData = data.Skip(0x1FE0).Take(0x20).ToArray();
            byte[] Region2 = data.Skip(0x2000).Take(0x600).ToArray();

            ColorPalette = new Color[0x10];
            for (int i = 0; i < 0x10; i++)
                ColorPalette[i] = getRGB555_16(BitConverter.ToUInt16(ColorData, i * 2));

            Tiles = new Tile[0xFF];
            for (int i = 0; i < 0xFF; i++)
            {
                Tiles[i] = new Tile(Region1.Skip(i * Tile.SIZE_TILE).Take(Tile.SIZE_TILE).ToArray());
                Tiles[i].setTile(ColorPalette);
            }

            Map = new TileMap(Region2);
        }

        public byte[] Write()
        {
            byte[] data = new byte[SIZE_CGB];
            for (int i = 0; i < Tiles.Length; i++)
                Array.Copy(Tiles[i].Write(), 0, data, i*Tile.SIZE_TILE, Tile.SIZE_TILE);

            for (int i = 0; i < ColorPalette.Length; i++)
                BitConverter.GetBytes(getRGB555(ColorPalette[i])).CopyTo(data, 0x1FE0 + i*2);

            Array.Copy(Map.Write(), 0, data, 0x2000, 0x600);

            return data;
        }

        public static bool getIsCGB(byte[] data)
        {
            return data[0x2001] == 0;
        }
        public static byte[] CGBtoPSK(byte[] cgb, bool B2W2)
        {
            byte[] psk = (byte[])cgb.Clone();
            int shiftVal = B2W2 ? 0xA000 : 0xA0A0;
            for (int i = 0x2000; i < 0x2600; i += 2)
            {
                int index = BitConverter.ToUInt16(cgb, i);
                int val = IndexToVal(index, shiftVal);
                BitConverter.GetBytes((ushort)val).CopyTo(psk, i);
            }
            return psk;
        }
        public static byte[] PSKtoCGB(byte[] psk, bool B2W2)
        {
            byte[] cgb = (byte[])psk.Clone();
            for (int i = 0x2000; i < 0x2600; i += 2)
            {
                int val = BitConverter.ToUInt16(psk, i);
                int index = ValToIndex(val);
                BitConverter.GetBytes((ushort)index).CopyTo(cgb, i);
            }
            return cgb;
        }

        private Color[] ColorPalette;
        private Tile[] Tiles;
        private TileMap Map;
        
        public Bitmap GetImage()
        {
            Bitmap img = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            // Fill Data
            using (Graphics g = Graphics.FromImage(img))
            for (int i = 0; i < Map.TileChoices.Length; i++)
            {
                int x = (i*8)%Width;
                int y = 8*((i*8)/Width);

                Bitmap b = Tiles[Map.TileChoices[i] % Tiles.Length].Rotate(Map.Rotations[i]);
                g.DrawImage(b, new Point(x, y));
            }
            return img;
        }
        public void SetImage(Bitmap img)
        {
            if (img.Width != Width)
                throw new ArgumentException($"Invalid image width. Expected {Width} pixels wide.");
            if (img.Height != Height)
                throw new ArgumentException($"Invalid image height. Expected {Height} pixels high.");

            // get raw bytes of image
            BitmapData bData = img.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] data = new byte[bData.Stride * bData.Height];
            Marshal.Copy(bData.Scan0, data, 0, data.Length);

            int bpp = bData.Stride/Width;
            img.UnlockBits(bData);
            // get colors
            Color[] colors = new Color[Width*Height];
            for (int i = 0; i < data.Length; i += bpp)
            {
                uint val = BitConverter.ToUInt32(data, i);
                colors[i/bpp] = getRGB555_32(val);
            }
            
            Color[] Palette = colors.Distinct().ToArray();
            if (Palette.Length > 0x10)
                throw new ArgumentException($"Too many unique colors. Expected <= 16, got {Palette.Length}");

            // Build Tiles
            Tile[] tiles = new Tile[0x300];
            for (int i = 0; i < tiles.Length; i++)
            {
                int x = (i*8)%Width;
                int y = 8*((i*8)/Width);

                Tile t = new Tile();
                for (uint ix = 0; ix < 8; ix++)
                    for (uint iy = 0; iy < 8; iy++)
                    {
                        int index = (int)(y + iy)*Width + (int)(x + ix);
                        Color c = colors[index];

                        t.ColorChoices[ix%8 + iy*8] = Array.IndexOf(Palette, c);
                    }
                t.setTile(Palette);
                tiles[i] = t;
            }

            List<Tile> tilelist = new List<Tile> {tiles[0]};
            TileMap tm = new TileMap(new byte[2*Width*Height/64]);
            for (int i = 1; i < tm.TileChoices.Length; i++)
            {
                for (int j = 0; j < tilelist.Count; j++)
                {
                    int rotVal = tiles[i].getRotationValue(tilelist[j].ColorChoices);
                    if (rotVal <= -1) continue;
                    tm.TileChoices[i] = j;
                    tm.Rotations[i] = rotVal;
                    goto next;
                }
                if (tilelist.Count == 0xFF)
                    throw new ArgumentException($"Too many unique tiles. Expected < 256, ran out of tiles at {i + 1} of {tm.TileChoices.Length}");
                tilelist.Add(tiles[i]);
                tm.TileChoices[i] = tilelist.Count - 1;

                next:;
            }

            // Finished!
            Map = tm;
            ColorPalette = Palette;
            Tiles = tilelist.ToArray();
        }

        private class Tile
        {
            public const int SIZE_TILE = 0x20;
            private const int TileWidth = 8;
            private const int TileHeight = 8;
            public readonly int[] ColorChoices;
            private Bitmap img;

            public Tile(byte[] data = null)
            {
                if (data == null)
                    data = new byte[SIZE_TILE];
                if (data.Length != SIZE_TILE)
                    return;

                ColorChoices = new int[TileWidth*TileHeight];
                for (int i = 0; i < data.Length; i++)
                {
                    ColorChoices[i*2+0] = data[i] & 0xF;
                    ColorChoices[i*2+1] = data[i] >> 4;
                }
            }
            public void setTile(Color[] Palette)
            {
                img = new Bitmap(8, 8);
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        var index = ColorChoices[x%8 + y*8];
                        var choice = Palette[index];
                        img.SetPixel(x, y, choice);
                    }
            }
            public byte[] Write()
            {
                byte[] data = new byte[SIZE_TILE];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] |= (byte)(ColorChoices[i*2+0] & 0xF);
                    data[i] |= (byte)((ColorChoices[i*2+1] & 0xF) << 4);
                }
                return data;
            }

            public Bitmap Rotate(int rotFlip)
            {
                if (rotFlip == 0)
                    return img;
                Bitmap tile = (Bitmap)img.Clone();
                if ((rotFlip & 4) > 0)
                    tile.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if ((rotFlip & 8) > 0)
                    tile.RotateFlip(RotateFlipType.RotateNoneFlipY);
                return tile;
            }

            public int getRotationValue(int[] tileColors)
            {
                // Check all rotation types
                if (ColorChoices.SequenceEqual(tileColors))
                    return 0;

                // flip x
                for (int i = 0; i < 64; i++)
                    if (ColorChoices[(7 - (i & 7)) + 8 * (i / 8)] != tileColors[i])
                        goto check8;
                return 4;

                // flip y
                check8:
                for (int i = 0; i < 64; i++)
                    if (ColorChoices[64 - 8 * (1 + (i / 8)) + (i & 7)] != tileColors[i])
                        goto check12;
                return 8;

                // flip xy
                check12:
                for (int i = 0; i < 64; i++)
                    if (ColorChoices[63 - i] != tileColors[i])
                        return -1;
                return 12;
            }
        }
        private class TileMap
        {
            public readonly int[] TileChoices;
            public readonly int[] Rotations;

            public TileMap(byte[] data)
            {
                TileChoices = new int[data.Length/2];
                Rotations = new int[data.Length/2];
                for (int i = 0; i < data.Length; i += 2)
                {
                    TileChoices[i/2] = data[i];
                    Rotations[i/2] = data[i+1];
                }
            }
            public byte[] Write()
            {
                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < TileChoices.Length; i++)
                    {
                        bw.Write((byte)TileChoices[i]);
                        bw.Write((byte)Rotations[i]);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static int IndexToVal(int index, int shiftVal)
        {
            int val = index + shiftVal;
            return val + 15*(index/17);
        }
        private static int ValToIndex(int val)
        {
            if ((val & 0x3FF) < 0xA0 || (val & 0x3FF) > 0x280)
                return ((val & 0x5C00) | 0xFF);
            return ((val % 0x20) + 0x11 * (((val & 0x3FF) - 0xA0) / 0x20)) | (val & 0x5C00);
        }
        
        private static byte convert8to5(int colorval)
        {
            byte i = 0;
            while (colorval > Convert5To8[i]) i++;
            return i;
        }
        private static Color getRGB555_32(uint val)
        {
            int R = (int)(val >> 0 >> 3) & 0x1F;
            int G = (int)(val >> 8 >> 3) & 0x1F;
            int B = (int)(val >> 16 >> 3) & 0x1F;
            return Color.FromArgb(0xFF, Convert5To8[R], Convert5To8[G], Convert5To8[B]);
        }
        private static Color getRGB555_16(ushort val)
        {
            int R = (val >> 0) & 0x1F;
            int G = (val >> 5) & 0x1F;
            int B = (val >> 10) & 0x1F;
            return Color.FromArgb(0xFF, Convert5To8[R], Convert5To8[G], Convert5To8[B]);
        }
        private static ushort getRGB555(Color c)
        {
            int val = 0;
            // val += c.A >> 8; // unused
            val |= convert8to5(c.R) << 0;
            val |= convert8to5(c.G) << 5;
            val |= convert8to5(c.B) << 10;
            return (ushort)val;
        }
        private static readonly int[] Convert5To8 = { 0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
                                                      0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
                                                      0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
                                                      0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF };
    }
}
