using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Pokémon Team that shows up at the title screen.
    /// </summary>
    public sealed class TitleScreen8 : SaveBlock
    {
        public TitleScreen8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        /// <summary>
        /// Gets an object that exposes the data of the corresponding party <see cref="index"/>.
        /// </summary>
        public TitleScreen8Poke ViewPoke(int index)
        {
            if ((uint)index >= 6)
                throw new ArgumentOutOfRangeException(nameof(index));
            return new TitleScreen8Poke(Data, Offset + 0x00 + (index * TitleScreen8Poke.SIZE));
        }

        /// <summary>
        /// Applies the current <see cref="SaveFile.PartyData"/> to the block.
        /// </summary>
        public void SetPartyData() => LoadTeamData(SAV.PartyData);

        public void LoadTeamData(IList<PKM> party)
        {
            for (int i = 0; i < party.Count; i++)
                ViewPoke(i).LoadFrom(party[i]);
        }
    }

    public class TitleScreen8Poke
    {
        public const int SIZE = 0x28;
        private readonly byte[] Data;
        private readonly int Offset;

        public TitleScreen8Poke(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public int Species
        {
            get => BitConverter.ToInt32(Data, Offset + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00);
        }

        public int AltForm
        {
            get => BitConverter.ToInt32(Data, Offset + 0x04);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x04);
        }

        public int Gender
        {
            get => BitConverter.ToInt32(Data, Offset + 0x08);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08);
        }

        public bool IsShiny
        {
            get => Data[Offset + 0xC] != 0;
            set => Data[Offset + 0xC] = (byte)(value ? 1 : 0);
        }

        public uint EncryptionConstant
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x10);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10);
        }

        public int FormArgument
        {
            get => BitConverter.ToInt32(Data, Offset + 0x14);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x14);
        }

        public uint Unknown18
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x18);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x18);
        }

        public uint Unknown1C
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x1C);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C);
        }

        public uint Unknown20
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x20);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x20);
        }

        public uint Unknown24
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x24);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x24);
        }

        public void LoadFrom(PKM pkm)
        {
            Species = pkm.Species;
            AltForm = pkm.AltForm;
            Gender = pkm.Gender;
            IsShiny = pkm.IsShiny;
            EncryptionConstant = pkm.EncryptionConstant;
            FormArgument = pkm is IFormArgument f && pkm.Species == (int)Core.Species.Alcremie ? (int)f.FormArgument : -1;
        }

        public void LoadFrom(TrainerCard8Poke pkm)
        {
            Species = pkm.Species;
            AltForm = pkm.AltForm;
            Gender = pkm.Gender;
            IsShiny = pkm.IsShiny;
            EncryptionConstant = pkm.EncryptionConstant;
            FormArgument = pkm.FormArgument;
        }
    }
}