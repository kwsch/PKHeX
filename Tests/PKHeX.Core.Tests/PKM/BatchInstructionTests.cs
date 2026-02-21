using FluentAssertions;
using Xunit;
namespace PKHeX.Core.Tests;

public class BatchInstructionTests
{
    private const string TestInstructions = """
        =Species=4
        .Form=0
        .Nickname=Hello
        """;

    [Theory]
    [InlineData(TestInstructions)]
    public void ParseCount(string lines)
    {
        var len = StringInstructionSet.GetInstructionSetLength(lines);
        len.Should().Be(lines.Length);
        lines += "\n;\n.Species=0";
        var extra = StringInstructionSet.GetInstructionSetLength(lines);
        (len + 1).Should().Be(extra);
    }

    [Theory]
    [InlineData('=')]
    [InlineData('!')]
    [InlineData('>')]
    [InlineData('<')]
    [InlineData('≥')]
    [InlineData('≤')]
    [InlineData('f', false)]
    public void ParseComparer(char c, bool expect = true)
    {
        var comparer = StringInstruction.GetComparer(c);
        comparer.IsSupported.Should().Be(expect);
    }

    [Theory]
    [InlineData("+EXP=100", InstructionOperation.Add, 250u, 350u)]
    [InlineData("-EXP=100", InstructionOperation.Subtract, 250u, 150u)]
    [InlineData("*EXP=2", InstructionOperation.Multiply, 250u, 500u)]
    [InlineData("/EXP=100", InstructionOperation.Divide, 250u, 2u)]
    [InlineData("%EXP=100", InstructionOperation.Modulo, 250u, 50u)]
    [InlineData("&EXP=10", InstructionOperation.BitwiseAnd, 12u, 8u)]
    [InlineData("|EXP=10", InstructionOperation.BitwiseOr, 12u, 14u)]
    [InlineData("^EXP=10", InstructionOperation.BitwiseXor, 12u, 6u)]
    [InlineData("«EXP=2", InstructionOperation.BitwiseShiftLeft, 3u, 12u)]
    [InlineData("»EXP=2", InstructionOperation.BitwiseShiftRight, 12u, 3u)]
    public void ApplyNumericOperation(string instruction, InstructionOperation operation, uint initialValue, uint expectedValue)
    {
        StringInstruction.TryParseInstruction(instruction, out var entry).Should().BeTrue();
        entry.Should().NotBeNull();
        entry.Operation.Should().Be(operation);

        var pk = CreateTestPK7(initialValue);
        var modified = EntityBatchEditor.Instance.TryModifyIsSuccess(pk, [], [entry]);

        modified.Should().BeTrue();
        pk.EXP.Should().Be(expectedValue);
    }

    private static PK7 CreateTestPK7(uint exp)
    {
        var pk = new PK7
        {
            Species = 1,
            EXP = exp,
        };
        pk.RefreshChecksum();
        return pk;
    }

    [Fact]
    public void ProcessDelegateReturnsTrueWhenModified()
    {
        var pk = CreateTestPK7(100);
        var editor = new EntityBatchProcessor();

        bool modified = editor.Process(pk, [], [], static p =>
        {
            p.EXP = 200;
            return true;
        });

        modified.Should().BeTrue();
    }

    [Fact]
    public void ProcessDelegateUpdatesExpWhenModified()
    {
        var pk = CreateTestPK7(100);
        var editor = new EntityBatchProcessor();

        _ = editor.Process(pk, [], [], static p =>
        {
            p.EXP = 200;
            return true;
        });

        pk.EXP.Should().Be(200u);
    }

    [Fact]
    public void ProcessInstructionsAndDelegateUpdatesExp()
    {
        var pk = CreateTestPK7(100);
        var editor = new EntityBatchProcessor();

        _ = editor.Process(pk, [], [], static p =>
        {
            p.EXP = 200;
            return true;
        });

        pk.EXP.Should().Be(200u);
    }

    [Fact]
    public void ProcessInstructionsAndDelegateSkipsWhenDelegateReturnsFalse()
    {
        var pk = CreateTestPK7(100);
        var editor = new EntityBatchProcessor();
        StringInstruction.TryParseInstruction(".EXP=200", out var instruction).Should().BeTrue();
        instruction.Should().NotBeNull();

        bool modified = editor.Process(pk, [], [instruction], static _ => false);

        modified.Should().BeFalse();
    }

    [Fact]
    public void ProcessInstructionsAndDelegatePreservesExpWhenDelegateReturnsFalse()
    {
        var pk = CreateTestPK7(100);
        var editor = new EntityBatchProcessor();
        StringInstruction.TryParseInstruction(".EXP=200", out var instruction).Should().BeTrue();
        instruction.Should().NotBeNull();

        _ = editor.Process(pk, [], [instruction], static _ => false);

        pk.EXP.Should().Be(100u);
    }
}
