using PKHeX.Core;

namespace PKHeX.Application.UseCases;

/// <summary>
/// A decrypted entity file ready to be written to disk (e.g. for OS drag-out).
/// </summary>
public readonly record struct ExportedEntityFile(string FileName, byte[] Data);

/// <summary>
/// Produces a decrypted, byte-identical entity file (e.g. ".pk9") for a single <see cref="PKM"/>,
/// using the same naming convention as Core's box dump/export paths (<see cref="EntityFileNamer"/>
/// + <see cref="PKM.Extension"/>).
/// </summary>
public sealed class ExportEntityToFileUseCase
{
    public ExportedEntityFile? Execute(PKM pk)
    {
        if (pk.Species == 0)
            return null;

        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        var fileName = PathUtil.CleanFileName(pk.FileName);
        return new ExportedEntityFile(fileName, data);
    }
}
