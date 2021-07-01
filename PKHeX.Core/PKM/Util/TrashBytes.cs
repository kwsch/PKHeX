using System;

namespace PKHeX.Core
{
    public static class TrashBytes
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
            var trash = full_trash[((topLength * 2) + 2)..];
            var nameBytes = StringConverter.SetString7b(under, under.Length, full_trash.Length / 2);
            var span = nameBytes.AsSpan((topLength * 2) + 2);
            return trash.SequenceEqual(span);
        }

        public static bool HasUnderlayer(ReadOnlySpan<byte> full_trash, string under, string top)
        {
            var trash = full_trash[((top.Length * 2) + 2)..];
            var nameBytes = StringConverter.SetString7b(under, under.Length, full_trash.Length / 2);
            var span = nameBytes.AsSpan((top.Length * 2) + 2);
            return trash.SequenceEqual(span);
        }

        public static bool HasFinalTerminator(ReadOnlySpan<byte> buffer, byte terminator = 0) => buffer[^1] == terminator && buffer[^2] == terminator;

        public static int FindLastTrash2(ReadOnlySpan<byte> buffer, int start, byte terminator = 0)
        {
            for (int i = buffer.Length - 2; i > start; i -= 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    continue;
                return i;
            }
            return start;
        }

        public static int FindFirstTrash2(ReadOnlySpan<byte> buffer, int start, byte terminator = 0)
        {
            for (int i = buffer.Length - 2; i > start; i -= 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    return i + 2;
            }
            return 0;
        }

        public static bool HasTrash2(ReadOnlySpan<byte> buffer)
        {
            var terminator = FindTerminator2(buffer);
            return terminator == -1 || !buffer[(terminator + 2)..].IsRangeEmpty();
        }

        public static int FindTerminator2(ReadOnlySpan<byte> buffer, byte terminator = 0)
        {
            for (int i = 0; i < buffer.Length; i += 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    return i;
            }
            return -1;
        }

        public static int FindTerminator1(ReadOnlySpan<byte> buffer, byte terminator = 0)
        {
            for (int i = 0; i < buffer.Length; i += 2)
            {
                if (buffer[i] == terminator)
                    return i;
            }
            return -1;
        }
    }
}