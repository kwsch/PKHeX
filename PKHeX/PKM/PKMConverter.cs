using System;
using System.Linq;

namespace PKHeX
{
    public static class PKMConverter
    {
        internal static int Country = 49;
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

        /// <summary>
        /// Gets the generation of the Pokemon data.
        /// </summary>
        /// <param name="data">Raw data representing a Pokemon.</param>
        /// <returns>An integer indicating the generation of the PKM file, or -1 if the data is invalid.</returns>
        public static int getPKMDataFormat(byte[] data)
        {
            if (!PKX.getIsPKM(data.Length))
                return -1;

            switch (data.Length)
            {
                case PKX.SIZE_1JLIST:
                case PKX.SIZE_1ULIST:
                    return 1;
                case PKX.SIZE_2ULIST:
                case PKX.SIZE_2JLIST:
                    return 2;
                case PKX.SIZE_3PARTY:
                case PKX.SIZE_3STORED:
                case PKX.SIZE_3CSTORED:
                case PKX.SIZE_3XSTORED:
                    return 3;
                case PKX.SIZE_4PARTY:
                case PKX.SIZE_4STORED:
                case PKX.SIZE_5PARTY:
                    if ((BitConverter.ToUInt16(data, 0x4) == 0) && (BitConverter.ToUInt16(data, 0x80) >= 0x3333 || data[0x5F] >= 0x10) && BitConverter.ToUInt16(data, 0x46) == 0) // PK5
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

        /// <summary>
        /// Creates an instance of <see cref="PKM"/> from the given data.
        /// </summary>
        /// <param name="data">Raw data of the Pokemon file.</param>
        /// <param name="ident">Optional identifier for the Pokemon.  Usually the full path of the source file.</param>
        /// <returns>An instance of <see cref="PKM"/> created from the given <paramref name="data"/>, or null if <paramref name="data"/> is invalid.</returns>
        public static PKM getPKMfromBytes(byte[] data, string ident = null)
        {
            checkEncrypted(ref data);
            switch (getPKMDataFormat(data))
            {
                case 1:
                    var PL1 = new PokemonList1(data, PokemonList1.CapacityType.Single, data.Length == PKX.SIZE_1JLIST);
                    if (ident != null)
                        PL1[0].Identifier = ident;
                    return PL1[0];
                case 2:
                    var PL2 = new PokemonList2(data, PokemonList2.CapacityType.Single, data.Length == PKX.SIZE_2JLIST);
                    if (ident != null)
                        PL2[0].Identifier = ident;
                    return PL2[0];
                case 3:
                    switch (data.Length) { 
                        case PKX.SIZE_3CSTORED: return new CK3(data, ident);
                        case PKX.SIZE_3XSTORED: return new XK3(data, ident);
                        default: return new PK3(data, ident);
                    }
                case 4:
                    var pk = new PK4(data, ident);
                    if (!pk.Valid || pk.Sanity != 0)
                    {
                        var bk = new BK4(data, ident);
                        if (bk.Valid)
                            return bk;
                    }
                    return pk;
                case 5:
                    return new PK5(data, ident);
                case 6:
                    PKM pkx = new PK6(data, ident);
                    if (pkx.SM)
                        pkx = new PK7(data, ident);
                    return pkx;
                default:
                    return null;
            }
        }
        internal static PKM convertToFormat(PKM pk, Type PKMType, out string comment)
        {
            if (pk == null || pk.Species == 0)
            {
                comment = "Null input. Aborting.";
                return null;
            }

            Type fromType = pk.GetType();
            int fromFormat = int.Parse(fromType.Name.Last().ToString());
            int toFormat = int.Parse(PKMType.Name.Last().ToString());
            Console.WriteLine($"Trying to convert {fromType.Name} to {PKMType.Name}.");

            PKM pkm = null;

            if (fromType == PKMType)
            {
                comment = "No need to convert, current format matches requested format.";
                return pk;
            }
            if (fromFormat <= toFormat || fromFormat == 2)
            {
                pkm = pk.Clone();
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
                switch (fromType.Name)
                {
                    case "PK1":
                        if (toFormat == 2)
                        {
                            pkm = PKMType == typeof (PK2) ? ((PK1) pk).convertToPK2() : null;
                            break;
                        }
                        if (toFormat == 7)
                            pkm = null; // pkm.convertPK1toPK7();
                        break;
                    case "PK2":
                        if (PKMType == typeof (PK1))
                        {
                            if (pk.Species > 151)
                            {
                                comment = $"Cannot convert a {PKX.getSpeciesName(pkm.Species, ((PK2)pkm).Japanese ? 1 : 2)} to {PKMType.Name}";
                                return null;
                            }
                            pkm = ((PK2) pk).convertToPK1();
                        }
                        else
                            pkm = null;
                        break;
                    case "CK3":
                    case "XK3":
                        // interconverting C/XD needs to visit main series format
                        // ends up stripping purification/shadow etc stats
                        pkm = pkm.convertToPK3();
                        goto case "PK3"; // fall through
                    case "PK3":
                        if (toFormat == 3) // Gen3 Inter-trading
                        {
                            switch (PKMType.Name)
                            {
                                case "CK3": pkm = pkm.convertToCK3(); break;
                                case "XK3": pkm = pkm.convertToXK3(); break;
                                case "PK3": pkm = pkm.convertToPK3(); break; // already converted, instantly returns
                                default: throw new FormatException();
                            }
                            break;
                        }
                        if (fromType.Name != "PK3")
                            pkm = pkm.convertToPK3();

                        pkm = ((PK3)pkm).convertToPK4();
                        if (toFormat == 4)
                        {
                            if (PKMType == typeof (BK4))
                                pkm = ((PK4) pkm).convertToBK4();
                            break;
                        }
                        pkm = ((PK4)pkm).convertToPK5();
                        if (toFormat == 5)
                            break;
                        pkm = ((PK5)pkm).convertToPK6();
                        if (toFormat == 6)
                            break;
                        pkm = new PK7(pkm.Data, pkm.Identifier);
                        break;
                    case "PK4":
                        if (PKMType == typeof(BK4))
                        {
                            pkm = ((PK4)pkm).convertToBK4();
                            break;
                        }
                        pkm = ((PK4)pkm).convertToPK5();
                        if (toFormat == 5)
                            break;
                        pkm = ((PK5)pkm).convertToPK6();
                        if (toFormat == 6)
                            break;
                        pkm = new PK7(pkm.Data, pkm.Identifier);
                        break;
                    case "BK4":
                        pkm = ((BK4)pkm).convertToPK4();
                        if (toFormat == 4)
                            break;
                        pkm = ((PK4)pkm).convertToPK5();
                        if (toFormat == 5)
                            break;
                        pkm = ((PK5)pkm).convertToPK6();
                        if (toFormat == 6)
                            break;
                        pkm = new PK7(pkm.Data, pkm.Identifier);
                        break;
                    case "PK5":
                        pkm = ((PK5)pkm).convertToPK6();
                        break;
                    case "PK6":
                        pkm = new PK7(pkm.Data, pkm.Identifier);
                        break;
                }
            }

            comment = pkm == null
                ? $"Cannot convert a {fromType.Name} to a {PKMType.Name}." 
                : $"Converted from {fromType.Name} to {PKMType.Name}.";

            return pkm;
        }
        internal static void checkEncrypted(ref byte[] pkm)
        {
            int format = getPKMDataFormat(pkm);
            ushort chk = 0;
            switch (format)
            {
                case 1:
                case 3: // TOneverDO, nobody exports encrypted pk3s
                    return;
                case 4:
                case 5:
                    if (BitConverter.ToUInt16(pkm, 4) != 0) // BK4
                        return;
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
