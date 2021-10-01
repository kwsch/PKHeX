namespace PKHeX.Core
{
    public static class EntityFileNamer
    {
        /// <summary>
        /// Object that converts the <see cref="PKM"/> data into a <see cref="string"/> file name.
        /// </summary>
        public static IFileNamer<PKM> Namer { get; set; } = new DefaultEntityNamer();

        /// <summary>
        /// Gets the file name (without extension) for the input <see cref="pk"/> data.
        /// </summary>
        /// <param name="pk">Input entity to create a file name for.</param>
        /// <returns>File name for the <see cref="pk"/> data</returns>
        public static string GetName(PKM pk) => Namer.GetName(pk);
    }

    public sealed class DefaultEntityNamer : IFileNamer<PKM>
    {
        public string GetName(PKM obj)
        {
            if (obj is GBPKM gb)
                return GetGBPKM(gb);
            return GetRegular(obj);
        }

        private static string GetRegular(PKM pk)
        {
            string form = pk.Form > 0 ? $"-{pk.Form:00}" : string.Empty;
            string star = pk.IsShiny ? " ★" : string.Empty;
            return $"{pk.Species:000}{form}{star} - {pk.Nickname} - {pk.Checksum:X4}{pk.EncryptionConstant:X8}";
        }

        private static string GetGBPKM(GBPKM gb)
        {
            string form = gb.Form > 0 ? $"-{gb.Form:00}" : string.Empty;
            string star = gb.IsShiny ? " ★" : string.Empty;
            var raw = gb switch
            {
                PK1 pk1 => new PokeList1(pk1).Write(),
                PK2 pk2 => new PokeList2(pk2).Write(),
                _ => gb.Data,
            };
            var checksum = Checksums.CRC16_CCITT(raw);
            return $"{gb.Species:000}{form}{star} - {gb.Nickname} - {checksum:X4}";
        }
    }

    public interface IFileNamer<in T>
    {
        string GetName(T obj);
    }
}
