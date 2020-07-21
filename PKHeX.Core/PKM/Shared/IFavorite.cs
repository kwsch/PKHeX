namespace PKHeX.Core
{
    /// <summary>
    /// Can mark as a "Favorite" in <see cref="GameVersion.GG"/>
    /// </summary>
    public interface IFavorite
    {
        /// <summary>
        /// Marked as a "Favorite" in <see cref="GameVersion.GG"/>
        /// </summary>
        bool Favorite { get; set; }
    }
}