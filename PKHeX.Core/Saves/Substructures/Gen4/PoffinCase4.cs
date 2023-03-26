using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

/// <summary>
/// Storage for all <see cref="Poffin4"/> the trainer has.
/// </summary>
public sealed class PoffinCase4
{
    private readonly SAV4Sinnoh SAV;
    private readonly int Offset;
    public readonly Poffin4[] Poffins;

    private const int Count = 100;

    public PoffinCase4(SAV4Sinnoh sav)
    {
        SAV = sav;

        Offset = sav.OFS_PoffinCase;
        Poffins = ReadPoffins(SAV, Offset);
    }

    public void Save() => WritePoffins(SAV, Offset, Poffins);

    private static Poffin4[] ReadPoffins(SAV4Sinnoh sav, int offset)
    {
        var result = new Poffin4[Count];
        for (int i = 0; i < result.Length; i++)
        {
            var ofs = offset + (i * Poffin4.SIZE);
            var span = sav.General.Slice(ofs, Poffin4.SIZE);
            result[i] = new Poffin4(span.ToArray());
        }
        return result;
    }

    private static void WritePoffins(SAV4Sinnoh sav, int offset, IReadOnlyList<Poffin4> poffins)
    {
        Debug.Assert(poffins.Count == Count);
        for (int i = 0; i < poffins.Count; i++)
        {
            var ofs = offset + (i * Poffin4.SIZE);
            var span = sav.General.Slice(ofs, Poffin4.SIZE);
            sav.SetData(span, poffins[i].Data);
        }
    }

    public void FillCase()
    {
        foreach (var p in Poffins)
            p.SetAll();
    }

    public void DeleteAll()
    {
        foreach (var p in Poffins)
            p.Delete();
    }
}
