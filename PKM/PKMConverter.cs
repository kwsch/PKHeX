using System;

namespace PKHeX
{
    internal static class PKMConverter
    {
        internal static int Country = 31;
        internal static int Region = 7;
        internal static int ConsoleRegion = 1;
        internal static string OT_Name = "PKHeX";
        internal static int OT_Gender;
        
        internal static void updateConfig(int SUBREGION, int COUNTRY, int _3DSREGION, string TRAINERNAME, int TRAINERGENDER)
        {
            Region = SUBREGION;
            Country = COUNTRY;
            ConsoleRegion = _3DSREGION;
            OT_Name = TRAINERNAME;
            OT_Gender = TRAINERGENDER;
        }

        private static int getPKMDataFormat(byte[] data)
        {
            if (!PKX.getIsPKM(data.Length))
                return -1;

            switch (data.Length)
            {
                case PKX.SIZE_3PARTY:
                case PKX.SIZE_3STORED:
                    return 3;
                case PKX.SIZE_4PARTY:
                case PKX.SIZE_4STORED:
                case PKX.SIZE_5PARTY:
                    if ((BitConverter.ToUInt16(data, 0x80) >= 0x3333 || data[0x5F] >= 0x10) && BitConverter.ToUInt16(data, 0x46) == 0) // PK5
                        return 5;
                    return 4;
                case PKX.SIZE_6STORED:
                    return 6;
                case PKX.SIZE_6PARTY: // collision with PGT, same size.
                    if (BitConverter.ToUInt16(data, 0x4) != 0) // Bad Sanity?
                        return -1;
                    if (BitConverter.ToUInt32(data, 0x06) == PKX.getCHK(data))
                        return 6;
                    if (BitConverter.ToUInt16(data, 0x58) != 0) // Encrypted?
                    {
                        for (int i = data.Length - 0x10; i < data.Length; i++) // 0x10 of 00's at the end != PK6
                            if (data[i] != 0)
                                return 6;
                        return -1;
                    }
                    return 6;
            }
            return -1;
        }
        internal static PKM getPKMfromBytes(byte[] data, string ident = null)
        {
            checkEncrypted(ref data);
            switch (getPKMDataFormat(data))
            {
                case 3:
                    return new PK3(data, ident);
                case 4:
                    return new PK4(data, ident);
                case 5:
                    return new PK5(data, ident);
                case 6:
                    return new PK6(data, ident);
                default:
                    return null;
            }
        }
        internal static PKM convertToFormat(PKM pk, int Format, out string comment)
        {
            if (pk == null)
            {
                comment = "Null input. Aborting.";
                return null;
            }
            if (pk.Format == Format)
            {
                comment = "No need to convert, current format matches requested format.";
                return pk;
            }
            if (pk.Format > Format)
            {
                comment = "Cannot convert a PKM backwards." + Environment.NewLine
                          + "Current Format: " + pk.Format + Environment.NewLine
                          + "Desired Format: " + Format;
                return null;
            }
            string currentFormat = pk.Format.ToString();
            PKM pkm = pk.Clone();
            if (pkm.IsEgg) // force hatch
            {
                pkm.IsEgg = false;
                if (pkm.AO)
                    pkm.Met_Location = 318; // Battle Resort
                else if (pkm.XY)
                    pkm.Met_Location = 38; // Route 7
                else if (pkm.Gen5)
                    pkm.Met_Location = 16; // Route 16
                else
                    pkm.Met_Location = 30001; // Pokétransfer
            }
            if (pkm.Format == 3 && Format > 3)
                pkm = (pkm as PK3).convertToPK4();
            if (pkm.Format == 4 && Format > 4)
                pkm = (pkm as PK4).convertToPK5();
            if (pkm.Format == 5 && Format > 5)
                pkm = (pkm as PK5).convertToPK6();
            comment = $"Converted from pk{currentFormat} to pk{Format}";
            return pkm;
        }
        internal static void checkEncrypted(ref byte[] pkm)
        {
            int format = getPKMDataFormat(pkm);
            ushort chk = 0;
            switch (format)
            {
                case 3: // TOneverDO, nobody exports encrypted pk3s
                    return;
                case 4:
                case 5:
                    for (int i = 8; i < PKX.SIZE_4STORED; i += 2)
                        chk += BitConverter.ToUInt16(pkm, i);
                    if (chk != BitConverter.ToUInt16(pkm, 0x06))
                        pkm = PKX.decryptArray45(pkm);
                    return;
                case 6:
                    if (BitConverter.ToUInt16(pkm, 0xC8) != 0 && BitConverter.ToUInt16(pkm, 0x58) != 0)
                        pkm = PKX.decryptArray(pkm);
                    return;
                default:
                    return; // bad!
            }
        }
    }
}
