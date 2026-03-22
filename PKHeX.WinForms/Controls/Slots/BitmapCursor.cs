using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Managed <see cref="Cursor"/> implementation that acquires the cursor appearance from a <see cref="Bitmap"/> and properly disposes of the underlying icon handle when done.
/// </summary>
public sealed class BitmapCursor : IDisposable
{
    public Cursor Cursor { get; }
    private nint IconHandle;

    public BitmapCursor(Bitmap bitmap)
    {
        IconHandle = bitmap.GetHicon(); // creates a handle we need to dispose of.
        Cursor = new Cursor(IconHandle);
    }

    // Need to use DestroyIcon as Cursor does not take ownership of the icon handle and will not destroy it when disposed.

    #pragma warning disable SYSLIB1054
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(nint hIcon);
    #pragma warning restore SYSLIB1054

    public void Dispose()
    {
        Cursor.Dispose();
        if (IconHandle == nint.Zero)
            return;
        _ = DestroyIcon(IconHandle);
        IconHandle = nint.Zero;
    }
}
