using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core;

public sealed class SecretBase3PKM : ISpeciesForm
{
    public uint PID { get; set; }
    public ushort Move1 { get; set; }
    public ushort Move2 { get; set; }
    public ushort Move3 { get; set; }
    public ushort Move4 { get; set; }
    public ushort SpeciesInternal { get; set; }
    public ushort Species
    {
        get => SpeciesConverter.GetNational3(SpeciesInternal);
        set => SpeciesInternal = SpeciesConverter.GetInternal3(value);
    }
    public ushort HeldItem { get; set; }
    public ushort SpriteItem { get => ItemConverter.GetItemFuture3(HeldItem); set => HeldItem = ItemConverter.GetItemOld3(value); }
    public byte Level { get; set; }
    public byte EVAll { get; set; }

    private PersonalInfo3 PersonalInfo => PersonalTable.E.GetFormEntry(Species, Form);
    public byte Form => Species == (int)Core.Species.Unown ? EntityPID.GetUnownForm3(PID) : (byte)0;
    public byte Gender => EntityGender.GetFromPIDAndRatio(PID, PersonalInfo.Gender);

    public void GetMoves(Span<ushort> moves)
    {
        moves[3] = Move4;
        moves[2] = Move3;
        moves[1] = Move2;
        moves[0] = Move1;
    }

    public string Summary => GetSummary(GameInfo.Strings);

    private string GetSummary(IBasicStrings g)
    {
        var sb = new StringBuilder(128);
        return GetSummary(sb, g);
    }

    private string GetSummary(StringBuilder sb, IBasicStrings g)
    {
        sb.Append($"{Species:000} - {g.Species[Species]}");
        if (HeldItem != 0)
            sb.Append($" @ {g.Item[SpriteItem]}");
        sb.AppendLine();

        var moveNames = g.Move;
        Span<ushort> moves = stackalloc ushort[4];
        GetMoves(moves);
        AddMoves(sb, moves, moveNames);
        sb.AppendLine();

        sb.Append($"Level: {Level}, EVs: {EVAll}, PID: {PID}");
        return sb.ToString();
    }

    private static void AddMoves(StringBuilder sb, Span<ushort> moves, IReadOnlyList<string> moveNames)
    {
        sb.Append("Moves: ");
        var first = true;
        foreach (var move in moves)
        {
            if (move == 0)
                continue;
            if (!first)
                sb.Append(Moveset.DefaultSeparator);

            var moveName = move >= moveNames.Count ? move.ToString() : moveNames[move];
            sb.Append(moveName);
            first = false;
        }
    }
}
