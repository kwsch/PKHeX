using FluentAssertions;
using Xunit;
using static PKHeX.Core.Species;

namespace PKHeX.Core.Tests;

public class SpeciesConverterTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1, Rhydon)]
    [InlineData(14, Gengar)]
    [InlineData(176, Charmander)]
    public void GetNational1(byte raw, Species national = 0) => ((Species)SpeciesConverter.GetNational1(raw)).Should().Be(national);

    [Theory]
    [InlineData(0)]
    [InlineData(1, Rhydon)]
    [InlineData(14, Gengar)]
    [InlineData(176, Charmander)]
    public void GetInternal1(byte raw, Species national = 0) => SpeciesConverter.GetInternal1((byte)national).Should().Be(raw);

    [Theory]
    [InlineData(0)]
    [InlineData(411, Chimecho)]
    [InlineData(407, Latias)]
    [InlineData(355, Mawile)]
    public void GetNational3(ushort raw, Species national = 0) => ((Species)SpeciesConverter.GetNational3(raw)).Should().Be(national);

    [Theory]
    [InlineData(0)]
    [InlineData(411, Chimecho)]
    [InlineData(407, Latias)]
    [InlineData(355, Mawile)]
    public void GetInternal3(ushort raw, Species national = 0) => SpeciesConverter.GetInternal3((ushort)national).Should().Be(raw);

    [Theory]
    [InlineData(0)]
    [InlineData(934, Palafin)]
    [InlineData(980, WalkingWake)]
    [InlineData(987, IronLeaves)]
    public void GetNational9(ushort raw, Species national = 0) => ((Species)SpeciesConverter.GetNational9(raw)).Should().Be(national);

    [Theory]
    [InlineData(0)]
    [InlineData(934, Palafin)]
    [InlineData(980, WalkingWake)]
    [InlineData(987, IronLeaves)]
    public void GetInternal9(ushort raw, Species national = 0) => SpeciesConverter.GetInternal9((ushort)national).Should().Be(raw);
}
