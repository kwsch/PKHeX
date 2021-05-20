using System;

namespace PKHeX.Core
{
    internal sealed class SAV2Offsets
    {
        public SAV2Offsets(SAV2 sav)
        {
            Options = 0x2000;
            Trainer1 = 0x2009;

            if (sav.Japanese)
                LoadOffsetsJapanese(sav.Version);
            else if (sav.Korean)
                LoadOffsetsKorean();
            else
                LoadOffsetsInternational(sav.Version);
            Daycare = PokedexSeen + 0x1F + 28 + 1; // right after first unown seen
            EventConst = EventFlag - 0x100;
        }

        public int RTCFlags { get; private set; } = -1;

        public int Options { get; }
        public int Trainer1 { get; }

        public int DaylightSavings { get; private set; } = -1;
        public int TimePlayed { get; private set; } = -1;
        public int Palette { get; private set; } = -1;
        public int Money { get; private set; } = -1;
        public int JohtoBadges { get; private set; } = -1;
        public int CurrentBoxIndex { get; private set; } = -1;
        public int BoxNames { get; private set; } = -1;
        public int Party { get; private set; } = -1;
        public int PokedexCaught { get; private set; } = -1;
        public int PokedexSeen { get; private set; } = -1;
        public int CurrentBox { get; private set; } = -1;
        public int OtherCurrentBox { get; private set; }
        public int Gender { get; private set; } = -1;
        public int AccumulatedChecksumEnd { get; private set; } = -1;
        public int OverallChecksumPosition { get; private set; } = -1;
        public int EventFlag { get; private set; } = -1;
        public int EventConst { get; }
        public int Daycare { get; }

        public int PouchTMHM { get; private set; } = -1;
        public int PouchItem { get; private set; } = -1;
        public int PouchKey { get; private set; } = -1;
        public int PouchBall { get; private set; } = -1;
        public int PouchPC { get; private set; } = -1;

        private void LoadOffsetsInternational(GameVersion Version)
        {
            RTCFlags = 0x0C60;

            DaylightSavings = 0x2042;
            OtherCurrentBox = 0x284C;
            switch (Version)
            {
                case GameVersion.GS:
                    TimePlayed = 0x2053;
                    Palette = 0x206B;
                    Money = 0x23DB;
                    JohtoBadges = 0x23E4;
                    CurrentBoxIndex = 0x2724;
                    BoxNames = 0x2727;
                    Party = 0x288A;
                    PokedexCaught = 0x2A4C;
                    PokedexSeen = 0x2A6C;
                    CurrentBox = 0x2D6C;
                    Gender = -1; // No gender in GS
                    AccumulatedChecksumEnd = 0x2D68;
                    OverallChecksumPosition = 0x2D69;
                    OverallChecksumPosition2 = 0x7E6D;

                    PouchTMHM = 0x23E6;
                    PouchItem = 0x241F;
                    PouchKey  = 0x2449;
                    PouchBall = 0x2464;
                    PouchPC   = 0x247E;

                    EventFlag = CurrentBoxIndex - 0x105;
                    break;
                case GameVersion.C:
                    TimePlayed = 0x2052;
                    Palette = 0x206A;
                    Money = 0x23DC;
                    JohtoBadges = 0x23E5;
                    CurrentBoxIndex = 0x2700;
                    BoxNames = 0x2703;
                    Party = 0x2865;
                    PokedexCaught = 0x2A27;
                    PokedexSeen = 0x2A47;
                    CurrentBox = 0x2D10;
                    Gender = 0x3E3D;
                    AccumulatedChecksumEnd = 0x2B82;
                    OverallChecksumPosition = 0x2D0D;
                    OverallChecksumPosition2 = 0x7F0D;

                    PouchTMHM = 0x23E7;
                    PouchItem = 0x2420;
                    PouchKey  = 0x244A;
                    PouchBall = 0x2465;
                    PouchPC   = 0x247F;

                    EventFlag = CurrentBoxIndex - 0x100;
                    break;

                default:
                    throw new ArgumentException(nameof(Version));
            }
        }

        private void LoadOffsetsJapanese(GameVersion Version)
        {
            DaylightSavings = 0x2029;
            TimePlayed = 0x2034;
            Palette = 0x204C;
            CurrentBox = 0x2D10;
            OtherCurrentBox = 0x2842;

            switch (Version)
            {
                case GameVersion.GS:
                    RTCFlags = 0x1000;

                    Money = 0x23BC;
                    JohtoBadges = 0x23C5;
                    CurrentBoxIndex = 0x2705;
                    BoxNames = 0x2708;
                    Party = 0x283E;
                    PokedexCaught = 0x29CE;
                    PokedexSeen = 0x29EE;
                    Gender = -1; // No gender in GS
                    AccumulatedChecksumEnd = 0x2C8B;
                    OverallChecksumPosition = 0x2D0D;
                    OverallChecksumPosition2 = 0x7F0D;

                    PouchTMHM = 0x23C7;
                    PouchItem = 0x2400;
                    PouchKey  = 0x242A;
                    PouchBall = 0x2445;
                    PouchPC   = 0x245F;

                    EventFlag = CurrentBoxIndex - 0x105;
                    break;
                case GameVersion.C:
                    RTCFlags = 0x0C80;

                    Money = 0x23BE;
                    JohtoBadges = 0x23C7;
                    CurrentBoxIndex = 0x26E2;
                    BoxNames = 0x26E5;
                    Party = 0x281A;
                    PokedexCaught = 0x29AA;
                    PokedexSeen = 0x29CA;
                    Gender = 0x8000;
                    AccumulatedChecksumEnd = 0x2AE2;
                    OverallChecksumPosition = 0x2D0D;
                    OverallChecksumPosition2 = 0x7F0D;

                    PouchTMHM = 0x23C9;
                    PouchItem = 0x2402;
                    PouchKey  = 0x242C;
                    PouchBall = 0x2447;
                    PouchPC   = 0x2461;

                    EventFlag = CurrentBoxIndex - 0x100;
                    break;

                default:
                    throw new ArgumentException(nameof(Version));
            }
        }

        public int OverallChecksumPosition2 { get; set; }

        private void LoadOffsetsKorean()
        {
            RTCFlags = 0x1060;

            // No Crystal Version
            DaylightSavings = 0x2042;
            OtherCurrentBox = 0x284C;

            TimePlayed = 0x204D;
            Palette = 0x2065;
            Money = 0x23D3;
            JohtoBadges = 0x23DC;
            BoxNames = 0x26FF;
            Party = 0x28CC;
            PokedexCaught = 0x2A8E;
            PokedexSeen = 0x2AAE;
            CurrentBox = 0x2DAE;
            CurrentBoxIndex = 0x26FC;
            Gender = -1; // No gender in GS
            AccumulatedChecksumEnd = 0x2DAA;
            OverallChecksumPosition = 0x2DAB;
            OverallChecksumPosition2 = 0x7E6B;

            PouchTMHM = 0x23DE;
            PouchItem = 0x2417;
            PouchKey = 0x2441;
            PouchBall = 0x245C;
            PouchPC = 0x2476;

            EventFlag = CurrentBoxIndex - 0x105;
        }
    }
}
