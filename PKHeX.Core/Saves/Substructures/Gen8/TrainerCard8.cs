using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core
{
    public sealed class TrainerCard8 : SaveBlock
    {
        public TrainerCard8(SAV8SWSH sav, SCBlock block) : base (sav, block.Data) { }

        public string OT
        {
            get => SAV.GetString(Data, 0x00, 0x1A);
            set => SAV.SetData(Data, SAV.SetString(value, SAV.OTLength), 0x00);
        }

        public byte Language
        {
            get => Data[0x1B];
            set => Data[0x1B] = value; // languageID
        }

        public int TrainerID
        {
            get => BitConverter.ToInt32(Data, 0x1C);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), 0x1C);
        }

        public ushort PokeDexOwned
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x20);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0x20);
        }

        public ushort ShinyPokemonFound
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x22);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0x22);
        }

        public byte Game
        {
            get => Data[0x24];
            set => Data[0x24] = value; // 0 = Sword, 1 = Shield
        }

        public byte Starter
        {
            get => Data[0x25];
            set => Data[0x25] = value; // Grookey=0, Scorbunny=1, Sobble=2
        }

        public ushort CurryTypesOwned
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x26);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0x26);
        }

        public const int RotoRallyScoreMax = 99_999;

        public int RotoRallyScore
        {
            get => BitConverter.ToInt32(Data, 0x28);
            set
            {
                if (value > RotoRallyScoreMax)
                    value = RotoRallyScoreMax;
                var data = BitConverter.GetBytes(value);
                SAV.SetData(Data, data, 0x28);
                // set to the other block since it doesn't have an accessor
                ((SAV8SWSH)SAV).SetValue(SaveBlockAccessor8SWSH.KRotoRally, (uint)value);
            }
        }

        public const int MaxPokemonCaught = 99_999;

        public int CaughtPokemon
        {
            get => BitConverter.ToInt32(Data, 0x2C);
            set
            {
                if (value > MaxPokemonCaught)
                    value = MaxPokemonCaught;
                var data = BitConverter.GetBytes(value);
                SAV.SetData(Data, data, 0x2C);
            }
        }

        public bool PokeDexComplete
        {
            get => Data[Offset + 0x30] == 1;
            set => Data[Offset + 0x30] = value ? 1 : 0;
        }

        public bool ArmorDexComplete
        {
            get => Data[Offset + 0x1B4] == 1;
            set => Data[Offset + 0x1B4] = value ? 1 : 0;
        }

        public bool CrownDexComplete
        {
            get => Data[Offset + 0x1B5] == 1;
            set => Data[Offset + 0x1B5] = value ? 1 : 0;
        }

        public int Gender
        {
            get => Data[0x38];
            set => Data[0x38] = (byte)value;
        }

        public string Number
        {
            get => Encoding.ASCII.GetString(Data, 0x39, 3);
            set
            {
                for (int i = 0; i < 3; i++)
                    Data[0x39 + i] = (byte) (value.Length > i ? value[i] : '\0');
                SAV.State.Edited = true;
            }
        }

        public ulong Skin // aka the base model
        {
            get => BitConverter.ToUInt64(Data, 0x40);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x40);
        }

        public ulong Hair
        {
            get => BitConverter.ToUInt64(Data, 0x48);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x48);
        }

        public ulong Brow
        {
            get => BitConverter.ToUInt64(Data, 0x50);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x50);
        }

        public ulong Lashes
        {
            get => BitConverter.ToUInt64(Data, 0x58);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x58);
        }

        public ulong Contacts
        {
            get => BitConverter.ToUInt64(Data, 0x60);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x60);
        }

        public ulong Lips
        {
            get => BitConverter.ToUInt64(Data, 0x68);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x68);
        }

        public ulong Glasses
        {
            get => BitConverter.ToUInt64(Data, 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x70);
        }

        public ulong Hat
        {
            get => BitConverter.ToUInt64(Data, 0x78);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x78);
        }

        public ulong Jacket
        {
            get => BitConverter.ToUInt64(Data, 0x80);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x80);
        }

        public ulong Top
        {
            get => BitConverter.ToUInt64(Data, 0x88);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x88);
        }

        public ulong Bag
        {
            get => BitConverter.ToUInt64(Data, 0x90);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x90);
        }

        public ulong Gloves
        {
            get => BitConverter.ToUInt64(Data, 0x98);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x98);
        }

        public ulong BottomOrDress
        {
            get => BitConverter.ToUInt64(Data, 0xA0);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0xA0);
        }

        public ulong Sock
        {
            get => BitConverter.ToUInt64(Data, 0xA8);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0xA8);
        }

        public ulong Shoe
        {
            get => BitConverter.ToUInt64(Data, 0xB0);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0xB0);
        }

        public ulong MomSkin // aka the base model
        {
            get => BitConverter.ToUInt64(Data, 0xC0);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0xC0);
        }

        // Trainer Card Pokemon
        // 0xC8 - 0xE3 (0x1C)
        // 0xE4
        // 0x100
        // 0x11C
        // 0x138
        // 0x154 - 0x16F

        /// <summary>
        /// Gets an object that exposes the data of the corresponding party <see cref="index"/>.
        /// </summary>
        public TrainerCard8Poke ViewPoke(int index)
        {
            if ((uint) index >= 6)
                throw new ArgumentOutOfRangeException(nameof(index));
            return new TrainerCard8Poke(Data, Offset + 0xC8 + (index * TrainerCard8Poke.SIZE));
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

        public ushort StartedYear
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x170);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0x170);
        }

        public byte StartedMonth
        {
            get => Data[Offset + 0x172];
            set => Data[Offset + 0x172] = value;
        }

        public byte StartedDay
        {
            get => Data[Offset + 0x173];
            set => Data[Offset + 0x173] = value;
        }

        public uint TimestampPrinted
        {
            // should this be a ulong?
            get => BitConverter.ToUInt32(Data, Offset + 0x1A8);
            set => SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0x1A8);
        }
    }

    public sealed class TrainerCard8Poke : ISpeciesForm
    {
        public const int SIZE = 0x1C;
        private readonly byte[] Data;
        private readonly int Offset;

        public TrainerCard8Poke(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public int Species
        {
            get => BitConverter.ToInt32(Data, Offset + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00);
        }

        public int Form
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
            set => Data[Offset + 0xC] = value ? 1 : 0;
        }

        public uint EncryptionConstant
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x10);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10);
        }

        public uint Unknown
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x14);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x14);
        }

        public int FormArgument
        {
            get => BitConverter.ToInt32(Data, Offset + 0x18);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x18);
        }

        public void Clear() => Array.Clear(Data, Offset, SIZE);

        public void LoadFrom(PKM pkm)
        {
            Species = pkm.Species;
            Form = pkm.Form;
            Gender = pkm.Gender;
            IsShiny = pkm.IsShiny;
            EncryptionConstant = pkm.EncryptionConstant;
            FormArgument = pkm is IFormArgument f && pkm.Species == (int) Core.Species.Alcremie ? (int)f.FormArgument : -1;
        }

        public void LoadFrom(TitleScreen8Poke pkm)
        {
            Species = pkm.Species;
            Form = pkm.Form;
            Gender = pkm.Gender;
            IsShiny = pkm.IsShiny;
            EncryptionConstant = pkm.EncryptionConstant;
            FormArgument = pkm.FormArgument;
        }
    }
}
