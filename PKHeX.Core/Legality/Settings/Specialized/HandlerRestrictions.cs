using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class HandlerSettings
{
    [LocalizedDescription("Severity to flag a Legality Check if Pokémon's Current Handler does not match the expected value.")]
    public Severity CurrentHandlerMismatch { get; set; } = Severity.Invalid;

    [LocalizedDescription("Checks the save file data and Current Handler state to determine if the Pokémon's Current Handler does not match the expected value.")]
    public bool CheckActiveHandler { get; set; } = true;

    public HandlerRestrictions Restrictions { get; set; } = new();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class HandlerRestrictions
{
    public bool AllowHandleOTGen6 { get; set; }
    public bool AllowHandleOTGen7 { get; set; }
    public bool AllowHandleOTGen7b { get; set; }
    public bool AllowHandleOTGen8 { get; set; }
    public bool AllowHandleOTGen8a { get; set; }
    public bool AllowHandleOTGen8b { get; set; }
    public bool AllowHandleOTGen9 { get; set; }

    public void Disable() => SetAllTo(true);

    public void SetAllTo(bool value)
    {
        AllowHandleOTGen6 = value;
        AllowHandleOTGen7 = value;
        AllowHandleOTGen7b = value;
        AllowHandleOTGen8 = value;
        AllowHandleOTGen8a = value;
        AllowHandleOTGen8b = value;
        AllowHandleOTGen9 = value;
    }

    public bool GetCanOTHandle(EntityContext encContext) => encContext switch
    {
        EntityContext.Gen6 => AllowHandleOTGen6,
        EntityContext.Gen7 => AllowHandleOTGen7,
        EntityContext.Gen7b => AllowHandleOTGen7b,
        EntityContext.Gen8 => AllowHandleOTGen8,
        EntityContext.Gen8a => AllowHandleOTGen8a,
        EntityContext.Gen8b => AllowHandleOTGen8b,
        EntityContext.Gen9 => AllowHandleOTGen9,
        _ => false,
    };
}
