namespace PKHeX.Core;

/// <summary>
/// Interface that exposes Shadow details for the object.
/// </summary>
/// <remarks>Used only for Colosseum/XD <see cref="PKM"/> that were shadow encounters.</remarks>
public interface IShadowCapture
{
    ushort ShadowID { get; set; }
    int Purification { get; set; }

    bool IsShadow { get; }
}
