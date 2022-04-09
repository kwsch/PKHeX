using System;
using System.Diagnostics;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="PKM"/> from one generation specific format to another.
/// </summary>
public static class EntityConverter
{
    /// <summary>
    /// If a conversion method does not officially (legally) exist, then the program can try to convert via other means (illegal).
    /// </summary>
    public static bool AllowIncompatibleConversion { get; set; }

    /// <summary>
    /// Checks if the input <see cref="PKM"/> file is capable of being converted to the desired format.
    /// </summary>
    /// <param name="pk"></param>
    /// <param name="format"></param>
    /// <returns>True if can be converted to the requested format value.</returns>
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

        if (pk is PK8 && destType == typeof(PB8))
            return new PB8(pk.Data);
        if (pk is PB8 && destType == typeof(PK8))
            return new PK8(pk.Data);

        // Try Incompatible Conversion
        pkm = EntityBlank.GetBlank(destType);
        pk.TransferPropertiesWithReflection(pkm);
        if (!IsCompatibleWithModifications(pkm))
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
        int destGeneration = destName[^1] - '0';
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
    /// <param name="limit">Value clamps for the destination format</param>
    /// <returns>Indication whether or not the PKM is compatible.</returns>
    public static bool IsCompatibleWithModifications(PKM pk, IGameValueLimit limit)
    {
        if (pk.Species > limit.MaxSpeciesID)
            return false;

        MakeCompatible(pk, limit);
        return true;
    }

    private static void MakeCompatible(PKM pk, IGameValueLimit limit)
    {
        if (pk.HeldItem > limit.MaxItemID)
            pk.HeldItem = 0;

        if (pk.Nickname.Length > limit.NickLength)
            pk.Nickname = pk.Nickname[..pk.NickLength];

        if (pk.OT_Name.Length > limit.OTLength)
            pk.OT_Name = pk.OT_Name[..pk.OTLength];

        if (pk.Move1 > limit.MaxMoveID || pk.Move2 > limit.MaxMoveID || pk.Move3 > limit.MaxMoveID || pk.Move4 > limit.MaxMoveID)
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
    }

    /// <inheritdoc cref="IsCompatibleWithModifications(PKM, IGameValueLimit)"/>
    public static bool IsCompatibleWithModifications(PKM pk) => IsCompatibleWithModifications(pk, pk);

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

    /// <summary>
    /// Returns an error string to indicate that a <see cref="GBPKM"/> is incompatible.
    /// </summary>
    public static string GetIncompatibleGBMessage(PKM pk, bool destJapanese)
    {
        var src = destJapanese ? MsgPKMConvertInternational : MsgPKMConvertJapanese;
        var dest = !destJapanese ? MsgPKMConvertInternational : MsgPKMConvertJapanese;
        return string.Format(MsgPKMConvertIncompatible, src, pk.GetType().Name, dest);
    }

    /// <summary>
    /// Checks if a <see cref="GBPKM"/> is incompatible with the Generation 1/2 destination environment.
    /// </summary>
    public static bool IsIncompatibleGB(PKM pk, bool destJapanese, bool srcJapanese)
    {
        if (pk.Format > 2)
            return false;
        if (destJapanese == srcJapanese)
            return false;
        if (pk is SK2 sk2 && sk2.IsPossible(srcJapanese))
            return false;
        return true;
    }
}
