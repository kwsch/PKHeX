using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PKHeX
{
    public partial class pk2pk
    {
        public byte[] ConvertPKM(byte[] input, byte[] savefile, int savindex)
        {
            #region Initialize Everything

            // Load Species Name Tables
            speclang_ja = Util.getStringList("Species", "ja");
            speclang_en = Util.getStringList("Species", "en");
            speclang_fr = Util.getStringList("Species", "fr");
            speclang_it = Util.getStringList("Species", "it");
            speclang_de = Util.getStringList("Species", "de");
            speclang_es = Util.getStringList("Species", "es");

            // Get the 6th Gen Data from the save file if it exists.
            int savshift = savindex * 0x7F000;

            // Import trainer details for importing
            if (BitConverter.ToUInt32(savefile, 0x6A810 + savshift) == 0x42454546) // Is set for X/Y when a SAV is loaded.
            {
                subreg = savefile[0x19426 + savshift];
                country = savefile[0x19427 + savshift];
                _3DSreg = savefile[0x1942C + savshift];
                g6trname = Util.TrimFromZero(Encoding.Unicode.GetString(savefile, 0x19448 + savshift, 0x1A));
                g6trgend = savefile[0x19405 + savshift];
            }
            else
            {
                country = 0x31; // US
                subreg = 0x7;   // California
                _3DSreg = 0x1;  // Americas
                g6trname = "PKHeX";
                g6trgend = 0;
            }
            #endregion
            #region Convert
            if (input.Length == 100 || input.Length == 80)          // We have a Gen 3 PKM.
            {
                byte[] g4data = convertPK3toPK4(input);
                byte[] g5data = convertPK4toPK5(g4data);
                byte[] g6data = convertPK5toPK6(g5data);
                return g6data;
            }
            else if (input.Length == 136 || input.Length == 236 || input.Length == 220)    // Ambiguous Gen4/5 file.
            {
                if (((BitConverter.ToUInt16(input, 0x80) < 0x3333) && (input[0x5F] < 0x10)) || (BitConverter.ToUInt16(input, 0x46) != 0))
                {
                    // If from Gen4-- && Not Met Location Poketransfer
                    // Or if Pt met data is set... it'd be hard to mess something up but someone will probably do just that.
                    byte[] g5data = convertPK4toPK5(input);
                    byte[] g6data = convertPK5toPK6(g5data);
                    return g6data;
                }
                else return convertPK5toPK6(input);
            }
            #endregion
            else return input; // Should never get here.
        }
        #region Utility
        public DateTime moment = DateTime.Now;
        public string[] speclang_ja;
        public string[] speclang_en;
        public string[] speclang_fr;
        public string[] speclang_it;
        public string[] speclang_de;
        public string[] speclang_es;
        public int country = 0x31; // US
        public int subreg = 0x7;   // California
        public int _3DSreg = 0x1;  // Americas
        public string g6trname = "PKHeX";
        public byte g6trgend = 0;
        private int getAbilityNumber(int species, int ability, int formnum)
        {
               PKX.PersonalParser.Personal MonData = PKX.PersonalGetter.GetPersonal(species, formnum);
               int[] spec_abilities = new int[3];
               Array.Copy(MonData.Abilities, spec_abilities, 3);
               int abilval = Array.IndexOf(spec_abilities, ability);
               if (abilval >= 0)
                   return 1 << abilval;
               else return -1;
        }
        public byte[] convertPK3toPK4(byte[] pk3)
        {
            byte[] pk4 = new byte[136];

            Array.Copy(pk3, 0, pk4, 0, 4);
            Array.Copy(pk3, 0x20, pk4, 0x08, 2); // Species
            Array.Copy(pk3, 0x04, pk4, 0x0C, 4); // SIDTID
            Array.Copy(pk3, 0x24, pk4, 0x10, 4); // EXP

            int species = getg3species(BitConverter.ToUInt16(pk4, 0x8));
            pk4[0x8] = (byte)(species & 0xFF); pk4[0x9] = (byte)(species >> 8);

            uint exp = BitConverter.ToUInt32(pk4, 0x10);
            pk4[0x14] = (byte)70;
            // ability later
            pk4[0x16] = pk3[27]; // Copy markings
            pk4[0x17] = pk3[18]; // Language
            Array.Copy(pk3, 0x38, pk4, 0x18, 12);// EVs & Contest Stats

            // Ribbons are annoying... 3bits per contest ribbon...
            uint ribo = BitConverter.ToUInt32(pk3, 0x4C);
            int fateful = (int)(ribo >> 31);
            uint newrib = 0;
            uint mask = 0xF;
            for (int i = 0; i < 5; i++) // copy contest ribbons
            {
                uint oldval = (ribo >> (3 * i)) & 0x7; // get 01234 stage
                uint newval = mask >> (int)(4 - oldval); // get 4 bit flags
                newrib |= newval << (4 * i);           // insert flags
            }
            // bits 20-31 are straight up copied from bit15-27 (12 bits) 
            newrib |= ((ribo >> 15) & 0xFFF) << 20;
            Array.Copy(BitConverter.GetBytes(newrib), 0, pk4, 0x3C, 4);

            // Copy Moves
            Array.Copy(pk3, 0x2C, pk4, 0x28, 8);
            // Copy PPUps
            byte ppup = pk3[0x28];
            pk4[0x34] = (byte)((ppup >> 0) & 3);
            pk4[0x35] = (byte)((ppup >> 2) & 3);
            pk4[0x36] = (byte)((ppup >> 4) & 3);
            pk4[0x37] = (byte)((ppup >> 6) & 3);
            // Get Move PP
            pk4[0x30] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk4, 0x28), pk4[0x34]));
            pk4[0x31] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk4, 0x2A), pk4[0x35]));
            pk4[0x32] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk4, 0x2C), pk4[0x36]));
            pk4[0x33] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk4, 0x2E), pk4[0x37]));

            // Copy IVs
            uint IVs = BitConverter.ToUInt32(pk3, 0x48);
            IVs &= 0x3FFFFFFF;
            Array.Copy(BitConverter.GetBytes(IVs), 0, pk4, 0x38, 4);
            int abilnum = pk3[0x4B] >> 7;
            DataTable g3abiltable = Gen3Abilities();
            pk4[0x15] = (byte)(int)((g3abiltable.Rows[species][1 + abilnum]));
            bool isegg = false;
            // check if the user transferred an egg... if so, hatch it...
            if (((pk4[0x3B] >> 6) & 1) == 1)
            {
                // set level to 5...
                Array.Copy(BitConverter.GetBytes(PKX.getEXP(5, species)), 0, pk4, 0x10, 4);
                // set nickname to species name later
                isegg = true;
            }

            // Gender Form Fateful
            PKX.PersonalParser.Personal MonData = PKX.PersonalGetter.GetPersonal(species);
            int genderratio = MonData.GenderRatio;
            uint PID = BitConverter.ToUInt32(pk4, 0);
            int gv = (int)(PID & 0xFF);
            int gender = 0;

            if (genderratio == 255)
                gender = 2;
            else if (genderratio == 254)
                gender = 1;
            else if (genderratio == 0)
                gender = 0;
            else
            {
                if (gv <= genderratio)
                    gender = 1;
                else
                    gender = 0;
            }

            int formnum = 0;
            // unown
            if (species == 201)
            {
                formnum =
                        ((pk4[3] & 3) << 6)
                    + ((pk4[2] & 3) << 4)
                    + ((pk4[1] & 3) << 2)
                    + ((pk4[0] & 3) << 0);
                formnum %= 28;
            }
            // screw deoxys, always normal form (default 0)

            pk4[0x40] = (byte)(fateful | (formnum << 3) | gender<<1);

            uint origins = BitConverter.ToUInt16(pk3, 0x46);
            pk4[0x5F] = (byte)((origins >> 7) & 0xF);   // Hometown Game
            pk4[0x82] = pk3[0x44];                      // Copy Pokerus
            pk4[0x83] = (byte)((origins >> 11) & 0xF);  // Ball
            pk4[0x84] = (byte)((pk3[0x47] & 0x80) | ((byte)PKX.getLevel(species, ref exp)));

            // Nickname and OT Name handling...
            byte[][] trash = new byte[8][];
            trash[1] = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            trash[2] = new byte[] { 0x18, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0xA1, 0x0C, 0x02, 0xE0, 0xFF };
            trash[3] = new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF };
            trash[4] = new byte[] { 0x54, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0xA1, 0x0C, 0x02, 0xE0, 0xFF };
            trash[5] = new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF };
            trash[7] = new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF };

            // copy in trash bytes to Nickname field
            Array.Copy(trash[pk3[18]], 0, pk4, 0x48 + 4, trash[pk3[18]].Length); // Nickname

            // Get Name
            DataTable chartable = Char3to4E();
            if (pk3[18] == 1) // JP, use JP table
                chartable = Char3to4J();

            // Get Species Names
            string[] names = new string[] 
            {
                speclang_ja[species].ToUpper(), 
                speclang_en[species].ToUpper(),
                speclang_fr[species].ToUpper(),
                speclang_it[species].ToUpper(),
                speclang_de[species].ToUpper(),
                speclang_ja[species].ToLower(),
                speclang_es[species].ToUpper(),
            };

            string nickname = names[pk3[18] - 1];
            if (isegg)
            {
                // Nickname is Species name for given language...
                // Get Nickname
                // Find each character at a time...
                for (int i = 0; i < nickname.Length; i++)
                {
                    char ch = nickname[i];
                    for (int j = 0; j < 247; j++)
                    {
                        if ((char)chartable.Rows[j][2] == ch)
                        {
                            int val = (int)chartable.Rows[j][0];    // fetch gen3 value
                            pk3[0x8 + i] = (byte)val;
                            break;
                        }
                    }
                }
                pk3[0x8 + nickname.Length + 1] = 0xFF; // cap off nickname
            }
            
            // Get 3->4 name
            {
                // Nickname is going to be translated.
                // Get Nickname string
                nickname = "";
                for (int i = 0; i < 11; i++)
                {
                    int val = pk3[0x8 + i];
                    if (val == 0xFF || i == 10)
                    {
                        // Nickname is capped here
                        Array.Copy(BitConverter.GetBytes(0xFFFF), 0, pk4, 0x48 + i * 2, 2);
                        break;
                    }
                    else
                    {
                        nickname += (char)chartable.Rows[val][2];
                        int newval = (int)chartable.Rows[val][1];
                        Array.Copy(BitConverter.GetBytes(newval), 0, pk4, 0x48 + i * 2, 2);
                    }
                }

                // nickname detection
                if (Array.IndexOf(names, nickname) < 0) // if it is not any of the species names
                    pk4[0x3B] |= 1 << 7; // set nickname flag
            }

            // Set Trainer Name
            // First, transfer the Nickname to trashbytes for OT
            string trainer = "";
            Array.Copy(pk4, 0x48, pk4, 0x68, 0x10);
            {
                for (int i = 0; i < 8; i++)
                {
                    int val = pk3[0x14 + i];
                    if (val == 0xFF || i == 7)
                    {
                        // Nickname is capped here
                        Array.Copy(BitConverter.GetBytes(0xFFFF), 0, pk4, 0x68 + i * 2, 2);
                        break;
                    }
                    else
                    {
                        trainer += (char)chartable.Rows[val][2];
                        int newval = (int)chartable.Rows[val][1];
                        Array.Copy(BitConverter.GetBytes(newval), 0, pk4, 0x68 + i * 2, 2);
                    }
                }
            }

            // Location
            pk4[0x80] = 0x37;
            // Date
            pk4[0x7B] = (byte)(moment.Year - 2000);
            pk4[0x7C] = (byte)moment.Month;
            pk4[0x7D] = (byte)moment.Day; 

            // fix checksum
            ushort checksum = 0;
            for (int i = 8; i < 136; i += 2)
                checksum += BitConverter.ToUInt16(pk4, i);
            Array.Copy(BitConverter.GetBytes(checksum), 0, pk4, 6, 2);

            return pk4;
        }
        public byte[] convertPK4toPK5(byte[] pk4)
        {
            byte[] pk5 = new byte[136];
            if (pk4[0x5F] < 0x10 && BitConverter.ToUInt16(pk4, 0x80) > 0x4000)
                return pk4;

            Array.Copy(pk4, 0, pk5, 0, 136); // copy the data, now we adjust.
            
            // zero out item
            // pk5[0x0A] = pk5[0x0B] = 0;

            for (int i = 0; i < 4; i++) // fix PP
                pk5[0x30 + i] = (byte)(PKX.getMovePP(BitConverter.ToUInt16(pk5, 0x28 + 2 * i), pk5[0x34 + i]));

            // fix nature
            pk5[0x41] = (byte)(BitConverter.ToUInt32(pk5, 0) % 0x19);

            // zero out platinum data
            pk5[0x44] = pk5[0x45] = pk5[0x46] = pk5[0x47] = 0;


            // Met / Crown Data Detection
            int species = BitConverter.ToInt16(pk5, 0x8);
            bool fateful = Convert.ToBoolean(BitConverter.ToUInt16(pk5, 0x40) & 0x1);
            int[] crownspec = new int[] { 251, 243, 244, 245 };
            if (fateful && Array.IndexOf(crownspec, species) >= 0)
            {
                if (species == 251)
                { pk5[0x80] = 0x3A; pk5[0x81] = 0x75; }// 30010 Celebi
                else
                { pk5[0x80] = 0x3C; pk5[0x81] = 0x75; } // 30012 Beast
            }
            else
                pk5[0x80] = 0x31; pk5[0x81] = 0x75; // 30001 PokeTransfer

            // Apply new met date
            pk5[0x7B] = (byte)(moment.Year - 2000); 
            pk5[0x7C] = (byte)moment.Month;
            pk5[0x7D] = (byte)moment.Day;
            //
            pk5[0x86] = pk5[0x87] = 0; // HGSS Data wiped
            // Transfer ball over
            if (pk4[0x86] > 0 && pk4[0x86] != 4)
                pk5[0x83] = pk4[0x86];
            // Transfer Nickname and OT Name
            DataTable CT45 = Char4to5();
            byte[] nicknamestr = new byte[24];
            string nickname = "";
            string trainer = "";
            nicknamestr[22] = nicknamestr[23] = 0xFF;
            try
            {
                for (int i = 0; i < 24; i += 2)
                {
                    int val = BitConverter.ToUInt16(pk5, 0x48 + i);
                    if (val == 0xFFFF)   // If given character is a terminator, stop conversion.
                        break;

                    // find entry
                    int newval = (int)CT45.Rows.Find(val)[1];
                    Array.Copy(BitConverter.GetBytes(newval), 0, pk5, 0x48 + i, 2);
                    nickname += (char)newval;
                }

                byte[] OTstr = new byte[24];
                OTstr[22] = OTstr[23] = 0xFF;
                for (int i = 0; i < 24; i += 2)
                {
                    int val = BitConverter.ToUInt16(pk5, 0x68 + i);
                    if (val == 0xFFFF)   // If given character is a terminator, stop conversion.
                        break;

                    // find entry
                    int newval = (int)CT45.Rows.Find(val)[1];
                    trainer += (char)newval;
                    Array.Copy(BitConverter.GetBytes(newval), 0, pk5, 0x68 + i, 2);
                }
            } catch { return pk4; }
            // Reset Friendship
            pk5[0x14] = 70;

            // Fix Level
            pk5[0x84] &= 0x80;
            uint exp = BitConverter.ToUInt32(pk5, 0x10);
            pk5[0x84] |= (byte)PKX.getLevel(species, ref exp);

            // Fix Checksum
            ushort chk = 0;
            for (int i = 8; i < 136; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(pk5, i);

            // Apply New Checksum
            Array.Copy(BitConverter.GetBytes(chk), 0, pk5, 06, 2);
            return pk5;
        }
        public byte[] convertPK5toPK6(byte[] pk5)
        {
            // To transfer, we will go down the pkm offset list and fill it into the PKX list.
            byte[] pk6 = new byte[232]; // Setup new array to store the new PKX

            // Check if G4PKM
            if (pk5[0x5F] < 0x10 && BitConverter.ToUInt16(pk5, 0x80) < 4000)
                pk5 = convertPK4toPK5(pk5);

            // Upon transfer, the PID is also set as the Encryption Key.
            // Copy intro data, it's the same (Encrypt Key -> EXP)
            Array.Copy(pk5, 0, pk6, 0, 0x14);
            pk6[0xA] = 0; pk6[0xB] = 0;     // Get rid of the item, those aren't transferred.
            // Set the PID in its new location as well...
            Array.Copy(pk5, 0, pk6, 0x18, 4);
            pk6[0xCA] = pk5[0x14]; // Friendship
            pk6[0x14] = pk5[0x15]; // Ability
            // Get Ability Number from the ability (To catch XD/C abilities)
            int abilnum = getAbilityNumber(BitConverter.ToUInt16(pk6, 0x8), pk6[0x14], pk5[0x40] >> 3);
            if (abilnum > 0)
                pk6[0x15] = (byte)abilnum;
            else // Unknown (hacked?), get the value from the PID as a failsafe.
            {
                if ((pk5[0x42] & 1) == 1)   // Hidden Ability Flag
                    pk6[0x15] = 4;
                else if (pk5[0x5F] < 0x10)  // Gen 3-4 Origin Method
                    pk6[0x15] = (byte)(pk6[0x0] & 1); // Old Ability Correlation
                else
                    pk6[0x15] = (byte)(pk6[0x2] & 1); // Gen5 Correlation
            }
            pk6[0x2A] = pk5[0x16];  // Markings
            pk6[0xE3] = pk5[0x17];  // OT Language

            // Copy EVs and Contest Stats
            for (int i = 0; i < 12; i++)
                pk6[0x1E + i] = pk5[0x18 + i];
            // Fix EVs (<=252)
            for (int i = 0; i < 6; i++)
                if (pk6[0x1E + i] > 252)
                    pk6[0x1E + i] = 252;

            // Copy Moves
            for (int i = 0; i < 16; i++)
                pk6[0x5A + i] = pk5[0x28 + i];
            // Fix PP; some moves have different PP in Gen 6.
            pk6[0x62] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk6, 0x5A), pk6[0x66]));
            pk6[0x63] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk6, 0x5C), pk6[0x67]));
            pk6[0x64] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk6, 0x5E), pk6[0x68]));
            pk6[0x65] = (byte)(PKX.getMovePP(BitConverter.ToInt16(pk6, 0x60), pk6[0x69]));

            // Copy 32bit IV value.
            for (int i = 0; i < 4; i++)
                pk6[0x74 + i] = pk5[0x38 + i];

            pk6[0x1D] = pk5[0x40];  // Copy FE & Gender Flags
            pk6[0x1C] = pk5[0x41];  // Copy Nature

            // Copy Nickname
            string nicknamestr = "";
            for (int i = 0; i < 24; i += 2)
            {
                if ((pk5[0x48 + i] == 0xFF) && pk5[0x48 + i + 1] == 0xFF)   // If given character is a terminator, stop copying. There are no trash bytes or terminators in Gen 6!
                    break;
                nicknamestr += (char)(BitConverter.ToUInt16(pk5, 0x48 + i));
            }
            // Decapitalize Logic
            if ((nicknamestr.Length > 0) && (pk6[0x77] >> 7 == 0))
                nicknamestr = char.ToUpper(nicknamestr[0]) + nicknamestr.Substring(1).ToLower();
            byte[] nkb = Encoding.Unicode.GetBytes(nicknamestr);
            Array.Resize(ref nkb, 24);
            Array.Copy(nkb, 0, pk6, 0x40, nkb.Length);

            pk6[0xDF] = pk5[0x5F];  // Copy Origin Game

            // Copy OT
            for (int i = 0; i < 24; i += 2)
            {
                if (BitConverter.ToUInt16(pk5, 0x68 + i) == 0xFFFF)  // If terminated, stop
                    break;
                else 
                    Array.Copy(pk5, 0x68 + i, pk6, 0xB0 + i, 2); // Copy 16bit Character
            }

            // Copy Met Info
            for (int i = 0; i < 0x6; i++)   // Dates are kept upon transfer
                pk6[0xD1 + i] = pk5[0x78 + i];

            // pkx[0xD7] has a gap.

            for (int i = 0; i < 0x4; i++)   // Locations are kept upon transfer
                pk6[0xD8 + i] = pk5[0x7E + i];

            pk6[0x2B] = pk5[0x82];  // Pokerus
            pk6[0xDC] = pk5[0x83];  // Ball

            // Get the current level of the specimen to be transferred
            int species = BitConverter.ToInt16(pk6, 0x08);
            uint exp = BitConverter.ToUInt32(pk6, 0x10);

            // Level isn't altered, keeps Gen5 Met Level
                // int currentlevel = getLevel((species), (exp));
                // (byte)(((pk5[0x84]) & 0x80) + currentlevel);  
            pk6[0xDD] = pk5[0x84];  // OT Gender & Encounter Level
            pk6[0xDE] = pk5[0x85];  // Encounter Type

            // Ribbon Decomposer (Contest & Battle)
            byte contestribbons = 0;
            byte battleribbons = 0;

            // Contest Ribbon Counter
            for (int i = 0; i < 8; i++) // Sinnoh 3, Hoenn 1
            {
                if (((pk5[0x60] >> i) & 1) == 1)
                    contestribbons++;
                if (((pk5[0x61] >> i) & 1) == 1)
                    contestribbons++;
                if (((pk5[0x3C] >> i) & 1) == 1)
                    contestribbons++;
                if (((pk5[0x3D] >> i) & 1) == 1)
                    contestribbons++;
            }
            for (int i = 0; i < 4; i++) // Sinnoh 4, Hoenn 2
            {
                if (((pk5[0x62] >> i) & 1) == 1)
                    contestribbons++;
                if (((pk5[0x3E] >> i) & 1) == 1)
                    contestribbons++;
            }

            // Battle Ribbon Counter
            if ((pk5[0x3E] & 0x20) >> 5 == 1)    // Winning Ribbon
                battleribbons++;
            if ((pk5[0x3E] & 0x40) >> 6 == 1)    // Victory Ribbon
                battleribbons++;
            for (int i = 1; i < 7; i++)     // Sinnoh Battle Ribbons
                if (((pk5[0x24] >> i) & 1) == 1)
                    battleribbons++;

            // Fill the Ribbon Counter Bytes
            pk6[0x38] = contestribbons;
            pk6[0x39] = battleribbons;

            // Copy Ribbons to their new locations.
            int bx30 = 0;
            // bx30 |= 0;                             // Kalos Champ - New Kalos Ribbon
            bx30 |= (((pk5[0x3E] & 0x10) >> 4) << 1); // Hoenn Champion
            bx30 |= (((pk5[0x24] & 0x01) >> 0) << 2); // Sinnoh Champ
            // bx30 |= 0;                             // Best Friend - New Kalos Ribbon
            // bx30 |= 0;                             // Training    - New Kalos Ribbon
            // bx30 |= 0;                             // Skillful    - New Kalos Ribbon
            // bx30 |= 0;                             // Expert      - New Kalos Ribbon
            bx30 |= (((pk5[0x3F] & 0x01) >> 0) << 7); // Effort Ribbon
            pk6[0x30] = (byte)bx30;

            int bx31 = 0;
            bx31 |= (((pk5[0x24] & 0x80) >> 7) << 0);  // Alert
            bx31 |= (((pk5[0x25] & 0x01) >> 0) << 1);  // Shock
            bx31 |= (((pk5[0x25] & 0x02) >> 1) << 2);  // Downcast
            bx31 |= (((pk5[0x25] & 0x04) >> 2) << 3);  // Careless
            bx31 |= (((pk5[0x25] & 0x08) >> 3) << 4);  // Relax
            bx31 |= (((pk5[0x25] & 0x10) >> 4) << 5);  // Snooze
            bx31 |= (((pk5[0x25] & 0x20) >> 5) << 6);  // Smile
            bx31 |= (((pk5[0x25] & 0x40) >> 6) << 7);  // Gorgeous
            pk6[0x31] = (byte)bx31;

            int bx32 = 0;
            bx32 |= (((pk5[0x25] & 0x80) >> 7) << 0);  // Royal
            bx32 |= (((pk5[0x26] & 0x01) >> 0) << 1);  // Gorgeous Royal
            bx32 |= (((pk5[0x3E] & 0x80) >> 7) << 2);  // Artist
            bx32 |= (((pk5[0x26] & 0x02) >> 1) << 3);  // Footprint
            bx32 |= (((pk5[0x26] & 0x04) >> 2) << 4);  // Record
            bx32 |= (((pk5[0x26] & 0x10) >> 4) << 5);  // Legend
            bx32 |= (((pk5[0x3F] & 0x10) >> 4) << 6);  // Country
            bx32 |= (((pk5[0x3F] & 0x20) >> 5) << 7);  // National
            pk6[0x32] = (byte)bx32;

            int bx33 = 0;
            bx33 |= (((pk5[0x3F] & 0x40) >> 6) << 0);  // Earth
            bx33 |= (((pk5[0x3F] & 0x80) >> 7) << 1);  // World
            bx33 |= (((pk5[0x27] & 0x04) >> 2) << 2);  // Classic
            bx33 |= (((pk5[0x27] & 0x08) >> 3) << 3);  // Premier
            bx33 |= (((pk5[0x26] & 0x08) >> 3) << 4);  // Event
            bx33 |= (((pk5[0x26] & 0x40) >> 6) << 5);  // Birthday
            bx33 |= (((pk5[0x26] & 0x80) >> 7) << 6);  // Special
            bx33 |= (((pk5[0x27] & 0x01) >> 0) << 7);  // Souvenir
            pk6[0x33] = (byte)bx33;

            int bx34 = 0;
            bx34 |= (((pk5[0x27] & 0x02) >> 1) << 0);  // Wishing Ribbon
            bx34 |= (((pk5[0x3F] & 0x02) >> 1) << 1);  // Battle Champion
            bx34 |= (((pk5[0x3F] & 0x04) >> 2) << 2);  // Regional Champion
            bx34 |= (((pk5[0x3F] & 0x08) >> 3) << 3);  // National Champion
            bx34 |= (((pk5[0x26] & 0x20) >> 5) << 4);  // World Champion
            pk6[0x34] = (byte)bx34;

            // 
            // Extra Modifications:
            // Write the Memories, Friendship, and Origin!
            //

            // Write latest notOT handler as PKHeX
            byte[] newOT = Encoding.Unicode.GetBytes(g6trname);
            Array.Resize(ref newOT, 24);
            Array.Copy(newOT, 0, pk6, 0x78, newOT.Length);

            // Write Memories as if it was Transferred: USA|California
            // 01 - Not handled by OT
            // 07 - CA
            // 31 - USA
            byte[] x90x = new byte[] { 0x00, 0x00, g6trgend, 0x01, (byte)subreg, (byte)country, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, (byte)PKX.getBaseFriendship(species), 0x00, 0x01, 0x04, (byte)(Util.rnd32() % 10), 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Copy(x90x, 0, pk6, 0x90, x90x.Length);
            // When transferred, friendship gets reset.
            pk6[0xCA] = (byte)PKX.getBaseFriendship(species);

            // Write Origin (USA California) - location is dependent on 3DS system that transfers.
            pk6[0xE0] = (byte)country;   
            pk6[0xE1] = (byte)subreg;   
            pk6[0xE2] = (byte)_3DSreg;

            // Antishiny Mechanism
            ushort TID = BitConverter.ToUInt16(pk6, 0x0C);
            ushort SID = BitConverter.ToUInt16(pk6, 0x0E);
            uint PID = BitConverter.ToUInt32(pk6, 0x18);
            ushort LID = (ushort)(PID & 0xFFFF);
            ushort HID = (ushort)(PID >> 0x10);

            int XOR = TID ^ SID ^ LID ^ HID;
            if (XOR >= 8 && XOR < 16) // If we get an illegal collision...
                Array.Copy(BitConverter.GetBytes(PID ^ 0x80000000), 0, pk6, 0x18, 4);

            // Fix Checksum
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(pk6, i);

            // Apply New Checksum
            Array.Copy(BitConverter.GetBytes(chk), 0, pk6, 06, 2);

            string trainer = Util.TrimFromZero(Encoding.Unicode.GetString(pk6, 0xB0, 24));
            
            return pk6; // Done!
        }
        static DataTable Char3to4E()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Old", typeof(int));
            table.Columns.Add("New", typeof(int));
            table.Columns.Add("Symbol", typeof(char));
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["Symbol"]; // find chars for Eggs
            table.PrimaryKey = keyColumns;

            #region DataTable Entries
            table.Rows.Add(0,478,' ');
            table.Rows.Add(1,351,'À');
            table.Rows.Add(2,352,'Á');
            table.Rows.Add(3,353,'Â');
            table.Rows.Add(4,358,'Ç');
            table.Rows.Add(5,359,'È');
            table.Rows.Add(6,360,'É');
            table.Rows.Add(7,361,'Ê');
            table.Rows.Add(8,362,'Ë');
            table.Rows.Add(9,363,'Ì');
            table.Rows.Add(10,20,'こ');
            table.Rows.Add(11,365,'Î');
            table.Rows.Add(12,366,'Ï');
            table.Rows.Add(13,369,'Ò');
            table.Rows.Add(14,370,'Ó');
            table.Rows.Add(15,371,'Ô');
            table.Rows.Add(16,415,'Œ');
            table.Rows.Add(17,376,'Ù');
            table.Rows.Add(18,377,'Ú');
            table.Rows.Add(19,378,'Û');
            table.Rows.Add(20,368,'Ñ');
            table.Rows.Add(21,382,'ß');
            table.Rows.Add(22,383,'à');
            table.Rows.Add(23,384,'á');
            table.Rows.Add(24,46,'ね');
            table.Rows.Add(25,358,'ç');
            table.Rows.Add(26,359,'è');
            table.Rows.Add(27,392,'é');
            table.Rows.Add(28,393,'ê');
            table.Rows.Add(29,394,'ë');
            table.Rows.Add(30,395,'ì');
            table.Rows.Add(31,396,'~');
            table.Rows.Add(32,397,'î');
            table.Rows.Add(33,398,'ï');
            table.Rows.Add(34,401,'ò');
            table.Rows.Add(35,402,'ó');
            table.Rows.Add(36,403,'ô');
            table.Rows.Add(37,416,'œ');
            table.Rows.Add(38,408,'ù');
            table.Rows.Add(39,409,'ú');
            table.Rows.Add(40,410,'û');
            table.Rows.Add(41,400,'ñ');
            table.Rows.Add(42,420,'º');
            table.Rows.Add(43,419,'ª');
            table.Rows.Add(44,479,'*');
            table.Rows.Add(45,450,'&');
            table.Rows.Add(46,445,'+');
            table.Rows.Add(47,3,'あ');
            table.Rows.Add(48,4,'ぃ');
            table.Rows.Add(49,6,'ぅ');
            table.Rows.Add(50,8,'ぇ');
            table.Rows.Add(51,10,'ぉ');
            table.Rows.Add(52,68,'ゃ');
            table.Rows.Add(53,449,'=');
            table.Rows.Add(54,72,'ょ');
            table.Rows.Add(55,13,'が');
            table.Rows.Add(56,15,'ぎ');
            table.Rows.Add(57,17,'ぐ');
            table.Rows.Add(58,19,'げ');
            table.Rows.Add(59,21,'ご');
            table.Rows.Add(60,23,'ざ');
            table.Rows.Add(61,25,'じ');
            table.Rows.Add(62,27,'ず');
            table.Rows.Add(63,29,'ぜ');
            table.Rows.Add(64,31,'ぞ');
            table.Rows.Add(65,33,'だ');
            table.Rows.Add(66,35,'ぢ');
            table.Rows.Add(67,38,'づ');
            table.Rows.Add(68,40,'で');
            table.Rows.Add(69,42,'ど');
            table.Rows.Add(70,49,'ば');
            table.Rows.Add(71,52,'び');
            table.Rows.Add(72,55,'ぶ');
            table.Rows.Add(73,58,'べ');
            table.Rows.Add(74,61,'ぼ');
            table.Rows.Add(75,50,'ぱ');
            table.Rows.Add(76,53,'ぴ');
            table.Rows.Add(77,56,'ぷ');
            table.Rows.Add(78,59,'ぺ');
            table.Rows.Add(79,62,'ぽ');
            table.Rows.Add(80,36,'っ');
            table.Rows.Add(81,426,'¿');
            table.Rows.Add(82,425,'¡');
            table.Rows.Add(83,480,'⒆');
            table.Rows.Add(84,481,'⒇');
            table.Rows.Add(85,91,'オ');
            table.Rows.Add(86,92,'カ');
            table.Rows.Add(87,94,'キ');
            table.Rows.Add(88,96,'ク');
            table.Rows.Add(89,98,'ケ');
            table.Rows.Add(90,364,'Í');
            table.Rows.Add(91,100,'コ');
            table.Rows.Add(92,102,'サ');
            table.Rows.Add(93,106,'ス');
            table.Rows.Add(94,108,'セ');
            table.Rows.Add(95,110,'ソ');
            table.Rows.Add(96,112,'タ');
            table.Rows.Add(97,114,'チ');
            table.Rows.Add(98,117,'ツ');
            table.Rows.Add(99,119,'テ');
            table.Rows.Add(100,121,'ト');
            table.Rows.Add(101,123,'ナ');
            table.Rows.Add(102,124,'ニ');
            table.Rows.Add(103,125,'ヌ');
            table.Rows.Add(104,385,'â');
            table.Rows.Add(105,127,'ノ');
            table.Rows.Add(106,128,'ハ');
            table.Rows.Add(107,131,'ヒ');
            table.Rows.Add(108,134,'フ');
            table.Rows.Add(109,137,'ヘ');
            table.Rows.Add(110,140,'ホ');
            table.Rows.Add(111,396,'í');
            table.Rows.Add(112,144,'ミ');
            table.Rows.Add(113,145,'ム');
            table.Rows.Add(114,146,'メ');
            table.Rows.Add(115,147,'モ');
            table.Rows.Add(116,149,'ヤ');
            table.Rows.Add(117,151,'ユ');
            table.Rows.Add(118,153,'ヨ');
            table.Rows.Add(119,154,'ラ');
            table.Rows.Add(120,155,'リ');
            table.Rows.Add(121,156,'ル');
            table.Rows.Add(122,157,'レ');
            table.Rows.Add(123,158,'ロ');
            table.Rows.Add(124,159,'ワ');
            table.Rows.Add(125,160,'ヲ');
            table.Rows.Add(126,161,'ン');
            table.Rows.Add(127,82,'ァ');
            table.Rows.Add(128,84,'ィ');
            table.Rows.Add(129,86,'ゥ');
            table.Rows.Add(130,88,'ェ');
            table.Rows.Add(131,90,'ォ');
            table.Rows.Add(132,148,'ャ');
            table.Rows.Add(133,150,'ュ');
            table.Rows.Add(134,152,'ョ');
            table.Rows.Add(135,93,'ガ');
            table.Rows.Add(136,95,'ギ');
            table.Rows.Add(137,97,'グ');
            table.Rows.Add(138,99,'ゲ');
            table.Rows.Add(139,101,'ゴ');
            table.Rows.Add(140,103,'ザ');
            table.Rows.Add(141,105,'ジ');
            table.Rows.Add(142,107,'ズ');
            table.Rows.Add(143,109,'ゼ');
            table.Rows.Add(144,111,'ゾ');
            table.Rows.Add(145,113,'ダ');
            table.Rows.Add(146,115,'ヂ');
            table.Rows.Add(147,118,'ヅ');
            table.Rows.Add(148,120,'デ');
            table.Rows.Add(149,122,'ド');
            table.Rows.Add(150,129,'バ');
            table.Rows.Add(151,132,'ビ');
            table.Rows.Add(152,135,'ブ');
            table.Rows.Add(153,138,'ベ');
            table.Rows.Add(154,141,'ボ');
            table.Rows.Add(155,130,'パ');
            table.Rows.Add(156,133,'ピ');
            table.Rows.Add(157,136,'プ');
            table.Rows.Add(158,139,'ペ');
            table.Rows.Add(159,142,'ポ');
            table.Rows.Add(160,116,'ッ');
            table.Rows.Add(161,289,'0');
            table.Rows.Add(162,290,'1');
            table.Rows.Add(163,291,'2');
            table.Rows.Add(164,292,'3');
            table.Rows.Add(165,293,'4');
            table.Rows.Add(166,294,'5');
            table.Rows.Add(167,295,'6');
            table.Rows.Add(168,296,'7');
            table.Rows.Add(169,297,'8');
            table.Rows.Add(170,298,'9');
            table.Rows.Add(171,427,'!');
            table.Rows.Add(172,428,'?');
            table.Rows.Add(173,430,'.');
            table.Rows.Add(174,241,'-'); // hyphen －
            table.Rows.Add(175,230,'・');
            table.Rows.Add(176,431,'…');
            table.Rows.Add(177,436,'“');
            table.Rows.Add(178,437,'”');
            table.Rows.Add(179,434,'‘');
            table.Rows.Add(180,435,'’');
            table.Rows.Add(181,443,'♂');
            table.Rows.Add(182,444,'♀');
            table.Rows.Add(183,424,'$');
            table.Rows.Add(184,429,',');
            table.Rows.Add(185,242,'×');
            table.Rows.Add(186,433,'/');
            table.Rows.Add(187,299,'A');
            table.Rows.Add(188,300,'B');
            table.Rows.Add(189,301,'C');
            table.Rows.Add(190,302,'D');
            table.Rows.Add(191,303,'E');
            table.Rows.Add(192,304,'F');
            table.Rows.Add(193,305,'G');
            table.Rows.Add(194,306,'H');
            table.Rows.Add(195,307,'I');
            table.Rows.Add(196,308,'J');
            table.Rows.Add(197,309,'K');
            table.Rows.Add(198,310,'L');
            table.Rows.Add(199,311,'M');
            table.Rows.Add(200,312,'N');
            table.Rows.Add(201,313,'O');
            table.Rows.Add(202,314,'P');
            table.Rows.Add(203,315,'Q');
            table.Rows.Add(204,316,'R');
            table.Rows.Add(205,317,'S');
            table.Rows.Add(206,318,'T');
            table.Rows.Add(207,319,'U');
            table.Rows.Add(208,320,'V');
            table.Rows.Add(209,321,'W');
            table.Rows.Add(210,322,'X');
            table.Rows.Add(211,323,'Y');
            table.Rows.Add(212,324,'Z');
            table.Rows.Add(213,325,'a');
            table.Rows.Add(214,326,'b');
            table.Rows.Add(215,327,'c');
            table.Rows.Add(216,328,'d');
            table.Rows.Add(217,329,'e');
            table.Rows.Add(218,330,'f');
            table.Rows.Add(219,331,'g');
            table.Rows.Add(220,332,'h');
            table.Rows.Add(221,333,'i');
            table.Rows.Add(222,334,'j');
            table.Rows.Add(223,335,'k');
            table.Rows.Add(224,336,'l');
            table.Rows.Add(225,337,'m');
            table.Rows.Add(226,338,'n');
            table.Rows.Add(227,339,'o');
            table.Rows.Add(228,340,'p');
            table.Rows.Add(229,341,'q');
            table.Rows.Add(230,342,'r');
            table.Rows.Add(231,343,'s');
            table.Rows.Add(232,344,'t');
            table.Rows.Add(233,345,'u');
            table.Rows.Add(234,346,'v');
            table.Rows.Add(235,347,'w');
            table.Rows.Add(236,348,'x');
            table.Rows.Add(237,349,'y');
            table.Rows.Add(238,350,'z');
            table.Rows.Add(239,289,'>');
            table.Rows.Add(240,452,':');
            table.Rows.Add(241,355,'Ä');
            table.Rows.Add(242,373,'Ö');
            table.Rows.Add(243,379,'Ü');
            table.Rows.Add(244,387,'ä');
            table.Rows.Add(245,405,'ö');
            table.Rows.Add(246,411,'ü');
            #endregion
            return table;
        }
        static DataTable Char3to4J()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Old", typeof(int));
            table.Columns.Add("New", typeof(int));
            table.Columns.Add("Symbol", typeof(char));

            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["Symbol"]; // find chars for Eggs
            table.PrimaryKey = keyColumns;
            #region DataTable Entries
            table.Rows.Add(0,1,'　');
            table.Rows.Add(1,3,'あ');
            table.Rows.Add(2,5,'い');
            table.Rows.Add(3,7,'う');
            table.Rows.Add(4,9,'え');
            table.Rows.Add(5,11,'お');
            table.Rows.Add(6,12,'か');
            table.Rows.Add(7,14,'き');
            table.Rows.Add(8,16,'く');
            table.Rows.Add(9,18,'け');
            table.Rows.Add(10,20,'こ');
            table.Rows.Add(11,22,'さ');
            table.Rows.Add(12,24,'し');
            table.Rows.Add(13,26,'す');
            table.Rows.Add(14,28,'せ');
            table.Rows.Add(15,30,'そ');
            table.Rows.Add(16,32,'た');
            table.Rows.Add(17,34,'ち');
            table.Rows.Add(18,37,'つ');
            table.Rows.Add(19,39,'て');
            table.Rows.Add(20,41,'と');
            table.Rows.Add(21,43,'な');
            table.Rows.Add(22,44,'に');
            table.Rows.Add(23,45,'ぬ');
            table.Rows.Add(24,46,'ね');
            table.Rows.Add(25,47,'の');
            table.Rows.Add(26,48,'は');
            table.Rows.Add(27,51,'ひ');
            table.Rows.Add(28,54,'ふ');
            table.Rows.Add(29,57,'へ');
            table.Rows.Add(30,60,'ほ');
            table.Rows.Add(31,63,'ま');
            table.Rows.Add(32,64,'み');
            table.Rows.Add(33,65,'む');
            table.Rows.Add(34,66,'め');
            table.Rows.Add(35,67,'も');
            table.Rows.Add(36,69,'や');
            table.Rows.Add(37,71,'ゆ');
            table.Rows.Add(38,73,'よ');
            table.Rows.Add(39,74,'ら');
            table.Rows.Add(40,75,'り');
            table.Rows.Add(41,76,'る');
            table.Rows.Add(42,77,'れ');
            table.Rows.Add(43,78,'ろ');
            table.Rows.Add(44,79,'わ');
            table.Rows.Add(45,80,'を');
            table.Rows.Add(46,81,'ん');
            table.Rows.Add(47,2,'ぁ');
            table.Rows.Add(48,4,'ぃ');
            table.Rows.Add(49,6,'ぅ');
            table.Rows.Add(50,8,'ぇ');
            table.Rows.Add(51,10,'ぉ');
            table.Rows.Add(52,68,'ゃ');
            table.Rows.Add(53,70,'ゅ');
            table.Rows.Add(54,72,'ょ');
            table.Rows.Add(55,13,'が');
            table.Rows.Add(56,15,'ぎ');
            table.Rows.Add(57,17,'ぐ');
            table.Rows.Add(58,19,'げ');
            table.Rows.Add(59,21,'ご');
            table.Rows.Add(60,23,'ざ');
            table.Rows.Add(61,25,'じ');
            table.Rows.Add(62,27,'ず');
            table.Rows.Add(63,29,'ぜ');
            table.Rows.Add(64,31,'ぞ');
            table.Rows.Add(65,33,'だ');
            table.Rows.Add(66,35,'ぢ');
            table.Rows.Add(67,38,'づ');
            table.Rows.Add(68,40,'で');
            table.Rows.Add(69,42,'ど');
            table.Rows.Add(70,49,'ば');
            table.Rows.Add(71,52,'び');
            table.Rows.Add(72,55,'ぶ');
            table.Rows.Add(73,58,'べ');
            table.Rows.Add(74,61,'ぼ');
            table.Rows.Add(75,50,'ぱ');
            table.Rows.Add(76,53,'ぴ');
            table.Rows.Add(77,56,'ぷ');
            table.Rows.Add(78,59,'ぺ');
            table.Rows.Add(79,62,'ぽ');
            table.Rows.Add(80,36,'っ');
            table.Rows.Add(81,83,'ア');
            table.Rows.Add(82,85,'イ');
            table.Rows.Add(83,87,'ウ');
            table.Rows.Add(84,89,'エ');
            table.Rows.Add(85,91,'オ');
            table.Rows.Add(86,92,'カ');
            table.Rows.Add(87,94,'キ');
            table.Rows.Add(88,96,'ク');
            table.Rows.Add(89,98,'ケ');
            table.Rows.Add(90,100,'コ');
            table.Rows.Add(91,102,'サ');
            table.Rows.Add(92,104,'シ');
            table.Rows.Add(93,106,'ス');
            table.Rows.Add(94,108,'セ');
            table.Rows.Add(95,110,'ソ');
            table.Rows.Add(96,112,'タ');
            table.Rows.Add(97,114,'チ');
            table.Rows.Add(98,117,'ツ');
            table.Rows.Add(99,119,'テ');
            table.Rows.Add(100,121,'ト');
            table.Rows.Add(101,123,'ナ');
            table.Rows.Add(102,124,'ニ');
            table.Rows.Add(103,125,'ヌ');
            table.Rows.Add(104,126,'ネ');
            table.Rows.Add(105,127,'ノ');
            table.Rows.Add(106,128,'ハ');
            table.Rows.Add(107,131,'ヒ');
            table.Rows.Add(108,134,'フ');
            table.Rows.Add(109,137,'ヘ');
            table.Rows.Add(110,140,'ホ');
            table.Rows.Add(111,143,'マ');
            table.Rows.Add(112,144,'ミ');
            table.Rows.Add(113,145,'ム');
            table.Rows.Add(114,146,'メ');
            table.Rows.Add(115,147,'モ');
            table.Rows.Add(116,149,'ヤ');
            table.Rows.Add(117,151,'ユ');
            table.Rows.Add(118,153,'ヨ');
            table.Rows.Add(119,154,'ラ');
            table.Rows.Add(120,155,'リ');
            table.Rows.Add(121,156,'ル');
            table.Rows.Add(122,157,'レ');
            table.Rows.Add(123,158,'ロ');
            table.Rows.Add(124,159,'ワ');
            table.Rows.Add(125,160,'ヲ');
            table.Rows.Add(126,161,'ン');
            table.Rows.Add(127,82,'ァ');
            table.Rows.Add(128,84,'ィ');
            table.Rows.Add(129,86,'ゥ');
            table.Rows.Add(130,88,'ェ');
            table.Rows.Add(131,90,'ォ');
            table.Rows.Add(132,148,'ャ');
            table.Rows.Add(133,150,'ュ');
            table.Rows.Add(134,152,'ョ');
            table.Rows.Add(135,93,'ガ');
            table.Rows.Add(136,95,'ギ');
            table.Rows.Add(137,97,'グ');
            table.Rows.Add(138,99,'ゲ');
            table.Rows.Add(139,101,'ゴ');
            table.Rows.Add(140,103,'ザ');
            table.Rows.Add(141,105,'ジ');
            table.Rows.Add(142,107,'ズ');
            table.Rows.Add(143,109,'ゼ');
            table.Rows.Add(144,111,'ゾ');
            table.Rows.Add(145,113,'ダ');
            table.Rows.Add(146,115,'ヂ');
            table.Rows.Add(147,118,'ヅ');
            table.Rows.Add(148,120,'デ');
            table.Rows.Add(149,122,'ド');
            table.Rows.Add(150,129,'バ');
            table.Rows.Add(151,132,'ビ');
            table.Rows.Add(152,135,'ブ');
            table.Rows.Add(153,138,'ベ');
            table.Rows.Add(154,141,'ボ');
            table.Rows.Add(155,130,'パ');
            table.Rows.Add(156,133,'ピ');
            table.Rows.Add(157,136,'プ');
            table.Rows.Add(158,139,'ペ');
            table.Rows.Add(159,142,'ポ');
            table.Rows.Add(160,116,'ッ');
            table.Rows.Add(161,162,'０');
            table.Rows.Add(162,163,'１');
            table.Rows.Add(163,164,'２');
            table.Rows.Add(164,165,'３');
            table.Rows.Add(165,166,'４');
            table.Rows.Add(166,167,'５');
            table.Rows.Add(167,168,'６');
            table.Rows.Add(168,169,'７');
            table.Rows.Add(169,170,'８');
            table.Rows.Add(170,171,'９');
            table.Rows.Add(171,225,'！');
            table.Rows.Add(172,226,'？');
            table.Rows.Add(173,228,'。');
            table.Rows.Add(174,241,'-'); // hyphen －-
            table.Rows.Add(175,230,'・');
            table.Rows.Add(176,229,'⋯');
            table.Rows.Add(177,234,'『');
            table.Rows.Add(178,235,'』');
            table.Rows.Add(179,232,'「');
            table.Rows.Add(180,233,'」');
            table.Rows.Add(181,443,'♂');
            table.Rows.Add(182,444,'♀');
            table.Rows.Add(183,424,'$');
            table.Rows.Add(184,430,'.');
            table.Rows.Add(185,242,'×');
            table.Rows.Add(186,433,'/');
            table.Rows.Add(187,172,'Ａ');
            table.Rows.Add(188,173,'Ｂ');
            table.Rows.Add(189,174,'Ｃ');
            table.Rows.Add(190,175,'Ｄ');
            table.Rows.Add(191,176,'Ｅ');
            table.Rows.Add(192,177,'Ｆ');
            table.Rows.Add(193,178,'Ｇ');
            table.Rows.Add(194,179,'Ｈ');
            table.Rows.Add(195,180,'Ｉ');
            table.Rows.Add(196,181,'Ｊ');
            table.Rows.Add(197,182,'Ｋ');
            table.Rows.Add(198,183,'Ｌ');
            table.Rows.Add(199,184,'Ｍ');
            table.Rows.Add(200,185,'Ｎ');
            table.Rows.Add(201,186,'Ｏ');
            table.Rows.Add(202,187,'Ｐ');
            table.Rows.Add(203,188,'Ｑ');
            table.Rows.Add(204,189,'Ｒ');
            table.Rows.Add(205,190,'Ｓ');
            table.Rows.Add(206,191,'Ｔ');
            table.Rows.Add(207,192,'Ｕ');
            table.Rows.Add(208,193,'Ｖ');
            table.Rows.Add(209,194,'Ｗ');
            table.Rows.Add(210,195,'Ｘ');
            table.Rows.Add(211,196,'Ｙ');
            table.Rows.Add(212,197,'Ｚ');
            table.Rows.Add(213,198,'ａ');
            table.Rows.Add(214,199,'ｂ');
            table.Rows.Add(215,200,'ｃ');
            table.Rows.Add(216,201,'ｄ');
            table.Rows.Add(217,202,'ｅ');
            table.Rows.Add(218,203,'ｆ');
            table.Rows.Add(219,204,'ｇ');
            table.Rows.Add(220,205,'ｈ');
            table.Rows.Add(221,206,'ｉ');
            table.Rows.Add(222,207,'ｊ');
            table.Rows.Add(223,208,'ｋ');
            table.Rows.Add(224,209,'ｌ');
            table.Rows.Add(225,210,'ｍ');
            table.Rows.Add(226,211,'ｎ');
            table.Rows.Add(227,212,'ｏ');
            table.Rows.Add(228,213,'ｐ');
            table.Rows.Add(229,214,'ｑ');
            table.Rows.Add(230,215,'ｒ');
            table.Rows.Add(231,216,'ｓ');
            table.Rows.Add(232,217,'ｔ');
            table.Rows.Add(233,218,'ｕ');
            table.Rows.Add(234,219,'ｖ');
            table.Rows.Add(235,220,'ｗ');
            table.Rows.Add(236,221,'ｘ');
            table.Rows.Add(237,222,'ｙ');
            table.Rows.Add(238,223,'ｚ');
            table.Rows.Add(239,289,'0');
            table.Rows.Add(240,452,':');
            table.Rows.Add(241,355,'Ä');
            table.Rows.Add(242,373,'Ö');
            table.Rows.Add(243,379,'Ü');
            table.Rows.Add(244,387,'ä');
            table.Rows.Add(245,405,'ö');
            table.Rows.Add(246,411,'ü');

            #endregion
            return table;
        }
        static DataTable Char4to5()
        {            
            // Converted from NebuK's Python Implementation SQL Database
            // http://projectpokemon.org/forums/showthread.php?14875
            // http://nopaste.ghostdub.de/?306
            DataTable table = new DataTable();
            table.Columns.Add("Old", typeof(int));
            table.Columns.Add("New", typeof(int));
            table.Columns.Add("Symbol", typeof(char));

            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["Old"]; // table.Rows.Find(val)[1] will look in the "Old" column, and return the "New" value.
            table.PrimaryKey = keyColumns;
            #region Old-New/Symbol Adding Entries
            table.Rows.Add(1, 12288, '　');
            table.Rows.Add(2, 12353, 'ぁ');
            table.Rows.Add(3, 12354, 'あ');
            table.Rows.Add(4, 12355, 'ぃ');
            table.Rows.Add(5, 12356, 'い');
            table.Rows.Add(6, 12357, 'ぅ');
            table.Rows.Add(7, 12358, 'う');
            table.Rows.Add(8, 12359, 'ぇ');
            table.Rows.Add(9, 12360, 'え');
            table.Rows.Add(10, 12361, 'ぉ');
            table.Rows.Add(11, 12362, 'お');
            table.Rows.Add(12, 12363, 'か');
            table.Rows.Add(13, 12364, 'が');
            table.Rows.Add(14, 12365, 'き');
            table.Rows.Add(15, 12366, 'ぎ');
            table.Rows.Add(16, 12367, 'く');
            table.Rows.Add(17, 12368, 'ぐ');
            table.Rows.Add(18, 12369, 'け');
            table.Rows.Add(19, 12370, 'げ');
            table.Rows.Add(20, 12371, 'こ');
            table.Rows.Add(21, 12372, 'ご');
            table.Rows.Add(22, 12373, 'さ');
            table.Rows.Add(23, 12374, 'ざ');
            table.Rows.Add(24, 12375, 'し');
            table.Rows.Add(25, 12376, 'じ');
            table.Rows.Add(26, 12377, 'す');
            table.Rows.Add(27, 12378, 'ず');
            table.Rows.Add(28, 12379, 'せ');
            table.Rows.Add(29, 12380, 'ぜ');
            table.Rows.Add(30, 12381, 'そ');
            table.Rows.Add(31, 12382, 'ぞ');
            table.Rows.Add(32, 12383, 'た');
            table.Rows.Add(33, 12384, 'だ');
            table.Rows.Add(34, 12385, 'ち');
            table.Rows.Add(35, 12386, 'ぢ');
            table.Rows.Add(36, 12387, 'っ');
            table.Rows.Add(37, 12388, 'つ');
            table.Rows.Add(38, 12389, 'づ');
            table.Rows.Add(39, 12390, 'て');
            table.Rows.Add(40, 12391, 'で');
            table.Rows.Add(41, 12392, 'と');
            table.Rows.Add(42, 12393, 'ど');
            table.Rows.Add(43, 12394, 'な');
            table.Rows.Add(44, 12395, 'に');
            table.Rows.Add(45, 12396, 'ぬ');
            table.Rows.Add(46, 12397, 'ね');
            table.Rows.Add(47, 12398, 'の');
            table.Rows.Add(48, 12399, 'は');
            table.Rows.Add(49, 12400, 'ば');
            table.Rows.Add(50, 12401, 'ぱ');
            table.Rows.Add(51, 12402, 'ひ');
            table.Rows.Add(52, 12403, 'び');
            table.Rows.Add(53, 12404, 'ぴ');
            table.Rows.Add(54, 12405, 'ふ');
            table.Rows.Add(55, 12406, 'ぶ');
            table.Rows.Add(56, 12407, 'ぷ');
            table.Rows.Add(57, 12408, 'へ');
            table.Rows.Add(58, 12409, 'べ');
            table.Rows.Add(59, 12410, 'ぺ');
            table.Rows.Add(60, 12411, 'ほ');
            table.Rows.Add(61, 12412, 'ぼ');
            table.Rows.Add(62, 12413, 'ぽ');
            table.Rows.Add(63, 12414, 'ま');
            table.Rows.Add(64, 12415, 'み');
            table.Rows.Add(65, 12416, 'む');
            table.Rows.Add(66, 12417, 'め');
            table.Rows.Add(67, 12418, 'も');
            table.Rows.Add(68, 12419, 'ゃ');
            table.Rows.Add(69, 12420, 'や');
            table.Rows.Add(70, 12421, 'ゅ');
            table.Rows.Add(71, 12422, 'ゆ');
            table.Rows.Add(72, 12423, 'ょ');
            table.Rows.Add(73, 12424, 'よ');
            table.Rows.Add(74, 12425, 'ら');
            table.Rows.Add(75, 12426, 'り');
            table.Rows.Add(76, 12427, 'る');
            table.Rows.Add(77, 12428, 'れ');
            table.Rows.Add(78, 12429, 'ろ');
            table.Rows.Add(79, 12431, 'わ');
            table.Rows.Add(80, 12434, 'を');
            table.Rows.Add(81, 12435, 'ん');
            table.Rows.Add(82, 12449, 'ァ');
            table.Rows.Add(83, 12450, 'ア');
            table.Rows.Add(84, 12451, 'ィ');
            table.Rows.Add(85, 12452, 'イ');
            table.Rows.Add(86, 12453, 'ゥ');
            table.Rows.Add(87, 12454, 'ウ');
            table.Rows.Add(88, 12455, 'ェ');
            table.Rows.Add(89, 12456, 'エ');
            table.Rows.Add(90, 12457, 'ォ');
            table.Rows.Add(91, 12458, 'オ');
            table.Rows.Add(92, 12459, 'カ');
            table.Rows.Add(93, 12460, 'ガ');
            table.Rows.Add(94, 12461, 'キ');
            table.Rows.Add(95, 12462, 'ギ');
            table.Rows.Add(96, 12463, 'ク');
            table.Rows.Add(97, 12464, 'グ');
            table.Rows.Add(98, 12465, 'ケ');
            table.Rows.Add(99, 12466, 'ゲ');
            table.Rows.Add(100, 12467, 'コ');
            table.Rows.Add(101, 12468, 'ゴ');
            table.Rows.Add(102, 12469, 'サ');
            table.Rows.Add(103, 12470, 'ザ');
            table.Rows.Add(104, 12471, 'シ');
            table.Rows.Add(105, 12472, 'ジ');
            table.Rows.Add(106, 12473, 'ス');
            table.Rows.Add(107, 12474, 'ズ');
            table.Rows.Add(108, 12475, 'セ');
            table.Rows.Add(109, 12476, 'ゼ');
            table.Rows.Add(110, 12477, 'ソ');
            table.Rows.Add(111, 12478, 'ゾ');
            table.Rows.Add(112, 12479, 'タ');
            table.Rows.Add(113, 12480, 'ダ');
            table.Rows.Add(114, 12481, 'チ');
            table.Rows.Add(115, 12482, 'ヂ');
            table.Rows.Add(116, 12483, 'ッ');
            table.Rows.Add(117, 12484, 'ツ');
            table.Rows.Add(118, 12485, 'ヅ');
            table.Rows.Add(119, 12486, 'テ');
            table.Rows.Add(120, 12487, 'デ');
            table.Rows.Add(121, 12488, 'ト');
            table.Rows.Add(122, 12489, 'ド');
            table.Rows.Add(123, 12490, 'ナ');
            table.Rows.Add(124, 12491, 'ニ');
            table.Rows.Add(125, 12492, 'ヌ');
            table.Rows.Add(126, 12493, 'ネ');
            table.Rows.Add(127, 12494, 'ノ');
            table.Rows.Add(128, 12495, 'ハ');
            table.Rows.Add(129, 12496, 'バ');
            table.Rows.Add(130, 12497, 'パ');
            table.Rows.Add(131, 12498, 'ヒ');
            table.Rows.Add(132, 12499, 'ビ');
            table.Rows.Add(133, 12500, 'ピ');
            table.Rows.Add(134, 12501, 'フ');
            table.Rows.Add(135, 12502, 'ブ');
            table.Rows.Add(136, 12503, 'プ');
            table.Rows.Add(137, 12504, 'ヘ');
            table.Rows.Add(138, 12505, 'ベ');
            table.Rows.Add(139, 12506, 'ペ');
            table.Rows.Add(140, 12507, 'ホ');
            table.Rows.Add(141, 12508, 'ボ');
            table.Rows.Add(142, 12509, 'ポ');
            table.Rows.Add(143, 12510, 'マ');
            table.Rows.Add(144, 12511, 'ミ');
            table.Rows.Add(145, 12512, 'ム');
            table.Rows.Add(146, 12513, 'メ');
            table.Rows.Add(147, 12514, 'モ');
            table.Rows.Add(148, 12515, 'ャ');
            table.Rows.Add(149, 12516, 'ヤ');
            table.Rows.Add(150, 12517, 'ュ');
            table.Rows.Add(151, 12518, 'ユ');
            table.Rows.Add(152, 12519, 'ョ');
            table.Rows.Add(153, 12520, 'ヨ');
            table.Rows.Add(154, 12521, 'ラ');
            table.Rows.Add(155, 12522, 'リ');
            table.Rows.Add(156, 12523, 'ル');
            table.Rows.Add(157, 12524, 'レ');
            table.Rows.Add(158, 12525, 'ロ');
            table.Rows.Add(159, 12527, 'ワ');
            table.Rows.Add(160, 12530, 'ヲ');
            table.Rows.Add(161, 12531, 'ン');
            table.Rows.Add(162, 65296, '０');
            table.Rows.Add(163, 65297, '１');
            table.Rows.Add(164, 65298, '２');
            table.Rows.Add(165, 65299, '３');
            table.Rows.Add(166, 65300, '４');
            table.Rows.Add(167, 65301, '５');
            table.Rows.Add(168, 65302, '６');
            table.Rows.Add(169, 65303, '７');
            table.Rows.Add(170, 65304, '８');
            table.Rows.Add(171, 65305, '９');
            table.Rows.Add(172, 65313, 'Ａ');
            table.Rows.Add(173, 65314, 'Ｂ');
            table.Rows.Add(174, 65315, 'Ｃ');
            table.Rows.Add(175, 65316, 'Ｄ');
            table.Rows.Add(176, 65317, 'Ｅ');
            table.Rows.Add(177, 65318, 'Ｆ');
            table.Rows.Add(178, 65319, 'Ｇ');
            table.Rows.Add(179, 65320, 'Ｈ');
            table.Rows.Add(180, 65321, 'Ｉ');
            table.Rows.Add(181, 65322, 'Ｊ');
            table.Rows.Add(182, 65323, 'Ｋ');
            table.Rows.Add(183, 65324, 'Ｌ');
            table.Rows.Add(184, 65325, 'Ｍ');
            table.Rows.Add(185, 65326, 'Ｎ');
            table.Rows.Add(186, 65327, 'Ｏ');
            table.Rows.Add(187, 65328, 'Ｐ');
            table.Rows.Add(188, 65329, 'Ｑ');
            table.Rows.Add(189, 65330, 'Ｒ');
            table.Rows.Add(190, 65331, 'Ｓ');
            table.Rows.Add(191, 65332, 'Ｔ');
            table.Rows.Add(192, 65333, 'Ｕ');
            table.Rows.Add(193, 65334, 'Ｖ');
            table.Rows.Add(194, 65335, 'Ｗ');
            table.Rows.Add(195, 65336, 'Ｘ');
            table.Rows.Add(196, 65337, 'Ｙ');
            table.Rows.Add(197, 65338, 'Ｚ');
            table.Rows.Add(198, 65345, 'ａ');
            table.Rows.Add(199, 65346, 'ｂ');
            table.Rows.Add(200, 65347, 'ｃ');
            table.Rows.Add(201, 65348, 'ｄ');
            table.Rows.Add(202, 65349, 'ｅ');
            table.Rows.Add(203, 65350, 'ｆ');
            table.Rows.Add(204, 65351, 'ｇ');
            table.Rows.Add(205, 65352, 'ｈ');
            table.Rows.Add(206, 65353, 'ｉ');
            table.Rows.Add(207, 65354, 'ｊ');
            table.Rows.Add(208, 65355, 'ｋ');
            table.Rows.Add(209, 65356, 'ｌ');
            table.Rows.Add(210, 65357, 'ｍ');
            table.Rows.Add(211, 65358, 'ｎ');
            table.Rows.Add(212, 65359, 'ｏ');
            table.Rows.Add(213, 65360, 'ｐ');
            table.Rows.Add(214, 65361, 'ｑ');
            table.Rows.Add(215, 65362, 'ｒ');
            table.Rows.Add(216, 65363, 'ｓ');
            table.Rows.Add(217, 65364, 'ｔ');
            table.Rows.Add(218, 65365, 'ｕ');
            table.Rows.Add(219, 65366, 'ｖ');
            table.Rows.Add(220, 65367, 'ｗ');
            table.Rows.Add(221, 65368, 'ｘ');
            table.Rows.Add(222, 65369, 'ｙ');
            table.Rows.Add(223, 65370, 'ｚ');
            table.Rows.Add(225, 65281, '！');
            table.Rows.Add(226, 65311, '？');
            table.Rows.Add(227, 12289, '、');
            table.Rows.Add(228, 12290, '。');
            table.Rows.Add(229, 8943, '⋯');
            table.Rows.Add(230, 12539, '・');
            table.Rows.Add(231, 65295, '／');
            table.Rows.Add(232, 12300, '「');
            table.Rows.Add(233, 12301, '」');
            table.Rows.Add(234, 12302, '『');
            table.Rows.Add(235, 12303, '』');
            table.Rows.Add(236, 65288, '（');
            table.Rows.Add(237, 65289, '）');
            table.Rows.Add(238, 9325, '♂');
            table.Rows.Add(239, 9326, '♀');
            table.Rows.Add(240, 65291, '＋');
            table.Rows.Add(241, 65293, '－');
            table.Rows.Add(242, 9319, '×');
            table.Rows.Add(243, 9320, '÷');
            table.Rows.Add(244, 65309, '＝');
            table.Rows.Add(245, 65370, 'ｚ');
            table.Rows.Add(246, 65306, '：');
            table.Rows.Add(247, 65307, '；');
            table.Rows.Add(248, 65294, '．');
            table.Rows.Add(249, 65292, '，');
            table.Rows.Add(250, 9327, '♤');
            table.Rows.Add(251, 9328, '♧');
            table.Rows.Add(252, 9329, '♥');
            table.Rows.Add(253, 9330, '♢');
            table.Rows.Add(254, 9331, '☆');
            table.Rows.Add(255, 9332, '◎');
            table.Rows.Add(256, 9333, '○');
            table.Rows.Add(257, 9334, '□');
            table.Rows.Add(258, 9335, '△');
            table.Rows.Add(259, 9336, '◇');
            table.Rows.Add(260, 65312, '＠');
            table.Rows.Add(261, 9337, '♪');
            table.Rows.Add(262, 65285, '％');
            table.Rows.Add(263, 9338, '☀');
            table.Rows.Add(264, 9339, '☁');
            table.Rows.Add(265, 9341, '☂');
            table.Rows.Add(266, 10052, '❄');
            table.Rows.Add(267, 9739, '☋');
            table.Rows.Add(268, 9812, '♔');
            table.Rows.Add(269, 9813, '♕');
            table.Rows.Add(270, 9738, '☊');
            table.Rows.Add(271, 8663, '⇗');
            table.Rows.Add(272, 8664, '⇘');
            table.Rows.Add(273, 9790, '☾');
            table.Rows.Add(274, 165, '¥');
            table.Rows.Add(275, 9800, '♈');
            table.Rows.Add(276, 9801, '♉');
            table.Rows.Add(277, 9802, '♊');
            table.Rows.Add(278, 9803, '♋');
            table.Rows.Add(279, 9804, '♌');
            table.Rows.Add(280, 9805, '♍');
            table.Rows.Add(281, 9806, '♎');
            table.Rows.Add(282, 9807, '♏');
            table.Rows.Add(283, 8592, '←');
            table.Rows.Add(284, 8593, '↑');
            table.Rows.Add(285, 8595, '↓');
            table.Rows.Add(286, 8594, '→');
            table.Rows.Add(287, 8227, '‣');
            table.Rows.Add(288, 65286, '＆');
            table.Rows.Add(289, 48, '0');
            table.Rows.Add(290, 49, '1');
            table.Rows.Add(291, 50, '2');
            table.Rows.Add(292, 51, '3');
            table.Rows.Add(293, 52, '4');
            table.Rows.Add(294, 53, '5');
            table.Rows.Add(295, 54, '6');
            table.Rows.Add(296, 55, '7');
            table.Rows.Add(297, 56, '8');
            table.Rows.Add(298, 57, '9');
            table.Rows.Add(299, 65, 'A');
            table.Rows.Add(300, 66, 'B');
            table.Rows.Add(301, 67, 'C');
            table.Rows.Add(302, 68, 'D');
            table.Rows.Add(303, 69, 'E');
            table.Rows.Add(304, 70, 'F');
            table.Rows.Add(305, 71, 'G');
            table.Rows.Add(306, 72, 'H');
            table.Rows.Add(307, 73, 'I');
            table.Rows.Add(308, 74, 'J');
            table.Rows.Add(309, 75, 'K');
            table.Rows.Add(310, 76, 'L');
            table.Rows.Add(311, 77, 'M');
            table.Rows.Add(312, 78, 'N');
            table.Rows.Add(313, 79, 'O');
            table.Rows.Add(314, 80, 'P');
            table.Rows.Add(315, 81, 'Q');
            table.Rows.Add(316, 82, 'R');
            table.Rows.Add(317, 83, 'S');
            table.Rows.Add(318, 84, 'T');
            table.Rows.Add(319, 85, 'U');
            table.Rows.Add(320, 86, 'V');
            table.Rows.Add(321, 87, 'W');
            table.Rows.Add(322, 88, 'X');
            table.Rows.Add(323, 89, 'Y');
            table.Rows.Add(324, 90, 'Z');
            table.Rows.Add(325, 97, 'a');
            table.Rows.Add(326, 98, 'b');
            table.Rows.Add(327, 99, 'c');
            table.Rows.Add(328, 100, 'd');
            table.Rows.Add(329, 101, 'e');
            table.Rows.Add(330, 102, 'f');
            table.Rows.Add(331, 103, 'g');
            table.Rows.Add(332, 104, 'h');
            table.Rows.Add(333, 105, 'i');
            table.Rows.Add(334, 106, 'j');
            table.Rows.Add(335, 107, 'k');
            table.Rows.Add(336, 108, 'l');
            table.Rows.Add(337, 109, 'm');
            table.Rows.Add(338, 110, 'n');
            table.Rows.Add(339, 111, 'o');
            table.Rows.Add(340, 112, 'p');
            table.Rows.Add(341, 113, 'q');
            table.Rows.Add(342, 114, 'r');
            table.Rows.Add(343, 115, 's');
            table.Rows.Add(344, 116, 't');
            table.Rows.Add(345, 117, 'u');
            table.Rows.Add(346, 118, 'v');
            table.Rows.Add(347, 119, 'w');
            table.Rows.Add(348, 120, 'x');
            table.Rows.Add(349, 121, 'y');
            table.Rows.Add(350, 122, 'z');
            table.Rows.Add(351, 192, 'À');
            table.Rows.Add(352, 193, 'Á');
            table.Rows.Add(353, 194, 'Â');
            table.Rows.Add(354, 195, 'Ã');
            table.Rows.Add(355, 196, 'Ä');
            table.Rows.Add(356, 197, 'Å');
            table.Rows.Add(357, 198, 'Æ');
            table.Rows.Add(358, 199, 'Ç');
            table.Rows.Add(359, 200, 'È');
            table.Rows.Add(360, 201, 'É');
            table.Rows.Add(361, 202, 'Ê');
            table.Rows.Add(362, 203, 'Ë');
            table.Rows.Add(363, 204, 'Ì');
            table.Rows.Add(364, 205, 'Í');
            table.Rows.Add(365, 206, 'Î');
            table.Rows.Add(366, 207, 'Ï');
            table.Rows.Add(367, 208, 'Ð');
            table.Rows.Add(368, 209, 'Ñ');
            table.Rows.Add(369, 210, 'Ò');
            table.Rows.Add(370, 211, 'Ó');
            table.Rows.Add(371, 212, 'Ô');
            table.Rows.Add(372, 213, 'Õ');
            table.Rows.Add(373, 214, 'Ö');
            table.Rows.Add(374, 215, '×');
            table.Rows.Add(375, 216, 'Ø');
            table.Rows.Add(376, 217, 'Ù');
            table.Rows.Add(377, 218, 'Ú');
            table.Rows.Add(378, 219, 'Û');
            table.Rows.Add(379, 220, 'Ü');
            table.Rows.Add(380, 221, 'Ý');
            table.Rows.Add(381, 222, 'Þ');
            table.Rows.Add(382, 223, 'ß');
            table.Rows.Add(383, 224, 'à');
            table.Rows.Add(384, 225, 'á');
            table.Rows.Add(385, 226, 'â');
            table.Rows.Add(386, 227, 'ã');
            table.Rows.Add(387, 228, 'ä');
            table.Rows.Add(388, 229, 'å');
            table.Rows.Add(389, 230, 'æ');
            table.Rows.Add(390, 231, 'ç');
            table.Rows.Add(391, 232, 'è');
            table.Rows.Add(392, 233, 'é');
            table.Rows.Add(393, 234, 'ê');
            table.Rows.Add(394, 235, 'ë');
            table.Rows.Add(395, 236, 'ì');
            table.Rows.Add(396, 237, 'í');
            table.Rows.Add(397, 238, 'î');
            table.Rows.Add(398, 239, 'ï');
            table.Rows.Add(399, 240, 'ð');
            table.Rows.Add(400, 241, 'ñ');
            table.Rows.Add(401, 242, 'ò');
            table.Rows.Add(402, 243, 'ó');
            table.Rows.Add(403, 244, 'ô');
            table.Rows.Add(404, 245, 'õ');
            table.Rows.Add(405, 246, 'ö');
            table.Rows.Add(406, 247, '÷');
            table.Rows.Add(407, 248, 'ø');
            table.Rows.Add(408, 249, 'ù');
            table.Rows.Add(409, 250, 'ú');
            table.Rows.Add(410, 251, 'û');
            table.Rows.Add(411, 252, 'ü');
            table.Rows.Add(412, 253, 'ý');
            table.Rows.Add(413, 254, 'þ');
            table.Rows.Add(414, 255, 'ÿ');
            table.Rows.Add(415, 338, 'Œ');
            table.Rows.Add(416, 339, 'œ');
            table.Rows.Add(417, 350, 'Ş');
            table.Rows.Add(418, 351, 'ş');
            table.Rows.Add(419, 170, 'ª');
            table.Rows.Add(420, 186, 'º');
            table.Rows.Add(421, 185, '¹');
            table.Rows.Add(422, 178, '²');
            table.Rows.Add(423, 179, '³');
            table.Rows.Add(424, 36, '$');
            table.Rows.Add(425, 161, '¡');
            table.Rows.Add(426, 191, '¿');
            table.Rows.Add(427, 33, '!');
            table.Rows.Add(428, 63, '?');
            table.Rows.Add(429, 44, ',');
            table.Rows.Add(430, 46, '.');
            table.Rows.Add(431, 9324, '…');
            table.Rows.Add(432, 65381, '･');
            table.Rows.Add(433, 47, '/');
            table.Rows.Add(434, 8216, '‘');
            table.Rows.Add(435, 8217, '’');
            table.Rows.Add(436, 8220, '“');
            table.Rows.Add(437, 8221, '”');
            table.Rows.Add(438, 8222, '„');
            table.Rows.Add(439, 12298, '《');
            table.Rows.Add(440, 12299, '》');
            table.Rows.Add(441, 40, '(');
            table.Rows.Add(442, 41, ')');
            table.Rows.Add(443, 9794, '♂');
            table.Rows.Add(444, 9792, '♀');
            table.Rows.Add(445, 43, '+');
            table.Rows.Add(446, 45, '-');
            table.Rows.Add(447, 42, '*');
            table.Rows.Add(448, 35, '#');
            table.Rows.Add(449, 61, '=');
            table.Rows.Add(450, 38, '&');
            table.Rows.Add(451, 126, '~');
            table.Rows.Add(452, 58, ':');
            table.Rows.Add(453, 59, ';');
            table.Rows.Add(454, 9327, '⑯');
            table.Rows.Add(455, 9328, '⑰');
            table.Rows.Add(456, 9329, '⑱');
            table.Rows.Add(457, 9330, '⑲');
            table.Rows.Add(458, 9331, '⑳');
            table.Rows.Add(459, 9332, '⑴');
            table.Rows.Add(460, 9333, '⑵');
            table.Rows.Add(461, 9334, '⑶');
            table.Rows.Add(462, 9335, '⑷');
            table.Rows.Add(463, 9336, '⑸');
            table.Rows.Add(464, 64, '@');
            table.Rows.Add(465, 9337, '⑹');
            table.Rows.Add(466, 37, '%');
            table.Rows.Add(467, 9338, '⑺');
            table.Rows.Add(468, 9339, '⑻');
            table.Rows.Add(469, 9340, '⑼');
            table.Rows.Add(470, 9341, '⑽');
            table.Rows.Add(471, 9342, '⑾');
            table.Rows.Add(472, 9343, '⑿');
            table.Rows.Add(473, 9344, '⒀');
            table.Rows.Add(474, 9345, '⒁');
            table.Rows.Add(475, 9346, '⒂');
            table.Rows.Add(476, 9347, '⒃');
            table.Rows.Add(477, 9348, '⒄');
            table.Rows.Add(478, 32, ' ');
            table.Rows.Add(479, 9349, '⒅');
            table.Rows.Add(480, 9350, '⒆');
            table.Rows.Add(481, 9351, '⒇');
            table.Rows.Add(488, 176, '°');
            table.Rows.Add(489, 95, '_');
            table.Rows.Add(490, 65343, '＿');
            table.Rows.Add(1024, 44032, '가');
            table.Rows.Add(1025, 44033, '각');
            table.Rows.Add(1026, 44036, '간');
            table.Rows.Add(1027, 44039, '갇');
            table.Rows.Add(1028, 44040, '갈');
            table.Rows.Add(1029, 44041, '갉');
            table.Rows.Add(1030, 44042, '갊');
            table.Rows.Add(1031, 44048, '감');
            table.Rows.Add(1032, 44049, '갑');
            table.Rows.Add(1033, 44050, '값');
            table.Rows.Add(1034, 44051, '갓');
            table.Rows.Add(1035, 44052, '갔');
            table.Rows.Add(1036, 44053, '강');
            table.Rows.Add(1037, 44054, '갖');
            table.Rows.Add(1038, 44055, '갗');
            table.Rows.Add(1040, 44057, '같');
            table.Rows.Add(1041, 44058, '갚');
            table.Rows.Add(1042, 44059, '갛');
            table.Rows.Add(1043, 44060, '개');
            table.Rows.Add(1044, 44061, '객');
            table.Rows.Add(1045, 44064, '갠');
            table.Rows.Add(1046, 44068, '갤');
            table.Rows.Add(1047, 44076, '갬');
            table.Rows.Add(1048, 44077, '갭');
            table.Rows.Add(1049, 44079, '갯');
            table.Rows.Add(1050, 44080, '갰');
            table.Rows.Add(1051, 44081, '갱');
            table.Rows.Add(1052, 44088, '갸');
            table.Rows.Add(1053, 44089, '갹');
            table.Rows.Add(1054, 44092, '갼');
            table.Rows.Add(1055, 44096, '걀');
            table.Rows.Add(1056, 44107, '걋');
            table.Rows.Add(1057, 44109, '걍');
            table.Rows.Add(1058, 44116, '걔');
            table.Rows.Add(1059, 44120, '걘');
            table.Rows.Add(1060, 44124, '걜');
            table.Rows.Add(1061, 44144, '거');
            table.Rows.Add(1062, 44145, '걱');
            table.Rows.Add(1063, 44148, '건');
            table.Rows.Add(1064, 44151, '걷');
            table.Rows.Add(1065, 44152, '걸');
            table.Rows.Add(1066, 44154, '걺');
            table.Rows.Add(1067, 44160, '검');
            table.Rows.Add(1068, 44161, '겁');
            table.Rows.Add(1069, 44163, '것');
            table.Rows.Add(1070, 44164, '겄');
            table.Rows.Add(1071, 44165, '겅');
            table.Rows.Add(1072, 44166, '겆');
            table.Rows.Add(1073, 44169, '겉');
            table.Rows.Add(1074, 44170, '겊');
            table.Rows.Add(1075, 44171, '겋');
            table.Rows.Add(1076, 44172, '게');
            table.Rows.Add(1077, 44176, '겐');
            table.Rows.Add(1078, 44180, '겔');
            table.Rows.Add(1079, 44188, '겜');
            table.Rows.Add(1080, 44189, '겝');
            table.Rows.Add(1081, 44191, '겟');
            table.Rows.Add(1082, 44192, '겠');
            table.Rows.Add(1083, 44193, '겡');
            table.Rows.Add(1084, 44200, '겨');
            table.Rows.Add(1085, 44201, '격');
            table.Rows.Add(1086, 44202, '겪');
            table.Rows.Add(1087, 44204, '견');
            table.Rows.Add(1088, 44207, '겯');
            table.Rows.Add(1089, 44208, '결');
            table.Rows.Add(1090, 44216, '겸');
            table.Rows.Add(1091, 44217, '겹');
            table.Rows.Add(1092, 44219, '겻');
            table.Rows.Add(1093, 44220, '겼');
            table.Rows.Add(1094, 44221, '경');
            table.Rows.Add(1095, 44225, '곁');
            table.Rows.Add(1096, 44228, '계');
            table.Rows.Add(1097, 44232, '곈');
            table.Rows.Add(1098, 44236, '곌');
            table.Rows.Add(1099, 44245, '곕');
            table.Rows.Add(1100, 44247, '곗');
            table.Rows.Add(1101, 44256, '고');
            table.Rows.Add(1102, 44257, '곡');
            table.Rows.Add(1103, 44260, '곤');
            table.Rows.Add(1104, 44263, '곧');
            table.Rows.Add(1105, 44264, '골');
            table.Rows.Add(1106, 44266, '곪');
            table.Rows.Add(1107, 44268, '곬');
            table.Rows.Add(1108, 44271, '곯');
            table.Rows.Add(1109, 44272, '곰');
            table.Rows.Add(1110, 44273, '곱');
            table.Rows.Add(1111, 44275, '곳');
            table.Rows.Add(1112, 44277, '공');
            table.Rows.Add(1113, 44278, '곶');
            table.Rows.Add(1114, 44284, '과');
            table.Rows.Add(1115, 44285, '곽');
            table.Rows.Add(1116, 44288, '관');
            table.Rows.Add(1117, 44292, '괄');
            table.Rows.Add(1118, 44294, '괆');
            table.Rows.Add(1119, 44300, '괌');
            table.Rows.Add(1120, 44301, '괍');
            table.Rows.Add(1121, 44303, '괏');
            table.Rows.Add(1122, 44305, '광');
            table.Rows.Add(1123, 44312, '괘');
            table.Rows.Add(1124, 44316, '괜');
            table.Rows.Add(1125, 44320, '괠');
            table.Rows.Add(1126, 44329, '괩');
            table.Rows.Add(1127, 44332, '괬');
            table.Rows.Add(1128, 44333, '괭');
            table.Rows.Add(1129, 44340, '괴');
            table.Rows.Add(1130, 44341, '괵');
            table.Rows.Add(1131, 44344, '괸');
            table.Rows.Add(1132, 44348, '괼');
            table.Rows.Add(1133, 44356, '굄');
            table.Rows.Add(1134, 44357, '굅');
            table.Rows.Add(1135, 44359, '굇');
            table.Rows.Add(1136, 44361, '굉');
            table.Rows.Add(1137, 44368, '교');
            table.Rows.Add(1138, 44372, '굔');
            table.Rows.Add(1139, 44376, '굘');
            table.Rows.Add(1140, 44385, '굡');
            table.Rows.Add(1141, 44387, '굣');
            table.Rows.Add(1142, 44396, '구');
            table.Rows.Add(1143, 44397, '국');
            table.Rows.Add(1144, 44400, '군');
            table.Rows.Add(1145, 44403, '굳');
            table.Rows.Add(1146, 44404, '굴');
            table.Rows.Add(1147, 44405, '굵');
            table.Rows.Add(1148, 44406, '굶');
            table.Rows.Add(1149, 44411, '굻');
            table.Rows.Add(1150, 44412, '굼');
            table.Rows.Add(1151, 44413, '굽');
            table.Rows.Add(1152, 44415, '굿');
            table.Rows.Add(1153, 44417, '궁');
            table.Rows.Add(1154, 44418, '궂');
            table.Rows.Add(1155, 44424, '궈');
            table.Rows.Add(1156, 44425, '궉');
            table.Rows.Add(1157, 44428, '권');
            table.Rows.Add(1158, 44432, '궐');
            table.Rows.Add(1159, 44444, '궜');
            table.Rows.Add(1160, 44445, '궝');
            table.Rows.Add(1161, 44452, '궤');
            table.Rows.Add(1162, 44471, '궷');
            table.Rows.Add(1163, 44480, '귀');
            table.Rows.Add(1164, 44481, '귁');
            table.Rows.Add(1165, 44484, '귄');
            table.Rows.Add(1166, 44488, '귈');
            table.Rows.Add(1167, 44496, '귐');
            table.Rows.Add(1168, 44497, '귑');
            table.Rows.Add(1169, 44499, '귓');
            table.Rows.Add(1170, 44508, '규');
            table.Rows.Add(1171, 44512, '균');
            table.Rows.Add(1172, 44516, '귤');
            table.Rows.Add(1173, 44536, '그');
            table.Rows.Add(1174, 44537, '극');
            table.Rows.Add(1175, 44540, '근');
            table.Rows.Add(1176, 44543, '귿');
            table.Rows.Add(1177, 44544, '글');
            table.Rows.Add(1178, 44545, '긁');
            table.Rows.Add(1179, 44552, '금');
            table.Rows.Add(1180, 44553, '급');
            table.Rows.Add(1181, 44555, '긋');
            table.Rows.Add(1182, 44557, '긍');
            table.Rows.Add(1183, 44564, '긔');
            table.Rows.Add(1184, 44592, '기');
            table.Rows.Add(1185, 44593, '긱');
            table.Rows.Add(1186, 44596, '긴');
            table.Rows.Add(1187, 44599, '긷');
            table.Rows.Add(1188, 44600, '길');
            table.Rows.Add(1189, 44602, '긺');
            table.Rows.Add(1190, 44608, '김');
            table.Rows.Add(1191, 44609, '깁');
            table.Rows.Add(1192, 44611, '깃');
            table.Rows.Add(1193, 44613, '깅');
            table.Rows.Add(1194, 44614, '깆');
            table.Rows.Add(1195, 44618, '깊');
            table.Rows.Add(1196, 44620, '까');
            table.Rows.Add(1197, 44621, '깍');
            table.Rows.Add(1198, 44622, '깎');
            table.Rows.Add(1199, 44624, '깐');
            table.Rows.Add(1200, 44628, '깔');
            table.Rows.Add(1201, 44630, '깖');
            table.Rows.Add(1202, 44636, '깜');
            table.Rows.Add(1203, 44637, '깝');
            table.Rows.Add(1204, 44639, '깟');
            table.Rows.Add(1205, 44640, '깠');
            table.Rows.Add(1206, 44641, '깡');
            table.Rows.Add(1207, 44645, '깥');
            table.Rows.Add(1208, 44648, '깨');
            table.Rows.Add(1209, 44649, '깩');
            table.Rows.Add(1210, 44652, '깬');
            table.Rows.Add(1211, 44656, '깰');
            table.Rows.Add(1212, 44664, '깸');
            table.Rows.Add(1213, 44665, '깹');
            table.Rows.Add(1214, 44667, '깻');
            table.Rows.Add(1215, 44668, '깼');
            table.Rows.Add(1216, 44669, '깽');
            table.Rows.Add(1217, 44676, '꺄');
            table.Rows.Add(1218, 44677, '꺅');
            table.Rows.Add(1219, 44684, '꺌');
            table.Rows.Add(1220, 44732, '꺼');
            table.Rows.Add(1221, 44733, '꺽');
            table.Rows.Add(1222, 44734, '꺾');
            table.Rows.Add(1223, 44736, '껀');
            table.Rows.Add(1224, 44740, '껄');
            table.Rows.Add(1225, 44748, '껌');
            table.Rows.Add(1226, 44749, '껍');
            table.Rows.Add(1227, 44751, '껏');
            table.Rows.Add(1228, 44752, '껐');
            table.Rows.Add(1229, 44753, '껑');
            table.Rows.Add(1230, 44760, '께');
            table.Rows.Add(1231, 44761, '껙');
            table.Rows.Add(1232, 44764, '껜');
            table.Rows.Add(1233, 44776, '껨');
            table.Rows.Add(1234, 44779, '껫');
            table.Rows.Add(1235, 44781, '껭');
            table.Rows.Add(1236, 44788, '껴');
            table.Rows.Add(1237, 44792, '껸');
            table.Rows.Add(1238, 44796, '껼');
            table.Rows.Add(1239, 44807, '꼇');
            table.Rows.Add(1240, 44808, '꼈');
            table.Rows.Add(1241, 44813, '꼍');
            table.Rows.Add(1242, 44816, '꼐');
            table.Rows.Add(1243, 44844, '꼬');
            table.Rows.Add(1244, 44845, '꼭');
            table.Rows.Add(1245, 44848, '꼰');
            table.Rows.Add(1246, 44850, '꼲');
            table.Rows.Add(1247, 44852, '꼴');
            table.Rows.Add(1248, 44860, '꼼');
            table.Rows.Add(1249, 44861, '꼽');
            table.Rows.Add(1250, 44863, '꼿');
            table.Rows.Add(1251, 44865, '꽁');
            table.Rows.Add(1252, 44866, '꽂');
            table.Rows.Add(1253, 44867, '꽃');
            table.Rows.Add(1254, 44872, '꽈');
            table.Rows.Add(1255, 44873, '꽉');
            table.Rows.Add(1256, 44880, '꽐');
            table.Rows.Add(1257, 44892, '꽜');
            table.Rows.Add(1258, 44893, '꽝');
            table.Rows.Add(1259, 44900, '꽤');
            table.Rows.Add(1260, 44901, '꽥');
            table.Rows.Add(1261, 44921, '꽹');
            table.Rows.Add(1262, 44928, '꾀');
            table.Rows.Add(1263, 44932, '꾄');
            table.Rows.Add(1264, 44936, '꾈');
            table.Rows.Add(1265, 44944, '꾐');
            table.Rows.Add(1266, 44945, '꾑');
            table.Rows.Add(1267, 44949, '꾕');
            table.Rows.Add(1268, 44956, '꾜');
            table.Rows.Add(1269, 44984, '꾸');
            table.Rows.Add(1270, 44985, '꾹');
            table.Rows.Add(1271, 44988, '꾼');
            table.Rows.Add(1272, 44992, '꿀');
            table.Rows.Add(1273, 44999, '꿇');
            table.Rows.Add(1274, 45000, '꿈');
            table.Rows.Add(1275, 45001, '꿉');
            table.Rows.Add(1276, 45003, '꿋');
            table.Rows.Add(1277, 45005, '꿍');
            table.Rows.Add(1278, 45006, '꿎');
            table.Rows.Add(1279, 45012, '꿔');
            table.Rows.Add(1280, 45020, '꿜');
            table.Rows.Add(1281, 45032, '꿨');
            table.Rows.Add(1282, 45033, '꿩');
            table.Rows.Add(1283, 45040, '꿰');
            table.Rows.Add(1284, 45041, '꿱');
            table.Rows.Add(1285, 45044, '꿴');
            table.Rows.Add(1286, 45048, '꿸');
            table.Rows.Add(1287, 45056, '뀀');
            table.Rows.Add(1288, 45057, '뀁');
            table.Rows.Add(1289, 45060, '뀄');
            table.Rows.Add(1290, 45068, '뀌');
            table.Rows.Add(1291, 45072, '뀐');
            table.Rows.Add(1292, 45076, '뀔');
            table.Rows.Add(1293, 45084, '뀜');
            table.Rows.Add(1294, 45085, '뀝');
            table.Rows.Add(1295, 45096, '뀨');
            table.Rows.Add(1296, 45124, '끄');
            table.Rows.Add(1297, 45125, '끅');
            table.Rows.Add(1298, 45128, '끈');
            table.Rows.Add(1299, 45130, '끊');
            table.Rows.Add(1300, 45132, '끌');
            table.Rows.Add(1301, 45134, '끎');
            table.Rows.Add(1302, 45139, '끓');
            table.Rows.Add(1303, 45140, '끔');
            table.Rows.Add(1304, 45141, '끕');
            table.Rows.Add(1305, 45143, '끗');
            table.Rows.Add(1306, 45145, '끙');
            table.Rows.Add(1307, 45149, '끝');
            table.Rows.Add(1308, 45180, '끼');
            table.Rows.Add(1309, 45181, '끽');
            table.Rows.Add(1310, 45184, '낀');
            table.Rows.Add(1311, 45188, '낄');
            table.Rows.Add(1312, 45196, '낌');
            table.Rows.Add(1313, 45197, '낍');
            table.Rows.Add(1314, 45199, '낏');
            table.Rows.Add(1315, 45201, '낑');
            table.Rows.Add(1316, 45208, '나');
            table.Rows.Add(1317, 45209, '낙');
            table.Rows.Add(1318, 45210, '낚');
            table.Rows.Add(1319, 45212, '난');
            table.Rows.Add(1320, 45215, '낟');
            table.Rows.Add(1321, 45216, '날');
            table.Rows.Add(1322, 45217, '낡');
            table.Rows.Add(1323, 45218, '낢');
            table.Rows.Add(1324, 45224, '남');
            table.Rows.Add(1325, 45225, '납');
            table.Rows.Add(1326, 45227, '낫');
            table.Rows.Add(1327, 45228, '났');
            table.Rows.Add(1328, 45229, '낭');
            table.Rows.Add(1329, 45230, '낮');
            table.Rows.Add(1330, 45231, '낯');
            table.Rows.Add(1331, 45233, '낱');
            table.Rows.Add(1332, 45235, '낳');
            table.Rows.Add(1333, 45236, '내');
            table.Rows.Add(1334, 45237, '낵');
            table.Rows.Add(1335, 45240, '낸');
            table.Rows.Add(1336, 45244, '낼');
            table.Rows.Add(1337, 45252, '냄');
            table.Rows.Add(1338, 45253, '냅');
            table.Rows.Add(1339, 45255, '냇');
            table.Rows.Add(1340, 45256, '냈');
            table.Rows.Add(1341, 45257, '냉');
            table.Rows.Add(1342, 45264, '냐');
            table.Rows.Add(1343, 45265, '냑');
            table.Rows.Add(1344, 45268, '냔');
            table.Rows.Add(1345, 45272, '냘');
            table.Rows.Add(1346, 45280, '냠');
            table.Rows.Add(1347, 45285, '냥');
            table.Rows.Add(1348, 45320, '너');
            table.Rows.Add(1349, 45321, '넉');
            table.Rows.Add(1350, 45323, '넋');
            table.Rows.Add(1351, 45324, '넌');
            table.Rows.Add(1352, 45328, '널');
            table.Rows.Add(1353, 45330, '넒');
            table.Rows.Add(1354, 45331, '넓');
            table.Rows.Add(1355, 45336, '넘');
            table.Rows.Add(1356, 45337, '넙');
            table.Rows.Add(1357, 45339, '넛');
            table.Rows.Add(1358, 45340, '넜');
            table.Rows.Add(1359, 45341, '넝');
            table.Rows.Add(1360, 45347, '넣');
            table.Rows.Add(1361, 45348, '네');
            table.Rows.Add(1362, 45349, '넥');
            table.Rows.Add(1363, 45352, '넨');
            table.Rows.Add(1364, 45356, '넬');
            table.Rows.Add(1365, 45364, '넴');
            table.Rows.Add(1366, 45365, '넵');
            table.Rows.Add(1367, 45367, '넷');
            table.Rows.Add(1368, 45368, '넸');
            table.Rows.Add(1369, 45369, '넹');
            table.Rows.Add(1370, 45376, '녀');
            table.Rows.Add(1371, 45377, '녁');
            table.Rows.Add(1372, 45380, '년');
            table.Rows.Add(1373, 45384, '녈');
            table.Rows.Add(1374, 45392, '념');
            table.Rows.Add(1375, 45393, '녑');
            table.Rows.Add(1376, 45396, '녔');
            table.Rows.Add(1377, 45397, '녕');
            table.Rows.Add(1378, 45400, '녘');
            table.Rows.Add(1379, 45404, '녜');
            table.Rows.Add(1380, 45408, '녠');
            table.Rows.Add(1381, 45432, '노');
            table.Rows.Add(1382, 45433, '녹');
            table.Rows.Add(1383, 45436, '논');
            table.Rows.Add(1384, 45440, '놀');
            table.Rows.Add(1385, 45442, '놂');
            table.Rows.Add(1386, 45448, '놈');
            table.Rows.Add(1387, 45449, '놉');
            table.Rows.Add(1388, 45451, '놋');
            table.Rows.Add(1389, 45453, '농');
            table.Rows.Add(1390, 45458, '높');
            table.Rows.Add(1391, 45459, '놓');
            table.Rows.Add(1392, 45460, '놔');
            table.Rows.Add(1393, 45464, '놘');
            table.Rows.Add(1394, 45468, '놜');
            table.Rows.Add(1395, 45480, '놨');
            table.Rows.Add(1396, 45516, '뇌');
            table.Rows.Add(1397, 45520, '뇐');
            table.Rows.Add(1398, 45524, '뇔');
            table.Rows.Add(1399, 45532, '뇜');
            table.Rows.Add(1400, 45533, '뇝');
            table.Rows.Add(1401, 45535, '뇟');
            table.Rows.Add(1402, 45544, '뇨');
            table.Rows.Add(1403, 45545, '뇩');
            table.Rows.Add(1404, 45548, '뇬');
            table.Rows.Add(1405, 45552, '뇰');
            table.Rows.Add(1406, 45561, '뇹');
            table.Rows.Add(1407, 45563, '뇻');
            table.Rows.Add(1408, 45565, '뇽');
            table.Rows.Add(1409, 45572, '누');
            table.Rows.Add(1410, 45573, '눅');
            table.Rows.Add(1411, 45576, '눈');
            table.Rows.Add(1412, 45579, '눋');
            table.Rows.Add(1413, 45580, '눌');
            table.Rows.Add(1414, 45588, '눔');
            table.Rows.Add(1415, 45589, '눕');
            table.Rows.Add(1416, 45591, '눗');
            table.Rows.Add(1417, 45593, '눙');
            table.Rows.Add(1418, 45600, '눠');
            table.Rows.Add(1419, 45620, '눴');
            table.Rows.Add(1420, 45628, '눼');
            table.Rows.Add(1421, 45656, '뉘');
            table.Rows.Add(1422, 45660, '뉜');
            table.Rows.Add(1423, 45664, '뉠');
            table.Rows.Add(1424, 45672, '뉨');
            table.Rows.Add(1425, 45673, '뉩');
            table.Rows.Add(1426, 45684, '뉴');
            table.Rows.Add(1427, 45685, '뉵');
            table.Rows.Add(1428, 45692, '뉼');
            table.Rows.Add(1429, 45700, '늄');
            table.Rows.Add(1430, 45701, '늅');
            table.Rows.Add(1431, 45705, '늉');
            table.Rows.Add(1432, 45712, '느');
            table.Rows.Add(1433, 45713, '늑');
            table.Rows.Add(1434, 45716, '는');
            table.Rows.Add(1435, 45720, '늘');
            table.Rows.Add(1436, 45721, '늙');
            table.Rows.Add(1437, 45722, '늚');
            table.Rows.Add(1438, 45728, '늠');
            table.Rows.Add(1439, 45729, '늡');
            table.Rows.Add(1440, 45731, '늣');
            table.Rows.Add(1441, 45733, '능');
            table.Rows.Add(1442, 45734, '늦');
            table.Rows.Add(1443, 45738, '늪');
            table.Rows.Add(1444, 45740, '늬');
            table.Rows.Add(1445, 45744, '늰');
            table.Rows.Add(1446, 45748, '늴');
            table.Rows.Add(1447, 45768, '니');
            table.Rows.Add(1448, 45769, '닉');
            table.Rows.Add(1449, 45772, '닌');
            table.Rows.Add(1450, 45776, '닐');
            table.Rows.Add(1451, 45778, '닒');
            table.Rows.Add(1452, 45784, '님');
            table.Rows.Add(1453, 45785, '닙');
            table.Rows.Add(1454, 45787, '닛');
            table.Rows.Add(1455, 45789, '닝');
            table.Rows.Add(1456, 45794, '닢');
            table.Rows.Add(1457, 45796, '다');
            table.Rows.Add(1458, 45797, '닥');
            table.Rows.Add(1459, 45798, '닦');
            table.Rows.Add(1460, 45800, '단');
            table.Rows.Add(1461, 45803, '닫');
            table.Rows.Add(1462, 45804, '달');
            table.Rows.Add(1463, 45805, '닭');
            table.Rows.Add(1464, 45806, '닮');
            table.Rows.Add(1465, 45807, '닯');
            table.Rows.Add(1466, 45811, '닳');
            table.Rows.Add(1467, 45812, '담');
            table.Rows.Add(1468, 45813, '답');
            table.Rows.Add(1469, 45815, '닷');
            table.Rows.Add(1470, 45816, '닸');
            table.Rows.Add(1471, 45817, '당');
            table.Rows.Add(1472, 45818, '닺');
            table.Rows.Add(1473, 45819, '닻');
            table.Rows.Add(1474, 45823, '닿');
            table.Rows.Add(1475, 45824, '대');
            table.Rows.Add(1476, 45825, '댁');
            table.Rows.Add(1477, 45828, '댄');
            table.Rows.Add(1478, 45832, '댈');
            table.Rows.Add(1479, 45840, '댐');
            table.Rows.Add(1480, 45841, '댑');
            table.Rows.Add(1481, 45843, '댓');
            table.Rows.Add(1482, 45844, '댔');
            table.Rows.Add(1483, 45845, '댕');
            table.Rows.Add(1484, 45852, '댜');
            table.Rows.Add(1485, 45908, '더');
            table.Rows.Add(1486, 45909, '덕');
            table.Rows.Add(1487, 45910, '덖');
            table.Rows.Add(1488, 45912, '던');
            table.Rows.Add(1489, 45915, '덛');
            table.Rows.Add(1490, 45916, '덜');
            table.Rows.Add(1491, 45918, '덞');
            table.Rows.Add(1492, 45919, '덟');
            table.Rows.Add(1493, 45924, '덤');
            table.Rows.Add(1494, 45925, '덥');
            table.Rows.Add(1495, 45927, '덧');
            table.Rows.Add(1496, 45929, '덩');
            table.Rows.Add(1497, 45931, '덫');
            table.Rows.Add(1498, 45934, '덮');
            table.Rows.Add(1499, 45936, '데');
            table.Rows.Add(1500, 45937, '덱');
            table.Rows.Add(1501, 45940, '덴');
            table.Rows.Add(1502, 45944, '델');
            table.Rows.Add(1503, 45952, '뎀');
            table.Rows.Add(1504, 45953, '뎁');
            table.Rows.Add(1505, 45955, '뎃');
            table.Rows.Add(1506, 45956, '뎄');
            table.Rows.Add(1507, 45957, '뎅');
            table.Rows.Add(1508, 45964, '뎌');
            table.Rows.Add(1509, 45968, '뎐');
            table.Rows.Add(1510, 45972, '뎔');
            table.Rows.Add(1511, 45984, '뎠');
            table.Rows.Add(1512, 45985, '뎡');
            table.Rows.Add(1513, 45992, '뎨');
            table.Rows.Add(1514, 45996, '뎬');
            table.Rows.Add(1515, 46020, '도');
            table.Rows.Add(1516, 46021, '독');
            table.Rows.Add(1517, 46024, '돈');
            table.Rows.Add(1518, 46027, '돋');
            table.Rows.Add(1519, 46028, '돌');
            table.Rows.Add(1520, 46030, '돎');
            table.Rows.Add(1521, 46032, '돐');
            table.Rows.Add(1522, 46036, '돔');
            table.Rows.Add(1523, 46037, '돕');
            table.Rows.Add(1524, 46039, '돗');
            table.Rows.Add(1525, 46041, '동');
            table.Rows.Add(1526, 46043, '돛');
            table.Rows.Add(1527, 46045, '돝');
            table.Rows.Add(1528, 46048, '돠');
            table.Rows.Add(1529, 46052, '돤');
            table.Rows.Add(1530, 46056, '돨');
            table.Rows.Add(1531, 46076, '돼');
            table.Rows.Add(1532, 46096, '됐');
            table.Rows.Add(1533, 46104, '되');
            table.Rows.Add(1534, 46108, '된');
            table.Rows.Add(1535, 46112, '될');
            table.Rows.Add(1536, 46120, '됨');
            table.Rows.Add(1537, 46121, '됩');
            table.Rows.Add(1538, 46123, '됫');
            table.Rows.Add(1539, 46132, '됴');
            table.Rows.Add(1540, 46160, '두');
            table.Rows.Add(1541, 46161, '둑');
            table.Rows.Add(1542, 46164, '둔');
            table.Rows.Add(1543, 46168, '둘');
            table.Rows.Add(1544, 46176, '둠');
            table.Rows.Add(1545, 46177, '둡');
            table.Rows.Add(1546, 46179, '둣');
            table.Rows.Add(1547, 46181, '둥');
            table.Rows.Add(1548, 46188, '둬');
            table.Rows.Add(1549, 46208, '뒀');
            table.Rows.Add(1550, 46216, '뒈');
            table.Rows.Add(1551, 46237, '뒝');
            table.Rows.Add(1552, 46244, '뒤');
            table.Rows.Add(1553, 46248, '뒨');
            table.Rows.Add(1554, 46252, '뒬');
            table.Rows.Add(1555, 46261, '뒵');
            table.Rows.Add(1556, 46263, '뒷');
            table.Rows.Add(1557, 46265, '뒹');
            table.Rows.Add(1558, 46272, '듀');
            table.Rows.Add(1559, 46276, '듄');
            table.Rows.Add(1560, 46280, '듈');
            table.Rows.Add(1561, 46288, '듐');
            table.Rows.Add(1562, 46293, '듕');
            table.Rows.Add(1563, 46300, '드');
            table.Rows.Add(1564, 46301, '득');
            table.Rows.Add(1565, 46304, '든');
            table.Rows.Add(1566, 46307, '듣');
            table.Rows.Add(1567, 46308, '들');
            table.Rows.Add(1568, 46310, '듦');
            table.Rows.Add(1569, 46316, '듬');
            table.Rows.Add(1570, 46317, '듭');
            table.Rows.Add(1571, 46319, '듯');
            table.Rows.Add(1572, 46321, '등');
            table.Rows.Add(1573, 46328, '듸');
            table.Rows.Add(1574, 46356, '디');
            table.Rows.Add(1575, 46357, '딕');
            table.Rows.Add(1576, 46360, '딘');
            table.Rows.Add(1577, 46363, '딛');
            table.Rows.Add(1578, 46364, '딜');
            table.Rows.Add(1579, 46372, '딤');
            table.Rows.Add(1580, 46373, '딥');
            table.Rows.Add(1581, 46375, '딧');
            table.Rows.Add(1582, 46376, '딨');
            table.Rows.Add(1583, 46377, '딩');
            table.Rows.Add(1584, 46378, '딪');
            table.Rows.Add(1585, 46384, '따');
            table.Rows.Add(1586, 46385, '딱');
            table.Rows.Add(1587, 46388, '딴');
            table.Rows.Add(1588, 46392, '딸');
            table.Rows.Add(1589, 46400, '땀');
            table.Rows.Add(1590, 46401, '땁');
            table.Rows.Add(1591, 46403, '땃');
            table.Rows.Add(1592, 46404, '땄');
            table.Rows.Add(1593, 46405, '땅');
            table.Rows.Add(1594, 46411, '땋');
            table.Rows.Add(1595, 46412, '때');
            table.Rows.Add(1596, 46413, '땍');
            table.Rows.Add(1597, 46416, '땐');
            table.Rows.Add(1598, 46420, '땔');
            table.Rows.Add(1599, 46428, '땜');
            table.Rows.Add(1600, 46429, '땝');
            table.Rows.Add(1601, 46431, '땟');
            table.Rows.Add(1602, 46432, '땠');
            table.Rows.Add(1603, 46433, '땡');
            table.Rows.Add(1604, 46496, '떠');
            table.Rows.Add(1605, 46497, '떡');
            table.Rows.Add(1606, 46500, '떤');
            table.Rows.Add(1607, 46504, '떨');
            table.Rows.Add(1608, 46506, '떪');
            table.Rows.Add(1609, 46507, '떫');
            table.Rows.Add(1610, 46512, '떰');
            table.Rows.Add(1611, 46513, '떱');
            table.Rows.Add(1612, 46515, '떳');
            table.Rows.Add(1613, 46516, '떴');
            table.Rows.Add(1614, 46517, '떵');
            table.Rows.Add(1615, 46523, '떻');
            table.Rows.Add(1616, 46524, '떼');
            table.Rows.Add(1617, 46525, '떽');
            table.Rows.Add(1618, 46528, '뗀');
            table.Rows.Add(1619, 46532, '뗄');
            table.Rows.Add(1620, 46540, '뗌');
            table.Rows.Add(1621, 46541, '뗍');
            table.Rows.Add(1622, 46543, '뗏');
            table.Rows.Add(1623, 46544, '뗐');
            table.Rows.Add(1624, 46545, '뗑');
            table.Rows.Add(1625, 46552, '뗘');
            table.Rows.Add(1626, 46572, '뗬');
            table.Rows.Add(1627, 46608, '또');
            table.Rows.Add(1628, 46609, '똑');
            table.Rows.Add(1629, 46612, '똔');
            table.Rows.Add(1630, 46616, '똘');
            table.Rows.Add(1631, 46629, '똥');
            table.Rows.Add(1632, 46636, '똬');
            table.Rows.Add(1633, 46644, '똴');
            table.Rows.Add(1634, 46664, '뙈');
            table.Rows.Add(1635, 46692, '뙤');
            table.Rows.Add(1636, 46696, '뙨');
            table.Rows.Add(1637, 46748, '뚜');
            table.Rows.Add(1638, 46749, '뚝');
            table.Rows.Add(1639, 46752, '뚠');
            table.Rows.Add(1640, 46756, '뚤');
            table.Rows.Add(1641, 46763, '뚫');
            table.Rows.Add(1642, 46764, '뚬');
            table.Rows.Add(1643, 46769, '뚱');
            table.Rows.Add(1644, 46804, '뛔');
            table.Rows.Add(1645, 46832, '뛰');
            table.Rows.Add(1646, 46836, '뛴');
            table.Rows.Add(1647, 46840, '뛸');
            table.Rows.Add(1648, 46848, '뜀');
            table.Rows.Add(1649, 46849, '뜁');
            table.Rows.Add(1650, 46853, '뜅');
            table.Rows.Add(1651, 46888, '뜨');
            table.Rows.Add(1652, 46889, '뜩');
            table.Rows.Add(1653, 46892, '뜬');
            table.Rows.Add(1654, 46895, '뜯');
            table.Rows.Add(1655, 46896, '뜰');
            table.Rows.Add(1656, 46904, '뜸');
            table.Rows.Add(1657, 46905, '뜹');
            table.Rows.Add(1658, 46907, '뜻');
            table.Rows.Add(1659, 46916, '띄');
            table.Rows.Add(1660, 46920, '띈');
            table.Rows.Add(1661, 46924, '띌');
            table.Rows.Add(1662, 46932, '띔');
            table.Rows.Add(1663, 46933, '띕');
            table.Rows.Add(1664, 46944, '띠');
            table.Rows.Add(1665, 46948, '띤');
            table.Rows.Add(1666, 46952, '띨');
            table.Rows.Add(1667, 46960, '띰');
            table.Rows.Add(1668, 46961, '띱');
            table.Rows.Add(1669, 46963, '띳');
            table.Rows.Add(1670, 46965, '띵');
            table.Rows.Add(1671, 46972, '라');
            table.Rows.Add(1672, 46973, '락');
            table.Rows.Add(1673, 46976, '란');
            table.Rows.Add(1674, 46980, '랄');
            table.Rows.Add(1675, 46988, '람');
            table.Rows.Add(1676, 46989, '랍');
            table.Rows.Add(1677, 46991, '랏');
            table.Rows.Add(1678, 46992, '랐');
            table.Rows.Add(1679, 46993, '랑');
            table.Rows.Add(1680, 46994, '랒');
            table.Rows.Add(1681, 46998, '랖');
            table.Rows.Add(1682, 46999, '랗');
            table.Rows.Add(1683, 47000, '래');
            table.Rows.Add(1684, 47001, '랙');
            table.Rows.Add(1685, 47004, '랜');
            table.Rows.Add(1686, 47008, '랠');
            table.Rows.Add(1687, 47016, '램');
            table.Rows.Add(1688, 47017, '랩');
            table.Rows.Add(1689, 47019, '랫');
            table.Rows.Add(1690, 47020, '랬');
            table.Rows.Add(1691, 47021, '랭');
            table.Rows.Add(1692, 47028, '랴');
            table.Rows.Add(1693, 47029, '략');
            table.Rows.Add(1694, 47032, '랸');
            table.Rows.Add(1695, 47047, '럇');
            table.Rows.Add(1696, 47049, '량');
            table.Rows.Add(1697, 47084, '러');
            table.Rows.Add(1698, 47085, '럭');
            table.Rows.Add(1699, 47088, '런');
            table.Rows.Add(1700, 47092, '럴');
            table.Rows.Add(1701, 47100, '럼');
            table.Rows.Add(1702, 47101, '럽');
            table.Rows.Add(1703, 47103, '럿');
            table.Rows.Add(1704, 47104, '렀');
            table.Rows.Add(1705, 47105, '렁');
            table.Rows.Add(1706, 47111, '렇');
            table.Rows.Add(1707, 47112, '레');
            table.Rows.Add(1708, 47113, '렉');
            table.Rows.Add(1709, 47116, '렌');
            table.Rows.Add(1710, 47120, '렐');
            table.Rows.Add(1711, 47128, '렘');
            table.Rows.Add(1712, 47129, '렙');
            table.Rows.Add(1713, 47131, '렛');
            table.Rows.Add(1714, 47133, '렝');
            table.Rows.Add(1715, 47140, '려');
            table.Rows.Add(1716, 47141, '력');
            table.Rows.Add(1717, 47144, '련');
            table.Rows.Add(1718, 47148, '렬');
            table.Rows.Add(1719, 47156, '렴');
            table.Rows.Add(1720, 47157, '렵');
            table.Rows.Add(1721, 47159, '렷');
            table.Rows.Add(1722, 47160, '렸');
            table.Rows.Add(1723, 47161, '령');
            table.Rows.Add(1724, 47168, '례');
            table.Rows.Add(1725, 47172, '롄');
            table.Rows.Add(1726, 47185, '롑');
            table.Rows.Add(1727, 47187, '롓');
            table.Rows.Add(1728, 47196, '로');
            table.Rows.Add(1729, 47197, '록');
            table.Rows.Add(1730, 47200, '론');
            table.Rows.Add(1731, 47204, '롤');
            table.Rows.Add(1732, 47212, '롬');
            table.Rows.Add(1733, 47213, '롭');
            table.Rows.Add(1734, 47215, '롯');
            table.Rows.Add(1735, 47217, '롱');
            table.Rows.Add(1736, 47224, '롸');
            table.Rows.Add(1737, 47228, '롼');
            table.Rows.Add(1738, 47245, '뢍');
            table.Rows.Add(1739, 47272, '뢨');
            table.Rows.Add(1740, 47280, '뢰');
            table.Rows.Add(1741, 47284, '뢴');
            table.Rows.Add(1742, 47288, '뢸');
            table.Rows.Add(1743, 47296, '룀');
            table.Rows.Add(1744, 47297, '룁');
            table.Rows.Add(1745, 47299, '룃');
            table.Rows.Add(1746, 47301, '룅');
            table.Rows.Add(1747, 47308, '료');
            table.Rows.Add(1748, 47312, '룐');
            table.Rows.Add(1749, 47316, '룔');
            table.Rows.Add(1750, 47325, '룝');
            table.Rows.Add(1751, 47327, '룟');
            table.Rows.Add(1752, 47329, '룡');
            table.Rows.Add(1753, 47336, '루');
            table.Rows.Add(1754, 47337, '룩');
            table.Rows.Add(1755, 47340, '룬');
            table.Rows.Add(1756, 47344, '룰');
            table.Rows.Add(1757, 47352, '룸');
            table.Rows.Add(1758, 47353, '룹');
            table.Rows.Add(1759, 47355, '룻');
            table.Rows.Add(1760, 47357, '룽');
            table.Rows.Add(1761, 47364, '뤄');
            table.Rows.Add(1762, 47384, '뤘');
            table.Rows.Add(1763, 47392, '뤠');
            table.Rows.Add(1764, 47420, '뤼');
            table.Rows.Add(1765, 47421, '뤽');
            table.Rows.Add(1766, 47424, '륀');
            table.Rows.Add(1767, 47428, '륄');
            table.Rows.Add(1768, 47436, '륌');
            table.Rows.Add(1769, 47439, '륏');
            table.Rows.Add(1770, 47441, '륑');
            table.Rows.Add(1771, 47448, '류');
            table.Rows.Add(1772, 47449, '륙');
            table.Rows.Add(1773, 47452, '륜');
            table.Rows.Add(1774, 47456, '률');
            table.Rows.Add(1775, 47464, '륨');
            table.Rows.Add(1776, 47465, '륩');
            table.Rows.Add(1777, 47467, '륫');
            table.Rows.Add(1778, 47469, '륭');
            table.Rows.Add(1779, 47476, '르');
            table.Rows.Add(1780, 47477, '륵');
            table.Rows.Add(1781, 47480, '른');
            table.Rows.Add(1782, 47484, '를');
            table.Rows.Add(1783, 47492, '름');
            table.Rows.Add(1784, 47493, '릅');
            table.Rows.Add(1785, 47495, '릇');
            table.Rows.Add(1786, 47497, '릉');
            table.Rows.Add(1787, 47498, '릊');
            table.Rows.Add(1788, 47501, '릍');
            table.Rows.Add(1789, 47502, '릎');
            table.Rows.Add(1790, 47532, '리');
            table.Rows.Add(1791, 47533, '릭');
            table.Rows.Add(1792, 47536, '린');
            table.Rows.Add(1793, 47540, '릴');
            table.Rows.Add(1794, 47548, '림');
            table.Rows.Add(1795, 47549, '립');
            table.Rows.Add(1796, 47551, '릿');
            table.Rows.Add(1797, 47553, '링');
            table.Rows.Add(1798, 47560, '마');
            table.Rows.Add(1799, 47561, '막');
            table.Rows.Add(1800, 47564, '만');
            table.Rows.Add(1801, 47566, '많');
            table.Rows.Add(1802, 47567, '맏');
            table.Rows.Add(1803, 47568, '말');
            table.Rows.Add(1804, 47569, '맑');
            table.Rows.Add(1805, 47570, '맒');
            table.Rows.Add(1806, 47576, '맘');
            table.Rows.Add(1807, 47577, '맙');
            table.Rows.Add(1808, 47579, '맛');
            table.Rows.Add(1809, 47581, '망');
            table.Rows.Add(1810, 47582, '맞');
            table.Rows.Add(1811, 47585, '맡');
            table.Rows.Add(1812, 47587, '맣');
            table.Rows.Add(1813, 47588, '매');
            table.Rows.Add(1814, 47589, '맥');
            table.Rows.Add(1815, 47592, '맨');
            table.Rows.Add(1816, 47596, '맬');
            table.Rows.Add(1817, 47604, '맴');
            table.Rows.Add(1818, 47605, '맵');
            table.Rows.Add(1819, 47607, '맷');
            table.Rows.Add(1820, 47608, '맸');
            table.Rows.Add(1821, 47609, '맹');
            table.Rows.Add(1822, 47610, '맺');
            table.Rows.Add(1823, 47616, '먀');
            table.Rows.Add(1824, 47617, '먁');
            table.Rows.Add(1825, 47624, '먈');
            table.Rows.Add(1826, 47637, '먕');
            table.Rows.Add(1827, 47672, '머');
            table.Rows.Add(1828, 47673, '먹');
            table.Rows.Add(1829, 47676, '먼');
            table.Rows.Add(1830, 47680, '멀');
            table.Rows.Add(1831, 47682, '멂');
            table.Rows.Add(1832, 47688, '멈');
            table.Rows.Add(1833, 47689, '멉');
            table.Rows.Add(1834, 47691, '멋');
            table.Rows.Add(1835, 47693, '멍');
            table.Rows.Add(1836, 47694, '멎');
            table.Rows.Add(1837, 47699, '멓');
            table.Rows.Add(1838, 47700, '메');
            table.Rows.Add(1839, 47701, '멕');
            table.Rows.Add(1840, 47704, '멘');
            table.Rows.Add(1841, 47708, '멜');
            table.Rows.Add(1842, 47716, '멤');
            table.Rows.Add(1843, 47717, '멥');
            table.Rows.Add(1844, 47719, '멧');
            table.Rows.Add(1845, 47720, '멨');
            table.Rows.Add(1846, 47721, '멩');
            table.Rows.Add(1847, 47728, '며');
            table.Rows.Add(1848, 47729, '멱');
            table.Rows.Add(1849, 47732, '면');
            table.Rows.Add(1850, 47736, '멸');
            table.Rows.Add(1851, 47747, '몃');
            table.Rows.Add(1852, 47748, '몄');
            table.Rows.Add(1853, 47749, '명');
            table.Rows.Add(1854, 47751, '몇');
            table.Rows.Add(1855, 47756, '몌');
            table.Rows.Add(1856, 47784, '모');
            table.Rows.Add(1857, 47785, '목');
            table.Rows.Add(1858, 47787, '몫');
            table.Rows.Add(1859, 47788, '몬');
            table.Rows.Add(1860, 47792, '몰');
            table.Rows.Add(1861, 47794, '몲');
            table.Rows.Add(1862, 47800, '몸');
            table.Rows.Add(1863, 47801, '몹');
            table.Rows.Add(1864, 47803, '못');
            table.Rows.Add(1865, 47805, '몽');
            table.Rows.Add(1866, 47812, '뫄');
            table.Rows.Add(1867, 47816, '뫈');
            table.Rows.Add(1868, 47832, '뫘');
            table.Rows.Add(1869, 47833, '뫙');
            table.Rows.Add(1870, 47868, '뫼');
            table.Rows.Add(1871, 47872, '묀');
            table.Rows.Add(1872, 47876, '묄');
            table.Rows.Add(1873, 47885, '묍');
            table.Rows.Add(1874, 47887, '묏');
            table.Rows.Add(1875, 47889, '묑');
            table.Rows.Add(1876, 47896, '묘');
            table.Rows.Add(1877, 47900, '묜');
            table.Rows.Add(1878, 47904, '묠');
            table.Rows.Add(1879, 47913, '묩');
            table.Rows.Add(1880, 47915, '묫');
            table.Rows.Add(1881, 47924, '무');
            table.Rows.Add(1882, 47925, '묵');
            table.Rows.Add(1883, 47926, '묶');
            table.Rows.Add(1884, 47928, '문');
            table.Rows.Add(1885, 47931, '묻');
            table.Rows.Add(1886, 47932, '물');
            table.Rows.Add(1887, 47933, '묽');
            table.Rows.Add(1888, 47934, '묾');
            table.Rows.Add(1889, 47940, '뭄');
            table.Rows.Add(1890, 47941, '뭅');
            table.Rows.Add(1891, 47943, '뭇');
            table.Rows.Add(1892, 47945, '뭉');
            table.Rows.Add(1893, 47949, '뭍');
            table.Rows.Add(1894, 47951, '뭏');
            table.Rows.Add(1895, 47952, '뭐');
            table.Rows.Add(1896, 47956, '뭔');
            table.Rows.Add(1897, 47960, '뭘');
            table.Rows.Add(1898, 47969, '뭡');
            table.Rows.Add(1899, 47971, '뭣');
            table.Rows.Add(1900, 47980, '뭬');
            table.Rows.Add(1901, 48008, '뮈');
            table.Rows.Add(1902, 48012, '뮌');
            table.Rows.Add(1903, 48016, '뮐');
            table.Rows.Add(1904, 48036, '뮤');
            table.Rows.Add(1905, 48040, '뮨');
            table.Rows.Add(1906, 48044, '뮬');
            table.Rows.Add(1907, 48052, '뮴');
            table.Rows.Add(1908, 48055, '뮷');
            table.Rows.Add(1909, 48064, '므');
            table.Rows.Add(1910, 48068, '믄');
            table.Rows.Add(1911, 48072, '믈');
            table.Rows.Add(1912, 48080, '믐');
            table.Rows.Add(1913, 48083, '믓');
            table.Rows.Add(1914, 48120, '미');
            table.Rows.Add(1915, 48121, '믹');
            table.Rows.Add(1916, 48124, '민');
            table.Rows.Add(1917, 48127, '믿');
            table.Rows.Add(1918, 48128, '밀');
            table.Rows.Add(1919, 48130, '밂');
            table.Rows.Add(1920, 48136, '밈');
            table.Rows.Add(1921, 48137, '밉');
            table.Rows.Add(1922, 48139, '밋');
            table.Rows.Add(1923, 48140, '밌');
            table.Rows.Add(1924, 48141, '밍');
            table.Rows.Add(1925, 48143, '및');
            table.Rows.Add(1926, 48145, '밑');
            table.Rows.Add(1927, 48148, '바');
            table.Rows.Add(1928, 48149, '박');
            table.Rows.Add(1929, 48150, '밖');
            table.Rows.Add(1930, 48151, '밗');
            table.Rows.Add(1931, 48152, '반');
            table.Rows.Add(1932, 48155, '받');
            table.Rows.Add(1933, 48156, '발');
            table.Rows.Add(1934, 48157, '밝');
            table.Rows.Add(1935, 48158, '밞');
            table.Rows.Add(1936, 48159, '밟');
            table.Rows.Add(1937, 48164, '밤');
            table.Rows.Add(1938, 48165, '밥');
            table.Rows.Add(1939, 48167, '밧');
            table.Rows.Add(1940, 48169, '방');
            table.Rows.Add(1941, 48173, '밭');
            table.Rows.Add(1942, 48176, '배');
            table.Rows.Add(1943, 48177, '백');
            table.Rows.Add(1944, 48180, '밴');
            table.Rows.Add(1945, 48184, '밸');
            table.Rows.Add(1946, 48192, '뱀');
            table.Rows.Add(1947, 48193, '뱁');
            table.Rows.Add(1948, 48195, '뱃');
            table.Rows.Add(1949, 48196, '뱄');
            table.Rows.Add(1950, 48197, '뱅');
            table.Rows.Add(1951, 48201, '뱉');
            table.Rows.Add(1952, 48204, '뱌');
            table.Rows.Add(1953, 48205, '뱍');
            table.Rows.Add(1954, 48208, '뱐');
            table.Rows.Add(1955, 48221, '뱝');
            table.Rows.Add(1956, 48260, '버');
            table.Rows.Add(1957, 48261, '벅');
            table.Rows.Add(1958, 48264, '번');
            table.Rows.Add(1959, 48267, '벋');
            table.Rows.Add(1960, 48268, '벌');
            table.Rows.Add(1961, 48270, '벎');
            table.Rows.Add(1962, 48276, '범');
            table.Rows.Add(1963, 48277, '법');
            table.Rows.Add(1964, 48279, '벗');
            table.Rows.Add(1965, 48281, '벙');
            table.Rows.Add(1966, 48282, '벚');
            table.Rows.Add(1967, 48288, '베');
            table.Rows.Add(1968, 48289, '벡');
            table.Rows.Add(1969, 48292, '벤');
            table.Rows.Add(1970, 48295, '벧');
            table.Rows.Add(1971, 48296, '벨');
            table.Rows.Add(1972, 48304, '벰');
            table.Rows.Add(1973, 48305, '벱');
            table.Rows.Add(1974, 48307, '벳');
            table.Rows.Add(1975, 48308, '벴');
            table.Rows.Add(1976, 48309, '벵');
            table.Rows.Add(1977, 48316, '벼');
            table.Rows.Add(1978, 48317, '벽');
            table.Rows.Add(1979, 48320, '변');
            table.Rows.Add(1980, 48324, '별');
            table.Rows.Add(1981, 48333, '볍');
            table.Rows.Add(1982, 48335, '볏');
            table.Rows.Add(1983, 48336, '볐');
            table.Rows.Add(1984, 48337, '병');
            table.Rows.Add(1985, 48341, '볕');
            table.Rows.Add(1986, 48344, '볘');
            table.Rows.Add(1987, 48348, '볜');
            table.Rows.Add(1988, 48372, '보');
            table.Rows.Add(1989, 48373, '복');
            table.Rows.Add(1990, 48374, '볶');
            table.Rows.Add(1991, 48376, '본');
            table.Rows.Add(1992, 48380, '볼');
            table.Rows.Add(1993, 48388, '봄');
            table.Rows.Add(1994, 48389, '봅');
            table.Rows.Add(1995, 48391, '봇');
            table.Rows.Add(1996, 48393, '봉');
            table.Rows.Add(1997, 48400, '봐');
            table.Rows.Add(1998, 48404, '봔');
            table.Rows.Add(1999, 48420, '봤');
            table.Rows.Add(2000, 48428, '봬');
            table.Rows.Add(2001, 48448, '뵀');
            table.Rows.Add(2002, 48456, '뵈');
            table.Rows.Add(2003, 48457, '뵉');
            table.Rows.Add(2004, 48460, '뵌');
            table.Rows.Add(2005, 48464, '뵐');
            table.Rows.Add(2006, 48472, '뵘');
            table.Rows.Add(2007, 48473, '뵙');
            table.Rows.Add(2008, 48484, '뵤');
            table.Rows.Add(2009, 48488, '뵨');
            table.Rows.Add(2010, 48512, '부');
            table.Rows.Add(2011, 48513, '북');
            table.Rows.Add(2012, 48516, '분');
            table.Rows.Add(2013, 48519, '붇');
            table.Rows.Add(2014, 48520, '불');
            table.Rows.Add(2015, 48521, '붉');
            table.Rows.Add(2016, 48522, '붊');
            table.Rows.Add(2017, 48528, '붐');
            table.Rows.Add(2018, 48529, '붑');
            table.Rows.Add(2019, 48531, '붓');
            table.Rows.Add(2020, 48533, '붕');
            table.Rows.Add(2021, 48537, '붙');
            table.Rows.Add(2022, 48538, '붚');
            table.Rows.Add(2023, 48540, '붜');
            table.Rows.Add(2024, 48548, '붤');
            table.Rows.Add(2025, 48560, '붰');
            table.Rows.Add(2026, 48568, '붸');
            table.Rows.Add(2027, 48596, '뷔');
            table.Rows.Add(2028, 48597, '뷕');
            table.Rows.Add(2029, 48600, '뷘');
            table.Rows.Add(2030, 48604, '뷜');
            table.Rows.Add(2031, 48617, '뷩');
            table.Rows.Add(2032, 48624, '뷰');
            table.Rows.Add(2033, 48628, '뷴');
            table.Rows.Add(2034, 48632, '뷸');
            table.Rows.Add(2035, 48640, '븀');
            table.Rows.Add(2036, 48643, '븃');
            table.Rows.Add(2037, 48645, '븅');
            table.Rows.Add(2038, 48652, '브');
            table.Rows.Add(2039, 48653, '븍');
            table.Rows.Add(2040, 48656, '븐');
            table.Rows.Add(2041, 48660, '블');
            table.Rows.Add(2042, 48668, '븜');
            table.Rows.Add(2043, 48669, '븝');
            table.Rows.Add(2044, 48671, '븟');
            table.Rows.Add(2045, 48708, '비');
            table.Rows.Add(2046, 48709, '빅');
            table.Rows.Add(2047, 48712, '빈');
            table.Rows.Add(2048, 48716, '빌');
            table.Rows.Add(2049, 48718, '빎');
            table.Rows.Add(2050, 48724, '빔');
            table.Rows.Add(2051, 48725, '빕');
            table.Rows.Add(2052, 48727, '빗');
            table.Rows.Add(2053, 48729, '빙');
            table.Rows.Add(2054, 48730, '빚');
            table.Rows.Add(2055, 48731, '빛');
            table.Rows.Add(2056, 48736, '빠');
            table.Rows.Add(2057, 48737, '빡');
            table.Rows.Add(2058, 48740, '빤');
            table.Rows.Add(2059, 48744, '빨');
            table.Rows.Add(2060, 48746, '빪');
            table.Rows.Add(2061, 48752, '빰');
            table.Rows.Add(2062, 48753, '빱');
            table.Rows.Add(2063, 48755, '빳');
            table.Rows.Add(2064, 48756, '빴');
            table.Rows.Add(2065, 48757, '빵');
            table.Rows.Add(2066, 48763, '빻');
            table.Rows.Add(2067, 48764, '빼');
            table.Rows.Add(2068, 48765, '빽');
            table.Rows.Add(2069, 48768, '뺀');
            table.Rows.Add(2070, 48772, '뺄');
            table.Rows.Add(2071, 48780, '뺌');
            table.Rows.Add(2072, 48781, '뺍');
            table.Rows.Add(2073, 48783, '뺏');
            table.Rows.Add(2074, 48784, '뺐');
            table.Rows.Add(2075, 48785, '뺑');
            table.Rows.Add(2076, 48792, '뺘');
            table.Rows.Add(2077, 48793, '뺙');
            table.Rows.Add(2078, 48808, '뺨');
            table.Rows.Add(2079, 48848, '뻐');
            table.Rows.Add(2080, 48849, '뻑');
            table.Rows.Add(2081, 48852, '뻔');
            table.Rows.Add(2082, 48855, '뻗');
            table.Rows.Add(2083, 48856, '뻘');
            table.Rows.Add(2084, 48864, '뻠');
            table.Rows.Add(2085, 48867, '뻣');
            table.Rows.Add(2086, 48868, '뻤');
            table.Rows.Add(2087, 48869, '뻥');
            table.Rows.Add(2088, 48876, '뻬');
            table.Rows.Add(2089, 48897, '뼁');
            table.Rows.Add(2090, 48904, '뼈');
            table.Rows.Add(2091, 48905, '뼉');
            table.Rows.Add(2092, 48920, '뼘');
            table.Rows.Add(2093, 48921, '뼙');
            table.Rows.Add(2094, 48923, '뼛');
            table.Rows.Add(2095, 48924, '뼜');
            table.Rows.Add(2096, 48925, '뼝');
            table.Rows.Add(2097, 48960, '뽀');
            table.Rows.Add(2098, 48961, '뽁');
            table.Rows.Add(2099, 48964, '뽄');
            table.Rows.Add(2100, 48968, '뽈');
            table.Rows.Add(2101, 48976, '뽐');
            table.Rows.Add(2102, 48977, '뽑');
            table.Rows.Add(2103, 48981, '뽕');
            table.Rows.Add(2104, 49044, '뾔');
            table.Rows.Add(2105, 49072, '뾰');
            table.Rows.Add(2106, 49093, '뿅');
            table.Rows.Add(2107, 49100, '뿌');
            table.Rows.Add(2108, 49101, '뿍');
            table.Rows.Add(2109, 49104, '뿐');
            table.Rows.Add(2110, 49108, '뿔');
            table.Rows.Add(2111, 49116, '뿜');
            table.Rows.Add(2112, 49119, '뿟');
            table.Rows.Add(2113, 49121, '뿡');
            table.Rows.Add(2114, 49212, '쀼');
            table.Rows.Add(2115, 49233, '쁑');
            table.Rows.Add(2116, 49240, '쁘');
            table.Rows.Add(2117, 49244, '쁜');
            table.Rows.Add(2118, 49248, '쁠');
            table.Rows.Add(2119, 49256, '쁨');
            table.Rows.Add(2120, 49257, '쁩');
            table.Rows.Add(2121, 49296, '삐');
            table.Rows.Add(2122, 49297, '삑');
            table.Rows.Add(2123, 49300, '삔');
            table.Rows.Add(2124, 49304, '삘');
            table.Rows.Add(2125, 49312, '삠');
            table.Rows.Add(2126, 49313, '삡');
            table.Rows.Add(2127, 49315, '삣');
            table.Rows.Add(2128, 49317, '삥');
            table.Rows.Add(2129, 49324, '사');
            table.Rows.Add(2130, 49325, '삭');
            table.Rows.Add(2131, 49327, '삯');
            table.Rows.Add(2132, 49328, '산');
            table.Rows.Add(2133, 49331, '삳');
            table.Rows.Add(2134, 49332, '살');
            table.Rows.Add(2135, 49333, '삵');
            table.Rows.Add(2136, 49334, '삶');
            table.Rows.Add(2137, 49340, '삼');
            table.Rows.Add(2138, 49341, '삽');
            table.Rows.Add(2139, 49343, '삿');
            table.Rows.Add(2140, 49344, '샀');
            table.Rows.Add(2141, 49345, '상');
            table.Rows.Add(2142, 49349, '샅');
            table.Rows.Add(2143, 49352, '새');
            table.Rows.Add(2144, 49353, '색');
            table.Rows.Add(2145, 49356, '샌');
            table.Rows.Add(2146, 49360, '샐');
            table.Rows.Add(2147, 49368, '샘');
            table.Rows.Add(2148, 49369, '샙');
            table.Rows.Add(2149, 49371, '샛');
            table.Rows.Add(2150, 49372, '샜');
            table.Rows.Add(2151, 49373, '생');
            table.Rows.Add(2152, 49380, '샤');
            table.Rows.Add(2153, 49381, '샥');
            table.Rows.Add(2154, 49384, '샨');
            table.Rows.Add(2155, 49388, '샬');
            table.Rows.Add(2156, 49396, '샴');
            table.Rows.Add(2157, 49397, '샵');
            table.Rows.Add(2158, 49399, '샷');
            table.Rows.Add(2159, 49401, '샹');
            table.Rows.Add(2160, 49408, '섀');
            table.Rows.Add(2161, 49412, '섄');
            table.Rows.Add(2162, 49416, '섈');
            table.Rows.Add(2163, 49424, '섐');
            table.Rows.Add(2164, 49429, '섕');
            table.Rows.Add(2165, 49436, '서');
            table.Rows.Add(2166, 49437, '석');
            table.Rows.Add(2167, 49438, '섞');
            table.Rows.Add(2168, 49439, '섟');
            table.Rows.Add(2169, 49440, '선');
            table.Rows.Add(2170, 49443, '섣');
            table.Rows.Add(2171, 49444, '설');
            table.Rows.Add(2172, 49446, '섦');
            table.Rows.Add(2173, 49447, '섧');
            table.Rows.Add(2174, 49452, '섬');
            table.Rows.Add(2175, 49453, '섭');
            table.Rows.Add(2176, 49455, '섯');
            table.Rows.Add(2177, 49456, '섰');
            table.Rows.Add(2178, 49457, '성');
            table.Rows.Add(2179, 49462, '섶');
            table.Rows.Add(2180, 49464, '세');
            table.Rows.Add(2181, 49465, '섹');
            table.Rows.Add(2182, 49468, '센');
            table.Rows.Add(2183, 49472, '셀');
            table.Rows.Add(2184, 49480, '셈');
            table.Rows.Add(2185, 49481, '셉');
            table.Rows.Add(2186, 49483, '셋');
            table.Rows.Add(2187, 49484, '셌');
            table.Rows.Add(2188, 49485, '셍');
            table.Rows.Add(2189, 49492, '셔');
            table.Rows.Add(2190, 49493, '셕');
            table.Rows.Add(2191, 49496, '션');
            table.Rows.Add(2192, 49500, '셜');
            table.Rows.Add(2193, 49508, '셤');
            table.Rows.Add(2194, 49509, '셥');
            table.Rows.Add(2195, 49511, '셧');
            table.Rows.Add(2196, 49512, '셨');
            table.Rows.Add(2197, 49513, '셩');
            table.Rows.Add(2198, 49520, '셰');
            table.Rows.Add(2199, 49524, '셴');
            table.Rows.Add(2200, 49528, '셸');
            table.Rows.Add(2201, 49541, '솅');
            table.Rows.Add(2202, 49548, '소');
            table.Rows.Add(2203, 49549, '속');
            table.Rows.Add(2204, 49550, '솎');
            table.Rows.Add(2205, 49552, '손');
            table.Rows.Add(2206, 49556, '솔');
            table.Rows.Add(2207, 49558, '솖');
            table.Rows.Add(2208, 49564, '솜');
            table.Rows.Add(2209, 49565, '솝');
            table.Rows.Add(2210, 49567, '솟');
            table.Rows.Add(2211, 49569, '송');
            table.Rows.Add(2212, 49573, '솥');
            table.Rows.Add(2213, 49576, '솨');
            table.Rows.Add(2214, 49577, '솩');
            table.Rows.Add(2215, 49580, '솬');
            table.Rows.Add(2216, 49584, '솰');
            table.Rows.Add(2217, 49597, '솽');
            table.Rows.Add(2218, 49604, '쇄');
            table.Rows.Add(2219, 49608, '쇈');
            table.Rows.Add(2220, 49612, '쇌');
            table.Rows.Add(2221, 49620, '쇔');
            table.Rows.Add(2222, 49623, '쇗');
            table.Rows.Add(2223, 49624, '쇘');
            table.Rows.Add(2224, 49632, '쇠');
            table.Rows.Add(2225, 49636, '쇤');
            table.Rows.Add(2226, 49640, '쇨');
            table.Rows.Add(2227, 49648, '쇰');
            table.Rows.Add(2228, 49649, '쇱');
            table.Rows.Add(2229, 49651, '쇳');
            table.Rows.Add(2230, 49660, '쇼');
            table.Rows.Add(2231, 49661, '쇽');
            table.Rows.Add(2232, 49664, '숀');
            table.Rows.Add(2233, 49668, '숄');
            table.Rows.Add(2234, 49676, '숌');
            table.Rows.Add(2235, 49677, '숍');
            table.Rows.Add(2236, 49679, '숏');
            table.Rows.Add(2237, 49681, '숑');
            table.Rows.Add(2238, 49688, '수');
            table.Rows.Add(2239, 49689, '숙');
            table.Rows.Add(2240, 49692, '순');
            table.Rows.Add(2241, 49695, '숟');
            table.Rows.Add(2242, 49696, '술');
            table.Rows.Add(2243, 49704, '숨');
            table.Rows.Add(2244, 49705, '숩');
            table.Rows.Add(2245, 49707, '숫');
            table.Rows.Add(2246, 49709, '숭');
            table.Rows.Add(2247, 49711, '숯');
            table.Rows.Add(2248, 49713, '숱');
            table.Rows.Add(2249, 49714, '숲');
            table.Rows.Add(2250, 49716, '숴');
            table.Rows.Add(2251, 49736, '쉈');
            table.Rows.Add(2252, 49744, '쉐');
            table.Rows.Add(2253, 49745, '쉑');
            table.Rows.Add(2254, 49748, '쉔');
            table.Rows.Add(2255, 49752, '쉘');
            table.Rows.Add(2256, 49760, '쉠');
            table.Rows.Add(2257, 49765, '쉥');
            table.Rows.Add(2258, 49772, '쉬');
            table.Rows.Add(2259, 49773, '쉭');
            table.Rows.Add(2260, 49776, '쉰');
            table.Rows.Add(2261, 49780, '쉴');
            table.Rows.Add(2262, 49788, '쉼');
            table.Rows.Add(2263, 49789, '쉽');
            table.Rows.Add(2264, 49791, '쉿');
            table.Rows.Add(2265, 49793, '슁');
            table.Rows.Add(2266, 49800, '슈');
            table.Rows.Add(2267, 49801, '슉');
            table.Rows.Add(2268, 49808, '슐');
            table.Rows.Add(2269, 49816, '슘');
            table.Rows.Add(2270, 49819, '슛');
            table.Rows.Add(2271, 49821, '슝');
            table.Rows.Add(2272, 49828, '스');
            table.Rows.Add(2273, 49829, '슥');
            table.Rows.Add(2274, 49832, '슨');
            table.Rows.Add(2275, 49836, '슬');
            table.Rows.Add(2276, 49837, '슭');
            table.Rows.Add(2277, 49844, '슴');
            table.Rows.Add(2278, 49845, '습');
            table.Rows.Add(2279, 49847, '슷');
            table.Rows.Add(2280, 49849, '승');
            table.Rows.Add(2281, 49884, '시');
            table.Rows.Add(2282, 49885, '식');
            table.Rows.Add(2283, 49888, '신');
            table.Rows.Add(2284, 49891, '싣');
            table.Rows.Add(2285, 49892, '실');
            table.Rows.Add(2286, 49899, '싫');
            table.Rows.Add(2287, 49900, '심');
            table.Rows.Add(2288, 49901, '십');
            table.Rows.Add(2289, 49903, '싯');
            table.Rows.Add(2290, 49905, '싱');
            table.Rows.Add(2291, 49910, '싶');
            table.Rows.Add(2292, 49912, '싸');
            table.Rows.Add(2293, 49913, '싹');
            table.Rows.Add(2294, 49915, '싻');
            table.Rows.Add(2295, 49916, '싼');
            table.Rows.Add(2296, 49920, '쌀');
            table.Rows.Add(2297, 49928, '쌈');
            table.Rows.Add(2298, 49929, '쌉');
            table.Rows.Add(2299, 49932, '쌌');
            table.Rows.Add(2300, 49933, '쌍');
            table.Rows.Add(2301, 49939, '쌓');
            table.Rows.Add(2302, 49940, '쌔');
            table.Rows.Add(2303, 49941, '쌕');
            table.Rows.Add(2304, 49944, '쌘');
            table.Rows.Add(2305, 49948, '쌜');
            table.Rows.Add(2306, 49956, '쌤');
            table.Rows.Add(2307, 49957, '쌥');
            table.Rows.Add(2308, 49960, '쌨');
            table.Rows.Add(2309, 49961, '쌩');
            table.Rows.Add(2310, 49989, '썅');
            table.Rows.Add(2311, 50024, '써');
            table.Rows.Add(2312, 50025, '썩');
            table.Rows.Add(2313, 50028, '썬');
            table.Rows.Add(2314, 50032, '썰');
            table.Rows.Add(2315, 50034, '썲');
            table.Rows.Add(2316, 50040, '썸');
            table.Rows.Add(2317, 50041, '썹');
            table.Rows.Add(2318, 50044, '썼');
            table.Rows.Add(2319, 50045, '썽');
            table.Rows.Add(2320, 50052, '쎄');
            table.Rows.Add(2321, 50056, '쎈');
            table.Rows.Add(2322, 50060, '쎌');
            table.Rows.Add(2323, 50112, '쏀');
            table.Rows.Add(2324, 50136, '쏘');
            table.Rows.Add(2325, 50137, '쏙');
            table.Rows.Add(2326, 50140, '쏜');
            table.Rows.Add(2327, 50143, '쏟');
            table.Rows.Add(2328, 50144, '쏠');
            table.Rows.Add(2329, 50146, '쏢');
            table.Rows.Add(2330, 50152, '쏨');
            table.Rows.Add(2331, 50153, '쏩');
            table.Rows.Add(2332, 50157, '쏭');
            table.Rows.Add(2333, 50164, '쏴');
            table.Rows.Add(2334, 50165, '쏵');
            table.Rows.Add(2335, 50168, '쏸');
            table.Rows.Add(2336, 50184, '쐈');
            table.Rows.Add(2337, 50192, '쐐');
            table.Rows.Add(2338, 50212, '쐤');
            table.Rows.Add(2339, 50220, '쐬');
            table.Rows.Add(2340, 50224, '쐰');
            table.Rows.Add(2341, 50228, '쐴');
            table.Rows.Add(2342, 50236, '쐼');
            table.Rows.Add(2343, 50237, '쐽');
            table.Rows.Add(2344, 50248, '쑈');
            table.Rows.Add(2345, 50276, '쑤');
            table.Rows.Add(2346, 50277, '쑥');
            table.Rows.Add(2347, 50280, '쑨');
            table.Rows.Add(2348, 50284, '쑬');
            table.Rows.Add(2349, 50292, '쑴');
            table.Rows.Add(2350, 50293, '쑵');
            table.Rows.Add(2351, 50297, '쑹');
            table.Rows.Add(2352, 50304, '쒀');
            table.Rows.Add(2353, 50324, '쒔');
            table.Rows.Add(2354, 50332, '쒜');
            table.Rows.Add(2355, 50360, '쒸');
            table.Rows.Add(2356, 50364, '쒼');
            table.Rows.Add(2357, 50409, '쓩');
            table.Rows.Add(2358, 50416, '쓰');
            table.Rows.Add(2359, 50417, '쓱');
            table.Rows.Add(2360, 50420, '쓴');
            table.Rows.Add(2361, 50424, '쓸');
            table.Rows.Add(2362, 50426, '쓺');
            table.Rows.Add(2363, 50431, '쓿');
            table.Rows.Add(2364, 50432, '씀');
            table.Rows.Add(2365, 50433, '씁');
            table.Rows.Add(2366, 50444, '씌');
            table.Rows.Add(2367, 50448, '씐');
            table.Rows.Add(2368, 50452, '씔');
            table.Rows.Add(2369, 50460, '씜');
            table.Rows.Add(2370, 50472, '씨');
            table.Rows.Add(2371, 50473, '씩');
            table.Rows.Add(2372, 50476, '씬');
            table.Rows.Add(2373, 50480, '씰');
            table.Rows.Add(2374, 50488, '씸');
            table.Rows.Add(2375, 50489, '씹');
            table.Rows.Add(2376, 50491, '씻');
            table.Rows.Add(2377, 50493, '씽');
            table.Rows.Add(2378, 50500, '아');
            table.Rows.Add(2379, 50501, '악');
            table.Rows.Add(2380, 50504, '안');
            table.Rows.Add(2381, 50505, '앉');
            table.Rows.Add(2382, 50506, '않');
            table.Rows.Add(2383, 50508, '알');
            table.Rows.Add(2384, 50509, '앍');
            table.Rows.Add(2385, 50510, '앎');
            table.Rows.Add(2386, 50515, '앓');
            table.Rows.Add(2387, 50516, '암');
            table.Rows.Add(2388, 50517, '압');
            table.Rows.Add(2389, 50519, '앗');
            table.Rows.Add(2390, 50520, '았');
            table.Rows.Add(2391, 50521, '앙');
            table.Rows.Add(2392, 50525, '앝');
            table.Rows.Add(2393, 50526, '앞');
            table.Rows.Add(2394, 50528, '애');
            table.Rows.Add(2395, 50529, '액');
            table.Rows.Add(2396, 50532, '앤');
            table.Rows.Add(2397, 50536, '앨');
            table.Rows.Add(2398, 50544, '앰');
            table.Rows.Add(2399, 50545, '앱');
            table.Rows.Add(2400, 50547, '앳');
            table.Rows.Add(2401, 50548, '앴');
            table.Rows.Add(2402, 50549, '앵');
            table.Rows.Add(2403, 50556, '야');
            table.Rows.Add(2404, 50557, '약');
            table.Rows.Add(2405, 50560, '얀');
            table.Rows.Add(2406, 50564, '얄');
            table.Rows.Add(2407, 50567, '얇');
            table.Rows.Add(2408, 50572, '얌');
            table.Rows.Add(2409, 50573, '얍');
            table.Rows.Add(2410, 50575, '얏');
            table.Rows.Add(2411, 50577, '양');
            table.Rows.Add(2412, 50581, '얕');
            table.Rows.Add(2413, 50583, '얗');
            table.Rows.Add(2414, 50584, '얘');
            table.Rows.Add(2415, 50588, '얜');
            table.Rows.Add(2416, 50592, '얠');
            table.Rows.Add(2417, 50601, '얩');
            table.Rows.Add(2418, 50612, '어');
            table.Rows.Add(2419, 50613, '억');
            table.Rows.Add(2420, 50616, '언');
            table.Rows.Add(2421, 50617, '얹');
            table.Rows.Add(2422, 50619, '얻');
            table.Rows.Add(2423, 50620, '얼');
            table.Rows.Add(2424, 50621, '얽');
            table.Rows.Add(2425, 50622, '얾');
            table.Rows.Add(2426, 50628, '엄');
            table.Rows.Add(2427, 50629, '업');
            table.Rows.Add(2428, 50630, '없');
            table.Rows.Add(2429, 50631, '엇');
            table.Rows.Add(2430, 50632, '었');
            table.Rows.Add(2431, 50633, '엉');
            table.Rows.Add(2432, 50634, '엊');
            table.Rows.Add(2433, 50636, '엌');
            table.Rows.Add(2434, 50638, '엎');
            table.Rows.Add(2435, 50640, '에');
            table.Rows.Add(2436, 50641, '엑');
            table.Rows.Add(2437, 50644, '엔');
            table.Rows.Add(2438, 50648, '엘');
            table.Rows.Add(2439, 50656, '엠');
            table.Rows.Add(2440, 50657, '엡');
            table.Rows.Add(2441, 50659, '엣');
            table.Rows.Add(2442, 50661, '엥');
            table.Rows.Add(2443, 50668, '여');
            table.Rows.Add(2444, 50669, '역');
            table.Rows.Add(2445, 50670, '엮');
            table.Rows.Add(2446, 50672, '연');
            table.Rows.Add(2447, 50676, '열');
            table.Rows.Add(2448, 50678, '엶');
            table.Rows.Add(2449, 50679, '엷');
            table.Rows.Add(2450, 50684, '염');
            table.Rows.Add(2451, 50685, '엽');
            table.Rows.Add(2452, 50686, '엾');
            table.Rows.Add(2453, 50687, '엿');
            table.Rows.Add(2454, 50688, '였');
            table.Rows.Add(2455, 50689, '영');
            table.Rows.Add(2456, 50693, '옅');
            table.Rows.Add(2457, 50694, '옆');
            table.Rows.Add(2458, 50695, '옇');
            table.Rows.Add(2459, 50696, '예');
            table.Rows.Add(2460, 50700, '옌');
            table.Rows.Add(2461, 50704, '옐');
            table.Rows.Add(2462, 50712, '옘');
            table.Rows.Add(2463, 50713, '옙');
            table.Rows.Add(2464, 50715, '옛');
            table.Rows.Add(2465, 50716, '옜');
            table.Rows.Add(2466, 50724, '오');
            table.Rows.Add(2467, 50725, '옥');
            table.Rows.Add(2468, 50728, '온');
            table.Rows.Add(2469, 50732, '올');
            table.Rows.Add(2470, 50733, '옭');
            table.Rows.Add(2471, 50734, '옮');
            table.Rows.Add(2472, 50736, '옰');
            table.Rows.Add(2473, 50739, '옳');
            table.Rows.Add(2474, 50740, '옴');
            table.Rows.Add(2475, 50741, '옵');
            table.Rows.Add(2476, 50743, '옷');
            table.Rows.Add(2477, 50745, '옹');
            table.Rows.Add(2478, 50747, '옻');
            table.Rows.Add(2479, 50752, '와');
            table.Rows.Add(2480, 50753, '왁');
            table.Rows.Add(2481, 50756, '완');
            table.Rows.Add(2482, 50760, '왈');
            table.Rows.Add(2483, 50768, '왐');
            table.Rows.Add(2484, 50769, '왑');
            table.Rows.Add(2485, 50771, '왓');
            table.Rows.Add(2486, 50772, '왔');
            table.Rows.Add(2487, 50773, '왕');
            table.Rows.Add(2488, 50780, '왜');
            table.Rows.Add(2489, 50781, '왝');
            table.Rows.Add(2490, 50784, '왠');
            table.Rows.Add(2491, 50796, '왬');
            table.Rows.Add(2492, 50799, '왯');
            table.Rows.Add(2493, 50801, '왱');
            table.Rows.Add(2494, 50808, '외');
            table.Rows.Add(2495, 50809, '왹');
            table.Rows.Add(2496, 50812, '왼');
            table.Rows.Add(2497, 50816, '욀');
            table.Rows.Add(2498, 50824, '욈');
            table.Rows.Add(2499, 50825, '욉');
            table.Rows.Add(2500, 50827, '욋');
            table.Rows.Add(2501, 50829, '욍');
            table.Rows.Add(2502, 50836, '요');
            table.Rows.Add(2503, 50837, '욕');
            table.Rows.Add(2504, 50840, '욘');
            table.Rows.Add(2505, 50844, '욜');
            table.Rows.Add(2506, 50852, '욤');
            table.Rows.Add(2507, 50853, '욥');
            table.Rows.Add(2508, 50855, '욧');
            table.Rows.Add(2509, 50857, '용');
            table.Rows.Add(2510, 50864, '우');
            table.Rows.Add(2511, 50865, '욱');
            table.Rows.Add(2512, 50868, '운');
            table.Rows.Add(2513, 50872, '울');
            table.Rows.Add(2514, 50873, '욹');
            table.Rows.Add(2515, 50874, '욺');
            table.Rows.Add(2516, 50880, '움');
            table.Rows.Add(2517, 50881, '웁');
            table.Rows.Add(2518, 50883, '웃');
            table.Rows.Add(2519, 50885, '웅');
            table.Rows.Add(2520, 50892, '워');
            table.Rows.Add(2521, 50893, '웍');
            table.Rows.Add(2522, 50896, '원');
            table.Rows.Add(2523, 50900, '월');
            table.Rows.Add(2524, 50908, '웜');
            table.Rows.Add(2525, 50909, '웝');
            table.Rows.Add(2526, 50912, '웠');
            table.Rows.Add(2527, 50913, '웡');
            table.Rows.Add(2528, 50920, '웨');
            table.Rows.Add(2529, 50921, '웩');
            table.Rows.Add(2530, 50924, '웬');
            table.Rows.Add(2531, 50928, '웰');
            table.Rows.Add(2532, 50936, '웸');
            table.Rows.Add(2533, 50937, '웹');
            table.Rows.Add(2534, 50941, '웽');
            table.Rows.Add(2535, 50948, '위');
            table.Rows.Add(2536, 50949, '윅');
            table.Rows.Add(2537, 50952, '윈');
            table.Rows.Add(2538, 50956, '윌');
            table.Rows.Add(2539, 50964, '윔');
            table.Rows.Add(2540, 50965, '윕');
            table.Rows.Add(2541, 50967, '윗');
            table.Rows.Add(2542, 50969, '윙');
            table.Rows.Add(2543, 50976, '유');
            table.Rows.Add(2544, 50977, '육');
            table.Rows.Add(2545, 50980, '윤');
            table.Rows.Add(2546, 50984, '율');
            table.Rows.Add(2547, 50992, '윰');
            table.Rows.Add(2548, 50993, '윱');
            table.Rows.Add(2549, 50995, '윳');
            table.Rows.Add(2550, 50997, '융');
            table.Rows.Add(2551, 50999, '윷');
            table.Rows.Add(2552, 51004, '으');
            table.Rows.Add(2553, 51005, '윽');
            table.Rows.Add(2554, 51008, '은');
            table.Rows.Add(2555, 51012, '을');
            table.Rows.Add(2556, 51018, '읊');
            table.Rows.Add(2557, 51020, '음');
            table.Rows.Add(2558, 51021, '읍');
            table.Rows.Add(2559, 51023, '읏');
            table.Rows.Add(2560, 51025, '응');
            table.Rows.Add(2561, 51026, '읒');
            table.Rows.Add(2562, 51027, '읓');
            table.Rows.Add(2563, 51028, '읔');
            table.Rows.Add(2564, 51029, '읕');
            table.Rows.Add(2565, 51030, '읖');
            table.Rows.Add(2566, 51031, '읗');
            table.Rows.Add(2567, 51032, '의');
            table.Rows.Add(2568, 51036, '읜');
            table.Rows.Add(2569, 51040, '읠');
            table.Rows.Add(2570, 51048, '읨');
            table.Rows.Add(2571, 51051, '읫');
            table.Rows.Add(2572, 51060, '이');
            table.Rows.Add(2573, 51061, '익');
            table.Rows.Add(2574, 51064, '인');
            table.Rows.Add(2575, 51068, '일');
            table.Rows.Add(2576, 51069, '읽');
            table.Rows.Add(2577, 51070, '읾');
            table.Rows.Add(2578, 51075, '잃');
            table.Rows.Add(2579, 51076, '임');
            table.Rows.Add(2580, 51077, '입');
            table.Rows.Add(2581, 51079, '잇');
            table.Rows.Add(2582, 51080, '있');
            table.Rows.Add(2583, 51081, '잉');
            table.Rows.Add(2584, 51082, '잊');
            table.Rows.Add(2585, 51086, '잎');
            table.Rows.Add(2586, 51088, '자');
            table.Rows.Add(2587, 51089, '작');
            table.Rows.Add(2588, 51092, '잔');
            table.Rows.Add(2589, 51094, '잖');
            table.Rows.Add(2590, 51095, '잗');
            table.Rows.Add(2591, 51096, '잘');
            table.Rows.Add(2592, 51098, '잚');
            table.Rows.Add(2593, 51104, '잠');
            table.Rows.Add(2594, 51105, '잡');
            table.Rows.Add(2595, 51107, '잣');
            table.Rows.Add(2596, 51108, '잤');
            table.Rows.Add(2597, 51109, '장');
            table.Rows.Add(2598, 51110, '잦');
            table.Rows.Add(2599, 51116, '재');
            table.Rows.Add(2600, 51117, '잭');
            table.Rows.Add(2601, 51120, '잰');
            table.Rows.Add(2602, 51124, '잴');
            table.Rows.Add(2603, 51132, '잼');
            table.Rows.Add(2604, 51133, '잽');
            table.Rows.Add(2605, 51135, '잿');
            table.Rows.Add(2606, 51136, '쟀');
            table.Rows.Add(2607, 51137, '쟁');
            table.Rows.Add(2608, 51144, '쟈');
            table.Rows.Add(2609, 51145, '쟉');
            table.Rows.Add(2610, 51148, '쟌');
            table.Rows.Add(2611, 51150, '쟎');
            table.Rows.Add(2612, 51152, '쟐');
            table.Rows.Add(2613, 51160, '쟘');
            table.Rows.Add(2614, 51165, '쟝');
            table.Rows.Add(2615, 51172, '쟤');
            table.Rows.Add(2616, 51176, '쟨');
            table.Rows.Add(2617, 51180, '쟬');
            table.Rows.Add(2618, 51200, '저');
            table.Rows.Add(2619, 51201, '적');
            table.Rows.Add(2620, 51204, '전');
            table.Rows.Add(2621, 51208, '절');
            table.Rows.Add(2622, 51210, '젊');
            table.Rows.Add(2623, 51216, '점');
            table.Rows.Add(2624, 51217, '접');
            table.Rows.Add(2625, 51219, '젓');
            table.Rows.Add(2626, 51221, '정');
            table.Rows.Add(2627, 51222, '젖');
            table.Rows.Add(2628, 51228, '제');
            table.Rows.Add(2629, 51229, '젝');
            table.Rows.Add(2630, 51232, '젠');
            table.Rows.Add(2631, 51236, '젤');
            table.Rows.Add(2632, 51244, '젬');
            table.Rows.Add(2633, 51245, '젭');
            table.Rows.Add(2634, 51247, '젯');
            table.Rows.Add(2635, 51249, '젱');
            table.Rows.Add(2636, 51256, '져');
            table.Rows.Add(2637, 51260, '젼');
            table.Rows.Add(2638, 51264, '졀');
            table.Rows.Add(2639, 51272, '졈');
            table.Rows.Add(2640, 51273, '졉');
            table.Rows.Add(2641, 51276, '졌');
            table.Rows.Add(2642, 51277, '졍');
            table.Rows.Add(2643, 51284, '졔');
            table.Rows.Add(2644, 51312, '조');
            table.Rows.Add(2645, 51313, '족');
            table.Rows.Add(2646, 51316, '존');
            table.Rows.Add(2647, 51320, '졸');
            table.Rows.Add(2648, 51322, '졺');
            table.Rows.Add(2649, 51328, '좀');
            table.Rows.Add(2650, 51329, '좁');
            table.Rows.Add(2651, 51331, '좃');
            table.Rows.Add(2652, 51333, '종');
            table.Rows.Add(2653, 51334, '좆');
            table.Rows.Add(2654, 51335, '좇');
            table.Rows.Add(2655, 51339, '좋');
            table.Rows.Add(2656, 51340, '좌');
            table.Rows.Add(2657, 51341, '좍');
            table.Rows.Add(2658, 51348, '좔');
            table.Rows.Add(2659, 51357, '좝');
            table.Rows.Add(2660, 51359, '좟');
            table.Rows.Add(2661, 51361, '좡');
            table.Rows.Add(2662, 51368, '좨');
            table.Rows.Add(2663, 51388, '좼');
            table.Rows.Add(2664, 51389, '좽');
            table.Rows.Add(2665, 51396, '죄');
            table.Rows.Add(2666, 51400, '죈');
            table.Rows.Add(2667, 51404, '죌');
            table.Rows.Add(2668, 51412, '죔');
            table.Rows.Add(2669, 51413, '죕');
            table.Rows.Add(2670, 51415, '죗');
            table.Rows.Add(2671, 51417, '죙');
            table.Rows.Add(2672, 51424, '죠');
            table.Rows.Add(2673, 51425, '죡');
            table.Rows.Add(2674, 51428, '죤');
            table.Rows.Add(2675, 51445, '죵');
            table.Rows.Add(2676, 51452, '주');
            table.Rows.Add(2677, 51453, '죽');
            table.Rows.Add(2678, 51456, '준');
            table.Rows.Add(2679, 51460, '줄');
            table.Rows.Add(2680, 51461, '줅');
            table.Rows.Add(2681, 51462, '줆');
            table.Rows.Add(2682, 51468, '줌');
            table.Rows.Add(2683, 51469, '줍');
            table.Rows.Add(2684, 51471, '줏');
            table.Rows.Add(2685, 51473, '중');
            table.Rows.Add(2686, 51480, '줘');
            table.Rows.Add(2687, 51500, '줬');
            table.Rows.Add(2688, 51508, '줴');
            table.Rows.Add(2689, 51536, '쥐');
            table.Rows.Add(2690, 51537, '쥑');
            table.Rows.Add(2691, 51540, '쥔');
            table.Rows.Add(2692, 51544, '쥘');
            table.Rows.Add(2693, 51552, '쥠');
            table.Rows.Add(2694, 51553, '쥡');
            table.Rows.Add(2695, 51555, '쥣');
            table.Rows.Add(2696, 51564, '쥬');
            table.Rows.Add(2697, 51568, '쥰');
            table.Rows.Add(2698, 51572, '쥴');
            table.Rows.Add(2699, 51580, '쥼');
            table.Rows.Add(2700, 51592, '즈');
            table.Rows.Add(2701, 51593, '즉');
            table.Rows.Add(2702, 51596, '즌');
            table.Rows.Add(2703, 51600, '즐');
            table.Rows.Add(2704, 51608, '즘');
            table.Rows.Add(2705, 51609, '즙');
            table.Rows.Add(2706, 51611, '즛');
            table.Rows.Add(2707, 51613, '증');
            table.Rows.Add(2708, 51648, '지');
            table.Rows.Add(2709, 51649, '직');
            table.Rows.Add(2710, 51652, '진');
            table.Rows.Add(2711, 51655, '짇');
            table.Rows.Add(2712, 51656, '질');
            table.Rows.Add(2713, 51658, '짊');
            table.Rows.Add(2714, 51664, '짐');
            table.Rows.Add(2715, 51665, '집');
            table.Rows.Add(2716, 51667, '짓');
            table.Rows.Add(2717, 51669, '징');
            table.Rows.Add(2718, 51670, '짖');
            table.Rows.Add(2719, 51673, '짙');
            table.Rows.Add(2720, 51674, '짚');
            table.Rows.Add(2721, 51676, '짜');
            table.Rows.Add(2722, 51677, '짝');
            table.Rows.Add(2723, 51680, '짠');
            table.Rows.Add(2724, 51682, '짢');
            table.Rows.Add(2725, 51684, '짤');
            table.Rows.Add(2726, 51687, '짧');
            table.Rows.Add(2727, 51692, '짬');
            table.Rows.Add(2728, 51693, '짭');
            table.Rows.Add(2729, 51695, '짯');
            table.Rows.Add(2730, 51696, '짰');
            table.Rows.Add(2731, 51697, '짱');
            table.Rows.Add(2732, 51704, '째');
            table.Rows.Add(2733, 51705, '짹');
            table.Rows.Add(2734, 51708, '짼');
            table.Rows.Add(2735, 51712, '쨀');
            table.Rows.Add(2736, 51720, '쨈');
            table.Rows.Add(2737, 51721, '쨉');
            table.Rows.Add(2738, 51723, '쨋');
            table.Rows.Add(2739, 51724, '쨌');
            table.Rows.Add(2740, 51725, '쨍');
            table.Rows.Add(2741, 51732, '쨔');
            table.Rows.Add(2742, 51736, '쨘');
            table.Rows.Add(2743, 51753, '쨩');
            table.Rows.Add(2744, 51788, '쩌');
            table.Rows.Add(2745, 51789, '쩍');
            table.Rows.Add(2746, 51792, '쩐');
            table.Rows.Add(2747, 51796, '쩔');
            table.Rows.Add(2748, 51804, '쩜');
            table.Rows.Add(2749, 51805, '쩝');
            table.Rows.Add(2750, 51807, '쩟');
            table.Rows.Add(2751, 51808, '쩠');
            table.Rows.Add(2752, 51809, '쩡');
            table.Rows.Add(2753, 51816, '쩨');
            table.Rows.Add(2754, 51837, '쩽');
            table.Rows.Add(2755, 51844, '쪄');
            table.Rows.Add(2756, 51864, '쪘');
            table.Rows.Add(2757, 51900, '쪼');
            table.Rows.Add(2758, 51901, '쪽');
            table.Rows.Add(2759, 51904, '쫀');
            table.Rows.Add(2760, 51908, '쫄');
            table.Rows.Add(2761, 51916, '쫌');
            table.Rows.Add(2762, 51917, '쫍');
            table.Rows.Add(2763, 51919, '쫏');
            table.Rows.Add(2764, 51921, '쫑');
            table.Rows.Add(2765, 51923, '쫓');
            table.Rows.Add(2766, 51928, '쫘');
            table.Rows.Add(2767, 51929, '쫙');
            table.Rows.Add(2768, 51936, '쫠');
            table.Rows.Add(2769, 51948, '쫬');
            table.Rows.Add(2770, 51956, '쫴');
            table.Rows.Add(2771, 51976, '쬈');
            table.Rows.Add(2772, 51984, '쬐');
            table.Rows.Add(2773, 51988, '쬔');
            table.Rows.Add(2774, 51992, '쬘');
            table.Rows.Add(2775, 52000, '쬠');
            table.Rows.Add(2776, 52001, '쬡');
            table.Rows.Add(2777, 52033, '쭁');
            table.Rows.Add(2778, 52040, '쭈');
            table.Rows.Add(2779, 52041, '쭉');
            table.Rows.Add(2780, 52044, '쭌');
            table.Rows.Add(2781, 52048, '쭐');
            table.Rows.Add(2782, 52056, '쭘');
            table.Rows.Add(2783, 52057, '쭙');
            table.Rows.Add(2784, 52061, '쭝');
            table.Rows.Add(2785, 52068, '쭤');
            table.Rows.Add(2786, 52088, '쭸');
            table.Rows.Add(2787, 52089, '쭹');
            table.Rows.Add(2788, 52124, '쮜');
            table.Rows.Add(2789, 52152, '쮸');
            table.Rows.Add(2790, 52180, '쯔');
            table.Rows.Add(2791, 52196, '쯤');
            table.Rows.Add(2792, 52199, '쯧');
            table.Rows.Add(2793, 52201, '쯩');
            table.Rows.Add(2794, 52236, '찌');
            table.Rows.Add(2795, 52237, '찍');
            table.Rows.Add(2796, 52240, '찐');
            table.Rows.Add(2797, 52244, '찔');
            table.Rows.Add(2798, 52252, '찜');
            table.Rows.Add(2799, 52253, '찝');
            table.Rows.Add(2800, 52257, '찡');
            table.Rows.Add(2801, 52258, '찢');
            table.Rows.Add(2802, 52263, '찧');
            table.Rows.Add(2803, 52264, '차');
            table.Rows.Add(2804, 52265, '착');
            table.Rows.Add(2805, 52268, '찬');
            table.Rows.Add(2806, 52270, '찮');
            table.Rows.Add(2807, 52272, '찰');
            table.Rows.Add(2808, 52280, '참');
            table.Rows.Add(2809, 52281, '찹');
            table.Rows.Add(2810, 52283, '찻');
            table.Rows.Add(2811, 52284, '찼');
            table.Rows.Add(2812, 52285, '창');
            table.Rows.Add(2813, 52286, '찾');
            table.Rows.Add(2814, 52292, '채');
            table.Rows.Add(2815, 52293, '책');
            table.Rows.Add(2816, 52296, '챈');
            table.Rows.Add(2817, 52300, '챌');
            table.Rows.Add(2818, 52308, '챔');
            table.Rows.Add(2819, 52309, '챕');
            table.Rows.Add(2820, 52311, '챗');
            table.Rows.Add(2821, 52312, '챘');
            table.Rows.Add(2822, 52313, '챙');
            table.Rows.Add(2823, 52320, '챠');
            table.Rows.Add(2824, 52324, '챤');
            table.Rows.Add(2825, 52326, '챦');
            table.Rows.Add(2826, 52328, '챨');
            table.Rows.Add(2827, 52336, '챰');
            table.Rows.Add(2828, 52341, '챵');
            table.Rows.Add(2829, 52376, '처');
            table.Rows.Add(2830, 52377, '척');
            table.Rows.Add(2831, 52380, '천');
            table.Rows.Add(2832, 52384, '철');
            table.Rows.Add(2833, 52392, '첨');
            table.Rows.Add(2834, 52393, '첩');
            table.Rows.Add(2835, 52395, '첫');
            table.Rows.Add(2836, 52396, '첬');
            table.Rows.Add(2837, 52397, '청');
            table.Rows.Add(2838, 52404, '체');
            table.Rows.Add(2839, 52405, '첵');
            table.Rows.Add(2840, 52408, '첸');
            table.Rows.Add(2841, 52412, '첼');
            table.Rows.Add(2842, 52420, '쳄');
            table.Rows.Add(2843, 52421, '쳅');
            table.Rows.Add(2844, 52423, '쳇');
            table.Rows.Add(2845, 52425, '쳉');
            table.Rows.Add(2846, 52432, '쳐');
            table.Rows.Add(2847, 52436, '쳔');
            table.Rows.Add(2848, 52452, '쳤');
            table.Rows.Add(2849, 52460, '쳬');
            table.Rows.Add(2850, 52464, '쳰');
            table.Rows.Add(2851, 52481, '촁');
            #endregion
            return table;
        }
        static DataTable Gen3Abilities()
        {
            
            DataTable table = new DataTable();
            table.Columns.Add("Species", typeof(int));
            table.Columns.Add("Ability0", typeof(int));
            table.Columns.Add("Ability1", typeof(int));

            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["Species"];
            table.PrimaryKey = keyColumns;
            #region entries
            table.Rows.Add(0, 0, 0);
            table.Rows.Add(1,0x41,0x41);
            table.Rows.Add(2,0x41,0x41);
            table.Rows.Add(3,0x41,0x41);
            table.Rows.Add(4,0x42,0x42);
            table.Rows.Add(5,0x42,0x42);
            table.Rows.Add(6,0x42,0x42);
            table.Rows.Add(7,0x43,0x43);
            table.Rows.Add(8,0x43,0x43);
            table.Rows.Add(9,0x43,0x43);
            table.Rows.Add(10,0x13,0x13);
            table.Rows.Add(11,0x3d,0x3d);
            table.Rows.Add(12,0x0e,0x0e);
            table.Rows.Add(13,0x13,0x13);
            table.Rows.Add(14,0x3d,0x3d);
            table.Rows.Add(15,0x44,0x44);
            table.Rows.Add(16,0x33,0x33);
            table.Rows.Add(17,0x33,0x33);
            table.Rows.Add(18,0x33,0x33);
            table.Rows.Add(19,0x32,0x3e);
            table.Rows.Add(20,0x32,0x3e);
            table.Rows.Add(21,0x33,0x33);
            table.Rows.Add(22,0x33,0x33);
            table.Rows.Add(23,0x3d,0x16);
            table.Rows.Add(24,0x3d,0x16);
            table.Rows.Add(25,0x09,0x09);
            table.Rows.Add(26,0x09,0x09);
            table.Rows.Add(27,0x08,0x08);
            table.Rows.Add(28,0x08,0x08);
            table.Rows.Add(29,0x26,0x26);
            table.Rows.Add(30,0x26,0x26);
            table.Rows.Add(31,0x26,0x26);
            table.Rows.Add(32,0x26,0x26);
            table.Rows.Add(33,0x26,0x26);
            table.Rows.Add(34,0x26,0x26);
            table.Rows.Add(35,0x38,0x38);
            table.Rows.Add(36,0x38,0x38);
            table.Rows.Add(37,0x12,0x12);
            table.Rows.Add(38,0x12,0x12);
            table.Rows.Add(39,0x38,0x38);
            table.Rows.Add(40,0x38,0x38);
            table.Rows.Add(41,0x27,0x27);
            table.Rows.Add(42,0x27,0x27);
            table.Rows.Add(43,0x22,0x22);
            table.Rows.Add(44,0x22,0x22);
            table.Rows.Add(45,0x22,0x22);
            table.Rows.Add(46,0x1b,0x1b);
            table.Rows.Add(47,0x1b,0x1b);
            table.Rows.Add(48,0x0e,0x0e);
            table.Rows.Add(49,0x13,0x13);
            table.Rows.Add(50,0x08,0x47);
            table.Rows.Add(51,0x08,0x47);
            table.Rows.Add(52,0x35,0x35);
            table.Rows.Add(53,0x07,0x07);
            table.Rows.Add(54,0x06,0x0d);
            table.Rows.Add(55,0x06,0x0d);
            table.Rows.Add(56,0x48,0x48);
            table.Rows.Add(57,0x48,0x48);
            table.Rows.Add(58,0x16,0x12);
            table.Rows.Add(59,0x16,0x12);
            table.Rows.Add(60,0x06,0x0b);
            table.Rows.Add(61,0x06,0x0b);
            table.Rows.Add(62,0x06,0x0b);
            table.Rows.Add(63,0x1c,0x27);
            table.Rows.Add(64,0x1c,0x27);
            table.Rows.Add(65,0x1c,0x27);
            table.Rows.Add(66,0x3e,0x3e);
            table.Rows.Add(67,0x3e,0x3e);
            table.Rows.Add(68,0x3e,0x3e);
            table.Rows.Add(69,0x22,0x22);
            table.Rows.Add(70,0x22,0x22);
            table.Rows.Add(71,0x22,0x22);
            table.Rows.Add(72,0x1d,0x40);
            table.Rows.Add(73,0x1d,0x40);
            table.Rows.Add(74,0x45,0x05);
            table.Rows.Add(75,0x45,0x05);
            table.Rows.Add(76,0x45,0x05);
            table.Rows.Add(77,0x32,0x12);
            table.Rows.Add(78,0x32,0x12);
            table.Rows.Add(79,0x0c,0x14);
            table.Rows.Add(80,0x0c,0x14);
            table.Rows.Add(81,0x2a,0x05);
            table.Rows.Add(82,0x2a,0x05);
            table.Rows.Add(83,0x33,0x27);
            table.Rows.Add(84,0x32,0x30);
            table.Rows.Add(85,0x32,0x30);
            table.Rows.Add(86,0x2f,0x2f);
            table.Rows.Add(87,0x2f,0x2f);
            table.Rows.Add(88,0x01,0x3c);
            table.Rows.Add(89,0x01,0x3c);
            table.Rows.Add(90,0x4b,0x4b);
            table.Rows.Add(91,0x4b,0x4b);
            table.Rows.Add(92,0x1a,0x1a);
            table.Rows.Add(93,0x1a,0x1a);
            table.Rows.Add(94,0x1a,0x1a);
            table.Rows.Add(95,0x45,0x05);
            table.Rows.Add(96,0x0f,0x0f);
            table.Rows.Add(97,0x0f,0x0f);
            table.Rows.Add(98,0x34,0x4b);
            table.Rows.Add(99,0x34,0x4b);
            table.Rows.Add(100,0x2b,0x09);
            table.Rows.Add(101,0x2b,0x09);
            table.Rows.Add(102,0x22,0x22);
            table.Rows.Add(103,0x22,0x22);
            table.Rows.Add(104,0x45,0x1f);
            table.Rows.Add(105,0x45,0x1f);
            table.Rows.Add(106,0x07,0x07);
            table.Rows.Add(107,0x33,0x33);
            table.Rows.Add(108,0x0c,0x14);
            table.Rows.Add(109,0x1a,0x1a);
            table.Rows.Add(110,0x1a,0x1a);
            table.Rows.Add(111,0x45,0x1f);
            table.Rows.Add(112,0x45,0x1f);
            table.Rows.Add(113,0x1e,0x20);
            table.Rows.Add(114,0x22,0x22);
            table.Rows.Add(115,0x30,0x30);
            table.Rows.Add(116,0x21,0x21);
            table.Rows.Add(117,0x26,0x26);
            table.Rows.Add(118,0x21,0x29);
            table.Rows.Add(119,0x21,0x29);
            table.Rows.Add(120,0x23,0x1e);
            table.Rows.Add(121,0x23,0x1e);
            table.Rows.Add(122,0x2b,0x2b);
            table.Rows.Add(123,0x44,0x44);
            table.Rows.Add(124,0x0c,0x0c);
            table.Rows.Add(125,0x09,0x09);
            table.Rows.Add(126,0x31,0x31);
            table.Rows.Add(127,0x34,0x34);
            table.Rows.Add(128,0x16,0x16);
            table.Rows.Add(129,0x21,0x21);
            table.Rows.Add(130,0x16,0x16);
            table.Rows.Add(131,0x0b,0x4b);
            table.Rows.Add(132,0x07,0x07);
            table.Rows.Add(133,0x32,0x32);
            table.Rows.Add(134,0x0b,0x0b);
            table.Rows.Add(135,0x0a,0x0a);
            table.Rows.Add(136,0x12,0x12);
            table.Rows.Add(137,0x24,0x24);
            table.Rows.Add(138,0x21,0x4b);
            table.Rows.Add(139,0x21,0x4b);
            table.Rows.Add(140,0x21,0x04);
            table.Rows.Add(141,0x21,0x04);
            table.Rows.Add(142,0x45,0x2e);
            table.Rows.Add(143,0x11,0x2f);
            table.Rows.Add(144,0x2e,0x2e);
            table.Rows.Add(145,0x2e,0x2e);
            table.Rows.Add(146,0x2e,0x2e);
            table.Rows.Add(147,0x3d,0x3d);
            table.Rows.Add(148,0x3d,0x3d);
            table.Rows.Add(149,0x27,0x27);
            table.Rows.Add(150,0x2e,0x2e);
            table.Rows.Add(151,0x1c,0x1c);
            table.Rows.Add(152,0x41,0x41);
            table.Rows.Add(153,0x41,0x41);
            table.Rows.Add(154,0x41,0x41);
            table.Rows.Add(155,0x42,0x42);
            table.Rows.Add(156,0x42,0x42);
            table.Rows.Add(157,0x42,0x42);
            table.Rows.Add(158,0x43,0x43);
            table.Rows.Add(159,0x43,0x43);
            table.Rows.Add(160,0x43,0x43);
            table.Rows.Add(161,0x32,0x33);
            table.Rows.Add(162,0x32,0x33);
            table.Rows.Add(163,0x0f,0x33);
            table.Rows.Add(164,0x0f,0x33);
            table.Rows.Add(165,0x44,0x30);
            table.Rows.Add(166,0x44,0x30);
            table.Rows.Add(167,0x0f,0x44);
            table.Rows.Add(168,0x0f,0x44);
            table.Rows.Add(169,0x27,0x27);
            table.Rows.Add(170,0x0a,0x23);
            table.Rows.Add(171,0x0a,0x23);
            table.Rows.Add(172,0x09,0x09);
            table.Rows.Add(173,0x38,0x38);
            table.Rows.Add(174,0x38,0x38);
            table.Rows.Add(175,0x37,0x20);
            table.Rows.Add(176,0x37,0x20);
            table.Rows.Add(177,0x1c,0x30);
            table.Rows.Add(178,0x1c,0x30);
            table.Rows.Add(179,0x09,0x09);
            table.Rows.Add(180,0x09,0x09);
            table.Rows.Add(181,0x09,0x09);
            table.Rows.Add(182,0x22,0x22);
            table.Rows.Add(183,0x2f,0x25);
            table.Rows.Add(184,0x2f,0x25);
            table.Rows.Add(185,0x45,0x05);
            table.Rows.Add(186,0x06,0x0b);
            table.Rows.Add(187,0x22,0x22);
            table.Rows.Add(188,0x22,0x22);
            table.Rows.Add(189,0x22,0x22);
            table.Rows.Add(190,0x32,0x35);
            table.Rows.Add(191,0x22,0x22);
            table.Rows.Add(192,0x22,0x22);
            table.Rows.Add(193,0x03,0x0e);
            table.Rows.Add(194,0x06,0x0b);
            table.Rows.Add(195,0x06,0x0b);
            table.Rows.Add(196,0x1c,0x1c);
            table.Rows.Add(197,0x1c,0x1c);
            table.Rows.Add(198,0x0f,0x0f);
            table.Rows.Add(199,0x0c,0x14);
            table.Rows.Add(200,0x1a,0x1a);
            table.Rows.Add(201,0x1a,0x1a);
            table.Rows.Add(202,0x17,0x17);
            table.Rows.Add(203,0x27,0x30);
            table.Rows.Add(204,0x05,0x05);
            table.Rows.Add(205,0x05,0x05);
            table.Rows.Add(206,0x20,0x32);
            table.Rows.Add(207,0x08,0x34);
            table.Rows.Add(208,0x45,0x05);
            table.Rows.Add(209,0x16,0x32);
            table.Rows.Add(210,0x16,0x16);
            table.Rows.Add(211,0x21,0x26);
            table.Rows.Add(212,0x44,0x44);
            table.Rows.Add(213,0x05,0x05);
            table.Rows.Add(214,0x44,0x3e);
            table.Rows.Add(215,0x27,0x33);
            table.Rows.Add(216,0x35,0x35);
            table.Rows.Add(217,0x3e,0x3e);
            table.Rows.Add(218,0x28,0x31);
            table.Rows.Add(219,0x28,0x31);
            table.Rows.Add(220,0x0c,0x0c);
            table.Rows.Add(221,0x0c,0x0c);
            table.Rows.Add(222,0x37,0x1e);
            table.Rows.Add(223,0x37,0x37);
            table.Rows.Add(224,0x15,0x15);
            table.Rows.Add(225,0x37,0x48);
            table.Rows.Add(226,0x21,0x0b);
            table.Rows.Add(227,0x33,0x05);
            table.Rows.Add(228,0x30,0x12);
            table.Rows.Add(229,0x30,0x12);
            table.Rows.Add(230,0x21,0x21);
            table.Rows.Add(231,0x35,0x35);
            table.Rows.Add(232,0x05,0x05);
            table.Rows.Add(233,0x24,0x24);
            table.Rows.Add(234,0x16,0x16);
            table.Rows.Add(235,0x14,0x14);
            table.Rows.Add(236,0x3e,0x3e);
            table.Rows.Add(237,0x16,0x16);
            table.Rows.Add(238,0x0c,0x0c);
            table.Rows.Add(239,0x09,0x09);
            table.Rows.Add(240,0x31,0x31);
            table.Rows.Add(241,0x2f,0x2f);
            table.Rows.Add(242,0x1e,0x20);
            table.Rows.Add(243,0x2e,0x2e);
            table.Rows.Add(244,0x2e,0x2e);
            table.Rows.Add(245,0x2e,0x2e);
            table.Rows.Add(246,0x3e,0x3e);
            table.Rows.Add(247,0x3d,0x3d);
            table.Rows.Add(248,0x2d,0x2d);
            table.Rows.Add(249,0x2e,0x2e);
            table.Rows.Add(250,0x2e,0x2e);
            table.Rows.Add(251,0x1e,0x1e);
            table.Rows.Add(252,0x41,0x41);
            table.Rows.Add(253,0x41,0x41);
            table.Rows.Add(254,0x41,0x41);
            table.Rows.Add(255,0x42,0x42);
            table.Rows.Add(256,0x42,0x42);
            table.Rows.Add(257,0x42,0x42);
            table.Rows.Add(258,0x43,0x43);
            table.Rows.Add(259,0x43,0x43);
            table.Rows.Add(260,0x43,0x43);
            table.Rows.Add(261,0x32,0x32);
            table.Rows.Add(262,0x16,0x16);
            table.Rows.Add(263,0x35,0x35);
            table.Rows.Add(264,0x35,0x35);
            table.Rows.Add(265,0x13,0x13);
            table.Rows.Add(266,0x3d,0x3d);
            table.Rows.Add(267,0x44,0x44);
            table.Rows.Add(268,0x3d,0x3d);
            table.Rows.Add(269,0x13,0x13);
            table.Rows.Add(270,0x21,0x2c);
            table.Rows.Add(271,0x21,0x2c);
            table.Rows.Add(272,0x21,0x2c);
            table.Rows.Add(273,0x22,0x30);
            table.Rows.Add(274,0x22,0x30);
            table.Rows.Add(275,0x22,0x30);
            table.Rows.Add(276,0x3e,0x3e);
            table.Rows.Add(277,0x3e,0x3e);
            table.Rows.Add(278,0x33,0x33);
            table.Rows.Add(279,0x33,0x33);
            table.Rows.Add(280,0x1c,0x24);
            table.Rows.Add(281,0x1c,0x24);
            table.Rows.Add(282,0x1c,0x24);
            table.Rows.Add(283,0x21,0x21);
            table.Rows.Add(284,0x16,0x16);
            table.Rows.Add(285,0x1b,0x1b);
            table.Rows.Add(286,0x1b,0x1b);
            table.Rows.Add(287,0x36,0x36);
            table.Rows.Add(288,0x48,0x48);
            table.Rows.Add(289,0x36,0x36);
            table.Rows.Add(290,0x0e,0x0e);
            table.Rows.Add(291,0x03,0x03);
            table.Rows.Add(292,0x19,0x19);
            table.Rows.Add(293,0x2b,0x2b);
            table.Rows.Add(294,0x2b,0x2b);
            table.Rows.Add(295,0x2b,0x2b);
            table.Rows.Add(296,0x2f,0x3e);
            table.Rows.Add(297,0x2f,0x3e);
            table.Rows.Add(298,0x2f,0x3e);
            table.Rows.Add(299,0x05,0x2a);
            table.Rows.Add(300,0x38,0x38);
            table.Rows.Add(301,0x38,0x38);
            table.Rows.Add(302,0x33,0x33);
            table.Rows.Add(303,0x34,0x16);
            table.Rows.Add(304,0x45,0x05);
            table.Rows.Add(305,0x45,0x05);
            table.Rows.Add(306,0x45,0x05);
            table.Rows.Add(307,0x4a,0x4a);
            table.Rows.Add(308,0x4a,0x4a);
            table.Rows.Add(309,0x09,0x1f);
            table.Rows.Add(310,0x09,0x1f);
            table.Rows.Add(311,0x39,0x39);
            table.Rows.Add(312,0x3a,0x3a);
            table.Rows.Add(313,0x23,0x44);
            table.Rows.Add(314,0x0c,0x0c);
            table.Rows.Add(315,0x1e,0x26);
            table.Rows.Add(316,0x40,0x3c);
            table.Rows.Add(317,0x40,0x3c);
            table.Rows.Add(318,0x18,0x18);
            table.Rows.Add(319,0x18,0x18);
            table.Rows.Add(320,0x29,0x0c);
            table.Rows.Add(321,0x29,0x0c);
            table.Rows.Add(322,0x0c,0x0c);
            table.Rows.Add(323,0x28,0x28);
            table.Rows.Add(324,0x49,0x49);
            table.Rows.Add(325,0x2f,0x14);
            table.Rows.Add(326,0x2f,0x14);
            table.Rows.Add(327,0x14,0x14);
            table.Rows.Add(328,0x34,0x47);
            table.Rows.Add(329,0x1a,0x1a);
            table.Rows.Add(330,0x1a,0x1a);
            table.Rows.Add(331,0x08,0x08);
            table.Rows.Add(332,0x08,0x08);
            table.Rows.Add(333,0x1e,0x1e);
            table.Rows.Add(334,0x1e,0x1e);
            table.Rows.Add(335,0x11,0x11);
            table.Rows.Add(336,0x3d,0x3d);
            table.Rows.Add(337,0x1a,0x1a);
            table.Rows.Add(338,0x1a,0x1a);
            table.Rows.Add(339,0x0c,0x0c);
            table.Rows.Add(340,0x0c,0x0c);
            table.Rows.Add(341,0x34,0x4b);
            table.Rows.Add(342,0x34,0x4b);
            table.Rows.Add(343,0x1a,0x1a);
            table.Rows.Add(344,0x1a,0x1a);
            table.Rows.Add(345,0x15,0x15);
            table.Rows.Add(346,0x15,0x15);
            table.Rows.Add(347,0x04,0x04);
            table.Rows.Add(348,0x04,0x04);
            table.Rows.Add(349,0x21,0x21);
            table.Rows.Add(350,0x3f,0x3f);
            table.Rows.Add(351,0x3b,0x3b);
            table.Rows.Add(352,0x10,0x10);
            table.Rows.Add(353,0x0f,0x0f);
            table.Rows.Add(354,0x0f,0x0f);
            table.Rows.Add(355,0x1a,0x1a);
            table.Rows.Add(356,0x2e,0x2e);
            table.Rows.Add(357,0x22,0x22);
            table.Rows.Add(358,0x1a,0x1a);
            table.Rows.Add(359,0x2e,0x2e);
            table.Rows.Add(360,0x17,0x17);
            table.Rows.Add(361,0x27,0x27);
            table.Rows.Add(362,0x27,0x27);
            table.Rows.Add(363,0x2f,0x2f);
            table.Rows.Add(364,0x2f,0x2f);
            table.Rows.Add(365,0x2f,0x2f);
            table.Rows.Add(366,0x4b,0x4b);
            table.Rows.Add(367,0x21,0x21);
            table.Rows.Add(368,0x21,0x21);
            table.Rows.Add(369,0x21,0x45);
            table.Rows.Add(370,0x21,0x21);
            table.Rows.Add(371,0x45,0x45);
            table.Rows.Add(372,0x45,0x45);
            table.Rows.Add(373,0x16,0x16);
            table.Rows.Add(374,0x1d,0x1d);
            table.Rows.Add(375,0x1d,0x1d);
            table.Rows.Add(376,0x1d,0x1d);
            table.Rows.Add(377,0x1d,0x1d);
            table.Rows.Add(378,0x1d,0x1d);
            table.Rows.Add(379,0x1d,0x1d);
            table.Rows.Add(380,0x1a,0x1a);
            table.Rows.Add(381,0x1a,0x1a);
            table.Rows.Add(382,0x02,0x02);
            table.Rows.Add(383,0x46,0x46);
            table.Rows.Add(384,0x4c,0x4c);
            table.Rows.Add(385,0x20,0x20);
            table.Rows.Add(386,0x2e,0x2e);
            #endregion
            return table;
        }

        private int getg3species(int g3index)
        {
            int[] newindex = new int[] 
            {
                0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,
                31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,
                59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,77,79,80,81,82,83,84,85,86,
                87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,
                111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,
                132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,
                153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,
                174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,
                195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,
                216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,
                237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,
                258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,290,291,292,
                276,277,285,286,327,278,279,283,284,320,321,300,301,352,343,344,299,324,302,339,340,
                370,341,342,349,350,318,319,328,329,330,296,297,309,310,322,323,363,364,365,331,332,
                361,362,337,338,298,325,326,311,312,303,307,308,333,334,360,355,356,315,287,288,289,
                316,317,357,293,294,295,366,367,368,359,353,354,336,335,369,304,305,306,351,313,314,
                345,346,347,348,280,281,282,371,372,373,374,375,376,377,378,379,382,383,384,380,381,
                385,386,358,
            };
            int[] oldindex = new int[] 
            {
                0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,
                31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,
                59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,
                87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,
                111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,
                132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,
                153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,
                174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,
                195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,
                216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,
                237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,277,278,279,280,281,282,
                283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,
                304,305,306,307,308,309,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,
                325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,
                346,347,348,349,350,351,352,353,354,355,356,357,358,359,360,361,362,363,364,365,366,
                367,368,369,370,371,372,373,374,375,376,377,378,379,380,381,382,383,384,385,386,387,
                388,389,390,391,392,393,394,395,396,397,398,399,400,401,402,403,404,405,406,407,408,
                409,410,411,
            };
            return newindex[Array.IndexOf(oldindex, g3index)];
        }
        #endregion
    }
}
