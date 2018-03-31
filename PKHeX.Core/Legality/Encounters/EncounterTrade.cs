using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data
    /// </summary>
    /// <remarks>
    /// Trade data is fixed level in all cases except for the first few generations of games.
    /// </remarks>
    public class EncounterTrade : IEncounterable, IMoveset, IGeneration, ILocation, IContestStats, IVersion
    {
        public int Species { get; set; }
        public int[] Moves { get; set; }
        public int Level { get; set; }
        public int LevelMin => Level;
        public int LevelMax => 100;
        public int Generation { get; set; } = -1;

        public int Location { get; set; } = -1;
        public int Ability { get; set; }
        public Nature Nature = Nature.Random;
        public int TID { get; set; }
        public int SID { get; set; }
        public GameVersion Version { get; set; } = GameVersion.Any;
        public int[] IVs { get; set; }
        public int[] Contest { set => this.SetContestStats(value); }
        public int CNT_Cool { get; set; }
        public int CNT_Beauty { get; set; }
        public int CNT_Cute { get; set; }
        public int CNT_Smart { get; set; }
        public int CNT_Tough { get; set; }
        public int CNT_Sheen { get; set; }
        public int Form { get; set; }
        public virtual Shiny Shiny { get; set; } = Shiny.Never;
        public int Gender { get; set; } = -1;
        public int OTGender { get; set; } = -1;
        public bool EggEncounter => false;
        public int EggLocation { get; set; }
        public bool EvolveOnTrade { get; set; }
        public int Ball { get; set; } = 4;
        public int CurrentLevel { get; set; } = -1;

        private const string _name = "In-game Trade";
        public string Name => _name;
        public bool Fateful { get; set; }
        public bool IsNicknamed { get; set; } = true;

        public string[] Nicknames { get; internal set; }
        public string[] TrainerNames { get; internal set; }
        public string GetNickname(int language) => Nicknames?.Length < language ? Nicknames[language] : null;
        public string GetOT(int language) => TrainerNames?.Length < language ? TrainerNames[language] : null;

        public static readonly int[] DefaultMetLocation = 
        {
            0, 126, 254, 2001, 30002, 30001, 30001,
        };

        public PKM ConvertToPKM(ITrainerInfo SAV)
        {
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            int lang = (int)Legal.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = CurrentLevel > 0 ? CurrentLevel : LevelMin;
            var pk = PKMConverter.GetBlank(Generation);

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = Species;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int)version;
            pk.PID = Util.Rand32();
            pk.Ball = Ball;
            pk.Met_Level = LevelMin;
            pk.Met_Location = Location;
            pk.MetDate = DateTime.Today;

            int nature = Nature == Nature.Random ? Util.Rand.Next(25) : (int)Nature;
            pk.Nature = nature;
            int gender = Gender < 0 ? Util.Rand.Next(2) : Gender;
            pk.Gender = pk.GetSaneGender(gender);
            pk.AltForm = Form;

            SAV.ApplyToPKM(pk);
            pk.TID = TID;
            pk.SID = SID;
            pk.OT_Name = GetOT(lang) ?? SAV.OT;
            pk.OT_Gender = GetOT(lang) != null ? OTGender : SAV.Gender;
            pk.SetNickname(GetNickname(lang));
            pk.Language = lang;

            pk.RefreshAbility(Ability >> 1);

            if (IVs != null)
                pk.SetRandomIVs(IVs, 0);
            else
                pk.SetRandomIVs(flawless: 3);

            if (pk.Format == 6)
                pk.SetRandomMemory6();

            if (pk is PK1 pk1 && this is EncounterTradeCatchRate c)
                pk1.Catch_Rate = (int)c.Catch_Rate;

            this.CopyContestStatsTo(pk);

            var moves = Moves ?? Legal.GetEncounterMoves(pk, level, version);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            if (Fateful)
                pk.FatefulEncounter = true;

            if (pk.Format < 6)
                return pk;

            SAV.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();

            return pk;
        }
    }
}
