using static PKHeX.Core.InstructionOperation;

namespace PKHeX.Core;

/// <summary>
/// Operation type for applying a modification.
/// </summary>
public enum InstructionOperation : byte
{
    Set,
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    BitwiseAnd,
    BitwiseOr,
    BitwiseXor,
    BitwiseShiftRight,
    BitwiseShiftLeft,
}

public static class InstructionOperationExtensions
{
    extension(InstructionOperation operation)
    {
        public bool IsBitwise => operation switch
        {
            BitwiseAnd => true,
            BitwiseOr => true,
            BitwiseXor => true,
            BitwiseShiftRight => true,
            BitwiseShiftLeft => true,
            _ => false,
        };
    }
}
