using System;

namespace PKHeX.Core
{
    /// <summary>
    /// 16-bit character trash byte utility logic.
    /// </summary>
    public static class TrashBytes16
    {
        public static bool HasUnderlayerAnySpecies(ReadOnlySpan<byte> full_trash, int firstTrash, int species, int generation)
        {
            for (int language = 1; language <= (int)LanguageID.ChineseT; language++)
            {
                if (language == (int)LanguageID.Hacked)
                    continue;

                var name = SpeciesName.GetSpeciesNameGeneration(species, language, generation);
                if (HasUnderlayer(full_trash, name, (firstTrash - 2) / 2))
                    return true;
            }
            return false;
        }

        public static bool HasUnderlayer(ReadOnlySpan<byte> full_trash, string under, int topLength)
        {
            var start = (topLength * 2) + 2;
            var trash = full_trash[start..];
            var nameBytes = StringConverter.SetString7b(under, under.Length, full_trash.Length / 2);
            var span = nameBytes.AsSpan(start);
            return trash.SequenceEqual(span);
        }

        public static bool HasUnderlayer(ReadOnlySpan<byte> full_trash, string under, string top) => HasUnderlayer(full_trash, under, top.Length);

        public static bool HasFinalTerminator(ReadOnlySpan<byte> buffer, byte terminator = 0) => buffer[^1] == terminator && buffer[^2] == terminator;

        public static int FindLastTrash(ReadOnlySpan<byte> buffer, int start, byte terminator = 0)
        {
            for (int i = buffer.Length - 2; i > start; i -= 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    continue;
                return i;
            }
            return start;
        }

        public static int FindNextTrashBackwards(ReadOnlySpan<byte> buffer, int start, byte terminator = 0)
        {
            for (int i = start; i > 0; i -= 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    return i + 2;
            }
            return 0;
        }

        public static int FindFirstTrash(ReadOnlySpan<byte> buffer, int start, byte terminator = 0)
        {
            for (int i = buffer.Length - 2; i > start; i -= 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    return i + 2;
            }
            return 0;
        }

        public static bool HasTrash(ReadOnlySpan<byte> buffer)
        {
            var terminator = FindTerminator(buffer);
            return terminator == -1 || !buffer[(terminator + 2)..].IsRangeEmpty();
        }

        public static int FindTerminator(ReadOnlySpan<byte> buffer, byte terminator = 0)
        {
            for (int i = 0; i < buffer.Length; i += 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    return i;
            }
            return -1;
        }
    }
}