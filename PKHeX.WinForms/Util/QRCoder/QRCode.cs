using System.Drawing;

// From: https://github.com/codebude/QRCoder
namespace QRCoder
{
    using System;

    public sealed class QRCode : AbstractQRCode<Bitmap>, IDisposable
    {
        public QRCode(QRCodeData data) : base(data) {}

        public override Bitmap GetGraphic(int pixelsPerModule)
        {
            return this.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
        }

        public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Bitmap(size, size);
            var gfx = Graphics.FromImage(bmp);
            for (var x = 0; x < size + offset; x += pixelsPerModule)
            {
                for (var y = 0; y < size + offset; y += pixelsPerModule)
                {
                    var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule)/pixelsPerModule - 1][(x + pixelsPerModule)/pixelsPerModule - 1];
                    if (module)
                    {
                        gfx.FillRectangle(new SolidBrush(darkColor), new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                    }
                    else
                    {
                        gfx.FillRectangle(new SolidBrush(lightColor), new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                    }
                }
            }

            gfx.Save();
            return bmp;
        }

        public void Dispose()
        {
            this.QrCodeData = null;
        }
    }
}
