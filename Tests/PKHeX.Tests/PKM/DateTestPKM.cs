using System;
using PKHeX.Core;

namespace PKHeX.Tests.PKM
{
    /// <summary>
    /// A <see cref="PKM"/> implementation designed to test <see cref="PKM.MetDate"/> and <see cref="PKM.EggMetDate"/>.
    /// </summary>
    internal class DateTestPKM : Core.PKM
    {
        public int MetYear { get; set; }
        public int MetMonth { get; set; }
        public int MetDay { get; set; }
        public int EggMetYear { get; set; }
        public int EggMetMonth { get; set; }
        public int EggMetDay { get; set; }
        public override PersonalInfo PersonalInfo => null;

        public override string getString(int Offset, int Count) { throw new NotImplementedException(); }
        public override byte[] setString(string value, int maxLength) { throw new NotImplementedException(); }

        public override byte[] Nickname_Trash
        {
            get => throw new NotImplementedException(); set => throw new NotImplementedException();
        }
        public override byte[] OT_Trash
        {
            get => throw new NotImplementedException(); set => throw new NotImplementedException();
        }

        public override int Met_Year
        {
            get => MetYear;

            set => MetYear = value;
        }

        public override int Met_Month
        {
            get => MetMonth;

            set => MetMonth = value;
        }

        public override int Met_Day
        {
            get => MetDay;

            set => MetDay = value;
        }

        public override int Egg_Year
        {
            get => EggMetYear;

            set => EggMetYear = value;
        }

        public override int Egg_Month
        {
            get => EggMetMonth;

            set => EggMetMonth = value;
        }

        public override int Egg_Day
        {
            get => EggMetDay;

            set => EggMetDay = value;
        }


        #region NotImplemented Properties        

        public override int Ability
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int AltForm
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Ball
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Characteristic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ushort Checksum
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CNT_Beauty
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CNT_Cool
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CNT_Cute
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CNT_Sheen
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CNT_Smart
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CNT_Tough
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CurrentFriendship
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int CurrentHandler
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Egg_Location
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override uint EncryptionConstant
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int EV_ATK
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int EV_DEF
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int EV_HP
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int EV_SPA
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int EV_SPD
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int EV_SPE
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override uint EXP
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override bool FatefulEncounter
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Format
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Gender
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int HeldItem
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override bool IsEgg
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override bool IsNicknamed
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int IV_ATK
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int IV_DEF
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int IV_HP
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int IV_SPA
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int IV_SPD
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int IV_SPE
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Language
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int MarkValue
        {
            get => throw new NotImplementedException();

            protected set => throw new NotImplementedException();
        }

        public override int Met_Level
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Met_Location
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move1
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move1_PP
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move1_PPUps
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move2
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move2_PP
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move2_PPUps
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move3
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move3_PP
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move3_PPUps
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move4
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move4_PP
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Move4_PPUps
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Nature
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override string Nickname
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int OT_Friendship
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int OT_Gender
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override string OT_Name
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override uint PID
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int PKRS_Days
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int PKRS_Strain
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int PSV
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ushort Sanity
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int SID
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int SIZE_PARTY
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int SIZE_STORED
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Species
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_ATK
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_DEF
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_HPCurrent
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_HPMax
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_Level
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_SPA
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_SPD
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int Stat_SPE
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int TID
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override int TSV
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Version
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

        public override Core.PKM Clone()
        {
            throw new NotImplementedException();
        }

        public override byte[] Encrypt()
        {
            throw new NotImplementedException();
        }

        public override bool getGenderIsValid()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
