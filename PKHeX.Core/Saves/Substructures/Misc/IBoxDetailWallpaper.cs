namespace PKHeX.Core
{
    /// <summary>
    /// Provides details about box wallpaper values within the save file.
    /// </summary>
    public interface IBoxDetailWallpaper
    {
        public int GetBoxWallpaper(int box);
        public void SetBoxWallpaper(int box, int value);
    }
}
