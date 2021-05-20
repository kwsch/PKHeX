using System;

namespace PKHeX.Core
{
    public sealed class QRPK7 : IEncounterInfo
    {
        public GameVersion Version => (GameVersion)CassetteVersion;
        public bool EggEncounter => false;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Generation => Version.GetGeneration();
        public bool IsShiny => false;

        private readonly byte[] Data;
        public const int SIZE = 0x30;
        public QRPK7(byte[] d) => Data = (byte[])d.Clone();

        public uint EncryptionConstant => BitConverter.ToUInt32(Data, 0);
        public int HT_Flags => Data[4];
        public int Unk_5 => Data[5];
        public int Unk_6 => Data[6];
        public int Unk_7 => Data[7];
        public int Move1_PPUps => Data[8];
        public int Move2_PPUps => Data[9];
        public int Move3_PPUps => Data[0xA];
        public int Move4_PPUps => Data[0xB];
        public uint IV32 { get => BitConverter.ToUInt32(Data, 0xC); set => BitConverter.GetBytes(value).CopyTo(Data, 0xC); }
        public int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); }
        public int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); }
        public int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); }
        public int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); }
        public int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); }
        public int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); }
        public uint PID => BitConverter.ToUInt32(Data, 0x10);
        public int Species => BitConverter.ToUInt16(Data, 0x14);
        public ushort HeldItem => BitConverter.ToUInt16(Data, 0x16);
        public ushort Move1 => BitConverter.ToUInt16(Data, 0x18);
        public ushort Move2 => BitConverter.ToUInt16(Data, 0x1A);
        public ushort Move3 => BitConverter.ToUInt16(Data, 0x1C);
        public ushort Move4 => BitConverter.ToUInt16(Data, 0x1E);
        public int Unk_20 => Data[0x20];
        public int AbilityIndex => Data[0x21];
        public int Nature => Data[0x22];
        public bool FatefulEncounter => (Data[0x23] & 1) == 1;
        public int Gender => (Data[0x23] >> 1) & 3;
        public int Form => Data[0x23] >> 3;
        public int EV_HP => Data[0x24];
        public int EV_ATK => Data[0x25];
        public int EV_DEF => Data[0x26];
        public int EV_SPE => Data[0x27];
        public int EV_SPA => Data[0x28];
        public int EV_SPD => Data[0x29];
        public int Unk_2A => Data[0x2A];
        public int Friendship => Data[0x2B];
        public int Ball => Data[0x2C];
        public int Level => Data[0x2D];
        public int CassetteVersion => Data[0x2E];
        public int Language => Data[0x2F];

        /// <summary>
        /// Converts the <see cref="Data"/> to a rough PKM.
        /// </summary>
        public PKM ConvertToPKM(ITrainerInfo sav) => ConvertToPKM(sav, EncounterCriteria.Unrestricted);

        /// <summary>
        /// Converts the <see cref="Data"/> to a rough PKM.
        /// </summary>
        public PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria)
        {
            var pk = new PK7
            {
                EncryptionConstant = EncryptionConstant,
                PID = PID,
                Language = Language,
                Species = Species,
                Gender = Gender,
                Nature = Nature,
                FatefulEncounter = FatefulEncounter,
                Form = Form,
                HyperTrainFlags = HT_Flags,
                IV_HP = IV_HP,
                IV_ATK = IV_ATK,
                IV_DEF = IV_DEF,
                IV_SPA = IV_SPA,
                IV_SPD = IV_SPD,
                IV_SPE = IV_SPE,
                EV_HP = EV_HP,
                EV_ATK = EV_ATK,
                EV_DEF = EV_DEF,
                EV_SPA = EV_SPA,
                EV_SPD = EV_SPD,
                EV_SPE = EV_SPE,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                HeldItem = HeldItem,
                HT_Friendship = Friendship,
                OT_Friendship = Friendship,
                Ball = Ball,
                Version = CassetteVersion,

                OT_Name = sav.OT,
                HT_Name = sav.OT,
                CurrentLevel = Level,
                Met_Level = Level,
                MetDate = DateTime.Now,
            };
            PKMConverter.SetConsoleRegionData3DS(pk);

            pk.RefreshAbility(AbilityIndex >> 1);
            pk.ForcePartyData();

            pk.RefreshChecksum();
            return pk;
        }
    }
}
