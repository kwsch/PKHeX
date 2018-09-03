using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for converting a <see cref="PKM"/> from one generation specific format to another.
    /// </summary>
    public static class PKMConverter
    {
        public static ITrainerInfo Trainer { private get; set; } = new SimpleTrainerInfo();
        public static int Country => Trainer.Country;
        public static int Region => Trainer.SubRegion;
        public static int ConsoleRegion => Trainer.ConsoleRegion;
        public static string OT_Name => Trainer.OT;
        public static int OT_Gender => Trainer.Gender;
        public static int Language => Trainer.Language;
        public static int Format => Trainer.Generation;
        public static bool AllowIncompatibleConversion { private get; set; }

        /// <summary>
        /// Gets the generation of the Pokemon data.
        /// </summary>
        /// <param name="data">Raw data representing a Pokemon.</param>
        /// <returns>An integer indicating the generation of the PKM file, or -1 if the data is invalid.</returns>
        public static int GetPKMDataFormat(byte[] data)
        {
            if (!PKX.IsPKM(data.Length))
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
                    if (BitConverter.ToUInt32(data, 0x06) == PKX.GetCHK(data))
                        return 6;
                    if (BitConverter.ToUInt16(data, 0x58) != 0) // Encrypted?
                    {
                        for (int i = data.Length - 0x10; i < data.Length; i++) // 0x10 of 00's at the end != PK6
                        {
                            if (data[i] != 0)
                                return 6;
                        }

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
        public static PKM GetPKMfromBytes(byte[] data, string ident = null, int prefer = 7)
        {
            int format = GetPKMDataFormat(data);
            switch (format)
            {
                case 1:
                    var PL1 = new PokeList1(data);
                    if (ident != null)
                        PL1[0].Identifier = ident;
                    return PL1[0];
                case 2:
                    var PL2 = new PokeList2(data);
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
                    return CheckPKMFormat7(pkx, prefer);
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
        private static PKM CheckPKMFormat7(PK6 pk, int prefer) => IsPK6FormatReallyPK7(pk, prefer) ? new PK7(pk.Data, pk.Identifier) : (PKM)pk;

        /// <summary>
        /// Checks if the input PK6 file is really a PK7.
        /// </summary>
        /// <param name="pk">PK6 to check</param>
        /// <param name="preferredFormat">Prefer a certain generation over another</param>
        /// <returns>Boolean is a PK7</returns>
        private static bool IsPK6FormatReallyPK7(PK6 pk, int preferredFormat)
        {
            if (pk.Version > Legal.MaxGameID_6)
                return true;

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
            {
                if ((mb >> (i << 1) & 3) == 3) // markings are 10 or 01 (or 00), never 11
                    return false;
            }

            if (pk.Data[0x2A] > 20) // ResortEventStatus is always < 20
                return false;

            return preferredFormat > 6;
        }

        /// <summary>
        /// Checks if the input <see cref="PKM"/> file is capable of being converted to the desired format.
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool IsConvertibleToFormat(PKM pk, int format)
        {
            if (pk.Format >= 3 && pk.Format > format)
                return false; // pk3->upward can't go backwards
            if (pk.Format <= 2 && format > 2 && format < 7)
                return false; // pk1/2->upward has to be 7 or greater
            return true;
        }

        /// <summary>
        /// Converts a PKM from one Generation 3 format to another. If it matches the destination format, the conversion will automatically return.
        /// </summary>
        /// <param name="pk">PKM to convert</param>
        /// <param name="PKMType">Format/Type to convert to</param>
        /// <param name="comment">Comments regarding the transfer's success/failure</param>
        /// <returns>Converted PKM</returns>
        public static PKM ConvertToType(PKM pk, Type PKMType, out string comment)
        {
            if (pk == null)
            {
                comment = $"Bad {nameof(pk)} input. Aborting.";
                return null;
            }

            Type fromType = pk.GetType();
            if (fromType == PKMType)
            {
                comment = "No need to convert, current format matches requested format.";
                return pk;
            }

            var pkm = ConvertPKM(pk, PKMType, fromType, out comment);
            if (!AllowIncompatibleConversion || pkm != null)
                return pkm;

            // Try Incompatible Conversion
            pkm = GetBlank(PKMType);
            pk.TransferPropertiesWithReflection(pkm);
            if (!IsPKMCompatibleWithModifications(pkm))
                return null;
            comment = "Converted via reflection.";
            return pkm;
        }

        private static PKM ConvertPKM(PKM pk, Type PKMType, Type fromType, out string comment)
        {
            if (IsNotTransferrable(pk, out comment))
                return null;

            Debug.WriteLine($"Trying to convert {fromType.Name} to {PKMType.Name}.");

            int fromFormat = int.Parse(fromType.Name.Last().ToString());
            int toFormat = int.Parse(PKMType.Name.Last().ToString());
            if (fromFormat > toFormat && fromFormat != 2)
            {
                comment = string.Format(MsgPKMConvertFailFormat, fromType.Name, PKMType.Name);
                return null;
            }

            var pkm = ConvertPKM(pk, PKMType, toFormat, ref comment);

            comment = pkm == null
                ? string.Format(MsgPKMConvertFailFormat, fromType.Name, PKMType.Name)
                : string.Format(MsgPKMConvertSuccess, fromType.Name, PKMType.Name);
            return pkm;
        }

        private static PKM ConvertPKM(PKM pk, Type PKMType, int toFormat, ref string comment)
        {
            PKM pkm = pk.Clone();
            if (pkm.IsEgg)
                pkm.ForceHatchPKM();
            while (true)
            {
                pkm = IntermediaryConvert(pkm, PKMType, toFormat, ref comment);
                if (pkm == null) // fail convert
                    return null;
                if (pkm.GetType() == PKMType) // finish convert
                    return pkm;
            }
        }

        private static PKM IntermediaryConvert(PKM pk, Type PKMType, int toFormat, ref string comment)
        {
            switch (pk)
            {
                // Non-sequential
                case PK1 pk1 when toFormat > 2: return pk1.ConvertToPK7();
                case PK2 pk2 when toFormat > 2: return pk2.ConvertToPK7();
                case PK3 pk3 when PKMType == typeof(CK3): return pk3.ConvertToCK3();
                case PK3 pk3 when PKMType == typeof(XK3): return pk3.ConvertToXK3();
                case PK4 pk4 when PKMType == typeof(BK4): return pk4.ConvertToBK4();

                // Invalid
                case PK2 pk2 when pk.Species > Legal.MaxSpeciesID_1:
                    var lang = pk2.Japanese ? (int)LanguageID.Japanese : (int)LanguageID.English;
                    var name = PKX.GetSpeciesName(pk2.Species, lang);
                    comment = string.Format(MsgPKMConvertFailFormat, name, PKMType.Name);
                    return null;

                // Sequential
                case PK1 pk1: return pk1.ConvertToPK2();
                case PK2 pk2: return pk2.ConvertToPK1();
                case CK3 ck3: return ck3.ConvertToPK3();
                case XK3 xk3: return xk3.ConvertToPK3();
                case PK3 pk3: return pk3.ConvertToPK4();
                case BK4 bk4: return bk4.ConvertToPK4();
                case PK4 pk4: return pk4.ConvertToPK5();
                case PK5 pk5: return pk5.ConvertToPK6();
                case PK6 pk6: return pk6.ConvertToPK7();

                // None
                default:
                    comment = MsgPKMConvertFailNoMethod;
                    return null;
            }
        }

        /// <summary>
        /// Checks to see if a PKM is transferrable relative to in-game restrictions and <see cref="PKM.AltForm"/>.
        /// </summary>
        /// <param name="pk">PKM to convert</param>
        /// <param name="comment">Comment indicating why the <see cref="PKM"/> is not transferrable.</param>
        /// <returns>Indication if Not Transferrable</returns>
        private static bool IsNotTransferrable(PKM pk, out string comment)
        {
            switch (pk.Species)
            {
                default:
                    comment = null;
                    return false;
                case 025 when pk.AltForm != 0 && pk.Gen6: // Cosplay Pikachu
                case 172 when pk.AltForm != 0 && pk.Gen4: // Spiky Eared Pichu
                    comment = MsgPKMConvertFailForme;
                    return true;
            }
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> is compatible with the input <see cref="PKM"/>, and makes any necessary modifications to force compatibility.
        /// </summary>
        /// <remarks>Should only be used when forcing a backwards conversion to sanitize the PKM fields to the target format.
        /// If the PKM is compatible, some properties may be forced to sanitized values.</remarks>
        /// <param name="pk">PKM input that is to be sanity checked.</param>
        /// <returns>Indication whether or not the PKM is compatible.</returns>
        public static bool IsPKMCompatibleWithModifications(PKM pk) => IsPKMCompatibleWithModifications(pk, pk);

        public static bool IsPKMCompatibleWithModifications(PKM pk, IGameValueLimit limit)
        {
            if (pk.Species > limit.MaxSpeciesID)
                return false;

            if (pk.HeldItem > limit.MaxItemID)
                pk.HeldItem = 0;

            if (pk.Nickname.Length > limit.NickLength)
                pk.Nickname = pk.Nickname.Substring(0, pk.NickLength);

            if (pk.OT_Name.Length > limit.OTLength)
                pk.OT_Name = pk.OT_Name.Substring(0, pk.OTLength);

            if (pk.Moves.Any(move => move > limit.MaxMoveID))
                pk.ClearInvalidMoves();

            if (pk.EVs.Any(ev => ev > limit.MaxEV))
                pk.EVs = pk.EVs.Select(ev => Math.Min(limit.MaxEV, ev)).ToArray();

            if (pk.IVs.Any(iv => iv > limit.MaxIV))
                pk.IVs = pk.IVs.Select(iv => Math.Min(limit.MaxIV, iv)).ToArray();

            return true;
        }

        /// <summary>
        /// Checks if a PKM is encrypted; if encrypted, decrypts the PKM.
        /// </summary>
        /// <remarks>The input PKM object is decrypted; no new object is returned.</remarks>
        /// <param name="pkm">PKM to check encryption for (and decrypt if appropriate).</param>
        /// <param name="format">Format specific check selection</param>
        public static void CheckEncrypted(ref byte[] pkm, int format)
        {
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
                        pkm = PKX.DecryptArray3(pkm);
                    return;
                case 4:
                case 5:
                    if (BitConverter.ToUInt16(pkm, 4) != 0) // BK4
                        return;
                    if (BitConverter.ToUInt32(pkm, 0x64) != 0)
                        pkm = PKX.DecryptArray45(pkm);
                    return;
                case 6:
                case 7:
                    if (BitConverter.ToUInt16(pkm, 0xC8) != 0 && BitConverter.ToUInt16(pkm, 0x58) != 0)
                        pkm = PKX.DecryptArray(pkm);
                    return;
                default:
                    return; // bad!
            }
        }

        /// <summary>
        /// Checks if the input <see cref="PKM"/> is compatible with the target <see cref="PKM"/>.
        /// </summary>
        /// <param name="pk">Input to check -> update/sanitize</param>
        /// <param name="target">Target type PKM with misc properties accessible for checking.</param>
        /// <param name="c">Comment output</param>
        /// <param name="pkm">Output compatible PKM</param>
        /// <returns>Indication if the input is (now) compatible with the target.</returns>
        public static bool TryMakePKMCompatible(PKM pk, PKM target, out string c, out PKM pkm)
        {
            if (!IsConvertibleToFormat(pk, target.Format))
            {
                pkm = null;
                c = string.Format(MsgPKMConvertFailBackwards, pk.GetType().Name, target.Format);
                if (!AllowIncompatibleConversion)
                    return false;
            }
            if (IsIncompatibleGB(target.Format, target.Japanese, pk.Japanese))
            {
                pkm = null;
                c = GetIncompatibleGBMessage(pk, target.Japanese);
                return false;
            }
            pkm = ConvertToType(pk, target.GetType(), out c);
            Debug.WriteLine(c);
            return pkm != null;
        }

        public static string GetIncompatibleGBMessage(PKM pk, bool destJP)
        {
            var src = destJP ? MsgPKMConvertInternational : MsgPKMConvertJapanese;
            var dest = !destJP ? MsgPKMConvertInternational : MsgPKMConvertJapanese;
            return string.Format(MsgPKMConvertIncompatible, src, pk.GetType().Name, dest);
        }

        public static bool IsIncompatibleGB(int format, bool destJP, bool srcJP) => format <= 2 && destJP != srcJP;

        /// <summary>
        /// Gets a Blank <see cref="PKM"/> object of the specified type.
        /// </summary>
        /// <param name="t">Type of <see cref="PKM"/> instance desired.</param>
        /// <returns>New instance of a blank <see cref="PKM"/> object.</returns>
        public static PKM GetBlank(Type t)
        {
            var constructors = t.GetTypeInfo().DeclaredConstructors.Where(z => !z.IsStatic);
            var argCount = constructors.First().GetParameters().Length;
            return (PKM)Activator.CreateInstance(t, new object[argCount]);
        }

        public static PKM GetBlank(int gen)
        {
            var type = Type.GetType($"PKHeX.Core.PK{gen}");
            return GetBlank(type);
        }
    }
}
