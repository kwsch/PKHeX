using System;
using System.Diagnostics;
using static PKHeX.Core.EntityConverterResult;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="PKM"/> from one generation specific format to another.
/// </summary>
public static class EntityConverter
{
    /// <summary>
    /// If a conversion method does not officially (legally) exist, then the program can try to convert via other means (illegal).
    /// </summary>
    public static EntityCompatibilitySetting AllowIncompatibleConversion { get; set; }

    /// <summary>
    /// Toggles rejuvenating lost data if direct transfer does not know how to revert fields like Met Location and Ball.
    /// </summary>
    public static EntityRejuvenationSetting RejuvenateHOME { get; set; } = EntityRejuvenationSetting.MissingDataHOME;

    /// <summary>
    /// Post-conversion rejuvenation worker to restore lost values.
    /// </summary>
    public static IEntityRejuvenator RejuvenatorHOME { get; set; } = new LegalityRejuvenator();

    /// <summary>
    /// Responsible for converting a <see cref="PKM"/> to a <see cref="PKH"/> for HOME.
    /// </summary>
    public static IHomeStorage HOME { get; set; } = new HomeStorageFacade();

    private static GameVersion _vc1 = RD;
    private static GameVersion _vc2 = SI;

    /// <summary>
    /// Default source game for trading from PK2 to PK7.
    /// </summary>
    public static GameVersion VirtualConsoleSourceGen1
    {
        get => _vc1;
        set => _vc1 = (value is RD or BU or GN or YW) ? value : RD;
    }

    /// <summary>
    /// Default source game for trading from PK2 to PK7.
    /// </summary>
    public static GameVersion VirtualConsoleSourceGen2
    {
        get => _vc2;
        set => _vc2 = (value is GD or SI or C) ? value : SI;
    }

    /// <summary>
    /// Retain the Met Date when transferring from Gen4 to Gen5.
    /// </summary>
    /// <remarks>Default behavior is to update it with a new date based on the time of transfer.</remarks>
    public static bool RetainMetDateTransfer45 { get; set; }

    /// <summary>
    /// Checks if the input <see cref="PKM"/> file is capable of being converted to the desired format.
    /// </summary>
    /// <param name="pk"></param>
    /// <param name="format"></param>
    /// <returns>True if it can be converted to the requested format value.</returns>
    public static bool IsConvertibleToFormat(PKM pk, byte format)
    {
        if (pk.Format >= 3 && pk.Format > format && format < 8)
            return false; // pk3->upward can't go backwards until Gen8+
        if (pk.Format <= 2 && format is > 2 and < 7)
            return false; // pk1/2->upward has to be 7 or greater
        return true;
    }

    /// <summary>
    /// Converts a PKM from one Generation format to another. If it matches the destination format, the conversion will automatically return.
    /// </summary>
    /// <param name="pk">PKM to convert</param>
    /// <param name="destType">Format/Type to convert to</param>
    /// <param name="result">Comments regarding the transfer's success/failure</param>
    /// <returns>Converted PKM</returns>
    public static PKM? ConvertToType(PKM pk, Type destType, out EntityConverterResult result)
    {
        Type fromType = pk.GetType();
        if (fromType == destType)
        {
            result = None;
            return pk;
        }

        var entity = ConvertPKM(pk, destType, fromType, out result);
        if (entity is not null)
        {
            if (RejuvenateHOME != EntityRejuvenationSetting.None)
                RejuvenatorHOME.Rejuvenate(entity, pk);
            return entity;
        }

        if (AllowIncompatibleConversion != EntityCompatibilitySetting.AllowIncompatibleAll)
        {
            if (result is not NoTransferRoute)
                return null;
            if (AllowIncompatibleConversion != EntityCompatibilitySetting.AllowIncompatibleSane)
                return null;
        }

        // Try Incompatible Conversion
        entity = EntityBlank.GetBlank(destType);
        pk.TransferPropertiesWithReflection(entity);
        if (!IsCompatibleWithModifications(entity))
            return null; // NoTransferRoute
        result = SuccessIncompatibleReflection;
        return entity;
    }

    private static PKM? ConvertPKM(PKM pk, Type destType, Type srcType, out EntityConverterResult result)
    {
        result = CheckTransferOutbound(pk);
        if (result != Success)
            return null;
        result = CheckTransferInbound(pk, destType);
        if (result != Success)
            return null;

        Debug.WriteLine($"Trying to convert {srcType.Name} to {destType.Name}.");

        // All types that inherit PKM have the generation specifier as the last char in their class name.
        return ConvertPKM(pk, destType, ref result);
    }

    private static PKM? ConvertPKM(PKM pk, Type destType, ref EntityConverterResult result)
    {
        PKM? entity = pk.Clone();
        if (entity.IsEgg)
            entity.ForceHatchPKM();
        while (true)
        {
            entity = IntermediaryConvert(entity, destType, ref result);
            if (entity == null) // fail convert
                return null;
            if (entity.GetType() == destType) // finish convert
                return entity;
        }
    }

    private static PKM? IntermediaryConvert(PKM pk, Type destType, ref EntityConverterResult result) => pk switch
    {
        // Non-sequential
        PK1 pk1 when destType.Name[^1] - '0' is not (1 or 2) => pk1.ConvertToPK7(),
        PK2 pk2 when destType.Name[^1] - '0' is not (1 or 2) => pk2.ConvertToPK7(),
        PK2 pk2 when destType == typeof(SK2) => pk2.ConvertToSK2(),
        PK3 pk3 when destType == typeof(CK3) => pk3.ConvertToCK3(),
        PK3 pk3 when destType == typeof(XK3) => pk3.ConvertToXK3(),
        PK4 pk4 when destType == typeof(BK4) => pk4.ConvertToBK4(),
        PK4 pk4 when destType == typeof(RK4) => pk4.ConvertToRK4(),

        // Sequential
        PK1 pk1 => pk1.ConvertToPK2(),
        PK2 pk2 => pk2.ConvertToPK1(),
        PK3 pk3 => pk3.ConvertToPK4(),
        PK4 pk4 => pk4.ConvertToPK5(),
        PK5 pk5 => pk5.ConvertToPK6(),
        PK6 pk6 => pk6.ConvertToPK7(),

        // Side-Formats back to Mainline
        SK2 sk2 => sk2.ConvertToPK2(),
        CK3 ck3 => ck3.ConvertToPK3(),
        XK3 xk3 => xk3.ConvertToPK3(),
        BK4 bk4 => bk4.ConvertToPK4(),
        RK4 rk4 => rk4.ConvertToPK4(),

        _ => GetFinalResult(pk, destType, ref result),
    };

    private static PKM? GetFinalResult(PKM pk, Type destType, ref EntityConverterResult result)
    {
        // Every format can eventually feed into HOME. Don't bother checking current type.
        var type = PKH.GetType(destType);
        if (type is not HomeGameDataFormat.None)
        {
            var pkh = HOME.GetEntity(pk);
            var converted = pkh.ConvertToPKM(type);
            if (converted is null)
                result = IncompatibleSpecies;
            return converted;
        }

        result = NoTransferRoute;
        return null;
    }

    /// <summary>
    /// Checks to see if a PKM is transferable out of a specific format.
    /// </summary>
    /// <param name="pk">PKM to convert</param>
    /// <returns>Indication if Not Transferable</returns>
    private static EntityConverterResult CheckTransferOutbound(PKM pk) => pk switch
    {
        PK4 { Species: (int)Species.Pichu, Form: not 0 } => IncompatibleForm,
        PK6 { Species: (int)Species.Pikachu, Form: not 0 } => IncompatibleForm,
        PB7 { Species: (int)Species.Pikachu, Form: not 0 } => IncompatibleForm,
        PB7 { Species: (int)Species.Eevee, Form: not 0 } => IncompatibleForm,
        PB8 { Species: (int)Species.Spinda } => IncompatibleSpecies, // Incorrect arrangement of spots (PID endianness)
        PB8 { Species: (int)Species.Nincada } => IncompatibleSpecies, // Clone paranoia with Shedinja
        PK9 { Species: (int)Species.Koraidon or (int)Species.Miraidon, FormArgument: not 0 } => IncompatibleForm, // Ride
        _ => Success,
    };

    /// <summary>
    /// Checks to see if a PKM is transferable into a specific format.
    /// </summary>
    /// <param name="pk">PKM to convert</param>
    /// <param name="destType">Type to convert to</param>
    /// <returns>Indication if Not Transferable</returns>
    private static EntityConverterResult CheckTransferInbound(PKM pk, Type destType)
    {
        if (destType == typeof(PB8))
        {
            return pk.Species switch
            {
                (int)Species.Nincada => IncompatibleSpecies,
                (int)Species.Spinda => IncompatibleSpecies,
                _ => Success,
            };
        }

        if (destType.Name.EndsWith('1') && pk.Species > Legal.MaxSpeciesID_1)
            return IncompatibleSpecies;

        return Success;
    }

    /// <summary>
    /// Checks if the <see cref="PKM"/> is compatible with the input <see cref="PKM"/>, and makes any necessary modifications to force compatibility.
    /// </summary>
    /// <remarks>Should only be used when forcing a backwards conversion to sanitize the PKM fields to the target format.
    /// If the PKM is compatible, some properties may be forced to sanitized values.</remarks>
    /// <param name="pk">PKM input that is to be sanity checked.</param>
    /// <param name="limit">Value clamps for the destination format</param>
    /// <returns>Indication whether the PKM is compatible.</returns>
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

        if (pk.Nickname.Length > limit.MaxStringLengthNickname)
            pk.Nickname = pk.Nickname[..pk.MaxStringLengthNickname];

        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        var max = limit.MaxStringLengthTrainer;
        if (len > max)
            pk.SetString(pk.OriginalTrainerTrash, trainer[..max], max, StringConverterOption.None);

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
    /// <param name="result">Comment output</param>
    /// <param name="converted">Output compatible PKM</param>
    /// <returns>Indication if the input is (now) compatible with the target.</returns>
    public static bool TryMakePKMCompatible(PKM pk, PKM target, out EntityConverterResult result, out PKM converted)
    {
        if (!IsConvertibleToFormat(pk, target.Format))
        {
            converted = target;
            if (AllowIncompatibleConversion == EntityCompatibilitySetting.DisallowIncompatible)
            {
                result = NoTransferRoute;
                return false;
            }
        }
        if (!IsCompatibleGB(target, target.Japanese, pk.Japanese))
        {
            converted = target;
            result = IncompatibleLanguageGB;
            return false;
        }
        var convert = ConvertToType(pk, target.GetType(), out result);
        if (convert == null)
        {
            converted = target;
            return false;
        }

        converted = convert;
        return true;
    }

    /// <summary>
    /// Checks if a <see cref="GBPKM"/> is incompatible with the Generation 1/2 destination environment.
    /// </summary>
    public static bool IsCompatibleGB(PKM pk, bool destJapanese, bool srcJapanese)
    {
        if (pk.Format > 2)
            return true;
        if (destJapanese == srcJapanese)
            return true;
        if (pk is SK2 sk2 && sk2.IsPossible(srcJapanese))
            return true;
        return false;
    }
}
