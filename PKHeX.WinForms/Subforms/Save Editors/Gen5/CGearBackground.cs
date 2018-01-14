using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PKHeX.WinForms
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
        * CGearBackgrounds tilemap (when stored on BW) employs some obfuscation.
        * BW obfuscates by adding 0xA0A0.
        * The obfuscated number is then tweaked by adding 15*(i/17)
        * To reverse, use a similar reverse calculation
        * PSK files are basically raw game rips (obfuscated)
        * CGB files are un-obfuscated / B2W2.
        * Due to BW and B2W2 using different obfuscation adds, PSK files are incompatible between the versions.
        */
        
        public CGearBackground(byte[] data)
        {
            if (data.Length != SIZE_CGB)
                return;

            // decode for easy handling
            if (!IsCGB(data))
            {
                _psk = data;
                data = PSKtoCGB(data);
            }
            else
                _cgb = data;

            byte[] Region1 = data.Take(0x1FE0).ToArray();
            byte[] ColorData = data.Skip(0x1FE0).Take(0x20).ToArray();
            byte[] Region2 = data.Skip(0x2000).Take(0x600).ToArray();

            ColorPalette = new int[0x10];
            for (int i = 0; i < 0x10; i++)
                ColorPalette[i] = GetRGB555_16(BitConverter.ToUInt16(ColorData, i * 2));

            Tiles = new Tile[0xFF];
            for (int i = 0; i < 0xFF; i++)
            {
                byte[] tiledata = new byte[Tile.SIZE_TILE];
                Array.Copy(Region1, i * Tile.SIZE_TILE, tiledata, 0, Tile.SIZE_TILE);

                Tiles[i] = new Tile(tiledata);
                Tiles[i].SetTile(ColorPalette);
            }

            Map = new TileMap(Region2);
        }

        private byte[] _cgb;
        private byte[] _psk;
        private byte[] GetCGB() => _cgb ?? Write();
        private byte[] GetPSK() => _psk ?? CGBtoPSK(Write());
        public byte[] GetSkin(bool B2W2) => B2W2 ? GetCGB() : GetPSK();

        private byte[] Write()
        {
            byte[] data = new byte[SIZE_CGB];
            for (int i = 0; i < Tiles.Length; i++)
                Array.Copy(Tiles[i].Write(), 0, data, i*Tile.SIZE_TILE, Tile.SIZE_TILE);

            for (int i = 0; i < ColorPalette.Length; i++)
                BitConverter.GetBytes(GetRGB555(ColorPalette[i])).CopyTo(data, 0x1FE0 + i*2);

            Array.Copy(Map.Write(), 0, data, 0x2000, 0x600);

            return data;
        }

        private static bool IsCGB(byte[] data)
        {
            if (data.Length != SIZE_CGB)
                return false;

            // check odd bytes for anything not rotation flag
            for (int i = 0x2000; i < 0x2600; i += 2)
                if ((data[i + 1] & ~0b1100) != 0)
                    return false;
            return true;
        }
        private static byte[] CGBtoPSK(byte[] cgb)
        {
            byte[] psk = (byte[])cgb.Clone();
            for (int i = 0x2000; i < 0x2600; i += 2)
            {
                var tileVal = BitConverter.ToUInt16(cgb, i);
                int val = GetPSKValue(tileVal);

                psk[i] = (byte)val;
                psk[i + 1] = (byte)(val >> 8);
            }
            return psk;
        }
        private static int GetPSKValue(ushort val)
        {
            int rot = val & 0xFF00;
            int tile = val & 0x00FF;
            if (tile == 0xFF) // invalid tile?
                tile = 0;
            
            int result = tile + 15 * (tile / 17)
                         + 0xA0A0
                         + rot;
            return result;
        }
        private static byte[] PSKtoCGB(byte[] psk)
        {
            byte[] cgb = (byte[])psk.Clone();
            for (int i = 0x2000; i < 0x2600; i += 2)
            {
                int val = BitConverter.ToUInt16(psk, i);
                int index = ValToIndex(val);

                byte tile = (byte)index;
                byte rot = (byte)(index >> 8);
                if (tile == 0xFF)
                    tile = 0;
                cgb[i] = tile;
                cgb[i + 1] = rot;
            }
            return cgb;
        }

        private int[] ColorPalette;
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
            byte[] data = ImageUtil.GetPixelData(img);
            const int bpp = 4;
            Debug.Assert(data.Length == Width * Height * bpp);

            // get colors
            int[] pixels = new int[data.Length / bpp];
            int[] colors = new int[pixels.Length];
            Buffer.BlockCopy(data, 0, pixels, 0, data.Length);
            for (int i = 0; i < pixels.Length; i++)
                colors[i] = GetRGB555_32(pixels[i]);
            
            var Palette = colors.Distinct().ToArray();
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
                        int c = colors[index];

                        t.ColorChoices[ix%8 + iy*8] = Array.IndexOf(Palette, c);
                    }
                t.SetTile(Palette);
                tiles[i] = t;
            }

            List<Tile> tilelist = new List<Tile> {tiles[0]};
            TileMap tm = new TileMap(new byte[2*Width*Height/64]);
            for (int i = 1; i < tm.TileChoices.Length; i++)
            {
                for (int j = 0; j < tilelist.Count; j++)
                {
                    int rotVal = tiles[i].GetRotationValue(tilelist[j].ColorChoices);
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
            _psk = null;
            _cgb = null;
        }

        private sealed class Tile : IDisposable
        {
            public const int SIZE_TILE = 0x20;
            private const int TileWidth = 8;
            private const int TileHeight = 8;
            public readonly int[] ColorChoices;
            private Bitmap img;
            public void Dispose() => img.Dispose();

            internal Tile(byte[] data = null)
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
            internal void SetTile(int[] Palette)
            {
                var tileData = GetTileData(Palette);
                img = ImageUtil.GetBitmap(tileData, TileWidth, TileHeight);
            }
            private byte[] GetTileData(int[] Palette)
            {
                const int pixels = TileWidth * TileHeight;
                byte[] data = new byte[pixels * 4];
                for (int i = 0; i < pixels; i++)
                {
                    var choice = ColorChoices[i];
                    var val = Palette[choice];
                    var o = 4 * i;
                    data[o + 0] = (byte)(val & 0xFF);
                    data[o + 1] = (byte)(val >> 8 & 0xFF);
                    data[o + 2] = (byte)(val >> 16 & 0xFF);
                    data[o + 3] = (byte)(val >> 24 & 0xFF);
                }
                return data;
            }

            internal byte[] Write()
            {
                byte[] data = new byte[SIZE_TILE];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] |= (byte)(ColorChoices[i*2+0] & 0xF);
                    data[i] |= (byte)((ColorChoices[i*2+1] & 0xF) << 4);
                }
                return data;
            }

            internal Bitmap Rotate(int rotFlip)
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

            internal int GetRotationValue(int[] tileColors)
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
        private sealed class TileMap
        {
            public readonly int[] TileChoices;
            public readonly int[] Rotations;

            internal TileMap(byte[] data)
            {
                TileChoices = new int[data.Length/2];
                Rotations = new int[data.Length/2];
                for (int i = 0; i < data.Length; i += 2)
                {
                    TileChoices[i/2] = data[i];
                    Rotations[i/2] = data[i+1];
                }
            }
            internal byte[] Write()
            {
                byte[] data = new byte[TileChoices.Length * 2];
                for (int i = 0; i < data.Length; i += 2)
                {
                    data[i] = (byte)TileChoices[i/2];
                    data[i+1] = (byte)Rotations[i/2];
                }
                return data;
            }
        }

        private static int ValToIndex(int val)
        {
            if ((val & 0x3FF) < 0xA0 || (val & 0x3FF) > 0x280)
                return ((val & 0x5C00) | 0xFF);
            return ((val % 0x20) + 0x11 * (((val & 0x3FF) - 0xA0) / 0x20)) | (val & 0x5C00);
        }
        
        private static byte Convert8to5(int colorval)
        {
            byte i = 0;
            while (colorval > Convert5To8[i]) i++;
            return i;
        }
        private static int GetRGB555_32(int val)
        {
            var R = (val >> 0 >> 3) & 0x1F;
            var G = (val >> 8 >> 3) & 0x1F;
            var B = (val >> 16 >> 3) & 0x1F;
            return 0xFF << 24 | R << 16 | G << 8 | B;
        }
        private static int GetRGB555_16(ushort val)
        {
            int R = (val >> 0) & 0x1F;
            int G = (val >> 5) & 0x1F;
            int B = (val >> 10) & 0x1F;

            R = Convert5To8[R];
            G = Convert5To8[G];
            B = Convert5To8[B];

            return 0xFF << 24 | R << 16 | G << 8 | B;
        }
        private static ushort GetRGB555(int v)
        {
            var R = (v >> 16) & 0x1F;
            var G = (v >> 8) & 0x1F;
            var B = (v >> 0) & 0x1F;

            int val = 0;
            val |= Convert8to5(R) << 0;
            val |= Convert8to5(G) << 5;
            val |= Convert8to5(B) << 10;
            return (ushort)val;
        }
        private static readonly int[] Convert5To8 = { 0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
                                                      0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
                                                      0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
                                                      0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF };
    }
}
