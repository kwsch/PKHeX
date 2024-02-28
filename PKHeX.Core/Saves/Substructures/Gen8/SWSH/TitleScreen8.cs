using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokémon Team that shows up at the title screen.
/// </summary>
public sealed class TitleScreen8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data)
{
    /// <summary>
    /// Gets an object that exposes the data of the corresponding party <see cref="index"/>.
    /// </summary>
    public TitleScreen8Poke ViewPoke(int index)
    {
        int ofs = GetPokeOffset(index);
        var raw = block.Data.AsMemory(ofs, TitleScreen8Poke.SIZE);
        return new TitleScreen8Poke(raw);
    }

    private static int GetPokeOffset(int index)
    {
        if ((uint)index >= 6)
            throw new ArgumentOutOfRangeException(nameof(index));
        return index * TitleScreen8Poke.SIZE;
    }

    /// <summary>
    /// Applies the current <see cref="SaveFile.PartyData"/> to the block.
    /// </summary>
    public void SetPartyData() => LoadTeamData(SAV.PartyData);

    public void LoadTeamData(IList<PKM> party)
    {
        for (int i = 0; i < party.Count; i++)
            ViewPoke(i).LoadFrom(party[i]);
        for (int i = party.Count; i < 6; i++)
            ViewPoke(i).Clear();
    }
}

public sealed class TitleScreen8Poke(Memory<byte> raw) : ISpeciesForm
{
    public const int SIZE = 0x28;

    private Span<byte> Data => raw.Span;

    public ushort Species
    {
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public byte Form
    {
        get => Data[0x04];
        set => WriteInt32LittleEndian(Data[0x04..], value);
    }

    public byte Gender
    {
        get => (byte)ReadInt32LittleEndian(Data[0x08..]);
        set => WriteInt32LittleEndian(Data[0x08..], value);
    }

    public bool IsShiny
    {
        get => Data[0xC] != 0;
        set => Data[0xC] = value ? (byte)1 : (byte)0;
    }

    public uint EncryptionConstant
    {
        get => ReadUInt32LittleEndian(Data[0x10..]);
        set => WriteUInt32LittleEndian(Data[0x10..], value);
    }

    public int FormArgument
    {
        get => ReadInt32LittleEndian(Data[0x14..]);
        set => WriteInt32LittleEndian(Data[0x14..], value);
    }

    public uint Unknown18
    {
        get => ReadUInt32LittleEndian(Data[0x18..]);
        set => WriteUInt32LittleEndian(Data[0x18..], value);
    }

    public uint Unknown1C
    {
        get => ReadUInt32LittleEndian(Data[0x1C..]);
        set => WriteUInt32LittleEndian(Data[0x1C..], value);
    }

    public uint Unknown20
    {
        get => ReadUInt32LittleEndian(Data[0x20..]);
        set => WriteUInt32LittleEndian(Data[0x20..], value);
    }

    public uint Unknown24
    {
        get => ReadUInt32LittleEndian(Data[0x24..]);
        set => WriteUInt32LittleEndian(Data[0x24..], value);
    }

    public void Clear() => Data.Clear();

    public void LoadFrom(PKM pk)
    {
        Species = pk.Species;
        Form = pk.Form;
        Gender = pk.Gender;
        IsShiny = pk.IsShiny;
        EncryptionConstant = pk.EncryptionConstant;
        FormArgument = pk is IFormArgument f && pk.Species == (int)Core.Species.Alcremie ? (int)f.FormArgument : -1;
    }

    public void LoadFrom(TrainerCard8Poke pk)
    {
        Species = pk.Species;
        Form = pk.Form;
        Gender = pk.Gender;
        IsShiny = pk.IsShiny;
        EncryptionConstant = pk.EncryptionConstant;
        FormArgument = pk.FormArgument;
    }
}
