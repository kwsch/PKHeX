using System;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core
{
    public static class PKMConverter
    {
        public static int Country = 49;
        public static int Region = 7;
        public static int ConsoleRegion = 1;
        public static string OT_Name = "PKHeX";
        public static int OT_Gender; // Male
        public static int Language = 1; // en

        public static void updateConfig(int SUBREGION, int COUNTRY, int _3DSREGION, string TRAINERNAME, int TRAINERGENDER, int LANGUAGE)
        {
            Region = SUBREGION;
            Country = COUNTRY;
            ConsoleRegion = _3DSREGION;
            OT_Name = TRAINERNAME;
            OT_Gender = TRAINERGENDER;
            Language = LANGUAGE;
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
        /// <param name="prefer">Optional identifier for the preferred generation.  Usually the generation of the destination save file.</param>
        /// <returns>An instance of <see cref="PKM"/> created from the given <paramref name="data"/>, or null if <paramref name="data"/> is invalid.</returns>
        public static PKM getPKMfromBytes(byte[] data, string ident = null, int prefer = 7)
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
                    var pkx = new PK6(data, ident);
                    return checkPKMFormat7(pkx, prefer);
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Checks if the input PK6 file is really a PK7, if so, updates the object.
        /// </summary>
        /// <param name="pk">PKM to check</param>
        /// <param name="prefer">Prefer a certain generation over another</param>
        /// <returns>Updated PKM if actually PK7</returns>
        private static PKM checkPKMFormat7(PK6 pk, int prefer) => checkPK6is7(pk, prefer) ? new PK7(pk.Data, pk.Identifier) : (PKM)pk;
        /// <summary>
        /// Checks if the input PK6 file is really a PK7.
        /// </summary>
        /// <param name="pk">PK6 to check</param>
        /// <param name="prefer">Prefer a certain generation over another</param>
        /// <returns>Boolean is a PK7</returns>
        private static bool checkPK6is7(PK6 pk, int prefer)
        {
            if (pk.Version > Legal.MaxGameID_6)
                return true;
            if (pk.Enjoyment != 0 || pk.Fullness != 0)
                return false;

            // Check Ranges
            if (pk.Species > Legal.MaxSpeciesID_6)
                return true;
            if (pk.Moves.Any(move => move > Legal.MaxMoveID_6_AO))
                return true;
            if (pk.RelearnMoves.Any(move => move > Legal.MaxMoveID_6_AO))
                return true;
            if (pk.Ability > Legal.MaxAbilityID_6_AO)
                return true;
            if (pk.HeldItem > Legal.MaxItemID_6_AO)
                return true;

            int et = pk.EncounterType;
            if (et != 0)
            {
                if (pk.CurrentLevel < 100) // can't be hyper trained
                    return false;

                if (pk.GenNumber != 4) // can't have encounter type
                    return true;
                if (et > 24) // invalid encountertype
                    return true;
            }

            int mb = BitConverter.ToUInt16(pk.Data, 0x16);
            if (mb > 0xAAA)
                return false;
            for (int i = 0; i < 6; i++)
                if ((mb >> (i << 1) & 3) == 3) // markings are 10 or 01 (or 00), never 11
                    return false;

            return prefer > 6;
        }

        public static PKM convertToFormat(PKM pk, Type PKMType, out string comment)
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
                    case nameof(PK1):
                        if (toFormat == 2)
                        {
                            pkm = PKMType == typeof (PK2) ? ((PK1) pk).convertToPK2() : null;
                            break;
                        }
                        if (toFormat == 7)
                            pkm = ((PK1) pk).convertToPK7();
                        break;
                    case nameof(PK2):
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
                    case nameof(CK3):
                    case nameof(XK3):
                        // interconverting C/XD needs to visit main series format
                        // ends up stripping purification/shadow etc stats
                        pkm = pkm.convertToPK3();
                        goto case nameof(PK3); // fall through
                    case nameof(PK3):
                        if (toFormat == 3) // Gen3 Inter-trading
                        {
                            switch (PKMType.Name)
                            {
                                case nameof(CK3): pkm = pkm.convertToCK3(); break;
                                case nameof(XK3): pkm = pkm.convertToXK3(); break;
                                case nameof(PK3): pkm = pkm.convertToPK3(); break; // already converted, instantly returns
                                default: throw new FormatException();
                            }
                            break;
                        }
                        if (fromType.Name != nameof(PK3))
                            pkm = pkm.convertToPK3();

                        pkm = ((PK3)pkm).convertToPK4();
                        if (toFormat == 4)
                            break;
                        goto case nameof(PK4);
                    case nameof(BK4):
                        pkm = ((BK4)pkm).convertToPK4();
                        if (toFormat == 4)
                            break;
                        goto case nameof(PK4);
                    case nameof(PK4):
                        if (PKMType == typeof(BK4))
                        {
                            pkm = ((PK4)pkm).convertToBK4();
                            break;
                        }
                        pkm = ((PK4)pkm).convertToPK5();
                        if (toFormat == 5)
                            break;
                        goto case nameof(PK5);
                    case nameof(PK5):
                        pkm = ((PK5)pkm).convertToPK6();
                        if (toFormat == 6)
                            break;
                        goto case nameof(PK6);
                    case nameof(PK6):
                        pkm = ((PK6)pkm).convertToPK7();
                        if (toFormat == 7)
                            break;
                        goto case nameof(PK7);
                    case nameof(PK7):
                        break;
                }
            }

            comment = pkm == null
                ? $"Cannot convert a {fromType.Name} to a {PKMType.Name}." 
                : $"Converted from {fromType.Name} to {PKMType.Name}.";

            return pkm;
        }
        public static void checkEncrypted(ref byte[] pkm)
        {
            int format = getPKMDataFormat(pkm);
            switch (format)
            {
                case 1:
                case 2: // no encryption
                    return;
                case 3:
                    if (pkm.Length == PKX.SIZE_3CSTORED || pkm.Length == PKX.SIZE_3XSTORED)
                        return; // no encryption for C/XD
                    ushort chk = 0;
                    for (int i = 0x20; i < PKX.SIZE_3STORED; i += 2)
                        chk += BitConverter.ToUInt16(pkm, i);
                    if (chk != BitConverter.ToUInt16(pkm, 0x1C))
                        pkm = PKX.decryptArray3(pkm);
                    return;
                case 4:
                case 5:
                    if (BitConverter.ToUInt16(pkm, 4) != 0) // BK4
                        return;
                    if (BitConverter.ToUInt32(pkm, 0x64) != 0)
                        pkm = PKX.decryptArray45(pkm);
                    return;
                case 6:
                case 7:
                    if (BitConverter.ToUInt16(pkm, 0xC8) != 0 && BitConverter.ToUInt16(pkm, 0x58) != 0)
                        pkm = PKX.decryptArray(pkm);
                    return;
                default:
                    return; // bad!
            }
        }

        /// <summary>
        /// Gets a Blank <see cref="PKM"/> object of the specified type.
        /// </summary>
        /// <param name="t">Type of <see cref="PKM"/> instance desired.</param>
        /// <returns>New instance of a blank <see cref="PKM"/> object.</returns>
        public static PKM getBlank(Type t) => (PKM)Activator.CreateInstance(t, Enumerable.Repeat(null as PKM, t.GetTypeInfo().DeclaredConstructors.First().GetParameters().Length).ToArray());

        public static void transferProperties(PKM source, PKM dest)
        {
            source.TransferPropertiesWithReflection(source, dest);
        }
    }
}
