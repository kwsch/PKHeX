using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using PKHeX.Core.Properties;

namespace PKHeX.WinForms
{
    public static class FontUtil
    {

        // Font Related
#if WINDOWS
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
#endif

        private static readonly PrivateFontCollection s_FontCollection = new PrivateFontCollection();
        private static FontFamily[] FontFamilies
        {
            get
            {
                if (s_FontCollection.Families.Length == 0) setPKXFont();
                return s_FontCollection.Families;
            }
        }
        public static Font getPKXFont(float size)
        {
            return new Font(FontFamilies[0], size);
        }
        private static void setPKXFont()
        {
            try
            {
                byte[] fontData = Resources.pgldings_normalregular;
#if WINDOWS
                IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                s_FontCollection.AddMemoryFont(fontPtr, Resources.pgldings_normalregular.Length); uint dummy = 0;
                AddFontMemResourceEx(fontPtr, (uint)Resources.pgldings_normalregular.Length, IntPtr.Zero, ref dummy);
                Marshal.FreeCoTaskMem(fontPtr);
#else
                GCHandle fontHandle = GCHandle.Alloc(fontData, GCHandleType.Pinned);
 				s_FontCollection.AddMemoryFont(fontHandle.AddrOfPinnedObject(), fontData.Length);
 				fontHandle.Free();
#endif

            }
            catch (Exception ex) { Console.WriteLine("Unable to add ingame font: " + ex.Message); }
        }
    }
}
