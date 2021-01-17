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
        public static void SetPrimaryTrainer(ITrainerInfo t)
        {
            Trainer = t;
            if (t is IRegionOrigin o)
                Trainer67 = o;
        }

        private static ITrainerInfo Trainer { get; set; } = new SimpleTrainerInfo();
        private static IRegionOrigin Trainer67 { get; set; } = new SimpleTrainerInfo(GameVersion.SN);
        public static string OT_Name => Trainer.OT;
        public static int OT_Gender => Trainer.Gender;
        public static int Language => Trainer.Language;
        public static int Format => Trainer.Generation;
        public static int Game => Trainer.Game;
        public static bool AllowIncompatibleConversion { private get; set; }

        public static void SetConsoleRegionData3DS(IRegionOrigin pkm)
        {
            var trainer = Trainer is IRegionOrigin r ? r : Trainer67;
            pkm.ConsoleRegion = trainer.ConsoleRegion;
            pkm.Country = trainer.Country;
            pkm.Region = trainer.Region;
        }

        public static void SetFirstCountryRegion(IGeoTrack pkm)
        {
            var trainer = Trainer is IRegionOrigin r ? r : Trainer67;
            pkm.Geo1_Country = trainer.Country;
            pkm.Geo1_Region = trainer.Region;
        }

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
                case PokeCrypto.SIZE_1JLIST or PokeCrypto.SIZE_1ULIST:
                    return 1;
                case PokeCrypto.SIZE_2JLIST or PokeCrypto.SIZE_2ULIST:
                case PokeCrypto.SIZE_2STADIUM:
                    return 2;
                case PokeCrypto.SIZE_3PARTY or PokeCrypto.SIZE_3STORED:
                case PokeCrypto.SIZE_3CSTORED:
                case PokeCrypto.SIZE_3XSTORED:
                    return 3;
                case PokeCrypto.SIZE_4PARTY or PokeCrypto.SIZE_4STORED:
                case PokeCrypto.SIZE_5PARTY:
                    if ((BitConverter.ToUInt16(data, 0x4) == 0) && (BitConverter.ToUInt16(data, 0x80) >= 0x3333 || data[0x5F] >= 0x10) && BitConverter.ToUInt16(data, 0x46) == 0) // PK5
                        return 5;
                    return 4;
                case PokeCrypto.SIZE_6STORED:
                    return 6;
                case PokeCrypto.SIZE_6PARTY: // collision with PGT, same size.
                    if (BitConverter.ToUInt16(data, 0x4) != 0) // Bad Sanity?
                        return -1;
                    if (BitConverter.ToUInt32(data, 0x06) == PokeCrypto.GetCHK(data, PokeCrypto.SIZE_6STORED))
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
                case PokeCrypto.SIZE_8PARTY or PokeCrypto.SIZE_8STORED:
                    return 8;

                default:
                    return -1;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="PKM"/> from the given data.
        /// </summary>
        /// <param name="data">Raw data of the Pokemon file.</param>
        /// <param name="prefer">Optional identifier for the preferred generation.  Usually the generation of the destination save file.</param>
        /// <returns>An instance of <see cref="PKM"/> created from the given <paramref name="data"/>, or null if <paramref name="data"/> is invalid.</returns>
        public static PKM? GetPKMfromBytes(byte[] data, int prefer = 7)
        {
            int format = GetPKMDataFormat(data);
            return format switch
            {
                1 => new PokeList1(data)[0],
                2 => data.Length != PokeCrypto.SIZE_2STADIUM ? new PokeList2(data)[0] : new SK2(data),
                3 => data.Length switch
                {
                    PokeCrypto.SIZE_3CSTORED => new CK3(data),
                    PokeCrypto.SIZE_3XSTORED => new XK3(data),
                    _ => new PK3(data)
                },
                4 => BitConverter.ToUInt16(data, 0x04) == 0 ? new PK4(data) : new BK4(data),
                5 => new PK5(data),
                6 => CheckPKMFormat7(new PK6(data), prefer),
                8 => new PK8(data),
                _ => null
            };
        }

        /// <summary>
        /// Checks if the input PK6 file is really a PK7, if so, updates the object.
        /// </summary>
        /// <param name="pk">PKM to check</param>
        /// <param name="prefer">Prefer a certain generation over another</param>
        /// <returns>Updated PKM if actually PK7</returns>
        private static G6PKM CheckPKMFormat7(PK6 pk, int prefer)
        {
            if (GameVersion.Gen7b.Contains(pk.Version))
                return new PB7(pk.Data);
            if (IsPK6FormatReallyPK7(pk, prefer))
                return new PK7(pk.Data);
            return pk;
        }

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

                if (!pk.Gen4) // can't have encounter type
                    return true;
                if (et > 24) // invalid gen4 EncounterType
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
            if (pk.Format <= 2 && format is > 2 and < 7)
                return false; // pk1/2->upward has to be 7 or greater
            return true;
        }

        /// <summary>
        /// Converts a PKM from one Generation format to another. If it matches the destination format, the conversion will automatically return.
        /// </summary>
        /// <param name="pk">PKM to convert</param>
        /// <param name="destType">Format/Type to convert to</param>
        /// <param name="comment">Comments regarding the transfer's success/failure</param>
        /// <returns>Converted PKM</returns>
        public static PKM? ConvertToType(PKM pk, Type destType, out string comment)
        {
            Type fromType = pk.GetType();
            if (fromType == destType)
            {
                comment = "No need to convert, current format matches requested format.";
                return pk;
            }

            var pkm = ConvertPKM(pk, destType, fromType, out comment);
            if (!AllowIncompatibleConversion || pkm != null)
                return pkm;

            // Try Incompatible Conversion
            pkm = GetBlank(destType);
            pk.TransferPropertiesWithReflection(pkm);
            if (!IsPKMCompatibleWithModifications(pkm))
                return null;
            comment = "Converted via reflection.";
            return pkm;
        }

        private static PKM? ConvertPKM(PKM pk, Type destType, Type srcType, out string comment)
        {
            if (IsNotTransferable(pk, out comment))
                return null;

            string destName = destType.Name;
            string srcName = srcType.Name;
            Debug.WriteLine($"Trying to convert {srcName} to {destName}.");

            // All types that inherit PKM have the generation specifier as the last char in their class name.
            int destGeneration = destName[destName.Length - 1] - '0';
            var pkm = ConvertPKM(pk, destType, destGeneration, ref comment);
            var msg = pkm == null ? MsgPKMConvertFailFormat : MsgPKMConvertSuccess;
            var formatted = string.Format(msg, srcName, destName);
            comment = comment.Length != 0 ? formatted : string.Concat(formatted, Environment.NewLine, comment);
            return pkm;
        }

        private static PKM? ConvertPKM(PKM pk, Type destType, int destGeneration, ref string comment)
        {
            PKM? pkm = pk.Clone();
            if (pkm.IsEgg)
                pkm.ForceHatchPKM();
            while (true)
            {
                pkm = IntermediaryConvert(pkm, destType, destGeneration, ref comment);
                if (pkm == null) // fail convert
                    return null;
                if (pkm.GetType() == destType) // finish convert
                    return pkm;
            }
        }

        private static PKM? IntermediaryConvert(PKM pk, Type destType, int destGeneration, ref string comment)
        {
            switch (pk)
            {
                // Non-sequential
                case PK1 pk1 when destGeneration > 2: return pk1.ConvertToPK7();
                case PK2 pk2 when destGeneration > 2: return pk2.ConvertToPK7();
                case PK2 pk2 when destType == typeof(SK2): return pk2.ConvertToSK2();
                case PK3 pk3 when destType == typeof(CK3): return pk3.ConvertToCK3();
                case PK3 pk3 when destType == typeof(XK3): return pk3.ConvertToXK3();
                case PK4 pk4 when destType == typeof(BK4): return pk4.ConvertToBK4();

                // Invalid
                case PK2 pk2 when pk.Species > Legal.MaxSpeciesID_1:
                    var lang = pk2.Japanese ? (int)LanguageID.Japanese : (int)LanguageID.English;
                    var name = SpeciesName.GetSpeciesName(pk2.Species, lang);
                    comment = string.Format(MsgPKMConvertFailFormat, name, destType.Name);
                    return null;

                // Sequential
                case PK1 pk1: return pk1.ConvertToPK2();
                case PK2 pk2: return pk2.ConvertToPK1();
                case PK3 pk3: return pk3.ConvertToPK4();
                case PK4 pk4: return pk4.ConvertToPK5();
                case PK5 pk5: return pk5.ConvertToPK6();
                case PK6 pk6: return pk6.ConvertToPK7();
                case PK7 pk7: return pk7.ConvertToPK8();
                case PB7 pb7: return pb7.ConvertToPK8();

                // Side-Formats back to Mainline
                case SK2 sk2: return sk2.ConvertToPK2();
                case CK3 ck3: return ck3.ConvertToPK3();
                case XK3 xk3: return xk3.ConvertToPK3();
                case BK4 bk4: return bk4.ConvertToPK4();

                // None
                default:
                    comment = MsgPKMConvertFailNoMethod;
                    return null;
            }
        }

        /// <summary>
        /// Checks to see if a PKM is transferable relative to in-game restrictions and <see cref="PKM.Form"/>.
        /// </summary>
        /// <param name="pk">PKM to convert</param>
        /// <param name="comment">Comment indicating why the <see cref="PKM"/> is not transferable.</param>
        /// <returns>Indication if Not Transferable</returns>
        private static bool IsNotTransferable(PKM pk, out string comment)
        {
            switch (pk)
            {
                case PK4 { Species: (int)Species.Pichu   } pk4   when pk4.Form != 0:
                case PK6 { Species: (int)Species.Pikachu } pk6   when pk6.Form != 0:
                case PB7 { Species: (int)Species.Pikachu } pika  when pika.Form != 0:
                case PB7 { Species: (int)Species.Eevee   } eevee when eevee.Form != 0:
                    comment = MsgPKMConvertFailForm;
                    return true;
                default:
                    comment = string.Empty;
                    return false;
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

            int maxEV = pk.MaxEV;
            for (int i = 0; i < 6; i++)
            {
                if (pk.GetEV(i) > maxEV)
                    pk.SetEV(i, maxEV);
            }

            int maxIV = pk.MaxIV;
            for (int i = 0; i < 6; i++)
            {
                if (pk.GetIV(i) > maxIV)
                    pk.SetIV(i, maxIV);
            }

            return true;
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
                pkm = target;
                c = string.Format(MsgPKMConvertFailBackwards, pk.GetType().Name, target.Format);
                if (!AllowIncompatibleConversion)
                    return false;
            }
            if (IsIncompatibleGB(target, target.Japanese, pk.Japanese))
            {
                pkm = target;
                c = GetIncompatibleGBMessage(pk, target.Japanese);
                return false;
            }
            var convert = ConvertToType(pk, target.GetType(), out c);
            if (convert == null)
            {
                pkm = target;
                return false;
            }

            pkm = convert;
            Debug.WriteLine(c);
            return true;
        }

        public static string GetIncompatibleGBMessage(PKM pk, bool destJapanese)
        {
            var src = destJapanese ? MsgPKMConvertInternational : MsgPKMConvertJapanese;
            var dest = !destJapanese ? MsgPKMConvertInternational : MsgPKMConvertJapanese;
            return string.Format(MsgPKMConvertIncompatible, src, pk.GetType().Name, dest);
        }

        public static bool IsIncompatibleGB(PKM pk, bool destJapanese, bool srcJapanese) => pk.Format <= 2 && destJapanese != srcJapanese && !(pk is SK2 sk2 && sk2.IsPossible(srcJapanese));

        /// <summary>
        /// Gets a Blank <see cref="PKM"/> object of the specified type.
        /// </summary>
        /// <param name="type">Type of <see cref="PKM"/> instance desired.</param>
        /// <returns>New instance of a blank <see cref="PKM"/> object.</returns>
        public static PKM GetBlank(Type type)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors.Where(z => !z.IsStatic);
            var argCount = constructors.Min(z => z.GetParameters().Length);
            return (PKM)Activator.CreateInstance(type, new object[argCount]);
        }

        public static PKM GetBlank(int gen, GameVersion ver) => gen switch
        {
            1 when ver == GameVersion.BU => new PK1(true),
            7 when GameVersion.Gen7b.Contains(ver) => new PB7(),
            _ => GetBlank(gen)
        };

        public static PKM GetBlank(int gen, int ver) => GetBlank(gen, (GameVersion) ver);

        public static PKM GetBlank(int gen)
        {
            var type = Type.GetType($"PKHeX.Core.PK{gen}");
            return GetBlank(type);
        }
    }
}
