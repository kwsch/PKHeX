using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Go Park Entity transferred from <see cref="GameVersion.GO"/> to <see cref="GameVersion.GG"/>.
    /// </summary>
    public sealed class GP1 : IEncounterInfo
    {
        public const int SIZE = 0x1B0;
        public readonly byte[] Data;

        public GameVersion Version => GameVersion.GO;
        public bool EggEncounter => false;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Generation => 7;
        public PKM ConvertToPKM(ITrainerInfo sav) => ConvertToPB7(sav);
        public PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria) => ConvertToPB7(sav, criteria);

        public GP1(byte[] data) => Data = data;
        public GP1() : this((byte[])Blank.Clone()) { }
        public void WriteTo(byte[] data, int offset) => Data.CopyTo(data, offset);

        public static GP1 FromData(byte[] data, int offset)
        {
            var gpkm = new GP1();
            Array.Copy(data, offset, gpkm.Data, 0, SIZE);
            return gpkm;
        }

        /// <summary>
        /// First 0x20 bytes of an empty <see cref="GP1"/>, all other bytes are 0.
        /// </summary>
        private static readonly byte[] Blank20 =
        {
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F,
            0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x85, 0xEC, 0x33, 0x01,
        };

        public static readonly byte[] Blank = GetBlank();

        public static byte[] GetBlank()
        {
            byte[] data = new byte[SIZE];
            Blank20.CopyTo(data, 0x20);
            return data;
        }

        public string Username1 => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x00, 0x10));
        public string Username2 => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x10, 0x20));

        public int Species => BitConverter.ToInt32(Data, 0x28);
        public int CP => BitConverter.ToInt32(Data, 0x2C);
        public float LevelF => BitConverter.ToSingle(Data, 0x30);
        public int Level => Math.Max(1, (int)Math.Round(LevelF));
        public int Stat_HP => BitConverter.ToInt32(Data, 0x34);
        // geolocation data 0x38-0x47?
        public float HeightF => BitConverter.ToSingle(Data, 0x48);
        public float WeightF => BitConverter.ToSingle(Data, 0x4C);

        public byte HeightScalar
        {
            get
            {
                var height = HeightF * 100f;
                var pi = PersonalTable.GG.GetFormEntry(Species, Form);
                var avgHeight = pi.Height;
                return PB7.GetHeightScalar(height, avgHeight);
            }
        }

        public byte WeightScalar
        {
            get
            {
                var height = HeightF * 100f;
                var weight = WeightF * 10f;
                var pi = PersonalTable.GG.GetFormEntry(Species, Form);
                var avgHeight = pi.Height;
                var avgWeight = pi.Weight;
                return PB7.GetWeightScalar(height, weight, avgHeight, avgWeight);
            }
        }

        public int IV_HP => BitConverter.ToInt32(Data, 0x50);
        public int IV_ATK => BitConverter.ToInt32(Data, 0x54);
        public int IV_DEF => BitConverter.ToInt32(Data, 0x58);
        public int Date => BitConverter.ToInt32(Data, 0x5C); // ####.##.## YYYY.MM.DD
        public int Year => Date / 1_00_00;
        public int Month => (Date / 1_00) % 1_00;
        public int Day => Date % 1_00;

        public int Gender => Data[0x70] - 1; // M=1, F=2, G=3 ;; shift down by 1.

        public int Form => Data[0x72];
        public bool IsShiny => Data[0x73] == 1;

        // https://bulbapedia.bulbagarden.net/wiki/List_of_moves_in_Pok%C3%A9mon_GO
        public int Move1 => BitConverter.ToInt32(Data, 0x74); // uses Go Indexes
        public int Move2 => BitConverter.ToInt32(Data, 0x78); // uses Go Indexes

        public string GeoCityName => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x7C, 0x60)); // dunno length

        public string Nickname => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x12D, 0x20)); // dunno length

        public static readonly IReadOnlyList<string> Genders = GameInfo.GenderSymbolASCII;
        public string GenderString => (uint) Gender >= Genders.Count ? string.Empty : Genders[Gender];
        public string ShinyString => IsShiny ? "★ " : string.Empty;
        public string FormString => Form != 0 ? $"-{Form}" : string.Empty;
        private string NickStr => string.IsNullOrWhiteSpace(Nickname) ? SpeciesName.GetSpeciesNameGeneration(Species, (int)LanguageID.English, 7) : Nickname;
        public string FileName => $"{FileNameWithoutExtension}.gp1";

        public string FileNameWithoutExtension
        {
            get
            {
                string form = Form > 0 ? $"-{Form:00}" : string.Empty;
                string star = IsShiny ? " ★" : string.Empty;
                return $"{Species:000}{form}{star} - {NickStr} - Lv. {Level:00} - {IV_HP:00}.{IV_ATK:00}.{IV_DEF:00} - CP {CP:0000} (Moves {Move1:000}, {Move2:000})";
            }
        }

        public string GeoTime => $"Captured in {GeoCityName} by {Username1} on {Year}/{Month:00}/{Day:00}";
        public string StatMove => $"{IV_HP:00}/{IV_ATK:00}/{IV_DEF:00}, CP {CP:0000} (Moves {Move1:000}, {Move2:000})";
        public string Dump(IReadOnlyList<string> speciesNames, int index) => $"{index:000} {Nickname} ({speciesNames[Species]}{FormString} {ShinyString}[{GenderString}]) @ Lv. {Level:00} - {StatMove}, {GeoTime}.";

        public PB7 ConvertToPB7(ITrainerInfo sav) => ConvertToPB7(sav, EncounterCriteria.Unrestricted);

        public PB7 ConvertToPB7(ITrainerInfo sav, EncounterCriteria criteria)
        {
            var pk = new PB7
            {
                Version = (int) GameVersion.GO,
                Species = Species,
                Form = Form,
                Met_Location = 50, // Go complex
                Met_Year = Year - 2000,
                Met_Month = Month,
                Met_Day = Day,
                CurrentLevel = Level,
                Met_Level = Level,
                TID = sav.TID,
                SID = sav.SID,
                OT_Name = sav.OT,
                Ball = 4,
                Language = sav.Language,
                PID = Util.Rand32(),
            };

            var nick = Nickname;
            if (!string.IsNullOrWhiteSpace(nick))
            {
                pk.Nickname = nick;
                pk.IsNicknamed = true;
            }
            else
            {
                pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, sav.Language, 7);
            }

            pk.IV_DEF = pk.IV_SPD = (IV_DEF * 2) + 1;
            pk.IV_ATK = pk.IV_SPA = (IV_ATK * 2) + 1;
            pk.IV_HP = (IV_HP * 2) + 1;
            pk.IV_SPE = Util.Rand.Next(32);

            var pi = pk.PersonalInfo;
            const int av = 3;
            pk.Gender = criteria.GetGender(Gender, pi);
            pk.Nature = (int)criteria.GetNature(Nature.Random);
            pk.RefreshAbility(criteria.GetAbilityFromType(av, pi));

            bool isShiny = pk.IsShiny;
            if (IsShiny && !isShiny) // Force Square
                pk.PID = (uint)(((sav.TID ^ sav.SID ^ (pk.PID & 0xFFFF) ^ 0) << 16) | (pk.PID & 0xFFFF));
            else if (isShiny)
                pk.PID ^= 0x1000_0000;

            var moves = MoveLevelUp.GetEncounterMoves(pk, Level, GameVersion.GO);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            pk.HeightScalar = HeightScalar;
            pk.WeightScalar = WeightScalar;

            pk.AwakeningSetAllTo(2);
            pk.ResetCalculatedValues();

            pk.SetRandomEC();
            return pk;
        }
    }
}