using System;

namespace PKHeX
{
    public class PCD
    {
        internal const int Size = 0x358; // 856

        public byte[] Data;
        public PCD(byte[] data = null)
        {
            Data = data ?? new byte[Size];

            byte[] giftData = new byte[PGT.Size];
            Array.Copy(Data, 0, giftData, 0, PGT.Size);
            Gift = new PGT(giftData);

            Information = new byte[Data.Length - PGT.Size];
            Array.Copy(Data, PGT.Size, Information, 0, Information.Length);
        }
        public readonly PGT Gift;

        public readonly byte[] Information;
        /* Big thanks to Grovyle91's Pokémon Mystery Gift Editor, from which the structure was referenced.
         * http://projectpokemon.org/forums/member.php?829-Grovyle91
         * http://projectpokemon.org/forums/showthread.php?6524
         * See also: http://tccphreak.shiny-clique.net/debugger/pcdfiles.htm
         */
    }
    public class PGT
    {
        internal static int Size = 0x104; // 260

        public byte[] Data;
        public PGT(byte[] data = null)
        {
            Data = data ?? new byte[Size];
            byte[] ekdata = new byte[PK4.SIZE_PARTY];
            Array.Copy(Data, 8, ekdata, 0, ekdata.Length);
            // Decrypt PK4
            PK = new PK4(PKM.decryptArray(ekdata, BitConverter.ToUInt16(ekdata, 6)));
            
            Unknown = new byte[0x10];
            Array.Copy(Data, 0xF4, Unknown, 0, 0x10);
        }

        public byte CardType { get { return Data[0]; } set { Data[0] = value; } }
        // Unused 0x01
        public byte Slot { get { return Data[2]; } set { Data[2] = value; } }
        public byte Detail { get { return Data[3]; } set { Data[3] = value; } }
        public PK4 PK;
        public byte[] Unknown;

        public bool IsPokémon { get { return CardType == 1; } set { if (value) CardType = 1; } }
        public bool IsEgg { get { return CardType == 2; } set { if (value) CardType = 2; } }
        public bool IsManaphyEgg { get { return CardType == 7; } set { if (value) CardType = 7; } }
        public bool PokémonGift => IsPokémon || IsEgg || IsManaphyEgg;

        public PK4 convertToPK4(SAV6 SAV)
        {
            if (!PokémonGift)
                return null;

            PK4 pk4 = new PK4(PK.Data);
            if (!IsPokémon && Detail == 0)
            {
                pk4.OT_Name = "PKHeX";
                pk4.TID = 12345;
                pk4.SID = 54321;
                pk4.OT_Gender = (int)(Util.rnd32()%2);
            }
            if (IsManaphyEgg)
            {
                // Since none of this data is populated, fill in default info.
                pk4.Species = 490;
                // Level 1 Moves
                pk4.Move1 = 294;
                pk4.Move2 = 145;
                pk4.Move3 = 346;
                pk4.FatefulEncounter = true;
                pk4.Ball = 4;
                pk4.Version = 10; // Diamond
                pk4.Language = 2; // English
                pk4.Nickname = "MANAPHY";
                pk4.Egg_Location = 1; // Ranger (will be +3000 later)
            }

            // Generate IV
            uint seed = Util.rnd32();
            if (pk4.PID == 1) // Create Nonshiny
            {
                uint pid1 = PKM.LCRNG(ref seed) >> 16;
                uint pid2 = PKM.LCRNG(ref seed) >> 16;

                while ((pid1 ^ pid2 ^ pk4.TID ^ pk4.SID) < 8)
                {
                    uint testPID = pid1 | pid2 << 16;

                    // Call the ARNG to change the PID
                    testPID = testPID * 0x6c078965 + 1;

                    pid1 = testPID & 0xFFFF;
                    pid2 = testPID >> 16;
                }
                pk4.PID = pid1 | (pid2 << 16);
            }

            // Generate IVs
            if (pk4.IV32 == 0)
            {
                uint iv1 = PKM.LCRNG(ref seed) >> 16;
                uint iv2 = PKM.LCRNG(ref seed) >> 16;
                pk4.IV32 = (iv1 | iv2 << 16) & 0x3FFFFFFF;
            }

            // Generate Met Info
            DateTime dt = DateTime.Now;
            if (IsPokémon)
            {
                pk4.Met_Location = pk4.Egg_Location + 3000;
                pk4.Egg_Location = 0;
                pk4.Met_Day = dt.Day;
                pk4.Met_Month = dt.Month;
                pk4.Met_Year = dt.Year - 2000;
                pk4.IsEgg = false;
            }
            else
            {
                pk4.Egg_Location = pk4.Egg_Location + 3000;
                pk4.Egg_Day = dt.Day;
                pk4.Egg_Month = dt.Month;
                pk4.Egg_Year = dt.Year - 2000;
                pk4.IsEgg = false;
                // Met Location is modified when transferred to pk5; don't worry about it.
            }
            if (pk4.Species == 201) // Never will be true; Unown was never distributed.
                pk4.AltForm = PKM.getUnownForm(pk4.PID);

            return pk4;
        }
    }
}
