using System;
using static PKHeX.Core.EntityConverterResult;

namespace PKHeX.Core;

public static class EntityConverterResultExtensions
{
    extension(EntityConverterResult result)
    {
        public bool IsSilent => result is None or Success;
        public bool IsSuccess => result is Success or SuccessIncompatibleManual or SuccessIncompatibleReflection;

        public string GetDisplayString(PKM src, Type dest)
        {
            if (result == None)
                return "No need to convert, current format matches requested format.";

            var msg = result.IsSuccess ? MessageStrings.MsgPKMConvertSuccess : MessageStrings.MsgPKMConvertFailFormat;
            var srcName = src.GetType().Name;
            var destName = dest.Name;
            var formatted = string.Format(msg, srcName, destName);
            if (result is Success)
                return formatted;

            var comment = result.GetMessage(src, dest);
            return string.Concat(formatted, Environment.NewLine, comment);
        }

        private string GetMessage(PKM src, Type dest) => result switch
        {
            SuccessIncompatibleReflection => "Converted via reflection.",
            SuccessIncompatibleManual => "Converted manually -- similar data.",
            IncompatibleForm  => MessageStrings.MsgPKMConvertFailForm,
            NoTransferRoute => MessageStrings.MsgPKMConvertFailNoMethod,
            IncompatibleSpecies => string.Format(MessageStrings.MsgPKMConvertFailFormat, SpeciesName.GetSpeciesNameGeneration(src.Species, src.Language, src.Format), dest.Name),
            IncompatibleLanguageGB => result.GetIncompatibleGBMessage(src, !src.Japanese),
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };

        /// <summary>
        /// Returns an error string to indicate that a <see cref="GBPKM"/> is incompatible.
        /// </summary>
        public string GetIncompatibleGBMessage(PKM pk, bool destJapanese)
        {
            if (result is not IncompatibleLanguageGB)
                return string.Empty;
            var src = destJapanese ? MessageStrings.MsgPKMConvertInternational : MessageStrings.MsgPKMConvertJapanese;
            var dest = !destJapanese ? MessageStrings.MsgPKMConvertInternational : MessageStrings.MsgPKMConvertJapanese;
            return string.Format(MessageStrings.MsgPKMConvertIncompatible, src, pk.GetType().Name, dest);
        }
    }
}
