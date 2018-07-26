using System;
using System.Linq;

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
        public string GetNickname(int language) => Nicknames?.Length > language ? Nicknames[language] : null;
        public string GetOT(int language) => TrainerNames?.Length > language ? TrainerNames[language] : null;
        public bool HasNickname => Nicknames != null;

        public static readonly int[] DefaultMetLocation =
        {
            0, 126, 254, 2001, 30002, 30001, 30001,
        };

        public PKM ConvertToPKM(ITrainerInfo SAV)
        {
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            int lang = (int)Legal.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = CurrentLevel > 0 ? CurrentLevel : LevelMin;
            if (level == 0)
                level = 25; // avoid some cases
            var pk = PKMConverter.GetBlank(Generation);

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = Species;
            pk.CurrentLevel = level;
            int gender = Gender < 0 ? pk.PersonalInfo.RandomGender : Gender;
            int nature = Nature == Nature.Random ? Util.Rand.Next(25) : (int)Nature;

            SAV.ApplyToPKM(pk);
            pk.Nature = nature;
            pk.Version = (int)version;
            pk.AltForm = Form;

            if (this is EncounterTradePID p)
            {
                pk.PID = p.PID;
                pk.Gender = PKX.GetGenderFromPID(Species, p.PID);
                pk.RefreshAbility(Ability >> 1);
            }
            else
            {
                int ability = Ability >> 1;
                PIDGenerator.SetRandomWildPID(pk, Generation, nature, ability, gender);
                pk.Gender = gender;
                pk.RefreshAbility(ability);
            }
            pk.Ball = Ball;
            if (pk.Format != 2 || version == GameVersion.C)
            {
                pk.Met_Level = level;
                pk.Met_Location = Location > 0 ? Location : DefaultMetLocation[Generation - 1];
            }
            var today = DateTime.Today;
            pk.MetDate = today;
            if (EggLocation != 0)
            {
                pk.Egg_Location = EggLocation;
                pk.EggMetDate = today;
            }

            pk.Language = lang;
            pk.TID = TID;
            pk.SID = SID;
            pk.OT_Name = pk.Format == 1 ? StringConverter.G1TradeOTStr : GetOT(lang) ?? SAV.OT;
            pk.OT_Gender = GetOT(lang) != null ? Math.Max(0, OTGender) : SAV.Gender;
            pk.SetNickname(GetNickname(lang));

            if (IVs != null)
                pk.SetRandomIVs(IVs, 0);
            else
                pk.SetRandomIVs(flawless: 3);

            if (pk.Format == 6)
                pk.SetRandomMemory6();

            if (pk is PK1 pk1 && this is EncounterTradeCatchRate c)
                pk1.Catch_Rate = (int)c.Catch_Rate;

            if (pk is IContestStats s)
                this.CopyContestStatsTo(s);

            var moves = Moves ?? MoveLevelUp.GetEncounterMoves(pk, level, version);
            if (pk.Format == 1 && moves.All(z => z == 0))
                moves = ((PersonalInfoG1)PersonalTable.RB[Species]).Moves;
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            if (Fateful)
                pk.FatefulEncounter = true;

            UpdateEdgeCase(pk);

            if (EvolveOnTrade)
                ++pk.Species;

            if (pk.Format < 6)
                return pk;

            SAV.ApplyHandlingTrainerInfo(pk, force: true);
            pk.SetRandomEC();

            if (pk.Format == 7)
                SetSMOTMemory(pk);

            return pk;
        }

        private void UpdateEdgeCase(PKM pkm)
        {
            switch (Generation)
            {
                case 3 when Species == 124 && pkm.Version == (int) GameVersion.LG && pkm.Language == (int) LanguageID.Italian:
                    // Italian LG Jynx untranslated from English name
                    pkm.OT_Name = GetOT(2);
                    pkm.SetNickname(GetNickname(2));
                    break;

                case 4 when Version == GameVersion.DPPt && Species == 129: // Meister Magikarp
                    // Has German Language ID for all except German origin, which is English
                    pkm.Language = (int)(pkm.Language == (int)LanguageID.German ? LanguageID.English : LanguageID.German);
                    break;

                case 4 when Version == GameVersion.DPPt && (pkm.Version == (int)GameVersion.D || pkm.Version == (int)GameVersion.P):
                    // DP English origin are Japanese lang
                    pkm.Language = 1;
                    break;

                case 4 when Version == GameVersion.HGSS && Species == 25: // Pikachu
                    // Has English Language ID for all except English origin, which is French
                    pkm.Language = (int)(pkm.Language == (int)LanguageID.English ? LanguageID.French : LanguageID.English);
                    break;

                case 5 when Version == GameVersion.BW && pkm.Language == 1:
                    // Trades for JPN games have language ID of 0, not 1.
                    pkm.Language = 0;
                    break;
            }
        }

        private static void SetSMOTMemory(PKM pk)
        {
            pk.OT_Memory = 1;
            pk.OT_Intensity = 3;
            pk.OT_TextVar = 40;
            pk.OT_Feeling = 5;
        }
    }
}
