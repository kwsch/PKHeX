using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

/// <summary>
/// Verifies miscellaneous data including <see cref="IFatefulEncounter.FatefulEncounter"/> and minor values.
/// </summary>
public sealed class MiscVerifier : Verifier
{
    // This class has become bloated with various checks that don't fit anywhere else, but are still important to verify.
    // To keep it organized, the actual checks are delegated to various helper classes based on category.
    // This class just orchestrates which ones to call based on the PKM's properties and encounter context.
    private static readonly EggVerifier Eggs = new();
    private static readonly MiscEncounterDetailsVerifier EncounterDetails = new();
    private static readonly MiscFatefulEncounterVerifier FatefulEncounter = new();
    private static readonly MiscScaleVerifier ScaleValues = new();
    private static readonly MiscPokerusVerifier Pokerus = new();
    private static readonly MiscDateVerifier DateValues = new();
    private static readonly MiscG1Verifier Gen1 = new();
    private static readonly MiscEvolutionVerifier Evolution = new();
    private static readonly MiscVerifierSK2 Stadium2 = new();
    private static readonly MiscVerifierG3 Gen3 = new();
    private static readonly MiscVerifierG4 Gen4 = new();
    private static readonly MiscVerifierPK6 Gen6 = new();
    private static readonly MiscVerifierPK5 Gen5 = new();
    private static readonly MiscVerifierPK7 Gen7 = new();
    private static readonly MiscVerifierPB7 Gen7b = new();
    private static readonly MiscVerifierPK8 Gen8 = new();
    private static readonly MiscVerifierPA8 Gen8a = new();
    private static readonly MiscVerifierPB8 Gen8b = new();
    private static readonly MiscVerifierPK9 Gen9 = new();
    private static readonly MiscVerifierPA9 Gen9a = new();

    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
            Eggs.Verify(data, pk);

        // Verify gimmick data
        switch (pk)
        {
            case G3PKM pk3: Gen3.Verify(data, pk3); break;
            case G4PKM pk4: Gen4.Verify(data, pk4); break;
            case PK5 pk5: Gen5.Verify(data, pk5); break;
            case PK6 pk6: Gen6.Verify(data, pk6); break;
            case PK7 pk7: Gen7.Verify(data, pk7); break;
            case PB7 pb7: Gen7b.Verify(data, pb7); break;
            case PK8 pk8: Gen8.Verify(data, pk8); break;
            case PB8 pb8: Gen8b.Verify(data, pb8); break;
            case PA8 pa8: Gen8a.Verify(data, pa8); break;
            case PK9 pk9: Gen9.Verify(data, pk9); break;
            case PA9 pa9: Gen9a.Verify(data, pa9); break;
        }

        var enc = data.EncounterMatch;
        EncounterDetails.Verify(data, pk, enc);
        FatefulEncounter.Verify(data, pk, enc);
        ScaleValues.Verify(data, pk, enc);

        Pokerus.Verify(data, pk);
        DateValues.Verify(data, pk);
        Evolution.Verify(data, pk);
    }

    // PK1/2 only needs to check this; MiscVerifier is un-called otherwise.
    public void VerifyMiscG12(LegalityAnalysis data)
    {
        var pk = data.Entity;
        Gen1.VerifyG1(data, pk);
        if (pk is SK2 sk2)
            Stadium2.Verify(data, sk2);
        Pokerus.Verify(data, pk);
    }
}
