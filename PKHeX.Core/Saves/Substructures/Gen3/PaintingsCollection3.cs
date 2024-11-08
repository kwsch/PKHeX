using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
public sealed class PaintingsCollection3
{
    private List<Paintings3> paintings;

    public PaintingsCollection3()
    {
        paintings = new();
    }

    public PaintingsCollection3(List<Paintings3> paintings)
    {
        this.paintings = paintings;
    }

    public PaintingsCollection3(Int32 first_offset, Int32 first_length, Int32 second_offset, Int32 second_length, int language, SAV3 sav)
    {
        paintings = new();
        byte[] sector = sav.Large.AsSpan().Slice(first_offset, first_length).Slice(second_offset, second_length).ToArray();
        for (int i = 0; i < 5; i++)
        {
            paintings.Add(new Paintings3(sector.AsSpan().Slice(i * 32, 32).ToArray(), language, i));
            paintings[i].Enabled = sav.GetEventFlag(paintings[i].Address);
        }
    }

    public void SaveCollection3(Int32 first_offset, Int32 first_length, Int32 second_offset, Int32 second_length, int language, SAV3 sav)
    {
        byte[] sector = new byte[160];
        for (int i = 0; i < 5; i++)
        {
            Array.Copy(paintings[i].Data, 0, sector, i * 32, 32);
            sav.SetEventFlag(paintings[i].Address, paintings[i].Enabled);
        }
        sector.AsSpan().CopyTo(sav.Large.AsSpan().Slice(first_offset, first_length).Slice(second_offset, second_length));
    }

    public void FixPaintings()
    {
        for (int i = 0; i < 5; i++)
        {
            if (!paintings[i].Enabled || paintings[i].Species == 0)
                paintings[i] = new Paintings3(paintings[i].Language, paintings[i].Category);
            else
                paintings[i].Category = paintings[i].Category;
            }
        }

    public List<Paintings3> Paintings
    {
        get => paintings;
        set => paintings = value;
    }
}
}
